#include <SPI.h>
#include <WiFiNINA.h>
#include <U8x8lib.h>
#include "secrets.h"

// Network credentials from secrets.h
char ssid[] = SECRET_SSID;
char pass[] = SECRET_PASS;

// TCP settings
WiFiClient client;
const char serverAddress[] = "10.65.191.176"; // Replace with the server's IP address
const unsigned int serverPort = 8888; // The server's port

// OLED display
U8X8_SH1106_128X64_NONAME_HW_I2C u8x8(U8X8_PIN_NONE);

void setup() {
    Serial.begin(9600);
    while (!Serial) {
        ; // Wait for serial port to connect. Needed for native USB port only
    }

    // Initialize OLED display
    u8x8.begin();
    u8x8.setFont(u8x8_font_chroma48medium8_r);
    u8x8.setFlipMode(0);

    // Connect to WiFi network
    WiFi.begin(ssid, pass);
    while (WiFi.status() != WL_CONNECTED) {
        u8x8.drawString(0, 0, "Connecting to: ");
        u8x8.drawString(0, 1, ssid);
        delay(500);
        Serial.print(".");
    }

    // Display WiFi connection status
    if (WiFi.status() == WL_CONNECTED) {
        u8x8.clear();
        u8x8.drawString(0, 0, "WiFi connected");
        u8x8.drawString(0, 2, "IP Address:");
        u8x8.drawString(0, 3, WiFi.localIP().toString().c_str());
        Serial.print("Client IP Address: ");
        Serial.println(WiFi.localIP());
    } else {
        u8x8.clear();
        u8x8.drawString(0, 0, "WiFi disconnected");
        Serial.println("WiFi disconnected");
        while (true); // Halt execution if WiFi connection fails
    }

    // Connect to the TCP server
    if (client.connect(serverAddress, serverPort)) {
        Serial.println("Connected to server");
        u8x8.drawString(0, 5, "Connected to");
        u8x8.drawString(0, 6, "server");
    } else {
        Serial.println("Connection to server failed");
        u8x8.drawString(0, 5, "Conn. failed");
        while (true); // Halt execution if connection fails
    }
}

void loop() {
    // Check if connected to server
    if (client.connected()) {
        if (client.available()) {
            byte data[sizeof(float)];
            client.readBytes(data, sizeof(float));
            float temperature;
            memcpy(&temperature, data, sizeof(float));

            // Round to two decimal places
            temperature = round(temperature * 100.0) / 100.0;

            // Display temperature
            u8x8.clearLine(5);
            u8x8.drawString(0, 5, "Temp= ");
            u8x8.setCursor(7, 5); // Set cursor position after "=" in "Temp ="
            u8x8.print(temperature);
            u8x8.drawString(12, 5, "C");

            Serial.print("Temperature: ");
            Serial.println(temperature);
        }
    } else {
        u8x8.drawString(0, 5, "Disconnected");
        Serial.println("Disconnected from server");
        // Try to reconnect to the server
        if (client.connect(serverAddress, serverPort)) {
            Serial.println("Reconnected to server");
            u8x8.clearLine(5);
            u8x8.drawString(0, 5, "Reconnected");
        } else {
            Serial.println("Reconnection to server failed");
            u8x8.drawString(0, 6, "Reconn. failed");
        }
    }

    delay(1000); // Delay to avoid flooding the display
}
