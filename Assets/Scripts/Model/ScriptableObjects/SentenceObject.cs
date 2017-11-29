using System;
using UnityEngine;
using Wazzapps;

public class SentenceObject
{
    public string Id
    {
        get
        {
            if (Localizator.GetLanguage() == SystemLanguage.Russian)
            {
                return _ru;
            }
            else
            {
                return _eng;
            }
        }
    }

    public CategoryValue[] Influence;

    private string _eng, _ru;

    private string _idMd5;

    private string GetIdMD5()
    {
        if (_idMd5 == null)
        {
            _idMd5 = Utils.MD5(Id);
        }
        return _idMd5;
    }

    public int GetWeight()
    {
        return PlayerPrefs.GetInt(GetIdMD5(), 0);
    }

    public void OnNewGameStarted()
    {
        PlayerPrefs.SetInt(GetIdMD5(), GetWeight() + 1);
    }

    public void SetUsed()
    {
        PlayerPrefs.SetInt(GetIdMD5(), 0);
    }

    public SentenceObject(string eng, string ru)
    {
//        Debug.Log(eng + " -----> " + ru);
        _eng = eng;
        _ru = ru;
        OnNewGameStarted();
    }
}
