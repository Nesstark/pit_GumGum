using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public class StretchyArmController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The XR Controller (your actual hand position)")]
    public Transform xrControllerTransform;
    
    [Tooltip("The original hand mesh under XR Controller (will be hidden when stretched)")]
    public GameObject originalHandMesh;
    
    [Tooltip("The duplicated stretchy hand that will be pushed away")]
    public Transform stretchyHand;
    
    [Tooltip("Root object containing all arm segments")]
    public Transform stretchyArmRoot;
    
    [Tooltip("All segments in order from shoulder to hand")]
    public List<Transform> armSegments = new List<Transform>();
    
    [Header("Stretch Settings")]
    [Tooltip("Maximum distance the hand can stretch")]
    public float maxStretchDistance = 5f;
    
    [Tooltip("Force applied to push hand away when stretching")]
    public float stretchForce = 20f;
    
    [Tooltip("How fast the arm retracts when button released")]
    public float retractSpeed = 10f;
    
    [Header("Physics Settings")]
    [Tooltip("Spring strength for segment connections")]
    public float springStrength = 50f;
    
    [Tooltip("Damping for spring joints (reduces oscillation)")]
    public float springDamper = 5f;
    
    [Tooltip("Mass of each segment")]
    public float segmentMass = 0.1f;
    
    [Tooltip("Drag applied to segments (air resistance)")]
    public float segmentDrag = 2f;
    
    [Header("Input")]
    [Tooltip("Button to hold for stretching")]
    public UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button stretchButton = 
        UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.Grip;
    
    private ActionBasedController controller;
    private List<Rigidbody> segmentRigidbodies = new List<Rigidbody>();
    private List<ConfigurableJoint> segmentJoints = new List<ConfigurableJoint>();
    private bool isStretched = false;
    private Rigidbody handRigidbody;
    private Vector3 stretchStartDirection;

    void Start()
    {
        // Get the controller component
        controller = xrControllerTransform.GetComponent<ActionBasedController>();
        
        // Hide stretchy hand initially
        if (stretchyHand != null)
        {
            stretchyHand.gameObject.SetActive(false);
        }
        
        SetupPhysics();
    }

    void SetupPhysics()
    {
        // Setup rigidbodies and joints for all segments
        for (int i = 0; i < armSegments.Count; i++)
        {
            Transform segment = armSegments[i];
            
            // Add or get Rigidbody
            Rigidbody rb = segment.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = segment.gameObject.AddComponent<Rigidbody>();
            }
            
            rb.mass = segmentMass;
            rb.linearDamping = segmentDrag;
            rb.angularDamping = segmentDrag;
            rb.useGravity = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            
            // Make kinematic initially (physics disabled until stretched)
            rb.isKinematic = true;
            
            segmentRigidbodies.Add(rb);
            
            // Create joints connecting segments
            if (i > 0)
            {
                ConfigurableJoint joint = segment.GetComponent<ConfigurableJoint>();
                if (joint == null)
                {
                    joint = segment.gameObject.AddComponent<ConfigurableJoint>();
                }
                
                // Connect to previous segment
                joint.connectedBody = segmentRigidbodies[i - 1];
                joint.autoConfigureConnectedAnchor = false;
                joint.anchor = new Vector3(0, 0, -0.5f); // Adjust based on your segment pivot
                joint.connectedAnchor = new Vector3(0, 0, 0.5f);
                
                // Configure spring behavior
                joint.xMotion = ConfigurableJointMotion.Limited;
                joint.yMotion = ConfigurableJointMotion.Limited;
                joint.zMotion = ConfigurableJointMotion.Limited;
                
                SoftJointLimit limit = new SoftJointLimit();
                limit.limit = 0.3f; // Allow slight compression/extension
                joint.linearLimit = limit;
                
                // Spring drive for that rubber effect
                JointDrive drive = new JointDrive();
                drive.positionSpring = springStrength;
                drive.positionDamper = springDamper;
                drive.maximumForce = Mathf.Infinity;
                
                joint.xDrive = drive;
                joint.yDrive = drive;
                joint.zDrive = drive;
                
                segmentJoints.Add(joint);
            }
        }
        
        // Setup the stretchy hand
        if (stretchyHand != null)
        {
            handRigidbody = stretchyHand.GetComponent<Rigidbody>();
            if (handRigidbody == null)
            {
                handRigidbody = stretchyHand.gameObject.AddComponent<Rigidbody>();
            }
            
            handRigidbody.mass = segmentMass;
            handRigidbody.linearDamping = segmentDrag;
            handRigidbody.useGravity = false;
            handRigidbody.isKinematic = true;
            
            // Connect hand to last segment if we have segments
            if (armSegments.Count > 0)
            {
                ConfigurableJoint handJoint = stretchyHand.GetComponent<ConfigurableJoint>();
                if (handJoint == null)
                {
                    handJoint = stretchyHand.gameObject.AddComponent<ConfigurableJoint>();
                }
                
                handJoint.connectedBody = segmentRigidbodies[segmentRigidbodies.Count - 1];
                handJoint.autoConfigureConnectedAnchor = false;
                handJoint.anchor = Vector3.zero;
                handJoint.connectedAnchor = new Vector3(0, 0, 0.5f);
                
                JointDrive drive = new JointDrive();
                drive.positionSpring = springStrength;
                drive.positionDamper = springDamper;
                drive.maximumForce = Mathf.Infinity;
                
                handJoint.xDrive = drive;
                handJoint.yDrive = drive;
                handJoint.zDrive = drive;
            }
        }
    }

    void Update()
    {
        // Check for stretch button input
        bool buttonPressed = IsButtonPressed();
        
        if (buttonPressed && !isStretched)
        {
            StartStretch();
        }
        else if (!buttonPressed && isStretched)
        {
            StopStretch();
        }
        
        if (isStretched)
        {
            UpdateStretchedArm();
        }
    }

    bool IsButtonPressed()
    {
        if (controller != null)
        {
            // For grip button, check the selectActionValue
            if (stretchButton == UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.Grip)
            {
                return controller.selectAction.action.ReadValue<float>() > 0.5f;
            }
            // For trigger, check activateActionValue
            else if (stretchButton == UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.Trigger)
            {
                return controller.activateAction.action.ReadValue<float>() > 0.5f;
            }
        }
        return false;
    }

    void StartStretch()
    {
        isStretched = true;
        
        Debug.Log("Starting stretch!");
        
        // Hide original hand, show stretchy hand
        if (originalHandMesh != null)
        {
            originalHandMesh.SetActive(false);
        }
        
        if (stretchyHand != null)
        {
            // Position stretchy hand at controller position before activating
            stretchyHand.position = xrControllerTransform.position;
            stretchyHand.rotation = xrControllerTransform.rotation;
            stretchyHand.gameObject.SetActive(true);
        }
        
        // Enable physics on all segments
        foreach (Rigidbody rb in segmentRigidbodies)
        {
            rb.isKinematic = false;
        }
        
        if (handRigidbody != null)
        {
            handRigidbody.isKinematic = false;
            
            // Calculate stretch direction (forward from controller)
            stretchStartDirection = xrControllerTransform.forward;
            
            // Apply force to push hand away from player
            handRigidbody.AddForce(stretchStartDirection * stretchForce, ForceMode.VelocityChange);
        }
    }

    void UpdateStretchedArm()
    {
        // The first segment should follow the real controller position
        if (armSegments.Count > 0 && segmentRigidbodies.Count > 0)
        {
            Rigidbody firstSegment = segmentRigidbodies[0];
            
            // Make first segment track the real hand position with physics
            Vector3 targetPos = xrControllerTransform.position;
            Vector3 force = (targetPos - firstSegment.position) * 100f;
            firstSegment.AddForce(force);
            
            // Apply rotation to match controller
            Quaternion targetRot = xrControllerTransform.rotation;
            Quaternion deltaRot = targetRot * Quaternion.Inverse(firstSegment.rotation);
            float angle;
            Vector3 axis;
            deltaRot.ToAngleAxis(out angle, out axis);
            
            if (angle > 180) angle -= 360;
            if (angle != 0 && !float.IsNaN(axis.x))
            {
                firstSegment.AddTorque(axis.normalized * angle * 10f);
            }
        }
        
        // Limit maximum stretch distance
        if (stretchyHand != null && xrControllerTransform != null)
        {
            float distance = Vector3.Distance(stretchyHand.position, xrControllerTransform.position);
            if (distance > maxStretchDistance)
            {
                Vector3 direction = (stretchyHand.position - xrControllerTransform.position).normalized;
                Vector3 clampedPos = xrControllerTransform.position + direction * maxStretchDistance;
                
                // Pull hand back if too far
                if (handRigidbody != null)
                {
                    Vector3 pullBack = (clampedPos - stretchyHand.position) * 50f;
                    handRigidbody.AddForce(pullBack);
                }
            }
        }
    }

    void StopStretch()
    {
        isStretched = false;
        
        Debug.Log("Stopping stretch!");
        
        // Disable physics
        foreach (Rigidbody rb in segmentRigidbodies)
        {
            rb.isKinematic = true;
        }
        
        if (handRigidbody != null)
        {
            handRigidbody.isKinematic = true;
        }
        
        // Show original hand, hide stretchy hand
        if (originalHandMesh != null)
        {
            originalHandMesh.SetActive(true);
        }
        
        if (stretchyHand != null)
        {
            stretchyHand.gameObject.SetActive(false);
        }
    }
}