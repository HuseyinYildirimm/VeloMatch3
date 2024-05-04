using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpeningAudioManager : MonoBehaviour
{
    public static GameAudioManager Instance;
    public Sound[] sounds;
    private List<AudioSource> audioSourceList = new List<AudioSource>();
    private List<float> originalVolumes = new List<float>();

  
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

    public void AdjustAllVolumes()
    {
        for (int i = 0; i < audioSourceList.Count; i++)
        {
            float originalVolume = originalVolumes[i];
            float newVolume = originalVolume * Match3Manager.Instance.slider.value;

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

    public void Button()
    {
        Play("Button");
    }

    public void Match()
    {
        Play("Match");
    }

    public void Boom()
    {
        Play("Boom");
    }

    public void Win()
    {
        Play("Win");
    }
}
