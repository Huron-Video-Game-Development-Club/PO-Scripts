using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound {
    public string soundName;
    
    public AudioClip audioClip;

    [Range(0f, 1f)]
    public float volume;
    public float pitch;

    [HideInInspector]
    public AudioSource source;

    public bool playOnLoad = false;
    public bool loop = false;
    public bool playThroughLoad = false;

    public bool hasStarter = false;
    public AudioClip starter;

    public bool paused;
}
