using Codice.Client.BaseCommands.BranchExplorer.Layout;
using UnityEditor;
using UnityEngine;

//P01.Editor.Editor_Tmp_Effect
namespace P01.Editor
{
    public class Editor_Tmp_Effect : Editor_Tmp_Base
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

        MaterialProperty FramePerSecond, DISTORTIONTYPE, OutlineOffset, LineSpeed, LineSize;
        MaterialProperty  _LineRotate, _LineFraction, _LineDistortion, _WaveSpeed, _WaveSize;
        MaterialProperty _WaveDistortion, _CartoonColor, _CartoonAngle, _CartoonSize, _CartoonTile;
        void SetCustom(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            EditorGUILayout.BeginVertical("box");

            FramePerSecond = FindProperty("_FramePerSecond", properties);
            materialEditor.ShaderProperty(FramePerSecond, new GUIContent("FramePerSecond"));

            DISTORTIONTYPE = FindProperty("_DISTORTIONTYPE", properties);
            materialEditor.ShaderProperty(DISTORTIONTYPE, new GUIContent("DISTORTIONTYPE"));

            OutlineOffset = FindProperty("_OutlineOffset", properties);
            materialEditor.FloatProperty(OutlineOffset, "OutlineOffset");

            LineSpeed = FindProperty("_LineSpeed", properties);
            materialEditor.ShaderProperty(LineSpeed, new GUIContent("LineSpeed"));

            LineSize = FindProperty("_LineSize", properties);
            materialEditor.ShaderProperty(LineSize, new GUIContent("LineSize"));

            _LineRotate = FindProperty("_LineRotate", properties);
            materialEditor.ShaderProperty(_LineRotate, new GUIContent("LineRotate"));
            _LineFraction = FindProperty("_LineFraction", properties);
            materialEditor.ShaderProperty(_LineFraction, new GUIContent("LineFraction"));
            _LineDistortion = FindProperty("_LineDistortion", properties);
            materialEditor.ShaderProperty(_LineDistortion, new GUIContent("LineDistortion"));
            _WaveSpeed = FindProperty("_WaveSpeed", properties);
            materialEditor.ShaderProperty(_WaveSpeed, new GUIContent("WaveSpeed"));
            _WaveSize = FindProperty("_WaveSize", properties);
            materialEditor.ShaderProperty(_WaveSize, new GUIContent("WaveSize"));

            _WaveDistortion = FindProperty("_WaveDistortion", properties);
            materialEditor.ShaderProperty(_WaveDistortion, new GUIContent("WaveDistortion"));
            //_CartoonColor = FindProperty("_CartoonColor", properties);
            //materialEditor.ShaderProperty(_CartoonColor, new GUIContent("CartoonColor"));
            //_CartoonAngle = FindProperty("_CartoonAngle", properties);
            //materialEditor.ShaderProperty(_CartoonAngle, new GUIContent("CartoonAngle"));
            //_CartoonSize = FindProperty("_CartoonSize", properties);
            //materialEditor.ShaderProperty(_CartoonSize, new GUIContent("CartoonSize"));
            //_CartoonTile = FindProperty("_CartoonTile", properties);
            //materialEditor.ShaderProperty(_CartoonTile, new GUIContent("CartoonTile"));
            EditorGUILayout.EndVertical();
        }
    }
}