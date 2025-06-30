using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Data_DialogType))]
public class Data_DialogType_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        GUIStyle fontStyle = new GUIStyle(GUI.skin.button);
        fontStyle.fontSize = 15;
        fontStyle.normal.textColor = Color.yellow;

        Data_DialogType Inspector = target as Data_DialogType;
        if (GUILayout.Button("Data Parse", fontStyle, GUILayout.Height(30f)))
        {
            Inspector.UpdateData();
            EditorUtility.SetDirty(Inspector);
        }
        GUILayout.Space(10f);
        base.OnInspectorGUI();
    }
}
#endif

[CreateAssetMenu(fileName = "Data_DialogType", menuName = "Scriptable Objects/Data_DialogType")]
public class Data_DialogType : ScriptableObject
{
    public enum TextStyle
    {
        None,
        A,
        B,
        C,
        D,
        E,
        F,
        G
    }

    [System.Serializable]
    public class ActionType
    {
        [HideInInspector]
        public string id;
        public float speed;
        public float interval;
        public Vector2 angle;
        public AnimationCurve curve;
    }
    public List<ActionType> actionType = new List<ActionType>();

    public void UpdateData()
    {
        int count = System.Enum.GetValues(typeof(TextStyle)).Length;
        for (int i = 1; i < count; i++)
        {
            if (i - 1 >= actionType.Count)
            {
                actionType.Add(new ActionType());
            }
            actionType[i - 1].id = ((TextStyle)i).ToString();
            Debug.LogWarning(((TextStyle)i).ToString());
        }
    }
}
