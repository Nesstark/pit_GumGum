using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public UnityEngine.AI.NavMeshAgent navMeshAgent;
    private bool isVulnerable = false; // Tracks if the enemy can take damage
    private Collider mainCollider;
    private Collider[] ragdollColliders;
    private Animator animator;

    private Renderer enemyRenderer; // Renderer to change material color
    private Color originalColor;    // To store the original color of the enemy
    public Color vulnerableColor = Color.red; // Color to display when vulnerable


    private void Awake()
    {
        // Get the main collider, ragdoll colliders, and animator
        mainCollider = GetComponent<Collider>();
        ragdollColliders = GetComponentsInChildren<Collider>();
        animator = GetComponentInChildren<Animator>();

        // Get the Renderer component to manipulate material color
        enemyRenderer = GetComponentInChildren<Renderer>();
        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color; // Save the original color
        }

        // Disable ragdoll by default
        ActivateRagdoll(false);

        // Initialize health
        currentHealth = maxHealth;
    }

    private void ActivateRagdoll(bool status)
    {
        foreach (Collider col in ragdollColliders)
        {
            if (col != mainCollider) // Ensure the main collider is excluded
            {
                col.enabled = status;
                Rigidbody rb = col.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = !status;
                }
            }
        }

        // Toggle the main collider and animator
        mainCollider.enabled = !status;
        animator.enabled = !status;

        // Disable gravity for the main Rigidbody (if present) when activating ragdoll
        Rigidbody mainRb = GetComponent<Rigidbody>();
        if (mainRb != null)
        {
            mainRb.isKinematic = status;
        }
    }

    public void TakeDamage(int amount)
    {
        if (!isVulnerable)
        {
            Debug.Log("Enemy is invulnerable and cannot be damaged.");
            return;
        }

        currentHealth -= amount;
        Debug.Log("Enemy Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die(); // Default behavior
        }
    }

    public void TakeDamageBypass(int amount)
    {
        currentHealth -= amount; // Ignores invulnerability
        Debug.Log("Enemy Health (Bypass): " + currentHealth);

        if (currentHealth <= 0)
        {
            Die(); // Default behavior
        }
    }

    public void EnableVulnerability(float duration = -1f)
    {
        isVulnerable = true;
        Debug.Log(gameObject.name + " is now vulnerable!");

        // Change the material color to vulnerableColor
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = vulnerableColor;
        }

    }


    public void DisableVulnerability()
    {
        isVulnerable = false;
        Debug.Log(gameObject.name + " is now invulnerable!");

        // Revert the material color to the original color
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = originalColor;
        }
    }

    public bool IsVulnerable()
    {
        return isVulnerable; // Return the current vulnerability status
    }

    public void Die()
    {
        Debug.Log("Enemy " + gameObject.name + " has died!");

        // Stop monster sound
        MonsterSound monsterSound = GetComponent<MonsterSound>();
        if (monsterSound != null)
        {
            monsterSound.StopMonsterSound();
        }

        // Disable NavMeshAgent if it's assigned
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
            Debug.Log("NavMeshAgent stopped.");
        }

        // Enable ragdoll
        ActivateRagdoll(true);
    }
}
