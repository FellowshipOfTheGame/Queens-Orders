using System.Collections;
using System.IO;

public interface SyncableObject {

    int getIndex();

    void WriteToBuffer(BinaryWriter buffer, int mask);

    void ReadFromBuffer(BinaryReader buffer, int mask);

    int CalculateDataSize(int mask);

}
