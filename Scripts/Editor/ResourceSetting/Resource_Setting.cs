using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace P01.Editor
{
    public class Resource_Setting : Resource_Renderer
    {
        [MenuItem("Graphics Tool/01. Resource_Setting")]
        public static void OpenWindow()
        {
            Resource_Setting window = EditorWindow.GetWindow<Resource_Setting>("Resource_Setting");
            window.minSize = new Vector2(500f, 200f);
            window.Show();
        }

        //public class PropList
        //{
        //    public ModelImporter importer;
        //    public GameObject data;
        //}
        //public List<PropList> props = new List<PropList>();
        //public List<Material> materials = new List<Material>();
        //Vector2 scrollPosition;

        public enum FindType
        {
            Renderer,
            FBX,
            Material,
            Texture
        }
        public FindType findType;
        bool tutorialToggle = false;

        void OnGUI()
        {
            GUIStyle guiText = new()
            {
                fontSize = 13,
                normal = { textColor = Color.yellow },
                alignment = TextAnchor.MiddleLeft
            };

            GUIStyle buttonText = new(GUI.skin.button)
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
                GUILayout.Label(" 1. Type 선택", guiText);
                GUILayout.Label(" 2. 리소스 폴더 추가", guiText);
                GUILayout.Label(" 3. Find 클릭", guiText);
                GUILayout.Label(" 4. 변경할 내용 버튼 클릭", guiText);
                GUILayout.Label(" 5. Save Project 버튼 클릭", guiText);

                GUILayout.Space(10f);
                guiText.normal.textColor = Color.yellow;
                GUILayout.Label(" ※ 이름 버튼 클릭 - 정렬 (재입력 시 역순 정렬)", guiText);
            }
            EditorGUILayout.EndVertical();

            GUIStyle enumText = new(GUI.skin.button)
            {
                fontSize = 15
            };

            EditorGUILayout.BeginVertical("box");
            {
                buttonText.fontSize = 15;
                buttonText.fontStyle = FontStyle.Bold;
                float width = (position.width - 20f) / 4f;

                EditorGUILayout.BeginHorizontal();
                {
                    GUI.color = (findType == FindType.Renderer) ? Color.yellow : Color.white;
                    if (GUILayout.Button("Renderer", buttonText, GUILayout.Width(width), GUILayout.Height(30f)))
                    {
                        findType = FindType.Renderer;
                    }
                    GUI.color = (findType == FindType.FBX) ? Color.cyan : Color.white;
                    if (GUILayout.Button("FBX", buttonText, GUILayout.Width(width), GUILayout.Height(30f)))
                    {
                        findType = FindType.FBX;
                    }
                    GUI.color = (findType == FindType.Material) ? Color.green : Color.white;
                    if (GUILayout.Button("Material", buttonText, GUILayout.Width(width), GUILayout.Height(30f)))
                    {
                        findType = FindType.Material;
                    }
                    GUI.color = (findType == FindType.Texture) ? Color.red : Color.white;
                    if (GUILayout.Button("Texture", buttonText, GUILayout.Width(width), GUILayout.Height(30f)))
                    {
                        findType = FindType.Texture;
                    }
                }
                EditorGUILayout.EndHorizontal();

                GUI.color = Color.white;
                switch (findType)
                {
                    case FindType.Renderer:
                        GUILayout.Label(" ※ Renderer가 포함되어 있는 Prefab", guiText);
                        break;

                    default:
                        break;
                }
                //// ReorderableList
                //if (this.serializedObject != null)
                //{
                //    serializedObject.Update();
                //    strings_ro_list.DoLayoutList();
                //    serializedObject.ApplyModifiedProperties();
                //}
                if (targetObject != null)
                {
                    targetObject.Update();

                    // objectList 는 위에서 선언한 List의 변수명
                    SerializedProperty testProp = targetObject.FindProperty("ResourceList");
                    EditorGUILayout.PropertyField(testProp, new GUIContent(testProp.displayName));

                    targetObject.ApplyModifiedProperties();
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUIUtility.wideMode = true;// 같은 라인에 정렬
            EditorGUILayout.BeginVertical();
            switch (findType)
            {
                case FindType.Renderer:
                    DisplayRenderer();
                    break;

                case FindType.FBX:
                    DisplayFBX();
                    break;

                case FindType.Material:
                    DisplayMaterial();
                    break;

                case FindType.Texture:
                    DisplayTexture();
                    break;
            }
            EditorGUILayout.EndVertical();

            buttonText.fontStyle = FontStyle.Bold;
            buttonText.normal.textColor = Color.yellow;
            EditorGUILayout.BeginHorizontal();
            if (ResourceList.Count > 0)
            {
                if (GUILayout.Button("Save Project", buttonText, GUILayout.Height(50f)))
                {
                    switch (findType)
                    {
                        case FindType.Renderer:
                            for (int i = 0; i < renderers.Count; i++)
                            {
                                EditorUtility.SetDirty(renderers[i].renderer);
                            }
                            break;

                        case FindType.FBX:
                            for (int i = 0; i < fbxs.Count; i++)
                            {
                                EditorUtility.SetDirty(fbxs[i].data);
                                Debug.LogWarning(fbxs[i].data);
                                fbxs[i].importer.SaveAndReimport();
                            }
                            break;

                        case FindType.Material:
                            for (int i = 0; i < materials.Count; i++)
                            {
                                EditorUtility.SetDirty(materials[i]);
                            }
                            break;

                        case FindType.Texture:
                            for (int i = 0; i < textures.Count; i++)
                            {
                                EditorUtility.SetDirty(textures[i].data);
                                textures[i].importer.SaveAndReimport();
                            }
                            break;
                    }
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                }

                if (GUILayout.Button("Find", buttonText, GUILayout.Height(50f)))
                {
                    FindObject();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        void FindObject()
        {
            if (ResourceList.Count == 0)
            {
                //props.Clear();
                materials.Clear();
                Debug.LogError("DataFolders 폴더 필요");
                return;
            }
            string[] paths = new string[ResourceList.Count];
            for (int i = 0; i < ResourceList.Count; i++)
            {
                paths[i] = AssetDatabase.GetAssetPath(ResourceList[i]);
                Debug.LogWarning("File paths : " + paths[i]);
            }

            renderers.Clear();
            fbxs.Clear();
            materials.Clear();
            textures.Clear();

            switch (findType)
            {
                case FindType.Renderer:
                    FindPrefab(paths);
                    break;
                case FindType.FBX:
                    FindFBX(paths);
                    break;

                case FindType.Material:
                    FindMaterial(paths);
                    break;

                case FindType.Texture:
                    FindTexture(paths);
                    break;
            }
        }
        //==============================================================================================================================
        // ReorderableList
        //==============================================================================================================================

        //ReorderableList strings_ro_list;
        //SerializedObject serializedObject;
        //SerializedProperty stringsProperty;
        //MyScriptable scriptableObject;

        SerializedObject targetObject;
        [SerializeField] private List<Object> ResourceList = new List<Object>();

        private void OnEnable()
        {
            //scriptableObject = ScriptableObject.CreateInstance<MyScriptable>();
            //serializedObject = new SerializedObject(scriptableObject);
            //stringsProperty = serializedObject.FindProperty("resources");

            //strings_ro_list = new ReorderableList(serializedObject, stringsProperty, true, true, true, true);
            //strings_ro_list.drawElementCallback = DrawListItems;
            //strings_ro_list.drawHeaderCallback = DrawHeader;

            //ReorderableList.defaultBehaviours.DoAddButton(strings_ro_list);
            targetObject = new SerializedObject(this);
            ResourceList.Clear();
        }

        //void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        //{
        //    //// your GUI code here for list content
        //    //strings_ro_list.elementHeight = 20;
        //    //SerializedProperty element = stringsProperty.GetArrayElementAtIndex(index);
        //    ////SerializedProperty go = element.FindPropertyRelative("gameObject");
        //    ////go.objectReferenceValue = EditorGUI.ObjectField(ReturnRectSize(rect, 0), go.objectReferenceValue, typeof(GameObject), false);

        //    //SerializedProperty obj = element.FindPropertyRelative("obj");
        //    //obj.objectReferenceValue = EditorGUI.ObjectField(ReturnRectSize(rect, 0), obj.objectReferenceValue, typeof(Object), false);

        //    //SerializedProperty myType = element.FindPropertyRelative("testEnum");
        //    //myType.enumValueIndex = (int)(MyScriptableObject.TestEnum)EditorGUI.EnumPopup(ReturnRectSize(rect, 2), "", (MyScriptableObject.TestEnum)myType.enumValueIndex);

        //    //SerializedProperty myIndex = element.FindPropertyRelative("index");
        //    //myIndex.intValue = EditorGUI.IntField(ReturnRectSize(rect, 3), myIndex.intValue);
        //    //Debug.LogWarning(go.objectReferenceValue.name + "     " + index);


        //    if (targetObject != null)
        //    {
        //        targetObject.Update();

        //        // objectList 는 위에서 선언한 List의 변수명
        //        SerializedProperty testProp = targetObject.FindProperty("ResourceList");
        //        EditorGUILayout.PropertyField(testProp, new GUIContent(testProp.displayName));

        //        targetObject.ApplyModifiedProperties();
        //    }
        //}

        //Rect ReturnRectSize(Rect _rect, int _index)
        //{
        //    int count = 1;
        //    int totalValue = (int)(_rect.width / count) - 2;
        //    var Rect = new Rect(_rect)
        //    {
        //        width = totalValue,
        //        x = _rect.x + (totalValue + 2) * _index,// 몇번째인지
        //        height = strings_ro_list.elementHeight
        //    };
        //    return Rect;
        //}

        //void DrawHeader(Rect rect)
        //{
        //    GUIStyle centerText = new GUIStyle(GUI.skin.box)
        //    {
        //        alignment = TextAnchor.MiddleCenter,
        //        normal = { textColor = Color.white },
        //        fontSize = 13
        //    };

        //    // your GUI code here for list header
        //    EditorGUI.LabelField(rect, "Resources", centerText);
        //}
    }
}

//public class MyScriptable : ScriptableObject
//{
//    public List<Resources> resources = new List<Resources>();
//    [System.Serializable]
//    public class Resources
//    {
//        public Object obj;
//    }
//}
#endif