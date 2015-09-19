using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class UnitSync : MonoBehaviour, SyncableObject
{
    [Flags]
    public enum BitMask
    {
        Position = 1,
        Rotation = 2,

        Animation = 4,
        Team = 8,
        HP = 16,
    }

    private int index; ///< Index on mailman vector
    Transform transform;

    public void Start()
    {
        transform = GetComponent<Transform>();

        index = MailmanS.Instance().UnitCreated(this);
    }

    public int getIndex()
    {
        return index;
    }

    public void WriteToBuffer(BinaryWriter buffer, int mask)
    {
        BitMask m = (BitMask)mask;

        if ((m & BitMask.Position) != 0)
            DataWriter.WritePosition(buffer, transform.position);
        if ((m & BitMask.Rotation) != 0)
            DataWriter.WriteQuaternion(buffer, transform.rotation);
    }

    public void ReadFromBuffer(BinaryReader buffer, int mask)
    {
        BitMask m = (BitMask)mask;

        if ((m & BitMask.Position) != 0)
            transform.position = DataReader.ReadPosition(buffer);
        if ((m & BitMask.Rotation) != 0)
            transform.rotation = DataReader.ReadQuaternion(buffer);
    }

    public int CalculateDataSize(int mask)
    {
        int s = 0;
        BitMask m = (BitMask)mask;

        if ((m & BitMask.Position) != 0)
            s += DataWriter.PositionSize();
        if ((m & BitMask.Rotation) != 0)
            s += DataWriter.QuaternionSize();

        return s;
    }

}
