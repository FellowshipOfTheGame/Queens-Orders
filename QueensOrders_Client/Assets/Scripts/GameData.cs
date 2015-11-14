using UnityEngine;
using System.Collections;

public class GameData : MonoBehaviour {

    [SerializeField]
    public GameObject Warrior;

    public GameObject Arrow;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this);
	}
	
}
