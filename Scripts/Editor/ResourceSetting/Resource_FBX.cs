using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace P01
{
    public class Resource_FBX : Resource_Material
    {
        public class FBXList
        {
            public ModelImporter importer;
            public GameObject data;
        }
        public List<FBXList> fbxs = new List<FBXList>();
        public Vector2 scrollFBX;
        bool sortFBX;

        //bool isReadable, constraints, animation;
        //ModelImporterMaterialImportMode materialMode;
        //ModelImporterAnimationType animMode;

        public void DisplayFBX()
        {
            if (fbxs.Count <= 0)
                return;

            GUIStyle centerText = new(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white },
                fontSize = 15
            };

            GUIStyle buttonText = new(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.yellow },
                fontSize = 15,
                fontStyle = FontStyle.Bold,
            };

            float setWidth = SetButtonWidth(6f);
            float setHight = 30;
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.BeginHorizontal("box");
                if (SetButton("Read/Write", setWidth, setHight))
                {
                    SortFBX("Read/Write");
                }
                if (SetButton("ImportConstraints", setWidth, setHight))
                {
                    SortFBX("ImportConstraints");
                }
                if (SetButton("ImportAnimation", setWidth, setHight))
                {
                    SortFBX("ImportAnimation");
                }
                if (SetButton("MaterialImportMode", setWidth, setHight))
                {
                    SortFBX("MaterialImportMode");
                }
                if (SetButton("AnimationType", setHight))
                {
                    SortFBX("AnimationType");
                }
                if (SetButton("FBX", setWidth, setHight))
                {
                    SortFBX("FBX");
                }
                EditorGUILayout.EndHorizontal();

                setHight = 20f;
                buttonText.fontSize = 13;
                buttonText.fontStyle = FontStyle.Normal;

                scrollFBX = EditorGUILayout.BeginScrollView(scrollFBX);
                for (int i = 0; i < fbxs.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    fbxs[i].data = EditorGUILayout.ObjectField("", fbxs[i].data, typeof(GameObject), false, GUILayout.Width(setWidth), GUILayout.Height(setHight)) as GameObject;

                    buttonText.normal.textColor = fbxs[i].importer.importConstraints ? Color.green : Color.red;
                    if (GUILayout.Button(fbxs[i].importer.importConstraints.ToString(), buttonText, GUILayout.Width(setWidth), GUILayout.Height(setHight)))
                    {
                        fbxs[i].importer.importConstraints = !fbxs[i].importer.importConstraints;
                    }

                    buttonText.normal.textColor = fbxs[i].importer.importAnimation ? Color.green : Color.red;
                    if (GUILayout.Button(fbxs[i].importer.importAnimation.ToString(), buttonText, GUILayout.Width(setWidth), GUILayout.Height(setHight)))
                    {
                        fbxs[i].importer.importAnimation = !fbxs[i].importer.importAnimation;
                    }

                    buttonText.normal.textColor = fbxs[i].importer.materialImportMode == ModelImporterMaterialImportMode.None ? Color.red : Color.green;
                    fbxs[i].importer.materialImportMode = (ModelImporterMaterialImportMode)EditorGUILayout.EnumPopup
                        ("", fbxs[i].importer.materialImportMode, buttonText, GUILayout.Width(setWidth), GUILayout.Height(setHight));


                    //buttonText.normal.textColor = fbxs[i].importer.importAnimation ? Color.green : Color.red;
                    //if (GUILayout.Button(fbxs[i].importer.importAnimation.ToString(), buttonText,  GUILayout.Height(setHight)))
                    //{
                    //    fbxs[i].importer.importAnimation = !fbxs[i].importer.importAnimation;
                    //}
                    buttonText.normal.textColor = fbxs[i].importer.animationType == ModelImporterAnimationType.None ? Color.red : Color.green;
                    fbxs[i].importer.animationType = (ModelImporterAnimationType)EditorGUILayout.EnumPopup
                        ("", fbxs[i].importer.animationType, buttonText, GUILayout.Width(setWidth), GUILayout.Height(setHight));

                    buttonText.normal.textColor = fbxs[i].importer.isReadable ? Color.green : Color.red;
                    if (GUILayout.Button(fbxs[i].importer.isReadable.ToString(), buttonText,  GUILayout.Height(setHight)))
                    {
                        fbxs[i].importer.isReadable = !fbxs[i].importer.isReadable;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
                SetMaterialImportMode();
            }
            EditorGUILayout.EndVertical();
            //if (GUILayout.Button("Save Project", buttonText, GUILayout.Height(50f)))
            //{
            //    AssetDatabase.Refresh();
            //    //for(int i = 0; i< fbxs.Count; i++)
            //    //{
            //    //    EditorUtility.SetDirty(fbxs[i].data);
            //    //}
            //    AssetDatabase.SaveAssets();
            //}
        }

        void SetMaterialImportMode()
        {
            GUIStyle buttonText = new(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.yellow },
                fontSize = 15,
                fontStyle = FontStyle.Bold,
            };

            if (GUILayout.Button(new GUIContent("가장 위 내용으로 모두 변경"), buttonText, GUILayout.Height(30f)))
            {
                ModelImporter importer = fbxs[0].importer;
                for (int i = 0; i < fbxs.Count; i++)
                {
                    fbxs[i].importer.isReadable = importer.isReadable;
                    fbxs[i].importer.importConstraints = importer.importConstraints;
                    fbxs[i].importer.importAnimation = importer.importAnimation;
                    fbxs[i].importer.materialImportMode = importer.materialImportMode;
                    fbxs[i].importer.animationType = importer.animationType;
                }
            }
        }

        public void FindFBX(string[] _paths)
        {
            fbxs.Clear();
            string[] assets = AssetDatabase.FindAssets("t: Model", _paths);
            // 데이터 추가
            for (int i = 0; i < assets.Length; i++)
            {
                var data = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[i]), typeof(GameObject)) as GameObject;
                var sdata = new SerializedObject(data);
                ModelImporter importer = AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(assets[i])) as ModelImporter;
                if (importer != null)
                {
                    FBXList fbxList = new FBXList
                    {
                        importer = importer,
                        data = data,
                    };
                    fbxs.Add(fbxList);
                }
            }
        }

        void SortFBX(string _sortType)
        {
            sortFBX = !sortFBX;
            switch (_sortType)
            {
                case "FBX":
                    fbxs.Sort(delegate (FBXList a, FBXList b)
                    {
                        if (sortFBX == true)
                            return a.data.name.CompareTo(b.data.name);
                        else
                            return b.data.name.CompareTo(a.data.name);
                    });
                    break;

                case "Read/Write":
                    fbxs.Sort(delegate (FBXList a, FBXList b)
                    {
                        if (a.importer.isReadable == b.importer.isReadable)
                            return a.data.name.CompareTo(b.data.name);

                        if (sortFBX == true)
                            return a.importer.isReadable.CompareTo(b.importer.isReadable);
                        else
                            return b.importer.isReadable.CompareTo(a.importer.isReadable);
                    });
                    break;

                case "ImportConstraints":
                    fbxs.Sort(delegate (FBXList a, FBXList b)
                    {
                        if (a.importer.importConstraints == b.importer.importConstraints)
                            return a.data.name.CompareTo(b.data.name);

                        if (sortFBX == true)
                            return a.importer.importConstraints.CompareTo(b.importer.importConstraints);
                        else
                            return b.importer.importConstraints.CompareTo(a.importer.importConstraints);
                    });
                    break;

                case "ImportAnimation":
                    fbxs.Sort(delegate (FBXList a, FBXList b)
                    {
                        if (a.importer.importAnimation == b.importer.importAnimation)
                            return a.data.name.CompareTo(b.data.name);

                        if (sortFBX == true)
                            return a.importer.importAnimation.CompareTo(b.importer.importAnimation);
                        else
                            return b.importer.importAnimation.CompareTo(a.importer.importAnimation);
                    });
                    break;

                case "MaterialImportMode":
                    fbxs.Sort(delegate (FBXList a, FBXList b)
                    {
                        if (a.importer.materialImportMode == b.importer.materialImportMode)
                            return a.data.name.CompareTo(b.data.name);

                        if (sortFBX == true)
                            return a.importer.materialImportMode.CompareTo(b.importer.materialImportMode);
                        else
                            return b.importer.materialImportMode.CompareTo(a.importer.materialImportMode);
                    });
                    break;

                case "AnimationType":
                    fbxs.Sort(delegate (FBXList a, FBXList b)
                    {
                        if (a.importer.animationType == b.importer.animationType)
                            return a.data.name.CompareTo(b.data.name);

                        if (sortFBX == true)
                            return a.importer.animationType.CompareTo(b.importer.animationType);
                        else
                            return b.importer.animationType.CompareTo(a.importer.animationType);
                    });
                    break;
            }
        }
    }
}
#endif