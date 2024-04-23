using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningAudioManager : MonoBehaviour
{
    [Header("----------- Audio Source -----------")]
    [SerializeField] AudioSource music;
    [SerializeField] AudioSource SFX;

    [Header("----------- Audio Clip -----------")]
    public AudioClip background;
    public AudioClip gate;
    public AudioClip gem1;
    public AudioClip gem2;
    public AudioClip gem3;
    public AudioClip gem4;
    public AudioClip gem5;

    
    private void Start()
    {
        music.clip = background;
        music.Play();

    }

    public void PlaySFX(AudioClip clip)
    {
        SFX.PlayOneShot(clip);
    }
}
