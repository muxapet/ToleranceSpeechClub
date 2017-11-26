#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Wazzapps
{
    [CustomEditor(typeof(LocalizeText))]
    public class LocalizeTextEditor : Editor
    {
        private readonly GUILayoutOption[] defaultGUI = new GUILayoutOption[0];

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Separator();

            LocalizeText locText = (LocalizeText)target;

            string[] values = Localizator.GetValuesForKey(locText.Id);

            for (int i = 0; i < values.Length; i++)
            {
                GUILayout.BeginHorizontal(defaultGUI);
                EditorGUILayout.LabelField(Localizator.Languages[i].ToString(), defaultGUI);

                var newText = EditorGUILayout.TextField(values[i], defaultGUI);
                if (newText.Equals(values[i]) == false)
                {
                    Localizator.SaveLocalization(locText.Id, Localizator.Languages[i], newText);
                    EditorUtility.SetDirty(target);
                }

                GUILayout.EndHorizontal();
            }

        }
    }
}
#endif
