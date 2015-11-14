using UnityEngine;
using System.Collections;
using System.IO;

public class ArrowSyncS_BHV : MonoBehaviour, SyncableObject{

    public const int ARROW_SYNC_TYPE = 2;
    public const int STEPS_UNTIL_UPDATE = 50;
    

    private int index; ///< Index on mailman vector
    Vector3 position;
    Vector3 velocity;
    int updateCounter = STEPS_UNTIL_UPDATE-1;

    public static int Created(ArrowSyncS_BHV u)
    {
        return MailmanS.Instance().SyncableObjectCreated(u, 0xff, 
            MailmanS.SendMode.Created | MailmanS.SendMode.UpdateRel );
    }

    // Use this for initialization
    void Start () {
        index = Created(this);
        Debug.Log("Unity id: " + index);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetPosition(Vector3 pos, Vector3 vel)
    {
        position = pos;
        velocity = vel;

        if (++updateCounter == STEPS_UNTIL_UPDATE)
        {
            MailmanS.Instance().ObjectUpdated(this, 0xff, MailmanS.SendMode.UNRELIABLE);
            updateCounter = 0;
        }
    }

    public int getIndex()
    {
        return index;
    }

    public int getSyncableType()
    {
        return ARROW_SYNC_TYPE;
    }
    
    public void WriteToBuffer(BinaryWriter buffer, int mask, int mode)
    {
        MailmanS.SendMode smode = (MailmanS.SendMode)mode;

        DataWriter.WriteVector3(buffer, position);
        DataWriter.WriteVector3(buffer, velocity);
    }

    public void ReadFromBuffer(BinaryReader buffer, int mask)
    {
        
    }

    public ushort CalculateDataSize(int mask, int mode)
    {
        int s = 0;

        s += DataWriter.Vector3Size();
        s += DataWriter.Vector3Size();

        return (ushort)s;
    }
}
