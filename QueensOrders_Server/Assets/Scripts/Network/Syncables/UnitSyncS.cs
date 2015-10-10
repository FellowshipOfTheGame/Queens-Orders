using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class UnitSyncS : MonoBehaviour, SyncableObject
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
    private Transform m_transform;

    public void SetPosition()
    {
        MailmanS.Instance().ObjectUpdated(this, index, (int)BitMask.Position);
    }

    public void Start()
    {
        m_transform = GetComponent<Transform>();

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
            DataWriter.WritePosition(buffer, m_transform.position);
        if ((m & BitMask.Rotation) != 0)
            DataWriter.WriteQuaternion(buffer, m_transform.rotation);
    }

    public void ReadFromBuffer(BinaryReader buffer, int mask)
    {
        BitMask m = (BitMask)mask;

        if ((m & BitMask.Position) != 0)
            m_transform.position = DataReader.ReadPosition(buffer);
        if ((m & BitMask.Rotation) != 0)
            m_transform.rotation = DataReader.ReadQuaternion(buffer);
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
