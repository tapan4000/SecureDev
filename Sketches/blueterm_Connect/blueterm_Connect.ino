#include <SoftwareSerial.h>
int state = 0;
int flag = 0;
int ledPin = 13;
SoftwareSerial moduleSerial(0, 1); //RX, TX
void setup() {
  // put your setup code here, to run once:
  pinMode(ledPin, OUTPUT);
  digitalWrite(ledPin, LOW);
  Serial.begin(9600);
  moduleSerial.begin(9600);
  delay(1000);
  moduleSerial.print("AT");
  delay(1000);
  moduleSerial.print("AT+Version");
  delay(1000);
  //moduleSerial.print("AT+NameHC-06");
  //delay(1000);
  //moduleSerial.print("AT+PIN1111");
  //moduleSerial.print("AT+BAUD7");// Set baud rate to 57600
}

void loop() {
  // put your main code here, to run repeatedly:
  if(Serial.available() > 0)
  {
    state = Serial.read();
    flag = 0;
  }

  if(state == '0')
  {
    digitalWrite(ledPin, LOW);
    if(flag == 0){
      Serial.print("Received a 0");
      Serial.println("LED: off");
      moduleSerial.print("AT");
      flag = 1;
    }
  }
  else if (state == '1')
  {
    digitalWrite(ledPin, HIGH);
    if(flag == 0){
      Serial.print("It's a 1");
      Serial.println("LED: on");
      flag = 1;
    }
  }
}
