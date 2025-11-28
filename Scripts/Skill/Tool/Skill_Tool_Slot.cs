using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static Data_Manager;

public class Skill_Tool_Slot : MonoBehaviour, IPointerClickHandler
{
    public Vector2Int slotNum;
    public bool inst;
    public Image iconImage, levelImage;
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
    public Sprite[] romanNumbers;
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
        int level = int.Parse(_Status.id.Substring(_Status.id.Length - 2, 2));
        Sprite romanImage = romanNumbers[level];
        levelImage.sprite = romanImage;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        clickHandler?.Invoke(this);
    }
}
