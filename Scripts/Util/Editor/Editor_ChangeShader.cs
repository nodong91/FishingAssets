using UnityEditor;
using UnityEngine;

//#if UNITY_EDITOR
//using UnityEditor;
//#endif
namespace P01.Editor
{
    public class Editor_ChangeShader : EditorWindow
    {
        [MenuItem("Graphics Tool/07. ChangeShader")]
        public static void OpenWindow()
        {
            Editor_ChangeShader window = EditorWindow.GetWindow<Editor_ChangeShader>("Editor_ChangeShader");
            window.minSize = new Vector2(500f, 200f);
            window.Show();
        }
        Material[] mats;
        Shader changeShader;
        Vector2 scrollPosition;
        void OnGUI()
        {
            float setWidth = DivideWigth(2f);
            float setHight = 20;
            GUIStyle fontStyle = new GUIStyle(GUI.skin.button);
            fontStyle.fontSize = 15;
            fontStyle.normal.textColor = Color.yellow;

            SetTutorial();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            object[] selectionObject = Selection.objects;
            mats = new Material[selectionObject.Length];
            int findMat = 0;
            for (int i = 0; i < selectionObject.Length; i++)
            {
                Material mat = selectionObject[i] as Material;
                mats[i] = mat;

                if (mat == null)
                    continue;
                findMat++;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField("", mat, typeof(Material), true, GUILayout.Width(setWidth), GUILayout.Height(setHight));

                Shader matShader = mat == null ? null : mat.shader;
                EditorGUILayout.ObjectField("", matShader, typeof(Shader), true, GUILayout.Height(setHight));

                EditorGUILayout.EndHorizontal();
            }

            if (findMat == 0)
            {
                GUIStyle warnningStyle = new()
                {
                    fontSize = 15,
                    normal = { textColor = Color.red },
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold,
                };
                GUILayout.Label("매터리얼 선택 필요", warnningStyle);
            }

            EditorGUILayout.EndScrollView();

            changeShader = EditorGUILayout.ObjectField("Change Shader", changeShader, typeof(Shader), true, GUILayout.Height(setHight)) as Shader;

            if (GUILayout.Button("Change", fontStyle, GUILayout.Height(35)))
            {
                //CheckShaderKeywordState();

                for (int i = 0; i < mats.Length; i++)
                {
                    ChangeTest(mats[i]);
                    EditorUtility.SetDirty(mats[i]);
                }
            }
        }

        float DivideWigth(float _value)
        {
            float setWidth = (position.width - 5f * (_value - 1f)) / _value;
            return setWidth;
        }
        bool tutorialToggle;
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
                GUILayout.Label(" 1. 변경이 필요한 모든 매터리얼 선택", guiText);
                GUILayout.Label(" 2. Change Shader 에 변경할 쉐이더 추가", guiText);
                GUILayout.Label(" 3. Change 버튼 클릭", guiText);

                GUILayout.Space(10f);
            }
            EditorGUILayout.EndVertical();
        }

        public class TempProperty
        {
            public Color _ShadowColor;
            public Texture _MainTexture;
            public Texture _SubTexture;
            public Texture _Splat_Main;
            public float _MatCap_Channel;
            public float _MatCap_Value;
            public float _OcclusionChannel;
            public Texture _Splat_Sub;
            public float _MatCap2_Channel;
            public float _MatCap2_Value;
            public float _Specular_Strength;
            public float _Rimlight_Strangth;
            public Color _EmissionColor;
        }

        void ChangeTest(Material _mat)
        {
            if (_mat.shader == changeShader)
                return;

            TempProperty tempProperty = SetData(_mat);
            _mat.shader = changeShader;
            LoadData(_mat, tempProperty);
        }

        TempProperty SetData(Material _mat)
        {
            TempProperty tempProperty = new TempProperty();
            string id = "Color_848ca65834b34db4bdde3dee4c8883d3";
            tempProperty._ShadowColor = _mat.GetColor(id);

            id = "Texture2D_95d61c5498a844db9381fa1d60718dc0";
            tempProperty._MainTexture = _mat.GetTexture(id);

            id = "Texture2D_c55c60fad6a14c61975c2bb1e20422bd";
            tempProperty._SubTexture = _mat.GetTexture(id);

            id = "Texture2D_c2a40dc56b3a45da85e781a3382f8161";
            tempProperty._Splat_Main = _mat.GetTexture(id);

            id = "Vector1_1";
            tempProperty._MatCap_Channel = _mat.GetFloat(id);

            id = "Vector1_1071188b9b934b89b52b5508caafe7a5";
            tempProperty._MatCap_Value = _mat.GetFloat(id);

            id = "Vector1_2";
            tempProperty._OcclusionChannel = _mat.GetFloat(id);

            id = "Texture2D_c2a40dc56b3a45da85e781a3382f8161_1";
            tempProperty._Splat_Sub = _mat.GetTexture(id);

            id = "Vector1_3";
            tempProperty._MatCap2_Channel = _mat.GetFloat(id);

            id = "Vector1_4";
            tempProperty._MatCap2_Value = _mat.GetFloat(id);

            id = "Vector1_33e6c7c6eed84512822471e65837d802";
            tempProperty._Specular_Strength = _mat.GetFloat(id);

            id = "Vector1_0dedfdaa4635448eb0bafbb3a87892ab";
            tempProperty._Rimlight_Strangth = _mat.GetFloat(id);

            id = "Color_cdf62f4e73e5446485ad67e233a6fab2";
            tempProperty._EmissionColor = _mat.GetColor(id);
            return tempProperty;
        }

        void LoadData(Material _mat, TempProperty _tempProperty)
        {
            string id = "_ShadowColor";
            _mat.SetColor(id, _tempProperty._ShadowColor);

            id = "_MainTexture";
            _mat.SetTexture(id, _tempProperty._MainTexture);

            id = "_SubTexture";
            _mat.SetTexture(id, _tempProperty._SubTexture);

            id = "_Splat_Main";
            _mat.SetTexture(id, _tempProperty._Splat_Main);

            id = "_MatCap_Channel";
            _mat.SetFloat(id, _tempProperty._MatCap_Channel);

            id = "_MatCap_Value";
            _mat.SetFloat(id, _tempProperty._MatCap_Value);

            id = "_OcclusionChannel";
            _mat.SetFloat(id, _tempProperty._OcclusionChannel);

            id = "_Splat_Sub";
            _mat.SetTexture(id, _tempProperty._Splat_Sub);

            id = "_MatCap2_Channel";
            _mat.SetFloat(id, _tempProperty._MatCap2_Channel);

            id = "_MatCap2_Value";
            _mat.SetFloat(id, _tempProperty._MatCap2_Value);

            id = "_Specular_Strength";
            _mat.SetFloat(id, _tempProperty._Specular_Strength);

            id = "_Rimlight_Strangth";
            _mat.SetFloat(id, _tempProperty._Rimlight_Strangth);

            id = "_EmissionColor";
            _mat.SetColor(id, _tempProperty._EmissionColor);
        }

        //void CheckShaderKeywordState()
        //{
        //    // Get the instance of the Shader class that the material uses
        //    var shader = material.shader;

        //    // Get all the local keywords that affect the Shader
        //    var keywordSpace = shader.keywordSpace;

        //    // Iterate over the local keywords
        //    foreach (var localKeyword in keywordSpace.keywords)
        //    {
        //        // If the local keyword is overridable (i.e., it was declared with a global scope),
        //        // and a global keyword with the same name exists and is enabled,
        //        // then Unity uses the global keyword state
        //        if (localKeyword.isOverridable && Shader.IsKeywordEnabled(localKeyword.name))
        //        {
        //            Debug.Log("Local keyword with name of " + localKeyword.name + " is overridden by a global keyword, and is enabled");
        //        }
        //        // Otherwise, Unity uses the local keyword state
        //        else
        //        {
        //            var state = material.IsKeywordEnabled(localKeyword) ? "enabled" : "disabled";
        //            Debug.Log("Local keyword with name of " + localKeyword.name + " is " + state);
        //        }
        //    }
        //}
    }
}