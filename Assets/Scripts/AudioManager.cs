using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Audio Source")]
    [SerializeField] AudioSource musicSorce;
    [SerializeField] AudioSource sfx;
    [Header("Audio Clips")]
    public AudioClip background;
    public AudioClip death;
    public AudioClip jump;
    public AudioClip colision;
    public AudioClip runing;
    public AudioClip groundTouch;

    private void Start()
    {
        musicSorce.clip = background;
        musicSorce.Play();
    }

}
