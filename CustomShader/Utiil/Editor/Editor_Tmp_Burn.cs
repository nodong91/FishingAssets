using P01.Editor;
using UnityEditor;
using UnityEngine;

namespace P01.Editor
{
    public class Editor_Tmp_Burn : Editor_Tmp_Base
    {
        Vector2 scrollPosition;
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            GUILayout.Label("타서 없어지는 글자", EditorStyles.boldLabel);
            EditorGUILayout.Space(10f);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.BeginHorizontal();
            FaceStruct();
            OutlineStruct();
            UnderlayStruct();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            LightStruct();
            BevelStruct();
            DebugSettingStruct();
            EditorGUILayout.EndHorizontal();

            GUI.color = Color.white;
            SetGUI(materialEditor, properties);
            bool onCustom = CustomStruct();
            GUI.color = Color.white;
            if (onCustom == true)
                SetCustom(materialEditor, properties);
            EditorGUILayout.EndScrollView();

            OptionStruct(materialEditor);
        }

        MaterialProperty _BurnAmount, _BurnPoint, _BurnNoise;
        MaterialProperty _BurnColor;

        void SetCustom(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            EditorGUILayout.BeginVertical("box");

            _BurnAmount = FindProperty("_BurnAmount", properties);
            materialEditor.RangeProperty(_BurnAmount, "BurnAmount");

            _BurnPoint = FindProperty("_BurnPoint", properties);
            materialEditor.RangeProperty(_BurnPoint, "BurnPoint");

            _BurnNoise = FindProperty("_BurnNoise", properties);
            materialEditor.FloatProperty(_BurnNoise, "BurnNoise");

            _BurnColor = FindProperty("_BurnColor", properties);
            materialEditor.ShaderProperty(_BurnColor, new GUIContent("BurnColor"));

            //testTexture = FindProperty("_TestTexture", properties);
            //materialEditor.TextureProperty(testTexture, "텍스처", true);
            //materialEditor.ShaderProperty(testFloat, new GUIContent("ShaderProperty"));
            EditorGUILayout.EndVertical();
        }
    }
}