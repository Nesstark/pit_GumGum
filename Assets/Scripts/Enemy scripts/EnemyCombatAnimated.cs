using UnityEngine;

public class EnemyCombatAnimated : MonoBehaviour
{
    private Animator animator;
    private bool inAttackRange = false;
    public Collider attackRangeCollider;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Update animator bool
        animator.SetBool("inRange", inAttackRange);

        // Trigger attack when in range
        if (inAttackRange)
        {
            animator.SetTrigger("attack");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // When ANY collider enters — or you can filter by tag if needed
        inAttackRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        inAttackRange = false;
    }
}
