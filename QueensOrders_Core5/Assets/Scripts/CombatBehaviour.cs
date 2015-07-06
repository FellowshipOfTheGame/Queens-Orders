using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum BEHAVIOUR_TYPE
{
    WARRIOR,
    ARCHER,
    BUILDER
}

interface CombatBehaviour
{
    int getState();

    BEHAVIOUR_TYPE getBehaviourType();

    float getStateCompleteness();

    void InputAttack1(bool state);
    void InputAttack2(bool state);

}