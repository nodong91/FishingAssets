using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_NewsDisplay : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text npcText;
    public TMP_Text descriptionText;
    public TMP_Text deadLineText;
    public GridLayoutGroup resultParent;
    //public

    public void SetDisplay(Data_Quest _quest)
    {
        titleText.text = _quest.title;
        npcText.text = _quest.npc_ID;
        descriptionText.text = _quest.description;
        deadLineText.text = (_quest.deadLine > 0) ? "남은 시간 " + _quest.deadLine + "일" : null;
        Debug.LogWarning($"Quest Display: {resultParent.name}");
        Debug.LogWarning($"Quest Display: {_quest.title}");
        Debug.LogWarning($"Quest Display: {_quest.resultData.inventorySize}");
        Debug.LogWarning($"Quest Display: {_quest.resultData.itemID.Length}");
        resultParent.gameObject.SetActive((_quest.resultData.itemID.Length > 0) == true);
    }
}
