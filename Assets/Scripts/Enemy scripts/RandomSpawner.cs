using UnityEngine;

public class SimpleDuplicateSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject template;          // disabled copy in scene
    public Transform[] spawnPoints;
    public float spawnInterval = 3f;

    [Header("Limit Settings")]
    public int maxActiveEnemies = 5;     // editable in Inspector

    private float timer;
    private int currentActiveEnemies = 0;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            TrySpawnFromTemplate();
            timer = 0f;
        }
    }

    private void TrySpawnFromTemplate()
    {
        if (currentActiveEnemies >= maxActiveEnemies)
            return; // cap reached, do not spawn

        SpawnFromTemplate();
    }

    private void SpawnFromTemplate()
    {
        if (template == null)
        {
            Debug.LogError("Spawner: No template assigned!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Spawner: No spawn points assigned!");
            return;
        }

        int index = Random.Range(0, spawnPoints.Length);
        Transform spawn = spawnPoints[index];

        // Create clone from disabled template
        GameObject clone = Instantiate(template, spawn.position, spawn.rotation);
        clone.SetActive(true);

        // Register this enemy so we can decrease count when it dies
        currentActiveEnemies++;

        // Hook into Unit/Enemy death if available
        var unit = clone.GetComponentInParent<Unit>();
        if (unit != null)
        {
            unit.OnDied += () =>
            {
                currentActiveEnemies = Mathf.Max(0, currentActiveEnemies - 1);
            };
        }

        // Optional: Clear ragdoll physics
        foreach (var rb in clone.GetComponentsInChildren<Rigidbody>())
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
