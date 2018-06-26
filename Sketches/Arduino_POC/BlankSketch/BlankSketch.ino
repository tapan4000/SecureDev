// Set the serial ports for the bluetooth module Receive and Transmit
//SoftwareSerial moduleSerial(0, 1); //RX, TX

// Determine if the 
int counter = 0;
unsigned long maxMicros = 0;
unsigned long lastMacros = 0;
unsigned long minMicros = 99999;
int potPin = 5;
int val = 0;
unsigned long sample = 0;
void setup() {
	Serial.begin(9600);
	Serial.println("Print -450");
	short sampleData = -450;
	byte lowByte = sampleData;
	Serial.println(lowByte);
	byte highByte = sampleData >> 8;
	Serial.println(highByte);
	short formedSample = lowByte | highByte << 8;
	Serial.println(formedSample);

	Serial.println("Print 450");
	short sampleData2 = 450;
	byte lowByte2 = sampleData2;
	Serial.println(lowByte2);
	byte highByte2 = sampleData2 >> 8;
	Serial.println(highByte2);

	Serial.println("Print 450 by forming a number reading the bytes from it.");
	short sampleData3 = lowByte2 | highByte2 << 8;
	Serial.println(sampleData3);
	byte lowByte3 = sampleData3;
	Serial.println(lowByte3);
	byte highByte3 = sampleData3 >> 8;
	Serial.println(highByte3);
}

void loop() {
	
	if (counter < 10005) {
		val = analogRead(potPin);
		Serial.println(val);
		delay(100);
		counter++;
	}
	
	if (counter == 1) {
		Serial.println("Start");
		lastMacros = micros();
	}
	else if (counter < 10000) {
		sample = micros() - lastMacros;
		lastMacros = micros();
		if (maxMicros < sample) {
			maxMicros = sample;
		}
		else if(minMicros > sample) {
			minMicros = sample;
		}
	}
	else if (counter == 10000) {
		Serial.println("End");
		Serial.println(minMicros);
		Serial.println(maxMicros);
	}
}
