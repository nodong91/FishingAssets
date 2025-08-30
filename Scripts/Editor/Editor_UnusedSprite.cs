using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

#if UNITY_EDITOR
namespace P01.Editor
{
    public class Editor_UnusedSprite : EditorWindow
    {
        [MenuItem("Graphics Tool/08. Unused files")]
        public static void OpenWindow()
        {
            Editor_UnusedSprite window = GetWindow<Editor_UnusedSprite>("Editor_UnusedSprite"); // 클래스 이름과 일치
            window.minSize = new Vector2(500f, 200f);
            window.Show();
        }

        bool tutorialToggle;
        GUIStyle guiText, buttonText;
        Vector2 imageScroll, spriteScroll;

        public struct PrefabList
        {
            public GameObject listGameObject;
            public Sprite listSprite;
        }
        List<PrefabList> prefabs = new List<PrefabList>();
        public struct SpriteList
        {
            //public string listPath;
            public Sprite listSprite;
        }
        List<SpriteList> sprites = new List<SpriteList>();
        bool sortPrefab, sortSprite;

        void OnGUI()
        {
            guiText = new()
            {
                fontSize = 13,
                normal = { textColor = Color.yellow },
                alignment = TextAnchor.MiddleLeft
            };
            Tutorial();

            buttonText.fontStyle = FontStyle.Bold;
            EditorGUILayout.BeginHorizontal("box");
            if (targetObject != null)
            {
                targetObject.Update();

                // objectList 는 위에서 선언한 List의 변수명
                SerializedProperty testProp = targetObject.FindProperty("ResourceList");
                EditorGUILayout.PropertyField(testProp, new GUIContent(testProp.displayName));

                targetObject.ApplyModifiedProperties();
            }
            EditorGUILayout.EndHorizontal();

            float width = position.width * 0.5f;
            if (prefabs.Count > 0)
            {
                EditorGUILayout.BeginHorizontal("box");
                if (GUILayout.Button("Sort Prefab", buttonText, GUILayout.Width(width), GUILayout.Height(30f)))
                {
                    SortPrefab();
                }
                if (GUILayout.Button("Sort Sprite", buttonText, GUILayout.Height(30f)))
                {
                    SortSprite();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal("box");
                {
                    imageScroll = EditorGUILayout.BeginScrollView(imageScroll, GUILayout.Width(width));
                    for (int i = 0; i < prefabs.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.ObjectField(prefabs[i].listGameObject, typeof(GameObject), true);
                        EditorGUILayout.ObjectField(prefabs[i].listSprite, typeof(Sprite), true);
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndScrollView();

                    spriteScroll = EditorGUILayout.BeginScrollView(spriteScroll);
                    for (int i = 0; i < sprites.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        //EditorGUILayout.LabelField(sprites[i].listPath, EditorStyles.boldLabel);
                        EditorGUILayout.ObjectField(sprites[i].listSprite, typeof(Sprite), true);
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndHorizontal();
            }
            GUI.color = Color.cyan;
            EditorGUILayout.BeginHorizontal("box");
            if (GUILayout.Button("Find Sprite", buttonText, GUILayout.Width(width), GUILayout.Height(50f)))
            {
                FindFiles();
            }
            if (GUILayout.Button("Find Unused", buttonText, GUILayout.Height(50f)))
            {
                UnusedList();
            }
            EditorGUILayout.EndHorizontal();
        }

        void Tutorial()
        {
            buttonText = new(GUI.skin.button)
            {
                fontSize = 13,
                normal = { textColor = Color.yellow },
                alignment = TextAnchor.MiddleCenter
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
                GUILayout.Label(" 1. 프리팹과 스프라이트 폴더를 Resource List에 추가", guiText);
                GUILayout.Label(" 2. Find Sprite 클릭", guiText);
                GUILayout.Label(" 3. Find Unused 클릭", guiText);
                GUILayout.Label(" 4. 스프라이트 클릭 - 추적", guiText);

                GUILayout.Space(10f);
            }
            EditorGUILayout.EndVertical();
        }

        void FindFiles()
        {
            if (ResourceList.Count == 0)
            {
                //props.Clear();
                //materials.Clear();
                Debug.LogError("DataFolders 폴더 필요");
                return;
            }
            string[] paths = new string[ResourceList.Count];
            for (int i = 0; i < ResourceList.Count; i++)
            {
                paths[i] = AssetDatabase.GetAssetPath(ResourceList[i]);
                Debug.LogWarning("File paths : " + paths[i]);
            }
            FindImage(paths);
            SetSprite(paths);
        }

        public void FindImage(string[] _paths)
        {
            prefabs.Clear();
            string[] assets = AssetDatabase.FindAssets("t:Prefab", _paths);
            // 데이터 추가
            for (int i = 0; i < assets.Length; i++)
            {
                var prefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[i]), typeof(GameObject)) as GameObject;
                //string assetPath = AssetDatabase.GetAssetPath(prefab);
                Image[] findImage = prefab.GetComponentsInChildren<Image>();
                //TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                for (int j = 0; j < findImage.Length; j++)
                {
                    Sprite sprite = findImage[j].sprite;
                    PrefabList list = new PrefabList
                    {
                        listGameObject = prefab,
                        listSprite = sprite,
                    };
                    prefabs.Add(list);
                }
            }
            Debug.LogWarning($"{_paths.Length} : {assets.Length} >> {prefabs.Count}");
        }

        void SetSprite(string[] _paths)
        {
            sprites.Clear();
            string[] assets = AssetDatabase.FindAssets("t:Sprite", _paths);
            // 데이터 추가
            for (int i = 0; i < assets.Length; i++)
            {
                Sprite sprite = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[i]), typeof(Sprite)) as Sprite;
                //string assetPath = AssetDatabase.GetAssetPath(sprite);
                //string spriteName = sprite.name;
                SpriteList list = new SpriteList
                {
                    //listPath = spriteName,
                    listSprite = sprite,
                };
                sprites.Add(list);
            }
        }

        void UnusedList()
        {
            List<SpriteList> temp = new List<SpriteList>(sprites);
            for (int i = 0; i < prefabs.Count; i++)
            {
                SpriteList spriteList = TrySameFile(prefabs[i].listSprite);
                if (spriteList.listSprite != null)
                {
                    temp.Remove(spriteList);
                }
            }
            sprites = temp;
        }

        SpriteList TrySameFile(Sprite _sprite)
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                if (_sprite == sprites[i].listSprite)
                    return sprites[i];
            }
            return default;
        }

        void SortPrefab()
        {
            sortPrefab = !sortPrefab;
            prefabs.Sort(delegate (PrefabList a, PrefabList b)
            {
                if (sortPrefab == true)
                    return a.listGameObject.name.CompareTo(b.listGameObject.name);
                else
                    return b.listGameObject.name.CompareTo(a.listGameObject.name);
            });
        }

        void SortSprite()
        {
            sortSprite = !sortSprite;
            sprites.Sort(delegate (SpriteList a, SpriteList b)
            {
                if (sortSprite == true)
                    return a.listSprite.name.CompareTo(b.listSprite.name);
                else
                    return b.listSprite.name.CompareTo(a.listSprite.name);
            });
        }

        SerializedObject targetObject;
        [SerializeField] private List<Object> ResourceList = new List<Object>();

        private void OnEnable()
        {
            targetObject = new SerializedObject(this);
            ResourceList.Clear();
        }
    }
}
#endif