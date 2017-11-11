using System;
using System.Collections;
using System.Collections.Generic;
using Model;
using UnityEngine;
using UnityEngine.UI;

public class DummyGUI : MonoBehaviour
{
    public RoundController Game;
    public DummyPersonController[] Labels;
    public DummyVariantController[] Variants;
    public Text Logger;

    private ICharacter[] _characters;
    private SentenceObject[] _variants;
    private ICharacter _current;

    private void Awake()
    {
        Game.OnRoundStarted += OnRoundStarted;
        Game.OnActivePlayerChanged += OnNewTurn;
        Game.OnActivePlayerAnswered += OnPlayerAnswered;
    }

    private void OnRoundStarted(ICharacter[] characters)
    {
        _characters = characters;
        for (int i = 0; i < characters.Length; i++)
        {
            Labels[i].Label.text = characters[i].GetCharacter().Title;
            Labels[i].Patience.fillAmount =
                (float) characters[i].GetPatience() / characters[i].GetCharacter().StartPatience;
        }
    }

    private void OnNewTurn(ICharacter from, ICharacter to, SentenceObject[] variants)
    {
        _current = to;
        
        for (int i = 0; i < _characters.Length; i++)
        {
            Labels[i].CurrentSelection.enabled = ReferenceEquals(_characters[i], to);
        }
        
        _variants = variants;
        for (int i = 0; i < _variants.Length; i++)
        {
            Variants[i].Init(this, _variants[i]);
        }
        
        DummyGUI.Log("<color=#ff0>"+_current.GetCharacter().Title + " думает...</color>");
    }

    public void OnAnswered(SentenceObject sentence)
    {
        if (_current.IsHuman())
        {
            ((HumanPlayer)_current).Answer(sentence);
        }
    }

    private void OnPlayerAnswered(ICharacter player, SentenceObject sentence)
    {
        DummyGUI.Log("<color=#f00>"+player.GetCharacter().Title + " сказал " + sentence.Id + "</color>");
    }

    public static void Log(string str)
    {
        GetInstance().Logger.text = (str + Environment.NewLine + GetInstance().Logger.text);
    }

    private static DummyGUI _instance;
    private static DummyGUI GetInstance()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<DummyGUI>();
        }
        return _instance;
    }
}