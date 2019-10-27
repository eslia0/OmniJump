using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance {
        get {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<SoundManager>();

                if (instance == null)
                {
                    GameObject managment = new GameObject("SoundManager");
                    instance = managment.AddComponent<SoundManager>();
                }
            }

            return instance;
        }
    }

    private AudioSource audio;
    [SerializeField] private AudioClip[] clips;
    public AudioClip currentClip;

    private void Awake()
    {
        instance = FindObjectOfType<SoundManager>();

        if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        audio = GetComponent<AudioSource>();
    }

    public void Play(string source)
    {
        AudioClip clip = FindClip(source);

        if (clip)
        {
            if (audio.clip != clip)
            {
                audio.clip = clip;
                audio.Play();
            }
        }
    }

    public AudioClip FindClip(string clip)
    {
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i].name == clip)
                return clips[i];
        }

        return null;
    }

    public void SetLoop(bool value)
    {
        audio.loop = value;
    }
}
