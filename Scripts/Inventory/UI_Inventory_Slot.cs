using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Data_Manager;
using static UnityEngine.Rendering.ProbeAdjustmentVolume;

public class UI_Inventory_Slot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool empty;
    public TMPro.TMP_Text m_Text;
    public Vector2Int slotNum;
    public Image iconImage, checkImage;
    public Sprite checkOn, checkOff;
    public UI_Inventory_Slot baseSlot;// 링크 베이스 - 다 묶이게

    public delegate void Dele_HelperSlot(UI_Inventory_Slot _slot);
    public Dele_HelperSlot dele_LeftClick, dele_RightClick;
    public Dele_HelperSlot dele_Enter;

    public delegate void Dele_Helper();
    public Dele_Helper dele_Exit;

    [System.Serializable]
    public class ItemClass
    {
        public ItemStruct item;
        public float angle;
        public Vector2Int[] shape;

        public void SetItemClass(ItemClass _item)
        {
            item = _item.item;
            angle = _item.angle;
            shape = _item.shape;
        }

        public void SetRotate(float _angle)
        {
            shape = new Vector2Int[item.Shape.Length];
            angle += _angle;
            if (angle >= 360f)
                angle = 0f;
            for (int i = 0; i < item.Shape.Length; i++)
            {
                int x = item.Shape[i].x;
                int y = item.Shape[i].y;
                switch (angle)
                {
                    case 0:

                        break;

                    case 90:
                        x = item.Shape[i].y * -1;
                        y = item.Shape[i].x;
                        break;

                    case 180:
                        x = item.Shape[i].x * -1;
                        y = item.Shape[i].y * -1;
                        break;
                    case 270:
                        x = item.Shape[i].y;
                        y = item.Shape[i].x * -1;
                        break;
                }
                Vector2Int newVector = new Vector2Int(x, y);
                shape[i] = newVector;
            }
        }
    }
    public ItemClass itemClass;

    public void SetStart(int _x, int _y)
    {
        slotNum = new Vector2Int(_x, _y);
        m_Text.text = _x + "/" + _y;
        gameObject.name = m_Text.text;
        CheckOff();
    }

    void SetSlot(ItemClass _itemClass)
    {
        empty = _itemClass == null;
        if (!empty)
            iconImage.sprite = _itemClass.item.Icon;
        iconImage.gameObject.SetActive(!empty);

        itemClass = _itemClass;
    }

    public void SetBase(ItemClass _itemClass)
    {
        baseSlot = this;
        SetSlot(_itemClass);
    }

    public void SetLink(UI_Inventory_Slot _slot)
    {
        baseSlot = _slot;
        SetSlot(_slot.itemClass);
    }

    public void SetEmpty()
    {
        baseSlot = null;
        SetSlot(default);
        iconImage.gameObject.SetActive(false);
    }

    public bool CheckSlot()
    {
        Sprite temp = (empty == true) ? checkOn : checkOff;
        {
            checkImage.sprite = temp;
        }
        checkImage.gameObject.SetActive(true);
        return (empty == true);
    }

    public void CheckOff()
    {
        checkImage.gameObject.SetActive(false);
    }














    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 액션
            dele_LeftClick?.Invoke(this);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 확인
            dele_RightClick?.Invoke(this);
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
