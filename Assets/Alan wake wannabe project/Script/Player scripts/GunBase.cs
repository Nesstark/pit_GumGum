using UnityEngine;

public class GunBase : MonoBehaviour
{
    public Camera gunCamera;      // Assign the camera used for aiming
    public int damage = 15;       // Damage per shot
    public float range = 200f;    // Shooting range
    public float fireRate = 1.5f; // Time between shots
    public float pushForce = 5f;  // Force to push the enemy back

    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime) // Left mouse button
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        // Create a ray from the center of the screen
        Ray ray = gunCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
        {
            // Check if the hit object has an EnemyHealth component
            EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // Apply a force to move the enemy back if it has a Rigidbody
            Rigidbody enemyRigidbody = hit.transform.GetComponent<Rigidbody>();
            if (enemyRigidbody != null)
            {
                Vector3 pushDirection = hit.transform.position - ray.origin; // Direction from the gun to the hit object
                pushDirection.Normalize(); // Normalize the direction to ensure consistent force application
                enemyRigidbody.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            }
            ExplosiveBarrel barrel = hit.transform.GetComponent<ExplosiveBarrel>();

            if (barrel != null)
            {
                barrel.TakeDamage(1); // Barrel takes 1 damage per shot
            }
            // Optional: Add hit effects like particles or decals
            Debug.Log("Hit: " + hit.transform.name);
        }
    }
}
