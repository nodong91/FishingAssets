using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inventory_Slot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool empty;
    public TMPro.TMP_Text m_Text;
    public int x, y;
    public Image iconImage, checkImage;
    public UI_Inventory_Slot baseSlot;// 링크 베이스 - 다 묶이게

    public delegate void Dele_HelperSlot(UI_Inventory_Slot _slot);
    public Dele_HelperSlot dele_Click;
    public Dele_HelperSlot dele_Enter;
    public Dele_HelperSlot dele_Begin;

    public delegate void Dele_Helper();
    public Dele_Helper dele_Drag;
    public Dele_Helper dele_End;
    public Dele_Helper dele_Exit;
    public UI_Inventory.ItemStruct item;

    public void SetStart(int _x, int _y)
    {
        x = _x;
        y = _y;
        m_Text.text = _x + "/" + _y;
        gameObject.name = m_Text.text;
    }

    void SetSlot(UI_Inventory.ItemStruct _item)
    {
        item = _item;
        empty = item.id == null;
        if (!empty)
            iconImage.sprite = item.icon;
        iconImage.gameObject.SetActive(!empty);
    }

    public void SetBase(UI_Inventory.ItemStruct _item)
    {
        baseSlot = this;
        SetSlot(_item);
    }

    public void SetLink(UI_Inventory_Slot _slot)
    {
        baseSlot = _slot;
        SetSlot(_slot.item);
    }

    public void SetEmpty()
    {
        baseSlot = null;
        SetSlot(default);
        iconImage.gameObject.SetActive(false);
    }

    public bool CheckSlot()
    {
        if (empty == true)
        {

        }
        else
        {

        }
        checkImage.gameObject.SetActive(empty == false);
        return (empty == true);
    }

















    public void OnBeginDrag(PointerEventData eventData)
    {
        dele_Begin?.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        dele_Drag?.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dele_End?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 액션
            dele_Click?.Invoke(this);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 확인
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        dele_Enter?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        dele_Exit?.Invoke();
    }
}
