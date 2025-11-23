using UnityEngine;

[CreateAssetMenu(fileName = "Data_Dialog", menuName = "Scriptable Objects/Data_Dialog")]
public class Data_Dialog : ScriptableObject
{
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
        [TextArea]
        public string selectDialog;
        public enum SelectType
        {
            None = 0,// 닫기
            Out = 1,// 섬으로 들어가기
            Upgrade = 5,// 스킬 업그레이드
            Rest = 6,// 휴식 - 잠자기
            Street = 7,// 거리 열기
            InLand = 8,// 튜토리얼 시작
            Loan = 9,// 돈 갚기
            GameOver = 10,
        }
        public SelectType selectType;
        [Header(" [ 아이템 리스트 ]")]
        public Data_ItemList itemList;// 아이템 리스트
        [Header(" [ 대화 ]")]
        public Data_NPC npcData;
        public Data_Dialog dialogData;
    }
    public SelectStruct[] selectStructs;
}
