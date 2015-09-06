using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {

    private enum State {
        Building,
        Built,
        Destroying,
    }

    public bool startsBuilded = false;
    public int buildingHardness = 1;
    public int destructionDelay = 1;

    public HitPoints HP;

    private int buildingCounter = 0;
    private int destructionTimer = 0;
    private State state = State.Building;

    public BuildingSlot slotOwner;

	void Start () {
        destructionTimer = destructionDelay;
        SetMeshesVisibility(startsBuilded);
        if (startsBuilded) {
            state = State.Built;
        }
        //BuildHit(0);
	}
	
	void Update () {
        if (state == State.Destroying) {
            destructionTimer--;
            if (destructionTimer <= 0) {
                Delete();
            }
        }
        // Only for test purposes -----------
        else if(state == State.Building){
            if(Input.GetButtonDown("Fire1")){
                BuildHit(1);
            }
        }
        //-----------------------------------
	}

    public bool BuildHit(int value) {
        if(state == State.Building){
            buildingCounter += value;
            if (buildingCounter >= buildingHardness) {
                FinishBuilding();
            }
            return true;
        }
        else{
            return false;
        }
    }

    public bool FinishBuilding() {
        if (state == State.Building) {
            state = State.Built;
            SetMeshesVisibility(true);
            return true;
        }
        else{
            return false;
        }
    }

    public bool Destroy() {
        if (state == State.Built) {
            state = State.Destroying;
            SetMeshesVisibility(false);
            return true;
        }
        else {
            return false;
        }
    }

    private void Delete() {
        slotOwner.DeleteBuilding();
        Destroy(gameObject);
    }

    private void SetMeshesVisibility(bool visibility) {
        gameObject.GetComponent<MeshRenderer>().enabled = visibility;
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>()) {
            mr.enabled = visibility;
        }
    }
}