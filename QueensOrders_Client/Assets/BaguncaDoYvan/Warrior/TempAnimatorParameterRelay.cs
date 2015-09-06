using UnityEngine;
using System.Collections;

public class TempAnimatorParameterRelay : MonoBehaviour {
	
	private Vector3 Position;
	private Vector3 LastPosition;
	private float AttackCharge = 0.0f; 
	private float ChargeTime = 0.2f;
	private float h = 0.01666666f;

	// Use this for initialization
	void Start () {
		
		LastPosition = transform.position;
		Position = transform.position;
		
	}
	
	// Update is called once per frame
	void Update () {
		
		LastPosition = Position;
		Position = transform.position;

		LastPosition.y = 0.0f;
		Position.y = 0.0f;

		GetComponent<Animator>().SetFloat("SpeedMagnitude", (Position-LastPosition).magnitude*60.0f/1.3f);

		if (Input.GetMouseButton(0)){

			if (AttackCharge < 1.0f){

				AttackCharge = AttackCharge+h/ChargeTime;

			}

			GetComponent<Animator>().SetInteger("State", 1);

		}
		else{

			if (AttackCharge > 0.0f){
				
				AttackCharge = AttackCharge-h/ChargeTime;
				GetComponent<Animator>().SetInteger("State", 2);
			}
			else {
				GetComponent<Animator>().SetInteger("State", 0);
			}

		}
		GetComponent<Animator>().SetFloat("AttackCharge", AttackCharge);  
		
	}
}
