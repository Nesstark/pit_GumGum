using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Transform player; // Reference to the player
    public float detectionRange = 15f; // Range within which the enemy detects the player
    public float movementSpeed = 3.5f; // Speed of the enemy when following the player
    private bool isDead = false; // To track if the enemy is dead
    private bool isColliding = false; // To track collision with the player

    private NavMeshAgent navMeshAgent; // NavMeshAgent for pathfinding
    private Animator animator; // Reference to the animator component

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Only process movement and animations if the enemy is not dead
        if (isDead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if the player is within detection range
        if (distanceToPlayer <= detectionRange)
        {
            FollowPlayer();
        }
        else
        {
            Idle();
        }

        // If the enemy is colliding with the player, trigger attack animation
        if (isColliding == true && animator != null)
        {
                animator.SetBool("isColliding", true); // Trigger attack animation
        }
        else
        {
            animator.SetBool("isColliding", false); // Reset attack animation
        }
    }

    void FollowPlayer()
    {
        if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled)
        {
            navMeshAgent.SetDestination(player.position);
        }

        if (animator != null)
        {
            animator.SetBool("isCrawling", true); // Switch to crawling animation
        }
    }

    void Idle()
    {
        if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled)
        {
            navMeshAgent.ResetPath(); // Stop moving
        }

        if (animator != null)
        {
            animator.SetBool("isCrawling", false); // Switch to idle animation
        }
    }


    // This will be triggered when the enemy collides with the player or another object
    void OnCollisionEnter(Collision collision)
    {
        // Check if the enemy collided with the player (player tag must be "Player")
        if (collision.gameObject.CompareTag("Player"))
        {
            isColliding = true; // Set isColliding to true
        }
    }

    // This will be triggered when the enemy stops colliding with the player or another object
    void OnCollisionExit(Collision collision)
    {
        // Check if the enemy stopped colliding with the player (player tag must be "Player")
        if (collision.gameObject.CompareTag("Player"))
        {
            isColliding = false; // Set isColliding to false
        }
    }
}
