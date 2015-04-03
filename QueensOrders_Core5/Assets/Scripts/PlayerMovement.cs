using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public Transform myCamera;
	
	public float speed = 3.0f;
	public float jumpSpeed = 12.0f;
	public float gravity = 28.0f;
	public float maxSpeed = 8.0f;

	private Vector3 velocity = Vector3.zero;
	//private Vector3 accel = Vector3.zero;

	private float groundFric = 180.0f;
	private float airFric = 170.0f;

	private CharacterController controller;
	private CapsuleCollider body;

	void Start()
	{
		controller = GetComponent<CharacterController>();
		body = GetComponent<CapsuleCollider> ();
	}


	void Update()
	{
		Vector3 movement = HandleInput ();


		//if ( new Vector3(velocity.x, 0, velocity.z).magnitude > maxSpeed)
		//	accel = Vector3.zero;

		// Rotate
		this.transform.LookAt( this.transform.position + new Vector3( movement.x, 0.0f, movement.z ) );

		if (controller.isGrounded)
		{
			velocity.y = 0;
			// Jump
			if (Input.GetButton ("Jump")) {
				RaycastHit hit;
				Vector3 vec = -Vector3.up;
				if (Physics.Raycast(transform.position, vec, out hit, body.height+0.1f)) {
					print(hit.normal);
					print("Before: "+velocity+" After: "+(velocity+hit.normal*jumpSpeed) );
					velocity += hit.normal*jumpSpeed;
				}
			}
			/*
			Vector2 v = new Vector2(velocity.x, velocity.z);
			Vector2 r = - v.normalized * groundFric * Time.deltaTime;

			print("V: "+v+" R: "+r);

			if ( r.magnitude > v.magnitude ){
				velocity.x = 0;
				velocity.z = 0;
			} else {
				velocity.x += r.x;
				velocity.z += r.y;
			}*/

			velocity.x *= 0.68f;
			velocity.z *= 0.68f;

		} else {

			// Apply gravity
			velocity.y -= gravity * Time.deltaTime;
			/*
			Vector2 v = new Vector2(velocity.x, velocity.z);
			Vector2 r = - v.normalized * airFric * Time.deltaTime;
			
			if ( r.magnitude > v.magnitude ){
				velocity.x = 0;
				velocity.z = 0;
			} else {
				velocity.x += r.x;
				velocity.z += r.y;
			}*/

			velocity.x *= 0.90f;
			velocity.z *= 0.90f;
		}

		// Apply friction
		// velocity += accel;

		// Move
		controller.Move( (velocity+movement) * Time.deltaTime);

	}

	Vector3 HandleInput()
	{
		// print ("Vertical: "+Input.GetAxis("Vertical"));

		// Movement direction
		Vector3 moveX = myCamera.forward * Input.GetAxis("Vertical");
		Vector3 moveY = myCamera.right * Input.GetAxis("Horizontal");

		Vector3 move = moveX + moveY;
		move.y = 0;
		moveX.Normalize();
		return move.normalized * speed;
	}

}