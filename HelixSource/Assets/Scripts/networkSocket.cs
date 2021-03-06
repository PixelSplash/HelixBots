using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class networkSocket : MonoBehaviour
{
    public String host = "localhost";
    public Int32 port = 50000;

    internal Boolean socket_ready = false;
    internal String input_buffer = "";
    TcpClient tcp_socket;
    NetworkStream net_stream;

    StreamWriter socket_writer;
    StreamReader socket_reader;

    internal string received_data;
    internal string last_received_data;
    internal bool dataReady;
    internal string key_stroke = null;

    private void Start()
    {
        // StartCoroutine(Server());
        Debug.Log("Start");
        // Create a new Thread
        new Thread(() =>
        {
            while (true)
            {
                if (key_stroke != null)
                {
                    writeSocket(key_stroke);
                }
                
                received_data = readSocket();
                
                
                //Debug.Log(System.Text.Encoding.UTF8.GetString(received_data));
                if (received_data != "")
                {
                    last_received_data = received_data;
                    dataReady = true;
                }
            }
        }).Start(); // Start the Thread

    }

    private IEnumerator Server()
    {

        yield return null;
        //Collects keystrokes into a buffer
        while (true)
        {
            if (key_stroke != null)
            {
                writeSocket(key_stroke);
            }
            received_data = readSocket();
            Debug.Log(received_data);
            if (received_data != "")
            {
                Debug.Log(received_data);
                dataReady = true;
            }
        }
       
        
        
    }

    void Update()
    {
        

        
    }

    

    public void sendData(string str)
    {
        dataReady = false;
        key_stroke = str;
    }

    public string getData()
    {
        if (dataReady)
        {

            return last_received_data;
        }

        return "";
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        setupSocket();
    }

    void OnApplicationQuit()
    {
        closeSocket();
    }

    public void setupSocket()
    {
        try
        {
            tcp_socket = new TcpClient(host, port);

            net_stream = tcp_socket.GetStream();
            socket_writer = new StreamWriter(net_stream);
            socket_reader = new StreamReader(net_stream);

            socket_ready = true;
        }
        catch (Exception e)
        {
        	// Something went wrong
            Debug.Log("Socket error: " + e);
        }
    }

    public void writeSocket(string line)
    {
        if (!socket_ready)
            return;
          
        socket_writer.Write(line);
        socket_writer.Flush();
        key_stroke = null;
    }

    public String readSocket()
    {
        if (!socket_ready)
            return "";
     
        if (net_stream.CanRead) {
   
            byte[] myReadBuffer = new byte[1024];
            StringBuilder myCompleteMessage = new StringBuilder();
            int numberOfBytesRead = 0;

            // Incoming message may be larger than the buffer size.
            if(net_stream.DataAvailable)
            {
          
                numberOfBytesRead = net_stream.Read(myReadBuffer, 0, myReadBuffer.Length);
          
                myCompleteMessage.AppendFormat("{0}", Encoding.UTF8.GetString(myReadBuffer, 0, numberOfBytesRead));
                // Print out the received message to the console.
                Debug.Log("You received the following message : " +
                                             myCompleteMessage);
                return "" + myCompleteMessage;
            }

            
        }
        else
        {
 
            Console.WriteLine("Sorry.  You cannot read from this NetworkStream.");
        }
    
        return "";
    }

    public void closeSocket()
    {
        if (!socket_ready)
            return;

        socket_writer.Close();
        socket_reader.Close();
        tcp_socket.Close();
        socket_ready = false;
    }

}