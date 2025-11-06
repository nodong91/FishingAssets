using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Data_Event_Result))]
public class Data_Event_Result_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        GUIStyle fontStyle = new GUIStyle(GUI.skin.button);
        fontStyle.fontSize = 15;
        fontStyle.normal.textColor = Color.yellow;

        Data_Event_Result Inspector = target as Data_Event_Result;
        if (GUILayout.Button("움직이는 글자 세팅", fontStyle, GUILayout.Height(30f)))
        {
            Inspector.UpdateData();
            EditorUtility.SetDirty(Inspector);
        }
        base.OnInspectorGUI();

        fontStyle = new GUIStyle();
        fontStyle.fontSize = 15;
        fontStyle.normal.textColor = Color.yellow;
        GUILayout.Label("Event Select - Select Event가 없으면 나가기", fontStyle);
    }
}
#endif

[CreateAssetMenu(fileName = "Data_Event_Result", menuName = "Scriptable Objects/Data_Event_Result")]
public class Data_Event_Result : Data_Event
{
    public enum ResultType
    {
        None,
        Reward,
        Shop,
    }
    public ResultType resultType;
    public string[] itemRewards;
}
