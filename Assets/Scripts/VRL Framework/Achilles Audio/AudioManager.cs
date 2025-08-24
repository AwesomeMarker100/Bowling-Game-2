using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum SpatialType
    {

        Close,
        Medium,
        Far,
        Alabama

    }

    public enum SpatialDirection
    {
        Left,
        Right,
        Forward,
        Back,
        
        Down,
        DownRight,
        DownLeft,
        DownForward,
        DownBack,

        Up,
        UpRight,
        UpLeft,
        UpForward,
        UpBack

    }

    [SerializeField] static float closeDistance = 5f;
    [SerializeField] static float mediumDistance = 10f;
    [SerializeField] static float farDistance = 20f;
    [SerializeField] static float alabamaDistance = 30f;

    [SerializeField] static AudioListener audioListener;
    [SerializeField] static AudioSource[] sounds;
    

    static HashSet<AudioSource> soundsList;

    // Start is called before the first frame update
    void Start()
    {

        soundsList = new HashSet<AudioSource>();

        foreach (AudioSource source in FindObjectsOfType<AudioSource>())
        {
            soundsList.Add(source);

        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static AudioSource CreateAudioSource()
    {
        return new GameObject("Audio Source").AddComponent<AudioSource>();
    }

    public static AudioSource PlaySound(AudioClip clip, Vector3 pos)
    {
        AudioSource audioSource = CreateAudioSource();
        soundsList.Add(audioSource);

        audioSource.clip = clip;
        audioSource.transform.position = pos;

        audioSource.Play();
        return audioSource;
    }

    public static AudioSource PlaySound(AudioClip clip, SpatialType spatialType, SpatialDirection spatialDirection)
    {

        Vector3 spatialPos = GetSpatialPos(spatialDirection, spatialType);
        return PlaySound(clip, spatialPos);
        

    }

    public static void PlaySound(AudioSource source, AudioClip clip)
    {

        source.clip = clip;
        source.Play();

    }

    public static void MoveSound(AudioSource source, Vector3 pos)
    {

        source.transform.position = pos;

    }

    public static Vector3 GetRandomSpatialPos(float minDistance, float maxDistance)
    {

        return audioListener.transform.position + (Random.Range(minDistance, maxDistance) * Random.insideUnitSphere);

    }


    public static Vector3 GetSpatialPos(SpatialDirection spatialDirection, SpatialType spatialType)
    {

        switch(spatialDirection){

            case SpatialDirection.Left:

                return GetDistance(spatialType) * -audioListener.transform.right;

            case SpatialDirection.Right:
                return GetDistance(spatialType) * audioListener.transform.right;

            case SpatialDirection.Up:
                return GetDistance(spatialType) * audioListener.transform.up;

            case SpatialDirection.Down:
                return GetDistance(spatialType) * -audioListener.transform.up;

            case SpatialDirection.Forward:
                return GetDistance(spatialType) * audioListener.transform.forward;

            case SpatialDirection.Back:
                return GetDistance(spatialType) * -audioListener.transform.forward;

            case SpatialDirection.UpLeft:
                return GetDistance(spatialType) * (audioListener.transform.up - audioListener.transform.right);

            case SpatialDirection.UpRight:
                return GetDistance(spatialType) * (audioListener.transform.up + audioListener.transform.right);

            case SpatialDirection.UpForward:
                return GetDistance(spatialType) * (audioListener.transform.up + audioListener.transform.forward);

            case SpatialDirection.UpBack:
                return GetDistance(spatialType) * (audioListener.transform.up - audioListener.transform.forward);

            case SpatialDirection.DownBack:
                return GetDistance(spatialType) * (-audioListener.transform.up - audioListener.transform.forward);

            case SpatialDirection.DownForward:
                return GetDistance(spatialType) * (-audioListener.transform.up + audioListener.transform.forward);

            case SpatialDirection.DownLeft:
                return GetDistance(spatialType) * (-audioListener.transform.up - audioListener.transform.right);

            default:
                return GetDistance(spatialType) * (-audioListener.transform.up + audioListener.transform.right);

              


        }

    }

    public static float GetDistance(SpatialType spatialType)
    {

        switch (spatialType)
        {
            case SpatialType.Close:
                return closeDistance;

            case SpatialType.Medium:

                return mediumDistance;

            case SpatialType.Far:

                return farDistance;

            default:

                return alabamaDistance;

        }



    }


    public static void StopSound(AudioSource source)
    {

        if (soundsList.Contains(source))
        {
            source.Stop();
            
        }
    }


    public static void RemoveSound(AudioSource source)
    {

        soundsList.Remove(source);

    }

    public static void StopAllSounds()
    {

        foreach(AudioSource sound in sounds)
        {

            sound.Stop();

        }

    }

}
