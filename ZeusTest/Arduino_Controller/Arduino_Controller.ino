// The flag signals to the rest of the program an interrupt occured
static bool button_flag = false;
// Remember the state the river in the Unity program is in
static bool river_state = true;
static int xValue = 0;
static int yValue = 0;

// Interrupt handler, sets the flag for later processing
void buttonPress() {
  button_flag = true;
}

void setup() {
  int buttonPin = 2;
  
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
  attachInterrupt(digitalPinToInterrupt(buttonPin),buttonPress,FALLING);


  // Wait for a serial connection
  while (!Serial.availableForWrite());
  // In case the Unity project isn't synced with the boolean.
  Serial.println("wet");
}

// Processes button input
void loop() {
  // Slows reaction down a bit
  // but prevents _most_ button press misdetections
  delay(200);
  
  if (button_flag) {
    if (river_state) {
      Serial.println("dry");
    } else {
      Serial.println("wet");
    }
    river_state = !river_state;
    button_flag = false;
  }

  xValue = analogRead(A0);
  yValue = analogRead(A1);
  Serial.print("x");
  Serial.println(xValue);
  Serial.print("y");
  Serial.println(yValue);
}

// Handles incoming messages
// Called by Arduino if any serial data has been received
void serialEvent()
{
  String message = Serial.readStringUntil('\n');
  if (message == "LED ON") {
    digitalWrite(7,LOW);
    digitalWrite(8,HIGH);
  } else if (message == "LED OFF") {
    digitalWrite(7,HIGH);
    digitalWrite(8,LOW);
  }
}
