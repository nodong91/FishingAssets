using UnityEngine;
using System.Collections.Generic;
using static Data_Manager;



#if UNITY_EDITOR
using UnityEditor;

namespace P01.Editor
{

    public class Editor_SkillTree : EditorWindow
    {
        [MenuItem("Graphics Tool/11. Editor_SkillTree")]
        public static void OpenWindow()
        {
            Editor_SkillTree window = EditorWindow.GetWindow<Editor_SkillTree>("Editor_SkillTree");
            window.minSize = new Vector2(500f, 200f);
            window.Show();
        }
        Vector2Int skillMap;
        Vector2 scrollPosition;

        StatusStruct[,] statusStructs;
        SerializedObject targetObject;

        private void OnEnable()
        {
            targetObject = new SerializedObject(this);
        }

        void OnGUI()
        {
            GUIStyle buttonText = new(GUI.skin.button)
            {
                fontSize = 13,
                normal = { textColor = Color.yellow },
                alignment = TextAnchor.MiddleCenter
            };

            skillMap = EditorGUILayout.Vector2IntField("SkillMap", skillMap);
            if (GUILayout.Button($"Set Field : {skillMap}", buttonText, GUILayout.Height(30f)))
            {
                SetNode();
            }

            if (statusStructs == null)
                return;

            NodeDisplay();
            GUILayout.Space(10f);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button($"Save", buttonText, GUILayout.Height(30f)))
            {
                if (EditorUtility.DisplayDialog("Save all editor preferences.",
                "기존 데이터가 제거 됩니다.\n저장 하시겠음??", "응", "아니") == true)
                {
                    SaveData();
                    Debug.Log("yes");
                }
            }

            if (GUILayout.Button($"Load", buttonText, GUILayout.Height(30f)))
            {
                if (Static_JsonManager.TryLoadSkillData("SkillMap", out List<StatusStruct> _statusStructs))
                {
                    int index = 0;
                    for (int y = 0; y < skillMap.y; y++)
                    {
                        for (int x = 0; x < skillMap.x; x++)
                        {
                            statusStructs[x, y] = _statusStructs[index];
                            index++;
                        }
                    }
                }
                else
                {
                    Debug.LogError("Failed to load skill data.");
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        void SaveData()
        {
            List<StatusStruct> status = new List<StatusStruct>();
            for (int y = 0; y < skillMap.y; y++)
            {
                for (int x = 0; x < skillMap.x; x++)
                {
                    StatusStruct skillClass = statusStructs[x, y];
                    status.Add(skillClass);
                }
            }
            Debug.LogError($"{status.Count}개 저장");
            Static_JsonManager.SaveSkillData("SkillMap", status);
        }

        void SetNode()
        {
            statusStructs = new StatusStruct[skillMap.x, skillMap.y];
            for (int y = 0; y < skillMap.y; y++)
            {
                for (int x = 0; x < skillMap.x; x++)
                {
                    statusStructs[x, y] = new StatusStruct();
                    statusStructs[x, y].setStatus = new List<StatusStruct.SetStruct>();
                }
            }
        }

        void NodeDisplay()
        {
            // 버튼 스타일 설정
            if (statusStructs == null)
                return;

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            float amount = (float)skillMap.x;
            float width = (position.width - 3.2f * (amount)) / amount;
            for (int y = 0; y < skillMap.y; y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < skillMap.x; x++)
                {
                    SetButton(x, y, width);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        void SetButton(int _x, int _y, float _width = 0)
        {
            GUIStyle buttonText = new(GUI.skin.button)
            {
                fontSize = 13,
                normal = { textColor = Color.yellow },
                alignment = TextAnchor.MiddleCenter
            };

            StatusStruct setStatus = statusStructs[_x, _y];

            string setName = $"{setStatus.name} ({_x}:{_y})";
            setName += $"\nCount : {setStatus.setStatus.Count}";
            setName += $"\nPrice : {setStatus.price}";
            GUI.color = (setStatus.setStatus.Count > 0) ? Color.white : Color.gray;

            if (GUILayout.Button(setName, buttonText, GUILayout.Width(_width), GUILayout.Height(_width)))
            {
                OpenSettingWindow(setStatus);
            }
            GUI.color = Color.white;
        }

        private void OpenSettingWindow(StatusStruct _setStatus)
        {
            // 새 창 열기
            SkillSettingWindow.ShowWindow(_setStatus);
        }
    }

    public class SkillSettingWindow : EditorWindow
    {
        public static void ShowWindow(StatusStruct _setStatus)
        {
            // 새 창을 열고 의존성 경로 가져오기
            SkillSettingWindow window = GetWindow<SkillSettingWindow>($"Slot Infomation");
            window.GetDependencies(_setStatus);
            window.Show();
        }
        [SerializeField] private StatusStruct setStatus;
        SerializedObject targetObject;
        Vector2 scrollPosition;

        public void GetDependencies(StatusStruct _setStatus)
        {
            targetObject = new SerializedObject(this);
            setStatus = _setStatus;
        }

        void OnGUI()
        {
            if (targetObject != null)
            {
                targetObject.Update();

                // setStatus는 위에서 선언한 List의 변수명
                SerializedProperty prop = targetObject.FindProperty("setStatus");
                EditorGUILayout.PropertyField(prop, new GUIContent(prop.displayName));

                targetObject.ApplyModifiedProperties();

                // 예시로 버튼을 추가
                if (GUILayout.Button("Close", GUILayout.Height(30f)))
                {
                    Close();
                }

            }
        }
    }
}
#endif