#include <SoftwareSerial.h>

// Libraries related to ArduCam.
#include <Wire.h>
#include "memorysaver.h"
#include <ArduCAM.h>
#include <SPI.h>
#include <SD.h>

#define cbi(sfr, bit) (_SFR_BYTE(sfr) &= ~_BV(bit))
#define sbi(sfr, bit) (_SFR_BYTE(sfr) |= _BV(bit))

// Set the serial ports for the bluetooth module Receive and Transmit
//SoftwareSerial moduleSerial(0, 1); //RX, TX

// Determine if the 
#if !(defined OV2640_MINI_2MP)
  #error Please select the hardware platform and camera module in the ../libraries/ArduCAM/memorysaver.h file
#endif

#define BMPIMAGEOFFSET 66
#define BUFFERSIZE 512
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

// Configuration for Audio
int audioRecordTimeInSeconds = 10;
bool isFirstOccurence = true;
bool isRecording = false;
unsigned long elapsedTimeInMilliSeconds = 0;
unsigned long fileSize = 0L;
unsigned long waveChunk = 16;
unsigned int waveType = 1;
unsigned int numChannels = 1;
unsigned long sampleRate = 22050;
unsigned long bytesPerSec = 22050;
unsigned int blockAlign = 1;
unsigned int bitsPerSample = 8;
unsigned long dataSize = 0L;
unsigned long recByteCount = 0L;
unsigned long recByteSaved = 0L;

unsigned long oldTime = 0L;
unsigned long newTime = 0L;
byte buf00[BUFFERSIZE]; // buffer array 1
byte buf01[BUFFERSIZE]; // buffer array 2
byte byte1, byte2, byte3, byte4;
unsigned int bufByteCount;
byte bufWrite;

File rec;

// set pin 10 as the slave select for the digital pot:
const int CS = 7;
const int SD_CARD_Pin = 4;
const int gpsIntervalInSeconds = 20;
unsigned long lastGpsCaptureInMillis = 0;
bool is_header = false;
int mode = 0;
int imgNameCounter = 520;
uint8_t start_capture = 0;
String imageCounterFileName = "IMGCTR.TXT";
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
  //Serial3.begin(9600);
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
  
  if(!SD.begin(SD_CARD_Pin, SPI_FULL_SPEED)){
    Serial.println(F("SD Initialization failed"));
    return;
  }
  
  Serial.println(F("SD Initialization complete"));

  File imageNameCounterStore = SD.open(imageCounterFileName);
  if (imageNameCounterStore) {
	  while (imageNameCounterStore.available()) {
		  String storedValue = imageNameCounterStore.readStringUntil('\n');
		  Serial.println(F("Stored value:"));
		  Serial.println(storedValue);
		  imgNameCounter = storedValue.toInt();
	  }

	  Serial.println("Closing image counter file");
	  imageNameCounterStore.close();
  }
  else {
	  Serial.println(F("Image counter file open failed"));
  }

  String str = GetFileName(2);
  Serial.println(str);
  rec = SD.open(str, O_WRITE | O_CREAT | O_TRUNC);

  if (rec) {
	  Serial.println("Wave file opened");
  }
  else {
	  Serial.println("Wave file open failed");
  }

  //// initialize SPI:
  //SPI.begin();
  //while(1){
  //  //Check if the ArduCAM SPI bus is OK
  //  myCAM.write_reg(ARDUCHIP_TEST1, 0x55);
  //  temp = myCAM.read_reg(ARDUCHIP_TEST1);
  //  if (temp != 0x55){
  //    Serial.println(F("ACK CMD SPI interface Error!"));
  //    delay(1000);continue;
  //  }else{
  //    Serial.println(F("ACK CMD SPI interface OK."));break;
  //  }
  //}
  /*
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
  */
  isFirstOccurence = true;
  Serial.println("Setting up timer.");
  Setup_timer2();

  Serial.println("Setting up ADC.");
  Setup_ADC();
  Serial.println("Setting complete.");
}

void loop() {
	if (isFirstOccurence) {
		/*String str = GetFileName(2);
		rec = SD.open(str);*/
		if (!rec)
		{
			Serial.println(F("Wave File open failed"));
			while (1);
			return;
		}

		Serial.println("Starting recording");
		StartRec();
		isFirstOccurence = false;
		isRecording = true;
		elapsedTimeInMilliSeconds = millis();
	}

	if (isRecording && !isFirstOccurence && (millis() > (elapsedTimeInMilliSeconds + gpsIntervalInSeconds * 1000))) {
		Serial.println("Completed recording");
		StopRec();

		// Setting the isFirstOccurrence flag to true so that recording can start again.
		isFirstOccurence = true;
		isRecording = false;
		delay(100000);
	}
	
	unsigned int signalMax = 0;
	unsigned int signalMin = 1024;
	if (isRecording && recByteCount % (BUFFERSIZE*2) == BUFFERSIZE) {
		int averageValue = buf00[0];
		for (int i = 1; i < BUFFERSIZE; i++) {
			averageValue = (averageValue + buf00[i]) / 2;

			if (buf00[i] > signalMax)
			{
				signalMax = buf00[i]; // save just the max levels
			}
			else if (buf00[i] < signalMin)
			{
				signalMin = buf00[i]; // save just the min levels
			}
		}
		
		Serial.print("A:");
		Serial.print(averageValue);
		Serial.print("Mx:");
		Serial.print(signalMax);
		Serial.print("Mn:");
		Serial.println(signalMin);
		//Serial.print("Lg:");
		//Serial.println(analogRead(5));
		//Serial.println(averageValue);

		rec.write(buf00, BUFFERSIZE); recByteSaved += BUFFERSIZE; // save buf01 to card
	} 
	else if (isRecording && recByteCount % (BUFFERSIZE*2) == 0) {
		int averageValue = buf01[0];
		for (int i = 1; i < BUFFERSIZE; i++) {
			averageValue = (averageValue + buf01[i]) / 2;

			if (buf01[i] < 255) // toss out spurious readings
			{
				if (buf01[i] > signalMax)
				{
					signalMax = buf01[i]; // save just the max levels
				}
				else if (buf01[i] < signalMin)
				{
					signalMin = buf01[i]; // save just the min levels
				}
			}
		}

		Serial.print("A:");
		Serial.print(averageValue);
		Serial.print("Mx:");
		Serial.print(signalMax);
		Serial.print("Mn:");
		Serial.println(signalMin);
		//Serial.print("Lg:");
		//Serial.println(analogRead(5));
		rec.write(buf01, BUFFERSIZE); recByteSaved += BUFFERSIZE; // save buf02 to card
	} 
	//else if (isRecording) {
	//	// Read the analog value
	//	byte analogHighByte = highByte(analogRead(5));
	//	recByteCount++; // increment sample counter
	//	bufByteCount++;
	//	if (bufByteCount == BUFFERSIZE && bufWrite == 0) {
	//		bufByteCount = 0;
	//		bufWrite = 1;
	//	}
	//	else if (bufByteCount == BUFFERSIZE & bufWrite == 1) {
	//		bufByteCount = 0;
	//		bufWrite = 0;
	//	}

	//	if (bufWrite == 0) { buf00[bufByteCount] = analogHighByte; }
	//	if (bufWrite == 1) { buf01[bufByteCount] = analogHighByte; }
	//}
}

void StartRec() { // begin recording process
	recByteCount = 0;
	recByteSaved = 0;
	writeWavHeader();

	// TIMSK2 is a 8 bit register [-,-,-,-,-,OCIE2B,OCIE2A,TOIE2]
	sbi(TIMSK2, OCIE2A); // enable timer interrupt, start grabbing audio
}

void StopRec() { // stop recording process, update WAV header, close file
	cbi(TIMSK2, OCIE2A); // disable timer interrupt
	//rec.close();
	writeOutHeader();
}

void writeOutHeader() { // update WAV header with final filesize/datasize
	Serial.println(F("Received bytes"));
	Serial.print(recByteSaved);
	rec.seek(4);
	byte1 = recByteSaved & 0xff;
	byte2 = (recByteSaved >> 8) & 0xff;
	byte3 = (recByteSaved >> 16) & 0xff;
	byte4 = (recByteSaved >> 24) & 0xff;
	rec.write(byte1);  rec.write(byte2);  rec.write(byte3);  rec.write(byte4);
	rec.seek(40);
	rec.write(byte1);  rec.write(byte2);  rec.write(byte3);  rec.write(byte4);
	rec.close();
}

void writeWavHeader() { // write out original WAV header to file
	
	recByteSaved = 0;
	//rec.write(zero_buf, 4);
	rec.write("RIFF"); // Specifies the chunk Id
	rec.write(zero_buf, 4); // Specifies the Chunk size
	/*byte1 = fileSize & 0xff;
	byte2 = (fileSize >> 8) & 0xff;
	byte3 = (fileSize >> 16) & 0xff;
	byte4 = (fileSize >> 24) & 0xff;
	rec.write(byte1);  rec.write(byte2);  rec.write(byte3);  rec.write(byte4);*/
	rec.write("WAVE"); // Specifies the format
	rec.write("fmt "); // Specifies the sub chunk 1 id
	byte1 = waveChunk & 0xff; 
	byte2 = (waveChunk >> 8) & 0xff;
	byte3 = (waveChunk >> 16) & 0xff;
	byte4 = (waveChunk >> 24) & 0xff;
	rec.write(byte1);  rec.write(byte2);  rec.write(byte3);  rec.write(byte4); // Specifies the sub chunk 1 size.
	byte1 = waveType & 0xff;
	byte2 = (waveType >> 8) & 0xff;
	rec.write(byte1);  rec.write(byte2); // Specifies the audio format. Values other than 1 specifies some sort of compression.
	byte1 = numChannels & 0xff;
	byte2 = (numChannels >> 8) & 0xff;
	rec.write(byte1);  rec.write(byte2); // Specifies the number of channels.
	byte1 = sampleRate & 0xff;
	byte2 = (sampleRate >> 8) & 0xff;
	byte3 = (sampleRate >> 16) & 0xff;
	byte4 = (sampleRate >> 24) & 0xff;
	rec.write(byte1);  rec.write(byte2);  rec.write(byte3);  rec.write(byte4); // Specifies the sample rate.
	byte1 = bytesPerSec & 0xff;
	byte2 = (bytesPerSec >> 8) & 0xff;
	byte3 = (bytesPerSec >> 16) & 0xff;
	byte4 = (bytesPerSec >> 24) & 0xff;
	rec.write(byte1);  rec.write(byte2);  rec.write(byte3);  rec.write(byte4); // Specifies the byte rate.
	byte1 = blockAlign & 0xff;
	byte2 = (blockAlign >> 8) & 0xff;
	rec.write(byte1);  rec.write(byte2); // Specifies the number of bytes for one sample including all channels.
	byte1 = bitsPerSample & 0xff;
	byte2 = (bitsPerSample >> 8) & 0xff;
	rec.write(byte1);  rec.write(byte2); // Specifies the bits per sample.
	rec.write("data");
	byte1 = dataSize & 0xff;
	byte2 = (dataSize >> 8) & 0xff;
	byte3 = (dataSize >> 16) & 0xff;
	byte4 = (dataSize >> 24) & 0xff;
	rec.write(byte1);  rec.write(byte2);  rec.write(byte3);  rec.write(byte4);
	Serial.println("Write header complete");
}

void Setup_timer2() {
	// https://www.arduino.cc/en/Tutorial/SecretsOfArduinoPWM
	// ATMega328P has three timers as Timer0, timer1 and timer2. Each of the timers has a counter that is incremented on each tick of the
	// timer's clock. CTC timer interrupts are triggered when the counter reaches a specific value, stored in the compare match register.
	// Once the timer counter reaches this value it will clear (reset to 0) on the next tick of the timer's clock, then it will continue
	// to count up to the compare match value again. By choosing the compare match value and setting the speed at which the timer increments
	// the counter, you can control the frequency of the interrupts. The Arduino clock runs at 16MHz, this is the fastest speed at which
	// the timers can increase their counters. Timer0 and Timer2 are 8 bit timers, meaning they can store a maximum counter value of 255.
	// Timer1 is a 16 bit counter and can store a maximum counter value of 65535. Once the counter reaches maximum it will tick back to 0,
	// it is called overflow. This means at 16MHz even if we set the compare match register to the max counter value, interrupts will occur
	// every 256/16*10^6 seconds (~16us) for the 8 bit counters, and every 65535/16*10^6 seconds (~4ms) for the 16 bit counter. We surely
	// do not need such speed of increment. We control the speed at which the counter is incremented by using a prescaler. A prescaler
	// dictates the speed of the timer by using following equation: timer speed(Hz) = Arduino clock speed(16MHz)/prescaler.
	// The prescaler can be equal to 1, 8, 64, 256 and 1024. The interrupt frequency can be calculated using the following formula
	// interrupt frequency(Hz) = Arduino clock speed(16MHz)/prescaler*(compare match register + 1).
	// The +1 is in there becuase compare match register is zero indexed.
	// Each timer has two output compare registers (TCCR2A and TCCR2B) that control the PWM (Pulse width modulation)
	// width for the timer's two outputs. When the timer reaches the compare register value, the corresponding output is toggled. These
	// registers hold several group of bits like Waveform Generation Mode bits (WGM) - they control the overall mode of the timer,
	// Clock Select bits (CS) - they control clock prescaler, Compare Match Output A Mode bits (COMnA) - they enable/disable/invert 
	// output A, similarly for outputB modes. Timer 2 controls pin 3 and 11.
	// Arduino performs some initialization on the timers. It generates a prescaler of 64 on all three timers. Timer 0 is initialized to
	// fast PWM, while Timer 1 and 2 are initialized to Phase correct PWM. Arduino uses Timer0 internally for delay() and millis(), so
	// this timer should not be touched else these functions would be affected. AnalogWrite(pin, duty_cycle) sets the appropriate pin
	// to PWM and sets the output compare register to appropriate output compare register duty cycle. digitalwrite turns off PWM on
	// specified pin. You need to both enable pin for output and enable the PWM mode on pin in order to get any output.
	// Set the time counter control registers A and B for timer counter2. 
	// COM1x controls whether the PWM output generated should be inverted or not. Setting WGM21 to 1 sets the mode as CTC (clear timer on compare match)
	// mode where the counter is cleared to 0, when the counter value matches OCR2A.
	//TCCR2A - [COM2A1, COM2A0, COM2B1, COM2B0, reserved, reserved, WGM21, WGM20]
	//TCCR2B - [FOC2A, FOC2B, reserved, reserved, WGM22, CS22, CS21, CS20]

	TCCR2B = _BV(CS21); // Timer2 clock prescaler to: 8. Setting the prescaler to 8 updates the counter every 8 clock cycles. BV sets the bit value as high for specific bit. Here it sets the value to 1 << 1.
	TCCR2A = _BV(WGM21); // Interrupt frequency =  16Mhz/(8*90 + 1) = 22191 Hz; Alternate: 16MHz/(8*45 + 1) = 44321Hz
	OCR2A = 90; // Output Compare match register A on timer 2 set to 90 (counts from 0 to 90); It is the counter for timer 2. Corresponds to pin 11.

	//cli(); // Stop interrupts
	//TCCR2A = 0; // Set entire TCCR2A register to 0.
	//TCCR2B = 0; // Set entire TCCR2B register to 0.
	//TCNT2 = 0; // Initialize counter value to 0.
	//OCR2A = 90; // Output Compare match register A on timer 2 set to 90 (counts from 0 to 90); It is the counter for timer 2. Corresponds to pin 11.
	//TCCR2A = _BV(WGM21); // Turn on CTC mode by setting the WGM21 flag. 
	//TCCR2B = _BV(CS21); // Set CS21 bit for 8 prescaler. BV sets the bit value as high for specific bit. Here it sets the value to 1 << 1.
	//TIMSK2 = _BV(OCIE2A); // Enable timer compare interrupt.
	//sei(); // Allow interrupts.
	// Final Interrupt frequency = 16Mhz / (8 * 90 + 1) = 22191 Hz
}

void Setup_ADC() {
	// ADMUX is an 8-bit that holds the settings for the analog reference voltage and the analog pin to select.
	// [(Bit 7)REFS1, REFS0, ADLAR, -, MUX3, MUX2, MUX1, MUX0]. Setting RefS1 as 0 and REFS0 as 1 sets the voltage as AVcc 
	// (5 V for Arduino) with external capacitor on AREF pin. This is the default for an arduino.
	// ADLAR determines the presentation of ADC conversion result. If 1, it is left adjusted. If 0, right adjusted. If 1, the ADC
	// result is stored left adjusted with the 8 most significant bits held in ADCH and the remaining two bits held in ADCL. Now if 
	// you want 8 bit resolution simply read ADCH. The MUX selects the analog pin. So, 0101 corresponds to pin 5.
	// cbi and sbi are used to clear and set the bits. We pass them a port variable and a pin to set
	// The ADCSRA (ADC control and status register A)-[ADEN, ADSC, ADATE, ADIF, ADIE, ADPS2, ADPS1, ADPS0]
	// Setting ADEN to 1 enables AD conversion, Setting ADSC (ADC Start conversion) to 1 chip begins AD conversion (When AD conversion
	// is executed this bit is set to 1 and after conversion it becomes 0), ADATE(ADC Auto trigger enable) controls automatic trigger
	// of AD conversion, ADIF(ADC interrupt flag) and ADIE(ADC interrupt enable) controls the interruption. ADPS (16, 4, 2)
	// are bits used to determine the division factor between the system clock frequency and the input clock to the AD converter.
	// Their value is set by multiplying the set bit values for ADPS.
	ADMUX = 0x65; // 0110 0101, set ADC to read pin A5, ADLAR to 1 (left adjust)
	cbi(ADCSRA, ADPS2); // set prescaler to 8 / ADC clock = 2MHz
	sbi(ADCSRA, ADPS1);
	sbi(ADCSRA, ADPS0);
	//sbi(ADCSRA, ADPS2); // set prescaler to 32 / ADC clock = 500KHz
	//cbi(ADCSRA, ADPS1);
	//sbi(ADCSRA, ADPS0);

	//ADCSRA = 0;
	//ADCSRB = 0; // Clear the ADCSRA and ADCSRB registers.
	//sbi(ADCSRA, ADPS2); // set prescaler to 32 / ADC clock = 500KHz. As reading the analog value takes 13 clock cycles, 
	//// it would supply the value at the rate 500KHz/13 = 38461 Hz
	//cbi(ADCSRA, ADPS1);
	//sbi(ADCSRA, ADPS0);
	//sbi(ADCSRA, ADATE); // Enable auto-trigger 
	//sbi(ADCSRA, ADEN); // Enable ADC
	//sbi(ADCSRA, ADSC); // Start ADC measurement
}

ISR(TIMER2_COMPA_vect) {
	// This interrupt is triggered every time the OCR2A value is reached.
	sbi(ADCSRA, ADSC); // start ADC sample
	while (bit_is_set(ADCSRA, ADSC));  // wait until ADSC bit goes low = new sample ready
	recByteCount++; // increment sample counter
	bufByteCount++;
	if (bufByteCount == BUFFERSIZE && bufWrite == 0) {
		bufByteCount = 0;
		bufWrite = 1;
	}
	else if (bufByteCount == BUFFERSIZE & bufWrite == 1) {
		bufByteCount = 0;
		bufWrite = 0;
	}

	if (bufWrite == 0) { buf00[bufByteCount] = ADCH; }
	if (bufWrite == 1) { buf01[bufByteCount] = ADCH; }


	//  if (recByteCount % 1024 < 512) { // determine which buffer to store sample into
	//    buf01[recByteCount % 512] = ADCH;
	//  } else {
	//    buf02[recByteCount % 512] = ADCH;
	//  }

}


String GetFileName(int fileType){
	// fileType = 0 represents jpg, 1 represents avi and 2 represents wav.
   // Construct the file name
  imgNameCounter = 1352;
  imgNameCounter = imgNameCounter+1;
  Serial.println(F("Opening the image counter file"));
  Serial.println(FreeRam());
  Serial.println(imageCounterFileName);
  /*File imageCounterStore = SD.open(imageCounterFileName, O_WRITE | O_CREAT | O_TRUNC);
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
  }*/

  String fileExtension = ".jpg";

  //Serial.println(F("Begin if check"));
  if(fileType == 1){
    fileExtension = ".avi";
  }
  else if (fileType == 2) {
	  fileExtension = ".wav";
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
	String str = GetFileName(1);
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