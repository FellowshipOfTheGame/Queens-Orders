using UnityEngine;


public class CharacterMovement : MonoBehaviour
{
	private const float h = 0.01666666f; // DeltaTime

	public enum MovementMode{FREE, BATTLE, RUN};

    public enum JumpState { JUMPSTATE_ONGROUND,
                            JUMPSTATE_JUMPSTART,
                            JUMPSTATE_OFFGROUND,
                            JUMPSTATE_BACKTOGROUND,
                            JUMPSTATE_RECOVERED
                            }

	// Curves
	public AnimationCurve angleAccelFactor;	// Lower speed on high hills

	// Movement 
	public float mass = 1.0f;
	public float accelFree = 20.0f;
	public float accelRun = 35.0f;
	public float jumpFree = 8.0f;
	public float jumpBattle = 6.0f;
	public float jumpRun = 12.0f;

	public float gravity = 9.81f;
	public float frictionAir = 0.01f;

	public float maxRunningSpeed = 10.0f;
	public float maxWalkingSpeed = 6.0f;

	[Tooltip("Time in frames")]
	public int afterJumpHold = 8; // After a jump the character should hold a few frames without moving
	[Tooltip("Time in frames")]
	public int beforeJumpHold = 6; // Before jumping the character should hold a few frames without moving
	[Tooltip("Time in frames")]
	public int cooldownJump = 14; // Cooldown between jumps
	[Tooltip("Time in frames")]
	public int framesPerStep = 10; // Number of frames for each step on BattleMode

	
	// Private members
	[Tooltip("DEBUG ONLY")]
	public MovementMode movementMode; // TODO: Set as PRIVATE - public for debugging purpouses
	[Tooltip("DEBUG ONLY")]
	public Vector3 accelForce = Vector3.zero;
	[Tooltip("DEBUG ONLY")]
	public Vector3 velocity = Vector3.zero;	

	private float currentAccel; // Character acceleration changes when running/walking
	private float currentMaxSpd; // Character acceleration changes when running/walking

    private JumpState jumpState;

	// Components
	private CharacterController controller;
	private CapsuleCollider body;

	// Lock movement direction when starts to run
	private bool isDirectionLocked = false;

	private Quaternion desiredRotation;

	[Tooltip("DEBUG ONLY")]
	public int stepState = 0; // Character may move in BattleMode only when stepState == 0
	[Tooltip("DEBUG ONLY")]
	public int jumpCD = 0; // May only jump if jumpCD == 0
	[Tooltip("DEBUG ONLY")]
	public int afterJumpHoldCD = 0; // Character may move only if jumpHoldCD == 0
	[Tooltip("DEBUG ONLY")]
	public int beforeJumpHoldCD = 0; // Character may move only if jumpHoldCD == 0

	// Input
	private Vector3 inputDirection;
	private bool jump;		// If true, the character will jump on the next update()
	private bool running;

	public virtual void Start()
	{
		movementMode = MovementMode.FREE;
		controller = GetComponent<CharacterController>();
		body = GetComponent<CapsuleCollider>();

		currentAccel = accelFree;
		currentMaxSpd = maxWalkingSpeed;

		desiredRotation = transform.rotation;
	}

	public MovementMode getMovementState(){
		return movementMode;
	}

    public JumpState getJumpState(){
        return jumpState;
    }

    public Vector3 getVelocity(){
        return velocity;
    }

	/* Send input command to character
	 * \param r: true for KeyDown, false for KeyUp
	 */
	public void InputRun(bool r){
		running = r;

		if ( running )
		{
			currentAccel = accelRun;
			currentMaxSpd = maxRunningSpeed;
			movementMode = MovementMode.RUN;
			isDirectionLocked = true;
		}
		else
		{
			currentAccel = accelFree;
			currentMaxSpd = maxWalkingSpeed;
			movementMode = MovementMode.FREE;
			isDirectionLocked = false;
		}
	}

	/** The combat mode needs a forward direction that
	 * the character should look at
	 */
	public void InputDesiredDirection(Quaternion direction)
	{
		if (movementMode == MovementMode.BATTLE)
			desiredRotation = direction;
	}

	/* Send jump input
	 * The character will jump on the next Update()
	 */
	public void InputJump(){

		if (jumpCD != 0 )
			return;

		jump = true;
		beforeJumpHoldCD = beforeJumpHold;
	}

	/* Change character to BattleMode
	 * Should be called when something changes the character state
	 * EX: Attacking or receiving damage
	 */
	public void InputSetInBattleMode(){
		movementMode = MovementMode.BATTLE;
		isDirectionLocked = false;
		stepState = 0;
		currentAccel = accelFree;
	}

	/* Send movement input
	 * The character will handle the input based on it's current MovementMode
	 */
	public void InputMovement(Vector3 direction){

		if (!isDirectionLocked) {
			direction.y = 0;
			direction.Normalize();

			inputDirection = direction.normalized;
			
		} else {
			if ( movementMode == MovementMode.RUN 
			&& Mathf.Abs(direction.x)+Mathf.Abs(direction.y) < 0.02f){
				isDirectionLocked = false;
			}
		}

	}

	/**
	 * Return the step current time in timeline
	 * 0.0 begin -> 1.0 middle -> 0.0 end
	 */
	public float getBattleStep()
	{
		float normalized = (float)stepState/((float)framesPerStep/2.0f); // 0 ~ 2

		if (normalized < 1.0f){
			return normalized;
		} 

		return 2.0f - normalized;
	}

	private Vector3 FindGroundNormal()
	{
		RaycastHit hit;
		if (Physics.Raycast (transform.position, -Vector3.up, out hit, body.height + 5.0f)) {
			return hit.normal.normalized;
		}

		return Vector3.up*0.01f; // weak normal
	}

	protected void OnJumpStart(){
        // ~~ efeitos
	}

	protected void Jump(Vector3 groundNormal)
	{
		if (!controller.isGrounded){
			return;
		}

		jumpCD = cooldownJump;
		afterJumpHoldCD = afterJumpHold;

		if ( movementMode ==  MovementMode.FREE ){
			velocity = (groundNormal + inputDirection*0.7f).normalized * jumpFree;
			accelForce.y = 0;
		} else if (movementMode == MovementMode.RUN && inputDirection.magnitude <= 0){
			velocity = (groundNormal + transform.forward*0.2f).normalized * jumpFree;
			accelForce.y = 0;
		} else if (movementMode == MovementMode.RUN) {
			// Jump with impulse forward to input direction
			velocity.y = 0;
			Vector3 w = inputDirection + groundNormal;
			w.Normalize();
			w.x *= w.y;
			w.z *= w.y;
			w.y *= 0.3f;
			velocity = w.normalized * jumpRun;
			accelForce = Vector3.zero;
		} else {
			// Jump with impulse forward to input direction
			velocity.y = 0;
			Vector3 w = inputDirection + groundNormal;
			w.Normalize();
			w.x *= w.y;
			w.z *= w.y;
			w.y *= 0.4f;
			velocity = w.normalized * jumpBattle;
			accelForce = Vector3.zero;
		}

		OnJumpStart();
	}

	/**
	 * Controls jump states and timers
	 * mov: Movement direction and speed may be changed
	 * 	Reaching ground after a jump will hold character movement briefly
	 */
	private void handleJumpStates(ref Vector3 mov)
	{
		// Time between jumps
		if (jumpCD > 0)
			jumpCD--;
		
		if (beforeJumpHoldCD > 0)
        {
			// mov.Set(0,0,0);	// Avoid movement
			beforeJumpHoldCD--;
			if (beforeJumpHoldCD == 0){ // Jump begins
                jumpState = JumpState.JUMPSTATE_JUMPSTART;
			}
		}
		
		// Do not move for a few frames after a jump
		if (afterJumpHoldCD == afterJumpHold){
            jumpState = JumpState.JUMPSTATE_BACKTOGROUND;
		}
		
		if (afterJumpHoldCD > 0)
		{
			mov.Set(0,0,0);	// Avoid movement
			afterJumpHoldCD--;
			if (afterJumpHoldCD == 0){ // Finished recovering
                jumpState = JumpState.JUMPSTATE_RECOVERED;
			}
		} else {
            jumpState = JumpState.JUMPSTATE_ONGROUND;
		}
	}
	
	/* Base movement used for Running and Free movement modes
	 */
	public void Update()
	{
		Vector3 friction = Vector3.zero;
		Vector3 groundNormal = FindGroundNormal();

		if (movementMode == MovementMode.BATTLE && stepState == 0){
			if (inputDirection.magnitude > 0.02f){
				stepState = framesPerStep;
				isDirectionLocked = true;
			}
		} else  if (stepState > 0){
			stepState--;
			if (stepState == 0){
				isDirectionLocked = false;
			}
		}
		
		if (controller.isGrounded)
		{
			// Use ground direction
			Vector3 mov = Vector3.ProjectOnPlane(inputDirection, groundNormal);
			float angle = Vector3.Angle(mov, inputDirection);
			handleJumpStates(ref mov);

			// Going up slopes makes you go slower
			if (mov.y >= 0) {
				accelForce = mov * angleAccelFactor.Evaluate(angle) * currentAccel;
			} else {
				accelForce = mov * currentAccel;
			}
			accelForce.y -= gravity;

			
			float frictionGroundMoving = (currentAccel/currentMaxSpd);
			if (inputDirection.magnitude > 0.01f)
			{
				friction = velocity*frictionGroundMoving;
				friction.y = 0;
			} else{
				Vector3 velXZ = new Vector3(velocity.x, 0, velocity.z);

				if (velXZ.magnitude < 0.3f){
					velocity.x = velocity.z = 0;
					accelForce = Vector3.zero;
					mov = Vector3.zero;
				} else {
					friction = velocity*frictionGroundMoving;
					friction.y = 0;
				}
			}
			
			// Stick to ground
			velocity.y = -gravity;

			// Jump
			if ( jump && jumpCD == 0 && beforeJumpHoldCD == 0 )
			{
				jump = false;
				Jump(groundNormal);
			}

		} else {
			accelForce.y -= gravity;
			friction = velocity*frictionAir;
			friction.y = 0;

            jumpState = JumpState.JUMPSTATE_OFFGROUND;
		}

		Vector3 DO = transform.position+new Vector3(0, 1.5f, 0); // debug offset
		Debug.DrawLine(DO, DO - friction/10, Color.red);
		Debug.DrawLine(DO, DO + accelForce/10, Color.blue);
		Debug.DrawLine(DO, DO + velocity/10, Color.green);

		// Apply friction
		velocity = velocity + (accelForce-friction)/mass * h;

		// Move
		controller.Move(velocity * h);
		
		Vector3 velocityXZ = new Vector3 (velocity.x, 0.0f, velocity.z);

		// Rotate character
		if (movementMode != MovementMode.BATTLE)
		{
			// Rotate - Character will look towards it's moving velocity
			if (velocityXZ.magnitude > 0.1f){
				desiredRotation = Quaternion.LookRotation(velocityXZ);
			}
		}
		this.transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, 10.0f);
	}
      


}