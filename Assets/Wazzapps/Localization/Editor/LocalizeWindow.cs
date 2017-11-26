using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Wazzapps
{
    public class LocalizeWindow : EditorWindow
    {
        Vector2 scrollPos;
        [MenuItem(Localizator.LOCALIZE_WINDOW)]

        public static void ShowWindow()
        {
            EditorWindow.GetWindowWithRect(typeof(LocalizeWindow),
                new Rect(0, 0, 600, 400), false, "Localizations");
        }

        void OnGUI()
        {
            var translations = Localizator.GetTranslations();
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(180));
            for (int j = 0; j < Localizator.Languages.Length; j++)
            {
                GUILayout.Label(Localizator.Languages[j].ToString(), GUILayout.Width(150));
            }
            GUILayout.EndHorizontal();

            using (var h = new EditorGUILayout.HorizontalScope())
            {
                using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos))
                {
                    scrollPos = scrollView.scrollPosition;
                    //GUILayout.BeginScrollView(Vector2.zero);
                    for (int i = 0; i < translations.Length; i++)
                    {
                        GUILayout.BeginHorizontal();


                        if (GUILayout.Button("X", GUILayout.Width(30)))
                        {
                            Localizator.RemoveTranslation(translations[i].code);
                            EditorUtility.SetDirty(this);
                        }
                        var transCode = EditorGUILayout.TextField(translations[i].code, GUILayout.Width(150));
                        if (transCode.Equals(translations[i].code) == false)
                        {
                            Localizator.ChangeKey(translations[i].code, transCode);
                            EditorUtility.SetDirty(this);
                        }
                        for (int j = 0; j < translations[i].val.Length; j++)
                        {
                            var transValue = EditorGUILayout.TextField(translations[i].val[j], GUILayout.Width(150));
                            if (transValue.Equals(translations[i].val[j]) == false)
                            {
                                Localizator.SaveLocalization(translations[i].code, j, transValue);
                                EditorUtility.SetDirty(this);
                            }
                        }
                        GUILayout.EndHorizontal();
                    }

                    if (GUILayout.Button("Add translation"))
                    {
                        Localizator.AddTranslation("transl");
                        EditorUtility.SetDirty(this);
                    }
                }
            }
         //   GUILayout.EndScrollView();
        }
    }
}