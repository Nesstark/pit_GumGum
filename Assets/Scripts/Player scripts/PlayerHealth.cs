using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;      // Maximum health of the player
    private int currentHealth;       // Current health of the player
    private bool canTakeDamage = true; // Tracks if the player can take damage
    public float damageCooldown = 5f; // Cooldown time in seconds
    private float damageTimer = 0f;   // Timer for how long the enemy has been colliding with the player
    private bool isCollidingWithEnemy = false; // Tracks if the player is colliding with the enemy

    void Start()
    {
        currentHealth = maxHealth; // Initialize health to maxHealth at the start
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Start the timer when the player starts colliding with an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            isCollidingWithEnemy = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // If still colliding with the enemy, increment the timer
        if (collision.gameObject.CompareTag("Enemy") && isCollidingWithEnemy)
        {
            damageTimer += Time.deltaTime;

            // If the player has been colliding for 1 second, deal damage
            if (damageTimer >= 2f && canTakeDamage)
            {
                TakeDamage(30); // Deal 30 damage after 1 second of contact
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Reset the timer when the player stops colliding with the enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            isCollidingWithEnemy = false;
            damageTimer = 0f; // Reset the timer
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Reduce health by damage amount
        Debug.Log("Player Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die(); // If health drops to 0 or below, die
        }
        else
        {
            StartCoroutine(DamageCooldown()); // Start the damage cooldown
        }
    }

    void Die()
    {
        Debug.Log("Player has died!");
        // Add any death logic here (e.g., respawn, game over screen)
        // For now, we destroy the player object
        Destroy(gameObject);
    }

    private System.Collections.IEnumerator DamageCooldown()
    {
        canTakeDamage = false; // Disable further damage
        yield return new WaitForSeconds(damageCooldown); // Wait for the cooldown duration
        canTakeDamage = true; // Re-enable damage after the cooldown
    }
}
