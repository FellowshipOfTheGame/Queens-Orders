using UnityEngine;
using System.Collections;
using System.IO;

public class DataReader {

    public static Vector3 ReadVector3(BinaryReader buffer)
    {
        float x = buffer.ReadSingle();
        float y = buffer.ReadSingle();
        float z = buffer.ReadSingle();

        return new Vector3(x, y, z);
    }

    public static Quaternion ReadQuaternion(BinaryReader buffer)
    {
        float x = buffer.ReadSingle();
        float y = buffer.ReadSingle();
        float z = buffer.ReadSingle();
        float w = buffer.ReadSingle();

        return new Quaternion(x, y, z, w);
    }
	
}
