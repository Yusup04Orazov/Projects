# Using the HoloLens 2 to communicate with Arduino MKR WiFi 1010
This section focuses on elucidating the intricacies of establishing communication between the HoloLens 2, a cutting-edge mixed reality device, and a WiFi-enabled Arduino Module. Leveraging the HoloLens 2's capabilities, users gain the ability to engage with augmented reality elements that serve as triggers for tangible, real-world events. This is achieved by transmitting a WiFi signal from the HoloLens 2 to the Arduino, a versatile microcontroller platform. Subsequently, the Arduino adeptly emits an electronic analog signal, enabling seamless interaction with an extensive range of electronic components, circuits, and devices.

## Tools
Navigating to the **Assets --> Scripts --> Arduino** folder, we find all the scripts and sketches we need that allow the HoloLens 2 to communicate with the Arduino:
- ArduinoCommunication
- ipaddress sketch
- secrets
- sketch_jun5a

### ArduinoCommunication
This code is responsible for establishing communication between a Unity application and an Arduino device over a TCP/IP connection. It allows the Unity application to send signals or commands to the Arduino for various purposes.

The code utilizes the **TcpClient** class from the **System.Net.Sockets** namespace to establish a TCP/IP connection with the Arduino. It relies on the IP address and port number to connect to the Arduino device. The IP address should be set to the current IP address of the Arduino(**details below**), which might change daily. The port number is the specific port on which the Arduino is listening for incoming connections.

The **SendSignal** method is called when the Unity application wants to send a signal to the Arduino. It first checks if the connection with the Arduino is established (**isConnected**). If not, it asynchronously calls the **ConnectToArduino** method to establish the connection.

If the connection is successful, the method writes the string "activate" to the **StreamWriter** instance, which is responsible for sending data over the network. The **Flush** method ensures that the data is sent immediately.

In case of any errors during the connection or signal sending process, appropriate error messages are logged and displayed using the **UserAlert** script attached to the **alertObject** game object. The **displayMessage** method of the **UserAlert** script is called to show the error message to the user.

The **ConnectToArduino** method attempts to connect to the Arduino by creating a new **TcpClient** instance and calling the **ConnectAsync** method with the Arduino's IP address and port number. If the connection is successful, a **NetworkStream** is obtained from the client, and a **StreamWriter** is created to write data to the stream. The **isConnected** flag is set to true to indicate a successful connection.

The **OnDestroy** method is responsible for closing the writer and client connection when the Unity application is being destroyed to ensure proper cleanup.

**IMPORTANT**
Note that the IP address of the Arduino will change each time it is disconnected from a power source, or each day it re-connects to the WiFi network. As such, you will need to upload the **ipaddress sketch** to the Arduino and get the IP address from the Serial Output in the Arduino IDE. Once you have the new IP address, simply change the address in the **ipAddress** field.

**Attach this script to a button**

### ipAddress sketch
The code begins by including the necessary libraries, including "WiFiNINA.h" and "secrets.h" which contains sensitive data such as the network SSID and password.

In the **setup()** function, the serial communication is initialized, and the code attempts to connect to the WiFi network using the provided SSID and password. It continuously attempts to connect until a successful connection is established. Once connected, it prints a message indicating the successful connection and calls the **printData()** function to display network information such as the board's IP address, SSID, signal strength (RSSI), and encryption type.

The **loop()** function is responsible for periodically checking the network connection and printing the network information every 10 seconds. It introduces a delay of 10 seconds using the **delay()** function to control the frequency of network checks and information printing.

The **printData()** function is responsible for printing the network information. It retrieves the local IP address, SSID, signal strength (RSSI), and encryption type using various functions provided by the WiFiNINA library, and then prints the retrieved information to the serial monitor.

Overall, this code enables the Arduino board to connect to a WiFi network, retrieve network information, and display it through the serial monitor. It can be useful for monitoring the network status and obtaining network-related data for further processing or analysis.

**Upload this script to a WiFi enabled Arduino Board each day and update the IP Address field in the ArduinoCommunication script with the IP Address acquired from the Serial Output**

### secrets
The header file contains two defined constants: **SECRET_SSID** and **SECRET_PASS**. These constants store the WiFi network name (SSID)
and password (PASS) in a secure manner. By storing the network name and password in a separate header file like this, you can keep this sensitive information separate from the main code.
This practice helps protect the credentials from being accidentally shared or exposed when sharing or publishing the code.

You can replace both the **SECRET_SSID** and **SECRET_PASS** variables with the name of your network and the actual password to enable the Arduino board to establish a connection.

### sketch_jun5a
This Arduino code facilitates the communication between an Arduino board equipped with a WiFi module and a client device connected over WiFi. The code establishes a WiFi connection by providing the appropriate SSID and password (network name and password specified in **secrets.h**). Once connected, it initializes a WiFi server that listens for incoming client connections on port 1234.

In the **loop()** function, the code continuously checks for client connections and, if a connection is established, it proceeds to handle the communication. When a message is received from the client, it is read and printed to the Serial monitor for debugging purposes. Additionally, the code prints a message indicating that it will initiate the servo motor rotation.

The **activateMotor()** function is responsible for controlling the servo motor. It uses a for-loop to rotate the motor from 0 to 180 degrees in increments of 1, with a slight delay of 15 milliseconds between each increment. After reaching 180 degrees, it reverses the rotation by decrementing from 179 to 0, again with a delay between each step. This action creates a back-and-forth sweeping motion of the servo motor.

Overall, this code allows a client device to trigger the servo motor movement via WiFi by sending a specific message to the Arduino, resulting in the physical rotation of the servo motor.

**After getting the new IP Address, upload this sketch to the Arduino. Keep an eye on the serial output during Unity Emulator, or HoloLens 2 App runtime for flags**
