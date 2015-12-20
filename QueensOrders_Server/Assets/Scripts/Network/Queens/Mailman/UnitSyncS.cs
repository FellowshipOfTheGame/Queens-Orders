using UnityEngine;
using System.Collections;
using System;
using System.IO;


#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(UnitSyncS))]
public class UnitSyncSEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UnitSyncS myScript = (UnitSyncS)target;
        if (GUILayout.Button("DIRTY!"))
        {
            Debug.Log("Dirty " + myScript.getIndex());
            MailmanS.Instance().ObjectUpdated(myScript, SendMode.Unreliable, (int)UnitSyncS.BitMask.Position);
        }
    }
}
#endif

public class UnitSyncS : MonoBehaviour, SyncableObject
{
    #region ENUMS
    public const int SYNC_TYPE = 1;

	// Indentify this unit type
	// Used to create the a new unit instance with the correct prefab
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

	// Handle newly created instances
	// Called automatically once the object is created
	// This functions just separates standard initialization
	// with mailman communitcation
    public static int UnitCreated(UnitSyncS u)
    {
        return MailmanS.Instance().SyncableObjectCreated(u,
                SendMode.Created | SendMode.Reliable,
                (byte)(UnitSyncS.BitMask.Position | UnitSyncS.BitMask.Rotation)
            );
    }

    ///
    ///
    ///

    [SerializeField]
    private UnitType unitType = UnitType.Warrior;

    private int index; ///< Index on mailman vector

    public void SetPosition()
    {
        MailmanS.Instance().ObjectUpdated(this, SendMode.Unreliable, (int)BitMask.Position);
    }

    public void Start()
    {
        index = UnitCreated(this);
        Debug.Log("Unity id: " + index);
    }

    public int getIndex()
    {
        return index;
    }

    public int getSyncableType()
    {
        return SYNC_TYPE;
    }

    public UnitType getUnitType()
    {
        return unitType;
    }
    
    public void WriteToBuffer(BinaryWriter buffer, SendMode mode, int mask)
    {
        BitMask m = (BitMask)mask;

        if ( SendModeBit.Check(mode, SendMode.Created) )
            buffer.Write((byte)unitType);
        
        if ((m & BitMask.Position) != 0)
            DataWriter.WriteVector3(buffer, transform.position);
        if ((m & BitMask.Rotation) != 0)
            DataWriter.WriteQuaternion(buffer, transform.rotation);
    }
    
    public ushort CalculateDataSize(SendMode mode, int mask)
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

}
