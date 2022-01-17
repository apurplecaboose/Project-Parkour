using UnityEngine;
using UnityEngine.Audio;
using System;
using UnityEngine.SceneManagement;
public class AudioManager : MonoBehaviour
{
    public AudioMixerGroup blender;

    public Sound[] sounds;

    public static AudioManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);


        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = blender;
        }
    }
    void Start()
    {
        Play("Menu");
    }
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }
    /*
    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex>3 & SceneManager.GetActiveScene().buildIndex != 10)
        {
            Destroy(gameObject);
        }
    }
    */
}
