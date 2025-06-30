
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
# if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;  // EditorSceneManager를 사용하기 위해 추가
namespace P01.Editor
{
    public class SearchForResource : EditorWindow
    {
        [MenuItem("Graphics Tool/04. Search For Resource")]
        public static void OpenWindow()
        {
            SearchForResource window = GetWindow<SearchForResource>("SearchForResource"); // 클래스 이름과 일치
            window.minSize = new Vector2(500f, 200f);
            window.Show();
        }

        private List<Object> registeredObjects = new List<Object>();
        private Vector2 scrollPosition;
        //private Object selectedObject;
        //private const float labelWidth = 200f;  // 씬 이름의 고정 너비
        //private const float buttonWidth = 80f;  // 버튼의 고정 너비
        private const string RegisteredObjectsKey = "RegisteredObjects";  // EditorPrefs에 저장할 키

        // 의존성 경로를 저장할 리스트
        private List<string> chaseDependencies = new List<string>();
        bool tutorialToggle;
        Object[] objects;

        string showFile = "";
        //Object source;

        private void OnEnable()
        {
            // EditorPrefs에서 등록된 객체 리스트를 복원
            LoadRegisteredObjects();
        }

        //private void OnDisable()
        //{
        //    // EditorPrefs에 등록된 객체 리스트를 저장
        //    SaveRegisteredObjects();
        //}

        //[MenuItem("Tools/RemoveEditorPrefs")]
        static void DeleteAllExample()
        {
            if (EditorUtility.DisplayDialog("Delete all editor preferences.",
                "기존 데이터가 제거 됩니다.\n" +
                "레알 삭제하시겠음??", "Yes", "No"))
            {
                Debug.Log("yes");
                EditorPrefs.DeleteKey(RegisteredObjectsKey);
            }
        }

        private void OnGUI()
        {
            //source = EditorGUILayout.ObjectField(source, typeof(Object), true)as Object;
            SetTutorial();
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white },
                fontStyle = FontStyle.Bold,
                fontSize = 17
            };
            objects = Selection.objects;

            float buttonNum = 4f;
            float width = (position.width - 7f * (buttonNum - 1f)) / buttonNum;
            GUILayout.BeginVertical("box");
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Add", buttonStyle, GUILayout.Width(width)))
                {
                    for (int i = 0; i < objects.Length; i++)
                    {
                        AddObjectToList(objects[i]);
                    }
                }
                if (GUILayout.Button("Save", buttonStyle, GUILayout.Width(width)))
                {
                    SaveRegisteredObjects();
                }
                if (GUILayout.Button("Load", buttonStyle, GUILayout.Width(width)))
                {
                    LoadRegisteredObjects();
                }
                GUI.color = Color.red;
                if (GUILayout.Button("Reset", buttonStyle))
                {
                    DeleteAllExample();
                    //registeredObjects.Clear();
                }
                GUILayout.EndHorizontal();
                SortCategory();
            }
            GUILayout.EndVertical();
            GUILayout.Space(5);

            //GUILayout.Label("Registered List:", EditorStyles.boldLabel);

            //// 등록된 객체 리스트를 분류하여 정렬
            //List<Object> sortedObjects = registeredObjects
            //    .OrderBy(obj => obj.GetType().Name)  // 타입별로 정렬
            //    .ThenBy(obj => obj.name)  // 이름별로 정렬
            //    .ToList();
            // 등록된 객체 리스트를 분류하여 정렬
            List<Object> sortedObjects = registeredObjects
                .OrderByDescending(obj => obj.name)  // 이름별로 정렬
                .ToList();

            GUIStyle fontStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = Color.white },
                //fontStyle = FontStyle.Bold,
                fontSize = 15
            };

            float windowWidth = position.width;
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(windowWidth), GUILayout.Height(position.height - 100));
            for (int i = sortedObjects.Count - 1; i >= 0; i--)
            {
                string fileExtension = GetFileExtension(sortedObjects[i]);
                if (fileExtension == showFile || showFile == "")
                {
                    GUILayout.BeginHorizontal("box");
                    {
                        GUILayout.Label($"{sortedObjects[i].name} <color=#808080>({fileExtension})</color>", fontStyle);
                        GUILayout.FlexibleSpace();// 공간만들기
                        GUI.color = Color.cyan;
                        if (sortedObjects[i] is SceneAsset)
                        {
                            if (GUILayout.Button("Open", GUILayout.Width(windowWidth * 0.1f)))
                            {
                                OpenSceneWithPrompt(sortedObjects[i]);
                            }
                        }

                        GUI.color = Color.green;
                        if (GUILayout.Button("Search", GUILayout.Width(windowWidth * 0.1f)))
                        {
                            EditorGUIUtility.PingObject(sortedObjects[i]);
                        }

                        GUI.color = Color.yellow;
                        if (GUILayout.Button("Chase", GUILayout.Width(windowWidth * 0.1f)))
                        {
                            OpenChaseWindow(sortedObjects[i]);
                        }

                        GUI.color = Color.red;
                        if (GUILayout.Button("Remove", GUILayout.Width(windowWidth * 0.1f)))
                        {
                            RemoveObjectFromList(sortedObjects[i]);
                            GUILayout.EndHorizontal();
                            continue;
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUI.color = Color.white;  // 원래 색상으로 복귀
            }
            //GUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }

        void SortCategory()
        {
            GUI.color = Color.white;
            float buttonNum = 9f;
            float width = (position.width - 7f * (buttonNum - 1f)) / buttonNum;
            GUILayout.BeginHorizontal();
            GUI.color = showFile == "" ? Color.yellow : Color.white;
            if (GUILayout.Button("All", GUILayout.Width(width)))
            {
                showFile = "";
            }
            GUI.color = showFile == "unity" ? Color.yellow : Color.white;
            if (GUILayout.Button("Scene", GUILayout.Width(width)))
            {
                showFile = "unity";
            }
            GUI.color = showFile == "asset" ? Color.yellow : Color.white;
            if (GUILayout.Button("Asset", GUILayout.Width(width)))
            {
                showFile = "asset";
            }
            GUI.color = showFile == "prefab" ? Color.yellow : Color.white;
            if (GUILayout.Button("Prefab", GUILayout.Width(width)))
            {
                showFile = "prefab";
            }
            GUI.color = showFile == "cs" ? Color.yellow : Color.white;
            if (GUILayout.Button("Script", GUILayout.Width(width)))
            {
                showFile = "cs";
            }
            GUI.color = showFile == "mat" ? Color.yellow : Color.white;
            if (GUILayout.Button("Material", GUILayout.Width(width)))
            {
                showFile = "mat";
            }
            GUI.color = showFile == "png" ? Color.yellow : Color.white;
            if (GUILayout.Button("Png", GUILayout.Width(width)))
            {
                showFile = "png";
            }
            GUI.color = showFile == "shadergraph" ? Color.yellow : Color.white;
            if (GUILayout.Button("Shader", GUILayout.Width(width)))
            {
                showFile = "shadergraph";
            }
            GUI.color = showFile == "anim" ? Color.yellow : Color.white;
            if (GUILayout.Button("Anim"))
            {
                showFile = "anim";
            }
            GUILayout.EndHorizontal();
        }

        void SetTutorial()
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
                GUILayout.Label(" 1. 리스팅을 하기 위한 오브젝트를 Project에서 선택", guiText);
                GUILayout.Label(" 2. Add - 선택한 오브젝트 리스팅", guiText);
                GUILayout.Label(" 3. Save - 리스팅 저장", guiText);
                GUILayout.Label(" 4. Load - 저장된 리스트 불러오기", guiText);
                GUILayout.Label(" 5. Reset - 모든 리스팅 리셋", guiText);
                GUILayout.Space(10f);
                GUILayout.Label(" Registered List", guiText);
                GUILayout.Label(" 1. Open - Scene 이동", guiText);
                GUILayout.Label(" 2. Search - 오브젝트 선택", guiText);
                GUILayout.Label(" 3. Chase - 링크된 오브젝트 리스팅", guiText);
                GUILayout.Label(" 4. Remove - 리스트에서 제거", guiText);

                GUILayout.Space(10f);
            }
            EditorGUILayout.EndVertical();
        }

        private void AddObjectToList(Object obj)
        {
            if (obj != null)
            {
                if (!registeredObjects.Contains(obj))
                {
                    registeredObjects.Add(obj);
                    Debug.Log($"{obj.name} has been added to the list.");
                }
                else
                {
                    EditorUtility.DisplayDialog("중복된 리소스", "리스트에 등록되어 있습니다.", "확인");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Warning", "No object selected. Please select a resource.", "OK");
            }
        }

        private void RemoveObjectFromList(Object obj)
        {
            Debug.Log($"{obj.name} has been removed from the list.");
            registeredObjects.Remove(obj);
        }

        private void OpenSceneWithPrompt(Object sceneObject)
        {
            if (EditorSceneManager.GetActiveScene().isDirty)  // EditorSceneManager 사용
            {
                if (EditorUtility.DisplayDialog("씬 저장", "현재 씬을 저장하시겠습니까?", "예", "아니오"))
                {
                    EditorSceneManager.SaveOpenScenes();  // EditorSceneManager 사용
                }
            }

            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(sceneObject));  // EditorSceneManager 사용
        }

        private string GetFileExtension(Object obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            return System.IO.Path.GetExtension(path).TrimStart('.').ToLower();
        }

        // Chase 버튼을 눌렀을 때 새로운 창을 열어 경로를 버튼으로 표시
        private void OpenChaseWindow(Object obj)
        {
            // 새 창 열기
            ChaseWindow.ShowWindow(obj);
        }

        private void SaveRegisteredObjects()
        {
            List<string> paths = new List<string>();
            foreach (var obj in registeredObjects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path))
                {
                    paths.Add(path);
                }
            }
            string warning = "기존 내용 위에 저장 됩니다.";
            if (EditorUtility.DisplayDialog("저장", warning, "그래", "아니요") == true)
            {
                EditorPrefs.SetString(RegisteredObjectsKey, string.Join(";", paths));
            }
        }

        private void LoadRegisteredObjects()
        {
            registeredObjects.Clear();
            string savedData = EditorPrefs.GetString(RegisteredObjectsKey, "");
            if (!string.IsNullOrEmpty(savedData))
            {
                string[] paths = savedData.Split(';');
                foreach (var path in paths)
                {
                    Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                    if (obj != null)
                    {
                        registeredObjects.Add(obj);
                    }
                }
            }
        }
    }

    // 새로운 창을 열어서 경로를 버튼으로 표시하는 클래스
    public class ChaseWindow : EditorWindow
    {
        private static List<string> chaseDependencies = new List<string>();
        private Vector2 scrollPos;

        public static void ShowWindow(Object obj)
        {
            // 새 창을 열고 의존성 경로 가져오기
            ChaseWindow window = GetWindow<ChaseWindow>($"{obj.name} Usage");
            window.GetDependencies(obj);
            window.Show();
        }

        private void GetDependencies(Object obj)
        {
            // 의존성 경로를 가져와 리스트에 저장
            string assetPath = AssetDatabase.GetAssetPath(obj);
            string[] dependencies = AssetDatabase.GetDependencies(assetPath);
            chaseDependencies = dependencies.ToList();
            if (chaseDependencies.Contains(assetPath))
            {
                chaseDependencies.Remove(assetPath);
            }
        }

        private void OnGUI()
        {
            //GUILayout.Label("Chase Results:", EditorStyles.boldLabel);
            SortCategory();
            GUILayout.Space(5);

            GUI.color = Color.white;
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(position.height - 50));

            GUIStyle buttonText = new(GUI.skin.button)
            {
                fontSize = 15,
                fontStyle = FontStyle.Normal,
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleLeft
            };

            // 경로를 버튼으로 표시
            foreach (var dependency in chaseDependencies)
            {
                string[] fileExtension = dependency.ToString().Split('.');
                string[] fileName = dependency.ToString().Split('/');

                if (fileExtension[^1].Equals(showFile) || showFile == "")
                {
                    if (GUILayout.Button(fileName[^1], buttonText))
                    {
                        Object targetObject = AssetDatabase.LoadAssetAtPath<Object>(dependency);
                        if (targetObject != null)
                        {
                            EditorGUIUtility.PingObject(targetObject);  // 경로 추적
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Error", $"Could not find object at path: {dependency}", "OK");
                        }
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }

        string showFile = "";
        void SortCategory()
        {
            GUI.color = Color.white;
            float buttonNum = 9f;
            float width = (position.width - 7f * (buttonNum - 1f)) / buttonNum;
            GUILayout.BeginHorizontal();
            GUI.color = showFile == "" ? Color.yellow : Color.white;
            if (GUILayout.Button("All", GUILayout.Width(width)))
            {
                showFile = "";
            }
            GUI.color = showFile == "unity" ? Color.yellow : Color.white;
            if (GUILayout.Button("Scene", GUILayout.Width(width)))
            {
                showFile = "unity";
            }
            GUI.color = showFile == "asset" ? Color.yellow : Color.white;
            if (GUILayout.Button("Asset", GUILayout.Width(width)))
            {
                showFile = "asset";
            }
            GUI.color = showFile == "prefab" ? Color.yellow : Color.white;
            if (GUILayout.Button("Prefab", GUILayout.Width(width)))
            {
                showFile = "prefab";
            }
            GUI.color = showFile == "cs" ? Color.yellow : Color.white;
            if (GUILayout.Button("Script", GUILayout.Width(width)))
            {
                showFile = "cs";
            }
            GUI.color = showFile == "mat" ? Color.yellow : Color.white;
            if (GUILayout.Button("Material", GUILayout.Width(width)))
            {
                showFile = "mat";
            }
            GUI.color = showFile == "png" ? Color.yellow : Color.white;
            if (GUILayout.Button("Png", GUILayout.Width(width)))
            {
                showFile = "png";
            }
            GUI.color = showFile == "shadergraph" ? Color.yellow : Color.white;
            if (GUILayout.Button("Shader", GUILayout.Width(width)))
            {
                showFile = "shadergraph";
            }
            GUI.color = showFile == "anim" ? Color.yellow : Color.white;
            if (GUILayout.Button("Anim"))
            {
                showFile = "anim";
            }
            GUILayout.EndHorizontal();
        }
    }
}
#endif