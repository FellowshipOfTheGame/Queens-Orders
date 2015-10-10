using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.IO;

public interface NetworkEvent
{

}

public abstract class NetworkServer : MonoBehaviour {

    //! Global Network Configuration
    [SerializeField]
    private GlobalConfig globalConfig;

    //! Channel Configuration
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

    //! [Start]
    public virtual void Start()
    {
        DontDestroyOnLoad(gameObject);

        // Initialize Network
        NetworkTransport.Init(globalConfig);
    }

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

    public abstract void OnDataEvent(int recHostID, int recConnectionID, int recChannelID, byte[] recData);

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

    public byte Send(int client, int channel, MemoryStream buffer)
    {
        if (buffer.Length <= 0)
            return 0;

        byte error = 0;
        NetworkTransport.Send(socket, client, channel, buffer.ToArray(), (int)buffer.Length, out error);

        return error;
    }

}