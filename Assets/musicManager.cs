using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicManager : MonoBehaviour
{
    public AudioClip audioClip;
    public float minDelay = 30f;
    public float maxDelay = 60f;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayAudioWithDelay();
    }

    private void PlayAudioWithDelay()
    {
        float delay = Random.Range(minDelay, maxDelay);
        Invoke("PlayAudio", delay);
    }

    private void PlayAudio()
    {
        audioSource.PlayOneShot(audioClip);
        Invoke("PlayAudioWithDelay", audioClip.length);
    }
}
