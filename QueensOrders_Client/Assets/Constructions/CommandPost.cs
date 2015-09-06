using UnityEngine;
using System.Collections;

public class CommandPost : MonoBehaviour {

    public GameObject currentTeam = null;

    private BuildingSlot mainBuildingSlot = null;
    private bool isConquered = false;

	void Start () {
        mainBuildingSlot = gameObject.GetComponent<BuildingSlot>();
    }
	
	void Update () {
        
    }

}
