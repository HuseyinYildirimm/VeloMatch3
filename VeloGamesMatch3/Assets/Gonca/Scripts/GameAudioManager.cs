using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    [Header("----------- Audio Source -----------")]
    [SerializeField] AudioSource music;
    [SerializeField] AudioSource SFX;

    [Header("----------- Audio Clip -----------")]
    public AudioClip background;
    public AudioClip stone;
    public AudioClip dynamite;

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
