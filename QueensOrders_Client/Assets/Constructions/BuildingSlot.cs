using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingSlot : MonoBehaviour {

    public List<GameObject> buildables;

    private GameObject building;
    private bool isEmpty = true;

	void Start () {
        
    }
	
	void Update () {
        // Only for test purposes -------
        if (isEmpty) {
            if (Input.GetButtonDown("Jump")) {
                StartBuilding(0);
            }
        }
        //-------------------------------
    }

    public bool StartBuilding(int listIndex) {
        if ((buildables.Count > listIndex) && (isEmpty)) {
            isEmpty = false;
            building = (GameObject)Instantiate(buildables[listIndex],
                                               buildables[listIndex].transform.position + transform.position,
                                               transform.rotation);
            building.transform.SetParent(transform);
            Building b = building.GetComponent<Building>();
            b.slotOwner = this;
            return true;
        }
        else{
            return false;
        }
    }

    public void DeleteBuilding() {
        isEmpty = true;
        building = null;
    }
}
