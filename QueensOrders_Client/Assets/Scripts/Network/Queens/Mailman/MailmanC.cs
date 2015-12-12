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
///     ID 1: Update characters
/// 
/// Channel 1: Reliable
///     ID 1:  Create/Destroy characters
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

    public delegate SyncableObject CreateSyncableFromMessage(int index, SendMode mode, int mask, BinaryReader reader);

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
        network.RegisterMsgIDReceiver(RECEIVE_IMPORTANT_UPDATE, FetchReliable);
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
        UnityEngine.Debug.Log("[Unreliable] Receive syncable obj!!!");
        while (reader.PeekChar() >= 0)
        {
            //UnityEngine.Debug.Log("reading...");
            ushort index = reader.ReadUInt16();
            //UnityEngine.Debug.Log("Index: " + index);
            SendMode mode = (SendMode)reader.ReadByte();
            //UnityEngine.Debug.Log("mode: " + mode);
            byte mask = reader.ReadByte();
            //UnityEngine.Debug.Log("mask: " + mask);
            ushort msgDataSize = reader.ReadUInt16();
            //UnityEngine.Debug.Log("data size: " + msgDataSize);

            if (index < objects.Count && objects[index] != null)
            {   
                /// TODO: Verify if object is of correct syncable type
                // read if object is valid
                objects[index].ReadFromBuffer(reader, mode, mask);
            } else {
                // ignore message data
                reader.ReadBytes(msgDataSize);
            }
        }

        return true;
    }

    private bool FetchReliable(BinaryReader reader, int size)
    {
        UnityEngine.Debug.Log("[Reliable] Receive syncable obj!!!");

        while (reader.PeekChar() >= 0)
        {
            //UnityEngine.Debug.Log("reading...");
            ushort index = reader.ReadUInt16();
            //UnityEngine.Debug.Log("Index: " + index);
            SendMode mode = (SendMode)reader.ReadByte();
            //UnityEngine.Debug.Log("mode: " + mode);
            byte mask = reader.ReadByte();
            //UnityEngine.Debug.Log("mask: " + mask);

            byte createdType = 0;

            if ((mode & SendMode.Created) > 0)
            {
                createdType = reader.ReadByte();
            }

            ushort msgDataSize = reader.ReadUInt16();
            //UnityEngine.Debug.Log("data size: " + msgDataSize);

            // Process data
            if ((mode & SendMode.Created) > 0)
            {
                SyncableObject comp = createFromMessage[createdType](index, mode, mask, reader);
                SyncableCreated(comp, index);
            }
            else
            {
                if (index < objects.Count && objects[index] != null)
                {
                    objects[index].ReadFromBuffer(reader, mode, mask);
                }
                else
                {
                    // ignore message data
                    reader.ReadBytes(msgDataSize);
                }
            }
        }

        return true;
    }

    private void SyncableCreated(SyncableObject s, int index)
    {
        // Resize list
        while (objects.Count <= index)
            objects.Add(null);

        UnityEngine.Assertions.Assert.IsTrue(objects[index] == null);

        objects[index] = s;
    }

}
