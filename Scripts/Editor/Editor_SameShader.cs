using UnityEngine;
using System.Collections.Generic;



#if UNITY_EDITOR
using UnityEditor;
namespace P01.Editor
{
    public class Editor_SameShader : EditorWindow
    {
        [MenuItem("Graphics Tool/10. SameShader")]
        public static void OpenWindow()
        {
            Editor_SameShader window = EditorWindow.GetWindow<Editor_SameShader>("Editor_SameShader");
            window.minSize = new Vector2(500f, 200f);
            window.Show();
        }

        bool tutorialToggle = false;
        SerializedObject targetObject;

        public List<Material> materials = new List<Material>();
        [SerializeField] private List<Object> ResourceList = new List<Object>();
        Vector2 scrollMaterial;
        Shader targetShader;
        List<Material> tempList = new List<Material>();
        bool sortMaterial;

        private void OnEnable()
        {
            targetObject = new SerializedObject(this);
            ResourceList.Clear();
        }

        void OnGUI()
        {
            GUIStyle fontStyle = new()
            {
                fontSize = 13,
                normal = { textColor = Color.yellow },
                alignment = TextAnchor.MiddleLeft
            };

            GUIStyle buttonStyle = new(GUI.skin.button)
            {
                fontSize = 13,
                normal = { textColor = Color.yellow },
                alignment = TextAnchor.MiddleCenter
            };

            if (GUILayout.Button("사용법", buttonStyle, GUILayout.Height(20f)))
            {
                tutorialToggle = !tutorialToggle;
            }
            ShowTutorial(fontStyle);

            GUILayout.BeginHorizontal("box");
            if (targetObject != null)
            {
                targetObject.Update();

                // objectList 는 위에서 선언한 List의 변수명
                SerializedProperty testProp = targetObject.FindProperty("ResourceList");
                EditorGUILayout.PropertyField(testProp, new GUIContent(testProp.displayName));

                targetObject.ApplyModifiedProperties();
            }
            GUILayout.EndHorizontal();
            DisplayMaterial();

            buttonStyle.fontSize = 18;
            buttonStyle.fontStyle = FontStyle.Bold;
            if (GUILayout.Button("Find All Material", buttonStyle, GUILayout.Height(50f)))
            {
                SetResource();
                tempList = materials;
            }
        }

        void ShowTutorial(GUIStyle _fontStyle)
        {
            if (tutorialToggle == true)
            {
                _fontStyle.normal.textColor = Color.gray;
                GUILayout.Space(10f);
                GUILayout.Label(" 1. ResourceList에 폴더 추가", _fontStyle);
                GUILayout.Label(" 2. Find All Material 클릭으로 폴더내 모든 Material 리스팅", _fontStyle);
                GUILayout.Label(" 3. 빈 슬롯에 찾을 Shader 추가", _fontStyle);
                GUILayout.Label(" 4. Check Listing 버튼 클릭", _fontStyle);

                GUILayout.Space(10f);
            }
        }

        void SetResource()
        {
            if (ResourceList.Count == 0)
            {
                Debug.LogError("DataFolders 폴더 필요");
                return;
            }

            materials.Clear();
            string[] paths = new string[ResourceList.Count];
            for (int i = 0; i < ResourceList.Count; i++)
            {
                paths[i] = AssetDatabase.GetAssetPath(ResourceList[i]);
                Debug.LogWarning("File paths : " + paths[i]);
            }
            FindMaterial(paths);
        }

        void FindMaterial(string[] _paths)
        {
            materials.Clear();
            string[] assets = AssetDatabase.FindAssets("t: Material", _paths);
            // 데이터 추가
            for (int i = 0; i < assets.Length; i++)
            {
                var data = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[i]), typeof(Material)) as Material;
                materials.Add(data);
            }
        }

        void DisplayMaterial()
        {
            if (materials?.Count <= 0)
                return;

            float setWidth = SetButtonWidth(2f);
            float setHight = 35;

            GUIStyle buttonText = new(GUI.skin.button)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleCenter
            };

            GUIStyle centerText = new(GUI.skin.box)
            {
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleLeft
            };

            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.BeginHorizontal("box");
                if (SetButton("Material", setWidth, setHight))
                {
                    tempList.Sort(delegate (Material a, Material b)
                    {
                        if (sortMaterial == true)
                            return a.name.CompareTo(b.name);
                        else
                            return b.name.CompareTo(a.name);
                    });
                    sortMaterial = !sortMaterial;
                }
                if (SetButton("Shader", setHight))
                {
                    tempList.Sort(delegate (Material a, Material b)
                    {
                        if (a.shader == b.shader)
                            return a.name.CompareTo(b.name);

                        if (sortMaterial == true)
                            return a.shader.name.CompareTo(b.shader.name);
                        else
                            return b.shader.name.CompareTo(a.shader.name);
                    });
                    sortMaterial = !sortMaterial;
                }
                EditorGUILayout.EndHorizontal();

                setHight = 20f;
                scrollMaterial = EditorGUILayout.BeginScrollView(scrollMaterial);

                for (int i = 0; i < tempList.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    tempList[i] = EditorGUILayout.ObjectField("", tempList[i], typeof(Material), true, GUILayout.Width(setWidth), GUILayout.Height(setHight)) as Material;
                    string shaderName = tempList[i].shader.name;
                    EditorGUILayout.ObjectField("", tempList[i].shader, typeof(Shader), true, GUILayout.Height(setHight));

                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();

                EditorGUILayout.BeginHorizontal("box");
                targetShader = EditorGUILayout.ObjectField("", targetShader, typeof(Shader), true, GUILayout.Width(setWidth), GUILayout.Height(35)) as Shader;

                if (SetButton("Check Listing", 35))
                {
                    tempList = FindSame();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        List<Material> FindSame()
        {
            tempList = new List<Material>();
            for (int i = 0; i < materials.Count; i++)
            {
                if (materials[i].shader == targetShader)
                {
                    tempList.Add(materials[i]);
                }
            }
            return tempList;
        }

        public float SetButtonWidth(float _value)
        {
            float setWidth = (position.width - 5f * (_value - 1f)) / _value;
            return setWidth;
        }

        public bool SetButton(string _name, float _setWidth, float _setHight)
        {
            GUIStyle textStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.yellow },
                fontStyle = FontStyle.Bold,
                fontSize = 15
            };

            string orderStr = "오더 세팅";
            return GUILayout.Button(new GUIContent(_name, orderStr), textStyle, GUILayout.Width(_setWidth), GUILayout.Height(_setHight));
        }

        public bool SetButton(string _name, float _setHight)
        {
            GUIStyle textStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.yellow },
                fontStyle = FontStyle.Bold,
                fontSize = 15
            };
            return GUILayout.Button(new GUIContent(_name), textStyle, GUILayout.Height(_setHight));
        }
    }
}
#endif