using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System;
using System.Linq;
using DG.Tweening;

public class MicroTruco : MonoBehaviour
{
    KeywordRecognizer keywordRecognizer;
    Dictionary<string, Action> keywords;
    void Awake()
    {
        keywords = new Dictionary<string, Action>();
        keywords.Add("blue", Azul);
        keywords.Add("red", Rojo);
        keywords.Add("green", Verde);
        keywords.Add("yellow", Amarillo);
        keywords.Add("down", Arriba);
        keywords.Add("Alonso is a woman", Raz);

        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += WorldRecognized;
        keywordRecognizer.Start();
    }
    private void Raz()
    {
        transform.DOScale(new Vector3(2, 2, 2), 1);
        transform.Translate(0, 0, 1);
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
        transform.Translate(0,1,0);
    }
}
