using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class UnitSyncC : MonoBehaviour, SyncableObject
{
    #region ENUMS
    public const int SYNC_TYPE = 1;

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

    public static SyncableObject CreateSyncableFromMessage(int index, DataMessage reader)
    {
        byte unitType = reader.data.ReadByte();
        SyncableObject obj = CreateNew(index, unitType);
        obj.ReadFromBuffer(reader);
        return obj;
    }

    protected static UnitSyncC CreateNew(int index, byte type)
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
        
    public int getIndex()
    {
        return index;
    }

    public int getSyncableType()
    {
        return SYNC_TYPE;
    }

    public void ReadFromBuffer(DataMessage buffer)
    {
        BitMask m = (BitMask)buffer.mask;

        if ((m & BitMask.Position) != 0)
            transform.position = DataReader.ReadVector3(buffer.data);
        if ((m & BitMask.Rotation) != 0)
            transform.rotation = DataReader.ReadQuaternion(buffer.data);
        
        foreach (SkinnedMeshRenderer s in GetComponentsInChildren<SkinnedMeshRenderer>()) { 
            s.enabled = SendModeBit.Check(buffer.mode, SendMode.Visible);
        }
        
    }

    public int CalculateDataSize(SendMode mode, int mask)
    {
        int s = 0;
        BitMask m = (BitMask)mask;

        if ( SendModeBit.Check(mode, SendMode.Created) )
            s += sizeof(byte);

        if ((m & BitMask.Position) != 0)
            s += DataWriter.Vector3Size();
        if ((m & BitMask.Rotation) != 0)
            s += DataWriter.QuaternionSize();

        return (ushort)s;
    }

    public void Destroy()
    {
        index = -1;
        // Object destroyed on server, but we will leave the corpose on the world.
        // GameObject.Destroy(gameObject);
    }

}
