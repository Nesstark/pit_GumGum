using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class ExtendingArmVR : MonoBehaviour
{
    public enum InputButton
    {
        Trigger,
        Grip,
        PrimaryButton,
        SecondaryButton
    }

    [Header("References")]
    [SerializeField] private Transform handTransform;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private LineRenderer armRenderer;

    [Header("Input Actions")]
    [Tooltip("Input action for trigger (float 0–1)")]
    [SerializeField] private InputActionReference triggerAction;
    [Tooltip("Input action for grip (float 0–1)")]
    [SerializeField] private InputActionReference gripAction;
    [Tooltip("Input action for primary button (bool or float)")]
    [SerializeField] private InputActionReference primaryButtonAction;
    [Tooltip("Input action for secondary button (bool or float)")]
    [SerializeField] private InputActionReference secondaryButtonAction;

    [Header("Arm Settings")]
    [SerializeField] private float normalArmLength = 0.6f;
    [SerializeField] private float maxExtendLength = 5f;
    [SerializeField] private float extendSpeed = 15f;
    [SerializeField] private float retractSpeed = 10f;
    [SerializeField] private float armWidth = 0.05f;
    [SerializeField] private Vector3 shoulderOffset = new Vector3(0.2f, -0.25f, 0.1f);

    [Header("Activation")]
    [SerializeField] private InputButton extendButton = InputButton.Trigger;
    [SerializeField] private float buttonThreshold = 0.7f;

    [Header("Visuals")]
    [SerializeField] private Material armMaterial;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color extendedColor = Color.red;
    [SerializeField] private bool elasticVisuals = true;
    [SerializeField] private float visualSmoothSpeed = 10f;

    [Header("Hit Detection")]
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private bool debugDrawRay = false;
    [SerializeField] private float hitCooldown = 0.3f;

    private float currentExtension = 0f;
    private float lastHitTime = 0f;
    private bool isExtending = false;
    private Vector3 currentVisualEndPos;

    void Awake()
    {
        if (armRenderer == null)
            armRenderer = GetComponent<LineRenderer>();

        SetupLineRenderer();
        currentVisualEndPos = handTransform.position;
    }

    void SetupLineRenderer()
    {
        armRenderer.positionCount = 2;
        armRenderer.startWidth = armWidth;
        armRenderer.endWidth = armWidth;
        armRenderer.useWorldSpace = true;

        if (armMaterial == null)
        {
            Shader unlit = Shader.Find("Universal Render Pipeline/Unlit");
            armRenderer.material = new Material(unlit);
        }
        else
        {
            armRenderer.material = armMaterial;
        }

        armRenderer.startColor = normalColor;
        armRenderer.endColor = normalColor;
    }

    void Update()
    {
        HandleExtension();
        UpdateArmVisual();
        HandleHitDetection();
    }

    void HandleExtension()
    {
        float buttonValue = GetButtonValue();
        isExtending = buttonValue > buttonThreshold;

        float target = isExtending ? maxExtendLength : 0f;
        currentExtension = Mathf.MoveTowards(
            currentExtension, target,
            (isExtending ? extendSpeed : retractSpeed) * Time.deltaTime
        );
    }

    void UpdateArmVisual()
    {
        Vector3 shoulderPos = GetShoulderPosition();
        Vector3 direction = (handTransform.position - shoulderPos).normalized;
        float targetDistance = normalArmLength + currentExtension;
        Vector3 targetEndPos = shoulderPos + direction * targetDistance;

        if (elasticVisuals)
            currentVisualEndPos = Vector3.Lerp(currentVisualEndPos, targetEndPos, Time.deltaTime * visualSmoothSpeed);
        else
            currentVisualEndPos = targetEndPos;

        armRenderer.SetPosition(0, shoulderPos);
        armRenderer.SetPosition(1, currentVisualEndPos);

        float extensionPercent = currentExtension / maxExtendLength;
        Color currentColor = Color.Lerp(normalColor, extendedColor, extensionPercent);
        armRenderer.startColor = currentColor;
        armRenderer.endColor = currentColor;
    }

    void HandleHitDetection()
    {
        if (currentExtension <= 0.1f || Time.time < lastHitTime + hitCooldown)
            return;

        Vector3 shoulderPos = GetShoulderPosition();
        Vector3 direction = (handTransform.position - shoulderPos).normalized;
        float rayDistance = normalArmLength + currentExtension;

        if (debugDrawRay)
            Debug.DrawRay(shoulderPos, direction * rayDistance, Color.red, 0.1f);

        if (Physics.Raycast(shoulderPos, direction, out RaycastHit hit, rayDistance, hitLayers))
        {
            lastHitTime = Time.time;
            if (hit.rigidbody != null)
                hit.rigidbody.AddForce(direction * 10f, ForceMode.Impulse);
        }
    }

    Vector3 GetShoulderPosition()
    {
        Vector3 flatForward = new Vector3(cameraTransform.forward.x, 0f, cameraTransform.forward.z).normalized;
        Vector3 shoulderPos = cameraTransform.position
                            + flatForward * shoulderOffset.z
                            + Vector3.up * shoulderOffset.y
                            + cameraTransform.right * shoulderOffset.x;
        return shoulderPos;
    }

    float GetButtonValue()
    {
        InputActionReference actionRef = null;
        switch (extendButton)
        {
            case InputButton.Trigger:
                actionRef = triggerAction;
                break;
            case InputButton.Grip:
                actionRef = gripAction;
                break;
            case InputButton.PrimaryButton:
                actionRef = primaryButtonAction;
                break;
            case InputButton.SecondaryButton:
                actionRef = secondaryButtonAction;
                break;
        }

        if (actionRef == null || actionRef.action == null)
            return 0f;

        return actionRef.action.ReadValue<float>();
    }

    public bool IsExtended => currentExtension > normalArmLength * 0.5f;
    public float ExtensionPercent => currentExtension / maxExtendLength;
}
