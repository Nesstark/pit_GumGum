using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 15f;
    public float movementSpeed = 3.5f;

    private NavMeshAgent navMeshAgent;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= detectionRange)
            FollowPlayer();
        else
            StopMoving();
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
            navMeshAgent.ResetPath();
    }
}
