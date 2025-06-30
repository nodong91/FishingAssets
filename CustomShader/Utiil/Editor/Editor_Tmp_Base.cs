using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace P01.Editor
{
    public class Editor_Tmp_Base : ShaderGUI
    {
        public enum DisplayType
        {
            None,
            Face,
            Outline,
            Underlay,
            Light,
            Bevel,
            DebugSetting,
        }
        public DisplayType displayType = DisplayType.None;

        public MaterialProperty FaceColor;
        public MaterialProperty FaceTexture;
        public MaterialProperty Softness;
        public MaterialProperty FaceText_ST;
        public MaterialProperty FaceUVSpeed;
        public static class TmpProperty
        {
            public static readonly GUIContent MainTexText = EditorGUIUtility.TrTextContent("Map", "Albedo(rgb)");
            // 혹은
            public static readonly GUIContent MainTexText2 = new GUIContent("Map", "Albedo(rgb)");
            public const string FaceColor = "_FaceColor";
            public const string FaceTexture = "_FaceTex";
            public const string Softness = "_Softness";
            public const string FaceText_ST = "_FaceTex_ST";
            public const string FaceUVSpeed = "_FaceUVSpeed";

            public const string OutlineMode = "_OutlineMode";
            public const string OutlineWidth = "_IsoPerimeter";
            public const string OutlineColor1 = "_OutlineColor1";
            public const string OutlineTex = "_OutlineTex";
            public const string OutlineOffset1 = "_OutlineOffset1";
            public const string OutlineColor2 = "_OutlineColor2";
            public const string OutlineOffset2 = "_OutlineOffset2";
            public const string OutlineColor3 = "_OutlineColor3";
            public const string OutlineOffset3 = "_OutlineOffset3";

            public const string OutlineTex_ST = "_OutlineTex_ST";
            public const string OutlineUVSpeed = "_OutlineUVSpeed";


            public const string UnderlayColor = "_UnderlayColor";
            public const string UnderlayOffset = "_UnderlayOffset";
            public const string UnderlayDilate = "_UnderlayDilate";
            public const string UnderlaySoftness = "_UnderlaySoftness";

            public const string SpecularColor = "_SpecularColor";
            public const string LightAngle = "_LightAngle";
            public const string SpecularPower = "_SpecularPower";
            public const string Reflectivity = "_Reflectivity";
            public const string Diffuse = "_Diffuse";
            public const string Ambient = "_Ambient";


            public const string BevelType = "_BevelType";
            public const string BevelAmount = "_BevelAmount";
            public const string BevelOffset = "_BevelOffset";
            public const string BevelWidth = "_BevelWidth";
            public const string BevelRoundness = "_BevelRoundness";
            public const string BevelClamp = "_BevelClamp";


            public const string MainTex = "_MainTex";
            public const string GradientScale = "_GradientScale";
            public const string ScaleRatioA = "_ScaleRatioA";
            //public const string TextureWidth = "_TextureWidth";
            //public const string TextureHeight = "_TextureHeight";
            //public const string WeightNormal = "_WeightNormal";
            //public const string WeightBold = "_WeightBold";
        }

        public void SetGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            switch (displayType)
            {
                case DisplayType.Face:
                    SetFace(materialEditor, properties);
                    break;

                case DisplayType.Outline:
                    SetOutline(materialEditor, properties);
                    break;

                case DisplayType.Underlay:
                    SetUnderlay(materialEditor, properties);
                    break;

                case DisplayType.Light:
                    SetLight(materialEditor, properties);
                    break;

                case DisplayType.Bevel:
                    SetBevel(materialEditor, properties);
                    break;

                case DisplayType.DebugSetting:
                    SetDebugSetting(materialEditor, properties);
                    break;
            }
        }

        void SetFace(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            EditorGUILayout.BeginVertical("box");
            FaceColor = FindProperty(TmpProperty.FaceColor, properties);
            materialEditor.ShaderProperty(FaceColor, new GUIContent("FaceColor"));
            FaceTexture = FindProperty(TmpProperty.FaceTexture, properties);
            materialEditor.ShaderProperty(FaceTexture, new GUIContent("FaceTexture"));
            Softness = FindProperty(TmpProperty.Softness, properties);
            Vector4 v_soft = materialEditor.VectorProperty(Softness, "Softness");
            v_soft.x = EditorGUILayout.FloatField(v_soft.x);

            FaceText_ST = FindProperty(TmpProperty.FaceText_ST, properties);
            materialEditor.ShaderProperty(FaceText_ST, new GUIContent("FaceText_ST"));
            FaceUVSpeed = FindProperty(TmpProperty.FaceUVSpeed, properties);
            materialEditor.ShaderProperty(FaceUVSpeed, new GUIContent("FaceUVSpeed"));
            EditorGUILayout.EndVertical();
        }

        public MaterialProperty OutlineMode;
        public MaterialProperty OutlineWidth;
        public MaterialProperty OutlineTex;
        public MaterialProperty OutlineColor1;
        public MaterialProperty OutlineOffset1;
        public MaterialProperty OutlineColor2;
        public MaterialProperty OutlineOffset2;
        public MaterialProperty OutlineColor3;
        public MaterialProperty OutlineOffset3;
        public MaterialProperty OutlineTex_ST;
        public MaterialProperty OutlineUVSpeed;

        void SetOutline(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            EditorGUILayout.BeginVertical("box");
            OutlineMode = FindProperty(TmpProperty.OutlineMode, properties);
            materialEditor.ShaderProperty(OutlineMode, new GUIContent("OutlineMode"));
            OutlineWidth = FindProperty(TmpProperty.OutlineWidth, properties);
            materialEditor.ShaderProperty(OutlineWidth, new GUIContent("OutlineWidth"));
            OutlineTex = FindProperty(TmpProperty.OutlineTex, properties);
            materialEditor.TextureProperty(OutlineTex, "OutlineTex", true);

            OutlineColor1 = FindProperty(TmpProperty.OutlineColor1, properties);
            materialEditor.ColorProperty(OutlineColor1, "OutlineColor1");
            OutlineOffset1 = FindProperty(TmpProperty.OutlineOffset1, properties);
            materialEditor.ShaderProperty(OutlineOffset1, new GUIContent("OutlineOffset1"));
            OutlineColor2 = FindProperty(TmpProperty.OutlineColor2, properties);
            materialEditor.ColorProperty(OutlineColor2, "OutlineColor2");
            OutlineOffset2 = FindProperty(TmpProperty.OutlineOffset2, properties);
            materialEditor.ShaderProperty(OutlineOffset2, new GUIContent("OutlineOffset2"));
            OutlineColor3 = FindProperty(TmpProperty.OutlineColor3, properties);
            materialEditor.ColorProperty(OutlineColor3, "OutlineColor3");
            OutlineOffset3 = FindProperty(TmpProperty.OutlineOffset3, properties);
            materialEditor.ShaderProperty(OutlineOffset3, new GUIContent("OutlineOffset3"));

            OutlineTex_ST = FindProperty(TmpProperty.OutlineTex_ST, properties);
            materialEditor.ShaderProperty(OutlineTex_ST, new GUIContent("OutlineTex_ST"));
            OutlineUVSpeed = FindProperty(TmpProperty.OutlineUVSpeed, properties);
            materialEditor.ShaderProperty(OutlineUVSpeed, new GUIContent("OutlineUVSpeed"));
            EditorGUILayout.EndVertical();
        }

        public MaterialProperty UnderlayColor;
        public MaterialProperty UnderlayOffset;
        public MaterialProperty UnderlayDilate;
        public MaterialProperty UnderlaySoftness;
        void SetUnderlay(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            EditorGUILayout.BeginVertical("box");
            UnderlayColor = FindProperty(TmpProperty.UnderlayColor, properties);
            materialEditor.ShaderProperty(UnderlayColor, new GUIContent("UnderlayColor"));
            UnderlayOffset = FindProperty(TmpProperty.UnderlayOffset, properties);
            materialEditor.ShaderProperty(UnderlayOffset, new GUIContent("UnderlayOffset"));
            UnderlayDilate = FindProperty(TmpProperty.UnderlayDilate, properties);
            materialEditor.ShaderProperty(UnderlayDilate, new GUIContent("UnderlayDilate"));
            UnderlaySoftness = FindProperty(TmpProperty.UnderlaySoftness, properties);
            materialEditor.ShaderProperty(UnderlaySoftness, new GUIContent("UnderlaySoftness"));
            EditorGUILayout.EndVertical();
        }

        public MaterialProperty SpecularColor;
        public MaterialProperty LightAngle;
        public MaterialProperty SpecularPower;
        public MaterialProperty Reflectivity;
        public MaterialProperty Diffuse;
        public MaterialProperty Ambient;
        void SetLight(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            EditorGUILayout.BeginVertical("box");
            SpecularColor = FindProperty(TmpProperty.SpecularColor, properties);
            materialEditor.ShaderProperty(SpecularColor, new GUIContent("SpecularColor"));
            LightAngle = FindProperty(TmpProperty.LightAngle, properties);
            materialEditor.ShaderProperty(LightAngle, new GUIContent("LightAngle"));
            SpecularPower = FindProperty(TmpProperty.SpecularPower, properties);
            materialEditor.ShaderProperty(SpecularPower, new GUIContent("SpecularPower"));
            Reflectivity = FindProperty(TmpProperty.Reflectivity, properties);
            materialEditor.ShaderProperty(Reflectivity, new GUIContent("Reflectivity"));
            Diffuse = FindProperty(TmpProperty.Diffuse, properties);
            materialEditor.ShaderProperty(Diffuse, new GUIContent("Diffuse"));
            Ambient = FindProperty(TmpProperty.Ambient, properties);
            materialEditor.ShaderProperty(Ambient, new GUIContent("Ambient"));
            EditorGUILayout.EndVertical();
        }

        public MaterialProperty BevelType;
        public MaterialProperty BevelAmount, BevelOffset, BevelWidth;
        public MaterialProperty BevelRoundness, BevelClamp;
        void SetBevel(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            EditorGUILayout.BeginVertical("box");
            BevelType = FindProperty(TmpProperty.BevelType, properties);
            materialEditor.ShaderProperty(BevelType, new GUIContent("BevelType"));
            BevelAmount = FindProperty(TmpProperty.BevelAmount, properties);
            materialEditor.ShaderProperty(BevelAmount, new GUIContent("BevelAmount"));
            BevelOffset = FindProperty(TmpProperty.BevelOffset, properties);
            materialEditor.ShaderProperty(BevelOffset, new GUIContent("BevelOffset"));
            BevelWidth = FindProperty(TmpProperty.BevelWidth, properties);
            materialEditor.ShaderProperty(BevelWidth, new GUIContent("BevelWidth"));
            BevelRoundness = FindProperty(TmpProperty.BevelRoundness, properties);
            materialEditor.ShaderProperty(BevelRoundness, new GUIContent("BevelRoundness"));
            BevelClamp = FindProperty(TmpProperty.BevelClamp, properties);
            materialEditor.ShaderProperty(BevelClamp, new GUIContent("BevelClamp"));
            EditorGUILayout.EndVertical();
        }
        MaterialProperty MainTex, GradientScale;
        MaterialProperty ScaleRatioA;
        //MaterialProperty WeightNormal, WeightBold;
        //MaterialProperty TextureWidth,TextureHeight;
        void SetDebugSetting(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            EditorGUILayout.BeginVertical("box");
            MainTex = FindProperty(TmpProperty.MainTex, properties);
            materialEditor.TextureProperty(MainTex, "MainTex", true);
            GradientScale = FindProperty(TmpProperty.GradientScale, properties);
            materialEditor.ShaderProperty(GradientScale, new GUIContent("GradientScale"));
            ScaleRatioA = FindProperty(TmpProperty.ScaleRatioA, properties);
            materialEditor.ShaderProperty(ScaleRatioA, new GUIContent("ScaleRatioA"));
            //TextureWidth = FindProperty(TmpProperty.TextureWidth, properties);
            //materialEditor.ShaderProperty(TextureWidth, new GUIContent("TextureWidth"));
            //TextureHeight = FindProperty(TmpProperty.TextureHeight, properties);
            //materialEditor.ShaderProperty(TextureHeight, new GUIContent("TextureHeight"));
            //WeightNormal = FindProperty(TmpProperty.WeightNormal, properties);
            //materialEditor.ShaderProperty(WeightNormal, new GUIContent("WeightNormal"));
            //WeightBold = FindProperty(TmpProperty.WeightBold, properties);
            //materialEditor.ShaderProperty(WeightBold, new GUIContent("WeightBold"));
            EditorGUILayout.EndVertical();
        }

        public void OptionStruct(MaterialEditor _materialEditor)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.Space(10f);
            _materialEditor.RenderQueueField();
            _materialEditor.EnableInstancingField();
            _materialEditor.DoubleSidedGIField();
            EditorGUILayout.EndVertical();
        }

        public void FaceStruct()
        {
            bool isOn = (displayType == DisplayType.Face);
            if (SetButton("페이스", ButtonStyle(isOn)))
            {
                displayType = (isOn == true) ? DisplayType.None : DisplayType.Face;
            }
        }

        public void OutlineStruct()
        {
            bool isOn = (displayType == DisplayType.Outline);
            if (SetButton("아웃라인", ButtonStyle(isOn)))
            {
                displayType = (isOn == true) ? DisplayType.None : DisplayType.Outline;
            }
        }

        public void UnderlayStruct()
        {
            bool isOn = (displayType == DisplayType.Underlay);
            if (GUILayout.Button("언더레이", ButtonStyle(isOn), GUILayout.Height(25)))
            {
                displayType = (isOn == true) ? DisplayType.None : DisplayType.Underlay;
            }
        }

        public void LightStruct()
        {
            bool isOn = (displayType == DisplayType.Light);
            if (SetButton("라이트", ButtonStyle(isOn)))
            {
                displayType = (isOn == true) ? DisplayType.None : DisplayType.Light;
            }
        }

        public void BevelStruct()
        {
            bool isOn = (displayType == DisplayType.Bevel);
            if (SetButton("베벨", ButtonStyle(isOn)))
            {
                displayType = (isOn == true) ? DisplayType.None : DisplayType.Bevel;
            }
        }

        public void DebugSettingStruct()
        {
            bool isOn = (displayType == DisplayType.DebugSetting);
            if (GUILayout.Button("디버그", ButtonStyle(isOn), GUILayout.Height(25)))
            {
                displayType = (isOn == true) ? DisplayType.None : DisplayType.DebugSetting;
            }
        }
        bool onCustom = false;
        public bool CustomStruct()
        {
            if (GUILayout.Button("커스텀", ButtonStyle(onCustom), GUILayout.Height(25)))
            {
                onCustom = !onCustom;
            }
            return onCustom;
        }

        bool SetButton(string _name, GUIStyle _style)
        {
            float width = EditorGUIUtility.currentViewWidth / 3f - 8f * (3f - 1f);
            return GUILayout.Button(_name, _style, GUILayout.Width(width), GUILayout.Height(25));
        }

        GUIStyle ButtonStyle(bool _isOn)
        {
            GUIStyle fontStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 15,
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleCenter,
                fontStyle = _isOn ? FontStyle.Bold : FontStyle.Normal,
            };
            GUI.color = _isOn ? Color.yellow : Color.white;
            return fontStyle;
        }
    }
}