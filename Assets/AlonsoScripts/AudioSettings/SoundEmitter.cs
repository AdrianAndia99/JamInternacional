using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class SoundEmitter : MonoBehaviour
{
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }
    public void PlaySound(SoundData data)
    {
        source.clip = data.clip;
        source.volume = data.volume;
        source.pitch = data.pitch;
        source.loop = data.loop;
        source.outputAudioMixerGroup = data.mixerGroup;
        source.Play();

        if (!data.loop)
        {
            Destroy(gameObject, data.clip.length / data.pitch);
        }
    }
    public void Mute()
    {
        if (source != null)
        {
            source.mute = true;
        }
    }
    public void Unmute()
    {
        if (source != null)
        {
            source.mute = false;
        }
    }
}