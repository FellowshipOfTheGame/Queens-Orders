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

    private class SyncableObjectsHandler
    {
        public SyncableObjectsHandler()
        {
            objects = new List<SyncableObject>();
            objectsToUpdate = new List<SyncableObject>();
            objectsBitMask = new List<byte>();
            objectsModeBitMask = new List<SendMode>();
        }

        public SyncableObject getObject(int index)
        {
            return objects[index];
        }
        
        public void ObjectUpdated(SyncableObject obj, byte datamask, SendMode sendmode)
        {
            int index = obj.getIndex();

            UnityEngine.Assertions.Assert.IsTrue(obj == objects[index]);

            UnityEngine.Debug.Log("Update: " + objectsBitMask[index] + " to NeoMask: " + datamask);

            addToUpdateList(index);

            // Update the bitmask
            objectsModeBitMask[index] |= sendmode;
            objectsBitMask[index] |= (byte)datamask;

            UnityEngine.Debug.Log("Updated: " + objectsBitMask[index]);
        }
        
        public void Dispatch(QOServer server) ///TODO: Remove from this class
        {
            UnityEngine.Debug.Log("Dispatch");
            //
            MessageToSend msgGeneral = SEND_GENERAL_UPDATE.CreateMessage();
            MessageToSend msgImportant = SEND_IMPORTANT_UPDATE.CreateMessage();

            int work = 0; // 0 = done nothing, 1 = unreliable only, 2 = reliable only, 3 = both
            foreach (SyncableObject s in objectsToUpdate)
            {
                int index = s.getIndex();
                byte mask = objectsBitMask[index];
                SendMode mode = objectsModeBitMask[s.getIndex()];
                ushort datasize = s.CalculateDataSize(mask, (int)mode);

                if (datasize == 0)
                    continue;

                if ((mode & SendMode.RELIABLE) > 0)
                {
                    work |= 2;
                    msgImportant.w.Write((ushort)index);
                    msgImportant.w.Write((byte)mode);
                    msgImportant.w.Write((byte)mask);
                    if ((mode & SendMode.Created) > 0)
                    {
                        msgImportant.w.Write((byte)s.getSyncableType());
                    }
                    msgImportant.w.Write((ushort)datasize);

                    long bufferBeginI = msgImportant.w.BaseStream.Position;
                    
                    // data
                    s.WriteToBuffer(msgImportant.w, (int)mask, (int)mode);

                    long bufferEnd = msgImportant.w.BaseStream.Position;

                    UnityEngine.Debug.Log(index + " begin: " + bufferBeginI + " End: " + bufferEnd + " datasize: " + datasize);
                    UnityEngine.Assertions.Assert.IsTrue((bufferEnd - bufferBeginI) == datasize);
                }
                else if ((mode & SendMode.UNRELIABLE) > 0)
                {
                    work |= 1;
                    msgGeneral.w.Write((ushort)index);
                    msgGeneral.w.Write((byte)mode);
                    msgGeneral.w.Write((byte)mask);
                    msgGeneral.w.Write((ushort)datasize);

                    long bufferBeginG = msgGeneral.w.BaseStream.Position;
                    
                    // data
                    s.WriteToBuffer(msgGeneral.w, (int)mask, (int)mode);

                    long bufferEnd = msgGeneral.w.BaseStream.Position;

                    UnityEngine.Debug.Log(index + " begin: " + bufferBeginG + " End: " + bufferEnd + " datasize: " + datasize);
                    UnityEngine.Assertions.Assert.IsTrue((bufferEnd - bufferBeginG) == datasize);
                }
            }

            if (work > 0)
            {
                UnityEngine.Debug.Log("UNIT Sent!");

                List<QOServer.Client> clients = server.getClientList();
                if ((work & 2) > 0)
                {
                    // TODO: Check if palyer should receive
                    foreach (QOServer.Client c in clients) {
                        server.SendToPlayer(c.id, msgImportant);
                        UnityEngine.Debug.Log("Reliable send to "+c.id);
                    }
                }
                if ((work & 1) > 0)
                {
                    // TODO: Check if palyer should receive
                    foreach (QOServer.Client c in clients){
                        server.SendToPlayer(c.id, msgGeneral);
                        UnityEngine.Debug.Log("UnReliable send to " + c.id);
                    }
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

        public int AddNewObject(SyncableObject obj, byte datamask, SendMode sendmode)
        {
            objects.Add(obj);
            objectsBitMask.Add(datamask);
            objectsModeBitMask.Add(sendmode | SendMode.Created);

            int index = objects.Count - 1;
            if (sendmode != 0 || sendmode != SendMode.NotChanged)
            {
                addToUpdateList(index, true); // force add to update list
            }

            return index;
        }

        private void addToUpdateList(int index, bool forced = false)
        {
            // Add to list if not inserted yet
            if (objectsBitMask[index] == 0 || forced)
            {
                objectsToUpdate.Add(objects[index]);
            }
        }

        private List<SyncableObject> objects; ///< All syncable objects in game
        private List<SyncableObject> objectsToUpdate; ///< Objects that have changed
        private List<byte> objectsBitMask; ///< Updated data bits for each object
        private List<SendMode> objectsModeBitMask; ///< UpdateMode data bits for each object
        private List<byte> objectSendTo; ///< Players id to send to each bit represents a player ID
    }

    // Messages
    public static MessageIdentifier SEND_GENERAL_UPDATE = new MessageIdentifier(1, 0);
    public static MessageIdentifier SEND_IMPORTANT_UPDATE = new MessageIdentifier(1, 1);

    
    // Atributes
    private static MailmanS mailman = null;
    private SyncableObjectsHandler unitHandler;

    
    // Member Functions
    private MailmanS()
    {
        unitHandler = new SyncableObjectsHandler();
    }

    public static MailmanS Instance()
    {
        if (mailman == null)
        {
            mailman = new MailmanS();
        }

        return mailman;
    }

    /// Register this Mailman to given network client
    /// to receive specific message IDs and from specific channels
    public void RegisterToNetwork(NetworkServer network)
    {
        // Does not receive anything yet...?
    }

    public void Dispatch(QOServer server)
    {
        unitHandler.Dispatch(server);
    }

    public int SyncableObjectCreated(SyncableObject s, byte mask, SendMode mode)
    {
        // Check type os SyncableObject
        return unitHandler.AddNewObject(s, mask, mode);
    }

    public void ObjectUpdated(SyncableObject obj, int mask, SendMode mode)
    {
        unitHandler.ObjectUpdated(obj, (byte)mask, mode);
    }    
}
