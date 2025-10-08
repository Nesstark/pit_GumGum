using System.Collections;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    public int health = 2;                 // Number of hits the barrel can take
    public float explosionDelay = 2f;     // Time before the barrel explodes after the first shot
    public float explosionRadius = 5f;    // Radius of the explosion
    public int explosionDamage = 100;     // Damage dealt to objects within the explosion radius
    public float explosionForce = 30f;    // Force applied to nearby objects when the barrel explodes
    public GameObject explosionEffect;    // Prefab for explosion VFX
    public AudioClip explosionSound;      // Sound clip for the explosion

    private bool isExploding = false;     // Tracks whether the barrel is already set to explode
    private AudioSource audioSource;      // Reference to the audio source component

    void Start()
    {
        // Get or add an AudioSource component
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isExploding) return;

        health -= damage;

        if (health <= 2)
        {
            // Explode immediately
            audioSource.PlayOneShot(explosionSound);
            Explode();
            audioSource.PlayOneShot(explosionSound);
        }
        else
        {
            // Start delayed explosion
            StartCoroutine(DelayedExplosion());
        }
    }

    IEnumerator DelayedExplosion()
    {
        isExploding = true;

        yield return new WaitForSeconds(explosionDelay);

        // Explode if not already destroyed
        if (health > 0)
        {
            audioSource.PlayOneShot(explosionSound);
            Explode();
            
        }
    }

    void Explode()
    {
        // Instantiate explosion effect
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }


        // Find all colliders in the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            // Apply damage bypassing invulnerability
            EnemyHealth enemyHealth = nearbyObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamageBypass(explosionDamage);
            }

            // Apply force if the object has a Rigidbody
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = nearbyObject.transform.position - transform.position;
                rb.AddForce(direction.normalized * explosionForce, ForceMode.Impulse);
            }
        }

        // Destroy the barrel
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the explosion radius in the editor for visualization
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
