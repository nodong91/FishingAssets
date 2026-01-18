using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Data_Manager;

public class UI_Inventory_Slot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool empty, destroy;
    public Vector2Int slotNum;

    public Image checkImage;
    public Image destroyImage;
    private Image itemImage;
    public Image SetSlotImage { set { itemImage = value; } }
    public Image GetSlotImage { get { return itemImage; } }

    UI_Inventory_Slot linkSlot;// 링크 베이스 - 다 묶이게
    public TMPro.TMP_Text adsfadsf;

    public UI_Inventory_Slot GetLinkSlot { get { return linkSlot; } }

    public delegate void Dele_HelperSlot(UI_Inventory_Slot _slot);
    public Dele_HelperSlot dele_LeftClick, dele_RightClick;
    public Dele_HelperSlot dele_Enter;

    public delegate void Dele_Helper();
    public Dele_Helper dele_Exit;

    public ItemInInventory itemInInventory;
    [System.Serializable]
    public class ItemInInventory
    {
        public ItemStruct item;
        public float angle;
        public Vector2Int[] shape;
        public int acquisition;// 입수 날짜

        public void SetSaveItem(ItemInInventory _item)
        {
            item = _item.item;
            angle = _item.angle;
            shape = _item.shape;
            acquisition = _item.acquisition;
        }

        public void SetRotate(float _angle)
        {
            shape = new Vector2Int[item.shape.Length];
            angle += _angle;
            if (angle >= 360f)
                angle = 0f;
            for (int i = 0; i < item.shape.Length; i++)
            {
                int x = item.shape[i].x;
                int y = item.shape[i].y;
                switch (angle)
                {
                    case 0:

                        break;

                    case 90:
                        x = item.shape[i].y;
                        y = item.shape[i].x * -1;
                        break;

                    case 180:
                        x = item.shape[i].x * -1;
                        y = item.shape[i].y * -1;
                        break;
                    case 270:
                        x = item.shape[i].y * -1;
                        y = item.shape[i].x;
                        break;
                }
                Vector2Int newVector = new Vector2Int(x, y);
                shape[i] = newVector;
            }
        }
    }

    public void SetStart(int _x, int _y)
    {
        slotNum = new Vector2Int(_x, _y);
        adsfadsf.text = slotNum.ToString();
        CheckOff();

        destroy = false;
        destroyImage.gameObject.SetActive(false);
    }

    void SetSlot(ItemInInventory _item)
    {
        empty = (_item == null);
        itemInInventory = _item;
        CheckOff();
    }

    public void SetBase(ItemInInventory _item)
    {
        linkSlot = this;
        SetSlot(_item);
    }

    public void SetLink(UI_Inventory_Slot _slot)
    {
        linkSlot = _slot;
        SetSlot(_slot.itemInInventory);
    }

    public void SetEmpty()
    {
        linkSlot = null;
        SetSlot(null);
    }

    public bool CheckSlot()
    {
        bool check = empty == true && destroy == false;
        Color checkColor = (check) ? Color.white : Color.red;
        {
            checkImage.color = checkColor;
        }
        checkImage.gameObject.SetActive(true);
        return check;
    }

    public void CheckOff()
    {
        checkImage.color = Color.white;
        checkImage.gameObject.SetActive(empty == false);
    }

    public void SetQuestSlot()
    {
        checkImage.color = Color.gray;
        checkImage.gameObject.SetActive(true);
    }

    public void DestroySlot()
    {
        destroy = true;
        CheckOff();
        destroyImage.gameObject.SetActive(true);
    }

    public void FixSlot()
    {
        destroy = false;
        destroyImage.gameObject.SetActive(false);
    }

    //===========================================================================================================================
    // 인풋 컨트롤
    //===========================================================================================================================
    Coroutine clicking;
    int clickAmount = 0;
    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                if (clicking != null)
                    StopCoroutine(clicking);
                clicking = StartCoroutine(OnDoubleClick());
                break;
            case PointerEventData.InputButton.Right:
                // 확인
                dele_RightClick?.Invoke(this);
                break;
            case PointerEventData.InputButton.Middle:
                break;
        }
    }
  
    IEnumerator OnDoubleClick()
    {
        clickAmount++;
        yield return new WaitForSeconds(0.15f);
        if (clickAmount > 1)
        {
            // 확인
            dele_RightClick?.Invoke(this);
        }
        else
        {
            // 액션
            dele_LeftClick?.Invoke(this);
        }
        clickAmount = 0;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        dele_Enter?.Invoke(this);
        if (empty == false)
        {
            Cursor_Manager.current?.OnMouseOver();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        dele_Exit?.Invoke();
        Cursor_Manager.current?.OnMouseExit();
    }
}
