using UnityEditor;
using UnityEngine;

//P01.Editor.Editor_Tmp_Cartoon
namespace P01.Editor
{
    public class Editor_Tmp_Cartoon : Editor_Tmp_Base
    {
        Vector2 scrollPosition;
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            GUILayout.Label("특수 효과 글자", EditorStyles.boldLabel);
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

        MaterialProperty _CartoonColor, _CartoonAngle, _CartoonSize, _CartoonTile;
        void SetCustom(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            EditorGUILayout.BeginVertical("box");

            _CartoonColor = FindProperty("_CartoonColor", properties);
            materialEditor.ShaderProperty(_CartoonColor, new GUIContent("CartoonColor"));
            _CartoonAngle = FindProperty("_CartoonAngle", properties);
            materialEditor.ShaderProperty(_CartoonAngle, new GUIContent("CartoonAngle"));
            _CartoonSize = FindProperty("_CartoonSize", properties);
            materialEditor.ShaderProperty(_CartoonSize, new GUIContent("CartoonSize"));
            _CartoonTile = FindProperty("_CartoonTile", properties);
            materialEditor.ShaderProperty(_CartoonTile, new GUIContent("CartoonTile"));
            EditorGUILayout.EndVertical();
        }
    }
}