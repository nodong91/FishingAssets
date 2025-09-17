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
        Vector2 scrollPosition;

        SkillStatus[,] statusStructs;
        SerializedObject targetObject;
        bool tutorialToggle;
        Data_SkillTree skillTreeData;
        Vector2Int startSlot;

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
            GUIStyle guiText = new()
            {
                fontSize = 13,
                normal = { textColor = Color.yellow },
                alignment = TextAnchor.MiddleLeft
            };

            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("사용법", buttonText, GUILayout.Height(20f)))
            {
                tutorialToggle = !tutorialToggle;
            }
            if (tutorialToggle == true)
            {
                guiText.normal.textColor = Color.gray;
                GUILayout.Space(10f);
                GUILayout.Label(" 1. SkillMap 에 노드 사이즈 기입", guiText);
                GUILayout.Label(" 2. Set Field 클릭", guiText);
                GUILayout.Label(" 3. 저장된 파일이 있는 경우 Load 선택", guiText);
                GUILayout.Label(" 4. 노드 버튼 클릭 후 내용 기입", guiText);
                GUILayout.Label(" 5. Save 클릭하여 스킬 트리 저장", guiText);

                GUILayout.Space(10f);
            }
            EditorGUILayout.EndVertical();

            startSlot = EditorGUILayout.Vector2IntField("Start Slot", startSlot);
            skillTreeData = EditorGUILayout.ObjectField(skillTreeData, typeof(Data_SkillTree), true) as Data_SkillTree;
            if (skillTreeData == null)
            {
                GUILayout.Space(10f);
                GUILayout.Label(" Data_SkillTree 데이터가 없음", guiText);
                return;
            }
            if (GUILayout.Button($"Set Field : {skillTreeData.skillMapSize}", buttonText, GUILayout.Height(30f)))
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
                LoadData();
            }
            EditorGUILayout.EndHorizontal();
        }

        void SaveData()
        {
            List<SkillStatus> status = new List<SkillStatus>();
            for (int y = 0; y < skillTreeData.skillMapSize.y; y++)
            {
                for (int x = 0; x < skillTreeData.skillMapSize.x; x++)
                {
                    SkillStatus skillClass = statusStructs[x, y];
                    status.Add(skillClass);
                }
            }
            skillTreeData.skillList = status;
            skillTreeData.startSlot = startSlot;
            Debug.LogError($"{status.Count}개 저장");
            EditorUtility.SetDirty(skillTreeData);
        }

        void LoadData()
        {
            if (skillTreeData == null)
                return;

            int index = 0;
            for (int y = 0; y < skillTreeData.skillMapSize.y; y++)
            {
                for (int x = 0; x < skillTreeData.skillMapSize.x; x++)
                {
                    statusStructs[x, y] = skillTreeData.skillList[index];
                    index++;
                }
            }
            startSlot = skillTreeData.startSlot;
        }

        void SetNode()
        {
            statusStructs = new SkillStatus[skillTreeData.skillMapSize.x, skillTreeData.skillMapSize.y];
            for (int y = 0; y < skillTreeData.skillMapSize.y; y++)
            {
                for (int x = 0; x < skillTreeData.skillMapSize.x; x++)
                {
                    statusStructs[x, y] = new SkillStatus();
                    //statusStructs[x, y].setStatus = new List<StatusStruct.SetStruct>();
                }
            }
        }

        void NodeDisplay()
        {
            // 버튼 스타일 설정
            if (statusStructs == null)
                return;

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            float amount = (float)skillTreeData.skillMapSize.x;
            float width = (position.width - 3.2f * (amount)) / amount;
            for (int y = 0; y < skillTreeData.skillMapSize.y; y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < skillTreeData.skillMapSize.x; x++)
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

            SkillStatus setStatus = statusStructs[_x, _y];

            string setName = $"{setStatus.name} ({_x}:{_y})";
            setName += $"\nPrice : {setStatus.price}";
            GUI.color = (setStatus.name?.Length > 0) ? Color.white : Color.gray;
            GUI.color = (skillTreeData.startSlot.x == _x && skillTreeData.startSlot.y == _y) ? Color.red : GUI.color;
            if (GUILayout.Button(setName, buttonText, GUILayout.Width(_width), GUILayout.Height(_width)))
            {
                OpenSettingWindow(setStatus);
            }
            GUI.color = Color.white;
        }

        private void OpenSettingWindow(SkillStatus _setStatus)
        {
            // 새 창 열기
            SkillSettingWindow.ShowWindow(_setStatus);
        }
    }

    //=====================================================================================================================
    // 새 창 열기
    //=====================================================================================================================

    public class SkillSettingWindow : EditorWindow
    {
        public static void ShowWindow(SkillStatus _setStatus)
        {
            // 새 창을 열고 의존성 경로 가져오기
            SkillSettingWindow window = GetWindow<SkillSettingWindow>($"Slot Infomation");
            window.GetDependencies(_setStatus);
            window.Show();
        }
        [SerializeField] private SkillStatus setStatus;
        SerializedObject targetObject;
        Vector2 scrollPosition;

        public void GetDependencies(SkillStatus _setStatus)
        {
            targetObject = new SerializedObject(this);
            setStatus = _setStatus;
        }

        void OnGUI()
        {
            if (targetObject != null)
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                targetObject.Update();

                // setStatus는 위에서 선언한 List의 변수명
                SerializedProperty prop = targetObject.FindProperty("setStatus");
                EditorGUILayout.PropertyField(prop, new GUIContent(prop.displayName));
                targetObject.ApplyModifiedProperties();
                EditorGUILayout.EndScrollView();

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