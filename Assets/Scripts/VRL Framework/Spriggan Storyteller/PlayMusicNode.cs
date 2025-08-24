using System;
using UnityEngine;

[Serializable]
public class PlayMusicNode : SprigganNode
{


    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip clip;
    [SerializeField][Min(0.1f)] public float duration;

    public PlayMusicNode()
    {
        duration = 0.1f;
    }

    public override async Awaitable Execute()
    {

        if(audioSource == null)
        {
            Debug.LogWarning("Audio Source is null!");
            return;
        }

        if(clip == null)
        {
            Debug.LogWarning("Clip assigned is null!");
            return;
        }

        audioSource.clip = clip;
        audioSource.Play();
        await Awaitable.WaitForSecondsAsync(duration);
        audioSource.Stop();

    }

}
