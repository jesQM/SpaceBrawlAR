using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private AudioSource source;
    
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.volume = 0.5f;
    }
    
    public void PlaySound(AudioClip sound)
    {
        source.clip = sound;
        source.Play();
    }
}
