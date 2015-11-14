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
            MailmanS.Instance().ObjectUpdated(myScript, (int)UnitSyncS.BitMask.Position, MailmanS.SendMode.UNRELIABLE);
        }
    }
}
#endif

public class UnitSyncS : MonoBehaviour, SyncableObject
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

    public static int UnitCreated(UnitSyncS u)
    {
        return MailmanS.Instance().SyncableObjectCreated(u, 
                (byte)(UnitSyncS.BitMask.Position | UnitSyncS.BitMask.Rotation),
                MailmanS.SendMode.Created | MailmanS.SendMode.UpdateRel
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
        MailmanS.Instance().ObjectUpdated(this, (int)BitMask.Position, MailmanS.SendMode.UNRELIABLE);
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
        return UNIT_SYNC_TYPE;
    }

    public UnitType getUnitType()
    {
        return unitType;
    }
    
    public void WriteToBuffer(BinaryWriter buffer, int mask, int mode)
    {
        BitMask m = (BitMask)mask;
        MailmanS.SendMode smode = (MailmanS.SendMode)mode;

        if ( (smode & MailmanS.SendMode.Created) > 0 )
            buffer.Write((byte)unitType);

        if ((smode & MailmanS.SendMode.UpdateRel) > 0)
        {
            if ((m & BitMask.Position) != 0)
                DataWriter.WriteVector3(buffer, m_transform.position);
            if ((m & BitMask.Rotation) != 0)
                DataWriter.WriteQuaternion(buffer, m_transform.rotation);
        }
    }

    public void ReadFromBuffer(BinaryReader buffer, int mask)
    {
        BitMask m = (BitMask)mask;

        if ((m & BitMask.Position) != 0)
            m_transform.position = DataReader.ReadVector3(buffer);
        if ((m & BitMask.Rotation) != 0)
            m_transform.rotation = DataReader.ReadQuaternion(buffer);
    }

    public ushort CalculateDataSize(int mask, int mode)
    {
        int s = 0;
        BitMask m = (BitMask)mask;
        MailmanS.SendMode smode = (MailmanS.SendMode)mode;

        if ((smode & MailmanS.SendMode.Created) > 0)
            s += sizeof(byte);

        if ((m & BitMask.Position) != 0)
            s += DataWriter.Vector3Size();
        if ((m & BitMask.Rotation) != 0)
            s += DataWriter.QuaternionSize();

        return (ushort)s;
    }

}
