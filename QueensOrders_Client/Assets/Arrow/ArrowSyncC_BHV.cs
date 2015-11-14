using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class ArrowSyncC_BHV : MonoBehaviour, SyncableObject{

    public const int ARROW_SYNC_TYPE = 2;

    int index = -1;

    ArrowC_BHV realObj;

    public static ArrowSyncC_BHV CreateNew(int index)
    {
        GameObject g = Instantiate(GameData.FindObjectOfType<GameData>().Arrow);
        ArrowSyncC_BHV comp = g.GetComponent<ArrowSyncC_BHV>();
        comp.index = index;
        comp.realObj = g.GetComponent<ArrowC_BHV>();

        return comp;
    }

    public int CalculateDataSize(int mask)
    {
        // never writes
        return 0;
    }

    public int getIndex()
    {
        return index;
    }

    public void ReadFromBuffer(BinaryReader buffer, int mask, int mode)
    {
        float ping = 16/1000;
        RaycastHit Hit;
        Vector3 p = DataReader.ReadVector3(buffer);
        Vector3 v = DataReader.ReadVector3(buffer);

        //UnityEngine.Debug.Log("Arrow pos rec: " + p);

        Vector3 newp = p + v * ping + Vector3.down * ((9.8f * ping * ping) / 2.0f);
        Vector3 newv = v + Vector3.down * (9.8f * ping);
        Vector3 direction = (newp - p);

        if (Physics.Raycast(p - 0.01f*direction, direction, out Hit, direction.magnitude, realObj.CollisionMask))
        {
            newp = Hit.point;
            realObj.MakeStuck(newp, newv);
        }

        realObj.Position = newp;
        realObj.Velocity = newv;
    }

    public void WriteToBuffer(BinaryWriter buffer, int mask, int mode)
    {
        // do nothing
    }

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
