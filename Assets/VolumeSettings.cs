
using UnityEngine;

using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField]  AudioMixer myMixer;
    [SerializeField]  Slider sliderMusicSorce;

    public void SetMusicVolume()
    {
        float volume = sliderMusicSorce.value;
        myMixer.SetFloat("Music", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void LoadVolume()
    {
        sliderMusicSorce.value = PlayerPrefs.GetFloat("musicVolume");

        SetMusicVolume();
    }
    public void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
        }
        
    }
}
