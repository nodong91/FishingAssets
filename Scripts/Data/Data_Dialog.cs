using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Data_Dialog))]
public class Data_Dialog_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUIStyle fontStyle = new GUIStyle(GUI.skin.button);
        fontStyle.fontSize = 15;
        fontStyle.normal.textColor = Color.yellow;

        Data_Dialog Inspector = target as Data_Dialog;
        if (GUILayout.Button("Check Action String", fontStyle, GUILayout.Height(30f)))
        {
            Inspector.UpdateData();
            EditorUtility.SetDirty(Inspector);
        }
    }
}
#endif

[CreateAssetMenu(fileName = "Data_Dialog", menuName = "Scriptable Objects/Data_Dialog")]
public class Data_Dialog : ScriptableObject
{
    public enum ActionType
    {
        None,
        Move,
        Wave,
        Jitter
    }
    [System.Serializable]
    public struct DialogStruct
    {
        [TextArea]
        public string contents;

        [System.Serializable]
        public struct DialogType
        {
            [HideInInspector] public string id;
            public Vector2Int dialogIndex;// 움직일 문장 시작과 끝 인덱스
            public float textSize;
            public float typingSpeed;
            public string textColor;
            [Header("Action")]
            public ActionType actionType;
            public float actionSpeed;
            public float actionInterval;
            public Vector2 actionAngle;
        }
        public DialogType[] dialogTypes;
    }
    public DialogStruct[] dialogStructs;
    [System.Serializable]
    public struct SelectStruct
    {
        [TextArea]
        public string selectDialog;
        public enum SelectType
        {
            None = 0,
            Out = 1,
            OpenShop = 2,
            OpenShipyard = 3,
            Quest = 4,
            Upgrade = 5,
            Result = 6,
            OpenSmuggler = 7,
        }
        public SelectType selectType;
    }
    public SelectStruct[] selectStructs;

    public void UpdateData()
    {
        for (int i = 0; i < dialogStructs.Length; i++)
        {
            for (int j = 0; j < dialogStructs[i].dialogTypes.Length; j++)
            {
                Vector2Int dialogIndex = dialogStructs[i].dialogTypes[j].dialogIndex;
                string contents = dialogStructs[i].contents;
                string setID = contents.Substring(dialogIndex.x, dialogIndex.y - dialogIndex.x);
                Debug.LogWarning(setID);
                dialogStructs[i].dialogTypes[j].id = setID;
            }
        }
    }
}
