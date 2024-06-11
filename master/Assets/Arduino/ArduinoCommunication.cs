using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class ArduinoCommunication : MonoBehaviour
{
    private const string ipAddress = "10.65.238.0"; // Arduino IP address (changes daily)
    private const int port = 1234; // Specify the port number

    private UdpClient udpClient;
    private IPEndPoint endPoint;
    private bool isConnected = false;

    private void Start()
    {
        udpClient = new UdpClient();
        endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
    }

    public void Update()
    {
        if (Input.GetKeyUp("a"))
        {
            SendSignal();
        }
    }

    public void SendSignal()
    {
        if (!isConnected)
        {
            ConnectToArduino();
        }

        if (isConnected)
        {
            try
            {
                string message = "activate";
                byte[] data = Encoding.ASCII.GetBytes(message);
                udpClient.Send(data, data.Length, endPoint);
            }
            catch (Exception e)
            {
                Debug.LogError("Error sending signal: " + e.Message);
            }
        }
    }

    private void ConnectToArduino()
    {
        isConnected = true;
        Debug.Log("Connected to Arduino");
    }

    private void OnDestroy()
    {
        if (isConnected)
        {
            udpClient.Close();
        }
    }
}
