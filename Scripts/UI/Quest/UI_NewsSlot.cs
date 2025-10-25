using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static Data_Manager;

public class UI_NewsSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public QuestStruct questData;
    public TMP_Text title;
    public TMP_Text description;

    public delegate void DeleClick(QuestStruct _questData);
    public DeleClick deleClick;
    public DeleClick deleMouseOver;

    public void SetQuest(QuestStruct _questData)
    {
        questData = _questData;
        title.text = questData.name;
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
        deleMouseOver?.Invoke(questData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        deleMouseOver?.Invoke(null);
    }
}
