using UnityEngine;

[CreateAssetMenu(fileName = "Data_Event_Result", menuName = "Scriptable Objects/Data_Event_Result")]
public class Data_Event_Result : Data_Event
{
    [Header(" [ 보상 아이템 ]")]
    public Data_ItemList itemList;// 보상 아이템
    [Header(" [ 엔피씨 대화 ]")]
    public Data_NPC npcData;// 엔피씨
    public Data_Dialog dialogData;// 엔피씨 대화 넘버
    [Header(" [ 돈 추가 ]")]
    public int addMoney;// 돈 추가
}
