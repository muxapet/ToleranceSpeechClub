using System.Collections.Generic;
using UnityEngine;

public class SentenceCollection
{
    private readonly string _data;
    private SentenceObject[] _sentences;

    public SentenceCollection(string data)
    {
        _data = data;
        CacheSentences();
    }

    public SentenceObject[] GetVariants(int count)
    {
        if (count > Length)
        {
            Debug.Log("Not enought variants");
            return new SentenceObject[0];
        }

        List<SentenceObject> choosed = new List<SentenceObject>();

        int errorCounter = 0;
        while (choosed.Count < count && errorCounter < 10000)
        {
            float weightSum = 0;
            for (int i = 0; i < _sentences.Length; i++)
            {
                weightSum += _sentences[i].GetWeight();
            }
            if (Mathf.Approximately(weightSum, 0))
            {
                for (int i = 0; i < _sentences.Length; i++)
                {
                    _sentences[i].OnNewGameStarted();
                    weightSum += _sentences[i].GetWeight();
                }
            }
            
            float varSum = Random.Range(0, weightSum);
            weightSum = 0;
            for (int i = 0; i < _sentences.Length; i++)
            {
                weightSum += _sentences[i].GetWeight();
                if (varSum < weightSum)
                {
                    choosed.Add(_sentences[i]);
                    _sentences[i].SetUsed();
                    break;
                }
            }
            errorCounter++;
        }

        return choosed.ToArray();
    }

    public int Length
    {
        get
        {
            CacheSentences();
            return _sentences.Length;
        }
    }

    public SentenceObject this[int key]
    {
        get
        {
            CacheSentences();
            return _sentences[key];
        }
        set { _sentences[key] = value; }
    }

    private void CacheSentences()
    {
        if (_sentences == null || _sentences.Length == 0)
        {
            string[] lines = _data.Split('\n');
            string[] titles = lines[0].Split('	');
            Category[] categories = new Category[titles.Length];
            for(int j = 0; j < titles.Length; j++)
            {
                categories[j] = GetCategoryByTitle(titles[j]);
            }
            
            List<SentenceObject> sentences = new List<SentenceObject>();
            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = lines[i].Split('	');
                if (values.Length >= 9)
                {
                    string ru = values[1].Trim();
                    if (string.IsNullOrEmpty(ru))
                    {
                        continue;
                    }
                    string eng = ru;
                    if (values.Length >= 10)
                    {
                        eng = values[9].Trim();
                    }
                    SentenceObject sentence = new SentenceObject(eng, ru);
                    
                    List<CategoryValue> influences = new List<CategoryValue>();
                    for (int j = 2; j < values.Length; j++)
                    {
                        int val = 0;
                        if (int.TryParse(values[j], out val) && j < categories.Length)
                        {
                            CategoryValue cv = new CategoryValue
                            {
                                CategoryName = categories[j],
                                Value = val
                            };
                            influences.Add(cv);
                        }
                    }
                    sentence.Influence = influences.ToArray();
                    sentences.Add(sentence);
                }
            }
            _sentences = sentences.ToArray();
        }
    }

    private Category GetCategoryByTitle(string title)
    {
        var id = title.ToLower().Trim();

        if (id.Equals("религия") || id.Equals("religion"))
        {
            return Category.Religion;
        }
        if (id.Equals("наркотики") || id.Equals("drugs"))
        {
            return Category.Drugs;
        }
        if (id.Equals("геи") || id.Equals("gays"))
        {
            return Category.Gays;
        }
        if (id.Equals("деньги") || id.Equals("money"))
        {
            return Category.Money;
        }
        if (id.Equals("сексизм") || id.Equals("sexism"))
        {
            return Category.Sexism;
        }
        if (id.Equals("расизм") || id.Equals("racism"))
        {
            return Category.Racism;
        }
        if (id.Equals("алкоголь") || id.Equals("alcohol"))
        {
            return Category.Alcohol;
        }

        return Category.Unknown;
    }
}