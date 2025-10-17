using UnityEngine;

[DefaultExecutionOrder(-1)]
public class AudioManager : SingletonPersistent<AudioManager>
{
    [SerializeField] private AudioMixerSO masterMixerSO;
    [SerializeField] private AudioMixerSO sfxMixerSO;
    [SerializeField] private AudioMixerSO musicMixerSO;

    public override void Awake()
    {
        base.Awake();
        masterMixerSO.EnableSound();
        sfxMixerSO.EnableSound();
        musicMixerSO.EnableSound();
    }

    public void UpdateMasterVolume(float value)
    {
        masterMixerSO.UpdateVolume(value);
    }

    public void UpdateSFXVolume(float value)
    {
        sfxMixerSO.UpdateVolume(value);
    }

    public void UpdateMusicVolume(float value)
    {
        musicMixerSO.UpdateVolume(value);
    }

    public float GetMasterVolume()
    {
        return masterMixerSO.GetCurrentVolumeValue();
    }

    public float GetSFXVolume()
    {
        return sfxMixerSO.GetCurrentVolumeValue();
    }

    public float GetMusicVolume()
    {
        return musicMixerSO.GetCurrentVolumeValue();
    }
}