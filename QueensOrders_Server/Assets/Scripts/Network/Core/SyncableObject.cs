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

public interface SyncableObject
{
    int getIndex();

    // CURRENT SUPPORTED RANGE: 0 ~ 255
    int getSyncableType();
    
    void WriteToBuffer(BinaryWriter buffer, SendMode mode, int mask);
    
    ushort CalculateDataSize(SendMode mode, int mask);

}
