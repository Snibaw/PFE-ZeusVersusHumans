#include <ESP8266WiFi.h>

WiFiClient client;
constexpr int buffer_size = 32;

void force_connect() {
  while(!client.connect("192.168.4.2", 55555)) {
    Serial.println("Connection failed, retrying in 2.5 second");
    delay(2500);
  }
  Serial.println("Successfully connected !");
}

void setup() {
  Serial.begin(9600);
  pinMode(LED_BUILTIN, OUTPUT);

  // Start WiFi access point and connect to server.
  bool err = WiFi.softAP("ESP8266", "motdepassemin8");
  // In order to connect to a network rather than creating a new one,
  // comment the line above and uncomment the two next ones.
  // WiFi.mode(WIFI_STA);
  // wl_status_t err = WiFi.begin("nom_wifi", "mdpwifi");
  
  Serial.print("WiFi status : ");
  Serial.println(err);
  Serial.println("Waiting two seconds before trying to connect");
  delay(2000);
  force_connect();
}

void loop() {
  // Buffer for inputs/outputs.
  static char wifi_buffer[buffer_size];
  
  delay(200);
  digitalWrite(LED_BUILTIN, HIGH);
  if (!client.connected()) {
    Serial.println("Client disconnected, attempting to reconnect");
    force_connect();
  }
  client.println("High");
  
  delay(200);
  digitalWrite(LED_BUILTIN, LOW);
  if (!client.connected()) {
    Serial.println("Client disconnected, attempting to reconnect");
    force_connect();
  }
  client.println("Low");

  // If a message is available, handle it.
  while (client.available()) {
      int read_size = client.read(wifi_buffer, buffer_size);
      // Write only what was received. Prevents overflows and remnants
      // of previous messages.
      Serial.write(wifi_buffer, read_size);
  }
}
