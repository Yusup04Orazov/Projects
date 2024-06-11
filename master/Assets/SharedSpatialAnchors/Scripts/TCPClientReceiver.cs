using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPClientReceiver : MonoBehaviour
{
    public string serverIp = "10.65.38.218"; // Replace with Arduino's IP address
    public int serverPort = 8888; // Arduino server port
    public UserAlert userAlert; // Reference to the UserAlert script

    private TcpClient client;
    private NetworkStream stream;
    private Thread clientThread;

    private void Start()
    {
        userAlert.displayMessage("Starting TCPClientReceiver...");
        ConnectToServer();
    }

    private void ConnectToServer()
    {
        try
        {
            userAlert.displayMessage($"Attempting to connect to server at {serverIp}:{serverPort}...");
            client = new TcpClient(serverIp, serverPort);
            stream = client.GetStream();
            clientThread = new Thread(new ThreadStart(ListenForData));
            clientThread.IsBackground = true;
            clientThread.Start();
            userAlert.displayMessage("Connected to server.");
        }
        catch (Exception e)
        {
            userAlert.displayMessage("Failed to connect to server: " + e.Message);
        }
    }

    private void ListenForData()
    {
        byte[] buffer = new byte[1024];
        while (true)
        {
            try
            {
                if (stream.CanRead)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        userAlert.displayMessage("Data received: " + dataReceived);
                        UpdateTemperatureDisplay(dataReceived);
                    }
                }
            }
            catch (Exception e)
            {
                userAlert.displayMessage("Error while reading data: " + e.Message);
                break;
            }
            Thread.Sleep(1000); // To avoid busy-waiting, adjust as needed
        }
    }

    private void UpdateTemperatureDisplay(string data)
    {
        float temperature;
        if (float.TryParse(data, out temperature))
        {
            string message = "Temperature: " + temperature.ToString("F2") + " °C";
            userAlert.displayMessage(message);
        }
        else
        {
            userAlert.displayMessage("Invalid data received: " + data);
        }
    }

    private void OnApplicationQuit()
    {
        CloseConnection();
    }

    private void OnDestroy()
    {
        CloseConnection();
    }

    private void CloseConnection()
    {
        if (clientThread != null)
        {
            clientThread.Abort();
        }

        if (stream != null)
        {
            stream.Close();
        }

        if (client != null)
        {
            client.Close();
        }

        userAlert.displayMessage("TCP Client closed");
    }
}
