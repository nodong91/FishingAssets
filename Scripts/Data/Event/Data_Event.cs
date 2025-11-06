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

        fontStyle = new GUIStyle();
        fontStyle.fontSize = 15;
        fontStyle.normal.textColor = Color.yellow;
        GUILayout.Label("Event Select - Select Event가 없으면 (나가기) 기능", fontStyle);
    }
}
#endif

[CreateAssetMenu(fileName = "Data_Event", menuName = "Scriptable Objects/Data_Event")]
public class Data_Event : ScriptableObject
{
    public DialogStruct eventDescription;
    public EventSelect[] eventSelect;

    [System.Serializable]
    public struct EventSelect
    {
        [TextArea]
        public string selectDialog;
        public Data_Event[] selectEvent;// 결과 종류
        public Data_Event GetEventData()
        {
            // 선택 이벤트가 없는 경우 나가기
            if (selectEvent == null || selectEvent.Length == 0)
                return null;
            return selectEvent[Random.Range(0, selectEvent.Length)];
        }
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
