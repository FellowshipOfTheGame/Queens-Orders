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

    // Not used? Client will never write syncable?
    void WriteToBuffer(BinaryWriter buffer, SendMode mode, int mask);

    void ReadFromBuffer(BinaryReader buffer, SendMode mode, int mask);

    // Also not used?
    int CalculateDataSize(int mask);

}
