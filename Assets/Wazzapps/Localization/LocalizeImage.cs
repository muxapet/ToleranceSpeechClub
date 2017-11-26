using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

namespace Wazzapps
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Image))]
    public class LocalizeImage : MonoBehaviour
    {
        [SerializeField]
        private SystemLanguage[] mLanguagesInternal;
        [SerializeField]
        private Sprite[] mSpritesInternal;
        [SerializeField]
        private Sprite mDefaultSprite;

        public Sprite DefaultSprite
        {
            get
            {
                return mDefaultSprite;
            }
            set
            {
                mDefaultSprite = value;
            }
        }

        public Sprite[] Sprites
        {
            get
            {
                return mSpritesInternal;
            }
        }

        public SystemLanguage[] Languages
        {
            get
            {
                return mLanguagesInternal;
            }
        }

        private Image mImage;

#if UNITY_EDITOR
        private Coroutine mCoroutine = null;
        IEnumerator UpdateCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f);
                UpdateImage();
            }
        }
#endif

        void Awake()
        {
            Localizator.OnLanguageChanged += () =>
            {
                UpdateImage();
#if UNITY_EDITOR
                if (this != null)
                {
                    UnityEditor.EditorUtility.SetDirty(this);
                }
#endif
            };
        }

        void OnEnable()
        {
            UpdateImage();

#if UNITY_EDITOR
            if (mCoroutine == null)
            {
                mCoroutine = StartCoroutine(UpdateCoroutine());
            }
#endif
        }

        private void UpdateImage()
        {
            if (this == null)
            {
                return;
            }
            if (mImage == null)
            {
                mImage = GetComponent<Image>();
            }
            if (mImage != null)
            {
                SystemLanguage language = Localizator.GetLanguage();
                int index = HasTranslations(language);
                if (index >= 0)
                {
                    mImage.sprite = mSpritesInternal[index];
                }
                else
                {
                    mImage.sprite = mDefaultSprite;
                }
            }
        }

        public void AddLanguage()
        {
            int[] languages = GetNotLocalizedLanguages();
            for (int i = 0; i < languages.Length; i++)
            {
                if (languages[i] == (int)SystemLanguage.Russian)
                {
                    AddLanguage(SystemLanguage.Russian, null);
                    return;
                }
            }
            AddLanguage((SystemLanguage)GetNotLocalizedLanguages()[0], null);
        }

        public void AddLanguage(SystemLanguage lang, Sprite sprite)
        {
            List<SystemLanguage> languages = new List<SystemLanguage>();
            List<Sprite> sprites = new List<Sprite>();
            for (int i = 0; i < mLanguagesInternal.Length; i++)
            {
                languages.Add(mLanguagesInternal[i]);
                sprites.Add(mSpritesInternal[i]);
            }
            languages.Add(lang);
            sprites.Add(sprite);

            mLanguagesInternal = languages.ToArray();
            mSpritesInternal = sprites.ToArray();
        }

        public void RemoveLanguage(SystemLanguage lang)
        {
            List<SystemLanguage> languages = new List<SystemLanguage>();
            List<Sprite> sprites = new List<Sprite>();
            for (int i = 0; i < mLanguagesInternal.Length; i++)
            {
                if (lang != mLanguagesInternal[i])
                {
                    languages.Add(mLanguagesInternal[i]);
                    sprites.Add(mSpritesInternal[i]);
                }
            }
            mLanguagesInternal = languages.ToArray();
            mSpritesInternal = sprites.ToArray();
        }

        public int HasTranslations(SystemLanguage item)
        {
            if(mLanguagesInternal != null)
            {
                for (int i = 0; i < mLanguagesInternal.Length; i++)
                {
                    if (mLanguagesInternal[i] == item)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public int[] GetNotLocalizedLanguages()
        {
            var values = Enum.GetValues(typeof(SystemLanguage));
            List<int> localized = new List<int>();
            List<int> nonLocalized = new List<int>();
            for (int i = 0; i < values.Length; i++)
            {
                int index = HasTranslations((SystemLanguage)values.GetValue(i));
                if (index >= 0)
                {
                    localized.Add((int)values.GetValue(i));
                }
                else
                {
                    nonLocalized.Add((int)values.GetValue(i));
                }
            }

            return nonLocalized.ToArray();
        }
    }
}