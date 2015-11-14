using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
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
/// </summary>
public class MailmanC
{
    public enum SendMode : byte
    {
        NotChanged = 0,

        // UNRELIABLE
        Updated = 1,

        // RELIABLE
        Destroy = 2,
        Hide = 4,
        Created = 8,
        UpdateRel = 16,

        UNRELIABLE = Updated,
        RELIABLE = Destroy | Hide | Created | UpdateRel,
    }

    public static MailmanC Instance()
    {
        if (mailman == null)
        {
            mailman = new MailmanC();
        }

        return mailman;
    }

    private static MailmanC mailman = null;

    ///
    ///
    ///

    private List<SyncableObject> objects; ///< All syncable objects in game

        
    /// Access by the static Instance() method
    private MailmanC()
    {
        objects = new List<SyncableObject>();
    }

    /// Register this Mailman to given network client
    /// to receive specific message IDs and from specific channels
    /// 
    /// \param network The network client to self assign to
    public void RegisterToNetwork(NetworkClient network)
    {
        network.RegisterMsgIDReceiver(0, 1, FetchUpdate);
        network.RegisterMsgIDReceiver(1, 1, FetchReliable);
    }

    private bool FetchUpdate(BinaryReader reader, int size)
    {

        while (reader.PeekChar() >= 0)
        {
            UnityEngine.Debug.Log("reading...");
            ushort index = reader.ReadUInt16();
            UnityEngine.Debug.Log("Index: " + index);
            SendMode mode = (SendMode)reader.ReadByte();
            UnityEngine.Debug.Log("mode: " + mode);
            byte mask = reader.ReadByte();
            UnityEngine.Debug.Log("mask: " + mask);
            ushort msgDataSize = reader.ReadUInt16();
            UnityEngine.Debug.Log("data size: " + msgDataSize);

            if (index < objects.Count){
                if (objects[index] != null)
                {
                    objects[index].ReadFromBuffer(reader, mask, (int)mode);
                } else {
                    // ignore message data
                    reader.ReadBytes(msgDataSize);
                }
            }
        }

        return true;
    }

    private bool FetchReliable(BinaryReader reader, int size)
    {
        UnityEngine.Debug.Log("Receive syncable obj!!!");

        while (reader.PeekChar() >= 0)
        {
            UnityEngine.Debug.Log("reading...");
            ushort index = reader.ReadUInt16();
            UnityEngine.Debug.Log("Index: " + index);
            SendMode mode = (SendMode)reader.ReadByte();
            UnityEngine.Debug.Log("mode: " + mode);
            byte mask = reader.ReadByte();
            UnityEngine.Debug.Log("mask: " + mask);

            byte createdType = 0;

            if ((mode & SendMode.Created) > 0)
            {
                createdType = reader.ReadByte();
            }

            ushort msgDataSize = reader.ReadUInt16();
            UnityEngine.Debug.Log("data size: " + msgDataSize);

            // Process data
            if ((mode & SendMode.Created) > 0)
            { 
                switch (createdType)
                {
                    case UnitSyncC.UNIT_SYNC_TYPE:
                        {
                            byte unitType = reader.ReadByte();
                            UnitSyncC comp = UnitSyncC.CreateNew(index, unitType);
                            SyncableCreated(comp, index);
                        }
                    break;

                    case ArrowSyncC_BHV.ARROW_SYNC_TYPE:
                        ArrowSyncC_BHV o = ArrowSyncC_BHV.CreateNew(index);
                        SyncableCreated(o, index);
                    break;
                }
            }

            if ((mode & SendMode.Destroy) > 0)
            {
                // Destroy object
            }

            if ((mode & SendMode.Hide) > 0)
            {
                // Hide object
            }

            if ((mode & SendMode.UpdateRel) > 0)
            {
                if (index < objects.Count && objects[index] != null)
                {
                    objects[index].ReadFromBuffer(reader, mask, (int)mode);
                } else {
                    // ignore message data
                    reader.ReadBytes(msgDataSize);
                }
            }
            UnityEngine.Debug.Log("one ok");
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
