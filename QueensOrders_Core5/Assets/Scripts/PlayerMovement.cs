using UnityEngine;

public class PlayerMovement : CharacterMovement
{
	// Camera
	public Transform myCamera;				// Used for 3rd person movement


	public override void Start()
	{
		base.Start ();
	}

	public void Update()
	{
		HandleInput();

		base.UpdateCharacterMovement();
	}
	
	private void HandleInput ()
	{
		float axisH = Input.GetAxis("Horizontal");
		float axisV = Input.GetAxis("Vertical");

		Vector3 moveX = myCamera.transform.forward * axisV;
		Vector3 moveY = myCamera.transform.right * axisH;

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

		Quaternion lookingDir = Quaternion.Euler( new Vector3(0, myCamera.transform.rotation.eulerAngles.y, 0) );
		InputDesiredDirection (lookingDir);

	}

}