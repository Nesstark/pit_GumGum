using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    public Transform player;
    public float detectionRange = 15f;
    public float movementSpeed = 3.5f;

    [Header("Attack")]
    public float attackCooldown = 2f; // seconds between attacks

    private bool isDead = false;
    private bool canAttack = true;

    private NavMeshAgent navMeshAgent;
    private Animator animator;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Move towards player if within detection range
        if (distanceToPlayer <= detectionRange)
        {
            FollowPlayer();
        }
        else
        {
            StopMoving();
        }
    }

    void FollowPlayer()
    {
        if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled)
        {
            navMeshAgent.speed = movementSpeed;
            navMeshAgent.SetDestination(player.position);
        }
    }

    void StopMoving()
    {
        if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled)
        {
            navMeshAgent.ResetPath();
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // Use OnCollisionStay so the enemy can attack repeatedly while touching player
        if (collision.gameObject.CompareTag("Player") && canAttack)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        canAttack = false;

        // Play attack animation once
        if (animator != null)
        {
            animator.SetTrigger("attack"); // use a trigger instead of bool
        }

        // Optionally: deal damage to player here

        // Wait for cooldown
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
