using UnityEngine;

[CreateAssetMenu(fileName = "Data_Quest", menuName = "Scriptable Objects/Data_Quest")]
public class Data_Quest : ScriptableObject
{
    public string id;
    public string title;// 퀘스트 제목
    [TextArea]
    public string description;// 퀘스트 설명
    public string npc_ID;// 퀘스트를 주는 NPC의 ID
    public int deadLine;// 퀘스트 마감 시간
    public string[] needItems; // 퀘스트 수행에 필요한 아이템 ID들
    [Header(" [ Dialog ]")]
    public Data_Dialog.SelectStruct selectStruct;// 대화 선택 버튼
    public Data_Dialog successDialogData, failDialogData;// 퀘스트 성공 및 실패 시 대화 데이터
    [Header(" [ Result ]")]
    public ResultStruct resultData;// 퀘스트 완료 후 보상 데이터
    [System.Serializable]
    public struct ResultStruct
    {
        public Vector2Int inventorySize;// 퀘스트 완료 후 인벤토리 크기
        public string[] itemID;// 퀘스트 완료 후 보상 아이템 ID들
    }
}
