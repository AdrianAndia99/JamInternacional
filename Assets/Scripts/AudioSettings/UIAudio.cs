using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIAudio : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVol", 0.75f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVol", 0.75f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVol", 0.75f);
        ApplyVolumes();
    }

    public void SetMasterVolume(float value)
    {
        mixer.SetFloat("MasterVol", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MasterVol", value);
    }

    public void SetMusicVolume(float value)
    {
        mixer.SetFloat("MusicVol", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MusicVol", value);
    }

    public void SetSFXVolume(float value)
    {
        mixer.SetFloat("SFXVol", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SFXVol", value);
    }

    private void ApplyVolumes()
    {
        SetMasterVolume(masterSlider.value);
        SetMusicVolume(musicSlider.value);
        SetSFXVolume(sfxSlider.value);
    }
}