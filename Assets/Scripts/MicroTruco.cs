
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System;
using System.Linq;
using DG.Tweening;

public class MicroTruco : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> keywords;
    private float lastCommandTime;
    private float commandCooldown = 1.0f; // segundos para evitar repeticiones

    void Awake()
    {
        keywords = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase)
        {
            { "shoot", Disparar },
            { "fire", Disparar },
            { "bang", Disparar },
        };

        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray(), ConfidenceLevel.Medium);
        keywordRecognizer.OnPhraseRecognized += WorldRecognized;
        keywordRecognizer.Start();
    }

    private void WorldRecognized(PhraseRecognizedEventArgs word)
    {
        Debug.Log($"He reconocido: {word.text} con confianza {word.confidence}");

        if (Time.time - lastCommandTime < commandCooldown)
            return; // evita spam

        lastCommandTime = Time.time;

        if (keywords.TryGetValue(word.text, out var action))
            action.Invoke();
    }

    private void Disparar()
    {
        Debug.Log("¡BANG! Disparo por voz detectado");
        // Aquí llamas al método del juego para disparar
        FindObjectOfType<ScareShootGame>()?.VoiceShoot();
    }
}
