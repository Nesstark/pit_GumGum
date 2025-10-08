using System.Collections.Generic;
using UnityEngine;

public class CubeDestruction : MonoBehaviour
{
    public float raycastRange = 20f;   // Range of the raycast
    public float allowedTime = 3f;   // Time required to make the enemy vulnerable
    private Dictionary<GameObject, float> exposureTimes = new Dictionary<GameObject, float>(); // Tracks exposure times of enemies

    private void Update()
    {
        // Cast a ray from the cube's position
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward); // Ray pointing forward from the cube

        if (Physics.Raycast(ray, out hit, raycastRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                GameObject enemy = hit.collider.gameObject;

                // Update exposure time for this enemy
                if (!exposureTimes.ContainsKey(enemy))
                {
                    exposureTimes[enemy] = 0f; // Start tracking this enemy
                }

                exposureTimes[enemy] += Time.deltaTime;

                // If the enemy has been exposed long enough, make it vulnerable
                if (exposureTimes[enemy] >= allowedTime)
                {
                    EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                    if (enemyHealth != null && !enemyHealth.IsVulnerable())
                    {
                        enemyHealth.EnableVulnerability();
                    }
                }
            }
        }

    }

}
