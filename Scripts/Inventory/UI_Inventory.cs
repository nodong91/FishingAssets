using NUnit.Framework.Internal;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    public GridLayoutGroup gridLayoutGroup;
    public UI_Inventory_Slot inventorySlot;
    public Vector2Int inventorySize;
    [System.Serializable]
    public struct ItemStruct
    {
        public string id;
        public Sprite icon;
        public Vector2Int[] size;
    }
    //public ItemStruct item;
    public ItemStruct a, b;
    public UI_Inventory_Slot[,] test;

    void Start()
    {
        SetInventory();
    }

    void SetInventory()
    {
        gridLayoutGroup.cellSize = new Vector2(50f, 50f);
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = inventorySize.x;

        test = new UI_Inventory_Slot[inventorySize.x, inventorySize.y];
        for (int y = 0; y < inventorySize.y; y++)
        {
            for (int x = 0; x < inventorySize.x; x++)
            {
                UI_Inventory_Slot inst = Instantiate(inventorySlot, gridLayoutGroup.transform);
                inst.SetStart(x, y);
                inst.SetEmpty();
                inst.dele_Begin = OnBeginDrag;
                inst.dele_Drag = OnDrag;
                inst.dele_End = OnEndDrag;
                inst.dele_Click = OnPointerClick;
                inst.dele_Enter = OnPointerEnter;
                inst.dele_Exit = OnPointerExit;
                test[x, y] = inst;
            }
        }

        //if (TrySetSlot(test[0, 0], a) == true)
        //{
        //    SetSlot(test[0, 0], a);
        //}
        SetSlot(test[1, 1], a);
        SetSlot(test[3, 3], b);
    }

    void SetSlot(UI_Inventory_Slot _slot, ItemStruct _item)
    {
        _slot.SetBase(_item);// 메인
        if (_item.size == null)
            return;
        Debug.LogWarning(_item.id + " : " + _item.size.Length);
        // 사이즈
        for (int i = 0; i < _item.size.Length; i++)
        {
            int slotX = _slot.x + _item.size[i].x;
            int slotY = _slot.y + _item.size[i].y;
            test[slotX, slotY].SetLink(_slot);
        }
    }

    void SetEmpty(UI_Inventory_Slot _slot)
    {
        ItemStruct item = _slot.item;
        _slot.SetEmpty();// 메인
        if (item.size == null)
            return;
        // 사이즈
        for (int i = 0; i < item.size.Length; i++)
        {
            int slotX = _slot.x + item.size[i].x;
            int slotY = _slot.y + item.size[i].y;
            test[slotX, slotY].SetEmpty();
        }
    }

    void SetCheck(UI_Inventory_Slot _slot)
    {
        ItemStruct item = dragItem;
        _slot.CheckSlot();// 메인
        if (item.size == null)
            return;
        // 사이즈
        onCheck = true;
        for (int i = 0; i < item.size.Length; i++)
        {
            int slotX = _slot.x + item.size[i].x;
            int slotY = _slot.y + item.size[i].y;
            if (slotX < 0 || slotX >= inventorySize.x || slotY < 0 || slotY >= inventorySize.y)
            {
                onCheck = false;
            }
            if (onCheck == true)
                onCheck = test[slotX, slotY].CheckSlot();
        }
    }

    void SetDragStart(UI_Inventory_Slot _slot)
    {
        if (_slot.empty == true)
            return;

        UI_Inventory_Slot slot = _slot.baseSlot;
        SetEmpty(slot);
    }

    void SetDragMove(UI_Inventory_Slot _slot)
    {
        // 이동 중 놓을 수 잇는 곳인지 체크
        SetCheck(_slot);
    }

    public ItemStruct dragItem;
    public UI_Inventory_Slot dragSlot, enterSlot;
    public Image iconImage;
    public bool onDrag, onCheck;
    void OnBeginDrag(UI_Inventory_Slot _slot)
    {
        if (_slot.empty == true)
            return;

        onDrag = true;
        dragSlot = _slot;
        dragItem = dragSlot.item;
        iconImage.sprite = dragSlot.iconImage.sprite;
        iconImage.gameObject.SetActive(dragSlot.empty == false);

        SetDragStart(_slot);
    }

    void OnDrag()
    {
        if (onDrag == true)
            iconImage.transform.position = Input.mousePosition;
    }

    void OnEndDrag()
    {
        onDrag = false;
        if (dragSlot != null && enterSlot != null && onCheck == true)
        {
            SetSlot(dragSlot, enterSlot.item);
            SetSlot(enterSlot, dragItem);
            //if (TrySetSlot(dragSlot, dragSlot.item) == true && TrySetSlot(enterSlot, enterSlot.item) == true)
            //{

            //}
            //dragSlot.SetSlot(enterSlot.item);
            //enterSlot.SetSlot(dragItem);
        }
        else
        {
            SetSlot(dragSlot, dragItem);
        }
        iconImage.gameObject.SetActive(false);
    }

    void OnPointerClick(UI_Inventory_Slot _slot)
    {
        dragSlot = _slot;
    }

    void OnPointerEnter(UI_Inventory_Slot _slot)
    {
        enterSlot = _slot;
        if (onDrag == true)
            SetDragMove(enterSlot);
    }

    void OnPointerExit()
    {
        enterSlot = null;
    }
}
