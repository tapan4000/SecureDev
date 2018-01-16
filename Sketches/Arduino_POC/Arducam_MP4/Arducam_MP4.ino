#include <SoftwareSerial.h>
#include <Wire.h>
#include "memorysaver.h"
#include <ArduCAM.h>
#include <SPI.h>
#include <SD.h>
#include <TimeLib.h>

#define pic_num 30
#define MP4OFFSET 24
unsigned long current_atom_size = 0;
unsigned long jpeg_size = 0;
unsigned long current_date = 0;
const char zero_buf[4] = { 0x00, 0x00, 0x00, 0x00 };
const int mp4_header[MP4OFFSET] PROGMEM = {
	0x00, 0x00, 0x00, 0x18, 0x66, 0x74, 0x79, 0x70, 0x6d, 0x70, 0x34, 0x32, 0x00, 0x00, 0x00, 0x00,
	0x6d, 0x70, 0x34, 0x31, 0x69, 0x73, 0x6f, 0x6d
};

// set pin 10 as the slave select for the digital pot:
const int CS = 7;
const int SD_CARD_Pin = 4;
bool is_header = false;
int mode = 0;
int imgNameCounter = 520;
uint8_t start_capture = 0;
String imageCounterFileName = "imgCtr3.txt";
ArduCAM myCAM( OV2640, CS );

void setup() {
	// put your setup code here, to run once:
	uint8_t vid, pid;
	uint8_t temp;

	Wire.begin();
	Serial.begin(115200);
	Serial.println(F("Start Serial at 115200"));

	Serial.println(F("ACK CMD ArduCAM Start!"));

	pinMode(CS, OUTPUT);

	Serial.print(F("Initializing SD card"));

	if (!SD.begin(SD_CARD_Pin)) {
		Serial.println(F("SD Initialization failed"));
		return;
	}

	Serial.println(F("SD Initialization complete"));

	// Open a file and read the value for the last image name.
	File imageNameCounterStore = SD.open(imageCounterFileName);
	if (imageNameCounterStore) {
		while (imageNameCounterStore.available()) {
			String storedValue = imageNameCounterStore.readStringUntil('\n');
			Serial.println(F("Stored value:"));
			Serial.println(storedValue);
			imgNameCounter = storedValue.toInt();
		}

		imageNameCounterStore.close();
	}
	else {
		Serial.println(F("Image counter file open failed"));
	}

	// initialize SPI:
	SPI.begin();
	while (1) {
		//Check if the ArduCAM SPI bus is OK
		myCAM.write_reg(ARDUCHIP_TEST1, 0x55);
		temp = myCAM.read_reg(ARDUCHIP_TEST1);
		if (temp != 0x55) {
			Serial.println(F("ACK CMD SPI interface Error!"));
			delay(1000); continue;
		}
		else {
			Serial.println(F("ACK CMD SPI interface OK.")); break;
		}
	}

	while (1) {
		//Check if the camera module type is OV2640
		myCAM.wrSensorReg8_8(0xff, 0x01);
		myCAM.rdSensorReg8_8(OV2640_CHIPID_HIGH, &vid);
		myCAM.rdSensorReg8_8(OV2640_CHIPID_LOW, &pid);
		if ((vid != 0x26) && ((pid != 0x41) || (pid != 0x42))) {
			Serial.println(F("ACK CMD Can't find OV2640 module!"));
			delay(1000); continue;
		}
		else {
			Serial.println(F("ACK CMD OV2640 detected.")); break;
		}
	}

	//Change to JPEG capture mode and initialize the OV5642 module
	myCAM.set_format(JPEG);
	myCAM.InitCAM();
	myCAM.OV2640_set_JPEG_size(OV2640_320x240);
	myCAM.clear_fifo_flag();
}

void loop() {
  // put your main code here, to run repeatedly:
  uint8_t temp = 0xff, temp_last = 0;
  bool is_header = false;
  current_atom_size = 0;
  if (Serial.available())
  {
    temp = Serial.read();
	temp = 0x10;
    switch (temp)
    {
      case 0x10:
		  Video2SD();
      break;
    }
  }
}

String GetFileName(bool isVideoStream){
   // Construct the file name
  imgNameCounter = imgNameCounter+1;
  Serial.println(F("Opening img counter file. RAM:"));
  Serial.println(FreeRam());
  Serial.println(imageCounterFileName);
  File imageCounterStore = SD.open(imageCounterFileName, O_CREAT | O_WRITE | O_TRUNC);
  Serial.println(imgNameCounter);
  if(imageCounterStore){
    Serial.println(F("Image counter file found"));
    imageCounterStore.println(imgNameCounter);  
    imageCounterStore.close();
  }
  else{
    // Print a log to identify the cause for failure.
    Serial.println(F("Image counter file not found"));
    return "";
  }

  Serial.println(F("File name generated: "));
  String fileExtension = ".mp4";
  String str = imgNameCounter + fileExtension;
  Serial.println(str);
  return str;
}

void Video2SD() {
	File outFile;
	byte buf[256];
	static int i = 0;
	uint8_t temp = 0, temp_last = 0;
	uint16_t frame_cnt = 0;
	uint8_t remnant = 0;
	uint32_t length = 0;
	bool is_header = false;
	String str = GetFileName(true);
	unsigned short fileChunkLength[pic_num];

	unsigned long top_level_position = 0;
	unsigned long child_level1_position = 0;
	unsigned long child_level2_position = 0;
	unsigned long child_level3_position = 0;
	unsigned long movie_start_time = now();

	//Open the new file
	outFile = SD.open(str, O_WRITE | O_CREAT | O_TRUNC);
	if (!outFile)
	{
		Serial.println(F("Video File open failed"));
		while (1);
		return;
	}

	//Write MP4 Header including the ftyp atom header
	for (i = 0; i < MP4OFFSET; i++)
	{
		char ch = pgm_read_byte(&mp4_header[i]);
		buf[i] = ch;
	}
	outFile.write(buf, MP4OFFSET);

	// Write the mdata atom
	outFile.write(zero_buf, 4);
	outFile.write("mdat");

	//Write video data
	Serial.println(F("Recording video, please wait..."));

	for (frame_cnt = 0; frame_cnt < pic_num; frame_cnt++)
	{
#if defined (ESP8266)
		yield();
#endif
		temp_last = 0; temp = 0;

		//Flush the FIFO
		myCAM.flush_fifo();

		//Clear the capture done flag
		myCAM.clear_fifo_flag();

		//Start capture
		myCAM.start_capture();
		while (!myCAM.get_bit(ARDUCHIP_TRIG, CAP_DONE_MASK));
		length = myCAM.read_fifo_length();

		i = 0;
		jpeg_size = 0;
		myCAM.CS_LOW();
		myCAM.set_fifo_burst();
		while (length--)
		{
			temp_last = temp;
			temp = SPI.transfer(0x00);
			//Read JPEG data from FIFO
			if ((temp == 0xD9) && (temp_last == 0xFF)) //If find the end ,break while,
			{
				buf[i++] = temp;  //save the last  0XD9     
								  //Write the remain bytes in the buffer
				myCAM.CS_HIGH();
				int bytesWritten = outFile.write(buf, i);
				if (bytesWritten != i) {
					Serial.println(F("M"));
					Serial.println(i);
					Serial.println(bytesWritten);
				}
				is_header = false;
				jpeg_size += i;
				i = 0;
			}
			if (is_header == true)
			{
				//Write image data to buffer if not full
				if (i < 256)
					buf[i++] = temp;
				else
				{
					//Write 256 bytes image data to file
					myCAM.CS_HIGH();
					int bytesWritten = outFile.write(buf, 256);
					if (bytesWritten != i) {
						Serial.println(F("Mis"));
						Serial.println(bytesWritten);
					}
					i = 0;
					buf[i++] = temp;
					myCAM.CS_LOW();
					myCAM.set_fifo_burst();
					jpeg_size += 256;
				}
			}
			else if ((temp == 0xD8) & (temp_last == 0xFF))
			{
				is_header = true;
				buf[i++] = temp_last;
				buf[i++] = temp;
			}
		}
		remnant = (4 - (jpeg_size & 0x00000003)) & 0x00000003;// Take bit-wise and with 3 (0000 0011)
		jpeg_size = jpeg_size + remnant;
		current_atom_size = current_atom_size + jpeg_size;
		if (remnant > 0)
			outFile.write(zero_buf, remnant);
		
		// Set the frame chunk size to be written to the index. The index contains the offset beginning from the frame list starting position.
		fileChunkLength[frame_cnt] = jpeg_size;
		//Serial.println(F("chunk size:"));
		//Serial.println(fileChunkLength[frame_cnt]);
	}

	unsigned long movie_duration = now() - movie_start_time;
	movie_duration = movie_duration * 600;
	current_atom_size = current_atom_size + 8; // The 8 bytes are added for the mdata and atom size fields.

	// print the movie size
	top_level_position = outFile.position();
	outFile.seek(top_level_position - current_atom_size);
	Serial.println(F("Writing mdat size at position:"));
	Serial.println(outFile.position());
	Serial.println(current_atom_size);
	print_bigEndianSize(current_atom_size, outFile);
	outFile.seek(top_level_position);

	// ------------------------------------------- moov ----------------------------------------------------------
	// Write the movie atom
	outFile.write(zero_buf, 4); // Atom size
	outFile.write("moov");
	
	
	// Write the movie header atom
	Serial.println(F("Size printing"));
	print_bigEndianSize(110, outFile); // Movie header size
	outFile.write("mvhd"); 
	outFile.write(zero_buf, 4); // Version and flags
	print_currentDate(outFile); // Creation time
	print_currentDate(outFile); // Modification time
	print_bigEndianSize(600, outFile);  // Time scale

	Serial.println(F("Duration"));
	Serial.println(movie_duration);
	print_bigEndianSize(movie_duration, outFile); // Duration
	print_bigEndianSize(65536, outFile); // Preferred rate
	print_bigEndianSize(256, outFile); // Preferred volume
	outFile.write(zero_buf, 4); outFile.write(zero_buf, 4); outFile.write(zero_buf, 2); // Reserved --46 bytes till now
	print_matrix(outFile); // matrix
	outFile.write(zero_buf, 4); outFile.write(zero_buf, 4); // Preview time and preview duration
	outFile.write(zero_buf, 4); // Poster time
	outFile.write(zero_buf, 4); outFile.write(zero_buf, 4); // Selection time and selection duration
	outFile.write(zero_buf, 4); print_bigEndianSize(3, outFile); // Next track id

	// -------------------------------------------- Video trak --------------------------------------------------------
	// Set the child level 1 position as the position of the trak atom beginning.
	child_level1_position = outFile.position();

	// Write the trak atom
	outFile.write(zero_buf, 4); // Atom size
	outFile.write("trak");

	// Write the track header atom
	outFile.write(zero_buf, 3); outFile.write(0x5c); // Atom size
	outFile.write("tkhd");
	outFile.write(zero_buf, 3); outFile.write(0x01); // Version and flags.
	print_currentDate(outFile); // Creation time
	print_currentDate(outFile); // Modification time
	outFile.write(zero_buf, 3); outFile.write(0x01); // Track id.
	outFile.write(zero_buf, 4); // Reserved
	print_bigEndianSize(movie_duration, outFile); // Duration ------------------ Fill ----------------
	outFile.write(zero_buf, 4); outFile.write(zero_buf, 4); // Reserved
	outFile.write(zero_buf, 4); // Layer and alternate group
	outFile.write(zero_buf, 4); // volume and reserved
	print_matrix(outFile); // print matrix
	print_bigEndianSize(320, outFile); print_bigEndianSize(240, outFile); // track width and track height.

	// ------------------------------------------- Media -------------------------------------------------
	// Set the child level 2 position as the position of the media atom beginning.
	child_level2_position = outFile.position();

	// Write the media atom
	outFile.write(zero_buf, 4); // Atom size
	outFile.write("mdia");
	
	// Write the Media header atom
	outFile.write(zero_buf, 3); outFile.write(0x20); // Atom size as 20 bytes
	outFile.write("mdhd");
	outFile.write(zero_buf, 4); // Version and flags
	print_currentDate(outFile); // Creation time
	print_currentDate(outFile); // Modification time
	print_bigEndianSize(15000, outFile); // Time scale set as 15000 (need to check) -----------------
	print_bigEndianSize((movie_duration / 600) * 15000, outFile); // Duration -------------- Fill it ----------------
	outFile.write(0x15); outFile.write(0xc7); // Language. Set to english. --------- Need to revisit -----------
	outFile.write(zero_buf, 2); // Quality

	// Write the handler reference atom.
	print_bigEndianSize(41, outFile); // Atom size. Set to 41.
	outFile.write("hdlr");
	outFile.write(zero_buf, 4); // Component type
	outFile.write("vide"); // Component sub-type. Set as 'vide'
	outFile.write(zero_buf, 4); outFile.write(zero_buf, 4); outFile.write(zero_buf, 4); // Component manufacturer, component flags and component flags mask
	outFile.write("VideoHandler"); // Component name
	outFile.write(zero_buf, 1); // Ending value. Can be removed. --------- Try to remove and check. Also update size accordingly.----

	// Set the child level 3 position as the position of the media information atom.
	child_level3_position = outFile.position();

	// Write the media information atom
	outFile.write(zero_buf, 4); // Atom size
	outFile.write("minf");

	// Write the video media information header atom
	print_bigEndianSize(20, outFile); // Atom size
	outFile.write("vmhd");
	outFile.write(zero_buf, 3); outFile.write(0x01); // Version and Flags
	outFile.write(zero_buf, 4); outFile.write(zero_buf, 4); // Graphics mode and op color

	// Write the data information atom
	outFile.write(zero_buf, 3); outFile.write(0x24); // Atom size. Set to 36
	outFile.write("dinf");

	// Write the data reference atom
	outFile.write(zero_buf, 3); outFile.write(0x1c); // Atom size. Set to 28
	outFile.write("dref");
	outFile.write(zero_buf, 4); // Version and flags
	outFile.write(zero_buf, 3); outFile.write(0x01); // Number of entries

	// Write the data reference array element
	outFile.write(zero_buf, 3); outFile.write(0x0c); // Atom size. Set to 12
	outFile.write("url ");
	outFile.write(zero_buf, 3); outFile.write(0x01); // Version and flags

	// Write the sample table atom.
	print_bigEndianSize(202 + pic_num*8, outFile); // Atom size
	outFile.write("stbl");

	// Write the sample description atom.
	print_bigEndianSize(102, outFile); // Atom size
	outFile.write("stsd");
	outFile.write(zero_buf, 4); // Version and flags
	outFile.write(zero_buf, 3); outFile.write(0x01); // Number of entries.

	// Write the sample description entry
	print_bigEndianSize(86, outFile); // Atom size
	outFile.write("mp4v");
	outFile.write(zero_buf, 4); outFile.write(zero_buf, 3); outFile.write(0x01); // Reserved and data reference index
	outFile.write(zero_buf, 4); // Version and revision level.
	outFile.write(zero_buf, 4); outFile.write(zero_buf, 4); outFile.write(zero_buf, 4); // Vendor, temporal quality and spatial quality.
	outFile.write(0x01); outFile.write(0x40); outFile.write(zero_buf, 1); outFile.write(0xf0); // Width and height.
	print_bigEndianSize(4718592, outFile); print_bigEndianSize(4718592, outFile); // Horizontal resolution and vertical resolution
	outFile.write(zero_buf, 4); // Data size
	outFile.write(zero_buf, 1); outFile.write(0x01); // Frame count
	outFile.write(zero_buf, 4); // Compressor name
	outFile.write(zero_buf, 4); outFile.write(zero_buf, 4); outFile.write(zero_buf, 4); outFile.write(zero_buf, 4); outFile.write(zero_buf, 4); outFile.write(zero_buf, 4); outFile.write(zero_buf, 4);// 28 bytes of empty space ------ check what this means -------
	outFile.write(zero_buf, 1); outFile.write(0x18); // Depth
	outFile.write(0xff); outFile.write(0xff); // Color id. Set to -1 to use default color.

	// Add the esds atom if needed ------------ Check if it is needed ---------

	// Write the time to sample atom
	print_bigEndianSize(24, outFile); // Atom size
	outFile.write("stts");
	outFile.write(zero_buf, 4); // Version and flags.
	outFile.write(zero_buf, 3); outFile.write(0x01); // Number of entries
	print_bigEndianSize(pic_num, outFile); // Sample count
	print_bigEndianSize(4718592, outFile); // Sample duration --------- Fill in the entry -------------

	// Write the sample to chunk atom
	print_bigEndianSize(32, outFile); // Atom size
	outFile.write("stsc");
	outFile.write(zero_buf, 4); // Version and flags.
	outFile.write(zero_buf, 3); outFile.write(0x01); // Number of entries
	outFile.write(zero_buf, 3); outFile.write(0x01); // First chunk
	outFile.write(zero_buf, 3); outFile.write(0x01); // Number of entries
	print_bigEndianSize(pic_num, outFile); // Samples per chunk
	outFile.write(zero_buf, 3); outFile.write(0x01); // Sample description id

	// Write the sample size atom
	print_bigEndianSize(pic_num * 4 + 20, outFile); // Atom size
	outFile.write("stsz");
	outFile.write(zero_buf, 4); // Version and flags.
	outFile.write(zero_buf, 4); // Default sample size
	print_bigEndianSize(pic_num, outFile); // Number of entries
	for (int i = 0; i < pic_num; i++) {
		// Print the sample size table
		print_bigEndianSize(fileChunkLength[i], outFile);
	}

	// Write the chunk offset atom
	print_bigEndianSize(pic_num * 4 + 16, outFile); // Atom size
	outFile.write("stco");
	outFile.write(zero_buf, 4); // Version and flags.
	outFile.write(zero_buf, 3); outFile.write(0x01); // Number of entries
	int chunkOffset = 32;
	for (int i = 0; i < pic_num; i++) {
		// Print the sample size table
		print_bigEndianSize(chunkOffset, outFile);
		chunkOffset = chunkOffset + fileChunkLength[i];
	}
	
	// Write the size of the media information atom
	current_atom_size = outFile.position() - child_level3_position;
	outFile.seek(child_level3_position);
	print_bigEndianSize(current_atom_size, outFile);
	outFile.seek(child_level3_position + current_atom_size);

	// Write the size of the media atom
	current_atom_size = outFile.position() - child_level2_position;
	outFile.seek(child_level2_position);
	print_bigEndianSize(current_atom_size, outFile);
	outFile.seek(child_level2_position + current_atom_size);

	// Write the size of the track atom
	current_atom_size = outFile.position() - child_level1_position;
	outFile.seek(child_level1_position);
	print_bigEndianSize(current_atom_size, outFile);
	outFile.seek(child_level1_position + current_atom_size);

	// Write the size of the movie atom
	current_atom_size = outFile.position() - top_level_position;
	outFile.seek(top_level_position);
	print_bigEndianSize(current_atom_size, outFile);
	outFile.seek(top_level_position + current_atom_size);
	current_atom_size = 0;
	myCAM.CS_HIGH();
	//Close the file
	outFile.close();
	Serial.println(F("Record video OK."));
}

void print_bigEndianSize(unsigned long i, File fd) {
	fd.write(zero_buf, 4);
	Serial.println(i);
	//unsigned long fileSizeStartPosition = fd.position();
	//unsigned long sizePosition = fileSizeStartPosition + 3;
	fd.seek(fd.position() - 1);
	unsigned long val = i;
	//if (val == 108) {
	//	Serial.println(F("Print 108:"));
	//	//Serial.println(fileSizeStartPosition);
	//	//Serial.println(sizePosition);
	//	Serial.println(fd.position());
	//}

	/*if (val == 108) {
		Serial.println(i % 0x100);
		Serial.println(fd.position());
	}*/

	int lastByteValue = i % 0x100;
	int bytesWritten = fd.write(lastByteValue);  i = i >> 8;
	if (bytesWritten != 1) {
		Serial.println(F("Z1"));
	}

	//sizePosition = fileSizeStartPosition + 2;
	fd.seek(fd.position() - 2);

	//if (val == 108) {
	//	Serial.println(i % 0x100);
	//	//Serial.println(sizePosition);
	//	Serial.println(fd.position());
	//}
	bytesWritten = fd.write(i % 0x100);  i = i >> 8;
	if (bytesWritten != 1) {
		Serial.println(F("Z2"));
	}

	//sizePosition = fileSizeStartPosition + 1;
	fd.seek(fd.position() - 2);

	//if (val == 108) {
	//	Serial.println(i % 0x100);
	//	//Serial.println(sizePosition);
	//	Serial.println(fd.position());
	//}

	bytesWritten = fd.write(i % 0x100);  i = i >> 8;
	if (bytesWritten != 1) {
		Serial.println(F("Z3"));
	}

	fd.seek(fd.position() - 2);

	/*if (val == 108) {
		Serial.println(i % 0x100);
		Serial.println(fd.position());
		Serial.println(F("End 108"));
	}*/
	bytesWritten = fd.write(i % 0x100);  i = i >> 8;
	if (bytesWritten != 1) {
		Serial.println(F("Z4"));
	}

	// Place the cursor at the last size byte and print the last byte value, so that the cursor moves to next position.
	// We may not be able to move to the next position directly as the file size may end at the 4th byte and moving to 5th byte may not work.
	fd.seek(fd.position() + 2);
	bytesWritten = fd.write(lastByteValue);
	if (bytesWritten != 1) {
		Serial.println(F("Z5"));
	}
}

void print_currentDate(File fd) {
	Serial.println(F("Now:"));
	Serial.println(now());
	current_date = now() + 2082844800;
	print_bigEndianSize(current_date, fd); // Add the time (in seconds) between 1 Jan 1904 to 1 Jan 1970 as now() gives time since 1 Jan 1970.
}

void print_matrix(File fd) {

	// Print the matrix as [1 0 0] [0 1 0] [4 0 0]
	fd.write(zero_buf, 1); fd.write(0x01); fd.write(zero_buf, 2); fd.write(zero_buf, 4); fd.write(zero_buf, 4); // Matrix column 1
	fd.write(zero_buf, 4); fd.write(zero_buf, 1); fd.write(0x01); fd.write(zero_buf, 2); fd.write(zero_buf, 4); // Matrix column 2
	fd.write(zero_buf, 4); fd.write(zero_buf, 4); fd.write(0x40); fd.write(zero_buf, 3); // Matrix column 3
}
