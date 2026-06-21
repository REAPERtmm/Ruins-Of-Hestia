using System;
using UnityEngine;

public class BackgroundPlaylist : MonoBehaviour
{
    [SerializeField]
    public AudioSource player;
    public AudioClip[] clips;

    private int index = 0;
    void Start()
    {
        player.clip = clips[0];
        player.Play();
    }

    private void Update()
    {
        if (player.isPlaying == false
            && index < clips.Length)
        {
            if (index + 1 >= clips.Length)
                player.loop = true;

            index++;
            player.clip = clips[0];
            player.Play();
        }
    }
}
