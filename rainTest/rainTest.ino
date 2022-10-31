#include "Ardunity.h"
#include "AnalogInput.h"

AnalogInput aInput0(0, A1);

void setup()
{
//  ArdunityApp.attachController((ArdunityController*)&aInput0);
//  ArdunityApp.resolution(256, 1024);
//  ArdunityApp.timeout(5000);
//  ArdunityApp.begin(115200);
  pinMode(3,INPUT);
  pinMode(13,OUTPUT);
  Serial.begin(9600);
}

void loop()
{
//  ArdunityApp.process();
  bool I=digitalRead(3);
 // Serial.print("*");
  if (I) 
  {
    digitalWrite(13,LOW);
    Serial.print("n");
    }
  else 
  {
    digitalWrite(13,HIGH);
    Serial.print("y");
   }
   //Serial.print("#");
   delay(500);
}
