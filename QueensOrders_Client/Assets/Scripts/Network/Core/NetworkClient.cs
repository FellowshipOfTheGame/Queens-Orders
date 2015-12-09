using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MessageIdentifier
{
    public byte id;
    public int channel;

    public MessageIdentifier(int _channel, byte _id)
    {
        channel = _channel;
        id = _id;
    }

    public BinaryWriter CreateMessage()
    {
        BinaryWriter bw = new BinaryWriter(new MemoryStream(32));
        bw.Write(id);
        return bw;
    }

    public BinaryWriter CreateMessage(BinaryWriter on)
    {
        on.Write(id);
        return on;
    }
}

public abstract class NetworkClient : MonoBehaviour
{
    //! Global Network Configuration
    [SerializeField]
    private GlobalConfig globalConfig;

    //! Channel Configuration
    [SerializeField]
    private ConnectionConfig connectionConfig;

    //! Maximum connections
    [SerializeField]
    private int maxConnections = 2;

    //! Server IP
    [SerializeField]
    private string ip = "127.0.0.1";

    //! Server port
    [SerializeField]
    private int port = 12345;

    //! Connection Socket
    private int socket;

    //! To host connection
    private int toHostConnection;

    private bool clientConnected = false;


    // DELEGATES
    // \return Returns if the msg is incomplete
    public delegate bool OnMessage(BinaryReader reader, int size);
    private OnMessage[,] onMessageReceivers = new OnMessage[2, 256];


    //! [Start]
    public virtual void Start()
    {
        DontDestroyOnLoad(gameObject);

        // Create host topology from config
        HostTopology ht = new HostTopology(this.connectionConfig, this.maxConnections);

        // Initialize Network
        NetworkTransport.Init(this.globalConfig);

        // Open socket
        this.socket = NetworkTransport.AddHost(ht); // Client Socket
        if (this.socket < 0)
            Debug.LogError("Client socket creating failed!");
        else
            Debug.Log("Client socket creation successful!");
    }

    public void RegisterMsgIDReceiver(MessageIdentifier id, OnMessage d)
    {
        if (id.channel < connectionConfig.ChannelCount)
        {
            if (onMessageReceivers[id.channel, id.id] != null)
            {
                throw new UnityException("Can't assign multiple receivers to the same message id on the same channel [channel: " + id.channel + ", id: " + id.id + "]");
            }
            onMessageReceivers[id.channel, id.id] = d;
        }
        else
        {
            throw new System.Exception("Invalid channel");
        }
    }

    public void setIp(string Ip)
    {
        ip = Ip;
    }

    public void setPort(int Port)
    {
        port = Port;
    }

    /// Connect to given ip and port
    ///
    /// \param Ip IP adress to connect to
    /// \param Port port to use
    public void Connect(string Ip, int Port)
    {
        ip = Ip;
        port = Port;

        Connect();
    }

    /// Connect on configured IP/Port
    public void Connect()
    {
        // Connect to server
        byte error;
        toHostConnection = NetworkTransport.Connect(socket, ip, port, 0, out error);
        LogNetworkError(error);
    }

    #region Recv Events
    public abstract void OnConnectEvent(int recHostID, int recConnectionID, int recChannelID);

    public abstract void OnDisconnectEvent(int recHostID, int recConnectionID, int recChannelID);
    #endregion

    public virtual void Update()
    {
        int recHostID;
        int recConnectionID;
        int recChannelID;
        int recDataSize;
        byte[] buffer = new byte[1024];
        byte error;

        NetworkEventType networkEvent = NetworkEventType.DataEvent;

        // Poll both server/client events
        do
        {
            // Recieve Data
            networkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, buffer, 1024, out recDataSize, out error);

            //! TODO [Tratar o bit de erro]

            // Process event
            switch (networkEvent)
            {
                // No event
                case NetworkEventType.Nothing:
                    break;

                // Client connected
                case NetworkEventType.ConnectEvent:
                    clientConnected = true;
                    OnConnectEvent(recConnectionID, recConnectionID, recChannelID);
                    break;

                // Data Event
                case NetworkEventType.DataEvent:
                    OnDataEvent(recConnectionID, recConnectionID, recChannelID, buffer, recDataSize);
                    break;

                // Client Disconnected
                case NetworkEventType.DisconnectEvent:
                    clientConnected = false;
                    OnDisconnectEvent(recConnectionID, recConnectionID, recChannelID);
                    break;
            }

        } while (networkEvent != NetworkEventType.Nothing);
    }

    public void LogNetworkError(byte error)
    {
        if (error != (byte)NetworkError.Ok)
        {
            NetworkError nerror = (NetworkError)error;
            Debug.Log("Error: " + nerror.ToString());
        }
    }

    public byte SendToServer(MemoryStream buffer)
    {
        if (!clientConnected)
            return (byte)NetworkError.WrongConnection;

        byte error;
        NetworkTransport.Send(socket, toHostConnection, 0, buffer.ToArray(), (int)buffer.Length, out error);

        return error;
    }


    private void OnDataEvent(int recHostID, int recConnectionID, int recChannelID, byte[] recData, int size)
    {
        // Decoding Message
        BinaryReader reader = new BinaryReader(new MemoryStream(recData, 0, size));
        byte msgID = reader.ReadByte();

        Debug.Log(recChannelID + " Id: " + msgID + " Size: "+size);

        if (onMessageReceivers[recChannelID, msgID] != null)
            onMessageReceivers[recChannelID, msgID](reader, size);
    }
}