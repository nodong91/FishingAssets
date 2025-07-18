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
    public CanvasStruct[] canvasStructs;

    public enum BargainType
    {
        None,
        Shop
    }
    public BargainType bargainType = BargainType.None;
    public BargainType SetBargainType { set { bargainType = value; } }

    public void SetStart()
    {
        weightSlider.material = Instantiate(weightSlider.material);
        SetInventory();
        SetRemoveBox();

        closeButton.onClick.AddListener(delegate { OpenCanvas(false); });
        OpenCanvas(false);
    }

    public void OpenCanvas(bool _open)
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

    //===========================================================================================================================
    // 아이템 확인
    //===========================================================================================================================

    Coroutine moneyCoroutine;
    float moneyValue;
    public TMPro.TMP_Text walletText;

    public void AddItem(ItemStruct _item)
    {
        onDrag = true;
        ItemClass itemClass = new ItemClass
        {
            item = _item,
            angle = 0,
            shape = _item.shape,
        };
        dragItemClass = itemClass;

        iconImage.sprite = _item.icon;
        iconImage.gameObject.SetActive(true);

        if (dragCoroutine != null)
            StopCoroutine(dragCoroutine);
        dragCoroutine = StartCoroutine(MoveTest());
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

        if (moneyValue < _item.price)// 돈이 있는지 확인
            return false;

        Debug.LogWarning("슬롯 이름 : " + slot.name);
        ItemClass itemClass = new ItemClass
        {
            item = _item,
            angle = 0,
            shape = _item.shape,
        };
        SetSlot(slot, itemClass);

        if (moneyCoroutine != null)
            StopCoroutine(moneyCoroutine);
        moneyCoroutine = StartCoroutine(WalletMoney(-_item.price));
        return true;
    }

    public void ShopDragBuyItem(ItemStruct _item)// 샵에 드래그로 구매
    {
        if (moneyCoroutine != null)
            StopCoroutine(moneyCoroutine);
        moneyCoroutine = StartCoroutine(WalletMoney(-_item.price));
    }

    void SellItem(UI_Inventory_Slot _slot)// 판매
    {
        if (_slot == null)
            return;

        ItemStruct item = _slot.itemClass.item;
        SetWeight(-item.weight);
        SetEmpty(_slot);

        if (moneyCoroutine != null)
            StopCoroutine(moneyCoroutine);
        moneyCoroutine = StartCoroutine(WalletMoney(item.price));
    }

    public void ShopDragSellItem()// 샵에 드래그로 팔기
    {
        if (moneyCoroutine != null)
            StopCoroutine(moneyCoroutine);
        moneyCoroutine = StartCoroutine(WalletMoney(dragItemClass.item.price));
        RemoveDragItem();
    }

    IEnumerator WalletMoney(float _money)
    {
        float prevMoney = moneyValue;
        moneyValue += _money;
        bool addMoney = (prevMoney < moneyValue);
        bool moveMoney = true;
        while (moveMoney == true)
        {
            prevMoney = Mathf.Lerp(prevMoney, moneyValue, 0.1f);
            walletText.text = Mathf.Round(prevMoney).ToString();

            if (addMoney == true)// 판매인 경우
            {
                if (prevMoney > moneyValue)
                {
                    moveMoney = false;
                }
            }
            else if (prevMoney < moneyValue)// 구매인 경우
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
        // 아이템 체크
        ClickMoveTest(_slot);
    }

    void OnPointerRightClick(UI_Inventory_Slot _slot)
    {
        if (onDrag == true)// 드래그 중일 때
            SetDragRotate();
        else
        {
            switch (bargainType)
            {
                case BargainType.None:
                    ActionSlot(_slot);
                    break;
                case BargainType.Shop:
                    // 아이템 판매
                    selectedSlot = _slot.GetLinkSlot;
                    if (selectedSlot != null)
                        SellItem(selectedSlot);
                    break;
                default:

                    break;
            }
        }
    }

    void ActionSlot(UI_Inventory_Slot _slot)// 상점이 아닌 곳에서 오른 클릭
    {
        if (_slot.empty == false)
        {
            switch (_slot.itemClass.item.itemType)
            {
                case ItemStruct.ItemType.Equip:// 장비 장착
                    Game_Manager.current.statusUI.AddEquip(_slot.itemClass.item.id);
                    Debug.LogWarning("장착 " + _slot.itemClass.item.id);
                    break;

                case ItemStruct.ItemType.Fish:

                    break;

                default:

                    break;
            }
        }
    }

    void OnPointerEnter(UI_Inventory_Slot _slot)
    {
        enterSlot = _slot;
        if (onDrag == true)
            SetDragMove();
        else
            SetInfomation(enterSlot);
    }

    void OnPointerExit()
    {
        enterSlot = null;
        SetInfomation(null);
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
            if (_slot == null || _slot.GetLinkSlot == null)
                return;

            OnDragStart(_slot);
            dragSlotType = DragSlotType.Inventory;
        }
        else
        {
            OnDragEnd();
            dragSlotType = DragSlotType.None;
        }
    }

    void OnDragStart(UI_Inventory_Slot _slot)
    {
        SetInfomation(null);// 인포메이션 제거

        onDrag = true;
        dragSlot = _slot.GetLinkSlot;
        dragItemClass = dragSlot.itemClass;

        if (dragItemClass == null)
            return;

        if (originItemClass == null)// 기존 위치 저장
            originItemClass = new ItemClass();
        originItemClass.SetItemClass(dragItemClass);

        iconImage.sprite = dragItemClass.item.icon;
        iconImage.gameObject.SetActive(dragSlot.empty == false);

        SetCheck(_slot);
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
            if (bargainType == BargainType.Shop)// 샵의 슬롯을 들었을 경우 구매
            {
                if (moneyValue < dragItemClass.item.price)// 돈이 있는지 확인
                {
                    Game_Manager.current.shopUI.ReturnPrevPosition(true);// 상점 물건 원래 위치로
                }
                else
                {
                    Game_Manager.current.shopUI.ReturnPrevPosition(false);// 상점 물건 제거
                    SetSlot(enterSlot, dragItemClass);
                    ShopDragBuyItem(dragItemClass.item);// 구매 성공
                }
                Debug.LogWarning("구매 "+ (moneyValue < dragItemClass.item.price));
            }
        }
        else
        {
            if (dragSlot == null)
            {
                // 획득 아이템일 때 싶패한 경우
                if (bargainType == BargainType.Shop)
                {
                    Game_Manager.current.shopUI.ReturnPrevPosition(true);// 원래 위치로
                }
            }
            else
            {
                // 기존 위치로
                dragItemClass.SetItemClass(originItemClass);// 기존 클라스로 돌림
                SetSlot(dragSlot, dragItemClass);
                Debug.LogWarning("hjofaisjdofjajd");
            }
        }
        RemoveDragItem();
    }

    IEnumerator MoveTest()
    {
        while (onDrag == true)
        {
            iconImage.transform.position = Input.mousePosition;
            yield return null;
        }
    }

    void SetDragStart(UI_Inventory_Slot _slot)
    {
        if (_slot.empty == true)
            return;

        ItemStruct item = _slot.itemClass.item;
        SetWeight(-item.weight);
        SetEmpty(_slot);
        Debug.LogWarning("SetDragStart");
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
    // 삭제
    //===========================================================================================================================

    public UI_Inventory_Remove_Box removeBox;
    void SetRemoveBox()
    {
        removeBox.deleRemove = RemoveDragItem;
    }

    public void RemoveDragItem()// 아이템 제거
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

    public UI_Inventory_Infomation infomation;
    void SetInfomation(UI_Inventory_Slot _slot)
    {
        infomation.SetStart(_slot);
    }














    private void Update()// 아이템 추가 테스트
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            int randomIndex = Random.Range(1, 4);
            Trigger_Fish fish = new Trigger_Fish();
            fish.SetFish("Fs_" + randomIndex + "001");
            ItemStruct fishItem = fish.fishStruct.itemStruct;
            float size = fish.randomSize.size;

            Game_Manager.current.inventory.AddItem(fishItem);// 인벤토리에 생선 추가
            Game_Manager.current.fishGuide.AddFishClass(fishItem.id, size);// 도감 추가
            Debug.LogWarning("TestTest");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            string randomID = "Eq_000" + Random.Range(1, 4);
            EquipStruct equip = Singleton_Data.INSTANCE.Dict_Equip[randomID];
            Game_Manager.current.inventory.AddItem(equip.itemStruct);// 인벤토리에 아이템 추가
            Debug.LogWarning("TestTest");
        }
    }















    public enum DragSlotType
    {
        None,
        Shop,
        Inventory
    }
    public DragSlotType dragSlotType = DragSlotType.None;
    public DragSlotType TryDragSlotType { get { return dragSlotType; } set { dragSlotType = value; } }
}
