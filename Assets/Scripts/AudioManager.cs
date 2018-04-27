using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class Sound
{
    private AudioSource source;
    public string clipName;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
    [Range(0f, 3f)]
    public float pitch;

    public bool loop = false;
    public bool playOnAwake = false;

    public void SetSource(AudioSource audioSource)
    {
        source = audioSource;
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = loop;
        source.playOnAwake = playOnAwake;
    }

    public void Play()
    {
        source.Play();
    }

    public void Stop()
    {
        source.Stop();
    }

    public void Pause()
    {
        source.Pause();
    }

    public void UnPause()
    {
        source.UnPause();
    }

    public bool IsPlaying()
    {
        return source.isPlaying;
    }
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager audioManager;
    [SerializeField]
    public Sound[] sounds;
    public Dictionary<string, Sound> soundClips;

    private void Awake()
    {
        if (audioManager == null)
        {
            audioManager = this;
            soundClips = new Dictionary<string, Sound>();
            CreateSoundObjects();
            foreach (var sound in sounds)
                soundClips.Add(sound.clipName, sound);
        }
            
        else if (audioManager != this)
            Destroy(gameObject);
    }

    private void CreateSoundObjects()
    {
        foreach (var sound in sounds)
        {
            GameObject gameObj = new GameObject("Sound: " + sound.clipName);
            gameObj.transform.SetParent(this.transform);
            sound.SetSource(gameObj.AddComponent<AudioSource>());
        }
    }

    public void PlaySound(string clipName)
    {
        Sound playableSound;
        soundClips.TryGetValue(clipName, out playableSound);
        if (playableSound != null)
            playableSound.Play();
    }

    public void StopSound(string clipName)
    {
        Sound stoppableSound;
        soundClips.TryGetValue(clipName, out stoppableSound);
        if (stoppableSound != null)
            stoppableSound.Stop();
    }

    public void PlaySoundLoop(string clipName)
    {
        Sound loopableSound;
        soundClips.TryGetValue(clipName, out loopableSound);
        if (loopableSound != null && !loopableSound.IsPlaying()) {
            loopableSound.loop = true;
            loopableSound.Play();
        }
    }

    public void PauseSoundLoop(string clipName)
    {
        Sound pausableSound;
        soundClips.TryGetValue(clipName, out pausableSound);
        if (pausableSound != null)
            pausableSound.Pause();
    }

    public void ContinuePausedSoundLoop(string clipName)
    {
        Sound continueableSound;
        soundClips.TryGetValue(clipName, out continueableSound);
        if (continueableSound != null)
            continueableSound.UnPause();
    }

    public void StopAllAudio()
    {
        foreach (var sound in sounds)
            sound.Stop();
    }
}

public static class AudioLevelFader {
    public static IEnumerator FadeOut(AudioSource source, float fadeTime)
    {
        var startVolume = source.volume;
        while (source.volume > 0)
        {
            source.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
        source.Stop();
        source.volume = startVolume;
    }
}
