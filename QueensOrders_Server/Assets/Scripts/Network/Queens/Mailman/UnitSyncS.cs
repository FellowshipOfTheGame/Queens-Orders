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
            MailmanS.Instance().ObjectUpdated(myScript, SendMode.UNRELIABLE,(int)UnitSyncS.BitMask.Position);
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
                SendMode.Created | SendMode.UpdateRel,
                (byte)(UnitSyncS.BitMask.Position | UnitSyncS.BitMask.Rotation)
            );
    }

    ///
    ///
    ///

    [SerializeField]
    private UnitType unitType = UnitType.Warrior;

    private int index; ///< Index on mailman vector
    private Transform m_transform;

    public void SetPosition()
    {
        MailmanS.Instance().ObjectUpdated(this, SendMode.UNRELIABLE, (int)BitMask.Position);
    }

    public void Start()
    {
        m_transform = GetComponent<Transform>();

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

        if ( (mode & SendMode.Created) > 0 )
            buffer.Write((byte)unitType);

        if ((mode & SendMode.UpdateRel) > 0)
        {
            if ((m & BitMask.Position) != 0)
                DataWriter.WriteVector3(buffer, m_transform.position);
            if ((m & BitMask.Rotation) != 0)
                DataWriter.WriteQuaternion(buffer, m_transform.rotation);
        }
    }
    
    public ushort CalculateDataSize(SendMode mode, int mask)
    {
        int s = 0;
        BitMask m = (BitMask)mask;

        if ((mode & SendMode.Created) > 0)
            s += sizeof(byte);

        if ((m & BitMask.Position) != 0)
            s += DataWriter.Vector3Size();
        if ((m & BitMask.Rotation) != 0)
            s += DataWriter.QuaternionSize();

        return (ushort)s;
    }

}
