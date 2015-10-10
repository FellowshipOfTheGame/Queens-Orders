using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BitMap8 : List<byte> { };

public class MailmanS 
{
    private static MailmanS mailman = null;
    private List<SyncableObject> objects; ///< All syncable objects in game
    private List<SyncableObject> objectsToUpdate; ///< Objects that have changed
    private BitMap8 objectsBitMask; ///< Updated data bits for each object

    private MailmanS()
    {
        objects = new List<SyncableObject>();
        objectsToUpdate = new List<SyncableObject>();
        objectsBitMask = new BitMap8();
    }

    public void Dispatch(QOserver server)
    {
        //
        MemoryStream stream = new MemoryStream();
        BinaryWriter buffer = new BinaryWriter(stream);
        foreach (SyncableObject s in objectsToUpdate)
        {
            byte mask = objectsBitMask[s.getIndex()];
            
            // header
            buffer.Write((ushort) s.getIndex());
            buffer.Write(mask);

            // data
            s.WriteToBuffer(buffer, mask);
        }

        server.BroadcastMessage(stream);

        // Clear masks after sending
        foreach (SyncableObject s in objectsToUpdate)
        {
            objectsBitMask[s.getIndex()] = 0;
        }
        objectsToUpdate.Clear();
    }

    public static MailmanS Instance()
    {
        if (mailman == null) {
            mailman = new MailmanS();
        }

        return mailman;
    }

    public int UnitCreated(UnitSyncS s)
    {
        objects.Add(s);
        return objects.Count;
    }

    public void ObjectUpdated(SyncableObject obj, int index, int mask)
    {
        // Add to list if not inserted yet
        if (objectsBitMask[index] == 0)
        {
            objectsToUpdate.Add(obj);
        }

        // Update the bitmask
        objectsBitMask[index] |= (byte)mask;
    }

}
