using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BubbleController : MonoBehaviour
{
    public Text Label;

    private VariantsUIController _ui;
    private SentenceObject _sentence;
    private RectTransform _rectTransform;
    private Vector2 _basePosition;
    private AudioClip _clip;

    public void Show(VariantsUIController ui, SentenceObject sentence, AudioClip sound, float delay)
    {
        _ui = ui;
        _clip = sound;
        _sentence = sentence;

        Label.text = sentence.Id;

        if (_rectTransform == null)
        {
            _rectTransform = GetComponent<RectTransform>();
            _basePosition = _rectTransform.anchoredPosition;
        }

        gameObject.SetActive(true);
        _rectTransform.anchoredPosition = _basePosition + new Vector2(0, 1000);
        Invoke("ShowBubble", 2f + delay);
    }

    private void ShowBubble()
    {
        SoundController.Play(_clip);
        _rectTransform.DOAnchorPos(_basePosition, 1f).SetEase(Ease.OutBack).Play();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
//        if (_rectTransform == null || !gameObject.activeSelf)
//        {
//            gameObject.SetActive(false);
//            return;
//        }
//        
//        _rectTransform.DOAnchorPos(_basePosition + new Vector2(0, 1000), 1f).SetEase(Ease.InBack)
//            .OnComplete(() => { gameObject.SetActive(false); }).Play();
    }

    public void OnClick()
    {
        _ui.OnAnswered(_sentence);
    }
}