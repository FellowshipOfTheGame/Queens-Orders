using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(QOClient))]
public class NetworkClientEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        QOClient myScript = (QOClient)target;
        if (GUILayout.Button("Connect!"))
        {
            myScript.Connect();
        }
    }
}
#endif

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

        MailmanC.Instance().RegisterToNetwork(this);
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
