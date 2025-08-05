using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_NewsSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Data_Quest questData;
    public TMP_Text title;
    public TMP_Text description;

    public delegate void DeleClick(Data_Quest _questData);
    public DeleClick deleClick;

    public void SetQuest(Data_Quest _questData)
    {
        questData = _questData;
        title.text = questData.title;
        description.text = questData.description;
    }

    void AcceptButton()
    {

    }

    void CencelButton()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        deleClick?.Invoke(questData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        deleClick?.Invoke(questData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        deleClick?.Invoke(null);
    }
}
