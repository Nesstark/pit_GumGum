using UnityEngine;
using UnityEngine.InputSystem;

public class InflatableHandAttack : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private InputActionProperty inflateButtonAction;

    [Header("Hand References")]
    [SerializeField] private Transform handTransform;
    [SerializeField] private Transform headTransform;
    [SerializeField] private Transform controllerTransform;

    [Header("Inflation Settings")]
    [SerializeField] private float inflatedScale = 3f;
    [SerializeField] private float inflationSpeed = 8f;
    [SerializeField] private float deflationSpeed = 5f;
    [SerializeField] private float faceDetectionDistance = 0.4f;
    [SerializeField] private float inflationDuration = 3f;

    [Header("Combat Settings")]
    [SerializeField] private int normalDamage = 10;
    [SerializeField] private int inflatedDamage = 50;
    [SerializeField] private float normalForce = 400f;
    [SerializeField] private float inflatedForce = 900f;
    [SerializeField] private float knockUpFactor = 0.3f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Visual Feedback")]
    [SerializeField] private ParticleSystem inflationEffect;
    [SerializeField] private AudioSource inflationSound;

    [Header("Haptic Feedback (Optional)")]
    [SerializeField] private InputActionProperty hapticDeviceAction;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isInflated = false;
    private bool isInflating = false;
    private float inflationTimer = 0f;
    private Rigidbody handRigidbody;
    private bool buttonWasPressed = false;

    private void Start()
    {
        // Store original scale
        if (handTransform != null)
        {
            originalScale = handTransform.localScale;
            targetScale = originalScale;
        }

        // Ensure Rigidbody for collisions
        if (handTransform != null)
        {
            handRigidbody = handTransform.GetComponent<Rigidbody>();
            if (handRigidbody == null)
            {
                handRigidbody = handTransform.gameObject.AddComponent<Rigidbody>();
            }
            handRigidbody.useGravity = false;
            handRigidbody.isKinematic = true;
        }

        if (inflateButtonAction.action != null)
            inflateButtonAction.action.Enable();

        if (controllerTransform == null)
            controllerTransform = transform;
    }

    private void Update()
    {
        bool buttonPressed = IsButtonPressed();
        bool buttonJustPressed = buttonPressed && !buttonWasPressed;
        buttonWasPressed = buttonPressed;

        if (IsHandNearFace() && buttonJustPressed && !isInflated && !isInflating)
        {
            StartInflation();
        }

        if (isInflated)
        {
            inflationTimer -= Time.deltaTime;
            if (inflationTimer <= 0f)
                StartDeflation();
        }

        if (handTransform != null)
        {
            float speed = isInflating ? inflationSpeed : deflationSpeed;
            handTransform.localScale = Vector3.Lerp(handTransform.localScale, targetScale, Time.deltaTime * speed);

            if (isInflating && Vector3.Distance(handTransform.localScale, targetScale) < 0.01f)
            {
                isInflating = false;
                isInflated = true;
                inflationTimer = inflationDuration;
            }
            else if (!isInflated && !isInflating &&
                     Vector3.Distance(handTransform.localScale, originalScale) < 0.01f)
            {
                handTransform.localScale = originalScale;
            }
        }
    }

    private bool IsHandNearFace()
    {
        if (controllerTransform == null || headTransform == null)
            return false;

        float distance = Vector3.Distance(controllerTransform.position, headTransform.position);
        return distance <= faceDetectionDistance;
    }

    private bool IsButtonPressed()
    {
        if (inflateButtonAction.action == null)
            return false;

        return inflateButtonAction.action.ReadValue<float>() > 0.5f;
    }

    private void StartInflation()
    {
        isInflating = true;
        targetScale = originalScale * inflatedScale;

        if (inflationEffect != null)
            inflationEffect.Play();

        if (inflationSound != null)
            inflationSound.Play();

        SendHapticFeedback(0.5f, 0.2f);
        Debug.Log("Hand inflation started!");
    }

    private void StartDeflation()
    {
        isInflated = false;
        targetScale = originalScale;

        SendHapticFeedback(0.2f, 0.1f);
        Debug.Log("Hand deflating...");
    }

    private void SendHapticFeedback(float amplitude, float duration)
    {
        if (hapticDeviceAction.action == null)
            return;

        var device = hapticDeviceAction.action.activeControl?.device;
        if (device == null) return;

        var command = UnityEngine.InputSystem.XR.Haptics
            .SendHapticImpulseCommand.Create(0, amplitude, duration);
        device.ExecuteCommand(ref command);
    }

    // DAMAGE PART – works with Unit / EnemyHealth
    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & enemyLayer.value) == 0)
            return;

        HitEnemy(collision);
    }

    private void HitEnemy(Collision collision)
    {
        int damageInt = isInflated ? inflatedDamage : normalDamage;
        float force = isInflated ? inflatedForce : normalForce;

        // Prefer your Unit system
        Unit unit = collision.gameObject.GetComponentInParent<Unit>();
        if (unit != null)
        {
            unit.TakeDamage(damageInt);
        }
        else
        {
            // Legacy fallback
            var enemyHealth = collision.gameObject.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null)
                enemyHealth.TakeDamage(damageInt);
        }

        // Apply physics force with contact-based direction and knock-up
        Rigidbody enemyRb = collision.rigidbody;
        if (enemyRb != null)
        {
            Vector3 handPos = (handTransform != null) ? handTransform.position : transform.position;
            ContactPoint contact = collision.GetContact(0);

            Vector3 baseDir = (contact.point - handPos).normalized;
            Vector3 knockDir = (baseDir + Vector3.up * knockUpFactor).normalized;

            enemyRb.AddForce(knockDir * force, ForceMode.Impulse);
        }

        float hapticStrength = isInflated ? 1.0f : 0.3f;
        SendHapticFeedback(hapticStrength, 0.15f);

        Debug.Log($"Hit enemy with {(isInflated ? "INFLATED" : "normal")} hand! Damage: {damageInt}, Force: {force}");
    }

    public bool IsHandInflated() => isInflated;
    public float GetCurrentDamageMultiplier() =>
        isInflated ? (float)inflatedDamage / normalDamage : 1f;

    private void OnDestroy()
    {
        if (inflateButtonAction.action != null)
            inflateButtonAction.action.Disable();
    }

    // Optional simple test health – can be removed when using Unit-based enemies
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        private float currentHealth;

        private void Start()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");
            if (currentHealth <= 0)
                Die();
        }

        private void Die()
        {
            Debug.Log($"{gameObject.name} defeated!");
            Destroy(gameObject, 0.1f);
        }
    }
}
