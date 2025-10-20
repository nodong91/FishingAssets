using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Data_Manager;

public class Skill_Tool_Slot : MonoBehaviour, IPointerClickHandler
{
    public Vector2Int slotNum;
    public bool inst;
    public Image iconImage;
    public GameObject selected;
    public SkillStruct Status;

    public delegate void OnPointerClickHandler(Skill_Tool_Slot _slot);
    public OnPointerClickHandler clickHandler;

    void Start()
    {

    }

    void ButtonAction()
    {

    }

    public void SetSlot(SkillStruct _Status)
    {
        Status = _Status;
        iconImage.gameObject.SetActive(_Status.id != null && _Status.id.Length > 0);
        selected.SetActive(false);
        if (_Status.id == null || _Status.id.Length == 0)
        {
            if (inst == true)
                gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);

        Sprite iconSprite = Singleton_Data.INSTANCE.Dict_Sprite[_Status.icon];
        iconImage.sprite = iconSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        clickHandler?.Invoke(this);
    }
}
