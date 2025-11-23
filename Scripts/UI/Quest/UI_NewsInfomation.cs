using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;

public class UI_NewsInfomation : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text npcText;
    public TMP_Text descriptionText;
    public TMP_Text deadLineText;
    public GridLayoutGroup resultParent;

    public void SetDisplay(QuestStruct _quest)
    {
        Debug.LogWarning($"Quest 클릭: {_quest.title}");
        titleText.text = _quest.title;
        npcText.text = _quest.client;
        descriptionText.text = _quest.description;
        deadLineText.text = (_quest.deadLine > 0) ? "남은 시간 " + _quest.deadLine + "일" : null;
        resultParent.gameObject.SetActive((_quest.result?.Length > 0) == true);
    }
}
