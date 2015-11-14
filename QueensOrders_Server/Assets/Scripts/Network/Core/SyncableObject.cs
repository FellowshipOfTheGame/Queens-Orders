using System.Collections;
using System.IO;

public interface SyncableObject {

    int getIndex();

    int getSyncableType();
    
    void WriteToBuffer(BinaryWriter buffer, int mask, int mode);

    void ReadFromBuffer(BinaryReader buffer, int mask);

    ushort CalculateDataSize(int mask, int mode);

}
