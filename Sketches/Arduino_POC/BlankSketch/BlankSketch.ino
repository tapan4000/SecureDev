// Set the serial ports for the bluetooth module Receive and Transmit
//SoftwareSerial moduleSerial(0, 1); //RX, TX

// Determine if the 

void setup() {
	Serial.begin(9600);
	short sampleData = -450;
	byte lowByte = sampleData;
	Serial.println(lowByte);
	byte highByte = sampleData >> 8;
	Serial.println(highByte);
	byte testByte = sampleData << 8;
	Serial.println(testByte);

	short sampleData2 = 450;
	byte lowByte2 = sampleData2;
	Serial.println(lowByte2);
	byte highByte2 = sampleData2 >> 8;
	Serial.println(highByte2);
	byte testByte2 = sampleData2 << 8;
	Serial.println(testByte2);
}

void loop() {
 
}
