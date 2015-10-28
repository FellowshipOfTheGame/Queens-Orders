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
    public enum UnitType
    {
        Warrior,
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
                MailmanS.SendMode.UpdateRel
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

    public UnitType getUnitType()
    {
        return unitType;
    }

    public void WriteToBuffer(BinaryWriter buffer, int mask)
    {
        BitMask m = (BitMask)mask;

        if ((m & BitMask.Position) != 0)
            DataWriter.WritePosition(buffer, m_transform.position);
        if ((m & BitMask.Rotation) != 0)
            DataWriter.WriteQuaternion(buffer, m_transform.rotation);
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
                DataWriter.WritePosition(buffer, m_transform.position);
            if ((m & BitMask.Rotation) != 0)
                DataWriter.WriteQuaternion(buffer, m_transform.rotation);
        }
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
