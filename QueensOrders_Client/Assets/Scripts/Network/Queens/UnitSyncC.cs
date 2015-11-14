using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class UnitSyncC : MonoBehaviour, SyncableObject
{
    #region ENUMS
    public const int UNIT_SYNC_TYPE = 1;

    public enum UnitType
    {
        Warrior = 0,
        Archer,
        Builder
    }

    [Flags]
    public enum BitMask : byte
    {
        Position = 1,
        Rotation = 2,

        Animation = 4,
        Team = 8,
        HP = 16,
    }
    #endregion

    public static UnitSyncC CreateNew(int index, byte type)
    {
        switch ((UnitType)type)
        {
            case UnitType.Warrior:
                GameObject g = Instantiate(GameData.FindObjectOfType<GameData>().Warrior);
                UnitSyncC comp = g.GetComponent<UnitSyncC>();
                comp.index = index;
                return comp;
        }

        return null;
    }


    ///
    /// 
    /// 

    private int index = -1; ///< Index on mailman vector
    Transform m_transform;

    public void Start()
    {
        m_transform = GetComponent<Transform>();
    }

    public int getIndex()
    {
        return index;
    }

    public void WriteToBuffer(BinaryWriter buffer, int mask, int mode)
    {
        BitMask m = (BitMask)mask;

        if ((m & BitMask.Position) != 0)
            DataWriter.WriteVector3(buffer, m_transform.position);
        if ((m & BitMask.Rotation) != 0)
            DataWriter.WriteQuaternion(buffer, m_transform.rotation);
    }

    public void ReadFromBuffer(BinaryReader buffer, int mask, int mode)
    {
        BitMask m = (BitMask)mask;

        if ((m & BitMask.Position) != 0)
            transform.position = DataReader.ReadVector3(buffer);
        if ((m & BitMask.Rotation) != 0)
            transform.rotation = DataReader.ReadQuaternion(buffer);
    }

    public int CalculateDataSize(int mask)
    {
        int s = 0;
        BitMask m = (BitMask)mask;

        if ((m & BitMask.Position) != 0)
            s += DataWriter.Vector3Size();
        if ((m & BitMask.Rotation) != 0)
            s += DataWriter.QuaternionSize();

        return s;
    }

}
