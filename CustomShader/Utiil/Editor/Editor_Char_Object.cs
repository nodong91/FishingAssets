using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;
//P01.Editor.Editor_Char_Object
namespace P01.Editor
{
    public class Editor_Char_Object : ShaderGUI
    {
        MaterialEditor materialEditor;
        MaterialProperty[] properties;
        public enum DisplayType
        {
            None,
            Main,
            Array,
            Emission,
            Splat_Main,
            Splat_Sub,
            RimLight,
            Damage,
            Potal
        }
        public DisplayType displayType = DisplayType.None;
        Vector2 scrollPosition;
        public MaterialProperty _ShadowColor, _MainColor, _MainTexture, _SDF;
        public MaterialProperty _IsArray, _ArrayIndex, _ArrayTexture;
        public MaterialProperty _EmissionColor, _EmissionTexture;
        public MaterialProperty _Splat_Main, _SubShadowChannel, _Sub_ShadowColor, _MatCap_Channel, _MatCap_Value, _MatCapBlend_MSO;
        public MaterialProperty _MatCap_Color, _MatCap_Texture, _OcclusionChannel, _OcclusionStrength;

        public override void OnGUI(MaterialEditor _materialEditor, MaterialProperty[] _properties)
        {
            materialEditor = _materialEditor;
            properties = _properties;

            string explain = "- 셀 방식의 캐릭터, 오브젝트 등에 활용";
            GUILayout.Label(explain, EditorStyles.boldLabel);
            EditorGUILayout.Space(10f);

            //EditorGUILayout.Space(0f);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                ButtonStruct();
                GUI.color = Color.white;
                EditorGUILayout.BeginVertical("box");
                switch (displayType)
                {
                    case DisplayType.Main:
                        MainStruct();
                        break;

                    case DisplayType.Array:
                        ArrayStruct();
                        break;

                    case DisplayType.Emission:
                        EmissionStruct();
                        break;

                    case DisplayType.Splat_Main:
                        Splat_MainStruct();
                        break;

                    case DisplayType.Splat_Sub:
                        Splat_SubStruct();
                        break;

                    case DisplayType.RimLight:
                        RimLightStruct();
                        break;

                    case DisplayType.Damage:
                        DamageStruct();
                        break;

                    case DisplayType.Potal:
                        PotalStruct();
                        break;
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();

            OptionStruct();
            //base.OnGUI(materialEditor, properties);
        }

        void OptionStruct()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.Space(10f);
            materialEditor.RenderQueueField();
            materialEditor.EnableInstancingField();
            materialEditor.DoubleSidedGIField();
            EditorGUILayout.EndVertical();
        }

        void ButtonStruct()
        {
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    bool isOn = (displayType == DisplayType.Main);
                    GUI.color = isOn ? Color.yellow : Color.white;
                    if (SetButton(DisplayType.Main.ToString(), ButtonStyle(isOn), 3f))
                    {
                        displayType = (isOn == true) ? DisplayType.None : DisplayType.Main;
                    }

                    isOn = (displayType == DisplayType.Array);
                    GUI.color = isOn ? Color.yellow : Color.white;
                    if (SetButton(DisplayType.Array.ToString(), ButtonStyle(isOn), 3f))
                    {
                        displayType = (isOn == true) ? DisplayType.None : DisplayType.Array;
                    }

                    isOn = (displayType == DisplayType.Emission);
                    GUI.color = isOn ? Color.yellow : Color.white;
                    if (SetButton(DisplayType.Emission.ToString(), ButtonStyle(isOn), 3f))
                    {
                        displayType = (isOn == true) ? DisplayType.None : DisplayType.Emission;
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    bool isOn = (displayType == DisplayType.Splat_Main);
                    GUI.color = isOn ? Color.yellow : Color.white;
                    if (SetButton(DisplayType.Splat_Main.ToString(), ButtonStyle(isOn), 3f))
                    {
                        displayType = (isOn == true) ? DisplayType.None : DisplayType.Splat_Main;
                    }

                    isOn = (displayType == DisplayType.Splat_Sub);
                    GUI.color = isOn ? Color.yellow : Color.white;
                    if (SetButton(DisplayType.Splat_Sub.ToString(), ButtonStyle(isOn), 3f))
                    {
                        displayType = (isOn == true) ? DisplayType.None : DisplayType.Splat_Sub;
                    }

                    isOn = (displayType == DisplayType.RimLight);
                    GUI.color = isOn ? Color.yellow : Color.white;
                    if (SetButton(DisplayType.RimLight.ToString(), ButtonStyle(isOn), 3f))
                    {
                        displayType = (isOn == true) ? DisplayType.None : DisplayType.RimLight;
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    bool isOn = (displayType == DisplayType.Damage);
                    GUI.color = isOn ? Color.yellow : Color.white;
                    if (SetButton(DisplayType.Damage.ToString(), ButtonStyle(isOn), 2f))
                    {
                        displayType = (isOn == true) ? DisplayType.None : DisplayType.Damage;
                    }

                    isOn = (displayType == DisplayType.Potal);
                    GUI.color = isOn ? Color.yellow : Color.white;
                    if (SetButton(DisplayType.Potal.ToString(), ButtonStyle(isOn), 2f))
                    {
                        displayType = (isOn == true) ? DisplayType.None : DisplayType.Potal;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            //GUILayout.Box("Box", GUILayout.Width(EditorGUIUtility.currentViewWidth));
        }

        bool SetButton(string _name, GUIStyle _style, float _index)
        {
            //GUILayout.Label( currentViewWidth.ToString(), EditorStyles.boldLabel);
            float width = ((EditorGUIUtility.currentViewWidth - 45f) - ((_index - 1f) * 3f)) / _index;
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
            return fontStyle;
        }

        void MainStruct()
        {
            _ShadowColor = FindProperty("_ShadowColor", properties);
            materialEditor.ShaderProperty(_ShadowColor, new GUIContent("_ShadowColor"));
            _MainColor = FindProperty("_MainColor", properties);
            materialEditor.ShaderProperty(_MainColor, new GUIContent("_MainColor"));
            _MainTexture = FindProperty("_MainTexture", properties);
            materialEditor.TextureProperty(_MainTexture, "_MainTexture", true);
            _SDF = FindProperty("_SDF", properties);
            materialEditor.TextureProperty(_SDF, "_SDF", true);
        }

        void ArrayStruct()
        {
            _IsArray = FindProperty("_IsArray", properties);
            materialEditor.ShaderProperty(_IsArray, new GUIContent("_IsArray"));
            _ArrayIndex = FindProperty("_ArrayIndex", properties);
            materialEditor.ShaderProperty(_ArrayIndex, new GUIContent("_ArrayIndex"));
            _ArrayTexture = FindProperty("_ArrayTexture", properties);
            materialEditor.TextureProperty(_ArrayTexture, "_ArrayTexture", true);
        }

        void EmissionStruct()
        {
            _EmissionColor = FindProperty("_EmissionColor", properties);
            materialEditor.ShaderProperty(_EmissionColor, new GUIContent("_EmissionColor"));
            _EmissionTexture = FindProperty("_EmissionTexture", properties);
            materialEditor.TextureProperty(_EmissionTexture, "_EmissionTexture", true);
        }

        void Splat_MainStruct()
        {
            _Splat_Main = FindProperty("_Splat_Main", properties);
            materialEditor.TextureProperty(_Splat_Main, "_Splat_Main", true);
            _SubShadowChannel = FindProperty("_SubShadowChannel", properties);
            materialEditor.ShaderProperty(_SubShadowChannel, new GUIContent("_SubShadowChannel"));
            _Sub_ShadowColor = FindProperty("_Sub_ShadowColor", properties);
            materialEditor.ShaderProperty(_Sub_ShadowColor, new GUIContent("_Sub_ShadowColor"));
            _MatCap_Channel = FindProperty("_MatCap_Channel", properties);
            materialEditor.ShaderProperty(_Sub_ShadowColor, new GUIContent("_MatCap_Channel"));
            _MatCap_Value = FindProperty("_MatCap_Value", properties);
            materialEditor.ShaderProperty(_MatCap_Value, new GUIContent("_MatCap_Value"));
            _MatCapBlend_MSO = FindProperty("_MatCapBlend_MSO", properties);
            materialEditor.ShaderProperty(_MatCapBlend_MSO, new GUIContent("_MatCapBlend_MSO"));
            _MatCap_Color = FindProperty("_MatCap_Color", properties);
            materialEditor.ShaderProperty(_MatCap_Color, new GUIContent("_MatCap_Color"));
            _MatCap_Texture = FindProperty("_MatCap_Texture", properties);
            materialEditor.TextureProperty(_MatCap_Texture, "_MatCap_Texture", true);
            _OcclusionChannel = FindProperty("_OcclusionChannel", properties);
            materialEditor.ShaderProperty(_OcclusionChannel, new GUIContent("_OcclusionChannel"));
            _OcclusionStrength = FindProperty("_OcclusionStrength", properties);
            materialEditor.ShaderProperty(_OcclusionStrength, new GUIContent("_OcclusionStrength"));
        }
        public MaterialProperty _Splat_Sub, _Outline_Mask_Channel, _MatCap2_Channel, _MatCap2_Value, _MatCap2Blend_MSO;
        public MaterialProperty _MatCap2_Color, _MatCap2_Texture, _SpecularChannel, _Specular_Strength, _Specular_Smooth, _SpecularColor;
        void Splat_SubStruct()
        {
            _Splat_Sub = FindProperty("_Splat_Sub", properties);
            materialEditor.TextureProperty(_Splat_Sub, "_Splat_Sub", true);
            _Outline_Mask_Channel = FindProperty("_Outline_Mask_Channel", properties);
            materialEditor.ShaderProperty(_Outline_Mask_Channel, new GUIContent("_Outline_Mask_Channel"));
            _MatCap2_Channel = FindProperty("_MatCap2_Channel", properties);
            materialEditor.ShaderProperty(_MatCap2_Channel, new GUIContent("_MatCap2_Channel"));
            _MatCap2_Value = FindProperty("_MatCap2_Value", properties);
            materialEditor.ShaderProperty(_MatCap2_Value, new GUIContent("_MatCap2_Value"));
            _MatCap2Blend_MSO = FindProperty("_MatCap2Blend_MSO", properties);
            materialEditor.ShaderProperty(_MatCap2Blend_MSO, new GUIContent("_MatCap2Blend_MSO"));
            _MatCap2_Color = FindProperty("_MatCap2_Color", properties);
            materialEditor.ShaderProperty(_MatCap2_Color, new GUIContent("_MatCap2_Color"));
            _MatCap2_Texture = FindProperty("_MatCap2_Texture", properties);
            materialEditor.TextureProperty(_MatCap2_Texture, "_MatCap2_Texture", true);
            _SpecularChannel = FindProperty("_SpecularChannel", properties);
            materialEditor.ShaderProperty(_SpecularChannel, new GUIContent("_SpecularChannel"));
            _Specular_Strength = FindProperty("_Specular_Strength", properties);
            materialEditor.ShaderProperty(_Specular_Strength, new GUIContent("_Specular_Strength"));
            _Specular_Smooth = FindProperty("_Specular_Smooth", properties);
            materialEditor.ShaderProperty(_Specular_Smooth, new GUIContent("_Specular_Smooth"));
            _SpecularColor = FindProperty("_SpecularColor", properties);
            materialEditor.ShaderProperty(_SpecularColor, new GUIContent("_SpecularColor"));
        }
        public MaterialProperty _Rimlight_Strangth, _Rimlight_Smooth;

        void RimLightStruct()
        {
            _Rimlight_Strangth = FindProperty("_Rimlight_Strangth", properties);
            materialEditor.ShaderProperty(_Rimlight_Strangth, new GUIContent("_Rimlight_Strangth"));
            _Rimlight_Smooth = FindProperty("_Rimlight_Smooth", properties);
            materialEditor.ShaderProperty(_Rimlight_Smooth, new GUIContent("_Rimlight_Smooth"));
        }
        public MaterialProperty _Damage, _DamageColor;
        void DamageStruct()
        {
            _Damage = FindProperty("_Damage", properties);
            materialEditor.ShaderProperty(_Damage, new GUIContent("_Damage"));
            _DamageColor = FindProperty("_DamageColor", properties);
            materialEditor.ShaderProperty(_DamageColor, new GUIContent("_DamageColor"));
        }
        public MaterialProperty _POTALTYPE, _PortalAmount, _PortalMinMax, _PortalColor;
        public MaterialProperty _NoiseAmount, _NoiseDistortion, _NoiseSpeed;
        void PotalStruct()
        {
            _POTALTYPE = FindProperty("_POTALTYPE", properties);
            materialEditor.ShaderProperty(_POTALTYPE, new GUIContent("_POTALTYPE"));
            _PortalAmount = FindProperty("_PortalAmount", properties);
            materialEditor.ShaderProperty(_PortalAmount, new GUIContent("_PortalAmount"));
            _PortalMinMax = FindProperty("_PortalMinMax", properties);
            materialEditor.ShaderProperty(_PortalMinMax, new GUIContent("_PortalMinMax"));
            _PortalColor = FindProperty("_PortalColor", properties);
            materialEditor.ShaderProperty(_PortalColor, new GUIContent("_PortalColor"));
            _NoiseAmount = FindProperty("_NoiseAmount", properties);
            materialEditor.ShaderProperty(_NoiseAmount, new GUIContent("_NoiseAmount"));
            _NoiseDistortion = FindProperty("_NoiseDistortion", properties);
            materialEditor.ShaderProperty(_NoiseDistortion, new GUIContent("_NoiseDistortion"));
            _NoiseSpeed = FindProperty("_NoiseSpeed", properties);
            materialEditor.ShaderProperty(_NoiseSpeed, new GUIContent("_NoiseSpeed"));
        }
    }
}