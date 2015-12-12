using UnityEngine;
using System.Collections;

public class ArrowS_BHV : MonoBehaviour {

	public float Gravity = 9.8f;
	public float StartingSpeed = 2.0f;
	public LayerMask CollisionMask;

	public Vector3 Position;
	public Vector3 Velocity;

	private bool Stuck = false;
	private ArrowSyncS_BHV Syncker;

	// Use this for initialization
	void Start () {

		Position = transform.position;
		Velocity = transform.forward*StartingSpeed;
		Syncker = GetComponent <ArrowSyncS_BHV> ();
    }
	
	// Update is called once per frame
	void Update () {

		if (! Stuck){
		
			Velocity = Velocity+Vector3.down*Gravity*Time.deltaTime;
			RaycastHit Hit;

			if (Physics.Raycast(Position, Velocity, out Hit, Velocity.magnitude*Time.deltaTime, CollisionMask)){

				Position = Hit.point;

				//if (Vector3.Dot (Velocity.normalized, Hit.normal)){
					Stuck = true;
				//}
			} else {

				Position = Position+Velocity*Time.deltaTime;

			}

			transform.position = Position;
			transform.rotation = Quaternion.LookRotation (Velocity, Vector3.up);

            Syncker.SetPosition(Position, Velocity);
        }

	}

}
