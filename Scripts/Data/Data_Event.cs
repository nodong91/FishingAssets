using UnityEngine;
using static Data_Dialog;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Data_Event))]
public class Data_Event_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        GUIStyle fontStyle = new GUIStyle(GUI.skin.button);
        fontStyle.fontSize = 15;
        fontStyle.normal.textColor = Color.yellow;

        Data_Event Inspector = target as Data_Event;
        if (GUILayout.Button("움직이는 글자 세팅", fontStyle, GUILayout.Height(30f)))
        {
            Inspector.UpdateData();
            EditorUtility.SetDirty(Inspector);
        }
        base.OnInspectorGUI();
    }
}
#endif

[CreateAssetMenu(fileName = "Data_Event", menuName = "Scriptable Objects/Data_Event")]
public class Data_Event : ScriptableObject
{
    public string id;
    public enum EventType
    {
        None = 0,
        Reward = 1,
    }
    public EventType evnetType;
    public string eventName;
    public DialogStruct eventDescription;
    public EventSelect[] eventSelect;

    [System.Serializable]
    public struct EventSelect
    {
        [TextArea]
        public string selectDialog;
        public Data_Event selectEvent;
    }

    public void UpdateData()
    {
        for (int i = 0; i < eventDescription.dialogTypes.Length; i++)
        {
            Vector2Int dialogIndex = eventDescription.dialogTypes[i].dialogIndex;
            string contents = eventDescription.contents;
            string setID = contents.Substring(dialogIndex.x, dialogIndex.y - dialogIndex.x);
            Debug.LogWarning(setID);
            eventDescription.dialogTypes[i].id = setID;
        }
    }
}
