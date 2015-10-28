using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 
/// ***** SEND *****
/// 
/// Channel 0: Unreliable
///     ID 1: Update characters
///     
/// Channel 1: Reliable
///     ID 1:  Create/Destroy characters
/// 
/// 
/// ***** RECEIVE *****
/// 
/// 
/// </summary>
public class MailmanS 
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

    // Messages
    public static MessageIdentifier SEND_GENERAL_UPDATE = new MessageIdentifier(1, 0);
    public static MessageIdentifier SEND_IMPORTANT_UPDATE = new MessageIdentifier(1, 1);

    
    // Atributes
    private static MailmanS mailman = null;
    private List<SyncableObject> objects; ///< All syncable objects in game
    private List<SyncableObject> objectsToUpdate; ///< Objects that have changed
    private List<byte> objectsBitMask; ///< Updated data bits for each object
    private List<SendMode> objectsModeBitMask; ///< UpdateMode data bits for each object

    
    // Member Functions
    private MailmanS()
    {
        objects = new List<SyncableObject>();
        objectsToUpdate = new List<SyncableObject>();
        objectsBitMask = new List<byte>();
        objectsModeBitMask = new List<SendMode>();
    }

    /// Register this Mailman to given network client
    /// to receive specific message IDs and from specific channels
    public void RegisterToNetwork(NetworkServer network)
    {

    }

    public void Dispatch(QOServer server)
    {
        UnityEngine.Debug.Log("Dispatch");
        //
        BinaryWriter msgGeneral = SEND_GENERAL_UPDATE.CreateMessage();
        BinaryWriter msgImportant = SEND_IMPORTANT_UPDATE.CreateMessage();

        int work = 0; // 0 = done nothing, 1 = unreliable only, 2 = reliable only, 3 = both
        foreach (SyncableObject s in objectsToUpdate)
        {
            byte mask = objectsBitMask[s.getIndex()];
            SendMode mode = objectsModeBitMask[s.getIndex()];

            if ((mode & SendMode.RELIABLE) > 0)
            {
                work |= 2;
                // header
                msgImportant.Write((ushort)s.getIndex());
                msgImportant.Write((byte)mode);
                msgImportant.Write((byte)mask);

                // data
                s.WriteToBuffer(msgImportant, (int)mask, (int)mode);
            }
            else if ((mode & SendMode.UNRELIABLE) > 0)
            {
                work |= 1;
                // header
                msgGeneral.Write((ushort)s.getIndex());
                msgGeneral.Write((byte)mask);

                // data
                s.WriteToBuffer(msgGeneral, (int)mask);
            }
        }

        if (work > 0)
        {
            UnityEngine.Debug.Log("UNIT Sent!");

            if ((work & 2) > 0)
            {
                server.SendToPlayer(1, 1, (MemoryStream)msgImportant.BaseStream);
                UnityEngine.Debug.Log("Reliable send");
            }
            if ((work & 1) > 0)
            {
                server.SendToPlayer(1, 0, (MemoryStream)msgGeneral.BaseStream);
                UnityEngine.Debug.Log("UnReliable send");
            }

            // Clear masks after sending
            foreach (SyncableObject s in objectsToUpdate)
            {
                objectsBitMask[s.getIndex()] = 0;
                objectsModeBitMask[s.getIndex()] = 0;
            }
            objectsToUpdate.Clear();
            UnityEngine.Debug.Log("List cleared.");
        }
    }

    public static MailmanS Instance()
    {
        if (mailman == null) {
            mailman = new MailmanS();
        }

        return mailman;
    }

    public int SyncableObjectCreated(SyncableObject s, byte mask, SendMode mode)
    {
        objects.Add(s);
        objectsBitMask.Add(mask);
        objectsModeBitMask.Add(mode | SendMode.Created);

        int index = objects.Count-1;
        if (mask != 0 || mode != SendMode.NotChanged)
        {
            addToUpdateList(index, true); // force add to update list
        }

        return index;
    }

    public void ObjectUpdated(SyncableObject obj, int mask, SendMode mode)
    {
        int index = obj.getIndex();

        UnityEngine.Assertions.Assert.IsTrue(obj == objects[index]);

        UnityEngine.Debug.Log("Update: "+objectsBitMask[index] + "NeoMask: "+mask);

        addToUpdateList(index);

        // Update the bitmask
        objectsModeBitMask[index] |= mode;
        objectsBitMask[index] |= (byte)mask;

        UnityEngine.Debug.Log("Updated: " + objectsBitMask[index]);
    }

    private void addToUpdateList(int index, bool forced=false)
    {
        // Add to list if not inserted yet
        if (objectsBitMask[index] != 0 || forced)
        {
            objectsToUpdate.Add(objects[index]);
        }
    }
    
}
