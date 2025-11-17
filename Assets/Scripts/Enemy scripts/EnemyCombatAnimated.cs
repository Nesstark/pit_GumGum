using UnityEngine;

public class EnemyCombatAnimated : Unit
{
    private Animator animator;
    private bool inAttackRange = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (IsDead()) return;

        // Tell animator whether target is in range
        animator.SetBool("inRange", inAttackRange);

        // If in range, play attack animation (looping or event-based)
        if (inAttackRange)
        {
            animator.SetTrigger("attack");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Unit targetUnit = other.GetComponent<Unit>();

        if (targetUnit != null && !targetUnit.IsDead())
        {
            inAttackRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Unit targetUnit = other.GetComponent<Unit>();

        if (targetUnit != null)
        {
            inAttackRange = false;
        }
    }

    public override int Attack(Unit target)
    {
        if (target == null || target.IsDead()) return 0;

        target.TakeDamage(Damage);
        return Damage;
    }
}
