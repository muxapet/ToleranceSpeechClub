using UnityEngine;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Wazzapps
{
    public static class Localizator
    {
        private const string TRANSLATIONS_PATH = "translations.txt";
        private const string FORCE_LANG_KEY = "localizatorForceLanguage";
        private const string MENU_DEFAULT_LANG = "Wazzapps/Localizator/Use Default Language";
        private const string MENU_ENG_LANG = "Wazzapps/Localizator/Force English";
        private const string MENU_RUS_LANG = "Wazzapps/Localizator/Force Russian";
        public const string LOCALIZE_WINDOW = "Wazzapps/Localizator/Editor";

        private static Trans[] translations = new Trans[0];
        private static SystemLanguage[] mLanguages;

        public delegate void OnLanguageChangedDelegate();
        public static OnLanguageChangedDelegate OnLanguageChanged;

        public static SystemLanguage[] Languages
        {
            get
            {
                return mLanguages;
            }
        }

        public class Trans
        {
            public string code;
            public string[] val;
            public Trans(string _code, string[] _val)
            {
                this.code = _code;
                this.val = _val;
            }

            public override string ToString()
            {
                return string.Format("{0}: {1}", code, string.Join(", ", val));
            }
        };

        static Localizator()
        {
            ParseTranslations();
#if UNITY_EDITOR
            ClearMenuChecks();
#endif
        }

        public static SystemLanguage GetLanguage()
        {
            if (PlayerPrefs.HasKey(FORCE_LANG_KEY))
            {
                return (SystemLanguage)PlayerPrefs.GetInt(FORCE_LANG_KEY);
            }
            else
            {
                return Application.systemLanguage;
            }
        }

        private static int GetLangIndex(SystemLanguage language)
        {
            for (int i = 0; i < mLanguages.Length; i++)
            {
                if (mLanguages[i] == language)
                {
                    return i;
                }
            }
            return -1;
        }

        public static Trans[] GetTranslations()
        {
            return translations;
        }

        public static string s(string s, SystemLanguage language = SystemLanguage.Unknown)
        {
            language = (language == SystemLanguage.Unknown) ? GetLanguage() : language;
            for (int i = 0; i < translations.Length; i++)
            {
                if (translations[i].code == s)
                {
                    int langIndex = GetLangIndex(language);
                    if (langIndex >= 0)
                    {
                        return translations[i].val[langIndex];
                    }
                }
            }
            return s;
        }

        public static string[] GetValuesForKey(string s)
        {
            for (int i = 0; i < translations.Length; i++)
            {
                if (translations[i].code == s)
                {
                    return translations[i].val;
                }
            }
            return new string[0];
        }


#if UNITY_EDITOR
        public static void SaveLocalization(string key, SystemLanguage language, string value)
        {

            int langIndex = GetLangIndex(language);
            SaveLocalization(key, langIndex, value);
        }
        public static void SaveLocalization(string key, int langIndex, string value)
        {
            if (langIndex >= 0)
            {
                for (int i = 0; i < translations.Length; i++)
                {
                    if (translations[i].code.Equals(key))
                    {
                        translations[i].val[langIndex] = value;
                        break;
                    }
                }
                WriteToFile();
            }
        }
        public static void AddTranslation(string key)
        {
            List<Trans> tr = new List<Trans>();
            tr.AddRange(translations);
            tr.Add(new Trans(key, new string[mLanguages.Length]));
            translations = tr.ToArray();
            WriteToFile();
        }
        public static void RemoveTranslation(string key)
        {
            List<Trans> tr = new List<Trans>();
            tr.AddRange(translations);
            for (int i = 0; i < tr.Count; i++)
            {
                if (tr[i].code.Equals(key))
                {
                    tr.RemoveAt(i);
                    break;
                }
            }
            translations = tr.ToArray();
            WriteToFile();
        }
        public static void ChangeKey(string fromKey, string toKey)
        {
            for (int i = 0; i < translations.Length; i++)
            {
                if (translations[i].code.Equals(fromKey))
                {
                    translations[i].code = toKey;
                    break;
                }
            }
            WriteToFile();
        }

        private static void WriteToFile()
        {
            string path = "Assets/Resources/" + TRANSLATIONS_PATH;
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write("#	");
                    for (int i = 0; i < mLanguages.Length; i++)
                    {
                        writer.Write(mLanguages[i].ToString() + "	");
                    }
                    writer.Write(System.Environment.NewLine);
                    for (int i = 0; i < translations.Length; i++)
                    {
                        writer.Write(translations[i].code + "	");
                        for (int j = 0; j < translations[i].val.Length; j++)
                        {
                            if (translations[i].val[j] == null)
                            {
                                translations[i].val[j] = "";
                            }
                            writer.Write(translations[i].val[j].Replace("\n", "<br>") + "	");
                        }
                        writer.Write(System.Environment.NewLine);
                    }
                }
            }
            UnityEditor.AssetDatabase.Refresh();
        }
#endif

#if UNITY_EDITOR
        [MenuItem("Wazzapps/Localizator/Parse Translations")]
#endif
        public static void ParseTranslations()
        {
            TextAsset file = Resources.Load<TextAsset>(
                Path.GetFileNameWithoutExtension(TRANSLATIONS_PATH));

            if (file != null)
            {
                List<Trans> tr = new List<Trans>();
                string[] data = file.text.Split('\n');

                if (data.Length > 0)
                {
                    // Parse language names
                    string[] line = data[0].Trim().Split('	');
                    if (line.Length > 1)
                    {
                        mLanguages = new SystemLanguage[line.Length - 1];
                        for (int i = 1; i < line.Length; i++)
                        {
                            string[] languagesName = GetAllLanguagesNames();
                            bool isFound = false;
                            for (int langId = 0; langId < languagesName.Length; langId++)
                            {
                                if (languagesName[langId].ToLower().Equals(line[i].Trim().ToLower()))
                                {
                                    mLanguages[i - 1] = (SystemLanguage)langId;
                                    isFound = true;
                                    break;
                                }
                            }
                            if (isFound == false)
                            {
                                if (i == 1)
                                {
                                    mLanguages[i - 1] = SystemLanguage.English;
                                }
                                else
                                {
                                    mLanguages[i - 1] = SystemLanguage.Unknown;
                                }
                            }
                        }

                        for (int i = 1; i < data.Length; i++)
                        {
                            line = data[i].Trim().Split('	');
                            if (line.Length > 1 && line[0].Length > 0)
                            {
                                string[] values = new string[line.Length - 1];
                                for (int j = 1; j < line.Length; j++)
                                {
                                    values[j - 1] = line[j].Trim().Replace("\"", "").Replace("<br>", "\n");
                                }
                                tr.Add(new Trans(line[0].Trim().Replace("\"", ""), values));
                                // Debug.Log(tr[tr.Count - 1].ToString());
                            }
                        }
                    }
                }
                translations = tr.ToArray();
            }
        }

        public static string[] GetAllLanguagesNames()
        {
            var values = System.Enum.GetValues(typeof(SystemLanguage));
            List<string> all = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                all.Insert((int)values.GetValue(i), values.GetValue(i).ToString());
            }

            return all.ToArray();
        }


#if UNITY_EDITOR
        [MenuItem(MENU_DEFAULT_LANG)]
#endif
        public static void LocalizatorSetDefault()
        {
            PlayerPrefs.DeleteKey(FORCE_LANG_KEY);
            ClearMenuChecks();

            if (OnLanguageChanged != null)
            {
                OnLanguageChanged();
            }
        }

#if UNITY_EDITOR
        [MenuItem(MENU_RUS_LANG)]
#endif
        public static void LocalizatorSetRussian()
        {
            PlayerPrefs.SetInt(FORCE_LANG_KEY, (int)SystemLanguage.Russian);
            ClearMenuChecks();

            if (OnLanguageChanged != null)
            {
                OnLanguageChanged();
            }
        }

#if UNITY_EDITOR
        [MenuItem(MENU_ENG_LANG)]
#endif
        public static void LocalizatorSetEnglish()
        {
            PlayerPrefs.SetInt(FORCE_LANG_KEY, (int)SystemLanguage.English);
            ClearMenuChecks();

            if (OnLanguageChanged != null)
            {
                OnLanguageChanged();
            }
        }

        private static void ClearMenuChecks()
        {
#if UNITY_EDITOR
            Menu.SetChecked(MENU_DEFAULT_LANG, false);
            Menu.SetChecked(MENU_RUS_LANG, false);
            Menu.SetChecked(MENU_ENG_LANG, false);

            switch (PlayerPrefs.GetInt(FORCE_LANG_KEY, -1))
            {
                case (int)SystemLanguage.English:
                    Menu.SetChecked(MENU_ENG_LANG, true);
                    break;
                case (int)SystemLanguage.Russian:
                    Menu.SetChecked(MENU_RUS_LANG, true);
                    break;
                default:
                    Menu.SetChecked(MENU_DEFAULT_LANG, true);
                    break;
            }

            LocalizeText[] texts = GameObject.FindObjectsOfType<LocalizeText>();
            foreach (LocalizeText t in texts)
            {
                EditorUtility.SetDirty(t);
            }

            LocalizeImage[] images = GameObject.FindObjectsOfType<LocalizeImage>();
            foreach (LocalizeImage t in images)
            {
                EditorUtility.SetDirty(t);
            }
#endif
        }
    }
}