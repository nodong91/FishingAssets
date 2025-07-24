using System;
using UnityEditor;
using UnityEngine;

//P01.Editor.Editor_Env_ObjectDefault
namespace P01.Editor
{
    public class Editor_Env_ObjectDefault : ShaderGUI
    {
        MaterialEditor materialEditor;
        MaterialProperty[] properties;
        //GUIStyle fontStyle;
        bool baseStruct, normalStruct, metallicStruct, aoStruct, emissionStruct, fogStruct;
        MaterialProperty _Alpha_Clipping, _Color, _BaseMap, _Base_Color_Base_Opacity, _isNormal;
        MaterialProperty _Normal_Strength, _NormalMap, _Metallic, _Roughness, _Roughness_In_Lightmap, _MRHMap;
        MaterialProperty _isAO, _UV_CHANNEL_AO, _AO_Strength, _AOMap;
        MaterialProperty _Emission_Map, _EmissionColor, _Fog_On, _Fog_Smooth, _Fog_Hight, _Fog_Color;

        public override void OnGUI(MaterialEditor _materialEditor, MaterialProperty[] _properties)
        {
            materialEditor = _materialEditor;
            properties = _properties;
            //base.OnGUI(materialEditor, properties);

            GUIColor(baseStruct, BaseStruct);
            GUIColor(normalStruct, NormalStruct);
            GUIColor(metallicStruct, MetallicStruct);
            GUIColor(aoStruct, AOStruct);
            GUIColor(emissionStruct, EmissionStruct);
            GUIColor(fogStruct, FogStruct);
            OptionStruct();
        }

        void BaseStruct()
        {
            if (SetButton("Base"))
            {
                baseStruct = !baseStruct;
            }
            if (baseStruct == true)
            {
                EditorGUILayout.BeginVertical("box");
                _Alpha_Clipping = FindProperty("_Alpha_Clipping", properties);
                materialEditor.ShaderProperty(_Alpha_Clipping, new GUIContent("_Alpha_Clipping"));
                _Color = FindProperty("_Color", properties);
                materialEditor.ShaderProperty(_Color, new GUIContent("_Color"));
                _BaseMap = FindProperty("_BaseMap", properties);
                materialEditor.ShaderProperty(_BaseMap, new GUIContent("_BaseMap"));
                _Base_Color_Base_Opacity = FindProperty("_Base_Color_Base_Opacity", properties);
                materialEditor.ShaderProperty(_Base_Color_Base_Opacity, new GUIContent("_Base_Color_Base_Opacity"));
                EditorGUILayout.EndVertical();
            }
        }

        void NormalStruct()
        {
            if (SetButton("Normal"))
            {
                normalStruct = !normalStruct;
            }
            if (normalStruct == true)
            {
                EditorGUILayout.BeginVertical("box");
                _isNormal = FindProperty("_isNormal", properties);
                materialEditor.ShaderProperty(_isNormal, new GUIContent("_isNormal"));
                _Normal_Strength = FindProperty("_Normal_Strength", properties);
                materialEditor.ShaderProperty(_Normal_Strength, new GUIContent("_Normal_Strength"));
                _NormalMap = FindProperty("_NormalMap", properties);
                materialEditor.ShaderProperty(_NormalMap, new GUIContent("_NormalMap"));
                EditorGUILayout.EndVertical();
            }
        }

        void MetallicStruct()
        {
            if (SetButton("Metallic"))
            {
                metallicStruct = !metallicStruct;
            }
            if (metallicStruct == true)
            {
                EditorGUILayout.BeginVertical("box");
                _Metallic = FindProperty("_Metallic", properties);
                materialEditor.ShaderProperty(_Metallic, new GUIContent("_Metallic"));
                _Roughness = FindProperty("_Roughness", properties);
                materialEditor.ShaderProperty(_Roughness, new GUIContent("_Roughness"));
                _Roughness_In_Lightmap = FindProperty("_Roughness_In_Lightmap", properties);
                materialEditor.ShaderProperty(_Roughness_In_Lightmap, new GUIContent("_Roughness_In_Lightmap"));
                _MRHMap = FindProperty("_MRHMap", properties);
                materialEditor.ShaderProperty(_MRHMap, new GUIContent("_MRHMap"));
                EditorGUILayout.EndVertical();
            }
        }

        void AOStruct()
        {
            if (SetButton("AO"))
            {
                aoStruct = !aoStruct;
            }
            if (aoStruct == true)
            {
                EditorGUILayout.BeginVertical("box");
                _isAO = FindProperty("_isAO", properties);
                materialEditor.ShaderProperty(_isAO, new GUIContent("_isAO"));
                _UV_CHANNEL_AO = FindProperty("_UV_CHANNEL_AO", properties);
                materialEditor.ShaderProperty(_UV_CHANNEL_AO, new GUIContent("_UV_CHANNEL_AO"));
                _AO_Strength = FindProperty("_AO_Strength", properties);
                materialEditor.ShaderProperty(_AO_Strength, new GUIContent("_AO_Strength"));
                _AOMap = FindProperty("_AOMap", properties);
                materialEditor.ShaderProperty(_AOMap, new GUIContent("_AOMap"));
                EditorGUILayout.EndVertical();
            }
        }

        void EmissionStruct()
        {
            if (SetButton("Emission"))
            {
                emissionStruct = !emissionStruct;
            }
            if (emissionStruct == true)
            {
                EditorGUILayout.BeginVertical("box");
                _Emission_Map = FindProperty("_Emission_Map", properties);
                materialEditor.ShaderProperty(_Emission_Map, new GUIContent("_Emission_Map"));
                _EmissionColor = FindProperty("_EmissionColor", properties);
                materialEditor.ShaderProperty(_EmissionColor, new GUIContent("_EmissionColor"));
                EditorGUILayout.EndVertical();
            }
        }

        void FogStruct()
        {
            if (SetButton("Fog"))
            {
                fogStruct = !fogStruct;
            }
            if (fogStruct == true)
            {
                EditorGUILayout.BeginVertical("box");
                _Fog_On = FindProperty("_Fog_On", properties);
                materialEditor.ShaderProperty(_Fog_On, new GUIContent("_Fog_On"));
                _Fog_Smooth = FindProperty("_Fog_Smooth", properties);
                materialEditor.ShaderProperty(_Fog_Smooth, new GUIContent("_Fog_Smooth"));
                _Fog_Hight = FindProperty("_Fog_Hight", properties);
                materialEditor.ShaderProperty(_Fog_Hight, new GUIContent("_Fog_Hight"));
                _Fog_Color = FindProperty("_Fog_Color", properties);
                materialEditor.ShaderProperty(_Fog_Color, new GUIContent("_Fog_Color"));
                EditorGUILayout.EndVertical();
            }
        }

        bool SetButton(string _name)
        {
            GUIStyle fontStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 15,
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
            };
            return GUILayout.Button(_name, fontStyle, GUILayout.Height(25));
        }

        void GUIColor(bool _struct, Action _action)
        {
            //Color OnColor = Color.white;
            //Color OffColor = Color.white;
            //GUI.color = (_struct == true) ? OnColor : OffColor;
            _action();
            //GUI.color = OffColor;
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
    }
}