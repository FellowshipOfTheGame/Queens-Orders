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
    public class Client{
        public Client(int _id) {
            id = _id;
            ping = 0;
        }

        public readonly int id;
        double ping;
    }

    private ServerState serverState;
    private List<Client> clientList;


    /// GET
    public List<Client> getClientList() //TODO: return a readonly
    {
        return clientList;
    }

    ///
    public override void Start()
    {
        base.Start();

        serverState = ServerState.NOT_ONLINE;
        clientList = new List<Client>();
     
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

    public override void LateUpdate()
    {
        base.LateUpdate();

        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            BroadcastMsg();
        }*/

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
        clientList.Add(new Client(recConnectionID));
    }

    public override void OnDisconnectEvent(int recHostID, int recConnectionID, int recChannelID)
    {
        Debug.Log("Server: Received disconnect from " + recConnectionID.ToString());
    }
    #endregion
    
    public void SendToPlayer(int playerID, MessageToSend msg)
    {
        Send(playerID, msg);
    }
}
