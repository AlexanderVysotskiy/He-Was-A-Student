using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MusicManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [Header("Audio Clip")]
    public AudioClip BackgroundMusic;

    public static MusicManager Instance;
    // Start is called before the first frame update
    void Start()
    {
        musicSource.clip = BackgroundMusic;
        musicSource.Play();
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
