

using System;
using UnityEngine;

/* 
	ANIMATOR STATES

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

	-- ONLY ON BATTLE --
	> StepXvel (float): Indicates left/right movement
		Left interval (-1, 0)
		Right interval (0, 1)

	> StepZvel: Indicates backward/forward movement
		Backward interval (-1, 0)
		Forward interval (0, 1)

	> BattleStep (float): Tempo de um passo no modo batalha
		0.0 [inicio] ~ 1.0 [meio] ~ 0.0 [fim]

*/

class WarriorAnimator : MonoBehaviour
{
    private Animator animator;
    private CharacterMovement movement;
    private WarriorBehaviour behaviour;

    private int ArmsLayer;

    private float speedUp;

    void Start()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<CharacterMovement>();
        behaviour = GetComponent<WarriorBehaviour>();
    }

    void LateUpdate()
    {
        CharacterMovement.MovementMode movementMode = movement.getMovementState();

        switch (movementMode)
        {
            case CharacterMovement.MovementMode.BATTLE:
                UpdateBattle();
                break;
            case CharacterMovement.MovementMode.RUN:
                UpdateRun();
                break;
            case CharacterMovement.MovementMode.FREE:
                UpdateFree();
                break;
        }

        UpdateWarrior();    
    }

    public void SetLevel(int lvl)
    {
        speedUp = (float)lvl / (float)Character.MAX_LEVEL;
    }

    void UpdateBattle()
    {
        // Input data
        float battleStep = movement.getBattleStep();
        int jumpState = (int)movement.getJumpState();

        float maxXZspeed = movement.maxRunningSpeed;

        Vector3 velocity = movement.getVelocity();
        Vector3 velocityXZ = new Vector3(velocity.x, 0, velocity.z);
        Vector2 velocityXZnormalized = velocityXZ.normalized;

        // Transform data
        Vector3 projRight = Vector3.Project(velocityXZ.normalized, transform.right);
        Vector3 projForward = Vector3.Project(velocityXZ.normalized, transform.forward);

        float stepXVel = projRight.magnitude * Mathf.Sign(projRight.x);
        float stepYVel = projForward.magnitude * Mathf.Sign(projForward.z);

        // Send results to animator
        //animator.SetInteger("MovementMode", (int)CharacterMovement.MovementMode.FREE);

        animator.SetFloat("SpeedMagnitude", velocityXZ.magnitude);
        //animator.SetFloat("MoveSpeedXZ", velocityXZ.magnitude / maxXZspeed);
        //animator.SetFloat("MoveSpeedY", velocity.y);
        //
        //animator.SetFloat("BattleStep", battleStep);
        //animator.SetFloat("StepXvel", stepXVel);
        //animator.SetFloat("StepZvel", stepYVel);
        //
        //animator.SetInteger("JumpState", jumpState);
    }

    void UpdateRun()
    {
        float maxXZspeed = movement.maxRunningSpeed;
        int jumpState = (int)movement.getJumpState();

        Vector3 velocity = movement.getVelocity();
        Vector2 velocityXZ = new Vector3(velocity.x, velocity.z);

        // Results
        // animator.SetInteger("MovementMode", (int)CharacterMovement.MovementMode.FREE);

        animator.SetFloat("SpeedMagnitude", velocityXZ.magnitude);
        // animator.SetFloat("MoveSpeedXZ", velocityXZ.magnitude / maxXZspeed);
        // animator.SetFloat("MoveSpeedY", velocity.y);
        // animator.SetInteger("JumpState", jumpState);
    }

    void UpdateFree()
    {
        float maxXZspeed = movement.maxRunningSpeed;
        int jumpState = (int)movement.getJumpState();

        Vector3 velocity = movement.getVelocity();
        Vector2 velocityXZ = new Vector3(velocity.x, velocity.z);

        // Results
        //animator.SetInteger("MovementMode", (int)CharacterMovement.MovementMode.FREE);

        animator.SetFloat("SpeedMagnitude", velocityXZ.magnitude);
        // animator.SetFloat("MoveSpeedXZ", velocityXZ.magnitude / maxXZspeed);
        // animator.SetFloat("MoveSpeedY", velocity.y);
        // animator.SetInteger("JumpState", jumpState);
    }

    void UpdateWarrior()
    {
        int state = behaviour.getState();
        float stateTime = behaviour.getStateCompleteness();
        // print(state + ": " + stateTime);


        // animator.Play(animationHashes[state], ArmsLayer, stateTime);
        // animator.Play()
        animator.SetInteger("CombatState", state);
        animator.SetFloat("CombatStateTime", stateTime);
        animator.SetFloat("CombatStateSpeedUp", speedUp);

        // ~~ 
    }
}

