using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Менеджер музыки - загружает из Resources файлы
/// </summary>
public class SoundController : MonoBehaviour
{
    private static SoundController instance = null;

    public Dictionary<string, AudioSource> sounds;
    private const int MUS_SND_LENGTH = 5;

    void Awake()
    {
        GenerateManager();

        sounds = new Dictionary<string, AudioSource>();
        AudioSource[] asourses = Resources.FindObjectsOfTypeAll<AudioSource>();
        foreach (AudioSource a in asourses)
        {
            if (instance.sounds.ContainsKey(a.gameObject.name) == false)
            {
                sounds.Add(a.gameObject.name, a);
                a.playOnAwake = false;
            }
        }
    }

    public static void GenerateManager()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<SoundController>();
            if (instance == null)
            {
                GameObject pGO = new GameObject();
                pGO.name = "SoundManager";
                instance = pGO.AddComponent<SoundController>();
            }
        }
        SoundController.SetMusicLevel(SoundController.GetMusicLevel());
        SoundController.SetSoundLevel(SoundController.GetSoundLevel());
    }

    public static void Play(AudioClip source, bool loop = false, bool restartIfPlaying = true)
    {
        string name = "aclip_" + source.name;
        if (instance == null)
        {
            GenerateManager();
        }
        if (instance != null && instance.sounds != null)
        {
            if (instance.sounds.ContainsKey(name))
            {
                AudioSource clip = instance.sounds[name];
                if (clip != null)
                {
                    if (clip.isPlaying && restartIfPlaying == false)
                    {
                        return;
                    }
                    clip.loop = loop;
                    clip.Play();
                }
            }
            else
            {
                GameObject pGO = new GameObject();
                pGO.name = "Sound_" + name;
                AudioSource asource = pGO.AddComponent<AudioSource>();
                asource.clip = source;
                asource.playOnAwake = false;
                instance.sounds.Add(name, asource);
                asource.loop = loop;
                if (source.length >= MUS_SND_LENGTH)
                {
                    asource.volume = GetMusicLevel();
                }
                else
                {
                    asource.volume = GetSoundLevel();
                }
                asource.Play();
            }
        }
    }

    public static void Play(string name, bool loop = false, bool restartIfPlaying = true)
    {
        if (instance == null)
        {
            GenerateManager();
        }
        if (instance != null && instance.sounds != null)
        {
            if (instance.sounds.ContainsKey(name))
            {
                AudioSource clip = instance.sounds[name];
                if (clip != null)
                {
                    if (clip.isPlaying && restartIfPlaying == false)
                    {
                        return;
                    }
                    clip.loop = loop;
                    clip.Play();
                }
            }
            else
            {
                AudioClip source = Resources.Load("Sounds/" + name) as AudioClip;
                if (source != null)
                {
                    GameObject pGO = new GameObject();
                    pGO.name = "Sound_" + name;
                    AudioSource asource = pGO.AddComponent<AudioSource>();
                    asource.clip = source;
                    asource.playOnAwake = false;
                    instance.sounds.Add(name, asource);
                    asource.loop = loop;
                    if (source.length >= MUS_SND_LENGTH)
                    {
                        asource.volume = SoundController.GetMusicLevel();
                    }
                    else
                    {
                        asource.volume = SoundController.GetSoundLevel();
                    }
                    asource.Play();
                }
            }
        }
    }

    public static bool IsPlaying(AudioClip clip)
    {
        string name = "aclip_" + clip.name;
        return IsPlaying(name);
    }

    public static bool IsPlaying(string name)
    {
        if (instance != null)
        {
            if (instance.sounds.ContainsKey(name))
            {
                AudioSource clip = instance.sounds[name];
                if (clip.isPlaying)
                    return true;
            }
        }

        return false;
    }

    public static void Stop(string name)
    {
        if (instance == null)
        {
            GenerateManager();
        }
        if (instance != null)
        {
            if (instance.sounds.ContainsKey(name))
            {
                AudioSource clip = instance.sounds[name];
                if (clip != null)
                {
                    clip.Stop();
                }
            }
        }
    }

    public static void SetMusicLevel(float value)
    {
        if (instance != null && instance.sounds != null)
        {
            PlayerPrefs.SetFloat("musicLevel", value);
            foreach (KeyValuePair<string, AudioSource> a in instance.sounds)
            {
                if (a.Value != null)
                {
                    if (a.Value.clip.length >= MUS_SND_LENGTH)
                    {
                        a.Value.volume = value;
                    }
                }
            }
        }
    }

    public static void SetSoundLevel(float value)
    {
        if (instance != null && instance.sounds != null)
        {
            PlayerPrefs.SetFloat("soundLevel", value);
            foreach (KeyValuePair<string, AudioSource> a in instance.sounds)
            {
                if (a.Value != null)
                {
                    if (a.Value.clip.length < MUS_SND_LENGTH)
                    {
                        a.Value.volume = value;
                    }
                }
            }
        }
    }

    public static float GetMusicLevel()
    {
        return PlayerPrefs.GetFloat("musicLevel", 1);
    }

    public static float GetSoundLevel()
    {
        return PlayerPrefs.GetFloat("soundLevel", 1);
    }
}