using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private GameObject soundEmitterPrefab;
    private List<SoundEmitter> emitters = new List<SoundEmitter>();

    public int emittersCount { get { return emitters.Count; } private set { } }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void PlaySound(SoundData soundData, Vector3 position)
    {
        SoundEmitter emitter = Instantiate(soundEmitterPrefab, position, Quaternion.identity).GetComponent<SoundEmitter>();
        emitter.PlaySound(soundData);
        emitters.Add(emitter);
    }
    public void PlaySoundGlobal(SoundData soundData)
    {
        PlaySound(soundData, Camera.main.transform.position);
    }
    public GameObject GetPlaySound(SoundData soundData, Vector3 position)
    {
        SoundEmitter emitter = Instantiate(soundEmitterPrefab, position, Quaternion.identity).GetComponent<SoundEmitter>();
        emitter.PlaySound(soundData);
        emitters.Add(emitter);

        return emitter.gameObject;
    }
    public GameObject GetPlaySoundGlobal(SoundData soundData)
    {
        return GetPlaySound(soundData, Camera.main.transform.position);
    }
    public void MuteAllEmitters()
    {
        foreach (SoundEmitter emitter in emitters)
        {
            if (emitter != null)
            {
                emitter.Mute();
            }
        }
    }
    public void UnmteAllEmitters()
    {
        foreach (SoundEmitter emitter in emitters)
        {
            if (emitter != null)
            {
                emitter.Unmute();
            }
        }
    }
    public void DestroyEmitterByIndex(int index)
    {
        if (emitters.Count > 0)
        {
            Destroy(emitters[index].gameObject);
        }
    }
    public void KillEveryAudio()
    {
        if (emitters.Count > 0)
        {
            for (int i = 0; i < emitters.Count; i++)
            {
                if (emitters[i] != null)
                {
                    Destroy(emitters[i].gameObject);
                }
            }
            emitters.Clear();
        }
    }
}