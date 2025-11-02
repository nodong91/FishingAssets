using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Data_Dialog))]
public class Data_Dialog_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        GUIStyle fontStyle = new GUIStyle(GUI.skin.button);
        fontStyle.fontSize = 15;
        fontStyle.normal.textColor = Color.yellow;

        Data_Dialog Inspector = target as Data_Dialog;
        if (GUILayout.Button("움직이는 글자 세팅", fontStyle, GUILayout.Height(30f)))
        {
            Inspector.UpdateData();
            EditorUtility.SetDirty(Inspector);
        }
        base.OnInspectorGUI();
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
            Out = 1,// 나가기
            OpenShop = 2,// 생선 상점 열기
            OpenShipyard = 3,// 조선소 상점 열기
            OpenSmuggler = 4,// 밀수꾼 상점 열기
            Upgrade = 5,// 스킬 업그레이드
            Result = 6,
            Rest = 7,// 휴식 - 잠자기
            NoticeBoard = 8,// 게시판 열기
            Tutorial = 9,// 튜토리얼 시작
        }
        public SelectType selectType;
        public Data_NPC npcData;
        public int dialogIndex;
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
