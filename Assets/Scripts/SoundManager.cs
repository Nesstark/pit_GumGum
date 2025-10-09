using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Range(0f,1f)] public float sfxVolume = 1f;

    private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.volume = sfxVolume;
    }

    public void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, Mathf.Clamp01(sfxVolume * volumeScale));
    }

    public void PlaySFXAtPosition(AudioClip clip, Vector3 position, float volumeScale = 1f)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, position, Mathf.Clamp01(sfxVolume * volumeScale));
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null) sfxSource.volume = sfxVolume;
    }
}