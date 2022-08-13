using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    private string playing;
    private bool paused;

    private static AudioManager instance;

    public static AudioManager Instance {
        get {
            if(instance == null) {
                instance = FindObjectOfType<AudioManager>();
            }
            return instance;
        }
    }

    void Awake() {
        paused = false;
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        foreach(Sound sound in sounds) {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.audioClip;
            sound.source.volume = sound.volume;
            if(sound.playOnLoad) {
                sound.source.Play();
                sound.source.loop = sound.loop;
                playing = sound.soundName;
            }
            // sound.source.pitch = sound.pitch;
        }
    }

    // Play audio track and save it as background track
    public void Play(string audioName) {
        Sound sound = Array.Find(sounds, s => s.soundName == audioName);
        
        if(sound.hasStarter) {
            sound.source.clip = sound.starter;
            sound.source.Play();
            sound.source.loop = false;
            
            StartCoroutine(PlaySoundAfterStarter(sound));
        } else {
            sound.source.Play();
            sound.source.loop = sound.loop;
        }

        /*if(CutsceneManager.Instance != null) {
            CutsceneManager.Instance.PlayNextEvent();
        }*/
        playing = audioName;
    }

    // Play without saving current track
    //!Triggers next event in cutscene
    public void PlayNoSave(string audioName) {
        Sound sound = Array.Find(sounds, s => s.soundName == audioName);
        sound.source.Play();
        sound.source.loop = sound.loop;
        if(CutsceneManager.Instance != null) {
            CutsceneManager.Instance.PlayNextEvent();
        }
    }

    // Play without saving current track
    //!DOES NOT Triggers next event in cutscene
    public void PlayNoNext(string audioName) {
        // Debug.Log("PLAYING NOW");
        Sound sound = Array.Find(sounds, s => s.soundName == audioName);
        if(sound.hasStarter) {
            sound.source.clip = sound.starter;
            sound.source.Play();
            sound.source.loop = false;
            Debug.Log("Starting coroutine");
            StartCoroutine(PlaySoundAfterStarter(sound));
        } else {
            sound.source.Play();
            sound.source.loop = sound.loop;
        }
        
    }

    public void Stop(string audioName, bool stopCoroutines = true) {
        Sound sound = Array.Find(sounds, s => s.soundName == audioName);
        sound.source.Stop();
        if(stopCoroutines) StopAllCoroutines();
    }

    // Override for stop that stops background track
    public void Stop() {
        Sound sound = Array.Find(sounds, s => s.soundName == playing);
        if(sound == null) return;
        if(sound.playThroughLoad) return;
        StopAllCoroutines();
        StartCoroutine(FadeOut(sound, 0.5f));
    }

    // Forces stop on the background track
    public void ForceStop() {
        Debug.Log(playing);
        Sound sound = Array.Find(sounds, s => s.soundName == playing);
        if(sound == null) return;
        StopAllCoroutines();
        StartCoroutine(FadeOut(sound, 0.5f));
    }

    // Pause background track
    // Used primarily for maintaining place in current track while playing a battle track
    public void Pause() {
        Sound sound = Array.Find(sounds, s => s.soundName == playing);
        sound.source.Pause();
        sound.paused = true;
        paused = true;
    }

    // Resumes background track at paused point
    public void UnPause() {
        Sound sound = Array.Find(sounds, s => s.soundName == playing);
        sound.source.UnPause();
        sound.paused = false;
        paused = false;
    }

    public bool IsPlaying(string audioName) {
        Sound sound = Array.Find(sounds, s => s.soundName == audioName);
        return sound.source.isPlaying;
    }

    IEnumerator FadeOut(Sound sound, float duration) {
        float currentTime = 0;
        float start = sound.source.volume;
        Debug.Log("IN HERE");
        while (currentTime < duration) {
            currentTime += Time.deltaTime;
            Debug.Log("Looping" + currentTime);
            sound.source.volume = Mathf.Lerp(start, 0, currentTime / duration);
            yield return null;
        }
        Debug.Log("OUT HERE");
        sound.source.Stop();
        yield break;
    }

    IEnumerator PlaySoundAfterStarter(Sound sound) {
        float timer = 0f;
        
        // In case track gets paused while still in starter
        // Pause the coroutine as well
        while(timer < sound.starter.length) {
            while(sound.paused) {
                yield return null;
            }
            Debug.Log("Looping");
            timer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Trying to play music...");

        sound.source.clip = sound.audioClip;
        sound.source.loop = sound.loop;
        sound.source.Play();
    }
}