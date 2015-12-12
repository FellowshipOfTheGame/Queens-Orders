using UnityEngine;
using System.Collections;

public class ArrowC_BHV : MonoBehaviour {

	public float Gravity = 9.8f;
	public float StartingSpeed = 2.0f;
	public LayerMask CollisionMask;

	public Vector3 Position;
	public Vector3 Velocity;
    
	private bool Stuck = false;

	// Use this for initialization
	void Awake()
    {        
        Position = transform.position;
        Velocity = transform.forward * StartingSpeed;
	}
	
	// Update is called once per frame
	void Update () {

		if (! Stuck)
        {
			Velocity = Velocity+Vector3.down*Gravity*Time.deltaTime;
			RaycastHit Hit;

			if (Physics.Raycast(Position, Velocity, out Hit, Velocity.magnitude*Time.deltaTime, CollisionMask)){

				Position = Hit.point;

				//if (Vector3.Dot (Velocity.normalized, Hit.normal)){

					Stuck = true;

				//}

			}
			else {
				Position = Position+Velocity*Time.deltaTime;
			}

		    transform.position = Position;
		    transform.rotation = Quaternion.LookRotation(Velocity, Vector3.up);
		}
     
	}

    public void MakeStuck(Vector3 postion, Vector3 velocity)
    {
        Stuck = true;
        Velocity = velocity;
        transform.position = postion;
        transform.rotation = Quaternion.LookRotation(velocity, Vector3.up);
    }

}
