#include <SoftwareSerial.h>

// Libraries related to ArduCam.
#include <Wire.h>
#include "memorysaver.h"
#include <ArduCAM.h>
#include <SPI.h>
#include <SD.h>

// Set the serial ports for the bluetooth module Receive and Transmit
//SoftwareSerial moduleSerial(0, 1); //RX, TX

// Determine if the 
#if !(defined OV2640_MINI_2MP)
#error Please select the hardware platform and camera module in the ../libraries/ArduCAM/memorysaver.h file
#endif

#define BMPIMAGEOFFSET 66
#define pic_num 50
#define rate 0x05
#define AVIOFFSET 240
unsigned long movi_size = 4; // Initialize this value with the 4 bytes of data post the LIST and LIST size FOURCC. These 4 bytes represent "movi" FOURCC
unsigned long jpeg_size = 0;
const char zero_buf[4] = { 0x00, 0x00, 0x00, 0x00 };
const int avi_header[AVIOFFSET] PROGMEM = {
	0x52, 0x49, 0x46, 0x46, 0xD8, 0x01, 0x0E, 0x00, 0x41, 0x56, 0x49, 0x20, 0x4C, 0x49, 0x53, 0x54,
	0xD0, 0x00, 0x00, 0x00, 0x68, 0x64, 0x72, 0x6C, 0x61, 0x76, 0x69, 0x68, 0x38, 0x00, 0x00, 0x00,
	0xA0, 0x86, 0x01, 0x00, 0x80, 0x66, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00,
	0x64, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
	0x40, 0x01, 0x00, 0x00, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x4C, 0x49, 0x53, 0x54, 0x84, 0x00, 0x00, 0x00,
	0x73, 0x74, 0x72, 0x6C, 0x73, 0x74, 0x72, 0x68, 0x30, 0x00, 0x00, 0x00, 0x76, 0x69, 0x64, 0x73,
	0x4D, 0x4A, 0x50, 0x47, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
	0x01, 0x00, 0x00, 0x00, rate, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x73, 0x74, 0x72, 0x66,
	0x28, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x40, 0x01, 0x00, 0x00, 0xF0, 0x00, 0x00, 0x00,
	0x01, 0x00, 0x18, 0x00, 0x4D, 0x4A, 0x50, 0x47, 0x00, 0x84, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x4C, 0x49, 0x53, 0x54,
	0x10, 0x00, 0x00, 0x00, 0x6F, 0x64, 0x6D, 0x6C, 0x64, 0x6D, 0x6C, 0x68, 0x04, 0x00, 0x00, 0x00,
	0x64, 0x00, 0x00, 0x00, 0x4C, 0x49, 0x53, 0x54, 0x00, 0x01, 0x0E, 0x00, 0x6D, 0x6F, 0x76, 0x69,
};

const char indexDwFlags[4] = {
	0x10, 0x00, 0x00, 0x00
};
/*const uint8_t bmp_header[BMPIMAGEOFFSET] PROGMEM =
{
0x42, 0x4D, 0x36, 0x58, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x42, 0x00, 0x00, 0x00, 0x28, 0x00,
0x00, 0x00, 0x40, 0x01, 0x00, 0x00, 0xF0, 0x00, 0x00, 0x00, 0x01, 0x00, 0x10, 0x00, 0x03, 0x00,
0x00, 0x00, 0x00, 0x58, 0x02, 0x00, 0xC4, 0x0E, 0x00, 0x00, 0xC4, 0x0E, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF8, 0x00, 0x00, 0xE0, 0x07, 0x00, 0x00, 0x1F, 0x00,
0x00, 0x00
};*/

// set pin 10 as the slave select for the digital pot:
const int CS = 7;
const int SD_CARD_Pin = 4;
bool is_header = false;
int mode = 0;
int isExecuted = 0;
int imgNameCounter = 520;
uint8_t start_capture = 0;
String imageCounterFileName = "imgCtr3.txt";
#if defined (OV2640_MINI_2MP)
ArduCAM myCAM(OV2640, CS);
#else
ArduCAM myCAM(OV5642, CS);
#endif
uint8_t read_fifo_burst(ArduCAM myCAM, String fileName, bool isVideoClip);

void setup() {
	// put your setup code here, to run once:
	uint8_t vid, pid;
	uint8_t temp;
#if defined(__SAM3X8E__) 
	//Wire1.begin();
	Serial.begin(115200);
	Serial.println(F("Start Serial at 115200"));
#else
	Wire.begin();
	Serial.begin(115200);
	Serial.println(F("Start Serial at 921600"));
#endif

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

#if defined (OV2640_MINI_2MP)
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
#else
	while (1) {
		//Check if the camera module type is OV5642
		myCAM.wrSensorReg16_8(0xff, 0x01);
		myCAM.rdSensorReg16_8(OV5642_CHIPID_HIGH, &vid);
		myCAM.rdSensorReg16_8(OV5642_CHIPID_LOW, &pid);
		if ((vid != 0x56) || (pid != 0x42)) {
			Serial.println(F("ACK CMD Can't find OV5642 module!"));
			delay(1000); continue;
		}
		else {
			Serial.println(F("ACK CMD OV5642 detected.")); break;
		}
	}
#endif

	//Change to JPEG capture mode and initialize the OV5642 module
	myCAM.set_format(JPEG);
	myCAM.InitCAM();
#if defined (OV2640_MINI_2MP)
	myCAM.OV2640_set_JPEG_size(OV2640_320x240);
#else
	myCAM.write_reg(ARDUCHIP_TIM, VSYNC_LEVEL_MASK);   //VSYNC is active HIGH
	myCAM.OV5642_set_JPEG_size(OV5642_320x240);
#endif

	myCAM.clear_fifo_flag();
#if !(defined (OV2640_MINI_2MP))
	myCAM.write_reg(ARDUCHIP_FRAMES, 0x00);
#endif
}

void loop()
{

  /* add main program code here */
	if (isExecuted == 0) {
		Serial.println("Starting execution");
		isExecuted = 1;
		String myFileName = "myt.txt";
		File outFile = SD.open(myFileName, O_CREAT | O_WRITE | O_TRUNC);
		if (outFile) {
			Serial.println("File opened");
			int counter = 0;
			int maxCount = 100000;
			while (counter < maxCount) {
				int bytesWritten = outFile.write(0x01);
				if (bytesWritten == 0) {
					Serial.println("Mis");
				}

				if (counter % 50000 == 0) {
					Serial.println("50000");
					Serial.println(counter);
				}

				if (counter > maxCount) {
					Serial.println("breaking");
					break;
				}
				counter++;
			}

			outFile.close();
		}
		else {
			Serial.println("File failed to open");
		}
	}
}

String GetFileName(bool isVideoStream) {
	delay(10000);
	// Construct the file name
	imgNameCounter = imgNameCounter + 1;
	Serial.println(F("Opening the image counter file"));
	Serial.println(FreeRam());
	delay(10000);
	Serial.println(imageCounterFileName);
	delay(10000);
	File imageCounterStore = SD.open(imageCounterFileName, O_CREAT | O_WRITE | O_TRUNC);
	Serial.println(imgNameCounter);
	//Serial.println("Opening the image counter file -op complete");
	if (imageCounterStore) {
		Serial.println(F("Image counter file found"));
		imageCounterStore.println(imgNameCounter);
		//Serial.println(F("Image counter value written"));
		imageCounterStore.close();
		//Serial.println(F("Image counter file closed"));
	}
	else {
		// Print a log to identify the cause for failure.
		Serial.println(F("Image counter file not found"));
		return "";
	}

	//delay(10000);
	//strcat(str, ".jpg");

	//Serial.println("File name generated");
	//Serial.println(str);

	String fileExtension = ".jpg";

	//Serial.println(F("Begin if check"));
	if (isVideoStream) {
		fileExtension = ".avi";
	}

	Serial.println(F("File name generated: "));
	String str = imgNameCounter + fileExtension;
	Serial.println(str);
	return str;
}


uint8_t read_fifo_burst(ArduCAM myCAM, String fileName, bool isVideoClip)
{
	//delay(30000);
	String str = "";
	byte buf[256];

	uint32_t i = 0;
	uint8_t temp = 0, temp_last = 0;
	uint32_t length = 0;

	length = myCAM.read_fifo_length();
	//Serial.println(length, DEC);

	//Serial.println(F("Beginning to read with length"));
	//Serial.println(length);
	//Serial.println(F("MAX size"));
	//Serial.println(MAX_FIFO_SIZE);

	if (length >= MAX_FIFO_SIZE) //512 kb
	{
		Serial.println(F("Over size."));

		if (isVideoClip) {
			myCAM.clear_fifo_flag();
			start_capture = 2;
		}

		return 0;
	}
	if (length == 0) //0 kb
	{
		Serial.println(F("Size is 0."));
		if (isVideoClip) {
			myCAM.clear_fifo_flag();
			start_capture = 2;
		}

		return 0;
	}

	/*
	// Construct the file name
	imgNameCounter = imgNameCounter+1;
	Serial.println(F("Opening the image counter file"));
	Serial.println(FreeRam());
	File imageCounterStore = SD.open(imageCounterFileName, O_CREAT | O_WRITE | O_TRUNC);
	Serial.println(imgNameCounter);
	//Serial.println("Opening the image counter file -op complete");
	if(imageCounterStore){
	Serial.println(F("Image counter file found"));
	imageCounterStore.println(imgNameCounter);
	//Serial.println("Image counter value written");
	imageCounterStore.close();
	}
	else{
	// Print a log to identify the cause for failure.
	Serial.println(F("Image counter file not found"));
	return 0;
	}

	//strcat(str, ".jpg");

	//Serial.println("File name generated");
	//Serial.println(str);

	String fileExtension = ".jpg";

	if(isVideoStream){
	fileExtension = ".avi";
	}

	str = imgNameCounter + fileExtension;
	*/
	str = fileName;

	//str = imgNameCounter + ".jpg";
	Serial.println(str);

	// Open the new file
	File outFile = SD.open(str, FILE_WRITE);

	Serial.println(F("Open requested"));
	if (!outFile)
	{
		Serial.println(F("File open failed"));
		return 0;
	}

	Serial.println(F("File opened successfully"));
	Serial.println(is_header);
	Serial.println(F("Begin write operation"));
	myCAM.CS_LOW();
	myCAM.set_fifo_burst();//Set fifo burst mode
	temp = SPI.transfer(0x00);
	uint32_t initialLength = length;
	length--;
	bool isFirst = true;
	uint32_t count = 0;
	uint8_t imageBytes[50];
	uint32_t imageByteCounter = 0;
	uint32_t totalByteCounter = 0;
	while (length--)
	{
		totalByteCounter++;
		temp_last = temp;
		temp = SPI.transfer(0x00);
		/*
		if(count < 100)
		{
		Serial.println(temp);
		}
		else if(count > initialLength - 5)
		{
		Serial.println("End char: ");
		Serial.println(temp);
		}*/

		if (isFirst) {
			//Serial.println("Received first");
			//Serial.println(temp_last);
			//Serial.println(temp);
			isFirst = false;
		}
		if (is_header == true)
		{
			/*Serial.println(temp);*/
			//imageFile.write(temp);
			//imageBytes[imageByteCounter] = temp;
			imageByteCounter++;
			//Serial.write(temp);
			if (i < 256) {
				buf[i++] = temp;
			}
			else {
				// Write 256 bytes image data to file
				//Serial.write(outFile.size());
				myCAM.CS_HIGH();

				outFile.write(buf, 256);

				i = 0;
				buf[i++] = temp;
				//Serial.println(F("Writing 256 bytes"));
				myCAM.CS_LOW();
				myCAM.set_fifo_burst();
			}
		}
		else if ((temp == 0xD8) & (temp_last == 0xFF)) // 216 and 255
		{
			is_header = true;
			Serial.println(F("ACK IMG"));
			//Serial.println(totalByteCounter);
			//Serial.println(i);
			//Serial.println(sizeof(buf));
			/*Serial.println(temp_last);*/
			/*Serial.println(temp);*/
			//imageFile.write(temp_last);
			//imageFile.write(temp);
			//Serial.write(temp_last);
			//Serial.write(temp);
			//imageBytes[imageByteCounter] = temp_last;
			imageByteCounter++;
			//imageBytes[imageByteCounter] = temp;
			//imageByteCounter++;
			buf[i++] = temp_last;
			buf[i++] = temp;
		}
		if ((temp == 0xD9) && (temp_last == 0xFF)) //If find the end ,break while, (217 and 255)
		{
			//if(!isVideoClip)
			{
				buf[i++] = temp; // save the last 0xD9

								 // Write the remaining bytes in the buffer
				myCAM.CS_HIGH();
				outFile.write(buf, i);

				// Close the file
				outFile.close();
			}

			Serial.println(F("Breaking the loop"));
			break;
		}
		/*
		if(imageByteCounter == 50){
		if(!imageFile){
		imageFile = SD.open("img.txt", FILE_WRITE);
		}

		imageFile.write(imageBytes, 50);
		imageFile.close();
		imageByteCounter = 0;
		}
		count++;
		delayMicroseconds(15);*/
	}

	/*
	Serial.println("Image byte counter length: ");
	Serial.println(imageByteCounter);
	Serial.println("Initial length: ");
	Serial.println(initialLength);
	Serial.println("Array values: ");
	Serial.println(imageBytes[0]);
	Serial.println(imageBytes[1]);
	Serial.println(imageBytes[2]);
	Serial.println(imageBytes[3]);*/

	//Serial.println("count: ");
	//Serial.println(count);
	//Serial.println("Checking for file closure");
	/*if(imageFile)
	{
	//Serial.println("Closing file");
	imageFile.close();
	//Serial.println("File closed");
	}*/

	myCAM.CS_HIGH();
	Serial.println(F("End function"));
	if (isVideoClip) {
		myCAM.clear_fifo_flag();
		start_capture = 2;
	}

	is_header = false;
	return 1;
}

void Video2SD() {
	/*
	240(header)-
	Begin for
	For_Each_Frame: 00dc (represents compressed video frame)-----zero_buffer(4 bytes)-----image_bytes-----remnant(fill with zero bytes)-----print_quartet(at current - 4 - jpeg_size print jpegsize%256)-----AVI1(current + 6)
	End for
	print_quartet(at byte 4 of file print movie_size+12*frame_cnt+4) - riff file size, overwrite hdrl
	print_quartet(at byte 32 of file print us(micros seconds) per frame (1000000/5(rate)))
	print_quartet(at byte 36 of file print max bytes per second (movie_size*5(rate)/frame_cnt))
	print_quartet(at byte 48 of file print total frames (frame_cnt)
	print_quartet(at byte 224 of file print frames (again frame_cnt)
	print_quartet(at byte 232 of file print movie size(movie_size)

	An AVI file consists of following chunks
	1) hdrl tag - file header contains metadata about the video (it's width, height, frame rate)
	2) movi tag - actual audio/video data
	3) idx1 tag - it indexes the offsets of the data chunk (movi tag) within the file

	Any RIFF container must have a signature tag (hex: 52, 49, 46, 46) at the beginning of the file.
	RIFF files are organized into data segments (chunks). Each segment is prefixed with a 12 byte header (4 byte signature (RIFF), 4 byte data size (little endian order, low byte first) and 4 byte RIFF type: signature AVI[space]. Chunk size is data bytes plus 8 bytes.

	For an index, we have idx1-4bytes(size of the index everything post the size to end)-00dc(id of the stream the index points to)-12bytes(specifies dwFlags, dwoffset and dwsize - refer "https://msdn.microsoft.com/en-us/library/windows/desktop/dd318181(v=vs.85).aspx")-00dc-12bytes-00dc..goes on---00dc-12bytes

	263564 + 36(at pos 207 two bytes differ and at pos 8 the bytes differ due to size difference) + 800
	264400
	00dc (c's actual offset 243, chunk length in index 4808) -- offset in index 4
	00dc (c's actual offset 5059, chunk length in index 4736) -- offset in index 4820
	00dc (c's actual offset 9803, chunk length in index 4984) -- offset in index 9564
	00dc (c's actual offset 14795, chunk length in index 4924) -- offset in index 14556
	The length of the chunk is actual length of chunk excluding 00dc minus 4.
	*/

	File outFile;
	byte buf[256];
	static int i = 0;
	uint8_t temp = 0, temp_last = 0;
	unsigned long position = 0;
	uint16_t frame_cnt = 0;
	uint8_t remnant = 0;
	uint32_t length = 0;
	bool is_header = false;
	String str = GetFileName(true);
	unsigned short fileChunkLength[pic_num];

	//Open the new file
	outFile = SD.open(str, O_WRITE | O_CREAT | O_TRUNC);
	if (!outFile)
	{
		Serial.println(F("Video File open failed"));
		while (1);
		return;
	}

	//Write AVI Header
	for (i = 0; i < AVIOFFSET; i++)
	{
		char ch = pgm_read_byte(&avi_header[i]);
		buf[i] = ch;
	}
	outFile.write(buf, AVIOFFSET);
	//Write video data
	Serial.println(F("Recording video, please wait..."));

	for (frame_cnt = 0; frame_cnt < pic_num; frame_cnt++)
	{
#if defined (ESP8266)
		yield();
#endif
		temp_last = 0; temp = 0;
		//Capture a frame            
		//Flush the FIFO
		myCAM.flush_fifo();
		//Clear the capture done flag
		myCAM.clear_fifo_flag();
		//Start capture
		myCAM.start_capture();
		while (!myCAM.get_bit(ARDUCHIP_TRIG, CAP_DONE_MASK));
		length = myCAM.read_fifo_length();

		outFile.write("00dc");
		outFile.write(zero_buf, 4);
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
				outFile.write(buf, i);
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
					outFile.write(buf, 256);
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
		movi_size = movi_size + jpeg_size + 8; // The 8 bytes are added for the 00dc and chunk size data.
		if (remnant > 0)
			outFile.write(zero_buf, remnant);
		position = outFile.position();
		outFile.seek(position - 4 - jpeg_size);
		print_quartet(jpeg_size, outFile);
		position = outFile.position();
		outFile.seek(position + 6);
		outFile.write("AVI1", 4);
		position = outFile.position();
		outFile.seek(position + jpeg_size - 10);

		// Set the frame chunk size to be written to the index. The index contains the offset beginning from the frame list starting position.
		fileChunkLength[frame_cnt] = jpeg_size;
		Serial.println(F("chunk size:"));
		Serial.println(fileChunkLength[frame_cnt]);
	}

	// Write the FOURCC marking the beginning of the index
	outFile.write("idx1");

	// Add a buffer to accomodate the size of the index.
	outFile.write(zero_buf, 4);
	unsigned long indexBodyStartPosition = outFile.position();
	unsigned long fileChunkOffset = 4;
	for (frame_cnt = 0; frame_cnt < pic_num; frame_cnt++) {
		// Print the index entry identifier
		outFile.write("00dc");

		// Print the dwFlags for the index indicating that this index entry corresponds to a key frame.
		outFile.write(indexDwFlags, 4);

		// Print the position quartet for the index entry
		print_quartet(fileChunkOffset, outFile);

		// Print the size of the index entry.
		print_quartet(fileChunkLength[frame_cnt], outFile);

		fileChunkOffset = fileChunkOffset + fileChunkLength[frame_cnt] + 8;
	}

	unsigned long indexBodySize = outFile.position() - indexBodyStartPosition;
	Serial.println(F("Index body size:"));
	Serial.println(indexBodySize);
	Serial.println(F("Index body start position:"));
	Serial.println(indexBodyStartPosition);
	// Print the size of the index
	outFile.seek(indexBodyStartPosition - 4);
	print_quartet(indexBodySize, outFile);

	//Modify the MJPEG header from the beginning of the file
	outFile.seek(4);
	// 228 represents the length of data after RIFF file size FOURCC to the "movi" FOURCC. As the list size (movisize) contains everything except the intial header (228) and index, we add those size.
	print_quartet(228 + movi_size + indexBodySize + 8, outFile);//riff file size.
																//overwrite hdrl + index size
	unsigned long us_per_frame = 1000000 / rate; //(per_usec); //hdrl.avih.us_per_frame
	outFile.seek(0x20);
	print_quartet(us_per_frame, outFile);
	unsigned long max_bytes_per_sec = movi_size * rate / frame_cnt; //hdrl.avih.max_bytes_per_sec
	outFile.seek(0x24);
	print_quartet(max_bytes_per_sec, outFile);
	unsigned long tot_frames = frame_cnt;    //hdrl.avih.tot_frames
	outFile.seek(0x30);
	print_quartet(tot_frames, outFile);
	unsigned long frames = frame_cnt;// (TOTALFRAMES); //hdrl.strl.list_odml.frames
	outFile.seek(0xe0);
	print_quartet(frames, outFile);
	outFile.seek(0xe8);
	print_quartet(movi_size, outFile);// size again

	myCAM.CS_HIGH();
	//Close the file
	outFile.close();
	Serial.println(F("Record video OK."));
}

void print_quartet(unsigned long i, File fd) {
	fd.write(i % 0x100);  i = i >> 8;   //i /= 0x100; 0x100 equals 256(0000 0000 0001 0000 0000). Then apply the bit-wise right shift operator. Each byte can represent numbers upto 255.
	fd.write(i % 0x100);  i = i >> 8;   //i /= 0x100;
	fd.write(i % 0x100);  i = i >> 8;   //i /= 0x100;
	fd.write(i % 0x100);
}