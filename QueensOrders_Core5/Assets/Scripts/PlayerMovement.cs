using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	// Camera
	public Transform myCamera;				// Used for 3rd person movement

	// Curves
	public AnimationCurve angleAccelFactor;	// Lower speed on high hills

	// Movement
	public float baseAccel = 20.0f;
	public float normalSpeed = 5.0f;
	public float runSpeed = 9.0f;
	public float jumpAccel = 80.0f;
	public float jumpDodge = 50.0f;
	public float gravity = 9.81f;
	public float frictionGroundMoving = 1.1f;
	public float frictionGroundNMoving = 3.0f;
	public float frictionAir = 0.01f;

	private Vector3 acceleration = Vector3.zero;
	private Vector3 velocity = Vector3.zero;

	// Private members
	private float currentMaxSpeed;
	private CharacterController controller;
	private CapsuleCollider body;

	private int movementMode; // 0 = normal, 1 = running, 2 = fight
	private const float h = 0.01666666f;

	void Start ()
	{
		movementMode = 0;
		controller = GetComponent<CharacterController> ();
		body = GetComponent<CapsuleCollider> ();

		currentMaxSpeed = normalSpeed;
	}

	Vector3 FindGroundNormal()
	{
		RaycastHit hit;
		if (Physics.Raycast (transform.position, -Vector3.up, out hit, body.height + 0.1f)) {
			return hit.normal;
		}
		return Vector3.up;
	}

	void UpdateModeNormal()
	{
		Vector3 friction = Vector3.zero;
		
		Vector3 movementDirection = HandleInput();
		Vector3 groundNormal = FindGroundNormal();
		
		if (controller.isGrounded)
		{
			if (movementDirection.magnitude > 0)
			{
				friction = velocity*frictionGroundMoving;
				friction.y = 0;
			} else {
				if (velocity.magnitude < 1){
					velocity = Vector3.zero;
				}else{
					friction = velocity*frictionGroundNMoving;
					friction.y = 0;
				}
			}
			
			// Use ground direction
			Vector3 mm = Vector3.ProjectOnPlane(movementDirection, groundNormal);
			float angle = Vector3.Angle(mm, movementDirection);
			
			
			// Going up slopes makes you go slower
			if (mm.y >= 0) {
				acceleration = mm * angleAccelFactor.Evaluate(angle) * baseAccel;
			} else {
				acceleration = mm * baseAccel;
			}
			acceleration.y -= gravity;
			
			// Jump
			if (Input.GetButton ("Jump"))
			{
				velocity.y = 0;
				acceleration = groundNormal * jumpAccel;
			}
			
		} else {
			acceleration.y -= gravity;
			friction = velocity*frictionAir;
			friction.y = 0;
		}
		
		Debug.DrawLine (transform.position, transform.position - friction, Color.red);
		Debug.DrawLine (transform.position, transform.position + acceleration, Color.blue);
		Debug.DrawLine (transform.position, transform.position + velocity, Color.green);

		// Apply friction
		Vector3 newvel = velocity + (acceleration-friction) * h;
		if (controller.isGrounded && newvel.magnitude > currentMaxSpeed) {
			velocity = newvel.normalized * currentMaxSpeed;
		} else {
			velocity = newvel;
		}
		
		print (controller.isGrounded+ "- Accel: " + acceleration.magnitude + " Velocity: " + velocity + " Friction: "+ friction.magnitude);
		// velocity = new Vector3(0,0,5);
		
		// Move
		controller.Move(velocity * h);
		
		// Rotate - Character will look towards it's moving velocity
		this.transform.LookAt(this.transform.position + new Vector3 (velocity.x, 0.0f, velocity.z));
	}

	void UpdateModeRunning()
	{
		// TODO: VERIFICAR O QUE MUDA PARA MODO DE CORRIDA 

		Vector3 friction = Vector3.zero;
		
		Vector3 movementDirection = HandleInput();
		Vector3 groundNormal = FindGroundNormal();
		
		if (controller.isGrounded)
		{
			if (movementDirection.magnitude > 0)
			{
				friction = velocity*frictionGroundMoving;
				friction.y = 0;
			} else {
				if (velocity.magnitude < 1){
					velocity = Vector3.zero;
				}else{
					friction = velocity*frictionGroundNMoving;
					friction.y = 0;
				}
			}
			
			// Use ground direction
			Vector3 mm = Vector3.ProjectOnPlane(movementDirection, groundNormal);
			float angle = Vector3.Angle(mm, movementDirection);
			
			
			// Going up slopes makes you go slower
			if (mm.y >= 0) {
				acceleration = mm * angleAccelFactor.Evaluate(angle) * baseAccel;
			} else {
				acceleration = mm * baseAccel;
			}
			acceleration.y -= gravity;
			
			// Jump
			if (Input.GetButton ("Jump"))
			{
				velocity.y = 0;
				Vector3 w = velocity.normalized + groundNormal;
				w.y = 1;
				acceleration = w * jumpDodge;
			}
			
		} else {
			acceleration.y -= gravity;
			friction = velocity*frictionAir;
			friction.y = 0;
		}
		
		Debug.DrawLine (transform.position, transform.position - friction, Color.red);
		Debug.DrawLine (transform.position, transform.position + acceleration, Color.blue);
		Debug.DrawLine (transform.position, transform.position + velocity, Color.green);
		print ("Accel: " + acceleration.magnitude + " Velocity: " + velocity.magnitude + " Friction: "+ friction.magnitude);
		
		// Apply friction
		Vector3 newvel = velocity + (acceleration-friction) * h;
		if (controller.isGrounded && newvel.magnitude > currentMaxSpeed) {
			velocity = newvel.normalized * currentMaxSpeed;
		} else {
			velocity = newvel;
		}
		
		// Move
		controller.Move(velocity * h);

		// Rotate - Character will be looking foward in the direction the camera is facing.
		Vector3 v = myCamera.transform.forward;
		v.y = 0;
		this.transform.LookAt(this.transform.position + v);
	}
	
	void UpdateModeFight()
	{
		// TODO: VERIFICAR O QUE MUDA PARA MODO DE BATALHA 

		Vector3 friction = Vector3.zero;
		
		Vector3 movementDirection = HandleInput();
		Vector3 groundNormal = FindGroundNormal();
		
		if (controller.isGrounded)
		{
			if (movementDirection.magnitude > 0)
			{
				friction = velocity*frictionGroundMoving;
				friction.y = 0;
			} else {
				if (velocity.magnitude < 1){
					velocity = Vector3.zero;
				}else{
					friction = velocity*frictionGroundNMoving;
					friction.y = 0;
				}
			}
			
			// Use ground direction
			Vector3 mm = Vector3.ProjectOnPlane(movementDirection, groundNormal);
			float angle = Vector3.Angle(mm, movementDirection);
			
			
			// Going up slopes makes you go slower
			if (mm.y >= 0) {
				acceleration = mm * angleAccelFactor.Evaluate(angle) * baseAccel;
			} else {
				acceleration = mm * baseAccel;
			}
			acceleration.y -= gravity;
			
			// Jump
			if (Input.GetButton ("Jump"))
			{
				velocity.y = 0;
				Vector3 w = velocity.normalized + groundNormal;
				w.y = 1;
				acceleration = w * jumpDodge;
			}
			
		} else {
			acceleration.y -= gravity;
			friction = velocity*frictionAir;
			friction.y = 0;
		}
		
		Debug.DrawLine (transform.position, transform.position - friction, Color.red);
		Debug.DrawLine (transform.position, transform.position + acceleration, Color.blue);
		Debug.DrawLine (transform.position, transform.position + velocity, Color.green);
		print ("Accel: " + acceleration.magnitude + " Velocity: " + velocity.magnitude + " Friction: "+ friction.magnitude);
		
		// Apply friction
		Vector3 newvel = velocity + (acceleration-friction) * h;
		if (controller.isGrounded && newvel.magnitude > currentMaxSpeed) {
			velocity = newvel.normalized * currentMaxSpeed;
		} else {
			velocity = newvel;
		}
		
		// Move
		controller.Move(velocity * h);

		// Rotate - Character will be looking foward in the direction the camera is facing.
		Vector3 v = myCamera.transform.forward;
		v.y = 0;
		this.transform.LookAt(this.transform.position + v);
	}

	void Update()
	{
		if (movementMode == 0) {
			UpdateModeNormal();
		} else if (movementMode == 1){
			UpdateModeRunning();
		} else {
			UpdateModeFight();
		}
	}

	Vector3 HandleInput ()
	{
		// print ("Vertical: "+Input.GetAxis("Vertical"));
		if (Input.GetAxis ("Run") > 0){
			currentMaxSpeed = runSpeed;
			movementMode = 1;
		}else{
			currentMaxSpeed = normalSpeed;
			movementMode = 0;
		}

		// Movement direction
		Vector3 moveX = myCamera.forward * Input.GetAxis ("Vertical");
		Vector3 moveY = myCamera.right * Input.GetAxis ("Horizontal");

		Vector3 move = moveX + moveY;
		move.y = 0;
		moveX.Normalize ();
		return move.normalized;
	}

}