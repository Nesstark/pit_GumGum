using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Rigidbody hips;            // root Rigidbody of your ragdoll child
    private NavMeshAgent agent;

    [Header("Settings")]
    public float detectionRange = 15f;
    public float moveForce = 100f;    // force pulling hips toward agent
    public float turnSpeed = 5f;      // how quickly ragdoll rotates toward target

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // We don’t want the agent to move the transform directly
        agent.updatePosition = false;
        agent.updateRotation = false;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= detectionRange)
        {
            FollowPlayer();
        }
        else
        {
            StopMoving();
        }

        // Keep the agent synced to the ragdoll's current hips position
        if (hips != null)
            agent.nextPosition = hips.position;
    }

    void FollowPlayer()
    {
        if (agent == null || !agent.isActiveAndEnabled) return;

        agent.SetDestination(player.position);

        // Drive the ragdoll hips physically toward the agent
        Vector3 toAgent = agent.nextPosition - hips.position;
        hips.AddForce(toAgent * moveForce * Time.fixedDeltaTime, ForceMode.Acceleration);

        // Turn hips to face target direction smoothly
        if (agent.desiredVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion desiredRot = Quaternion.LookRotation(agent.desiredVelocity.normalized, Vector3.up);
            hips.MoveRotation(Quaternion.Slerp(hips.rotation, desiredRot, Time.deltaTime * turnSpeed));
        }
    }

    void StopMoving()
    {
        if (agent != null && agent.isActiveAndEnabled)
            agent.ResetPath();
    }
}
