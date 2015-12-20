using System.Collections;
using System.IO;

// Send mode of messages sent by the Mailman
// Used to define how to handle the message and which comunication channel to use
public enum SendMode : byte
{
    NotChanged = 0,

    Unreliable = 0,
    Reliable = 1, // Any message is implicitly unreliable, unless Reliable is specified
    Created = 2, // New object, if bit is off, means it is an already existing object
    Destroyed = 4, // Object connections destroyed

    Visible = 8, // If set, the object is visible for the client
}

// Handle bits for SendMode enum
public class SendModeBit
{
    public static bool Check(SendMode variable, SendMode flag)
    {
        return (variable & flag) > 0;
    }

    public static SendMode TurnOffFlag(SendMode variable, SendMode flag)
    {
        return variable & ~flag;
    }
}

// Used to pass data for syncable object when new data is received from server
public class DataMessage
{
    public DataMessage()
    {

    }

    public DataMessage(SendMode mode, int mask, MemoryStream data)
    {
        this.mode = mode;
        this.mask = mask;
        this.data = new BinaryReader(data);
    }

    public SendMode mode;
    public int mask;
    public BinaryReader data;
}


// All syncable objects should register a function on the mailman by a RegisterSyncable call
// This function enables custom object creation parameters inside the creation message
// for example create a unit of given type
// SyncableObject CreateSyncableFromMessage(int index, SendMode mode, int mask, BinaryReader reader);
public interface SyncableObject {

    int getIndex();

    // CURRENT SUPPORTED RANGE: 0 ~ 255
    int getSyncableType();

    // The object has been "destroyed" by the server
    // This means that the server does not care about this object anymore and it will not be
    // syncronized any further. The object may still exist on the client (Like stuck arrows may remain longer)
    void Destroy();

    void ReadFromBuffer(DataMessage buffer);

    // Use to confirm received message packet size?
    int CalculateDataSize(SendMode mode, int mask);

}
