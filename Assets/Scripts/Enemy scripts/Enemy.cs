using UnityEngine;

public class EnemyCombat : Unit
{
    [Header("Which collider should trigger damage?")]
    public Collider damageTrigger;

    protected override void Awake()  // change to private if Unit has no Awake()
    {
        base.Awake();                // remove this line if Unit has no Awake()

        if (damageTrigger != null)
            damageTrigger.isTrigger = true;

        // Add DamageForwarder to the trigger collider
        if (damageTrigger != null)
        {
            DamageForwarder forwarder = damageTrigger.gameObject.AddComponent<DamageForwarder>();
            forwarder.Init(this);
        }
    }

    public override int Attack(Unit target)
    {
        if (target == null || target.IsDead()) return 0;

        target.TakeDamage(Damage);
        return Damage;
    }

    protected override void Die()
    {
        base.Die();

        // Remove all Configurable Joints
        ConfigurableJoint[] joints = GetComponentsInChildren<ConfigurableJoint>();
        foreach (var joint in joints)
            Destroy(joint);
    }
}
