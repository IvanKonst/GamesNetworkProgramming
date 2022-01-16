using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }
    private AudioSource source;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        source = GetComponent<AudioSource>();
    }

 public void PlaySound(AudioClip _sound)
    {
        source.PlayOneShot(_sound);
    }
}
