using UnityEngine;

public enum BEHAVIOUR_TYPE
{
    WARRIOR,
    ARCHER,
    BUILDER
}

public interface CharacterBehaviour
{
    void FixedUpdate();

    int getState();

    BEHAVIOUR_TYPE getBehaviourType();

    float getStateCompleteness();

    void InputAttack1(bool state);
    void InputAttack2(bool state);
}