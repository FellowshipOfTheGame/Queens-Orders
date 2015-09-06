using UnityEngine;
using System.Collections;

public class WarriorBehaviour : MonoBehaviour, CharacterBehaviour
{
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

    public GameObject equipHandR;

	// Time in frames
    public int state1_Charging = 30;    // Frames to fully charge
    public int state2_Charged = 0;
	public int state3_WeakDuration = 40;
	public int state4_StrongDuration = 80;
	public int state5_ShieldUp = 50;

    public COMBAT_STATE state;

    public INPUT_STATE input_attack1 = INPUT_STATE.NONE;
    
	private bool input_defend;

    private GameObject weaponRight;

    //
	private float animationTimer = 0;
	private float animationTimerEnd = -1;
    private Humanoid humanoid;

    private int[] animationEnd = new int[(int)COMBAT_STATE.NUM_STATES+1]; // In frames

    private void Init()
    {
        state = 0;
        animationEnd[0] = 0;
        animationEnd[1] = state1_Charging;
        animationEnd[2] = state2_Charged;
        animationEnd[3] = state3_WeakDuration;
        animationEnd[4] = state4_StrongDuration;
        animationEnd[5] = state5_ShieldUp;
    }

    /// OVERRIDES

    public void Start()
    {
        Init();

        humanoid = GetComponent<Humanoid>();

        weaponRight = (GameObject)GameObject.Instantiate(equipHandR);
        weaponRight.transform.SetParent(humanoid.rightHand.transform, false);
    }
    
	/** Attack 1 - SWORD
	 **/
	public void InputAttack1(bool down) {
		if (down) {
            input_attack1 = INPUT_STATE.DOWN;
		} else {
            input_attack1 = INPUT_STATE.RELEASED;
		}
	}

    /** Attack 2 - SHIELD
     **/
    public void InputAttack2(bool down)
    {
        // TODO
    }
	
	public void FixedUpdate() {

		HandleAttack ();
		HandleDefense ();
	}

    public int getState()
    {
		return (int)state;
	}

    public BEHAVIOUR_TYPE getBehaviourType()
    {
        return BEHAVIOUR_TYPE.WARRIOR;
    }

    public float getStateCompleteness()
    {
        if (animationTimerEnd == 0)
            return 1.0f;

		return animationTimer / animationTimerEnd;
	}

    // OVERRIDE END

	private void HandleAttack()
	{
        COMBAT_STATE statePrev = state;

        switch (state)
		{
            case COMBAT_STATE.IDDLE:
                if (input_attack1 == INPUT_STATE.DOWN)
                {
                    state = COMBAT_STATE.BASIC_ATTACK1_CHARGING;
                    input_attack1 = 0;
                }
            break;

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

                    weaponRight.transform.localScale = new Vector3(4, 4, 1.2f);
                
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
                    weaponRight.transform.localScale = new Vector3(1, 1, 1);
                }
            break;
        }

        if (statePrev != state)
        {
            // Init();
            animationTimer = 0;
            animationTimerEnd = animationEnd[(int)state];
        }

    }

    private void HandleDefense()
    {
        if (state < COMBAT_STATE.HOLDING_SHIELD || state > COMBAT_STATE.HOLDING_SHIELD)
            return;
    }
}
