using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Linq;

namespace P01.Editor
{
    public class FindScriptPrefab : EditorWindow
    {
        [MenuItem("Graphics Tool/02. FindScriptPrefab")]
        public static void OpenWindow()
        {
            FindScriptPrefab window = EditorWindow.GetWindow<FindScriptPrefab>("FindScriptPrefab");
            window.minSize = new Vector2(500f, 200f);
            window.Show();
        }

        const string one = "Scene Prefab(Script)";
        const string two = "Scene Prefab(Missing)";
        const string three = "Project Prefab(Missing)";
        const string four = "Selected Prefab(Missing)";
        const string five = "Scene Prefab(String)";
        const string six = "Project Prefab(Script)";

        public struct MissingPrefab
        {
            public string missingName;
            public GameObject prefab;
        }
        List<MissingPrefab> findMissing = new List<MissingPrefab>();
        Vector2 scrollBehaviour;
        bool sortBehaviour, tutorialToggle;

        void OnGUI()
        {
            SetTutorial();
            GUIStyle buttonText = new(GUI.skin.button)
            {
                fontSize = 13,
                normal = { textColor = Color.yellow },
                alignment = TextAnchor.MiddleCenter
            };

            float amount = 3f;
            float width = (position.width - 5f * (amount - 1f)) / amount;

            GUI.color = Color.green;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(five, buttonText, GUILayout.Width(width), GUILayout.Height(30f)))
            {
                FindSceneObject();
            }

            if (GUILayout.Button(six, buttonText, GUILayout.Width(width), GUILayout.Height(30f)))
            {
                
            }

            if (GUILayout.Button(one, buttonText, GUILayout.Height(30f)))
            {
                FindSceneScript();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUI.color = Color.yellow;
            if (GUILayout.Button(two, buttonText, GUILayout.Width(width), GUILayout.Height(30f)))
            {
                FindScene();
            }

            if (GUILayout.Button(three, buttonText, GUILayout.Width(width), GUILayout.Height(30f)))
            {
                FindMissing();
            }

            if (GUILayout.Button(four, buttonText, GUILayout.Height(30f)))
            {
                FindChild();
            }
            EditorGUILayout.EndHorizontal();

            DisplayFindMissing();
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
                GUILayout.Label($" {five} - Scene에서 앞뒤 띄어쓰기 오브젝트 리스팅", guiText);
                GUILayout.Label($" {one} - 스크립트가 붙은 프리팹 리스팅", guiText);
                GUILayout.Label($" {two} - Scene에서 스크립트가 빠진 프리팹 리스팅", guiText);
                GUILayout.Label($" {three} - Project에서 스크립트가 빠진 프리팹 리스팅", guiText);
                GUILayout.Label($" {four} - 선택한 파일(Prefab) 내부 검색", guiText);

                GUILayout.Space(10f);
            }
            EditorGUILayout.EndVertical();
        }

        public void DisplayFindMissing()
        {
            if (findMissing?.Count <= 0)
                return;

            float amount = 2f;
            float setWidth = (position.width - 5f * (amount - 1f)) / amount;

            string temp = "";
            scrollBehaviour = EditorGUILayout.BeginScrollView(scrollBehaviour);
            for (int i = 0; i < findMissing.Count; i++)
            {
                EditorGUILayout.BeginHorizontal("box");
                {
                    if (findMissing[i].prefab != null)
                    {
                        if (temp != findMissing[i].missingName)
                        {
                            temp = findMissing[i].missingName;
                            GUI.color = GUI.color == Color.yellow ? Color.green : Color.yellow;
                        }
                        GUILayout.Label(findMissing[i].missingName, GUILayout.Width(setWidth));
                        EditorGUILayout.ObjectField(findMissing[i].prefab, typeof(GameObject), true);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            GUI.color = Color.white;
            EditorGUILayout.EndScrollView();
        }

        void FindMissing()
        {
            findMissing.Clear();
            string[] prefabPaths = AssetDatabase.GetAllAssetPaths().Where(path
                => path.EndsWith(".prefab", System.StringComparison.OrdinalIgnoreCase)).ToArray();
            foreach (string prefabPath in prefabPaths)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (prefab != null)
                    foreach (Component component in prefab.GetComponentsInChildren<Component>())
                    {
                        string name = prefabPath;
                        if (component == null)
                        {
                            MissingPrefab missing = new MissingPrefab
                            {
                                prefab = prefab,
                                missingName = name,
                            };
                            findMissing.Add(missing);
                            break;
                        }
                    }
            }
        }

        void FindChild()
        {
            findMissing.Clear();

            //Object[] objects = Selection.objects;
            GameObject[] objects = Selection.gameObjects;
            foreach (GameObject obj in objects)
            {
                foreach (Transform trans in obj.GetComponentsInChildren<Transform>(true))
                {
                    foreach (Component component in trans.GetComponents<Component>())
                    {
                        if (component == null)
                        {
                            MissingPrefab missing = new MissingPrefab
                            {
                                prefab = trans.gameObject,
                                missingName = trans.gameObject.name,
                            };
                            findMissing.Add(missing);
                            break;
                        }
                    }
                }
            }
        }

        void FindScene()
        {
            findMissing.Clear();
            GameObject[] objects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.InstanceID);
            foreach (GameObject obj in objects)
            {
                foreach (Component component in obj.GetComponents<Component>())
                {
                    if (component == null)
                    {
                        MissingPrefab missing = new MissingPrefab
                        {
                            prefab = obj.gameObject,
                            missingName = obj.gameObject.name,
                        };
                        findMissing.Add(missing);
                        break;
                    }
                }
            }
        }

        void FindSceneScript()
        {
            findMissing.Clear();
            GameObject[] objects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.InstanceID);
            foreach (GameObject obj in objects)
            {
                foreach (MonoBehaviour component in obj.GetComponents<MonoBehaviour>())
                {
                    if (component != null)
                    {
                        string scriptName = component.GetType().ToString();
                        // 유니티 기본 스크립트 제외
                        if (scriptName.Contains("Unity") == false && scriptName.Contains("TMPro") == false)
                        {
                            MissingPrefab missing = new MissingPrefab
                            {
                                prefab = obj.gameObject,
                                missingName = scriptName,
                            };
                            findMissing.Add(missing);
                            break;
                        }
                    }
                }
            }

            findMissing.Sort(delegate (MissingPrefab a, MissingPrefab b)
            {
                return a.missingName.CompareTo(b.missingName);
            });
        }

        void FindSceneObject()
        {
            findMissing.Clear();
            GameObject[] objects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.InstanceID);
            foreach (GameObject obj in objects)
            {
                string name = obj.name;
                if (name.Length > 0)
                {
                    if (name.StartsWith(" ") || name.EndsWith(" "))
                    {
                        MissingPrefab missing = new MissingPrefab
                        {
                            prefab = obj.gameObject,
                            missingName = name,
                        };
                        findMissing.Add(missing);
                    }
                }
            }

            findMissing.Sort(delegate (MissingPrefab a, MissingPrefab b)
            {
                return a.missingName.CompareTo(b.missingName);
            });
        }
    }
}
#endif