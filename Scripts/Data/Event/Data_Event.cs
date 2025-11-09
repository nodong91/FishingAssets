using UnityEngine;
using static Data_Dialog;

[CreateAssetMenu(fileName = "Data_Event", menuName = "Scriptable Objects/Data_Event")]
public class Data_Event : ScriptableObject
{
    public TextStruct eventStruct;
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
}
