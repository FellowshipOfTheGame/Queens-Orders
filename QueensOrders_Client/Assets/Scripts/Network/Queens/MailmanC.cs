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
            ushort index = reader.ReadUInt16();
            byte mask = reader.ReadByte();

            if (index < objects.Count){
                objects[index].ReadFromBuffer(reader, mask);
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
            UnityEngine.Debug.Log("Index: "+index);
            SendMode mode = (SendMode)reader.ReadByte();
            UnityEngine.Debug.Log("mode: " + mode);
            byte mask = reader.ReadByte();
            UnityEngine.Debug.Log("mask: " + mask);

            if ((mode & SendMode.Created) > 0)
            {
                UnitSyncC.UnitType type = (UnitSyncC.UnitType)reader.ReadByte();

                UnitSyncC comp = UnitSyncC.CreateUnit(type, index);
                UnitCreated(comp, index);
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
                    objects[index].ReadFromBuffer(reader, mask);
                }
            }
            UnityEngine.Debug.Log("one ok");
        }

        return true;
    }

    private int UnitCreated(UnitSyncC s, int index)
    {
        // Resize list
        while (objects.Count <= index)
            objects.Add(null);

        UnityEngine.Assertions.Assert.IsTrue(objects[index] == null);

        objects[index] = s;
        return objects.Count;
    }

}
