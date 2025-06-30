using UnityEngine;



#if UNITY_EDITOR
using UnityEditor;

namespace P01.Editor
{
    public class Tool_Manager : EditorWindow
    {
        [MenuItem("Graphics Tool/00. Tool_Manger")]
        public static void OpenWindow()
        {
            Tool_Manager window = GetWindow<Tool_Manager>("Tool_Manager");
            window.minSize = new Vector2(window.minSize.x, window.minSize.y);
            window.Show();
        }

        private Vector2 scrollPosition;

        void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            SetButtons();
            EditorGUILayout.EndScrollView();
        }

        void SetButtons()
        {
            EditorGUILayout.BeginVertical("box");
            if (SetButton("Resource_Setting"))
            {
                Resource_Setting.OpenWindow();
            }

            if (SetButton("Search For Resource"))
            {
                SearchForResource.OpenWindow();
            }

            if (SetButton("CharacterPreviewTool"))
            {
               
            }
            
            if (SetButton("FindScriptPrefab"))
            {
                FindScriptPrefab.OpenWindow();
            }

            if (SetButton("FullScreenSetting"))
            {
                FullScreenSetting.ShowWindow();
            }
            EditorGUILayout.EndVertical();
        }

        bool SetButton(string _name)
        {
            GUIStyle textStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.yellow },
                fontStyle = FontStyle.Bold,
                fontSize = 15
            };

            string orderStr = "오더 세팅";

            return GUILayout.Button(new GUIContent(_name, orderStr), textStyle, GUILayout.Height(35));
        }
    }
}
#endif