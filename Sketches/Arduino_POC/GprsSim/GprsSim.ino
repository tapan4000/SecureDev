#include <Suli.h>
#include <GPRS_Shield_Arduino.h>
#include <SoftwareSerial.h>
#include <Wire.h>

#define PIN_RX    7
#define PIN_TX    8
#define MAXBUFFERSIZE 128
#define MAXSIMMESAGEDELETEBUFFER 20


// This program performs below tasks.
// 1) Read a newly received SMS (As soon as the message is received, display the message)
// 2) Send a new SMS (Take an input from the user in the format "SMS")
// 3) Make a call (Take an input from the user in the format "CALL")
// 4) Receive a call (Display message as call received)
// 5) Send a GET request to a URI (Receive input from the user as "GET". Show the response to the user)
// 6) Send a POST request to a URI (Receive input from the user as "POST". Show the post completed message to the user).

//make sure that the baud rate of SIM900 is 9600!
//you can use the AT Command(AT+IPR=9600) to set it through SerialDebug
#define BAUDRATE  9600
#define Array_Size(x) sizeof(x)/sizeof(x[0])

char http_cmd[] = "GET /media/uploads/mbed_official/hello.txt HTTP/1.0\r\n\r\n";
char buffer[MAXBUFFERSIZE];
int deleteSmsIndexBuffer[MAXSIMMESAGEDELETEBUFFER] = {99};
char* nullTerminatingString = "\0";
String expectedResponseSet[3];
int count = 0;
unsigned long currentMillis;
int callDurationInSeconds = 30;
int maxWaitTimeForSimResponseInSeconds = 10;
bool shouldWaitForCmtiPacketFromSim = false;
bool isCallInProgress = false;
bool shouldWaitForCmgrPacketFromSim = false;
bool shouldWaitForClipPacketFromSim = false;
//GPRS gprs(PIN_TX, PIN_RX, BAUDRATE);
SoftwareSerial SIM900(PIN_RX, PIN_TX);
char incomingChar = 0;
void setup() {
	Serial.begin(9600);
	SIM900.begin(9600);
	delay(100);
	Serial.println("Begin Setup.");

	//while (SIM900.available()) {
	//	buffer[count++] = SIM900.read();
	//	if (count == 64)
	//		break;
	//}

	//Serial.write(buffer, count);
	//clearBufferArray();
	//count = 0;

	/*char* mytext[] = { "hello", "make" };
	Serial.print("Size:");
	Serial.println(sizeof(mytext));*/
	Serial.println("AT validation begin");

	if (!sendSimCommandWithOkValidation("AT")) {
		return;
	}

	delay(100);
	Serial.println("AT validated");
	if (!sendSimCommandWithOkValidation("AT+CMGF=1")) {
		return;
	}

	delay(100);
	Serial.println("CMGF validated");
	//SIM900.print("AT+CNMI=2,2,0,0,0\r");
	//Serial.println("S3");
	//delay(100);

	if (!establistgprsConnection()) {
		return;
	}

	// Turns on the caller identification number.
	if (!sendSimCommandWithOkValidation("AT+CLIP=1")) {
		return;
	}

	//sendSimCommandWithoutValidation("AT+CMGR=23");
	//readSms(24);
	delay(100);
	Serial.println("Setup completed.");
}

void populateNullTerminatedExpectedResponseSet(char* element) {
	expectedResponseSet[0] = element;
	expectedResponseSet[1] = nullTerminatingString;
	expectedResponseSet[2] = nullTerminatingString;
}

void populateNullTerminatedExpectedResponseSet(char* element1, char* element2) {
	expectedResponseSet[0] = element1;
	expectedResponseSet[1] = element2;
	expectedResponseSet[2] = nullTerminatingString;
}

bool sendSimCommandWithOkValidation(char* command) {
	if (!sendFormattedCommand(command)) {
		return false;
	}

	delay(100);

	if (!validateOkResponse()) {
		Serial.print("\n");
		Serial.print("OK validation failed for command: ");
		Serial.println(command);
		return false;
	}

	return true;
}

bool sendSimCommandWithoutValidation(char* command) {
	if (!sendFormattedCommand(command)) {
		return false;
	}

	return true;
}

bool sendSimCommandWithResponseValidation(char* command, char* expectedResponse) {
	populateNullTerminatedExpectedResponseSet(expectedResponse);
	return sendSimCommandWithResponseValidation(command);
}

bool sendSimCommandWithResponseValidation(byte command, char* expectedResponse) {
	Serial.print("Sending:");
	Serial.write(command);
	SIM900.write(command);

	populateNullTerminatedExpectedResponseSet(expectedResponse);
	return validateSimResponse();

}

bool sendSimCommandWithResponseValidation(char* command) {
	if (!sendFormattedCommand(command)) {
		return false;
	}

	if (!validateSimResponse()) {
		/*Serial.print("\n");
		for (int i = 0; i < Array_Size(expectedResponseSet); i++) {
			Serial.print(expectedResponseSet[i]);
			if (i < Array_Size(expectedResponseSet) - 1) {
				Serial.print(',');
			}
		}
		
		Serial.print(" not received for command: ");
		Serial.println(command);*/
		return false;
	}

	return true;
}

bool sendFormattedCommand(String command) {
	int commandLength = command.length();
	if (commandLength == 0) {
		Serial.print("Empty Command Received(OK).");
		return false;
	}

	Serial.print("Command received:");

	for (int i = 0; i < commandLength; i++) {
		Serial.print(command[i]);
	}

	command.concat("\r");
	commandLength++;
	Serial.print("Sending:");
	for (int i = 0; i < commandLength; i++) {
		if (command[i] == '\r') {
			Serial.print("r");
		}
		else {
			Serial.print(command[i]);
		}
	}

	SIM900.print(command);
	return true;
}

bool establistgprsConnection() {
	// Establish connection using GPRS object.
	//while (0 != gprs.init()) {
	//	Serial.println("init error");
	//	delay(1000);
	//}

	//// attempt DHCP--fast.t-mobile.com
	//// data.lycamobile.com
	//while (!gprs.join(F("fast.t-mobile.com"))) {
	//	Serial.println("gprs join network error");
	//	delay(2000);
	//}

	//Serial.print("IP Address is ");
	//Serial.println(gprs.getIPAddress());

	
	// Establish connection using Sim software serail commands.

	populateNullTerminatedExpectedResponseSet("OK", "ERROR");
	if (!sendSimCommandWithResponseValidation("AT+HTTPINIT")) {
		return false;
	}

	delay(100);
	Serial.println("Sending PARA request");
	if (!sendSimCommandWithOkValidation("AT+HTTPPARA=\"CID\",1")) {
		return false;
	}

	Serial.println("PARA request complete");
	
	delay(100);
	return true;
}

bool validateOkResponse() {
	return validateSimResponse("OK");
}

bool validateSimResponse(char* validResponse) {
	populateNullTerminatedExpectedResponseSet(validResponse);
	return validateSimResponse();
}

bool validateSimResponse() {
	/*Serial.print("Inner Size:");
	Serial.println(sizeof(validResponseSet));*/
	int simIncomingCharCount = 0;	
	int totalDelayCompletedInSeconds = 0;
	int waitPeriodInSeconds = 1;
	// Keep waiting till the response is received from the sim.
	while (true) {
		Serial.println("Begin read Sim");
		simIncomingCharCount = readSim();
		if (simIncomingCharCount > 0) {
			Serial.println("Received from Sim");
			Serial.write(buffer, simIncomingCharCount);
			if (isBufferContains(simIncomingCharCount)) {
				// Once the response validation is complete clear the buffer and set the count to 0.
				count = 0;
				clearBufferArray();
				return true;
			}
		}
		else if (totalDelayCompletedInSeconds < maxWaitTimeForSimResponseInSeconds) {
			Serial.println("Enter delay");
			delay(waitPeriodInSeconds * 1000);
			totalDelayCompletedInSeconds += waitPeriodInSeconds;
		}
		else {
			Serial.println(totalDelayCompletedInSeconds);
			Serial.println(maxWaitTimeForSimResponseInSeconds);
			return false;
		}
	}

	return false;
}

int readSerial() {
	//count = 0;
	if (Serial.available() > 0) {
		Serial.println("Incoming Serial:");
		/*while (1) {
			int incomingCharacter = Serial.read();

			if (incomingCharacter == '\n') {
				break;
			}

			buffer[count++] = incomingCharacter;
			Serial.print((char)incomingCharacter);
			if (count == 64) {
				Serial.println("Reached max buffer count.");
				break;
			}
		}*/
		
		while (Serial.available() > 0) {
			char incomingCharacter = (char)Serial.read();
			buffer[count++] = incomingCharacter;
			Serial.print(incomingCharacter);
			if (count == MAXBUFFERSIZE) {
				Serial.println("Reached max buffer count.");
				break;
			}
			delay(1);
		}
	}
	else {
		return 0;
	}

	return count;
}

int readSim() {
	//count = 0;
	if (SIM900.available() > 0) {
		Serial.print("Count:");
		Serial.println(count);
		while (SIM900.available()) {
			int readValue = SIM900.read();
			//Serial.print((char)readValue);
			
			buffer[count++] = readValue;
			if (count == MAXBUFFERSIZE) {
				delay(10000);
				Serial.println("Reached max buffer count for Sim response.");
				break;
			}
		}
	}

	return count;
}

int getIndexOfSearchTextInBuffer(int bufferCount, String searchText) {

	for (int i = 0; i < bufferCount; i++)
	{
		if ((searchText[0] == buffer[i]) && ((i + searchText.length()) <= bufferCount)) {
			// If the first character matches and the length of remaining text can contain the input string. compare the strings.
			bool isStringFound = true;
			for (int j = i; j < i + searchText.length(); j++) {
				if (searchText[j - i] != buffer[j]) {
					isStringFound = false;
					break;
				}
			}

			if (isStringFound) {
				return i;
			}
		}
	}

	return -1;
}

void loop() {
	// Keep checking the serial port to determine if any command has been provided. The commands can be for sending SMS, making call, sending GET request, sending POST request.
	int serialIncomingCharCount = readSerial();
	if (serialIncomingCharCount > 0) {
		if (isBufferContains("SMS", serialIncomingCharCount)) {
			// Send an SMS
			sendSms();
		}
		else if (isBufferContains("CALL", serialIncomingCharCount)) {
			// Make a call.
			makeCall();
		}
		else if (isBufferContains("GET", serialIncomingCharCount)) {
			// Make a GET API call
			makeGetRequest();
		}
		else if (isBufferContains("POST", serialIncomingCharCount)) {
			// Make a POST API call
			makePostRequest();
		}

		count = 0;
		clearBufferArray();
	}

	if (isCallInProgress && ((millis()-currentMillis) > callDurationInSeconds*1000)) {
		// Hang up the call after 30 seconds.
		sendSimCommandWithOkValidation("ATH");
		Serial.println("Call Hang");
		isCallInProgress = false;
	}
	

	//delay(100);

	// Read the sim to check if any SMS or RING is incoming.
	int simIncomingCharCount = readSim();
	if (simIncomingCharCount > 0) {
		Serial.println("Received SIM Content: ");
		//delay(10);

		for (int i = 0; i < simIncomingCharCount; i++) {
			Serial.print(buffer[i]);
		}

		//delay(10);
		//Serial.print(buffer, simIncomingCharCount);
		//Serial.flush();

		// Check if an SMS has arrived in the SIM.
		int incomingMessageIndex = getIndexOfSearchTextInBuffer(simIncomingCharCount, "+CMTI");
		//Serial.println("Incoming index:");
		Serial.println(incomingMessageIndex);
		Serial.println(simIncomingCharCount);
		if ((incomingMessageIndex > -1) || shouldWaitForCmtiPacketFromSim) {
			if (simIncomingCharCount > (incomingMessageIndex + 13)) {
				// Read the SMS
				Serial.println("Received SMS. Attempt read.");

				Serial.print(buffer[incomingMessageIndex + 12]);
				String messageNumber = String(buffer[incomingMessageIndex + 12]);
				if (buffer[incomingMessageIndex + 13] != '\r') {
					Serial.print(buffer[incomingMessageIndex + 13]);
					messageNumber += buffer[incomingMessageIndex + 13];
				}
				else {
					Serial.print('r');
				}

				//Serial.print(messageNumber);
				readSms(messageNumber.toInt());
				addMessageToSimDeleteBuffer(messageNumber.toInt());
				shouldWaitForCmtiPacketFromSim = false;
				count = 0;
				clearBufferArray();
				//delay(1000);
			}
			else {
				Serial.println("Wait for CMTI packet");
				// Wait for the complete +CMTI packet to be received.
				shouldWaitForCmtiPacketFromSim = true;
			}
		}

		// Check if a received SMS has been read.
		incomingMessageIndex = getIndexOfSearchTextInBuffer(simIncomingCharCount, "+CMGR");
		//Serial.println("Incoming index:");
		//Serial.println(incomingMessageIndex);
		//Serial.println(simIncomingCharCount);
		if ((incomingMessageIndex > -1) || shouldWaitForCmgrPacketFromSim) {
			// Read the SMS
			if (isBufferContains("OK", simIncomingCharCount)) {
				if (isBufferContains("Hello", simIncomingCharCount)) {
					Serial.println("Hello found");
				}
				else{
					Serial.println("Hello not found");
				}

				shouldWaitForCmgrPacketFromSim = false;
				count = 0;
				clearBufferArray();
				deleteMessagesFromSimBuffer();
			}
			else {
				Serial.println("OK not found");
				shouldWaitForCmgrPacketFromSim = true;
			}
		}

		// Check if call has been received.
		if (isCallInProgress) {
			incomingMessageIndex = getIndexOfSearchTextInBuffer(simIncomingCharCount, "+CLIP:");
			if (incomingMessageIndex > -1 || shouldWaitForClipPacketFromSim) {
				if (isBufferContains("OK", simIncomingCharCount)) {
					Serial.println("Clearing the call content");
					count = 0;
					clearBufferArray();
					shouldWaitForClipPacketFromSim = false;
				}
				else {
					Serial.println("OK not found");
					shouldWaitForClipPacketFromSim = true;
				}
			}

			// Check if no carrier has been received in which case, turn off the call parameters
			incomingMessageIndex = getIndexOfSearchTextInBuffer(simIncomingCharCount, "NO CARRIER");
			if (incomingMessageIndex > -1) {
				Serial.println("No Carrier received.");
				isCallInProgress = false;
				// Clear the buffer
				count = 0;
				clearBufferArray();
				shouldWaitForClipPacketFromSim = false;
			}
		}
		
		// Check if a CALL has arrived in the SIM.
		if (!isCallInProgress && getIndexOfSearchTextInBuffer(simIncomingCharCount, "RING") > -1) {
			// Receive the call.
			receiveCall();
		}

		//clearBufferArray();
	}

	//if (SIM900.available() > 0) {
	//	Serial.println("Begin Read");
	//	while (SIM900.available()) {
	//		int readValue = SIM900.read();
	//		Serial.print((char)readValue);

	//		buffer[count++] = readValue;
	//		if (count == MAXBUFFERSIZE) {
	//			delay(10000);
	//			Serial.println("Reached max buffer count for Sim response.");
	//			break;
	//		}

	//		// Add a delay so that the incoming messages can be picked even if there is some delay in receiving remaining bytes.
	//		delay(10);
	//	}
	//	Serial.println("End Read");
	//}

	/*if (SIM900.available() > 0) {
		Serial.println("Begin Read");
		while (SIM900.available()) {
			buffer[count++] = SIM900.read();
			if (count == 128)
				break;
		}
		Serial.println("End Read");
	}*/
}

bool addMessageToSimDeleteBuffer(int indexForSimMessageToDelete) {
	for (int i = MAXSIMMESAGEDELETEBUFFER - 1; i >= 0; i--) {
		if (deleteSmsIndexBuffer[i] == 99)
		{
			if (i == MAXSIMMESAGEDELETEBUFFER) {
				Serial.println("Max SIM delete buffer reached.");
				return false;
			}

			// Shift the terminator one index right
			deleteSmsIndexBuffer[i + 1] = 99;
			deleteSmsIndexBuffer[i] = indexForSimMessageToDelete;
			return true;
		}
	}

	return false;
}

void deleteMessagesFromSimBuffer() {
	for (int i = MAXSIMMESAGEDELETEBUFFER; i >= 0; i--) {
		if (i>0 && deleteSmsIndexBuffer[i] == 99) {
			bool isDeleteSuccess = false;
			// delete the SMS whose index lies before terminator(99).
			if (deleteSmsIndexBuffer[i - 1] > 0) {
				if (deleteSms(deleteSmsIndexBuffer[i - 1])) {
					isDeleteSuccess = true;
				}
			}
			else {
				isDeleteSuccess = true;
			}

			if (isDeleteSuccess) {
				// Move the terminator one index lower.
				deleteSmsIndexBuffer[i] = 0;
				deleteSmsIndexBuffer[i-1] = 99;
			}

			break;
		}
	}
}

bool isBufferContains(char* textToCheck, int bufferLength) {
	populateNullTerminatedExpectedResponseSet(textToCheck);
	return isBufferContains(bufferLength);
}

bool isBufferContains(int bufferLength) {
	Serial.print("Buffer response set: ");
	int i = 0;
	while (expectedResponseSet[i] != nullTerminatingString)
	{
		//Serial.println("loop");
		/*if (sizeof(textSetToCheck[i]) == 0 || sizeof(textSetToCheck[i]) == 1 || sizeof(textSetToCheck[i]) == 2) {
			Serial.println("Nullterm");
			Serial.print(sizeof(textSetToCheck[i]));
			Serial.print((int)textSetToCheck[i][0]);
		}
		else {*/
			Serial.print(expectedResponseSet[i]);
		//}
		Serial.print(",");
		i++;
	}

	bool isMatchFound = false;
	/*if (textSetToCheck[0] == "ERROR") {
		Serial.println("Received Error. Next element:");
		Serial.println(textSetToCheck[1]);
	}*/
	/*Serial.print(Array_Size(textSetToCheck));
	Serial.print(sizeof(textSetToCheck));
	Serial.print(sizeof(textSetToCheck[0]));
	Serial.print(textSetToCheck[0]);*/
	
	i = 0;
	while(expectedResponseSet[i] != nullTerminatingString)
	{
		String textToCheck = expectedResponseSet[i];
		int searchTextLength = textToCheck.length();
		for (int i = 0; i < bufferLength; i++) {
			if (buffer[i] == textToCheck[0] && (i + searchTextLength <= bufferLength)) {
				isMatchFound = true;
				for (int j = 0; j < searchTextLength; j++) {
					if (buffer[i + j] != textToCheck[j]) {
						isMatchFound = false;
						break;
					}
				}

				if (isMatchFound) {
					Serial.print("Match found for:");
					Serial.println(textToCheck);
					break;
				}
			}
		}

		if (isMatchFound) {
			break;
		}
		else {
			Serial.print("Match not found for:");
			Serial.println(textToCheck);
		}

		i++;
	}

	return isMatchFound;
}

void clearBufferArray()
{
	for (int i = 0; i < count; i++) {
		buffer[i] = NULL;
	}
}

void sendSms() {
	Serial.println("Send SMS begin");
	// Write the message to the SIM memory
	if (!sendSimCommandWithOkValidation("AT+CMGF=1")) {
		return;
	}

	if (!sendSimCommandWithResponseValidation("AT+CPMS=\"SM\"", "+CPMS:")) {
		return;
	}

	sendSimCommandWithResponseValidation("AT+CMGS=\"+13132135019\"", "AT+CMGS=");
	sendSimCommandWithResponseValidation("Hello", "Hello");
	sendSimCommandWithResponseValidation(0x1a, "+CMGS:");
	//SIM900.write(0x1a);
	//SIM900.print("AT+CMGW");
	//delay(100);

	//// Read the sms id for the message written to memory.
	//int simIncomingCharCount = readSim();
	//if (simIncomingCharCount > 0) {
	//	Serial.println("Received SIM Content: ");
	//	Serial.write(buffer, simIncomingCharCount);

	//	int outgoingMessageIndex = getIndexOfSearchTextInBuffer(simIncomingCharCount, "+CMGW");
	//	if (outgoingMessageIndex > -1 && simIncomingCharCount > (outgoingMessageIndex + 7)) {
	//		// Write the SMS to the network
	//		int outgoingSmsId = buffer[outgoingMessageIndex + 7];
	//		String sendSmsToNetworkCommand = "AT+CMSS=" + outgoingSmsId;
	//		sendSmsToNetworkCommand += ",\"+13132135019\"";
	//		SIM900.print(sendSmsToNetworkCommand);
	//		Serial.println("SMS sent");
	//	}

	//	clearBufferArray();
	//}
	// Send the message to the network;

	Serial.println("Send SMS end");
}

void makeCall() {
	if (sendSimCommandWithOkValidation("ATD++13132135019;")) {
		Serial.println("Call sent");
		currentMillis = millis();
		isCallInProgress = true;
	}
	else {
		Serial.println("Failed to make the call.");
	}

	//// Hang up the call after 30 seconds.
	//sendSimCommandWithOkValidation("ATH");
	//Serial.println("Call Hang");
}

void makeGetRequest() {
	// Get request using GPRS object.
	/*if (false == gprs.connect(TCP, "mbed.org", 80)) {
		Serial.println("connect error");
	}
	else {
		Serial.println("connect mbed.org success");
	}

	Serial.println("waiting to fetch...");
	gprs.send(http_cmd, sizeof(http_cmd) - 1);
	while (true) {
		int ret = gprs.recv(buffer, sizeof(buffer) - 1);
		if (ret <= 0) {
			Serial.println("fetch over...");
			break;
		}
		buffer[ret] = '\0';
		Serial.print("Recv: ");
		Serial.print(ret);
		Serial.print(" bytes: ");
		Serial.println(buffer);
	}
	gprs.close();
	gprs.disconnect();*/

	
	// Get request using SIM commands.
	if (!sendSimCommandWithOkValidation("AT+HTTPPARA=\"URL\",\"http://www.iforce2d.net/test.php\"")) {
		return;
	}

	if (!sendSimCommandWithOkValidation("ATP+SAPBR=3,1,\"APN\",\"fast.t-mobile.com\"")) {
		return;
	}

	populateNullTerminatedExpectedResponseSet("OK", "ERROR");
	if (!sendSimCommandWithResponseValidation("AT+SAPBR=1,1")) {
		return;
	}

	if (!sendSimCommandWithResponseValidation("AT+HTTPACTION=0", "+HTTPACTION:0,200")) {
		return;
	}

	Serial.println("Begin Read");

	if (!sendSimCommandWithResponseValidation("AT+HTTPREAD=0,10", "OK")) {
		return;
	}
}

void makePostRequest() {
	// This can be implemented later.
}

void readSms(int smsId) {
	// Read the SMS at specified id.
	String smsString = "AT+CMGR=";
	smsString += smsId;
	Serial.println("Reading:");
	Serial.println(smsString);
	char commandBuffer[11];
	smsString.toCharArray(commandBuffer, 11);
	Serial.print("--");
	Serial.print(commandBuffer);
	Serial.print("--");
	sendSimCommandWithoutValidation(commandBuffer);
}

bool deleteSms(int smsId) {
	// Read the SMS at specified id.
	String smsString = "AT+CMGD=";
	smsString += smsId;
	char commandBuffer[11];
	smsString.toCharArray(commandBuffer, 11);
	return sendSimCommandWithOkValidation(commandBuffer);
}

void receiveCall() {
	// Receive the call.
	sendSimCommandWithoutValidation("ATA");
	currentMillis = millis();
	//SIM900.print("ATA");
	Serial.println("Call received");
	isCallInProgress = true;
	//delay(30000);

	// Hang up the call after 30 seconds.
	//sendSimCommandWithOkValidation("ATH");
	//SIM900.print("ATH");
	//Serial.println("Call Hang");
}

//
//// Code to send commands to the sim using serial monitor.
//unsigned char buffer[128];
//int count = 0;
//SoftwareSerial SIM900(PIN_RX, PIN_TX);
//char incomingChar = 0;
//void setup() {
//	Serial.begin(9600);
//	SIM900.begin(9600);
//	Serial.println("Setup complete");
//}
//
//void loop() {
//	/*if (Serial.available() > 0) {
//		incomingChar = Serial.read();
//		Serial.print("1");
//		Serial.write(incomingChar);
//		Serial.print("2");
//		SIM900.print(incomingChar);
//	}*/
//
//	if (SIM900.available() > 0) {
//		Serial.println("Begin Read");
//		while (SIM900.available()) {
//			buffer[count++] = SIM900.read();
//			if (count == 128)
//				break;
//		}
//		Serial.println("End Read");
//	}
//	
//	delay(100);
//	Serial.write(buffer, count);
//	clearBufferArray();
//	count = 0;
//
//	if (Serial.available())
//	{
//		byte b = Serial.read();
//		if (b == '*') {
//			SIM900.write(0x1a);
//		}
//		else if (b == '#') {
//			SIM900.print("AT\r");
//		}
//		else {
//			SIM900.write(b);
//		}
//	}
//}
//
//void clearBufferArray()
//{
//	for (int i = 0; i < count; i++) {
//		buffer[i] = NULL;
//	}
//}
