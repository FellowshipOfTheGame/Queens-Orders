using System.Collections;
using System.IO;

public interface SyncableObject {

    int getIndex();

    void WriteToBuffer(BinaryWriter buffer, int mask, int mode);

    void ReadFromBuffer(BinaryReader buffer, int mask, int mode);

    int CalculateDataSize(int mask);

}
