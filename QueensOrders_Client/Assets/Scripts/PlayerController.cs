using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // private Character character;
	private CharacterMovement movement;
    private CharacterBehaviour behaviour;

	// Camera
	public Transform myCamera;				// Used for 3rd person movement

	public void Start(){
        // character = GetComponent<Character>();
        movement = GetComponent<CharacterMovement>();
        behaviour = GetComponent<CharacterBehaviour>();
	}

	public void Update()
	{
		HandleInput();
	}
	
	private void HandleInput ()
	{
		float axisH = Input.GetAxis("Horizontal");
		float axisV = Input.GetAxis("Vertical");

		Vector3 moveX = myCamera.transform.forward * axisV;
		Vector3 moveY = myCamera.transform.right * axisH;

		movement.InputMovement(moveX + moveY);

		if (Input.GetButtonDown("Run"))
			movement.InputRun(true);
		else if (Input.GetButtonUp("Run"))
			movement.InputRun(false);

		/* Enter combat mode DEBUG ONLY*/
		if (Input.GetKeyDown(KeyCode.LeftControl)){
            if (movement.getMovementMode() == MovementMode.FREE)
            {
				movement.InputSetInBattleMode();
            }
            else if (movement.getMovementMode() == MovementMode.BATTLE)
            {
                movement.ForceMovementMode(MovementMode.FREE);
			}
		}

		if (Input.GetButtonDown("Jump")){
			movement.InputJump();
		}

		Quaternion lookingDir = Quaternion.Euler( new Vector3(0, myCamera.transform.rotation.eulerAngles.y, 0) );
		movement.InputDesiredDirection (lookingDir);


		///////// WARRIOR BEHAVIOUR

		if (Input.GetMouseButtonDown(0)) {
			behaviour.InputAttack1(true);
		} else if (Input.GetMouseButtonUp(0)) {
			behaviour.InputAttack1(false);
		}


	}

}