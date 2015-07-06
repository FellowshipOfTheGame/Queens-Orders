using UnityEngine;
using System.Collections;

public class WarriorBehaviour : MonoBehaviour, CombatBehaviour{

    public enum COMBAT_STATE{
        IDDLE,
        BASIC_ATTACK1_CHARGING, BASIC_ATTACK1_WEAK, BASIC_ATTACK1_STRONG,
        HOLDING_SHIELD
    };

    public enum INPUT_STATE{
        NONE,
        DOWN,
        RELEASED
    }

	// Time in frames
	public int FullChargeBasicAttack1 = 60;
	public int StrongAttackTimer = 80; // Tempo da animacao
	public int WeakAttackTimer = 50; // Tempo da animacao

    public COMBAT_STATE state;
	private float attackCharge;

    public INPUT_STATE input_attack1 = INPUT_STATE.NONE;
	private bool input_defend;

	private float animationTimer = 0;
	private float animationTimerEnd = -1;

    

	// Components
	Animator animator;

	// Use this for initialization
	WarriorBehaviour() {
		state = 0;
		attackCharge = 0;
	}

	public void Start(){
		animator = GetComponent<Animator> ();
	}

	/** Attack 1 - SWORD
	 **/
	public void InputAttack1(bool down){
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
	
	// Update is called once per frame
	public void Update () {

		HandleAttack ();
		HandleDefense ();
	}

    public int getState()
    {
		return (int)state;
	}

    public BEHAVIOUR_TYPE getBehaviourType(){
        return BEHAVIOUR_TYPE.WARRIOR;
    }

	public float getStateCompleteness(){
		return animationTimer / animationTimerEnd;
	}

	void HandleAttack()
	{
        if (state == COMBAT_STATE.IDDLE && input_attack1 == INPUT_STATE.DOWN)
		{
            state = COMBAT_STATE.BASIC_ATTACK1_CHARGING;
			attackCharge = 0;
			input_attack1 = 0;

			animator.SetInteger("State", 1);
		}

		switch (state)
		{
			case COMBAT_STATE.BASIC_ATTACK1_CHARGING:
				attackCharge++;
			animator.SetFloat("AttackCharge", ((float)attackCharge)/FullChargeBasicAttack1);

                if (input_attack1 == INPUT_STATE.RELEASED)
				{
					if (attackCharge >= FullChargeBasicAttack1)
					{
						state = COMBAT_STATE.BASIC_ATTACK1_STRONG;
						animationTimer = 0;
						animationTimerEnd = StrongAttackTimer;

						animator.SetInteger("State", 2);
						GameObject.Find("Sword_MDL").transform.localScale = new Vector3(4,4,1.2f);
					}
					else
					{
						state = COMBAT_STATE.BASIC_ATTACK1_WEAK;
						animationTimer = 0;
						animationTimerEnd = WeakAttackTimer;

						animator.SetInteger("State", 2);
						GameObject.Find("Sword_MDL").transform.localScale = new Vector3(1,1,1);
					}

					input_attack1 = 0;
				}
			break;

			case COMBAT_STATE.BASIC_ATTACK1_WEAK:
				animationTimer++;
				if (animationTimer >= animationTimerEnd){
					state = COMBAT_STATE.IDDLE;

					animator.SetInteger("State", 0);
					animator.SetFloat("AttackCharge", 0);
				}
			break;

			case COMBAT_STATE.BASIC_ATTACK1_STRONG:
				animationTimer++;
				if (animationTimer >= animationTimerEnd){
					state = COMBAT_STATE.IDDLE;

					animator.SetInteger("State", 0);
					animator.SetFloat("AttackCharge", 0);
					GameObject.Find("Sword_MDL").transform.localScale = new Vector3(1,1,1);
				}
			break;

		}

	}

	void HandleDefense(){
		if (state < COMBAT_STATE.HOLDING_SHIELD || state > COMBAT_STATE.HOLDING_SHIELD)
			return;
	}
}
