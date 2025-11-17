using UnityEngine;

public class EnemyCombat : Unit
{
    [Header("Which collider should trigger damage?")]
    public Collider damageTrigger;

    private Animator animator;
    public RagdollJointFollower[] followers;
    private ConfigurableJoint[] joints;

    protected override void Awake()
    {
        base.Awake();

        animator = GetComponent<Animator>();
        followers = GetComponentsInChildren<RagdollJointFollower>();
        joints = GetComponentsInChildren<ConfigurableJoint>();

        if (damageTrigger != null)
            damageTrigger.isTrigger = true;

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

        // 1. Disable animation driver
        if (animator != null)
            animator.enabled = false;

        // 2. Disable active-ragdoll joint followers
        foreach (var f in followers)
            f.enabled = false;

        // 3. OPTIONAL: Make joints fully floppy by removing motor force
        JointDrive zero = new JointDrive { positionSpring = 0, positionDamper = 0, maximumForce = 0 };
        foreach (var j in joints)
        {
            j.angularXDrive = zero;
            j.angularYZDrive = zero;
            j.slerpDrive = zero;
        }

        // Now the enemy is a FULL floppy ragdoll both in VR and physics
    }
}
