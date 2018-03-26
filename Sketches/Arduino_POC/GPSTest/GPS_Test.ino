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

// GPS related information
int byteGPS = -1;
char cmd[7] = "$GPRMC";
int counter1 = 0; // counts how many bytes were received (max 300)
int counter2 = 0; // counts how many commas were seen
int offsets[13];
char buf[300] = "";
int gpsRxPin = 2;
int gpsTxPin = 3;
//SoftwareSerial gpsSerial(gpsRxPin, gpsTxPin);

// set pin 10 as the slave select for the digital pot:
const int CS = 7;
const int SD_CARD_Pin = 4;
const int gpsIntervalInSeconds = 20;
unsigned long lastGpsCaptureInMillis = 0;
bool is_header = false;
int mode = 0;
int imgNameCounter = 520;
uint8_t start_capture = 0;
String imageCounterFileName = "imgCtr3.txt";
String message = "";
#if defined (OV2640_MINI_2MP)
  ArduCAM myCAM( OV2640, CS );
#else
  ArduCAM myCAM( OV5642, CS );
#endif
uint8_t read_fifo_burst(ArduCAM myCAM, String fileName, bool isVideoClip);

void setup() {
	// Setup GPS
	//pinMode(gpsRxPin, INPUT);
	//pinMode(gpsTxPin, OUTPUT);

  // put your setup code here, to run once:
  uint8_t vid, pid;
  uint8_t temp;
  Serial3.begin(9600);
  //gpsSerial.begin(9600);
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
  
  if(!SD.begin(SD_CARD_Pin)){
    Serial.println(F("SD Initialization failed"));
    return;
  }
  
  Serial.println(F("SD Initialization complete"));

  // initialize SPI:
  SPI.begin();
  while(1){
    //Check if the ArduCAM SPI bus is OK
    myCAM.write_reg(ARDUCHIP_TEST1, 0x55);
    temp = myCAM.read_reg(ARDUCHIP_TEST1);
    if (temp != 0x55){
      Serial.println(F("ACK CMD SPI interface Error!"));
      delay(1000);continue;
    }else{
      Serial.println(F("ACK CMD SPI interface OK."));break;
    }
  }

    while(1){
      //Check if the camera module type is OV2640
      myCAM.wrSensorReg8_8(0xff, 0x01);
      myCAM.rdSensorReg8_8(OV2640_CHIPID_HIGH, &vid);
      myCAM.rdSensorReg8_8(OV2640_CHIPID_LOW, &pid);
      if ((vid != 0x26 ) && (( pid != 0x41 ) || ( pid != 0x42 ))){
        Serial.println(F("ACK CMD Can't find OV2640 module!"));
        delay(1000);continue;
      }
      else{
        Serial.println(F("ACK CMD OV2640 detected."));break;
      } 
    }

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
  myCAM.write_reg(ARDUCHIP_FRAMES,0x00);
  #endif

  // Clear GPS info
  offsets[0] = 0;
  reset();
}

void reset() {
	counter1 = 0;
	counter2 = 0;
}

int getSize(int offset) {
	return offsets[offset + 1] - offsets[offset] - 1;
}

int handle_byte(int byteGPS) {
	buf[counter1] = byteGPS;
	Serial.print((char)byteGPS);
	counter1++;
	if (counter1 == 300) {
		return 0;
	}

	if (byteGPS == ',') {
		counter2++;
		offsets[counter2] = counter1;
		if (counter2 == 13) {
			return 0;
		}
	}

	/*if (counter2 == 1 && getSize(0) == 6) {
		bool isGPRMC = true;
		for (int j = 0; j < 6; j++) {
			if (buf[j] != cmd[j]) {
				isGPRMC = false;
			}
		}

		if (isGPRMC) {
			Serial.print('-');
			Serial.print(byteGPS);
			Serial.print('-');
		}
	}*/

	if (byteGPS == '*') {
		offsets[12] = counter1;
	}

	// Check if we got <LF> which indicates the end of line.
	if (byteGPS == 10) {
		//Serial.println("Reached EOL");
		// Check that we got 12 pieces and the first piece is 6 characters.
		if (counter2 != 12 || (getSize(0) != 6)) {
			/*Serial.println("Size not right.");
			Serial.print(counter2);
			Serial.println("--");
			Serial.print(getSize(0));*/
			return 3;
		}

		// Check that we received $GPRMC
		for (int j = 0; j < 6; j++) {
			if (buf[j] != cmd[j]) {
				//Serial.println("Not GPRMC");
				return 3;
			}
		}

		// Check that the time is well formatted.
		/*if (getSize(1) != 9) {
			Serial.println("Time not formatted");
			Serial.print(getSize(1));
			return 3;
		}*/

		// Check that the date is well formatted
		/*if (getSize(9) != 6) {
			Serial.println("Date not formatted");
			Serial.print(getSize(9));
			return 3;
		}*/

		// TODO: Compute and validate checksum

		// TODO: Handle timezone offset
		//Serial.println("EOL processing complete.");
		return 2;
	}

	return 1;
}

void loop() {
	//Serial.println("Start");
	//Serial.print(lastGpsCaptureInMillis);
	if (lastGpsCaptureInMillis == 0) {
		Serial.println("Empty");
		Serial.println(millis() - lastGpsCaptureInMillis);
	}
  /* put your main code here, to run repeatedly:
  uint8_t temp = 0xff, temp_last = 0;
  bool is_header = false;
  if (Serial.available())
  {
    temp = Serial.read();
    Serial.println(F("Received a value as: "));
    Serial.write(temp);
	temp = 0x10;
    switch (temp)
    {
      case 2: 
        myCAM.OV2640_set_JPEG_size(OV2640_320x240);delay(1000);
        Serial.println(F("ACK CMD switch to OV2640_320x240"));
      temp = 0xff;
      break;
      case 0x10:
	  Video2SD();

      break;
    }
  }
  if (mode == 1)
  {
    if (start_capture == 1)
    {
      myCAM.flush_fifo();
      myCAM.clear_fifo_flag();
      //Start capture
      myCAM.start_capture();
      start_capture = 0;
    }
    if (myCAM.get_bit(ARDUCHIP_TRIG, CAP_DONE_MASK))
    {
      Serial.println(F("ACK CMD CAM Capture Done."));
      String fileName = GetFileName(false);
      if(fileName != ""){
        read_fifo_burst(myCAM, fileName, false);
      }
           
      //Clear the capture done flag
      myCAM.clear_fifo_flag();
    }

	mode = 0;
  }
  else if (mode == 2)
  {
    // Set the video interval for which the video would be captured.
    int videoInterval = 2000;
    String fileName = GetFileName(true);
    //Serial.println(F("Printing time:"));
    unsigned long timer = millis();
    //Serial.println(timer);
    //Serial.println(videoInterval);
    //Serial.println(millis());
    while (millis() - timer < videoInterval)
    {      
      if (start_capture == 2)
      {
        Serial.println(F("Entered capture mode 2"));
        myCAM.flush_fifo();
        myCAM.clear_fifo_flag();
        //Start capture
        myCAM.start_capture();
        start_capture = 0;
      }
      if (myCAM.get_bit(ARDUCHIP_TRIG, CAP_DONE_MASK))
      {      
        //Serial.println(F("Entered bit mast"));
        if(fileName != ""){
          //Serial.println(F("Entered file name present"));
          read_fifo_burst(myCAM, fileName, true);
        }
      }
    }

	mode = 0;
  }*/
	if (lastGpsCaptureInMillis == 0 || millis() - lastGpsCaptureInMillis > gpsIntervalInSeconds * 1000) {
		// Capture the GPS data
		lastGpsCaptureInMillis = millis();
		bool isGpsDataCaptureComplete = false;
		while (!isGpsDataCaptureComplete) {
			byteGPS = Serial3.read();
			if (byteGPS == -1) {
				//Serial.println("Delay");
				delay(100);
			}
			else {
				int byteHandleRespone = handle_byte(byteGPS);
				if (byteHandleRespone == 0) {
					reset();
				}
				else if (byteHandleRespone == 2) {
					// If the GPRMC data is successfully found, then print the data.
					Serial.println(F("Success"));

					isGpsDataCaptureComplete = true;

					// Print out the GPRMC line
					for (int i = 0; i < counter1; i++) {
						Serial.print(buf[i]);
					}

					Serial.println("");
					Serial.println("Decoded Data:");

					////
					//Serial.println("Start");
					//String numStr = "23.43243";
					//float num1 = numStr.toFloat();
					//double num2 = numStr.toDouble();
					//Serial.println(num1, 5);
					//Serial.println("--");
					//Serial.println(num2, 5);
					//Serial.println("End");
					////

					// Print time
					Serial.print("Time: ");

					if (buf[offsets[1]] != ',') {
						for (int i = offsets[1]; i < offsets[2] - 1; i++) {
							Serial.print(buf[i]);
						}

						Serial.print("Formatted: ");
						Serial.print(buf[offsets[1]]);
						Serial.print(buf[offsets[1] + 1]);
						Serial.print(":");
						Serial.print(buf[offsets[1] + 2]);
						Serial.print(buf[offsets[1] + 3]);
						Serial.print(":");
						Serial.print(buf[offsets[1] + 4]);
						Serial.print(buf[offsets[1] + 5]);
						Serial.print(":");
						Serial.print(buf[offsets[1] + 7]);
						Serial.print(buf[offsets[1] + 8]);
						Serial.println(" UTC");
					}
					else {
						Serial.println("Not available");
					}

					// Print Signal Type
					Serial.print("Signal Type: ");
					Serial.println(buf[offsets[2]]);

					//Print latitude
					Serial.print("Latitude: ");
					if (buf[offsets[2]] == 'A') {
						Serial.println(GetPositionInDegrees(offsets[3], offsets[4] - 1, buf[offsets[4]], true), 5);
						//int decimalPosition = 0;
						//for (int i = offsets[3]; i < offsets[4] - 1; i++) {
						//	if (buf[i] == '.') {
						//		decimalPosition = i;
						//	}
						//}

						//// Two degits before decimal represent the minutes and everything after decimal respresent minutes.
						//// So, the remaining digits prefixing the minutes represents the degrees.
						//for (int i = offsets[3]; i < decimalPosition - 2; i++) {
						//	Serial.print(buf[i]);
						//}

						//Serial.print(" Degrees ");
						//for (int i = decimalPosition - 2; i < offsets[4] - 1; i++) {
						//	Serial.print(i);
						//}

						//Serial.print(" Minutes - ");
						//Serial.println(buf[offsets[4]]);
					}
					else {
						Serial.println("Not available");
					}

					//Print longitude
					Serial.print("Longitude: ");
					if (buf[offsets[2]] == 'A') {
						Serial.println(GetPositionInDegrees(offsets[5], offsets[6] - 1, buf[offsets[6]], false), 5);
					}
					else {
						Serial.println("Not available");
					}

					// Print Speed
					Serial.print("Speed: ");
					if (buf[offsets[2]] == 'A') {
						for (int i = offsets[7]; i < offsets[8] - 1; i++) {
							Serial.print(buf[i]);
						}
					}
					else {
						Serial.println("Not available");
					}

					// Print Date
					Serial.println("");
					Serial.print("Date: ");

					if (buf[offsets[9]] != ',') {
						Serial.print(buf[offsets[9]]);
						Serial.print(buf[offsets[9] + 1]);
						Serial.print("-");
						Serial.print(buf[offsets[9] + 2]);
						Serial.print(buf[offsets[9] + 3]);
						Serial.print("-");
						Serial.print(buf[offsets[9] + 4]);
						Serial.print(buf[offsets[9] + 5]);
					}
					else {
						Serial.println("Not available");
					}

					reset();
				}
				else if (byteHandleRespone == 3) {
					//delay(1000);
					reset();
				}
			}
		}
	}
	else {
		//Serial.println("NA");
		//Serial.print(lastGpsCaptureInMillis);
	}
	
	//if (Serial3.available()) {
	//	byteGPS = Serial3.read();
	//	if (byteGPS == -1) {
	//		delay(100);
	//	}
	//	else {
	//		int byteHandleRespone = handle_byte(byteGPS);
	//		if (byteHandleRespone == 0) {
	//			reset();
	//		}
	//		else if(byteHandleRespone == 2) {
	//			// If the GPRMC data is successfully found, then print the data.
	//			//Serial.println(F("Success"));
	//			reset();
	//		}
	//		else if (byteHandleRespone == 3) {
	//			//delay(1000);
	//			reset();
	//		}
	//	}
	//}
	//else {
	//	//Serial.println(F("N/A"));
	//	//Serial3.print("B");
	//	delay(100);
	//}
}

float GetPositionInDegrees(int startPosition, int endPosition, char direction, bool isLatitude) {
	int decimalPosition = 0;
	for (int i = startPosition; i < endPosition; i++) {
		if (buf[i] == '.') {
			decimalPosition = i;
		}
	}

	// Two degits before decimal represent the minutes and everything after decimal respresent minutes.
	// So, the remaining digits prefixing the minutes represents the degrees.
	String numberAsString = "";
	for (int i = startPosition; i < decimalPosition - 2; i++) {
		numberAsString += buf[i];
	}

	float degrees = numberAsString.toFloat();
	
	numberAsString = "";
	for (int i = decimalPosition - 2; i < endPosition; i++) {
		numberAsString += buf[i];
	}

	float minutes = numberAsString.toFloat();
	degrees = degrees + minutes / 60;

	if (isLatitude) {
		if(direction == 'S'){
			degrees = degrees * -1;
		}
	}
	else {
		if (direction == 'W') {
			degrees = degrees * -1;
		}
	}

	return degrees;
}

String GetFileName(bool isVideoStream){
   // Construct the file name
  imgNameCounter = imgNameCounter+1;
  Serial.println(F("Opening the image counter file"));
  Serial.println(FreeRam());
  Serial.println(imageCounterFileName);
  File imageCounterStore = SD.open(imageCounterFileName, O_CREAT | O_WRITE | O_TRUNC);
  Serial.println(imgNameCounter);
  //Serial.println("Opening the image counter file -op complete");
  if(imageCounterStore){
    Serial.println(F("Image counter file found"));
    imageCounterStore.println(imgNameCounter);  
    //Serial.println(F("Image counter value written"));
    imageCounterStore.close();
    //Serial.println(F("Image counter file closed"));
  }
  else{
    // Print a log to identify the cause for failure.
    Serial.println(F("Image counter file not found"));
    return "";
  }

  String fileExtension = ".jpg";

  //Serial.println(F("Begin if check"));
  if(isVideoStream){
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

  uint32_t i=0;
  uint8_t temp = 0, temp_last = 0;
  uint32_t length = 0;
  
  length = myCAM.read_fifo_length();
 
  if (length >= MAX_FIFO_SIZE) //512 kb
  {
    Serial.println(F("Over size."));

    if(isVideoClip){
      myCAM.clear_fifo_flag();
      start_capture = 2;  
    }
    
    return 0;
  }
  if (length == 0 ) //0 kb
  {
    Serial.println(F("Size is 0."));
    if(isVideoClip){
      myCAM.clear_fifo_flag();
      start_capture = 2;  
    }
    
    return 0;
  }

  str = fileName;
  
  //str = imgNameCounter + ".jpg";
  Serial.println(str);
  
  // Open the new file
  File outFile = SD.open(str, FILE_WRITE);

  Serial.println(F("Open requested"));
  if(!outFile)
  {
    Serial.println(F("File open failed"));
    return 0;
  }

  Serial.println(F("File opened successfully"));
  Serial.println(is_header);
  Serial.println(F("Begin write operation"));
  myCAM.CS_LOW();
  myCAM.set_fifo_burst();//Set fifo burst mode
  temp =  SPI.transfer(0x00);
  uint32_t initialLength = length;
  length --;
  bool isFirst = true;
  uint32_t count = 0;
  uint8_t imageBytes[50];
  uint32_t imageByteCounter = 0;
  uint32_t totalByteCounter = 0;
  while ( length-- )
  {
    totalByteCounter++;
    temp_last = temp;
    temp =  SPI.transfer(0x00);

    if(isFirst){
      isFirst = false;
    }
    if (is_header == true)
    {
      imageByteCounter++;
      //Serial.write(temp);
      if(i < 256){
        buf[i++] = temp;
      }
      else{
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
      imageByteCounter++;
      buf[i++] = temp_last;
      buf[i++] = temp;
    }
    if ( (temp == 0xD9) && (temp_last == 0xFF) ) //If find the end ,break while, (217 and 255)
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
  }


  myCAM.CS_HIGH();
  Serial.println(F("End function"));
  if(isVideoClip){
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
		/*Serial.println(F("Length:"));
		Serial.println(length);
		Serial.println(outFile.position());*/
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

				/*Serial.println(F("Last:"));
				Serial.println(jpeg_size);
				Serial.println(outFile.position());
				Serial.println(i);*/

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

		/*Serial.println(F("Post Write:"));
		Serial.println(is_header);
		Serial.println(jpeg_size);
		Serial.println(outFile.position());*/
		if (jpeg_size == 0) {
			Serial.println(F("Jpeg size is zero"));
			Serial.println(i);
		}

		remnant = (4 - (jpeg_size & 0x00000003)) & 0x00000003;// Take bit-wise and with 3 (0000 0011)
		jpeg_size = jpeg_size + remnant;
		movi_size = movi_size + jpeg_size + 8; // The 8 bytes are added for the 00dc and chunk size data.
		if (remnant > 0)
		{
			//Serial.println("Received remnant:");
			//Serial.println(outFile.position());
			//Serial.println(remnant);
			outFile.write(zero_buf, remnant);
		}
		position = outFile.position();
		outFile.seek(position - 4 - jpeg_size);

		/*Serial.println("Size printing:");
		Serial.println(jpeg_size);
		Serial.println(position);*/

		print_quartet(jpeg_size, outFile);
		position = outFile.position();
		outFile.seek(position + 6);
		outFile.write("AVI1", 4);
		position = outFile.position();
		outFile.seek(position + jpeg_size - 10);

		// Set the frame chunk size to be written to the index. The index contains the offset beginning from the frame list starting position.
		fileChunkLength[frame_cnt] = jpeg_size;
		//Serial.println(F("chunk size:"));
		//Serial.println(fileChunkLength[frame_cnt]);
	}

	Serial.println(F("Beginning to write index:"));
	Serial.println(outFile.position());
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

	Serial.println(F("Movie Size:"));
	Serial.println(movi_size);
	Serial.println(F("Outfile position:"));
	Serial.println(outFile.position());
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
	Serial.println(F("Movie Size:"));
	Serial.println(movi_size);
	print_quartet(movi_size, outFile);// size again

	// Reset the movie size.
	movi_size = 4;
	myCAM.CS_HIGH();
	//Close the file
	outFile.flush();
	outFile.close();
	Serial.println(F("Record video OK."));
}

void print_quartet(unsigned long i, File fd) {
	//Serial.println(i % 0x100);

	/*if (i == 5120) {
		Serial.println(F("Printing 5120"));
		fd.write(size5120, 4);
		return;
	}

	if (i == 4096) {
		Serial.println(F("Printing 4096"));
		fd.write(size4096, 4);
		return;
	}*/
	delayMicroseconds(100);
	unsigned long orig = i;
	int a = i % 0x100;
	int bytesWritten = 0;

	if (orig == 0) {
		Serial.println(F("Orig is 0."));
	}

	bytesWritten = fd.write(a);  
	if (bytesWritten == 0) {
		Serial.println(F("Zero bytes at 0."));
		Serial.println(a);
	}
	i = i >> 8;   //i /= 0x100; 0x100 equals 256(0000 0000 0001 0000 0000). Then apply the bit-wise right shift operator. Each byte can represent numbers upto 255.
	

	//Serial.println(i % 0x100);
	int b = i % 0x100;
	bytesWritten = fd.write(b);  
	if (bytesWritten == 0) {
		Serial.println(F("Zero bytes at 1."));
		Serial.println(b);
	}
	i = i >> 8;   //i /= 0x100;
	
	//Serial.println(i % 0x100);
	int c = i % 0x100;
	bytesWritten = fd.write(c);  
	if (bytesWritten == 0) {
		Serial.println(F("Zero bytes at 2."));
		Serial.println(c);
	}

	i = i >> 8;   //i /= 0x100;
	
	//Serial.println(i % 0x100);
	int d = i % 0x100;
	bytesWritten = fd.write(d);
	if (bytesWritten == 0) {
		Serial.println(F("Zero bytes at 3."));
		Serial.println(d);
	}

	if (a == 0 && b == 0 && c == 0 && d == 0) {
		Serial.println("Zero:");
		Serial.println(orig);
	}

	if (orig == 0) {
		Serial.println(a);
		Serial.println(b);
		Serial.println(c);
		Serial.println(d);
	}
}