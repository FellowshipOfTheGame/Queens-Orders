using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class QOClient : NetworkClient 
{
    private static QOClient client = null;


    bool isConnected;


    public static QOClient Instance()
    {
        return client;
    }

  	// Use this for initialization
    public override void Start()
    {
        base.Start();

        client = this;
	}

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendTestDataToHost();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            GetComponent<Transform>().position = Vector3.zero;
        }
    }

    #region Recv Events
    public override void OnConnectEvent(int recHostID, int recConnectionID, int recChannelID)
    {
        Debug.Log("Client: Client connected to " + recConnectionID.ToString() + "!");
    }

    public override void OnDataEvent(int recHostID, int recConnectionID, int recChannelID, byte[] recData)
    {
        // Decoding Message
        Stream stream = new MemoryStream(recData);
        BinaryFormatter f = new BinaryFormatter();
        string msg = f.Deserialize(stream).ToString();

        Debug.Log("Client: Received Data from " + recConnectionID.ToString() + "! Message: " + msg);

        GetComponent<Transform>().position += Vector3.right;
    }

    public override void OnDisconnectEvent(int recHostID, int recConnectionID, int recChannelID)
    {
        Debug.Log("Client: Disconnected from server!");
    }
    #endregion


    // Test
    public void SendTestDataToHost()
    {
        // Send the server a message
        byte[] buffer = new byte[1024];
        MemoryStream stream = new MemoryStream(buffer);
        BinaryFormatter f = new BinaryFormatter();
        f.Serialize(stream, "Hello!");

        byte error = SendToServer(stream);

        LogNetworkError(error);
    }
}
