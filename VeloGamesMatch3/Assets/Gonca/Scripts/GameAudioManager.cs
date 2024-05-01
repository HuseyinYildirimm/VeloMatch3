using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager Instance;
    public Sound[] sounds;
    private List<AudioSource> audioSourceList = new List<AudioSource>();
    private List<float> originalVolumes = new List<float>();

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        foreach (var sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.playOnAwake = false;
            sound.source.loop = sound.loop;
            sound.source.spatialBlend = sound.spetialBlend;

            audioSourceList.Add(sound.source);
        }

        foreach (var audioSource in audioSourceList)
        {
            originalVolumes.Add(audioSource.volume);
        }

        Play("Main");
    }

    public void AdjustAllVolumes(float volume)
    {
        for (int i = 0; i < audioSourceList.Count; i++)
        {
            float originalVolume = originalVolumes[i];
            float newVolume = originalVolume * volume;

            audioSourceList[i].volume = newVolume;
        }
    }

    public void Play(string audioName)
    {
        Sound s = Array.Find(sounds, sound => sound.audioName == audioName);

        if (s != null && s.source != null) 
        {
            s.source.Play();
        }
        else
        {
            Debug.LogWarning("Null: " + audioName);
        }
    }

    public void Stop(string audioName)
    {
        Sound s = Array.Find(sounds, sound => sound.audioName == audioName);

        if (s != null && s.source != null) 
        {
            s.source.Stop();
        }
        else
        {
            Debug.LogWarning("Null: " + audioName);
        }
    }

    public void ButtonSound()
    {
        Play("Button");
        Debug.Log("Button");
    }

    public void MatchSound()
    {
        Play("Match");
    }

    public void BoomSound()
    {

    }

    public void WinSound()
    {
        Play("Win");
    }
}
