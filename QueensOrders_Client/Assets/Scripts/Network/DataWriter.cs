using UnityEngine;
using System.Collections;
using System.IO;

public class DataWriter 
{
    public static int PositionSize(){
        return sizeof(float) * 3;
    }

    public static int QuaternionSize(){
        return sizeof(float) * 4;
    }

    public static void WritePosition(BinaryWriter buffer, Vector3 pos)
    {
        buffer.Write(pos.x);
        buffer.Write(pos.y);
        buffer.Write(pos.z);
    }

    public static void WriteQuaternion(BinaryWriter buffer, Quaternion quat)
    {
        buffer.Write(quat.x);
        buffer.Write(quat.y);
        buffer.Write(quat.z);
        buffer.Write(quat.w);
    }

}
