using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;
using static UI_Inventory_Slot;
using static UI_Main;

public class UI_Shop : MonoBehaviour
{
    public CanvasStruct[] canvasStructs;
    public Button outButton;
    public GridLayoutGroup gridLayoutGroup;
    public Vector2Int inventorySize;
    public float slotSize;
    private UI_Inventory_Slot[,] allSlots;
    public UI_Inventory_Slot inventorySlot;

    public Data_Shop shopItem;

    public void SetStart()
    {
        outButton.onClick.AddListener(OutCanvas);
        OpenCanvas(false);

        SetInventory();
    }

    public void OpenCanvas(bool _open)
    {
        Game_Manager.current.inventory.OpenCanvas(_open);
        StartCoroutine(OpenCanvasMoving(canvasStructs, _open));

        if (_open == true)
            ChangeMode(UI_Inventory.BargainType.Shop);
        else
            ChangeMode(UI_Inventory.BargainType.None);
    }

    void ChangeMode(UI_Inventory.BargainType _mode)
    {
        Game_Manager.current.inventory.SetBargainType = _mode;
    }

    void OutCanvas()
    {
        OpenCanvas(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetFixedItem();
            SetRandomItem();
        }
    }

    void SetFixedItem()
    {
        string[] setID = shopItem.fixedID;
        for (int i = 0; i < setID.Length; i++)
        {
            ItemStruct item = Singleton_Data.INSTANCE.GetItemStruct(setID[i]);
            if (AddItem(item) == false)
            {
                break;// 빈칸이 없으면 그만
            }
        }
    }

    void SetRandomItem()
    {
        List<string> setID = new List<string>(shopItem.randomID);
        setID = P01_Utility.ShuffleList(setID, 0);

        // 아이템 반복 되지 않게 세팅
        int amount = Random.Range(0, setID.Count);
        for (int i = 0; i < amount; i++)
        {
            ItemStruct item = Singleton_Data.INSTANCE.GetItemStruct(setID[i]);
            if (AddItem(item) == false)
            {
                break;// 빈칸이 없으면 그만
            }
        }
    }

    void SetInventory()
    {
        gridLayoutGroup.cellSize = Vector2.one * slotSize;
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = inventorySize.x;

        allSlots = new UI_Inventory_Slot[inventorySize.x, inventorySize.y];
        for (int y = 0; y < inventorySize.y; y++)
        {
            for (int x = 0; x < inventorySize.x; x++)
            {
                UI_Inventory_Slot inst = Instantiate(inventorySlot, gridLayoutGroup.transform);
                inst.SetStart(x, y);
                inst.SetEmpty();
                inst.dele_LeftClick = OnPointerLeftClick;
                inst.dele_RightClick = OnPointerRightClick;
                inst.dele_Enter = OnPointerEnter;
                inst.dele_Exit = OnPointerExit;
                allSlots[x, y] = inst;
            }
        }
    }

    bool AddItem(ItemStruct _item)
    {
        UI_Inventory_Slot slot = GetEmptySlot(_item);
        if (slot == null)
        {
            // 넣을만한 빈 슬롯 없음
            Debug.LogWarning("넣을만한 빈 슬롯 없음");
            return false;
        }
        Debug.LogWarning("슬롯 이름 : " + slot.name);
        float setAngle = Random.Range(0, 4) * 90f;
        ItemClass itemClass = new ItemClass
        {
            item = _item,
            angle = setAngle,
            shape = _item.shape,
        };
        SetSlot(slot, itemClass);
        return true;
    }

    void SetEmpty(UI_Inventory_Slot _slot)// 비우기
    {
        Vector2Int[] shape = _slot.itemClass.shape;
        _slot.SetEmpty();// 메인 슬롯 비우기
        if (shape == null)
            return;
        // 사이즈
        for (int i = 0; i < shape.Length; i++)
        {
            int slotX = _slot.slotNum.x + shape[i].x;
            int slotY = _slot.slotNum.y + shape[i].y;
            allSlots[slotX, slotY].SetEmpty();
        }
    }

    UI_Inventory_Slot GetEmptySlot(ItemStruct _item)
    {
        for (int y = 0; y < inventorySize.y; y++)
        {
            for (int x = 0; x < inventorySize.x; x++)
            {
                bool empty = true;
                UI_Inventory_Slot slot = allSlots[x, y];
                if (slot.empty == false)
                {
                    continue;
                }

                for (int i = 0; i < _item.shape.Length; i++)
                {
                    int slotX = slot.slotNum.x + _item.shape[i].x;
                    int slotY = slot.slotNum.y + _item.shape[i].y;
                    if (slotX < 0 || slotX >= inventorySize.x || slotY < 0 || slotY >= inventorySize.y)
                    {
                        empty = false;
                        break;
                    }
                    else
                    {
                        bool temp = allSlots[slotX, slotY].empty;
                        if (temp == false)
                        {
                            empty = false;
                            break;
                        }
                    }
                }
                if (empty == true)
                    return slot;
            }
        }
        return null;
    }

    void SetSlot(UI_Inventory_Slot _slot, ItemClass _itemClass)
    {
        _slot.SetBase(_itemClass);// 메인 슬롯
        if (_itemClass == null)
            return;

        Vector2Int[] shape = _itemClass.shape;
        // 사이즈
        for (int i = 0; i < shape.Length; i++)
        {
            int slotX = _slot.slotNum.x + shape[i].x;
            int slotY = _slot.slotNum.y + shape[i].y;
            allSlots[slotX, slotY].SetLink(_slot);
        }
    }

    public UI_Inventory_Slot prevSlot;
    public ItemClass prevItemClass;
    public void ReturnPrevPosition(bool _return)
    {
        if (_return == true)
            SetSlot(prevSlot, prevItemClass);// 원래 위치로 돌리기

        prevSlot = null;
        prevItemClass = null;
        Game_Manager.current.inventory.TryDragSlotType = UI_Inventory.DragSlotType.None;
    }

    void OnPointerLeftClick(UI_Inventory_Slot _slot)
    {
        UI_Inventory inventory = Game_Manager.current.inventory;
        if (inventory.TryDragSlotType == UI_Inventory.DragSlotType.Shop)//상점 아이템을 상점에 놓은 경우
        {
            inventory.RemoveDragItem();
            ReturnPrevPosition(true);
            return;
        }

        if (inventory.TryDragSlotType == UI_Inventory.DragSlotType.Inventory)// 드래그로 판매
        {
            inventory.ShopDragSellItem();
            ReturnPrevPosition(false);
            return;
        }

        if (_slot.empty == true)
            return;

        // 상점 물건 들기
        prevSlot = _slot.GetLinkSlot;
        prevItemClass = _slot.itemClass;

        ItemStruct item = _slot.itemClass.item;
        inventory.AddItem(item);
        inventory.TryDragSlotType = UI_Inventory.DragSlotType.Shop;
        SetEmpty(_slot.GetLinkSlot);
    }

    void OnPointerRightClick(UI_Inventory_Slot _slot)
    {
        if (_slot.empty == true || prevSlot != null)
            return;

        ItemStruct item = _slot.itemClass.item;
        if (Game_Manager.current.inventory.BuyItem(item) == true)// 판매 성공
        {
            SetEmpty(_slot.GetLinkSlot);// 슬롯 비우기
        }
    }

    void OnPointerEnter(UI_Inventory_Slot _slot)
    {

    }

    void OnPointerExit()
    {

    }
}
