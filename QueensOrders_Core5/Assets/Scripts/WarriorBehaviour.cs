using UnityEngine;
using System.Collections;

public class WarriorBehaviour : CombatBehaviour{

    public enum COMBAT_STATE{
        IDDLE = 0,
        BASIC_ATTACK1_CHARGING = 1,
        BASIC_ATTACK1_CHARGED = 2,
        BASIC_ATTACK1_WEAK = 3,
        BASIC_ATTACK1_STRONG = 4,
        HOLDING_SHIELD = 5,

        NUM_STATES
    };

    public enum INPUT_STATE{
        NONE,
        DOWN,
        RELEASED
    }

	// Time in frames
    public int state1_Charging = 30;    // Frames to fully charge
    public int state2_Charged = 0;
	public int state3_WeakDuration = 40;
	public int state4_StrongDuration = 80;
	public int state5_ShieldUp = 50;

    public COMBAT_STATE state;

    public INPUT_STATE input_attack1 = INPUT_STATE.NONE;
	private bool input_defend;

    //
	private float animationTimer = 0;
	private float animationTimerEnd = -1;

    private int[] animationEnd = new int[(int)COMBAT_STATE.NUM_STATES+1]; // In frames

	// Use this for initialization
	WarriorBehaviour() {
		state = 0;
	}

	public void Start(){
        animationEnd[0] = 0;
        animationEnd[1] = state1_Charging;
        animationEnd[2] = state2_Charged;
        animationEnd[3] = state3_WeakDuration;
        animationEnd[4] = state4_StrongDuration;
        animationEnd[5] = state5_ShieldUp;
	}

	/** Attack 1 - SWORD
	 **/
	public override void InputAttack1(bool down) {
		if (down) {
            input_attack1 = INPUT_STATE.DOWN;
		} else {
            input_attack1 = INPUT_STATE.RELEASED;
		}
	}

    /** Attack 2 - SHIELD
     **/
    public override void InputAttack2(bool down)
    {
        // TODO
    }
	
	// Update is called once per frame
	public void Update () {

		HandleAttack ();
		HandleDefense ();
	}

    public override int getState()
    {
		return (int)state;
	}

    public override BEHAVIOUR_TYPE getBehaviourType()
    {
        return BEHAVIOUR_TYPE.WARRIOR;
    }

    public override float getStateCompleteness()
    {
        if (animationTimerEnd == 0)
            return 1.0f;

		return animationTimer / animationTimerEnd;
	}

	void HandleAttack()
	{
        COMBAT_STATE statePrev = state;

        if (state == COMBAT_STATE.IDDLE && input_attack1 == INPUT_STATE.DOWN)
		{
            state = COMBAT_STATE.BASIC_ATTACK1_CHARGING;
			input_attack1 = 0;
		}

		switch (state)
		{
			case COMBAT_STATE.BASIC_ATTACK1_CHARGING:
                animationTimer++;

                if (animationTimer >= state1_Charging){
                    state = COMBAT_STATE.BASIC_ATTACK1_CHARGED;
                }

                if (input_attack1 == INPUT_STATE.RELEASED)
                {
                    if (state == COMBAT_STATE.BASIC_ATTACK1_CHARGED){
                        state = COMBAT_STATE.BASIC_ATTACK1_STRONG;
                    }else{
                        state = COMBAT_STATE.BASIC_ATTACK1_WEAK;
                    }

                    input_attack1 = 0;
                }
            break;

            case COMBAT_STATE.BASIC_ATTACK1_CHARGED:
                if (input_attack1 == INPUT_STATE.RELEASED)
                {
                    state = COMBAT_STATE.BASIC_ATTACK1_STRONG;

                    GameObject.Find("Sword_MDL").transform.localScale = new Vector3(4, 4, 1.2f);
                
                    input_attack1 = 0;
                }
			break;

			case COMBAT_STATE.BASIC_ATTACK1_WEAK:
				animationTimer++;
				if (animationTimer >= animationTimerEnd){
					state = COMBAT_STATE.IDDLE;
				}
			break;

			case COMBAT_STATE.BASIC_ATTACK1_STRONG:
				animationTimer++;
				if (animationTimer >= animationTimerEnd){
					state = COMBAT_STATE.IDDLE;
                    GameObject.Find("Sword_MDL").transform.localScale = new Vector3(1, 1, 1);
                }
            break;
        }

        if (statePrev != state)
        {
            Start();
            animationTimer = 0;
            animationTimerEnd = animationEnd[(int)state];
        }

    }

    void HandleDefense()
    {
        if (state < COMBAT_STATE.HOLDING_SHIELD || state > COMBAT_STATE.HOLDING_SHIELD)
            return;
    }
}
