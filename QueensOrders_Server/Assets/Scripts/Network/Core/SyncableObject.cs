using System.Collections;
using System.IO;

// Send mode of messages sent by the Mailman
// Used to define how to handle the message and which comunication channel to use
public enum SendMode : byte
{
    NotChanged = 0,

    // UNRELIABLE
    Updated = 1,

    // RELIABLE
    Destroy = 2,
    Hide = 4,		// not in use... yet
    Created = 8,	// not in use... yet
    UpdateRel = 16,	// any update that MUST reach the player

	// Simplify tests to see if the message need to be sent as reliable or not
    UNRELIABLE = Updated,
    RELIABLE = Destroy | Hide | Created | UpdateRel,
}

public interface SyncableObject {

    int getIndex();

    int getSyncableType();
    
    void WriteToBuffer(BinaryWriter buffer, SendMode mode, int mask);
    
    ushort CalculateDataSize(SendMode mode, int mask);

}
