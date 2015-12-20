using System.Collections;
using System.Collections.Generic;
using System.IO;

public interface VisibilityProvider
{
    bool IsObjectVisibleForClient(SyncableObject obj, int clientID);
}

///
/// 
/// ***** SEND *****
/// 
/// Channel 0: Unreliable
///     ID 1: ex: Update characters
///     
/// Channel 1: Reliable
///     ID 1:  ex: Create/Destroy characters
/// 
/// 
/// ***** RECEIVE *****
/// 
/// 
///
public class MailmanS
{
    #region Static stuff
    // Messages
    public static MessageIdentifier SEND_GENERAL_UPDATE = new MessageIdentifier(0, 1);
    public static MessageIdentifier SEND_IMPORTANT_UPDATE = new MessageIdentifier(1, 1);

    public static MailmanS Instance()
    {
        if (mailman == null)
        {
            mailman = new MailmanS();
        }

        return mailman;
    }
    private static MailmanS mailman = null;
    #endregion

    private class SyncableObjectsHandler
    {
        public SyncableObjectsHandler()
        {
            objects = new List<SyncableObject>();
            objectsToUpdate = new List<SyncableObject>();
            objectsBitMask = new List<byte>();
            objectsModeBitMask = new List<SendMode>();
            objectsPrevVis = new List<List<SendMode>>();
            openIndices = new List<int>();
        }

        public SyncableObject getObject(int index)
        {
            return objects[index];
        }
        
        public void ObjectUpdated(SyncableObject obj, SendMode sendmode, byte datamask)
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
        
        public void Dispatch(QOServer server, VisibilityProvider vp)
        {
            UnityEngine.Debug.Log("Dispatch");
            long sentObjs = 0;

            // What am I sending this frame? 0 = nothing, 1 = unreliable only, 2 = reliable only, 3 = unreliable and reliable
            int work = 0;

            // Check if there is a client
            List<QOServer.Client> clients = server.getClientList();
            int nClients = clients.Count;
            if (nClients <= 0)
            {
                UnityEngine.Debug.Log("No clients connected!");
                return;
            }
            
            // Check previous state list
            if (objectsPrevVis.Count < nClients)
            {
                // Create new index and objects info if necessary
                List<SendMode> c = new List<SendMode>();
                objectsPrevVis.Add(c);
                for (int i = 0; i < objectsBitMask.Count; ++i)
                {
                    c.Add(0);
                }
            }

            // Create base messages
            MessageToSend[,] msgs = new MessageToSend[nClients, 2];
            for (int i = 0; i < nClients; ++i){
                msgs[i, 0] = SEND_IMPORTANT_UPDATE.CreateMessage();
                msgs[i, 1] = SEND_GENERAL_UPDATE.CreateMessage();
            }

            // Handle updated objects
            foreach (SyncableObject s in objectsToUpdate)
            {
                int index = s.getIndex();
                byte mask = objectsBitMask[index];
                SendMode mode = objectsModeBitMask[index];
                ushort datasize = s.CalculateDataSize(mode, mask);

                if (datasize == 0)
                    continue;

                for (int clientID = 0; clientID < nClients; ++clientID)
                {
                    // Guarantee list size
                    for (int i = objectsPrevVis[clientID].Count; i < objectsBitMask.Count; ++i){
                        objectsPrevVis[clientID].Add(0);
                    }

                    // Check if should send based on visibility
                    bool shouldSend = true;
                    bool isVisibleNow = vp.IsObjectVisibleForClient(s, clients[clientID].id);
                    SendMode lastState = objectsPrevVis[clientID][index];
                    if (isVisibleNow)
                    {
                        if ( !SendModeBit.Check(lastState, SendMode.Visible)) // was not visible
                        {
                            mode |= SendMode.Visible | SendMode.Reliable; // force reliable 
                            objectsPrevVis[clientID][index] = SendMode.Visible;
                        }
                    }
                    else // not visible
                    {
                        if ( SendModeBit.Check(lastState, SendMode.Visible) ) // was visible
                        {
                            mode |= SendMode.Reliable; // force reliable
                            mode = SendModeBit.TurnOffFlag(mode, SendMode.Visible);
                            objectsPrevVis[clientID][index] = 0;
                        }
                        else if (!SendModeBit.Check(lastState, SendMode.Visible))
                        {
                            shouldSend = false;
                        }
                    }

                    // Write data for this client
                    if (shouldSend)
                    {
                        BinaryWriter writeTo = null;

                        // Get correct stream
                        if ( SendModeBit.Check(mode, SendMode.Reliable) )
                        {
                            work |= 2;
                            writeTo = msgs[clientID, 0].w; // reliable stream
                        }
                        else // If not reliable
                        {
                            work |= 1;
                            writeTo = msgs[clientID, 1].w; // unreliable stream
                        }

                        // Write Header
                        writeTo.Write((ushort)index);
                        writeTo.Write((byte)s.getSyncableType());
                        writeTo.Write((byte)mode);
                        writeTo.Write((byte)mask);
                        writeTo.Write((ushort)datasize);

                        // Write SyncableObject data checking the data size written
                        long bufferBegin = writeTo.BaseStream.Position;
                        // data
                        s.WriteToBuffer(writeTo, mode, (int)mask);
                        long bufferEnd = writeTo.BaseStream.Position;

                        bool sizError = (bufferEnd - bufferBegin) == datasize;
                        if (sizError)
                        {
                            UnityEngine.Debug.Log("index: " + index + " Begin reliable: " + bufferBegin + " End: " + bufferEnd + " datasize: " + datasize + " diff: " + (bufferEnd - bufferBegin));
                            UnityEngine.Debug.Log("SendMode " + mode + " Mask: " + mask);
                            UnityEngine.Assertions.Assert.IsTrue(sizError);
                        }

                        sentObjs++;
                    }
                }

                if ( SendModeBit.Check(mode, SendMode.Destroyed))
                {
                    openIndices.Add(index);
                    objects[index] = null;
                }
            }

            if (work > 0)
            {
                UnityEngine.Debug.Log("Sent update for "+sentObjs+" syncables!");

                if ((work & 2) > 0)
                {
                    for (int i = 0; i < nClients; ++i)
                    {
                        server.SendToPlayer(clients[i].id, msgs[i, 0]);
                        UnityEngine.Debug.Log("Reliable send to "+clients[i].id);
                    }
                }
                if ((work & 1) > 0)
                {
                    for (int i = 0; i < nClients; ++i)
                    {
                        server.SendToPlayer(clients[i].id, msgs[i, 1]);
                        UnityEngine.Debug.Log("UnReliable send to " + clients[i].id);
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

        public int AddNewObject(SyncableObject obj, SendMode sendmode, int datamask)
        {
            int index = -1;
            if (openIndices.Count > 0)
            {
                index = openIndices[0];
                openIndices.RemoveAt(0);

                objects[index] = obj;
                objectsBitMask[index] = (byte)datamask;
                objectsModeBitMask[index] = (sendmode | SendMode.Created);

                foreach (List<SendMode> l in objectsPrevVis)
                {
                    l[index] = SendMode.NotChanged;
                }
            }
            else
            {
                // Append
                objects.Add(obj);
                objectsBitMask.Add((byte)datamask);
                objectsModeBitMask.Add(sendmode | SendMode.Created);
                index = objects.Count - 1;

                foreach (List<SendMode> l in objectsPrevVis)
                {
                    l.Add(SendMode.NotChanged);
                }
            }
            
            if (sendmode != 0 || sendmode != SendMode.NotChanged)
            {
                addToUpdateList(index, true); // force add to update list
            }

            return index;
        }

        public void DestroyObject(SyncableObject obj)
        {
            ObjectUpdated(obj, SendMode.Destroyed, 0);
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
        private List<SendMode> objectsModeBitMask; ///< UpdateMode SendMode for each object
        private List<List<SendMode>> objectsPrevVis; ///< Previous state of visibility of each object for each player
        private List<int> openIndices;
    }

    /// Atributes
    
    private SyncableObjectsHandler unitHandler;


    /// Member Functions

    private MailmanS()
    {
        unitHandler = new SyncableObjectsHandler();
    }
    
    /// Register this Mailman to given network client
    /// to receive specific message IDs and from specific channels
    public void RegisterToNetwork(NetworkServer network)
    {
        // Does not receive anything yet...?
    }

	// Send anything the Mailman have to send
    public void Dispatch(QOServer server, VisibilityProvider vp)
    {
        unitHandler.Dispatch(server, vp);
    }

	// Register new Syncables created.
	// This function MUST be called when a new SyncableObject is created.
    public int SyncableObjectCreated(SyncableObject s, SendMode mode, int mask)
    {
        // Check type os SyncableObject
        return unitHandler.AddNewObject(s, mode, mask);
    }

	// Register the SyncableObject as updated with given flags
	// These flags are "added" with current state (defined by previous calls)
    public void ObjectUpdated(SyncableObject obj, SendMode mode, int mask)
    {
        unitHandler.ObjectUpdated(obj, mode, (byte)mask);
    }
}
