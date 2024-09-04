using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioPlayer : MonoBehaviour
{
    public static AudioPlayer Instance;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip[] audioClips; 

    

    void Awake()
    {
        Instance = this;
    }

    
    public void Play(int id)
    {
        audioSource.PlayOneShot(audioClips[id]);
    }

}