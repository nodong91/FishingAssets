using UnityEngine;

public class UI_QuestSlot : MonoBehaviour
{
    public TMPro.TMP_Text slotTitle;
    public Data_Quest questData;

    public delegate void DeleSlotClick(UI_QuestSlot slot);
    public DeleSlotClick slotClick;

    public Custom_Button clickButton;

    public void SetStart()
    {
        clickButton.SetButton(delegate { SelectedSlot(true); });
    }

    public void SetQuestSlot(Data_Quest _questData)
    {
        questData = _questData;
        slotTitle.text = _questData.title;
        SelectedSlot(false);
    }

    public void SelectedSlot(bool _on)
    {
        if (_on == true)
            slotClick?.Invoke(this);
        clickButton.buttonImage.gameObject.SetActive(_on);
    }
}
