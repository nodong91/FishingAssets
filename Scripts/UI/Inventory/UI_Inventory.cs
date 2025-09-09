using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;
using static Data_Quest;
using static UI_Inventory;
using static UI_Inventory_Base;
using static UI_Inventory_Slot;

public class UI_Inventory : MonoBehaviour
{
    public SlotType currentType;

    public UI_MyBox myBox;
    public UI_Shop shop;
    public Image iconImage;
    bool onRepair;

    public bool onDrag, onCheck;
    Coroutine slotMoving;

    public UI_Inventory_Infomation infomation;
    public List<Vector2Int> TryDestroySlot
    {
        get { return myBox.destroySlot; }
        set { myBox.destroySlot = value; }
    }

    public SlotType enterSlotType, selectSlotType;
    private UI_Inventory_Slot enterSlot, selectSlot;

    ItemClass selectItemClass;
    ItemClass originItemClass;

    int slotSize = 40;
    ResultStruct resultItem;

    float GetMoney => Game_Manager.current.GetMainUI.TryMoney;

    void Update()// 아이템 추가 테스트
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Game_Manager.current.GetMainUI.MoveMoney(1000f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            string randomID = addFishTest;
            if (Singleton_Data.INSTANCE.Dict_Fish.ContainsKey(addFishTest) == false)
                return;
            FishStruct fishStruct = Singleton_Data.INSTANCE.Dict_Fish[randomID];
            FishStruct.RandomSize randomSize = fishStruct.GetRandom();

            SetIconImage(fishStruct.itemStruct);
            AddItem(fishStruct.itemStruct);
        }
    }
    public string addFishTest;

    public void SetStart()
    {
        myBox.SetSlotSize = slotSize;
        shop.SetSlotSize = slotSize;
        myBox.SetStart();
        shop.SetStart();

        SetInfomation(null);// 인포메이션 제거
    }

    public void OpenInventory(bool _open)
    {
        if (_open == true)
        {
            currentType = SlotType.MyBox;
        }
        else
        {
            currentType = SlotType.None;
        }
        myBox.OpenCanvas(_open);
    }

    public void CloseShop()
    {
        if (currentType != SlotType.None)
        {
            currentType = SlotType.None;
            myBox.OpenCanvas(false);
            shop.OpenCanvas(false);
        }
    }

    public void OpenShop(Data_NPC _npc)
    {
        if (currentType != SlotType.Shop)
        {
            currentType = SlotType.Shop;
            myBox.OpenCanvas(true);
            shop.SetShop(true, _npc);
        }
    }

    public void OpenShipyard(Data_NPC _npc)
    {
        if (currentType != SlotType.Shipyard)
        {
            currentType = SlotType.Shipyard;
            myBox.OpenCanvas(true);
            shop.SetShipyard(true, _npc);
        }
    }

    public void OpenStorage(bool _open)
    {
        if (currentType != SlotType.Storage)
        {
            currentType = SlotType.Storage;
            myBox.OpenCanvas(_open);
            shop.SetStorage(_open);
        }
    }

    public void OpenResult()
    {
        if (resultItem.itemID == null || resultItem.itemID.Length == 0)
            return;

        if (currentType != SlotType.Result)
        {
            ResultStruct result = resultItem;
            currentType = SlotType.Result;
            myBox.OpenCanvas(true);
            shop.SetResult(true, result);
        }
    }

    public void CloseResult()
    {
        if (currentType == SlotType.Result)
        {
            currentType = SlotType.None;
            myBox.OpenCanvas(false);
            shop.SetResult(false);

            resultItem = default; // 퀘스트 결과 아이템 초기화
        }
        Game_Manager.current.GetMainUI.OpenCanvas(true);
    }

    public void SetResult(ResultStruct _resultItem)
    {
        // 퀘스트 결과 아이템 세팅
        resultItem = _resultItem;
    }

    public void AddItem(ItemStruct _itemStruct)
    {
        ItemClass itemClass = new ItemClass
        {
            item = _itemStruct,
            angle = 0,
            shape = _itemStruct.shape,
            acquisition = Game_Manager.current.GetTimeUI.day,
        };
        selectItemClass = itemClass;
        DragSlot();// 아이템 추가
    }

    void SellItem(string _id)
    {
        ItemStruct item = Singleton_Data.INSTANCE.GetItemStruct(_id);
        float price = item.price;
        Game_Manager.current.GetMainUI.MoveMoney(price);
    }

    bool BuyItem(string _id)// 구매
    {
        ItemStruct item = Singleton_Data.INSTANCE.GetItemStruct(_id);
        if (GetMoney < item.price)
            return false;

        if (myBox.AddItem(item) == true)// 살 공간이 있으면 슬롯세팅
        {
            float price = -item.price;
            Game_Manager.current.GetMainUI.MoveMoney(price);
        }
        return true;
    }

    void SetEmptySlot(UI_Inventory_Slot _slot)// 슬롯 비우기
    {
        UI_Inventory_Base getInventory = GetInventory(enterSlotType);
        getInventory.SlotEmpty(_slot);
    }

    bool TryTradeItem()// 상점
    {
        if (selectSlotType == SlotType.Shop && enterSlotType == SlotType.MyBox)// 구매
        {
            float price = selectItemClass.item.price;
            Debug.LogWarning(GetMoney + ">>>>" + price);
            if (GetMoney < price)
            {
                Debug.LogWarning("돈없음");
                return false;// 돈없음
            }
            else
            {
                Game_Manager.current.GetMainUI.MoveMoney(-price);
                Debug.LogWarning("구매");
            }
        }
        else if (selectSlotType == SlotType.MyBox && enterSlotType == SlotType.Shop)// 판매
        {
            float price = selectItemClass.item.price;
            Game_Manager.current.GetMainUI.MoveMoney(price);
            Debug.LogWarning("판매");
        }
        return true;
    }

    //===========================================================================================================================
    // 인풋 컨트롤
    //===========================================================================================================================
    public void OnPointerLeftClick(UI_Inventory_Slot _slot)
    {
        // 아이템 체크
        if (onDrag == true)// 드래그 중일 때 드랍
        {
            if (onCheck == true)// 놓을 수 있다.
            {
                if (TryTradeItem() == true)// 트레이드가 가능한지
                {
                    if (enterSlotType == SlotType.Shop)// 판매
                    {
                        Debug.LogWarning("판매");
                    }
                    UI_Inventory_Base getInventory = GetInventory(enterSlotType);
                    getInventory.SetSlot(enterSlot, selectItemClass);
                }
                else
                {
                    // 돈없음
                    UI_Inventory_Base getInventory = GetInventory(selectSlotType);
                    getInventory.SetSlot(selectSlot, originItemClass);
                    Game_Manager.current.GetMainUI.SetWarnningText("돈 없음");
                    Debug.LogWarning("돈 없음");
                }
            }
            else// 놓을 수 없다면
            {
                if (selectSlot == null)
                {
                    // 낚시 성공등 트레이드가 아닌 경우?
                    Debug.LogError("낚시 성공등 트레이드가 아닌 경우?");
                }
                else
                {
                    // 놓을 수 없다면 원래 위치로 돌리기
                    UI_Inventory_Base getInventory = GetInventory(selectSlotType);
                    getInventory.SetSlot(selectSlot, originItemClass);
                    Game_Manager.current.GetMainUI.SetWarnningText("놓을 수 없음");
                    Debug.LogWarning("놓을 수 없음");
                }
            }
            originItemClass = null;
            OffDragReset();
        }
        else// 드래그 중이 아닐 때 클릭
        {
            if (_slot.destroy == true && onRepair == true)
            {
                onRepair = false;
                myBox.FixSlot(_slot);    // 하나씩 수리
                if (selectSlot != null)// 선택된 슬롯이 있으면
                {
                    SetEmptySlot(selectSlot);// 선택된 슬롯 비우기
                }
            }

            if (_slot.empty == true)
                return;

            // 픽업
            selectSlot = _slot.GetLinkSlot;
            selectItemClass = selectSlot.itemClass;
            selectSlotType = enterSlotType;

            SetOriginItemClass();// 기존 위치 저장
            SetEmptySlot(selectSlot);// 위치 비우기
            DragSlot();// 드래그 시작
        }
    }

    public void OnPointerRightClick(UI_Inventory_Slot _slot)
    {
        if (onDrag == true)// 드래그 중일 때
        {
            SetDragRotate();
        }
        else if (onRepair == true)// 수리 모드일 때
        {
            onRepair = false;
            Game_Manager.current.GetMainUI.SetWarnningText("수리 모드 취소");
        }
        else if (_slot.empty == false)
        {
            selectSlot = _slot.GetLinkSlot;
            Debug.LogWarning($"{currentType} 클릭!!!!!!!!!!!!!!!!!!!!");
            switch (currentType)
            {
                case SlotType.Shop:// 샵이 열려있을 때 우클릭
                case SlotType.Shipyard:
                    if (enterSlotType == SlotType.MyBox)// 판매
                    {
                        Debug.LogWarning("판매");
                        SellItem(_slot.itemClass.item.id);
                    }
                    else if (enterSlotType == SlotType.Shop || enterSlotType == SlotType.Shipyard)// 구매
                    {
                        Debug.LogWarning("구매");
                        if (BuyItem(_slot.itemClass.item.id) == false)
                        {
                            Game_Manager.current.GetMainUI.NoMoney();
                            return;
                        }
                    }
                    SetEmptySlot(_slot.GetLinkSlot);// 슬롯 비우기
                    break;
                case SlotType.Storage:// 창고가 열려있을 때 우클릭
                    break;
                case SlotType.Result:
                    break;
                case SlotType.MyBox:
                    ItemAction();// 사용하기
                    break;
                default:
                    break;
            }
        }
    }

    void ItemAction()
    {
        // 아이템 사용
        switch (selectSlot.itemClass.item.itemType)
        {
            case ItemStruct.ItemType.Fish:
                UseFish();
                Game_Manager.current.GetNews.OpenNewsPaper();// 신문 열기
                break;
            case ItemStruct.ItemType.Used:
                UseItem();
                break;
            case ItemStruct.ItemType.Quest:
                Game_Manager.current.GetNews.OpenNewsPaper();// 신문 열기
                break;
        }
    }

    void UseFish()
    {
        Debug.LogWarning("Fish");
    }

    void UseItem()
    {
        UsedStruct usedStruct = Singleton_Data.INSTANCE.Dict_Used[selectSlot.itemClass.item.id];
        switch (usedStruct.usedType)
        {
            case UsedStruct.UsedType.Energy:
                Game_Manager.current.GetPlayer.AddEnergy(usedStruct.value);
                SetEmptySlot(selectSlot);// 사용한 아이템 비우기
                break;
            case UsedStruct.UsedType.Health:
                IndividualRepair();
                break;
        }
    }

    public void IndividualRepair()
    {
        onRepair = true;
        // 하나씩 수리 모드
        Game_Manager.current.GetMainUI.SetWarnningText("수리할 아이템을 선택하세요.");
    }

    public void AllRepair()
    {
        myBox.FixAll();
    }

    public void OnPointerEnter(UI_Inventory_Slot _slot, SlotType _dragSlotType)
    {
        enterSlotType = _dragSlotType;
        enterSlot = _slot;
        CheckSlot(_dragSlotType);

        SetInfomation(_slot);
    }

    public void OnPointerExit()
    {
        SetInfomation(null);// 켜져 있던 정보 끄기

        enterSlotType = SlotType.None;
        enterSlot = null;
        CheckSlot(SlotType.None);
    }

    public void OffDragReset()
    {
        selectSlot = null;
        selectItemClass = null;
        selectSlotType = SlotType.None;
        CheckSlot(SlotType.None);// 체크 제거

        onDrag = false;
    }

    //===========================================================================================================================
    // 액션
    //===========================================================================================================================

    void DragSlot()
    {
        SetInfomation(null);// 켜져 있던 정보 끄기
        onDrag = true;

        CheckSlot(enterSlotType);

        if (slotMoving != null)
            StopCoroutine(slotMoving);
        slotMoving = StartCoroutine(DragingSlot());
    }

    IEnumerator DragingSlot()
    {
        iconImage.gameObject.SetActive(true);
        if (selectItemClass != null)
        {
            ItemStruct itemStruct = selectItemClass.item;
            SetIconImage(itemStruct);
        }

        while (onDrag == true)
        {
            iconImage.transform.position = Input.mousePosition;
            iconImage.transform.rotation = Quaternion.Euler(0f, 0f, selectItemClass.angle);
            yield return null;
        }
        iconImage.gameObject.SetActive(false);
    }

    void SetIconImage(ItemStruct _itemStruct)
    {
        iconImage.sprite = _itemStruct.icon;
        iconImage.rectTransform.sizeDelta = new Vector2(_itemStruct.iconSize.x, _itemStruct.iconSize.y) * slotSize;
        iconImage.rectTransform.pivot = new Vector2(_itemStruct.iconSize.z, _itemStruct.iconSize.w);
    }

    void SetDragRotate()
    {
        if (selectItemClass == null)
            return;

        selectItemClass.SetRotate(90f);
        CheckSlot(enterSlotType);
    }

    void SetOriginItemClass()
    {
        if (originItemClass == null)// 기존 위치 저장
            originItemClass = new ItemClass();
        originItemClass.SetItemClass(selectItemClass);
    }

    UI_Inventory_Base GetInventory(SlotType _dragSlotType)
    {
        switch (_dragSlotType)
        {
            case SlotType.None:
                return null;

            case SlotType.MyBox:
                return myBox;

            case SlotType.Shop:
            case SlotType.Shipyard:
            case SlotType.Storage:
            case SlotType.Result:
                return shop;
        }
        return null;
    }

    public void SetInfomation(UI_Inventory_Slot _slot)
    {
        if (onDrag == true)
            return;

        infomation.SetStart(_slot);
    }

    public void DistroySlot()
    {
        myBox.DistroySlot();
    }

    //===========================================================================================================================
    // 체크 인벤토리
    //===========================================================================================================================

    void CheckSlot(SlotType _dragSlotType)
    {
        if (onDrag == false)
            return;

        UI_Inventory_Base getInventory = GetInventory(_dragSlotType);
        if (getInventory == null || _dragSlotType == SlotType.None)
        {
            onCheck = false;
            // 체크칸 모두 제거
            myBox.ClearCheckList();
            shop.ClearCheckList();
            IconImageColor(onCheck);
            return;
        }
        onCheck = getInventory.SetCheck(enterSlot, selectItemClass);
        IconImageColor(onCheck);
    }

    void IconImageColor(bool _onCheck)
    {
        if (iconImage == null)
            return;

        // 아이콘 색상 변경
        Color iconColor = _onCheck ? Color.white : P01_Utility.HexToColor("800000");
        iconImage.color = iconColor;
    }

    //===========================================================================================================================
    // 가방 안에 아이템이 있는지 확인
    //===========================================================================================================================

    List<UI_Inventory_Slot> checkSlot = new List<UI_Inventory_Slot>();
    public bool CheckQuestItem(string[] _needItems)
    {
        checkSlot.Clear();
        bool check = true;
        for (int i = 0; i < _needItems.Length; i++)
        {
            check = myBox.CheckItem(_needItems[i], out UI_Inventory_Slot _slot);
            if (check == false)
                return false;
            checkSlot.Add(_slot.GetLinkSlot);
        }
        Debug.LogWarning("CheckQuestItem: " + checkSlot.Count + " items found.");
        return check;
    }

    public void RemoveQuestItem()
    {
        // 아이템이 모두 있는 경우
        for (int i = 0; i < checkSlot.Count; i++)
        {
            UI_Inventory_Slot slot = checkSlot[i];
            myBox.SlotEmpty(slot);// 해당 아이템을 비우기
        }
    }
}
