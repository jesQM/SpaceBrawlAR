using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicManager : MonoBehaviour
{
    public AudioClip music;
    private AudioSource source;

    void Awake()
    {
        // Singleton
        BackgroundMusicManager[] objs = GameObject.FindObjectsOfType<BackgroundMusicManager>();

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
    
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.volume = 0.5f;

        source.clip = music;
        source.loop = true;
        source.Play();
    }
}
