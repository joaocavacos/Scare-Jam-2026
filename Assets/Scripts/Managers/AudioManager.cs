using System;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private AudioClip ambienceSound;

    protected override void Awake()
    {
        base.Awake();
        musicSource.loop = true;
    }

    private void Start()
    {
        PlayMusic(ambienceSound);
    }

    // Music
    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        if (clip == null)
        {
            return;
        }
        
        musicSource.clip = clip;
        musicSource.volume = AudioListener.volume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    // Sound Effects

    /// <summary>
    /// Plays a standard 2D sound
    /// </summary>
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    /// <summary>
    /// Plays a 3D sound at a specific location
    /// </summary>
    public void PlaySFXAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null)
        {
            return;
        }
        
        // Creates a temporary GameObject to play the sound at the exact location
        GameObject tempAudioObj = new GameObject("TempAudio_" + clip.name);
        tempAudioObj.transform.position = position;
        
        AudioSource tempSource = tempAudioObj.AddComponent<AudioSource>();
        tempSource.clip = clip;
        tempSource.volume = AudioListener.volume;

        // Configuration for 3D space
        tempSource.spatialBlend = 1f;
        tempSource.minDistance = 2f;  // Distance where sound is at maximum volume
        tempSource.maxDistance = 20f; // Distance where sound fades to zero
        tempSource.rolloffMode = AudioRolloffMode.Linear;
        
        tempSource.Play();

        Destroy(tempAudioObj, clip.length);
    }
}
