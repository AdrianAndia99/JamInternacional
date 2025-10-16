using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "NewSound", menuName = "Audio/Sound Data")]
public class SoundData : ScriptableObject
{
    public string soundName;
    public AudioClip clip;
    public float volume = 1.0f;
    public float pitch = 1.0f;
    public bool loop = false;
    public AudioMixerGroup mixerGroup;
}