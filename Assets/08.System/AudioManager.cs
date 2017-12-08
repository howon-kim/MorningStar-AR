// MADE BY HOWON KIM
// IT MAINTAINS THE AUDIO SOUNDS
// MODIFIED ON NOV 28TH, 2017

using UnityEngine;
using System.Collections.Generic;

public enum BackgroundMusic
{
    Lobby,
    Normal,
    Boss,
    Win,
    Defeat,
    Result,
    Alert // BossBeep
};


public class AudioManager : MonoBehaviour
{
    public BackgroundMusic sample;

    AudioSource audioSource;
    public List<AudioClip> audioList = new List<AudioClip>();

    private static AudioManager _instance;
    public static AudioManager instance
    {
        get
        {
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    void Awake()
    {

        if (_instance == null)
            _instance = this;

        audioSource = gameObject.AddComponent<AudioSource>();

    }

    public void PlayAudio(BackgroundMusic audioFile)
    {
        audioSource.loop = true;
        audioSource.clip = audioList[(int)audioFile];
        audioSource.Play();
    }

    public void StopAudio()
    {
        audioSource.Stop();
    }
}