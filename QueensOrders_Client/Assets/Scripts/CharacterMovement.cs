using UnityEngine;
using System.Collections;

public enum MovementMode { FREE, BATTLE, RUN };
public enum JumpState
{
    JUMPSTATE_ONGROUND,
    JUMPSTATE_JUMPSTART,
    JUMPSTATE_OFFGROUND,
    JUMPSTATE_BACKTOGROUND,
    JUMPSTATE_RECOVERED
}

public interface CharacterMovement {
    
    void Update();

    void InputRun(bool r);
    void InputDesiredDirection(Quaternion direction);
    void InputJump();
    void InputSetInBattleMode();
    void InputMovement(Vector3 direction);

    float getBattleStep();

    MovementMode getMovementMode();
    JumpState getJumpState();

    void ForceMovementMode(MovementMode mode);
}
