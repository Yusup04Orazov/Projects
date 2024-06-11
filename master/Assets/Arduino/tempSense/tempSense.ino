// #include <Wire.h>
// #include <SPI.h>
// #include <Adafruit_Sensor.h>
// #include <Adafruit_BME280.h>
// #include <WiFiNINA.h>
// #include "secrets.h" // Include the secrets header file
// #include <U8x8lib.h> // https://github.com/olikraus/u8glib

// #define BME_SCK 13
// #define BME_MISO 12
// #define BME_MOSI 11
// #define BME_CS 10

// #define SEALEVELPRESSURE_HPA (1013.25)

// Adafruit_BME280 bme; // I2C
// WiFiServer server(8888); // TCP server on port 8888

// // Network credentials from secrets.h
// char ssid[] = SECRET_SSID;
// char pass[] = SECRET_PASS;

// unsigned long delayTime;

// // Objects
// U8X8_SH1106_128X64_NONAME_HW_I2C u8x8(U8X8_PIN_NONE);

// void setup() {
//     Serial.begin(9600);
//     Serial.println(F("BME280 test"));

//     // Init OLED screen
//     u8x8.begin();
//     u8x8.setFont(u8x8_font_chroma48medium8_r);
//     u8x8.setFlipMode(0);

//     Serial.println("OLED begun");

//     // Initialize Wi-Fi
//     if (WiFi.status() == WL_NO_MODULE) {
//         u8x8.drawString(0, 0, "WiFi Failed");
//         while (true);
//     }

//     // Attempt to connect to WiFi network using secrets
//     while (WiFi.status() != WL_CONNECTED) {
//         u8x8.drawString(0, 0, "Connecting to: ");
//         u8x8.drawString(0, 1, ssid);
//         WiFi.begin(ssid, pass);
//         delay(5000); // Wait 5 seconds for connection
//     }

//     u8x8.clearLine(0);
//     u8x8.drawString(0, 0, "Connected to");

//     // Display the IP address on OLED
//     IPAddress ip = WiFi.localIP();
//     Serial.print("Server IP Address: ");
//     Serial.println(ip);
//     u8x8.drawString(0, 2, "IP Address:");
//     u8x8.drawString(0, 3, ip.toString().c_str());

//     // Start the server
//     server.begin();
//     Serial.println("Server started");

//     // default settings
//     if (!bme.begin()) {
//         Serial.println("Could not find a valid BME280 sensor, check wiring, address, sensor ID!");
//         while (true);
//     }

//     Serial.println("-- Default Test --");
//     delayTime = 2000;

//     Serial.println();
// }

// void loop() {
//     // Check for incoming clients
//     WiFiClient client = server.available();

//     //Read and broadcast sensor data if there is a client
//     if (client) {
//         printValues(client);
//         delay(delayTime);
//     }
// }

// void printValues(WiFiClient& client) {
//     // Read temperature and humidity
//     float temperature = bme.readTemperature();
//     float humidity = bme.readHumidity();

//     // Round temperature and humidity to two decimal places
//     temperature = round(temperature * 100.0) / 100.0;
//     humidity = round(humidity * 100.0) / 100.0;

//     // Debug printing
//     Serial.print("Temperature = ");
//     Serial.print(temperature);
//     Serial.println(" °C");
//     Serial.print("Humidity = ");
//     Serial.print(humidity);
//     Serial.println(" %");

//     // Display temperature
//     u8x8.clearLine(5);
//     u8x8.drawString(0, 5, "Temp = ");
//     u8x8.setCursor(7, 5); // Set cursor position after "=" in "Temp ="
//     u8x8.print(temperature);
//     u8x8.drawString(13, 5, "C");

//     // Display humidity
//     u8x8.clearLine(6);
//     u8x8.drawString(0, 6, "Humidity = ");
//     u8x8.setCursor(10, 6); // Set cursor position after "=" in "Humidity ="
//     u8x8.print(humidity);
//     u8x8.drawString(14, 6, "%");

//     // Send data to client
//     client.print("Temperature: ");
//     client.print(temperature);
//     client.println(" C");
//     client.print("Humidity: ");
//     client.print(humidity);
//     client.println(" %");
// }
