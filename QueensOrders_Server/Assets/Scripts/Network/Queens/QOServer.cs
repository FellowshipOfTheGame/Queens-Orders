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


    // get all connected clients
    public List<Client> getClientList() //TODO: return a readonly
    {
        return clientList;
    }

    // Kind of a Unity Constructor
    public override void Start()
    {
        base.Start();

		// Basic initialization
        serverState = ServerState.NOT_ONLINE;
        clientList = new List<Client>();
    }

	// Start server with current configuration
    public bool StartServer()
    {
        if (IsServerOpen())
        {
			Debug.LogError("Error: The server is already open!");
            return false;
        }

        if (!CreateServer())
        {
            Debug.LogError("Error: Failed to create server!");
            return false;
        }

        // DEBUG -----
		// Somewhere this should be initialized with a "starting" server state.
		// and when "in game" set to something like "GAME state"
        serverState = ServerState.GAME;
        MailmanS.Instance().RegisterToNetwork(this); // Registered here for "game mode" test
		// ---------------

        return true;
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
        
        if (IsServerOpen())
        {
            if (serverState == ServerState.GAME) // Handle the game mode server state
            {
				// Let the Mailman work
                MailmanS.Instance().Dispatch(this);
				
				// Mailman is _currently_ the only functional service ~
				
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
    
	// The only way out for services to send data
	// Can log some data from informations here
    public void SendToPlayer(int playerID, MessageToSend msg)
    {
        Send(playerID, msg);
    }
}
