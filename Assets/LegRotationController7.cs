using UnityEngine;
using UnityEngine.AI;

public class LegRotationController7 : MonoBehaviour
{
    public Transform leftLeg;
    public Transform rightLeg;
    public float swingAngle = 30f;      // how far the legs swing
    public float swingSpeed = 3f;       // how fast they swing
    public float moveThreshold = 0.05f; // minimum velocity before walking animation plays

    private NavMeshAgent agent;
    private float swingTimer = 0f;

    private void Start()
    {
        agent = GetComponentInParent<NavMeshAgent>();
        // ↑ get the agent from the parent, not this object
    }

    private void Update()
    {
        bool isMoving = agent != null && agent.velocity.magnitude > moveThreshold;

        if (isMoving)
        {
            swingTimer += Time.deltaTime * swingSpeed;

            // Smooth back-and-forth leg motion using sine wave
            float leftLegAngle = Mathf.Sin(swingTimer) * swingAngle;
            float rightLegAngle = Mathf.Sin(swingTimer + Mathf.PI) * swingAngle;

            // Apply rotations locally (doesn't affect world transform)
            leftLeg.localRotation = Quaternion.Euler(leftLegAngle, 0, 0);
            rightLeg.localRotation = Quaternion.Euler(rightLegAngle, 0, 0);
        }
        else
        {
            // Return legs to neutral when not moving
            leftLeg.localRotation = Quaternion.Lerp(leftLeg.localRotation, Quaternion.identity, Time.deltaTime * 5f);
            rightLeg.localRotation = Quaternion.Lerp(rightLeg.localRotation, Quaternion.identity, Time.deltaTime * 5f);
        }
    }
}
