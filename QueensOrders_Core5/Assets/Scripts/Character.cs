

using UnityEngine;


class Character : MonoBehaviour
{
    public const int MAX_LEVEL = 10;

    private CombatBehaviour behaviour;
    private CharacterMovement movement;
    private WarriorAnimator animator;

    public int level;

    void Start()
    {
        animator = GetComponent<WarriorAnimator>();
        movement = GetComponent<CharacterMovement>();
        behaviour = GetComponent<CombatBehaviour>();
    }

    void Update()
    {
        // DEBUG -> atualizar via inspector
        animator.SetLevel(level);
        // // 
    }

    public void setLevel(int lvl)
    {
        level = lvl;

        animator.SetLevel(level);
    }
}