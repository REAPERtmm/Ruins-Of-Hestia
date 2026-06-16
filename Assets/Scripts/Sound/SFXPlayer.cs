using System;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public static SFXPlayer instance;

    private static AudioSource audioSource;
    private void Start()
    {
        if (instance)
            return;

        DontDestroyOnLoad(this);
        instance = this;
        audioSource = gameObject.AddComponent<AudioSource>();

    }

    public static void PlaySFX( AudioClip clip, float volume = 1.0f )
    {
        audioSource.volume = volume;
        audioSource.clip = clip;
        audioSource.Play();
    }
}
