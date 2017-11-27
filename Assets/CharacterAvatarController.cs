using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAvatarController : MonoBehaviour
{
    public Transform Hands;
    public Animator CharacterAnimator;
    private ICharacter _character;
    private RoundController _game;
    public SpriteRenderer Emoticon;
    public Sprite[] EmoticonSprites;
    public TextMesh PatienceLabel;
    public GameObject CharacterRoot;
    public ParticleSystem Green, Red, Smoke;

    private int _lastPatience = 0;
    private bool _isSubscribed = false;

    public void Init(ICharacter character, RoundController game)
    {
        _character = character;
        _game = game;

        if (Green != null)
        {
            Green.Stop();
            Green.Clear();
        }
        if (Red != null)
        {
            Red.Stop();
            Red.Clear();
        }
        if (Smoke != null)
        {
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
    }

    public Transform GetHandsTransform()
    {
        return Hands;
    }

    private void OnNewTurn(ICharacter from, ICharacter to, SentenceObject[] variants)
    {
        SetEmoticon();
    }

    private void OnPlayerAnswered(ICharacter player, SentenceObject sentence)
    {
        if (!ReferenceEquals(_character, player))
        {
            if (_lastPatience < _character.GetPatience())
            {
                if (CharacterAnimator != null)
                {
                    CharacterAnimator.Play("Thumb");
                }
                if (Green != null)
                {
                    Green.Stop();
                    Green.Clear();
                    Green.Play();
                }
            } else if (_lastPatience > _character.GetPatience())
            {
                if (Red != null)
                {
                    Red.Stop();
                    Red.Clear();
                    Red.Play();
                }
            }
        }
        else
        {
            float patienceValue = _character.GetPatience() / (float) _character.GetCharacter().StartPatience;
            AudioClip saySound = player.GetCharacter().GetSaySound();
            if (!player.IsHuman() && patienceValue < 2f / EmoticonSprites.Length)
            {
                saySound = player.GetCharacter().GetSayRageSound();
            }
            if (saySound != null)
            {
                SoundController.Play(saySound);
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

    private void Update()
    {
        if(Smoke == null) return;
        if (_lastPatience <= 0 && _lastPatience > -50)
        {
            _lastPatience = -100;
            Smoke.gameObject.SetActive(true);
            Smoke.Stop();
            Smoke.Clear();
            Smoke.Play();
        }
    }
}