using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Wazzapps;

public class MainMenuController : MonoBehaviour
{
    public CanvasGroup Fader;
    public RoundController Round;
    public Text SurvivedTimer;
    public GameObject Logo;
    public GameObject Credits;
    public CanvasGroup Intro;

    private Tween fader;
    private bool showing;
    

    private void Awake()
    {
        Show();
    }

    public void StartGame()
    {
        if(!showing) return;
        
        Intro.gameObject.SetActive(true);
        Intro.alpha = 1f;

        fader = Fader.DOFade(0, 1f).OnComplete(() =>
        {
            gameObject.SetActive(false);
            Round.StartRound();
            Intro.DOFade(0, 2f).SetDelay(5f).OnComplete(() =>
            {
                Intro.gameObject.SetActive(false);
            });
        }).Play();
        showing = false;
    }

    public void Show(float score = 0)
    {
        showing = true;
        gameObject.SetActive(true);
        Credits.SetActive(false);

        if (score > 0)
        {
            float record = PlayerPrefs.GetFloat("record", 0);
            if (record < score)
            {
                record = score;
                PlayerPrefs.SetFloat("record", record);
            }
            SurvivedTimer.gameObject.SetActive(true);
            SurvivedTimer.text = string.Format(Localizator.s("score"), 
                (int)score/60, 
                ((int)score)%60, 
                (int)record/60, 
                ((int)record)%60);
            SurvivedTimer.transform.localScale = Vector3.one * 2;
            SurvivedTimer.transform.DOScale(Vector3.one, 1f);
            Logo.SetActive(false);
        }
        else
        {
            Logo.SetActive(true);
            SurvivedTimer.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (fader != null) fader.Kill();
        Fader.alpha = 1f;
    }

    public void ToggleLanguage()
    {
        if (Localizator.GetLanguage() == SystemLanguage.Russian)
        {
            Localizator.LocalizatorSetEnglish();
        }
        else
        {
            Localizator.LocalizatorSetRussian();
        }
    }

    public void ToggleCredits()
    {
        Credits.SetActive(!Credits.activeSelf);
    }
}
