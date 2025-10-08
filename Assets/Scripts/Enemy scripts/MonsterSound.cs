using System.Collections;
using UnityEngine;

public class MonsterSound : MonoBehaviour
{
    public AudioClip monsterSound;   // Sound clip for the monster
    public float interval = 5f;     // Interval between sounds in seconds
    [Range(0f, 1f)] public float volume = 0.01f; // Volume of the sound, adjustable in the Inspector
    private AudioSource audioSource;
    private bool isAlive = true;    // Tracks if the monster is alive

    void Start()
    {
        // Get or add an AudioSource component
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Set the clip, volume, and play the sound at intervals
        audioSource.clip = monsterSound;
        audioSource.volume = volume; // Set volume based on the Inspector value
        audioSource.loop = false;    // Ensure it does not loop automatically
        InvokeRepeating(nameof(PlayMonsterSound), 0f, interval);
    }

    void Update()
    {
        // Dynamically update the volume in case it's changed in the Inspector
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    void PlayMonsterSound()
    {
        if (isAlive && audioSource != null && monsterSound != null)
        {
            audioSource.PlayOneShot(monsterSound);
        }
    }

    // Public method to stop the sound when the monster dies
    public void StopMonsterSound()
    {
        isAlive = false;
        CancelInvoke(nameof(PlayMonsterSound));
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
