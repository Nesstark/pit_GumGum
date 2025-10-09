using UnityEngine;

public class LegRotationController7 : MonoBehaviour
{
    public Transform hips;
    public Transform leftLeg;
    public Transform rightLeg;
    public float rotationAngle = 45f;
    public float switchInterval = 1f;
    public float rotationSpeed = 90f;

    private float timer;
    private bool rotateLeftLeg = true;
    private bool rotateLegsForward = false;
    private bool rotatingLegs = false;

    private void Start()
    {
        timer = switchInterval;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W) && !rotatingLegs)
        {
            rotateLegsForward = true;
            rotatingLegs = true;

        }
        else if (Input.GetKey(KeyCode.S) && !rotatingLegs)
        {
            rotateLegsForward = false;
            rotatingLegs = true;
        }

        timer -= Time.deltaTime;

        if (timer <= 0f && rotatingLegs)
        {
            rotatingLegs = false;
            SwitchLegRotation();
            timer = switchInterval;
        }

        if (rotatingLegs)
        {
            RotateLegs();
        }
    }

    private void SwitchLegRotation()
    {
        rotateLeftLeg = !rotateLeftLeg;
    }

    private void RotateLeg(bool leftLegRotation, bool forwardRotation)
    {
        Transform legToRotate = leftLegRotation ? leftLeg : rightLeg;
        float rotationDirection = forwardRotation ? 1f : -1f;
        float rotationStep = rotationSpeed * Time.deltaTime * rotationDirection * rotationAngle;

        legToRotate.Rotate(Vector3.right, rotationStep);
    }

    private void RotateLegs()
    {
        if (rotateLegsForward)
        {
            RotateLeg(rotateLeftLeg, true);
        }
        else
        {
            RotateLeg(rotateLeftLeg, false);
        }
    }
}