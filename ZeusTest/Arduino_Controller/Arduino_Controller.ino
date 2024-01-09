//Libraries
#include <Wire.h>//https://www.arduino.cc/en/reference/wire
#include <Adafruit_MPU6050.h>//https://github.com/adafruit/Adafruit_MPU6050
#include <Adafruit_Sensor.h>//https://github.com/adafruit/Adafruit_Sensor
#include "LiquidCrystal.h" // ajout de la librairie


LiquidCrystal lcd(41,40,39,38,37,36,35,34,33,32); // liaison 8 bits de données
Adafruit_MPU6050 mpu;

// // The flag signals to the rest of the program an interrupt occured
static bool button_flag = false;
// // Remember the state the river in the Unity program is in
// static bool river_state = true;
static int xValue = 0;
static int yValue = 0;

byte BlackCharacter[8] = {
	0b11111,
	0b11111,
	0b11111,
	0b11111,
	0b11111,
	0b11111,
	0b11111,
	0b11111
};

byte WhiteCharacter[8] = {
	0b11111,
	0b10001,
	0b10001,
	0b10001,
	0b10001,
	0b10001,
	0b10001,
	0b11111
};



// Interrupt handler, sets the flag for later processing
void buttonPress() {
  button_flag = !button_flag;
}

void setup() {
  int buttonPin = 2;

  lcd.begin(16,2); // utilisation d'un écran 16 colonnes et 2 lignes
  lcd.write("Ready to be used");
  lcd.createChar(0, WhiteCharacter);
  lcd.createChar(1, BlackCharacter);
  
  pinMode(LED_BUILTIN, OUTPUT);
  // Internal pullup, no external resistor necessary
  pinMode(buttonPin,INPUT_PULLUP);
  pinMode(7,OUTPUT);
  pinMode(8,OUTPUT);
  pinMode(A0,INPUT);
  pinMode(A1,INPUT);
  // 115200 is a common baudrate : fast without being overwhelming
  Serial.begin(115200);

  // As the button is in pullup, detect a connection to ground
  attachInterrupt(digitalPinToInterrupt(buttonPin),buttonPress,CHANGE);


  //Init Serial USB
  Serial.println(F("Initialize System"));
  if (!mpu.begin(0x68)) { // Change address if needed
    Serial.println("Failed to find MPU6050 chip");
    while (1) {
      delay(10);
    }
  }

  mpu.setAccelerometerRange(MPU6050_RANGE_16_G);
  mpu.setGyroRange(MPU6050_RANGE_250_DEG);
  mpu.setFilterBandwidth(MPU6050_BAND_21_HZ);

  // Wait for a serial connection
  while (!Serial.availableForWrite());

 
}

// Processes button input
void loop() {
  // Slows reaction down a bit
  // but prevents _most_ button press misdetections
  delay(200);
  

  

  xValue = analogRead(A0);
  yValue = analogRead(A1);
  Serial.println("Joystick");
  Serial.print("x");
  Serial.println(xValue);
  Serial.print("y");
  Serial.println(yValue);
  Serial.print("b");
  Serial.println(button_flag);

    
  // if (button_flag) {
  //   button_flag = false;
  // }

  readMPU();
}

// Handles incoming messages
// Called by Arduino if any serial data has been received
void serialEvent()
{
  String message = Serial.readStringUntil('\n');
  if (message == "LED ON") {
    digitalWrite(7,LOW);
    digitalWrite(8,HIGH);
    //lcd.clear();
    //lcd.write("LED ON");
  } else if (message == "LED OFF") {
    digitalWrite(7,HIGH);
    digitalWrite(8,LOW);
    //lcd.clear();
    //lcd.write("LED OFF");
  }else{
    //ToDo Substring ;
    int index = message.lastIndexOf(';');
    int length = message.length();
    String score = message.substring(0,index-1);
    String barNb = message.substring(index+1,length);
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.write("score = ");
    lcd.write(score.c_str());
    lcd.setCursor(0, 1);
    int barNbWhiteCharacter = barNb.toInt()+1;
    int barNbBlackCharacter = 16-barNbWhiteCharacter;
    for(int i = 0; i < 16; i++){
      if(i <= barNbBlackCharacter){
        lcd.write(byte(1));
      }else{
        lcd.write(byte(0));
      }
    }
    
  }
}



void readMPU( ) { /* function readMPU */
  ////Read acceleromter data
  sensors_event_t a, g, temp;
  mpu.getEvent(&a, &g, &temp);

  /* Print out the values */
  Serial.println("Accelerometre");
  Serial.print("X");
  Serial.println(a.acceleration.x);
  Serial.print("Y");
  Serial.println(a.acceleration.y);
  Serial.print("Z");
  Serial.println(a.acceleration.z);
  //Serial.println(" m/s^2");

/*
  Serial.print("Rotation X: ");
  Serial.print(g.gyro.x);
  Serial.print(", Y: ");
  Serial.print(g.gyro.y);
  Serial.print(", Z: ");
  Serial.print(g.gyro.z);
  Serial.println(" rad/s");

  Serial.print("Temperature: ");
  Serial.print(temp.temperature);
  Serial.println("°C");
  */
}
