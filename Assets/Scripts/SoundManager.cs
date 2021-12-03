using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip _clip)
    {
        audioSource.PlayOneShot(_clip);
    }

    public void SetVolume(float _volume)
    {
        audioSource.volume = _volume;
    }
}
