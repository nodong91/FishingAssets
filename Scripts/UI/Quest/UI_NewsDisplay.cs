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
        resultParent.gameObject.SetActive((_quest.resultID.Length > 0) == true);
    }
}
