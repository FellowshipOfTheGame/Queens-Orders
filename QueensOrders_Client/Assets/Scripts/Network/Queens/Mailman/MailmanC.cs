using System.Collections;
using System.Collections.Generic;
using System.IO;

/// Mailman
/// Manages all SyncableObjects in the world
/// sending only the necessary data for the correct players
/// 
/// ***** SEND *****
/// 
/// 
/// ***** RECEIVE *****
/// 
/// Channel 0: Unreliable
///     ID 1:  ex: Update characters
/// 
/// Channel 1: Reliable
///     ID 1:  ex: Create/Destroy characters
/// 
/// 
///
public class MailmanC
{
    #region Static stuff
    // Singleton
    public static MailmanC Instance()
    {
        if (mailman == null)
        {
            mailman = new MailmanC();
        }

        return mailman;
    }

    private static MailmanC mailman = null;

    // Messages used by this service
    public static MessageIdentifier RECEIVE_GENERAL_UPDATE = new MessageIdentifier(0, 1);
    public static MessageIdentifier RECEIVE_IMPORTANT_UPDATE = new MessageIdentifier(1, 1);
    #endregion

    ///
    ///
    ///

    public delegate SyncableObject CreateSyncableFromMessage(int index, DataMessage reader);

    private List<CreateSyncableFromMessage> createFromMessage; // Ordered list of delegates to call when receiving create messages
    private List<SyncableObject> objects; // All syncable objects in game

        
    /// Access by the static Instance() method
    private MailmanC()
    {
        objects = new List<SyncableObject>();
        createFromMessage = new List<CreateSyncableFromMessage>();
    }

    /// Register this Mailman to given network client
    /// to receive specific message IDs and from specific channels
    /// 
    /// \param network The network client to self assign to
    public void RegisterToNetwork(NetworkClient network)
    {
        network.RegisterMsgIDReceiver(RECEIVE_GENERAL_UPDATE, FetchUpdate);
        network.RegisterMsgIDReceiver(RECEIVE_IMPORTANT_UPDATE, FetchUpdate);
    }

    public void RegisterSyncable(int id, CreateSyncableFromMessage cfm)
    {
        while (createFromMessage.Count <= id)
            createFromMessage.Add(null);

        if (createFromMessage[id] != null)
        {
            throw new System.Exception("More than one Syncable with the same ID");
        }
        createFromMessage[id] = cfm;
    }

    private bool FetchUpdate(BinaryReader reader, int size)
    {
        UnityEngine.Debug.Log("Receive syncable obj!!!");

        MemoryStream mstream = (MemoryStream)reader.BaseStream;
        DataMessage dataMsg = new DataMessage();

        while (reader.PeekChar() >= 0)
        {
            //UnityEngine.Debug.Log("reading...");
            ushort index = reader.ReadUInt16();
            //UnityEngine.Debug.Log("Index: " + index);
            byte syncableType = reader.ReadByte();
            //UnityEngine.Debug.Log("syncable: " + syncableType);
            SendMode mode = (SendMode)reader.ReadByte();
            //UnityEngine.Debug.Log("mode: " + mode);
            byte mask = reader.ReadByte();
            //UnityEngine.Debug.Log("mask: " + mask);
            ushort msgDataSize = reader.ReadUInt16();
            //UnityEngine.Debug.Log("data size: " + msgDataSize);
            
            // Create DataMessage
            MemoryStream msgStream = new MemoryStream(mstream.GetBuffer(), (int)mstream.Position, msgDataSize, false);
            dataMsg.mode = mode;
            dataMsg.mask = mask;
            dataMsg.data = new BinaryReader(msgStream);

            // Process data
            if ( SendModeBit.Check(mode, SendMode.Created) )
            {
                SyncableObject comp = createFromMessage[syncableType](index, dataMsg);
                SyncableCreated(comp, index);
            }
            else
            {
                if (index < objects.Count && objects[index] != null)
                {
                    if (objects[index].getSyncableType() != syncableType)
                    {
                        UnityEngine.Debug.LogError("Incompatible syncable object type message! index: " + index
                                                    + "expected type: " + objects[index].getSyncableType() 
                                                    + "received: " + syncableType);
                    }
                    else
                    {
                        objects[index].ReadFromBuffer(dataMsg);
                    }
                }
            }

            // Advance read position
            reader.ReadBytes(msgDataSize);

            if (SendModeBit.Check(mode, SendMode.Destroyed))
            {
                objects[index].Destroy();
                objects[index] = null;
            }
        }

        return true;
    }

    private void SyncableCreated(SyncableObject s, int index)
    {
        // Resize list
        while (objects.Count <= index)
            objects.Add(null);

        if (objects[index] != null)
        {
            UnityEngine.Debug.LogError("Creating object over an already existing object? index: "+index);
        }

        UnityEngine.Assertions.Assert.IsTrue(objects[index] == null);

        objects[index] = s;
    }

}
