using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAvatarController : MonoBehaviour
{
    public Transform StartBubble;
    public Transform Hands;
    public Animator CharacterAnimator;
    private ICharacter _character;
    private RoundController _game;
    public SpriteRenderer Emoticon;
    public Sprite[] EmoticonSprites;
    public TextMesh PatienceLabel;
    public GameObject CharacterRoot;
    public ParticleSystem Smoke;

    private int _lastPatience = 0;
    private bool _isSubscribed = false;

    public string DefaultAnimation = "Poof";
    private readonly string[] OkAnimations = {"Thumb", "Yes", "Yes1"};
    private readonly string[] BadAnimations = {"No2", "No3"};

    public int Pose = 0;

    public void Init(ICharacter character, RoundController game)
    {
        _character = character;
        _game = game;

        if (Smoke != null)
        {
            Smoke.Clear();
            Smoke.Stop();
            Smoke.gameObject.SetActive(false);
        }

        if (!_isSubscribed)
        {
            _isSubscribed = true;
            _game.OnActivePlayerChanged += OnNewTurn;
            _game.OnActivePlayerAnswered += OnPlayerAnswered;
        }

        if (CharacterRoot != null)
        {
            for (int i = 0; i < CharacterRoot.transform.childCount; i++)
            {
                Destroy(CharacterRoot.transform.GetChild(i).gameObject);
            }

            if (_character.GetCharacter().Prefab != null)
            {
                GameObject characterModel = Instantiate(_character.GetCharacter().Prefab);
                characterModel.transform.SetParent(CharacterRoot.transform);
                characterModel.transform.localEulerAngles = new Vector3(0, 126, 0);
                CharacterAnimator = characterModel.GetComponentInChildren<Animator>();
            }
        }

        _lastPatience = _character.GetPatience();
        SetEmoticon();

        if (CharacterAnimator != null)
        {
            CharacterAnimator.SetInteger("Pose", Pose);
            CharacterAnimator.Play(DefaultAnimation);
        }
    }

    public Transform GetHandsTransform()
    {
        return Hands;
    }

    private void OnNewTurn(ICharacter from, ICharacter to, SentenceObject[] variants)
    {
        SetEmoticon();

        if (ReferenceEquals(from, _character) && _character.GetPatience() > 0)
        {
            if (CharacterAnimator != null)
            {
                CharacterAnimator.Play("Throw");
            }
        }
        if (ReferenceEquals(to, _character) && _character.GetPatience() > 0)
        {
            if (CharacterAnimator != null)
            {
                CharacterAnimator.Play("Catch");
            }
        }
    }

    private void OnPlayerAnswered(ICharacter player, SentenceObject sentence)
    {
        if (!ReferenceEquals(_character, player))
        {
            if (_lastPatience < _character.GetPatience())
            {
                if (CharacterAnimator != null)
                {
                    CharacterAnimator.Play(OkAnimations[UnityEngine.Random.Range(0, OkAnimations.Length)]);
                }
            }
            else if (_lastPatience > _character.GetPatience())
            {
                if (CharacterAnimator != null)
                {
                    if (_character.GetPatience() <= 0)
                    {
                        CharacterAnimator.Play("RageMode");
                        

                        if (Smoke != null)
                        {
                            Smoke.Clear();
                            Smoke.Stop();
                            Smoke.gameObject.SetActive(true);
                            Smoke.Play();
                            SoundController.Play(_character.GetCharacter().GetSayRageSound());
                        }
                    }
                    else
                    {
                        CharacterAnimator.Play(BadAnimations[UnityEngine.Random.Range(0, BadAnimations.Length)]);
                    }
                }
            }
        }
        else
        {
            float patienceValue = _character.GetPatience() / (float) _character.GetCharacter().StartPatience;
            AudioClip saySound = player.GetCharacter().GetSaySound();
            string sayAnim = "Speak";
            if (!player.IsHuman() && patienceValue < 2f / EmoticonSprites.Length)
            {
                saySound = player.GetCharacter().GetSayRageSound();
                sayAnim = "Rage Speak";
            }
            if (saySound != null)
            {
                SoundController.Play(saySound);
            }
            
            if (CharacterAnimator != null)
            {
                CharacterAnimator.Play(sayAnim);
            }
        }
        _lastPatience = _character.GetPatience();
    }

    private void SetEmoticon()
    {
        if (_character.IsHuman()) return;
        PatienceLabel.text = _lastPatience.ToString();
        float patienceValue = _lastPatience / (float) _character.GetCharacter().StartPatience;
        PatienceLabel.color = Color.Lerp(Color.red, Color.green, patienceValue);
        float size = 1f / EmoticonSprites.Length;
        for (int i = 0; i < EmoticonSprites.Length; i++)
        {
            if (patienceValue >= i * size && patienceValue < (i + 1) * size)
            {
                Emoticon.sprite = EmoticonSprites[i];
                Emoticon.color = Color.Lerp(Color.red, Color.green, patienceValue);
                break;
            }
        }
    }
}