using UnityEngine;
using System.Collections;
using System.IO;

public class ArrowSyncS_BHV : MonoBehaviour, SyncableObject{

    public const int SYNC_TYPE = 2;
    public const int STEPS_UNTIL_UPDATE = 50; // in frames
    

    private int index; ///< Index on mailman vector
    Vector3 position;
    Vector3 velocity;
    int updateCounter = STEPS_UNTIL_UPDATE-1;

    public static int Created(ArrowSyncS_BHV u)
    {
        return MailmanS.Instance().SyncableObjectCreated(u, SendMode.Created | SendMode.UpdateRel, 0xff);
    }

    // Use this for initialization
    void Start () {
        index = Created(this);
        Debug.Log("Unity id: " + index);
    }

    public void SetPosition(Vector3 pos, Vector3 vel)
    {
        position = pos;
        velocity = vel;

        if (++updateCounter == STEPS_UNTIL_UPDATE)
        {
            MailmanS.Instance().ObjectUpdated(this, SendMode.UNRELIABLE, 0xff);
            updateCounter = 0;
        }
    }
    
    public int getIndex()
    {
        return index;
    }

    public int getSyncableType()
    {
        return SYNC_TYPE;
    }
    
    public void WriteToBuffer(BinaryWriter buffer, SendMode mode, int mask)
    {
        DataWriter.WriteVector3(buffer, position);
        DataWriter.WriteVector3(buffer, velocity);
    }

    public ushort CalculateDataSize(SendMode mode, int mask)
    {
        int s = 0;

        s += DataWriter.Vector3Size();
        s += DataWriter.Vector3Size();

        return (ushort)s;
    }
}
