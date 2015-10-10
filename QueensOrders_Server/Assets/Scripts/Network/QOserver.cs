using UnityEngine;
using System.Collections;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(QOserver))]
public class NetworkClientEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        QOserver myScript = (QOserver)target;
        if (GUILayout.Button("Host!"))
        {
            myScript.StartServer();
        }
    }
}
#endif


enum ServerState {
    NOT_ONLINE,
    LOBBY,
    GAME,
};

public class QOserver : NetworkServer 
{
    private ServerState serverState;
    private List<int> clientsConnection;

    public override void Start()
    {
        base.Start();

        serverState = ServerState.NOT_ONLINE;
        clientsConnection = new List<int>();
    }

    public bool StartServer()
    {
        if (IsServerOpen())
        {
            return false;
        }

        if (!CreateServer())
        {
            Debug.LogError("Failed to create server!");
            return false;
        }

        //TODO: remove this - DEBUG
        serverState = ServerState.GAME;

        return true;
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            BroadcastMsg();
        }

        if (IsServerOpen())
        {
            if (serverState == ServerState.GAME)
            {
                MailmanS.Instance().Dispatch(this);
            }
        }
    }

    #region Recv Events
    public override void OnConnectEvent(int recHostID, int recConnectionID, int recChannelID)
    {
        Debug.Log("Server: Player " + recConnectionID.ToString() + " connected!");
        clientsConnection.Add(recConnectionID);
    }

    public override void OnDataEvent(int recHostID, int recConnectionID, int recChannelID, byte[] recData)
    {
        // Decoding Message
        Stream stream = new MemoryStream(recData);
        BinaryFormatter f = new BinaryFormatter();
        string msg = f.Deserialize(stream).ToString();

        Debug.Log("Server: Received Data from " + recConnectionID.ToString() + "! Message: " + msg);

        GetComponent<Transform>().position += Vector3.right;
    }

    public override void OnDisconnectEvent(int recHostID, int recConnectionID, int recChannelID)
    {
        Debug.Log("Server: Received disconnect from " + recConnectionID.ToString());
    }
    #endregion

    public void BroadcastMsg()
    {
        // Send the server a message
        byte error;
        byte[] buffer = new byte[1024];
        MemoryStream stream = new MemoryStream(buffer);
        BinaryFormatter f = new BinaryFormatter();
        f.Serialize(stream, "Hello!");

        for (int i = 0; i < clientsConnection.Count; i++)
        {
            error = Send(clientsConnection[i], 0, stream);
            LogNetworkError(error);
        }
    }

    public void BroadcastMessage(MemoryStream stream)
    {
        for (int i = 0; i < clientsConnection.Count; i++)
        {
            byte error = Send(clientsConnection[i], 0, stream);
            LogNetworkError(error);
        }
    }
}
