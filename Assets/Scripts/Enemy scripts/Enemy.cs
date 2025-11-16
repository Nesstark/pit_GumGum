using UnityEngine;

public class EnemyCombat : Unit
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Deal damage on trigger using the Unit system
    public override void OnTriggerEnter(Collider other)
    {
        Unit targetUnit = other.GetComponent<Unit>();
        if (targetUnit != null && !targetUnit.IsDead())
        {
            if (animator != null)
                animator.SetTrigger("attack");

            int dmgDealt = Attack(targetUnit); // uses base.damage
            Debug.Log($"Enemy dealt {dmgDealt} damage to {targetUnit.gameObject.name}");
        }
    }

    public override int Attack(Unit target)
    {
        if (target == null || target.IsDead()) return 0;

        // Use the base Unit damage
        target.TakeDamage(Damage);
        return Damage;
    }

    protected override void Die()
    {
        base.Die();
        if (animator != null)
            animator.SetTrigger("death");
    }
}
