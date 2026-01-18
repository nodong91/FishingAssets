using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Data_Dialog))]
public class Data_Dialog_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUIStyle fontStyle = new GUIStyle();
        fontStyle.fontSize = 15;
        fontStyle.normal.textColor = Color.yellow;

        GUILayout.Label("선택창에 추가 가능한 ScriptableObject", fontStyle);
        GUILayout.Label("1. Data_Dialog : 다음 대화 이어가기 (선택 버튼 출력 : SelectID)");
        GUILayout.Label("2. Data_Dialog_If : 대화 상황 if 선택 출력");
        GUILayout.Label("3. Data_ItemList : 물건 교환창 오픈");
    }
}
#endif
[CreateAssetMenu(fileName = "Data_Dialog", menuName = "Scriptable Objects/Data_Dialog")]
public class Data_Dialog : ScriptableObject
{
    public Data_NPC npc;
    //public enum DialogType
    //{
    //    Dialog = 0,
    //    Select = 1,
    //    FishNews = 2,
    //}
    //public DialogType dialogType;
    [System.Serializable]
    public struct TextStruct
    {
        [TextArea]
        public string contents;
        public enum EmotionType
        {
            Normal = 0,
            Happy = 1,
            Angry = 2,
            Sad = 3,
            Surprised = 4,
            Anxious = 5,//불안해하는
        }
        public EmotionType emotionType;

        [System.Serializable]
        public struct DialogType
        {
            public string replaceText;
            public float textSize;
            public float typingSpeed;
            public Color textColor;
            [Header("Action")]
            public ActionType actionType;
            public float actionSpeed;
            public float actionInterval;
            public Vector2 actionAngle;
        }
        public DialogType[] dialogTypes;
    }
    public string selectID;
    public TextStruct[] textStruct;

    public enum ActionType
    {
        None,
        Move,
        Wave,
        Jitter
    }

    [System.Serializable]
    public struct SelectStruct
    {
        //[TextArea]
        //public string selectDialog;
        public enum SelectType
        {
            None = 0,// 닫기
            Out = 1,// 섬으로 들어가기
            FishPrice = 2,// 마을로 들어가기
            Inn = 3,// 마을로 들어가기
            Upgrade = 5,// 스킬 업그레이드
            Rest = 6,// 휴식 - 잠자기
            Street = 7,// 거리 열기
            InLand = 8,// 튜토리얼 시작
            PayBack = 9,// 돈 갚기
            GameOver = 10,
        }
        public SelectType selectType;
        //[Header(" [ 아이템 리스트 ]")]
        //public Data_ItemList itemList;// 아이템 리스트
        //[Header(" [ 대화 ]")]
        //public Data_NPC npcData;
        //public Data_Dialog dialogData;
        //[Header(" [ 조건 ]")]
        //public Data_Dialog_If dataDialogIf;
        public ScriptableObject scriptableObject;
    }
    public enum AddMoneyType
    {
        None = 0,
        Loan = 1,
    }
    public AddMoneyType addMoneyType;
    public float addMoney;
    public SelectStruct[] selectStructs;
}
