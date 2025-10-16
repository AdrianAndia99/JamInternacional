using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider volumeSlider;

    private void OnEnable()
    {
        if (masterSlider != null)
        {
            masterSlider.onValueChanged.AddListener(OnMasterSliderChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
        }

        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        }
    }

    private void OnDisable()
    {
        if (masterSlider != null)
        {
            masterSlider.onValueChanged.RemoveListener(OnMasterSliderChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.RemoveListener(OnSFXSliderChanged);
        }

        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);
        }
    }

    private void Start()
    {
        masterSlider.value = AudioManager.Instance.GetMasterVolume();
        sfxSlider.value = AudioManager.Instance.GetSFXVolume();
        volumeSlider.value = AudioManager.Instance.GetMusicVolume();

    }

    private void OnMasterSliderChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.UpdateMasterVolume(value);
        }
    }

    private void OnSFXSliderChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.UpdateSFXVolume(value);
        }
    }

    private void OnMusicSliderChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.UpdateMusicVolume(value);
        }
    }
}