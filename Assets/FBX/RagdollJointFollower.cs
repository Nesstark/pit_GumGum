using UnityEngine;

[RequireComponent(typeof(ConfigurableJoint))]
public class RagdollJointFollower : MonoBehaviour
{
    public Transform targetBone; // The animated bone this joint follows
    private ConfigurableJoint joint;
    private Quaternion startLocalRotation;

    void Start()
    {
        joint = GetComponent<ConfigurableJoint>();
        startLocalRotation = transform.localRotation;
    }

    void FixedUpdate()
    {
        if (!targetBone) return;

        // Compute target rotation relative to joint
        Quaternion targetLocalRot =
            Quaternion.Inverse(startLocalRotation) *
            Quaternion.Inverse(targetBone.localRotation);

        joint.targetRotation = targetLocalRot;
    }
}
