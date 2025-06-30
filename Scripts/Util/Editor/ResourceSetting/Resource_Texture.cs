using UnityEngine;
using System.Collections.Generic;
using static P01.Resource_Texture;
using Codice.Client.BaseCommands;



#if UNITY_EDITOR
using UnityEditor;

namespace P01
{
    public class Resource_Texture : EditorWindow
    {
        public class TextureList
        {
            public string path;
            public TextureImporter importer;
            public Texture data;
            public int textureSize;
        }
        public List<TextureList> textures = new List<TextureList>();
        Vector2 scrollTexture;
        bool sortTexture;

        public void DisplayTexture()
        {
            if (textures?.Count <= 0)
                return;

            float setWidth = SetButtonWidth(7f);
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
                //fontSize = 15,
                //fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleCenter
            };

            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.BeginHorizontal("box");
                {
                    if (SetButton("Texture", setWidth, setHight))
                    {
                        SortTexture("Texture");
                    }

                    if (SetButton("Type", setWidth, setHight))
                    {
                        SortTexture("Type");
                    }

                    if (SetButton("Mipmap", setWidth, setHight))
                    {
                        SortTexture("Mipmap");
                    }

                    if (SetButton("WrapMode", setWidth, setHight))
                    {
                        SortTexture("WrapMode");
                    }

                    if (SetButton("Read/Write", setWidth, setHight))
                    {
                        SortTexture("Read/Write");
                    }

                    if (SetButton("Android", setWidth, setHight))
                    {
                        SortTexture("Android");
                    }

                    if (SetButton("iOS", setHight))
                    {
                        SortTexture("iOS");
                    }

                    //if (SetButton("MaxSize", setHight))
                    //{
                    //    //SortTexture("iOS");
                    //}
                }
                EditorGUILayout.EndHorizontal();

                setHight = 20;
                buttonText.fontStyle = FontStyle.Normal;
                scrollTexture = EditorGUILayout.BeginScrollView(scrollTexture);
                for (int i = 0; i < textures.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        string textureName = System.IO.Path.GetFileName(textures[i].path);
                        EditorGUILayout.ObjectField(textures[i].data, typeof(Texture), true, GUILayout.Width(setWidth));
                        buttonText.normal.textColor = Color.white;
                        GUILayout.Label(textureName.Split('.')[1], centerText, GUILayout.Width(setWidth), GUILayout.Height(setHight));

                        buttonText.normal.textColor = textures[i].importer.mipmapEnabled ? Color.green : Color.red;
                        if (GUILayout.Button(textures[i].importer.mipmapEnabled.ToString(), buttonText, GUILayout.Width(setWidth), GUILayout.Height(setHight)))
                        {
                            textures[i].importer.mipmapEnabled = !textures[i].importer.mipmapEnabled;
                        }
                        buttonText.normal.textColor = textures[i].importer.wrapMode == TextureWrapMode.Repeat ? Color.cyan : Color.white;
                        textures[i].importer.wrapMode = (TextureWrapMode)EditorGUILayout.EnumPopup(textures[i].importer.wrapMode, buttonText, GUILayout.Width(setWidth), GUILayout.Height(setHight));

                        buttonText.normal.textColor = textures[i].importer.isReadable ? Color.green : Color.red;
                        if (GUILayout.Button(textures[i].importer.isReadable.ToString(), buttonText, GUILayout.Width(setWidth), GUILayout.Height(setHight)))
                        {
                            textures[i].importer.isReadable = !textures[i].importer.isReadable;
                        }

                        var androidOverrides = textures[i].importer.GetPlatformTextureSettings("Android");
                        buttonText.normal.textColor = androidOverrides.overridden ? Color.green : Color.red;
                        string format = androidOverrides.overridden ? androidOverrides.format.ToString() : "None";
                        string size = $"({androidOverrides.maxTextureSize})";
                        if (GUILayout.Button($"{format} {size}", buttonText, GUILayout.Width(setWidth), GUILayout.Height(setHight)))
                        {
                            androidOverrides.overridden = !androidOverrides.overridden;
                            androidOverrides.format = TextureImporterFormat.ETC2_RGB4;
                            textures[i].importer.SetPlatformTextureSettings(androidOverrides);
                        }
                        //buttonText.normal.textColor = fbxs[i].importer.animationType == ModelImporterAnimationType.None ? Color.red : Color.green;
                        //fbxs[i].importer.animationType = (ModelImporterAnimationType)EditorGUILayout.EnumPopup
                        //    ("", fbxs[i].importer.animationType, buttonText, GUILayout.Width(setWidth), GUILayout.Height(setHight));


                        var iOSOverrides = textures[i].importer.GetPlatformTextureSettings("iOS");
                        buttonText.normal.textColor = iOSOverrides.overridden ? Color.green : Color.red;
                        format = iOSOverrides.overridden ? iOSOverrides.format.ToString() : "None";
                        size = $"({iOSOverrides.maxTextureSize})";
                        if (GUILayout.Button($"{format} {size}", buttonText, GUILayout.Height(setHight)))
                        {
                            iOSOverrides.overridden = !iOSOverrides.overridden;
                            iOSOverrides.format = TextureImporterFormat.ASTC_6x6;
                            textures[i].importer.SetPlatformTextureSettings(iOSOverrides);
                        }

                        //TextureImporterPlatformSettings setting = isAndroid == true ? androidOverrides : iOSOverrides;
                        //buttonText.normal.textColor = Color.white;
                        //string buttonName = isAndroid == true ? "Android" : "IOS";
                        //buttonName = $"{setting.maxTextureSize} ({buttonName})";
                        //if (GUILayout.Button(buttonName, buttonText, GUILayout.Height(setHight)))
                        //{
                        //    isAndroid = !isAndroid;
                        //    setting.maxTextureSize = (int)textureSize;
                        //}
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();

            //buttonText.fontStyle = FontStyle.Bold;
            //buttonText.normal.textColor = Color.yellow;
            //if (textures.Count > 0 && GUILayout.Button("Complate", buttonText, GUILayout.Height(30f)))
            //{
            //    for (int i = 0; i < textures.Count; i++)
            //    {
            //        textures.Remove(textures[i]);
            //        //EditorGUILayout.EndHorizontal();
            //    }
            //}
        }
        bool isAndroid;
        public enum TextureSize
        {
            _32 = 32,
            _64 = 64,
            _128 = 128,
            _256 = 256,
            _512 = 512,
            _1024 = 1024,
            _2048 = 2048,
            _4096 = 4096
        }
        public TextureSize textureSize;
        public void FindTexture(string[] _paths)
        {
            textures.Clear();
            string[] assets = AssetDatabase.FindAssets("t:Texture", _paths);
            // 데이터 추가
            for (int i = 0; i < assets.Length; i++)
            {
                var texture = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[i]), typeof(Texture)) as Texture;
                string assetPath = AssetDatabase.GetAssetPath(texture);
                TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (textureImporter != null)
                {
                    TextureList list = new TextureList
                    {
                        path = assetPath,
                        data = texture,
                        importer = textureImporter,
                        textureSize = textureImporter.GetPlatformTextureSettings("Android").maxTextureSize
                    };
                    textures.Add(list);
                }
            }
        }

        void SortTexture(string _sortType)
        {
            sortTexture = !sortTexture;
            switch (_sortType)
            {
                case "Texture":
                    textures.Sort(delegate (TextureList a, TextureList b)
                    {
                        if (sortTexture == true)
                            return a.data.name.CompareTo(b.data.name);
                        else
                            return b.data.name.CompareTo(a.data.name);
                    });
                    break;

                case "Type":
                    textures.Sort(delegate (TextureList a, TextureList b)
                    {

                        string typeA = a.path.Split('.')[1];
                        string typeB = b.path.Split('.')[1];
                        //string typeB = b.path.Substring(b.path.Length - 3);
                        if (typeA == typeB)
                            return a.data.name.CompareTo(b.data.name);

                        else if (sortTexture == true)
                        {
                            return typeA.CompareTo(typeB);
                        }
                        else
                        {
                            return typeB.CompareTo(typeA);
                        }
                    });
                    break;

                case "Mipmap":
                    textures.Sort(delegate (TextureList a, TextureList b)
                    {
                        if (a.importer.mipmapEnabled == b.importer.mipmapEnabled)
                            return a.data.name.CompareTo(b.data.name);

                        else if (sortTexture == true)
                        {
                            return a.importer.mipmapEnabled.CompareTo(b.importer.mipmapEnabled);
                        }
                        else
                        {
                            return b.importer.mipmapEnabled.CompareTo(a.importer.mipmapEnabled);
                        }
                    });
                    break;

                case "WrapMode":

                    textures.Sort(delegate (TextureList a, TextureList b)
                    {
                        if (a.importer.wrapMode == b.importer.wrapMode)
                            return a.data.name.CompareTo(b.data.name);

                        else if (sortTexture == true)
                        {
                            return a.importer.wrapMode.CompareTo(b.importer.wrapMode);
                        }
                        else
                        {
                            return b.importer.wrapMode.CompareTo(a.importer.wrapMode);
                        }
                    });
                    break;

                case "Read/Write":
                    textures.Sort(delegate (TextureList a, TextureList b)
                    {
                        if (a.importer.isReadable == b.importer.isReadable)
                            return a.data.name.CompareTo(b.data.name);

                        else if (sortTexture == true)
                        {
                            return a.importer.isReadable.CompareTo(b.importer.isReadable);
                        }
                        else
                        {
                            return b.importer.isReadable.CompareTo(a.importer.isReadable);
                        }
                    });
                    break;

                case "Android":
                    textures.Sort(delegate (TextureList a, TextureList b)
                    {
                        var androidOverrides = a.importer.GetPlatformTextureSettings("Android");
                        string A = androidOverrides.overridden ? androidOverrides.format.ToString() : "None";

                        androidOverrides = b.importer.GetPlatformTextureSettings("Android");
                        string B = androidOverrides.overridden ? androidOverrides.format.ToString() : "None";

                        if (A == B)
                            return a.data.name.CompareTo(b.data.name);

                        else if (sortTexture == true)
                        {
                            return A.CompareTo(B);
                        }
                        else
                        {
                            return B.CompareTo(A);
                        }
                    });
                    break;

                case "iOS":
                    textures.Sort(delegate (TextureList a, TextureList b)
                    {
                        var iOSOverrides = a.importer.GetPlatformTextureSettings("iOS");
                        string A = iOSOverrides.overridden ? iOSOverrides.format.ToString() : "None";

                        iOSOverrides = b.importer.GetPlatformTextureSettings("iOS");
                        string B = iOSOverrides.overridden ? iOSOverrides.format.ToString() : "None";

                        if (A == B)
                            return a.data.name.CompareTo(b.data.name);

                        else if (sortTexture == true)
                        {
                            return A.CompareTo(B);
                        }
                        else
                        {
                            return B.CompareTo(A);
                        }
                    });
                    break;
            }
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