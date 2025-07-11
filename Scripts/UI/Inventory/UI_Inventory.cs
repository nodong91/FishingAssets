using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Data_Manager;
using static UI_Inventory_Slot;

public class UI_Inventory : MonoBehaviour, IPointerClickHandler
{
    public GridLayoutGroup gridLayoutGroup;
    public UI_Inventory_Slot inventorySlot;
    public float slotSize;
    public Vector2Int inventorySize;
    public Image weightSlider;
    public float currentWeight, maxWeight;
    private UI_Inventory_Slot[,] allSlots;
    private List<UI_Inventory_Slot> checkList = new List<UI_Inventory_Slot>();

    private ItemClass dragItemClass;
    private UI_Inventory_Slot dragSlot, enterSlot, selectedSlot;
    public Image iconImage;
    public bool onDrag, onCheck;
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

    public void SetStart()
    {
        weightSlider.material = Instantiate(weightSlider.material);
        SetInventory();
        SetInfomation();
        SetRemovePopup();

        closeButton.onClick.AddListener(CloseButton);
        CloseButton();
    }

    public void OpenCanvas()
    {
        gameObject.SetActive(true);
    }

    void CloseButton()
    {
        gameObject.SetActive(false);
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

    private void Update()// 아이템 추가 테스트
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ItemStruct temp = GetTempItem();
            AddItem(temp);
        }
    }

    public ItemStruct GetTempItem()
    {
        int randomIndex = Random.Range(1, 4);
        ItemStruct temp = Singleton_Data.INSTANCE.Dict_Fish["Fs_000" + randomIndex].itemStruct;
        return temp;
    }

    void DictCheck()
    {
        saveItems = new List<SaveItemClass>();
        foreach (var child in dictItemClass)
        {
            SaveItemClass dictCheck = new SaveItemClass
            {
                slotNum = child.Key,
                id = child.Value.item.ID,
                angle = child.Value.angle,
                shape = child.Value.shape,
            };
            saveItems.Add(dictCheck);
        }
        SaveInventory();// 내려놓으면 저장
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
        SetWeight(_itemClass.item.Weight);

        dictItemClass[_slot.slotNum] = _itemClass;
        DictCheck();
    }

    void SetWeight(float _weight)
    {
        currentWeight += _weight;
        float sliderValue = currentWeight / maxWeight;
        weightSlider.material.SetFloat("_FillAmount", sliderValue);
    }

    void SetEmpty(UI_Inventory_Slot _slot)// 비우기
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

                for (int i = 0; i < _item.Shape.Length; i++)
                {
                    int slotX = slot.slotNum.x + _item.Shape[i].x;
                    int slotY = slot.slotNum.y + _item.Shape[i].y;
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

    //===========================================================================================================================
    // 체크
    //===========================================================================================================================

    void SetCheck(UI_Inventory_Slot _slot)
    {
        ClearCheckList();

        Vector2Int[] shape = dragItemClass.shape;
        onCheck = _slot.CheckSlot();// 메인
        checkList.Add(_slot);

        if (shape == null)
            return;

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
    }

    void ClearCheckList()
    {
        for (int i = 0; i < checkList.Count; i++)
        {
            checkList[i].CheckOff();
        }
        checkList.Clear();
    }







    //===========================================================================================================================
    // 아이템 확인
    //===========================================================================================================================

    Coroutine moneyCoroutine;
    float walletMoney;
    public TMPro.TMP_Text walletText;
    public UI_Inventory_Infomation inventoryInfomation;

    void SetInfomation()
    {
        inventoryInfomation.SetInfomation();
        inventoryInfomation.deleBuyButton += BuyItem;
        inventoryInfomation.deleSellButton += SellItem;
    }

    void SetInfomationDisplay(ItemStruct _item)
    {
        inventoryInfomation.SetDisplay(_item);
    }

    public void AddItem(ItemStruct _item)
    {
        onDrag = true;
        ItemClass itemClass = new ItemClass
        {
            item = _item,
            angle = 0,
            shape = _item.Shape,
        };
        dragItemClass = itemClass;

        iconImage.sprite = _item.Icon;
        iconImage.gameObject.SetActive(true);

        if (dragCoroutine != null)
            StopCoroutine(dragCoroutine);
        dragCoroutine = StartCoroutine(MoveTest());
    }

    void BuyItem()// 구매
    {
        if (selectedSlot == null || selectedSlot.empty == true)
        {
            Debug.LogWarning("선택된 아이템 없음");
            return;
        }

        ItemClass itemClass = selectedSlot.itemClass;
        UI_Inventory_Slot slot = GetEmptySlot(selectedSlot.itemClass.item);
        if (slot == null)
        {
            // 넣을만한 빈 슬롯 없음
            Debug.LogWarning("넣을만한 빈 슬롯 없음");
            return;
        }
        Debug.LogWarning("슬롯 이름 : " + slot.name);
        SetSlot(slot, itemClass);

        if (moneyCoroutine != null)
            StopCoroutine(moneyCoroutine);
        moneyCoroutine = StartCoroutine(WalletMoney(-itemClass.item.Price));
    }

    void SellItem()// 판매
    {
        if (selectedSlot == null)
            return;

        ItemStruct item = selectedSlot.itemClass.item;
        SetWeight(-item.Weight);
        SetEmpty(selectedSlot);

        if (moneyCoroutine != null)
            StopCoroutine(moneyCoroutine);
        moneyCoroutine = StartCoroutine(WalletMoney(item.Price));
    }

    IEnumerator WalletMoney(float _money)
    {
        float prevMoney = walletMoney;
        walletMoney += _money;
        bool addMoney = (prevMoney < walletMoney);
        bool moveMoney = true;
        while (moveMoney == true)
        {
            prevMoney = Mathf.Lerp(prevMoney, walletMoney, 0.1f);
            walletText.text = Mathf.Round(prevMoney).ToString();

            if (addMoney == true)// 판매인 경우
            {
                if (prevMoney > walletMoney)
                {
                    moveMoney = false;
                }
            }
            else if (prevMoney < walletMoney)// 구매인 경우
            {
                moveMoney = false;
            }
            yield return null;
        }
    }

    //===========================================================================================================================
    // Input
    //===========================================================================================================================

    void OnPointerLeftClick(UI_Inventory_Slot _slot)
    {
        //// 아이템 체크
        //selectedSlot = _slot.baseSlot;
        //if (selectedSlot != null)
        //    SetInfomationDisplay(selectedSlot.itemClass.item);

        ClickMoveTest(_slot);
    }

    void OnPointerRightClick(UI_Inventory_Slot _slot)
    {
        if (onDrag == true)
            SetDragRotate();
        else
        {
            // 삭제
            selectedSlot = _slot.baseSlot;
            if (selectedSlot != null)
                SellItem();
        }
    }

    void OnPointerEnter(UI_Inventory_Slot _slot)
    {
        enterSlot = _slot;
        if (onDrag == true)
            SetDragMove();
    }

    void OnPointerExit()
    {
        enterSlot = null;
    }

    //===========================================================================================================================
    // 드래그
    //===========================================================================================================================
    Coroutine dragCoroutine;
    ItemClass originItemClass;
    void ClickMoveTest(UI_Inventory_Slot _slot)
    {
        if (dragCoroutine != null)
            StopCoroutine(dragCoroutine);

        if (onDrag == false)
        {
            OnDragStart(_slot);
        }
        else
        {
            OnDragEnd();
        }
    }

    void OnDragStart(UI_Inventory_Slot _slot)
    {
        if (_slot == null || _slot.baseSlot == null)
            return;

        onDrag = true;
        dragSlot = _slot.baseSlot;
        dragItemClass = dragSlot.itemClass;

        if (dragItemClass == null)
            return;

        if (originItemClass == null)// 기존 위치 저장
            originItemClass = new ItemClass();
        originItemClass.SetItemClass(dragItemClass);

        iconImage.sprite = dragItemClass.item.Icon;
        iconImage.gameObject.SetActive(dragSlot.empty == false);

        SetDragStart(dragSlot);
        dragCoroutine = StartCoroutine(MoveTest());
    }

    void OnDragEnd()
    {
        if (enterSlot == null)
        {
            // 버리기
            Debug.LogWarning("버리기");
        }
        else if (onCheck == true)
        {
            SetSlot(enterSlot, dragItemClass);
        }
        else
        {
            if (dragSlot == null)
            {
                // 이동이 아니라 얻은 아이템인 경우
            }
            else
            {
                // 기존 위치로
                dragItemClass.SetItemClass(originItemClass);// 기존 클라스로 돌림
                SetSlot(dragSlot, dragItemClass);
            }
        }
        RemoveDragItem();
        //if (dragSlot != null && enterSlot != null && onCheck == true)
        //{
        //    // 교체
        //    SetSlot(dragSlot, enterSlot.itemClass);
        //    SetSlot(enterSlot, dragItemClass);
        //}
        //else
        //{
        //    // 기존 위치로
        //    dragItemClass.SetItemClass(originItemClass);// 기존 클라스로 돌림
        //    SetSlot(dragSlot, dragItemClass);
        //}
    }

    IEnumerator MoveTest()
    {
        while (onDrag == true)
        {
            iconImage.transform.position = Input.mousePosition;
            yield return null;
        }
    }
    //===========================================================================================================================
    // 드래그
    //===========================================================================================================================

    void SetDragStart(UI_Inventory_Slot _slot)
    {
        if (_slot.empty == true)
            return;

        ItemStruct item = _slot.itemClass.item;
        SetWeight(-item.Weight);
        SetEmpty(_slot);
        SetCheck(_slot);
    }

    void SetDragMove()
    {
        // 이동 중 놓을 수 잇는 곳인지 체크
        SetCheck(enterSlot);
    }

    void SetDragRotate()
    {
        if (dragItemClass == null)
            return;

        dragItemClass.SetRotate(90f);
        SetCheck(enterSlot);
    }

    void OpenRemovePopup()
    {
        Debug.LogWarning("삭제");
        uiInventoryRemove.OpenRemovePopup();
    }

    //===========================================================================================================================
    // 삭제 확인 팝업
    //===========================================================================================================================

    public UI_Inventory_Remove uiInventoryRemove;
    void SetRemovePopup()
    {
        uiInventoryRemove.deleRemove = RemoveDragItem;
        uiInventoryRemove.CancelButton();
    }

    void RemoveDragItem()
    {
        onDrag = false;
        iconImage.gameObject.SetActive(false);

        ClearCheckList();
        dragSlot = null;
        dragItemClass = null;
    }

    //===========================================================================================================================
    // 저장 및 불러오기
    //===========================================================================================================================

    void SaveInventory()
    {
        Static_JsonManager.SaveInventoryData("InventoryData", saveItems);
    }

    void LoadInventory()
    {
        if (Static_JsonManager.TryLoadInventoryData("InventoryData", out List<SaveItemClass> _data))
        {
            LoadItem(_data);
        }
    }

    void LoadItem(List<SaveItemClass> _items)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            ItemStruct setItem = Singleton_Data.INSTANCE.Dict_Fish[_items[i].id].itemStruct;
            ItemClass itemClass = new ItemClass
            {
                item = setItem,
                angle = _items[i].angle,
                shape = _items[i].shape,
            };// 새로운 클라스 캡슐화
            UI_Inventory_Slot slot = allSlots[_items[i].slotNum.x, _items[i].slotNum.y];
            SetSlot(slot, itemClass);
        }
    }




















    public void OnPointerClick(PointerEventData eventData)
    {
        if (onDrag == true)
            OpenRemovePopup();
    }
}
