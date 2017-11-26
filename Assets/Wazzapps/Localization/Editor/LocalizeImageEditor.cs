#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Wazzapps
{
    [CustomEditor(typeof(LocalizeImage))]
    public class LocalizeImageEditor : Editor
    {
        private readonly GUILayoutOption[] defaultGUI = new GUILayoutOption[0];
        public override void OnInspectorGUI()
        {
            LocalizeImage locImage = (LocalizeImage)target;

            GUILayout.BeginHorizontal(defaultGUI);
            GUILayout.Label("Default sprite", defaultGUI);
            var newDefaultSprite = (Sprite)EditorGUILayout.ObjectField(locImage.DefaultSprite, typeof(Sprite), true);
            if (newDefaultSprite != locImage.DefaultSprite)
            {
                locImage.DefaultSprite = newDefaultSprite;
                EditorUtility.SetDirty(target);
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Add Language"))
            {
                Undo.RecordObject(target, "Add language");
                locImage.AddLanguage();
            }

            if (locImage.Sprites != null)
            {
                string[] options = Localizator.GetAllLanguagesNames();

                for (int i = 0; i < locImage.Sprites.Length; i++)
                {
                    SystemLanguage langId = locImage.Languages[i];

                    GUILayout.BeginHorizontal(defaultGUI);
                    if (GUILayout.Button("X"))
                    {
                        Undo.RecordObject(target, "Remove");
                        locImage.RemoveLanguage(locImage.Languages[i]);
                        EditorUtility.SetDirty(target);
                        break;
                    }

                    int choosedLang = EditorGUILayout.Popup("", (int)langId, options, defaultGUI);
                    if (choosedLang != (int)langId)
                    {
                        int index = locImage.HasTranslations((SystemLanguage)choosedLang);
                        if (index >= 0)
                        {
                            locImage.Sprites[index] = locImage.Sprites[i];
                            locImage.RemoveLanguage(langId);
                            EditorUtility.SetDirty(target);
                            break;
                        }
                        else
                        {
                            locImage.Languages[i] = (SystemLanguage)choosedLang;
                            EditorUtility.SetDirty(target);
                            break;
                        }
                    }
                    var newSprite = (Sprite)EditorGUILayout.ObjectField(locImage.Sprites[i], typeof(Sprite), true);
                    if (newSprite != locImage.Sprites[i])
                    {
                        locImage.Sprites[i] = newSprite;
                        EditorUtility.SetDirty(target);
                    }

                    GUILayout.EndHorizontal();
                }
            }
        }
    }
}
#endif