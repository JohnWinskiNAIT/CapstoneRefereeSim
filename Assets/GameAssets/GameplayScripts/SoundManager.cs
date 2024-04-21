using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundUtilities
{
    public static string SoundPath = "GamePrefabs/AudioObject";

    static public void PlaySound(AudioClip clip, float volume, Vector3 position)
    {
        GameObject soundEffect = Resources.Load<GameObject>(SoundPath);
        GameObject newSound = Object.Instantiate(soundEffect, position, Quaternion.identity, null);
        newSound.GetComponent<SoundManager>().InitializeSound(clip, volume);
    }
}

public class SoundManager : MonoBehaviour
{
    public float clipDuration;
    public float timer;
    public bool active = false;

    public void InitializeSound(AudioClip clip, float volume)
    {
        AudioSource audioSource = GetComponent<AudioSource>();

        audioSource.clip = clip;
        audioSource.volume = volume;

        clipDuration = clip.length;
        timer = 0;
        active = true;

        audioSource.Play();
    }

    void Update()
    {
        if (active)
        {
            timer += Time.deltaTime;
            if (timer > clipDuration)
            {
                Destroy(gameObject);
            }
        }
    }
}
