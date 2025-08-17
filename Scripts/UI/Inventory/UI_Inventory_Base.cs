using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;
using static UI_Inventory_Slot;

public class UI_Inventory_Base : MonoBehaviour
{
    public enum SlotType
    {
        None,
        Shop,
        Shipyard,
        Storage,
        Result,
        MyBox,
    }
    public SlotType slotType = SlotType.None;
    public string saveData { get; set; }

    public Canvas canvas;
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    public GridLayoutGroup gridLayoutGroup;
    public UI_Inventory_Slot inventorySlot;
    float slotSize;
    public float SetSlotSize { set { slotSize = value; } }
    public Vector2Int inventorySize;

    private UI_Inventory_Slot[,] allSlots;
    Queue<UI_Inventory_Slot> slotPool = new Queue<UI_Inventory_Slot>();
    private List<UI_Inventory_Slot> checkList = new List<UI_Inventory_Slot>();
    Queue<Image> iconQueue = new Queue<Image>();
    public RectTransform iconParent;
    public RectTransform infomationRect;
    Coroutine loadingItem;
    Dictionary<Vector2Int, ItemClass> dictItemClass = new Dictionary<Vector2Int, ItemClass>();

    protected virtual void SetWeight(float _weight) { }

    public virtual void SetStart()
    {
        OpenCanvas(false);
    }

    public void SetInventoryItem(string _saveData)// 기존 비우고 다시 세팅
    {
        saveData = _saveData;
        if (loadingItem != null)
            StopCoroutine(loadingItem);
        loadingItem = StartCoroutine(SetLoadingItem());
    }

    IEnumerator SetLoadingItem()
    {
        EmptyInventory();
        LoadInventory();
        while (saveInventoryData == null)
            yield return null;

        SetInventorySlot(GetSaveInventoryData.invenSize);// 데이터 불러온 이후
        yield return null;

        LoadItem(saveInventoryData);
        SetLoadDestroy();
    }

    public virtual void OpenCanvas(bool _open)
    {
        StaticOpenCanvas.deleEndOpen = EndOpenCanvas;
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
    }

    void EndOpenCanvas()
    {
        Static_JsonManager.SaveInventory(saveData, GetSaveInventoryData); ;// 창닫힐 때 저장
    }

    public void EmptyInventory()
    {
        if (allSlots == null)
            return;

        foreach (var item in allSlots)
        {
            if (item.empty == true)
                continue;

            UI_Inventory_Slot slot = item.GetLinkSlot;
            SlotEmpty(slot);
        }
    }

   public void SetInventorySlot(Vector2Int _size)
    {
        if (allSlots != null)
        {
            foreach (var slot in allSlots)
            {
                slot.gameObject.SetActive(false);
                slotPool.Enqueue(slot);
            }
        }

        inventorySize = _size;
        gridLayoutGroup.cellSize = new Vector2(1f, 1f) * slotSize;
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = inventorySize.x;

        allSlots = new UI_Inventory_Slot[inventorySize.x, inventorySize.y];
        for (int y = 0; y < inventorySize.y; y++)
        {
            for (int x = 0; x < inventorySize.x; x++)
            {
                UI_Inventory_Slot inst = TrySlotPool();
                inst.SetStart(x, y);
                inst.SetEmpty();
                inst.dele_LeftClick = OnPointerLeftClick;
                inst.dele_RightClick = OnPointerRightClick;
                inst.dele_Enter = OnPointerEnter;
                inst.dele_Exit = OnPointerExit;
                allSlots[x, y] = inst;
                inst.transform.SetAsLastSibling();
            }
        }
    }

    UI_Inventory_Slot TrySlotPool()
    {
        if (slotPool.Count > 0)
        {
            UI_Inventory_Slot slot = slotPool.Dequeue();
            slot.gameObject.SetActive(true);
            return slot;
        }
        UI_Inventory_Slot inst = Instantiate(inventorySlot, gridLayoutGroup.transform);
        return inst;
    }

    public void SlotEmpty(UI_Inventory_Slot _slot)// 비우기
    {
        dictItemClass.Remove(_slot.slotNum);
        SaveDictionary();

        SetWeight(-_slot.itemClass.item.weight);// 무게 빼기
        iconQueue.Enqueue(_slot.GetSlotImage);
        _slot.GetSlotImage.gameObject.SetActive(false);

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

    public void SetSlot(UI_Inventory_Slot _slot, ItemClass _itemClass)
    {
        dictItemClass[_slot.slotNum] = _itemClass;
        SaveDictionary();

        _slot.SetBase(_itemClass);// 메인 슬롯
        if (_itemClass != null)// 비워져 있는지
        {
            SetWeight(_itemClass.item.weight);// 무게 세팅

            Image iconImage = IconPool();
            iconImage.transform.SetPositionAndRotation(_slot.transform.position, Quaternion.Euler(0f, 0f, _itemClass.angle));
            SetImage(iconImage, _itemClass.item);
            _slot.SetSlotImage = iconImage;// 이미지 세팅
            // 슬롯 세팅
            Vector2Int[] shape = _itemClass.shape;
            if (shape != null)
            {
                for (int i = 0; i < shape.Length; i++)
                {
                    int slotX = _slot.slotNum.x + shape[i].x;
                    int slotY = _slot.slotNum.y + shape[i].y;
                    allSlots[slotX, slotY].SetLink(_slot);
                }
            }
        }
    }

    Image IconPool()
    {
        if (iconQueue.Count > 0)
        {
            return iconQueue.Dequeue();
        }
        Image baseImage = Game_Manager.current.GetInventory.iconImage;
        baseImage.color = Color.white;
        return Instantiate(baseImage, iconParent);
    }

    void SetImage(Image _image, ItemStruct _itemStruct)
    {
        Vector2 size = new Vector2(_itemStruct.iconSize.x, _itemStruct.iconSize.y);
        Vector2 pivot = new Vector2(_itemStruct.iconSize.w, _itemStruct.iconSize.z);
        _image.sprite = _itemStruct.icon;
        _image.rectTransform.sizeDelta = size * slotSize;
        _image.rectTransform.pivot = pivot;
        _image.gameObject.SetActive(true);
    }

    public UI_Inventory_Slot GetEmptySlot(ItemStruct _item)// 빈슬롯 찾기
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

    public bool AddItem(ItemStruct _item)// 구매
    {
        UI_Inventory_Slot slot = GetEmptySlot(_item);// 아이템 넣을 수 있는 칸 찾기
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
            acquisition = Game_Manager.current.GetTimeUI.day,
        };
        SetSlot(slot, itemClass);
        return true;
    }

    //===========================================================================================================================
    // Input
    //===========================================================================================================================

    void OnPointerLeftClick(UI_Inventory_Slot _slot)
    {
        Game_Manager.current.GetInventory.OnPointerLeftClick(_slot);
    }

    void OnPointerRightClick(UI_Inventory_Slot _slot)
    {
        Game_Manager.current.GetInventory.OnPointerRightClick(_slot);
    }

    void OnPointerEnter(UI_Inventory_Slot _slot)
    {
        Game_Manager.current.GetInventory.OnPointerEnter(_slot, slotType);
    }

    void OnPointerExit()
    {
        Game_Manager.current.GetInventory.OnPointerExit();
    }

    public void RemoveDragItem()
    {
        Game_Manager.current.GetInventory.OffDragReset();
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
                bool linkCheck = allSlots[slotX, slotY].CheckSlot();
                if (onCheck == true)
                    onCheck = linkCheck;
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

    public bool CheckItem(string _itemID, out UI_Inventory_Slot _slot)
    {
        foreach (var slot in allSlots)
        {
            if (slot.itemClass?.item.id == _itemID)// 아이템 ID가 같은지
            {
                _slot = slot;
                return true;
            }
        }
        _slot = null;
        return false;
    }









    //===========================================================================================================================
    // 저장 및 불러오기
    //===========================================================================================================================

    [System.Serializable]
    public class SaveItemClass
    {
        public string id;
        public float angle;
        public Vector2Int slotNum;
        public Vector2Int[] shape;
        public int acquisition;
    }
    Static_JsonManager.InventoryData saveInventoryData;
    public Static_JsonManager.InventoryData GetSaveInventoryData { get { return saveInventoryData; } }
    Vector2Int defaultInvenSize = new Vector2Int(4, 4);

    void SaveDictionary()
    {
        List<SaveItemClass> saveItems = new List<SaveItemClass>();
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

        saveInventoryData = new Static_JsonManager.InventoryData
        {
            lastSetDay = Game_Manager.current.GetTimeUI.day,
            invenSize = inventorySize,
            invenClass = saveItems,
        };
    }

    public void LoadInventory()
    {
        if (Static_JsonManager.TryLoadInventory(saveData, out Static_JsonManager.InventoryData _data))
        {
            saveInventoryData = _data;
        }
        else
        {
            saveInventoryData = new Static_JsonManager.InventoryData
            {
                lastSetDay = -1,
                invenSize = defaultInvenSize,
                invenClass = new List<SaveItemClass>(),
            };
            //Static_JsonManager.SaveInventory(saveData, saveInventoryData); ;// 디폴트로 저장
        }
    }

    void LoadItem(Static_JsonManager.InventoryData _data)
    {
        inventorySize = _data.invenSize;
        for (int i = 0; i < _data.invenClass.Count; i++)
        {
            ItemClass itemClass = new ItemClass
            {
                item = Singleton_Data.INSTANCE.GetItemStruct(_data.invenClass[i].id),
                angle = _data.invenClass[i].angle,
                shape = _data.invenClass[i].shape,
                acquisition = _data.invenClass[i].acquisition,
            };// 새로운 클라스 캡슐화
            UI_Inventory_Slot slot = allSlots[_data.invenClass[i].slotNum.x, _data.invenClass[i].slotNum.y];
            SetSlot(slot, itemClass);
        }
    }












    void SetLoadDestroy()
    {
        if (destroySlot == null)
            return;

        for (int i = 0; i < destroySlot.Count; i++)
        {
            int x = destroySlot[i].x;
            int y = destroySlot[i].y;
            allSlots[x, y].DestroySlot();
        }
    }

    public List<Vector2Int> destroySlot = new List<Vector2Int>();
    public List<Vector2Int> GetDestroySlot { get { return destroySlot; } }
    public void DistroySlot()// 슬롯 부수기
    {
        bool find = false;
        while (find == false)
        {
            int x = Random.Range(0, inventorySize.x);
            int y = Random.Range(0, inventorySize.y);

            if (allSlots[x, y].destroy == false)
            {
                find = true;
                UI_Inventory_Slot linkSlot = allSlots[x, y].GetLinkSlot;
                if (linkSlot?.empty == false)
                {
                    SlotEmpty(linkSlot);
                }
                allSlots[x, y].DestroySlot();// 슬롯 부수기
                destroySlot.Add(new Vector2Int(x, y));
            }
        }
    }

    public void FixSlot(UI_Inventory_Slot _slot)// 슬롯 복구
    {
        _slot.FixSlot();
        destroySlot.Remove(_slot.slotNum);
    }

    public void FixAll()// 모든 슬롯 복구
    {
        for (int i = 0; i < destroySlot.Count; i++)
        {
            int x = destroySlot[i].x;
            int y = destroySlot[i].y;
            allSlots[x, y].FixSlot();
        }
        destroySlot.Clear();
    }
}
