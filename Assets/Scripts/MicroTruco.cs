using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System;
using System.Linq;

public class MicroTruco : MonoBehaviour
{
    KeywordRecognizer keywordRecognizer;
    Dictionary<string, Action> keywords;
    void Start()
    {
        keywords = new Dictionary<string, Action>();
        keywords.Add("azul", Azul);
        keywords.Add("rojo", Rojo);
        keywords.Add("verde", Verde);
        keywords.Add("amarillo", Amarillo);
        keywords.Add("arriba", Arriba);

        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += WorldRecognized;
        keywordRecognizer.Start();
    }
    private void WorldRecognized(PhraseRecognizedEventArgs word)
    {
        Debug.Log("He reconocido la palabra: " + word.text);
        keywords[word.text].Invoke();
    }
    private void Azul()
    {
        GetComponent<Renderer>().material.color = Color.blue;
    }

    private void Rojo()
    {
        GetComponent<Renderer>().material.color = Color.red;
    }

    private void Verde()
    {
        GetComponent<Renderer>().material.color = Color.green;
    }
    private void Amarillo()
    {
        GetComponent<Renderer>().material.color = Color.yellow;
    }
    private void Arriba()
    {
        transform.Translate(1,0,0);
    }
}
