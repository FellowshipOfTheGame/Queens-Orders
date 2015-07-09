using UnityEngine;

public enum BEHAVIOUR_TYPE
{
    WARRIOR,
    ARCHER,
    BUILDER
}

public abstract class CombatBehaviour : MonoBehaviour
{
    abstract public int getState();

    abstract public BEHAVIOUR_TYPE getBehaviourType();

    abstract public float getStateCompleteness();

    abstract public void InputAttack1(bool state);
    abstract public void InputAttack2(bool state);

}