using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character_", menuName = "Character info")]
public class CharacterObject : ScriptableObject
{
public GameObject Prefab;
    public SkinObject[] Skins;
    public string Title = "";
    public TextAsset SentencesTSV;
    public int StartPatience = 100;
    public AudioClip[] SpeechSounds;
    public AudioClip[] RageSpeechSounds;
    
    [SerializeField, OneLine.OneLine, OneLine.HideLabel]
    public CategoryValue[] CaresOf = {
        new CategoryValue { CategoryName = Category.Religion },
        new CategoryValue { CategoryName = Category.Drugs },
        new CategoryValue { CategoryName = Category.Gays },
        new CategoryValue { CategoryName = Category.Money },
        new CategoryValue { CategoryName = Category.Sexism },
        new CategoryValue { CategoryName = Category.Racism },
        new CategoryValue { CategoryName = Category.Alcohol }
    };

    private SentenceCollection _sentences;
    public SentenceCollection Sentences
    {
        get
        {
            if (_sentences == null)
            {
                _sentences = new SentenceCollection(SentencesTSV.text);
            }
            return _sentences;
        }
    }

    public AudioClip GetSaySound()
    {
        if(SpeechSounds == null || SpeechSounds.Length == 0) return null;
        return SpeechSounds[Random.Range(0, SpeechSounds.Length)];
    }

    public AudioClip GetSayRageSound()
    {
        if(RageSpeechSounds == null || RageSpeechSounds.Length == 0) return null;
        return RageSpeechSounds[Random.Range(0, RageSpeechSounds.Length)];
    }
}
