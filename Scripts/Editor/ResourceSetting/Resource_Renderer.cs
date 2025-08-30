
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace P01
{
    public class Resource_Renderer : Resource_FBX
    {
        public class RendererList
        {
            public string path;
            public GameObject gameObject;
            public Renderer renderer;
        }
        public List<RendererList> renderers = new List<RendererList>();
        public Vector2 scrollRenderer;
        bool sortRenderer;

        public void DisplayRenderer()
        {
            if (renderers.Count <= 0)
                return;

            float setWidth = SetButtonWidth(6f);
            float setHight = 30;

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
                alignment = TextAnchor.MiddleCenter
            };

            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.BeginHorizontal("box");
                {
                    if (SetButton("Renderer", setWidth, setHight))
                    {
                        SortRenderer("Renderer");
                    }

                    if (SetButton("Type", setWidth, setHight))
                    {
                        SortRenderer("Type");
                    }

                    if (SetButton("Prefab", setWidth, setHight))
                    {
                        SortRenderer("Prefab");
                    }

                    if (SetButton("Cast Shadow", setWidth, setHight))
                    {
                        SortRenderer("Cast Shadow");
                    }

                    if (SetButton("Static Shadow", setWidth, setHight))
                    {
                        SortRenderer("Static Shadow");
                    }

                    if (SetButton("Illumination",  setHight))
                    {
                        SortRenderer("Illumination");
                    }
                }
                EditorGUILayout.EndHorizontal();

                setHight = 20;
                buttonText.fontStyle = FontStyle.Normal;
                scrollRenderer = EditorGUILayout.BeginScrollView(scrollRenderer);
                for (int i = 0; i < renderers.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        string textureName = System.IO.Path.GetFileName(renderers[i].path);
                        EditorGUILayout.ObjectField(renderers[i].renderer, typeof(GameObject), true, GUILayout.Width(setWidth));
                        buttonText.normal.textColor = Color.white;
                        //renderers[i].renderer.GetType()
                        string typeName = renderers[i].renderer.GetType().ToString();
                        GUILayout.Label(typeName.Split('.')[1], centerText, GUILayout.Width(setWidth), GUILayout.Height(setHight));
                        EditorGUILayout.ObjectField(renderers[i].gameObject, typeof(GameObject), true, GUILayout.Width(setWidth));

                        buttonText.normal.textColor = renderers[i].renderer.shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.On ? Color.green : Color.red;
                        if (GUILayout.Button(renderers[i].renderer.shadowCastingMode.ToString(), buttonText, GUILayout.Width(setWidth), GUILayout.Height(setHight)))
                        {
                            if (renderers[i].renderer.shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.On)
                            {
                                renderers[i].renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                            }
                            else if (renderers[i].renderer.shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.Off)
                            {
                                renderers[i].renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                            }
                            renderers[i].renderer.shadowCastingMode = renderers[i].renderer.shadowCastingMode;
                        }
                        buttonText.normal.textColor = renderers[i].renderer.staticShadowCaster == true ? Color.green : Color.red;
                        if (GUILayout.Button(renderers[i].renderer.staticShadowCaster.ToString(), buttonText, GUILayout.Width(setWidth), GUILayout.Height(setHight)))
                        {
                            renderers[i].renderer.staticShadowCaster = !renderers[i].renderer.staticShadowCaster;
                        }

                        var flags = (StaticEditorFlags)GameObjectUtility.GetStaticEditorFlags(renderers[i].renderer.gameObject);
                        bool illumination = flags == StaticEditorFlags.ContributeGI;
                        buttonText.normal.textColor = illumination == true ? Color.green : Color.red;
                        if (GUILayout.Button(flags.ToString(), buttonText,  GUILayout.Height(setHight)))
                        {
                            if (flags == StaticEditorFlags.ContributeGI)
                                flags = 0;
                            else
                                flags = StaticEditorFlags.ContributeGI;
                            GameObjectUtility.SetStaticEditorFlags(renderers[i].renderer.gameObject, flags);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }

        public void FindPrefab(string[] _paths)
        {
            renderers.Clear();
            string[] assets = AssetDatabase.FindAssets("t:Prefab", _paths);
            // 데이터 추가
            for (int i = 0; i < assets.Length; i++)
            {
                var prefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[i]), typeof(GameObject)) as GameObject;
                string assetPath = AssetDatabase.GetAssetPath(prefab);
                Renderer[] findRenderer = prefab.GetComponentsInChildren<Renderer>();
                //TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                for (int j = 0; j < findRenderer.Length; j++)
                {
                    RendererList list = new RendererList
                    {
                        path = assetPath,
                        gameObject = prefab,
                        renderer = findRenderer[j],
                    };
                    renderers.Add(list);
                }
            }
            Debug.LogWarning($"{_paths.Length} : {assets.Length} >> {renderers.Count}");
        }

        void SortRenderer(string _sortType)
        {
            sortRenderer = !sortRenderer;
            switch (_sortType)
            {
                case "Renderer":
                    renderers.Sort(delegate (RendererList a, RendererList b)
                    {
                        if (sortRenderer == true)
                            return a.renderer.name.CompareTo(b.renderer.name);
                        else
                            return b.renderer.name.CompareTo(a.renderer.name);
                    });
                    break;

                case "Type":
                    renderers.Sort(delegate (RendererList a, RendererList b)
                    {
                        string typeA = a.renderer.GetType().ToString().Split('.')[1];
                        string typeB = b.renderer.GetType().ToString().Split('.')[1];

                        if (typeA == typeB)
                            return a.renderer.name.CompareTo(b.renderer.name);

                        if (sortRenderer == true)
                        {
                            return typeA.CompareTo(typeB);
                        }
                        else
                        {
                            return typeB.CompareTo(typeA);
                        }
                    });
                    break;

                case "Prefab":
                    renderers.Sort(delegate (RendererList a, RendererList b)
                    {
                        if (sortRenderer == true)
                        {
                            return a.gameObject.name.CompareTo(b.gameObject.name);
                        }
                        else
                        {
                            return b.gameObject.name.CompareTo(a.gameObject.name);
                        }
                    });
                    break;

                case "Cast Shadow":

                    renderers.Sort(delegate (RendererList a, RendererList b)
                    {
                        if (a.renderer.shadowCastingMode == b.renderer.shadowCastingMode)
                            return a.renderer.name.CompareTo(b.renderer.name);

                        else if (sortRenderer == true)
                        {
                            return a.renderer.shadowCastingMode.CompareTo(b.renderer.shadowCastingMode);
                        }
                        else
                        {
                            return b.renderer.shadowCastingMode.CompareTo(a.renderer.shadowCastingMode);
                        }
                    });
                    break;

                case "Static Shadow":
                    renderers.Sort(delegate (RendererList a, RendererList b)
                    {
                        if (a.renderer.staticShadowCaster == b.renderer.staticShadowCaster)
                            return a.renderer.name.CompareTo(b.renderer.name);

                        else if (sortRenderer == true)
                        {
                            return a.renderer.staticShadowCaster.CompareTo(b.renderer.staticShadowCaster);
                        }
                        else
                        {
                            return b.renderer.staticShadowCaster.CompareTo(a.renderer.staticShadowCaster);
                        }
                    });
                    break;

                case "Illumination":
                    renderers.Sort(delegate (RendererList a, RendererList b)
                    {
                        var flagsA = (StaticEditorFlags)GameObjectUtility.GetStaticEditorFlags(a.renderer.gameObject);
                        var flagsB = (StaticEditorFlags)GameObjectUtility.GetStaticEditorFlags(b.renderer.gameObject);
                        //bool illumination = flags == StaticEditorFlags.ContributeGI;

                        if (flagsA == flagsB)
                            return a.renderer.name.CompareTo(b.renderer.name);

                        else if (sortRenderer == true)
                        {
                            return flagsA.CompareTo(flagsB);
                        }
                        else
                        {
                            return flagsB.CompareTo(flagsA);
                        }
                    });
                    break;
            }
        }
    }
}
#endif