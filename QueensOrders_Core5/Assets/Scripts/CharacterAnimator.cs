using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class CharacterAnimator : MonoBehaviour
{
    private Animator animator;
    private CharacterMovement movement;
    private WarriorBehaviour behaviour;

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

        BEHAVIOUR_TYPE behaviourType = behaviour.getBehaviourType();
        switch (behaviourType)
        {
            case BEHAVIOUR_TYPE.WARRIOR:
                UpdateWarrior();
                break;
            case BEHAVIOUR_TYPE.ARCHER:
                UpdateArcher();
                break;
            case BEHAVIOUR_TYPE.BUILDER:
                UpdateBuilder();
                break;
        }
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
        animator.SetInteger("MovementMode", (int)CharacterMovement.MovementMode.FREE);

        animator.SetFloat("SpeedMagnitude", velocityXZ.magnitude);
        animator.SetFloat("MoveSpeedXZ", velocityXZ.magnitude / maxXZspeed);
        animator.SetFloat("MoveSpeedY", velocity.y);

        animator.SetFloat("BattleStep", battleStep);
        animator.SetFloat("StepXvel", stepXVel);
        animator.SetFloat("StepZvel", stepYVel);

        animator.SetInteger("JumpState", jumpState);
    }

    void UpdateRun()
    {
        float maxXZspeed = movement.maxRunningSpeed;
        int jumpState = (int)movement.getJumpState();

        Vector3 velocity = movement.getVelocity();
        Vector2 velocityXZ = new Vector3(velocity.x, velocity.z);

        // Results
        animator.SetInteger("MovementMode", (int)CharacterMovement.MovementMode.FREE);

        animator.SetFloat("SpeedMagnitude", velocityXZ.magnitude);
        animator.SetFloat("MoveSpeedXZ", velocityXZ.magnitude / maxXZspeed);
        animator.SetFloat("MoveSpeedY", velocity.y);

        animator.SetInteger("JumpState", jumpState);
    }

    void UpdateFree()
    {
        float maxXZspeed = movement.maxRunningSpeed;
        int jumpState = (int)movement.getJumpState();

        Vector3 velocity = movement.getVelocity();
        Vector2 velocityXZ = new Vector3(velocity.x, velocity.z);

        // Results
        animator.SetInteger("MovementMode", (int)CharacterMovement.MovementMode.FREE);

        animator.SetFloat("SpeedMagnitude", velocityXZ.magnitude);
        animator.SetFloat("MoveSpeedXZ", velocityXZ.magnitude / maxXZspeed);
        animator.SetFloat("MoveSpeedY", velocity.y);

        animator.SetInteger("JumpState", jumpState);
    }

    void UpdateWarrior()
    {
        int state = behaviour.getState();
        float stateTime = behaviour.getStateCompleteness();

        // ~~ 
    }

    void UpdateArcher()
    {

    }

    void UpdateBuilder()
    {

    }
}

