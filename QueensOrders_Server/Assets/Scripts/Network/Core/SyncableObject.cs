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

    UNRELIABLE = Updated,
    RELIABLE = Destroy | Hide | Created | UpdateRel,
}

public interface SyncableObject {

    int getIndex();

    int getSyncableType();
    
    void WriteToBuffer(BinaryWriter buffer, SendMode mode, int mask);

    // Probably not used on server side? Server will never receive update from a Syncable...
    void ReadFromBuffer(BinaryReader buffer, int mask);

    ushort CalculateDataSize(SendMode mode, int mask);

}
