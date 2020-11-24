using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public AudioClip BtnContinue;
    public AudioClip BtnBack;

    private AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.volume = 0.5f;
    }

    public void PlayBtnContinue()
    {
        source.clip = BtnContinue;
        source.Play();
    }

    public void PlayBtnBack()
    {
        source.clip = BtnBack;
        source.Play();
    }
}
