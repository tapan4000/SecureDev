#include "Math.h"
int value = 34;
int ledPin = 13;
int buttonSwitch = 10;
void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  Serial.println("Hello Tapan!");
  pinMode(ledPin, OUTPUT);
  pinMode(buttonSwitch, INPUT);
  digitalWrite(ledPin, LOW);
}

void loop() {
  // put your main code here, to run repeatedly:
  float myFloat = value;
  Serial.print("Sqrt: ");
  Serial.println(sqrt(myFloat));
  myFloat = --value;
  Serial.print("Button Value: ");
  Serial.println(digitalRead(buttonSwitch));
  delay(1000);
  
}
