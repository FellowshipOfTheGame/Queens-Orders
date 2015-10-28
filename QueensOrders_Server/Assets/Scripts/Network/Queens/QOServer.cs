using UnityEngine;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(QOServer))]
public class NetworkClientEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        QOServer myScript = (QOServer)target;
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

public class QOServer : NetworkServer 
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
        MailmanS.Instance().RegisterToNetwork(this);

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

    public void SendToPlayer(int playerConnID, int channel, MemoryStream stream)
    {
        Send(playerConnID, channel, stream);
    }
}
