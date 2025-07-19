using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;
using static UI_Inventory_Slot;
using static UI_Main;

public class UI_Inventory : MonoBehaviour
{
    public Canvas canvas;
    public GridLayoutGroup gridLayoutGroup;
    public UI_Inventory_Slot inventorySlot;
    public float slotSize;
    public Vector2Int inventorySize;
    public Image weightSlider;
    public float currentWeight, maxWeight;
    private UI_Inventory_Slot[,] allSlots;
    private List<UI_Inventory_Slot> checkList = new List<UI_Inventory_Slot>();

    private ItemClass dragItemClass;
    public Button closeButton;

    [System.Serializable]
    public class SaveItemClass
    {
        public string id;
        public float angle;
        public Vector2Int slotNum;
        public Vector2Int[] shape;
    }
    public List<SaveItemClass> saveItems;
    Dictionary<Vector2Int, ItemClass> dictItemClass = new Dictionary<Vector2Int, ItemClass>();
    public CanvasStruct[] canvasStructs;



    public virtual void SetStart()
    {
        weightSlider.material = Instantiate(weightSlider.material);
        SetInventory();
        SetRemoveBox();

        closeButton.onClick.AddListener(delegate { OpenCanvas(false); });
        OpenCanvas(false);
    }

    public virtual void OpenCanvas(bool _open)
    {
        StartCoroutine(OpenCanvasMoving(canvasStructs, _open));
    }

    void SetInventory()
    {
        gridLayoutGroup.cellSize = new Vector2(1f, 1f) * slotSize;
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
        LoadInventory();
    }

    void DictCheck()
    {
        saveItems = new List<SaveItemClass>();
        foreach (var child in dictItemClass)
        {
            SaveItemClass dictCheck = new SaveItemClass
            {
                slotNum = child.Key,
                id = child.Value.item.id,
                angle = child.Value.angle,
                shape = child.Value.shape,
            };
            saveItems.Add(dictCheck);
        }
        SaveInventory();// 내려놓으면 저장
    }

    public void SetSlot(UI_Inventory_Slot _slot, ItemClass _itemClass)
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
        SetWeight(_itemClass.item.weight);

        dictItemClass[_slot.slotNum] = _itemClass;
        DictCheck();
    }

    void SetWeight(float _weight)
    {
        currentWeight += _weight;
        float sliderValue = currentWeight / maxWeight;
        weightSlider.material.SetFloat("_FillAmount", sliderValue);
    }

    public void SetEmpty(UI_Inventory_Slot _slot)// 비우기
    {
        dictItemClass.Remove(_slot.slotNum);
        DictCheck();

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

    UI_Inventory_Slot GetEmptySlot(ItemStruct _item)// 빈슬롯 찾기
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

    public bool BuyItem(ItemStruct _item)// 구매
    {
        UI_Inventory_Slot slot = GetEmptySlot(_item);
        if (slot == null)
        {
            // 넣을만한 빈 슬롯 없음
            Debug.LogWarning("넣을만한 빈 슬롯 없음");
            return false;
        }

        ItemClass itemClass = new ItemClass
        {
            item = _item,
            angle = 0,
            shape = _item.shape,
        };
        SetSlot(slot, itemClass);
        return true;
    }

    //===========================================================================================================================
    // Input
    //===========================================================================================================================

    void OnPointerLeftClick(UI_Inventory_Slot _slot)
    {
        Game_Manager.current.inventory.OnPointerLeftClick(_slot);
    }

    void OnPointerRightClick(UI_Inventory_Slot _slot)
    {
        Game_Manager.current.inventory.OnPointerRightClick(_slot);
    }

    void OnPointerEnter(UI_Inventory_Slot _slot)
    {
        Game_Manager.current.inventory.OnPointerEnter(_slot, dragSlotType);
    }

    void OnPointerExit()
    {
        Game_Manager.current.inventory.OnPointerExit();
    }

    void RemoveDragItem()
    {
        Game_Manager.current.inventory.OnTreshBox();
    }

    //===========================================================================================================================
    // 체크
    //===========================================================================================================================

    public bool SetCheck(UI_Inventory_Slot _slot, ItemClass _itemClass)
    {
        ClearCheckList();

        Vector2Int[] shape = _itemClass.shape;
        bool onCheck = _slot.CheckSlot();// 메인
        checkList.Add(_slot);

        if (shape == null)
            return onCheck;

        for (int i = 0; i < shape.Length; i++)
        {
            int slotX = _slot.slotNum.x + shape[i].x;
            int slotY = _slot.slotNum.y + shape[i].y;
            if (slotX < 0 || slotX >= inventorySize.x || slotY < 0 || slotY >= inventorySize.y)
            {
                onCheck = false;
            }
            else
            {
                bool temp = allSlots[slotX, slotY].CheckSlot();
                if (onCheck == true)
                    onCheck = temp;
                checkList.Add(allSlots[slotX, slotY]);
            }
        }
        return onCheck;
    }

    public void ClearCheckList()
    {
        for (int i = 0; i < checkList.Count; i++)
        {
            checkList[i].CheckOff();
        }
        checkList.Clear();
    }

    //===========================================================================================================================
    // 삭제
    //===========================================================================================================================

    public UI_Inventory_Remove_Box removeBox;
    void SetRemoveBox()
    {
        removeBox.deleRemove = RemoveDragItem;
    }

    //===========================================================================================================================
    // 저장 및 불러오기
    //===========================================================================================================================
    public string saveData = "Temp";
    void SaveInventory()
    {
        Static_JsonManager.SaveInventoryData(saveData, saveItems);
    }

    void LoadInventory()
    {
        if (Static_JsonManager.TryLoadInventoryData(saveData, out List<SaveItemClass> _data))
        {
            LoadItem(_data);
        }
    }

    void LoadItem(List<SaveItemClass> _items)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            ItemClass itemClass = new ItemClass
            {
                item = Singleton_Data.INSTANCE.GetItemStruct(_items[i].id),
                angle = _items[i].angle,
                shape = _items[i].shape,
            };// 새로운 클라스 캡슐화
            UI_Inventory_Slot slot = allSlots[_items[i].slotNum.x, _items[i].slotNum.y];
            SetSlot(slot, itemClass);
        }
    }

    //===========================================================================================================================
    // 정보 출력
    //===========================================================================================================================

    public enum DragSlotType
    {
        None,
        Shop,
        Inventory
    }
    public DragSlotType dragSlotType = DragSlotType.None;
    public UI_Inventory_Infomation infomation;
    public void SetInfomation(UI_Inventory_Slot _slot)
    {
        infomation.SetStart(_slot);
    }
}
