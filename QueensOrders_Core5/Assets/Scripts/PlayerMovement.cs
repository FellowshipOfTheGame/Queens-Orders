using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	private const float h = 0.01666666f; // DeltaTime

	public enum MovementMode{FREE, BATTLE, RUN};

	public const int JUMPSTATE_ONGROUND 	= 0;
	public const int JUMPSTATE_JUMPSTART 	= 1;
	public const int JUMPSTATE_OFFGROUND 	= 2;
	public const int JUMPSTATE_BACKTOGROUND = 3;
	public const int JUMPSTATE_RECOVERED 	= 4;

	/* ANIMATOR STATES
		> JumpState (int): Define os estados de pulo.
			0: Nao esta pulando.
			1: Inicio do pulo (carregar pulo)
			2: Fora do chao
			3: Toca no chao
			4: Termina de se recuperar -> frame seguinte passa para estado 0
		> MovementMode (int): Define estado de movimento
			0: Free
			1: Battle
			2: Run
		> MoveSpeedXZ (float): Velocidade de movimento em XZ
		> MoveSpeedY (float): Velocidade de movimento em Y
		> BattleStep (float): Tempo de um passo no modo batalha
			0.0 [inicio] ~ 1.0 [meio] ~ 0.0 [fim]
	*/

	// Camera
	public Transform myCamera;				// Used for 3rd person movement

	// Curves
	public AnimationCurve angleAccelFactor;	// Lower speed on high hills

	// Movement 
	public float mass = 1.0f;
	public float accelFree = 20.0f;
	public float accelRun = 35.0f;
	public float jumpFree = 8.0f;
	public float jumpBattle = 15.0f;

	public float gravity = 9.81f;
	public float frictionGroundMoving = 5.0f;
	public float frictionGroundNMoving = 10.0f;
	public float frictionAir = 0.01f;

	[Tooltip("Time in frames")]
	public int afterJumpHold = 8; // After a jump the character should hold a few frames without moving
	[Tooltip("Time in frames")]
	public int beforeJumpHold = 6; // Before jumping the character should hold a few frames without moving
	[Tooltip("Time in frames")]
	public int cooldownJump = 15; // Cooldown between jumps
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

	// Components
	private CharacterController controller;
	private CapsuleCollider body;
	private Animator animator;

	// Lock movement direction when starts to run
	private bool isDirectionLocked = false;

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

	public void Start()
	{
		movementMode = MovementMode.FREE;
		controller = GetComponent<CharacterController>();
		body = GetComponent<CapsuleCollider>();
		animator = GetComponent<Animator>();

		currentAccel = accelFree;
	}

	public MovementMode getMovementState(){
		return movementMode;
	}

	/* Send input command to character
	 * \param r: true for KeyDown, false for KeyUp
	 */
	public void InputRun(bool r){
		running = r;

		if ( running )
		{
			currentAccel = accelRun;
			movementMode = MovementMode.RUN;
			isDirectionLocked = true;
		}
		else
		{
			currentAccel = accelFree;
			movementMode = MovementMode.FREE;
			isDirectionLocked = false;
		}
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
		animator.SetInteger("JumpState", JUMPSTATE_JUMPSTART);

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
			velocity = (groundNormal + transform.forward*0.1f) * jumpFree;
			accelForce.y = 0;
		} else if (movementMode == MovementMode.RUN && inputDirection.magnitude <= 0){
			velocity = groundNormal*jumpBattle + transform.forward*0.1f;
			accelForce.y = 0;
		} else {
			// Jump with impulse forward to input direction
			velocity.y = 0;
			Vector3 w = inputDirection + groundNormal;
			w.Normalize();
			w.x *= w.y;
			w.z *= w.y;
			w.y *= 0.3f;
			velocity = w.normalized * jumpBattle;
			accelForce = Vector3.zero;
		}

		OnJumpStart();
	}

	/* Base movement used for Running and Free movement modes
	 */
	private void UpdateModeNormal()
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
			Vector3 mm = Vector3.ProjectOnPlane(inputDirection, groundNormal);
			float angle = Vector3.Angle(mm, inputDirection);


			// Time between jumps
			if (jumpCD > 0)
				jumpCD--;

			if (beforeJumpHoldCD > 0){
				mm = Vector3.zero;	// Avoid movement
				beforeJumpHoldCD--;
				if (beforeJumpHoldCD == 0){ // Finished recovering
					animator.SetInteger("JumpState", JUMPSTATE_JUMPSTART);
				}
			}

			// Do not move for a few frames after a jump
			if (afterJumpHoldCD == afterJumpHold){
				animator.SetInteger("JumpState", JUMPSTATE_BACKTOGROUND);
			}

			if (afterJumpHoldCD > 0)
			{
				mm = Vector3.zero;	// Avoid movement
				afterJumpHoldCD--;
				if (afterJumpHoldCD == 0){ // Finished recovering
					animator.SetInteger("JumpState", JUMPSTATE_RECOVERED);
				}
			} else {
				animator.SetInteger("JumpState", JUMPSTATE_ONGROUND);
			}


			// Going up slopes makes you go slower
			if (mm.y >= 0) {
				accelForce = mm * angleAccelFactor.Evaluate(angle) * currentAccel;
			} else {
				accelForce = mm * currentAccel;
			}
			accelForce.y -= gravity;

			
			if (inputDirection.magnitude > 0.01f)
			{
				friction = velocity*frictionGroundMoving;
				friction.y = 0;
			} else{
				Vector3 velXZ = new Vector3(velocity.x, 0, velocity.z);

				if (velXZ.magnitude < 0.3f){
					velocity.x = velocity.z = 0;
					accelForce = Vector3.zero;
					mm = Vector3.zero;
				} else {
					friction = velocity*frictionGroundNMoving;
					friction.y = 0;
				}
			}
			

			velocity.y = -gravity;
			// Colocar isso dentro de HandleInput?
			if ( jump && jumpCD == 0 && beforeJumpHoldCD == 0 )
			{
				jump = false;
				Jump(groundNormal);
			}

		} else {
			accelForce.y -= gravity;
			friction = velocity*frictionAir;
			friction.y = 0;

			animator.SetInteger("JumpState", JUMPSTATE_OFFGROUND);
		}

		Vector3 DO = transform.position+new Vector3(0, 1.5f, 0); // debug offset
		Debug.DrawLine(DO, DO - friction, Color.red);
		Debug.DrawLine(DO, DO + accelForce, Color.blue);
		Debug.DrawLine(DO, DO + velocity, Color.green);

		// Apply friction
		velocity = velocity + (accelForce-friction)/mass * h;

		// Move
		controller.Move(velocity * h);
		
		if (movementMode == MovementMode.BATTLE)
		{
			animator.SetFloat("BattleStep", getBattleStep());

			Quaternion rotation = Quaternion.Euler( 0, myCamera.transform.rotation.eulerAngles.y, 0 );
			this.transform.LookAt(this.transform.position +  rotation*Vector3.forward);
		} else {
			// Rotate - Character will look towards it's moving velocity
			this.transform.LookAt(this.transform.position + new Vector3 (velocity.x, 0.0f, velocity.z));
		}

		// Update animator
		animator.SetInteger("MovementMode", (int)movementMode);
		animator.SetFloat("MoveSpeedXZ", (new Vector3(velocity.x, 0, velocity.z)).magnitude );
		animator.SetFloat("MoveSpeedY", velocity.y);
	}

	public void Update()
	{
		HandleInput();
		UpdateModeNormal();
	}

	/* Handle input - Just for testing
	 * Other classes (IA/Network) should call the InputX methods
	 * to send input to the character
	 */
	private void HandleInput ()
	{
		float axisH = Input.GetAxis("Horizontal");
		float axisV = Input.GetAxis("Vertical");

		Vector3 moveX = myCamera.forward * axisV;
		Vector3 moveY = myCamera.right * axisH;

		InputMovement(moveX + moveY);

		if (Input.GetButtonDown("Run"))
			InputRun(true);
		else if (Input.GetButtonUp("Run"))
			InputRun(false);

		/* Enter combat mode */
		if (Input.GetKeyDown(KeyCode.LeftControl)){
			if (movementMode == MovementMode.FREE){
				InputSetInBattleMode();
			} else if (movementMode == MovementMode.BATTLE){
				movementMode = MovementMode.FREE;
			}
		}

		if (Input.GetButtonDown("Jump")){
			InputJump();
		}

	}

}