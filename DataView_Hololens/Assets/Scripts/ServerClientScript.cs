using UnityEngine;
using System;
using System.IO;
using System.Text;

// Based on : http://wiki.unity3d.com/index.php/Simple_TCP/IP_Client_DLL_Code
// and: https://foxypanda.me/tcp-client-in-a-uwp-unity-app-on-hololens/

//This script runs the client side of the server. 
//Since for networking the hololens runs ons on a different codebase than unity this code had to be written differently for both. 
//The #if !UNITY_EDITOR and #if UNITY_EDITOR change between the two.

//Import threading for the hololens
#if !UNITY_EDITOR
using System.Threading.Tasks;  
#endif

//This script enables the connection to the server. And establishes the reading and writing stream.
public class Connector
{

//Set the boolean to check in which codebase it is running and import the specific packages
#if !UNITY_EDITOR
        private bool _useUWP = true;
        private Windows.Networking.Sockets.StreamSocket socket;
        private Task exchangeTask;
#endif

#if UNITY_EDITOR
    private bool _useUWP = false;
    System.Net.Sockets.TcpClient client;
#endif

    //Set the read buffer size and array. This is the maximal size recieving messages can be.
    private const int READ_BUFFER_SIZE = 1000;
    private byte[] readBuffer = new byte[READ_BUFFER_SIZE];
#if !UNITY_EDITOR
    private char[] readBufferUwp = new char[READ_BUFFER_SIZE];
#endif

    //In the strMessage the latest recieved information will be stored
    public string strMessage = "";

    //Create the streamReader and streamWriter
    private StreamReader reader;
    private StreamWriter writer;

    //This function start the connection process for the specific codebase
    public void Connect(string host, int port)
    {
        if (_useUWP)
        {
            ConnectUWP(host, port);
        }
        else
        {
            ConnectUnity(host, port);
        }
    }

#if UNITY_EDITOR
    //dummy function
    private void ConnectUWP(string host, int port)
    {
        Debug.Log("UWP TCP client used in Unity!");
    }

    //This one is used to make a connection
#else
    private async void ConnectUWP(string host, int port)
    {
        try
        {
            //Connect using streamSockets
            socket = new Windows.Networking.Sockets.StreamSocket();
            Windows.Networking.HostName serverHost = new Windows.Networking.HostName(host);
            await socket.ConnectAsync(serverHost, port.ToString());

            //Set up asynchronous reading
            Stream streamIn = socket.InputStream.AsStreamForRead();
            reader = new StreamReader(streamIn);
            
            //Make thread to read lines
            exchangeTask = Task.Run(() => DoReadUwp());

            Debug.Log("Connected! uwp");
        }
        catch (Exception e)
        {
            Debug.Log("uwp: " + e.ToString());
        }

    }
#endif

    //Connect using the Unity codebase
    private void ConnectUnity(string host, int port)
    {
#if !UNITY_EDITOR
        Debug.Log("Unity TCP client used in UWP!");
#else
        try
        {
            //Setup the connection
            client = new System.Net.Sockets.TcpClient(host, port);

            //Start recieving server information on a sepparete thread
            client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(DoReadUnity), null);

            Debug.Log("Connection Succeeded");
        }

        catch (Exception ex)
        {
            Debug.Log("Connection failed" + ex + "\n");
        }
#endif
    }

    //This function reads the information from the server on the hololens
#if !UNITY_EDITOR
    private void DoReadUwp(){
        try
        {
            //As long as it is connected keep reading information
            while (true){
                reader.Read(readBufferUwp,0, READ_BUFFER_SIZE);
                strMessage = new string(readBufferUwp);
            }    
        }
        catch
        {
            Debug.Log("Disconnected uwp");
        }
    }

#endif

    //This function reads the information from the unity editor
    private void DoReadUnity(IAsyncResult ar)
    {
#if !UNITY_EDITOR
        Debug.Log("Unity TCP client used in UWP!");
#else
        int BytesRead;
        try
        {
            // Finish asynchronous read into readBuffer and return number of bytes read.
            BytesRead = client.GetStream().EndRead(ar);
            if (BytesRead < 1)
            {
                // if no bytes were read server has close.  
                Debug.Log("Disconnected unity");
                return;
            }
            // Convert the byte array the message was saved into, minus two for the
            // Chr(13) and Chr(10)
            strMessage = Encoding.ASCII.GetString(readBuffer, 0, BytesRead);

            // Start a new asynchronous read into readBuffer.
            client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(DoReadUnity), null);

        }
        catch
        {
            Debug.Log("Disconnected uwp");
        }
#endif
    }

    //This function switches the writing of the data between the two codebases
    public void writeData(string data)
    {
        if (_useUWP)
        {
            writeDataUwp(data);
        }
        else
        {
            writeDataUnity(data);
        }
    }

    //This function asynchronously writes data from the hololens to the server
    private void writeDataUwp(String message)
    {
#if UNITY_EDITOR
        Debug.Log("Unity TCP client used in UWP!");
#else

        Stream outputStream = socket.OutputStream.AsStreamForWrite();
        var streamWriter = new StreamWriter(outputStream) {AutoFlush = true};
        streamWriter.WriteLineAsync(message);
#endif
    }

    //This function asynchronously writes data from the unity editor to the server
    private void writeDataUnity(String message)
    {
#if !UNITY_EDITOR
        Debug.Log("Unity TCP client used in UWP!");
#else
        
        var stream = client.GetStream();
        writer = new StreamWriter(stream);
        writer.WriteLine(message);
        stream.Flush();
        writer.Flush();
#endif
    }
}


public class ServerClientScript : MonoBehaviour
{
    // Set up the server information
    private Connector server = new Connector();
    private string ip = "192.168.1.227";
    private int port = 20000;

    // Create the strings for the individial information parts
    private String time = null;
    private String weatherStatus = null;
    private String weatherTemperature = null;
    private String weatherHumidity = null;
    private String musicTitle = null;
    private String musicTime = null;
    private String musicLength = null;
    private String musicPlaying = null;


    // A function that splits the server input and updates the information strings
    private void splitServerInput(String input)
    {
        //Split the seperate parts on ';'
        string[] info = input.Split(';');
        for (int i = 0; i < info.Length; i++)
        {
            //Split the single iformation parts on '#'
            string[] variable = info[i].Split('#');
            switch (variable[0])
            {
                case "time":
                    time = variable[1];
                    break;
                case "weatherStatus":
                    weatherStatus = variable[1];
                    break;
                case "weatherTemperature":
                    weatherTemperature = variable[1];
                    break;
                case "weatherHumidity":
                    weatherHumidity = variable[1];
                    break;
                case "musicTitle":
                    musicTitle = variable[1];
                    break;
                case "musicTime":
                    musicTime = variable[1];
                    break;
                case "musicLength":
                    musicLength = variable[1];
                    break;
                case "musicPlaying":
                    musicPlaying = variable[1];
                    break;
            }
        }
    }

    // This function is called from the outside to get information
    public String getString(String name)
    {
        switch(name)
        {
            case "time":
                return time;
            case "weatherStatus":
                return weatherStatus;
            case "weatherTemperature":
                return weatherTemperature;
            case "weatherHumidity":
                return weatherHumidity;
            case "musicTitle":
                return musicTitle;
            case "musicTime":
                return musicTime;
            case "musicLength":
                return musicLength;
            case "musicPlaying":
                return musicPlaying;
            case "fullString":
                return server.strMessage;
        }
        return "Error, no valid class name";
    }

    //Write a string to the server
    public void writeString(string message)
    {
        server.writeData(message);
        return;
    }

    // Use this for initialization
    void Start()
    {
        //Connect with server
        server.Connect(ip, port);
    }

    // Update is called once per frame
    void Update()
    {
        // Split and update the latest server information
        splitServerInput(server.strMessage);
    }
}
