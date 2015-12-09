using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public abstract class NetworkServer : MonoBehaviour {

    //! Global Network Configuration
    [SerializeField]
    private GlobalConfig globalConfig;

    //! Channel Configuration
    //! Do not EVER change number of channels!
    //! Only by Inspector GUI of PREFAB!
    [SerializeField]
    private ConnectionConfig connectionConfig;

    //! Maximum connections
    [SerializeField]
    private int maxConnections=32;

    //! Server port
    [SerializeField]
    private int port = 12345;

    //! Connection Socket
    private int socket;

    private bool serverOpen = false;

    // DELEGATES
    // \return Returns if the msg is incomplete
    public delegate bool OnMessage(BinaryReader reader, int size);
    private OnMessage[,] onMessageReceivers = null;
    //

    
    //! [Start]
    public virtual void Start()
    {
        DontDestroyOnLoad(gameObject);

        // Initialize Network
        NetworkTransport.Init(globalConfig);

        onMessageReceivers = new OnMessage[connectionConfig.ChannelCount, 256];
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

    public virtual void LateUpdate()
    {
        int recHostID;
        int recConnectionID;
        int recChannelID;
        int recDataSize;
        byte[] buffer = new byte[2048];
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
                    OnConnectEvent(recConnectionID, recConnectionID, recChannelID);
                    break;

                // Data Event
                case NetworkEventType.DataEvent:
                    OnDataEvent(recConnectionID, recConnectionID, recChannelID, buffer);
                    break;

                // Client Disconnected
                case NetworkEventType.DisconnectEvent:
                    OnDisconnectEvent(recConnectionID, recConnectionID, recChannelID);
                    break;

                default:
                    Debug.Log("Unhandled network event: " + networkEvent.ToString());
                    break;
            }

        } while (networkEvent != NetworkEventType.Nothing);
    }

    protected bool CreateServer()
    {
        HostTopology ht = new HostTopology(connectionConfig, maxConnections);

        // Open socket
        this.socket = NetworkTransport.AddHost(ht, port); // Server Socket
        if (this.socket < 0)
        {
            Debug.LogError("Server socket creating failed!");
            serverOpen = false;
        }
        else
        {
            Debug.Log("Server socket creation successful!");
            serverOpen = true;
        }

        return serverOpen;
    }

    public bool IsServerOpen()
    {
        return serverOpen;
    }

    #region Recv Events
    public abstract void OnConnectEvent(int recHostID, int recConnectionID, int recChannelID);

    public abstract void OnDisconnectEvent(int recHostID, int recConnectionID, int recChannelID);
    #endregion

    public void LogNetworkError(byte error)
    {
        if (error != (byte)NetworkError.Ok)
        {
            NetworkError nerror = (NetworkError)error;
            Debug.Log("Error: " + nerror.ToString());
        }
    }
    
    protected byte Send(int client, MessageToSend message)
    {
        byte error = 0;

        MemoryStream buffer = (MemoryStream)message.w.BaseStream;
        if (buffer.Length <= 0)
            return 0;

        NetworkTransport.Send(socket, client, message.id.channel,  buffer.ToArray(), (int)buffer.Length, out error);

        Debug.Log("Sending  "+buffer.Length+" bytes "+error);

        return error;
    }

    /// On receiving data
    /// Read the first byte and calls the corresponding ID handler.
    private void OnDataEvent(int recHostID, int recConnectionID, int recChannelID, byte[] recData)
    {
        // Decoding Message
        BinaryReader reader = new BinaryReader(new MemoryStream(recData));
        byte msgID = reader.ReadByte();

        if (onMessageReceivers[recChannelID, msgID] != null)
            onMessageReceivers[recChannelID, msgID](reader, recData.Length);
    }

}