using System.Collections.Generic;
using UnityEngine;

//#if UNITY_EDITOR
//using UnityEditor;

//[CustomEditor(typeof(Data_DialogType))]
//public class Data_DialogType_Editor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        GUIStyle fontStyle = new GUIStyle(GUI.skin.button);
//        fontStyle.fontSize = 15;
//        fontStyle.normal.textColor = Color.yellow;

//        Data_DialogType Inspector = target as Data_DialogType;
//        if (GUILayout.Button("Data Parse", fontStyle, GUILayout.Height(30f)))
//        {
//            Inspector.UpdateData();
//            EditorUtility.SetDirty(Inspector);
//        }
//        GUILayout.Space(10f);
//        base.OnInspectorGUI();
//    }
//}
//#endif

[CreateAssetMenu(fileName = "Data_DialogType", menuName = "Scriptable Objects/Data_DialogType")]
public class Data_DialogType : ScriptableObject
{
    public enum DialogAnimation
    {
        None,
        Shake,
        Wave
    }

    [HideInInspector]
    public string id;
    public float speed;
    [Range(0f, 1f)]
    public float interval;
    public Vector2 angle;
    public AnimationCurve curve;
}
