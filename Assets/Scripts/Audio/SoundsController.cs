using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundsController : SingletonPersistent<SoundsController>
{
    [Serializable]
    private class SceneAudioEntry
    {
        [Tooltip("Nombre exacto de la escena (SceneManager).")]
        public string sceneName;

        [Tooltip("Clip que se reproducirá cuando la escena esté activa.")]
        public AudioClipSO audioClip;

        [Tooltip("Reproducir como loop (true) o como one-shot (false).")]
        public bool playAsLoop = true;

        [Tooltip("Si está deshabilitado, la escena no dispara audio de forma automática.")]
        public bool triggerOnSceneLoaded = true;
    }

    [Header("Configuración de escenas y audio")]
    [SerializeField] private List<SceneAudioEntry> sceneAudioEntries = new List<SceneAudioEntry>();

    [Header("Opciones globales")]
    [SerializeField] private bool playOnStartup = true;
    [SerializeField] private bool stopAudioWhenSceneHasNoEntry = true;

    private AudioClipSO _currentClip;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;

        if (playOnStartup)
        {
            var activeScene = SceneManager.GetActiveScene();
            if (activeScene.IsValid())
            {
                PlayClipForScene(activeScene.name);
            }
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayClipForScene(scene.name);
    }

    public void PlayClipForScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            return;
        }

        var entry = sceneAudioEntries.Find(e =>
            !string.IsNullOrWhiteSpace(e.sceneName) &&
            string.Equals(e.sceneName, sceneName, StringComparison.OrdinalIgnoreCase));

        if (entry == null || entry.audioClip == null)
        {
            if (stopAudioWhenSceneHasNoEntry)
            {
                StopCurrentClip();
            }
            return;
        }

        if (!entry.triggerOnSceneLoaded)
        {
            return;
        }

        PlayClip(entry.audioClip, entry.playAsLoop);
    }

    public void PlayClip(AudioClipSO audioClip, bool loop)
    {
        if (audioClip == null)
        {
            return;
        }

        if (audioClip == _currentClip)
        {
            return;
        }

        StopCurrentClip();

        _currentClip = audioClip;
        if (loop)
        {
            audioClip.PlayLoop();
        }
        else
        {
            audioClip.PlayOneShoot();
        }
    }

    public void StopCurrentClip()
    {
        if (_currentClip != null)
        {
            _currentClip.StopPlay();
            _currentClip = null;
        }
    }
}