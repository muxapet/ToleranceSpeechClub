using System;
using UnityEngine;

public class SentenceObject
{
    public string Id { get; private set; }
    public CategoryValue[] Influence;

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

    public SentenceObject(string id)
    {
        Id = id;
        OnNewGameStarted();
    }
}
