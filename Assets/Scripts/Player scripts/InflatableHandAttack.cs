using UnityEngine;
using UnityEngine.InputSystem;

public class InflatableHandAttack : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private InputActionProperty inflateButtonAction; // Button to press for inflation
    
    [Header("Hand References")]
    [SerializeField] private Transform handTransform; // The hand object to scale (hands:hands_geom)
    [SerializeField] private Transform headTransform; // Camera/head position
    [SerializeField] private Transform controllerTransform; // The right hand parent (for position checking)
    
    [Header("Inflation Settings")]
    [SerializeField] private float inflatedScale = 3f; // How big the hand gets
    [SerializeField] private float inflationSpeed = 8f; // How fast it inflates
    [SerializeField] private float deflationSpeed = 5f; // How fast it deflates
    [SerializeField] private float faceDetectionDistance = 0.4f; // How close hand needs to be to face
    [SerializeField] private float inflationDuration = 3f; // Max time hand stays inflated
    
    [Header("Combat Settings")]
    [SerializeField] private float normalDamage = 10f;
    [SerializeField] private float inflatedDamage = 50f;
    [SerializeField] private float normalForce = 100f;
    [SerializeField] private float inflatedForce = 500f;
    [SerializeField] private LayerMask enemyLayer;
    
    [Header("Visual Feedback")]
    [SerializeField] private ParticleSystem inflationEffect;
    [SerializeField] private AudioSource inflationSound;
    
    [Header("Haptic Feedback (Optional)")]
    [SerializeField] private InputActionProperty hapticDeviceAction; // Optional: for haptic feedback
    
    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isInflated = false;
    private bool isInflating = false;
    private float inflationTimer = 0f;
    private Rigidbody handRigidbody;
    private bool buttonWasPressed = false; // For edge detection
    
    private void Start()
    {
        // Store original hand scale
        if (handTransform != null)
        {
            originalScale = handTransform.localScale;
            targetScale = originalScale;
        }
        
        // Get or add Rigidbody for collision detection
        if (handTransform != null)
        {
            handRigidbody = handTransform.GetComponent<Rigidbody>();
            if (handRigidbody == null)
            {
                handRigidbody = handTransform.gameObject.AddComponent<Rigidbody>();
                handRigidbody.useGravity = false;
                handRigidbody.isKinematic = true;
            }
        }
        
        // Enable the input action
        if (inflateButtonAction.action != null)
        {
            inflateButtonAction.action.Enable();
        }
        
        // Use controller transform if not manually set
        if (controllerTransform == null)
        {
            controllerTransform = transform;
        }
    }
    
    private void Update()
    {
        // Check for button press (edge detection)
        bool buttonPressed = IsButtonPressed();
        bool buttonJustPressed = buttonPressed && !buttonWasPressed;
        buttonWasPressed = buttonPressed;
        
        // Check if hand is near face and button is pressed
        if (IsHandNearFace() && buttonJustPressed && !isInflated && !isInflating)
        {
            StartInflation();
        }
        
        // Handle inflation timer
        if (isInflated)
        {
            inflationTimer -= Time.deltaTime;
            if (inflationTimer <= 0f)
            {
                StartDeflation();
            }
        }
        
        // Smoothly scale the hand
        if (handTransform != null)
        {
            float speed = isInflating ? inflationSpeed : deflationSpeed;
            handTransform.localScale = Vector3.Lerp(handTransform.localScale, targetScale, Time.deltaTime * speed);
            
            // Check if inflation/deflation is complete
            if (isInflating && Vector3.Distance(handTransform.localScale, targetScale) < 0.01f)
            {
                isInflating = false;
                isInflated = true;
                inflationTimer = inflationDuration;
            }
            else if (!isInflated && !isInflating && Vector3.Distance(handTransform.localScale, originalScale) < 0.01f)
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
        
        // Play effects
        if (inflationEffect != null)
            inflationEffect.Play();
        
        if (inflationSound != null)
            inflationSound.Play();
        
        // Haptic feedback (if configured)
        SendHapticFeedback(0.5f, 0.2f);
        
        Debug.Log("Hand inflation started!");
    }
    
    private void StartDeflation()
    {
        isInflated = false;
        targetScale = originalScale;
        
        // Small haptic feedback for deflation
        SendHapticFeedback(0.2f, 0.1f);
        
        Debug.Log("Hand deflating...");
    }
    
    private void SendHapticFeedback(float amplitude, float duration)
    {
        // Simple haptic feedback using Input System
        if (hapticDeviceAction.action != null)
        {
            var device = hapticDeviceAction.action.activeControl?.device;
            if (device != null)
            {
                // Try to send haptic command if device supports it
                var command = UnityEngine.InputSystem.XR.Haptics.SendHapticImpulseCommand.Create(0, amplitude, duration);
                device.ExecuteCommand(ref command);
            }
        }
    }
    
    // Call this from collision detection or trigger
    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & enemyLayer) != 0)
        {
            HitEnemy(collision);
        }
    }
    
    private void HitEnemy(Collision collision)
    {
        float damage = isInflated ? inflatedDamage : normalDamage;
        float force = isInflated ? inflatedForce : normalForce;
        
        // Apply damage if enemy has health component
        var enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }
        
        // Apply physics force
        Rigidbody enemyRb = collision.rigidbody;
        if (enemyRb != null)
        {
            Vector3 forceDirection = (collision.transform.position - handTransform.position).normalized;
            enemyRb.AddForce(forceDirection * force, ForceMode.Impulse);
        }
        
        // Strong haptic feedback on hit
        float hapticStrength = isInflated ? 1.0f : 0.3f;
        SendHapticFeedback(hapticStrength, 0.15f);
        
        Debug.Log($"Hit enemy with {(isInflated ? "INFLATED" : "normal")} hand! Damage: {damage}");
    }
    
    // Public methods for external access
    public bool IsHandInflated()
    {
        return isInflated;
    }
    
    public float GetCurrentDamageMultiplier()
    {
        return isInflated ? (inflatedDamage / normalDamage) : 1f;
    }
    
    private void OnDestroy()
    {
        // Disable the input action when destroyed
        if (inflateButtonAction.action != null)
        {
            inflateButtonAction.action.Disable();
        }
    }
}

// Simple enemy health component for testing
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
        {
            Die();
        }
    }
    
    private void Die()
    {
        Debug.Log($"{gameObject.name} defeated!");
        // Add death effects, ragdoll, etc.
        Destroy(gameObject, 0.1f);
    }
}