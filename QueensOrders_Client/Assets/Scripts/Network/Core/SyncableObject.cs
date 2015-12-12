using System.Collections;
using System.IO;

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

    // Used to simplify msg type verification
    UNRELIABLE = Updated,
    RELIABLE = Destroy | Hide | Created | UpdateRel,
}

// All syncable objects should register a function on the mailman by a RegisterSyncable call
// This function enables custom object creation parameters inside the creation message
// for example create a unit of given type
// SyncableObject CreateSyncableFromMessage(int index, SendMode mode, int mask, BinaryReader reader);
public interface SyncableObject {

    int getIndex();
    
    void ReadFromBuffer(BinaryReader buffer, SendMode mode, int mask);

    // Use to confirm received message packet size?
    int CalculateDataSize(SendMode mode, int mask);

}
