using UnityEngine;

public class SimpleDuplicateSpawner : MonoBehaviour
{
    public GameObject template;      // disabled copy in scene
    public Transform[] spawnPoints;
    public float spawnInterval = 3f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnFromTemplate();
            timer = 0f;
        }
    }

    void SpawnFromTemplate()
    {
        if (template == null)
        {
            Debug.LogError("Spawner: No template assigned!");
            return;
        }

        int index = Random.Range(0, spawnPoints.Length);
        Transform spawn = spawnPoints[index];

        // Create clone from disabled template
        GameObject clone = Instantiate(template, spawn.position, spawn.rotation);

        // Enable it AFTER it is placed so nothing explodes
        clone.SetActive(true);

        // Optional: Clear ragdoll physics
        foreach (var rb in clone.GetComponentsInChildren<Rigidbody>())
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
