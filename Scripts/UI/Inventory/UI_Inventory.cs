using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;
using static Data_Quest;
using static UI_Inventory_Base;
using static UI_Inventory_Slot;

public class UI_Inventory : MonoBehaviour
{
    public SlotType currentType;
    public Custom_Button backButton;

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
    ItemInInventory selectItemClass, originItemClass;

    const int slotSize = 40;
    ResultStruct resultItem;

    public void CloseCanvas() => Game_Manager.current.GetMainUI?.CloseCanvas();

    public void SetStart()
    {
        backButton.SetButton(CloseCanvas);
        myBox.SetSlotSize = slotSize;
        shop.SetSlotSize = slotSize;
        myBox.SetStart();
        shop.SetStart();

        SetInfomation(null);// 인포메이션 제거
    }

    //===========================================================================================================================
    // 열기
    //===========================================================================================================================

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
        Camera_Manager.current.CameraFocusOut(_open);
        myBox.OpenCanvas(_open);
    }

    public void OpenShop(Data_ItemList _itemList)
    {
        if (currentType != SlotType.Shop)
        {
            currentType = SlotType.Shop;
            myBox.OpenCanvas(true);
            shop.SetShop(true, _itemList);
        }
    }

    public void CloseShop()
    {
        if (currentType != SlotType.None)
        {
            currentType = SlotType.None;
            myBox.OpenCanvas(false);
            shop.OpenCanvas(false);

            Cursor_Manager.current?.OnMouseRepair(false);// 커서 변경
        }
    }

    public void OpenShipyard(Data_ItemList _itemList)
    {
        if (currentType != SlotType.Shipyard)
        {
            currentType = SlotType.Shipyard;
            myBox.OpenCanvas(true);
            shop.SetShipyard(true, _itemList);
        }
    }

    public void OpenSmuggler(Data_ItemList _itemList)
    {
        if (currentType != SlotType.Shipyard)
        {
            currentType = SlotType.Shipyard;
            myBox.OpenCanvas(true);
            shop.SetSmuggler(true, _itemList);
        }
    }

    public void OpenInn(Data_ItemList _itemList)
    {
        if (currentType != SlotType.Shipyard)
        {
            currentType = SlotType.Shipyard;
            myBox.OpenCanvas(true);
            shop.SetInn(true, _itemList);
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

    public void OpenReward()
    {
        //if (resultItem.itemID == null || resultItem.itemID.Length == 0)
        //    return;

        if (currentType != SlotType.Result)
        {
            currentType = SlotType.Result;
            ResultStruct result = resultItem;
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
    }

    public void OpenSubmit()
    {
        if (currentType != SlotType.Submit)
        {
            currentType = SlotType.Submit;
            myBox.OpenCanvas(true);
            shop.OpenSubmit(true);
        }
    }

    public void CloseSubMit()
    {
        currentType = SlotType.None;
        myBox.OpenCanvas(false);
        shop.OpenSubmit(false);

        Game_Manager.current.GetQuest.SubmitBackButton();// 다시 열기
    }

    public void OpenGhost(bool _open)
    {
        if (_open == true)
        {
            if (currentType != SlotType.Result)
            {
                currentType = SlotType.Result;
                Game_Manager.current.GetMainUI.OpenCanvas(false);
            }
        }
        else if (currentType == SlotType.Result)
        {
            currentType = SlotType.None;
            Game_Manager.current.GetMainUI.OpenCanvas(true);
        }
        myBox.OpenCanvas(_open);
        shop.SetGhoset(_open);
    }

    //===========================================================================================================================
    // 아이템 이동
    //===========================================================================================================================

    public void SetReward(string[] _itemID)
    {
        ResultStruct resultStruct = new ResultStruct
        {
            inventorySize = new Vector2Int(5, 5),
            itemID = _itemID// 보상 아이템
        };
        // 퀘스트 결과 아이템 세팅
        resultItem = resultStruct;
        OpenReward();
    }

    void SellItem(ItemStruct _item)
    {
        float addPrice = _item.price * Game_Manager.current.currentStatus.fishPrice * 0.01f;// 퍼센트 만큼 비싸게 판매 
        float price = Mathf.Round(_item.price + addPrice);// 스킬 스탯 추가
        Game_Manager.current.GetMainUI.MoveMoney(price);
        Debug.Log($"아이템 판매: {_item.id} for {_item.price} + {addPrice} = {price}");

        if (_item.itemType == ItemStruct.ItemType.Fish)// 생선 판매 시 통계 추가
        {
            FishStruct fishItem = Singleton_Data.INSTANCE.Dict_Fish[_item.id];
            //shop.SellFishCheckCount(fishItem);
        }
    }

    void BuyItem(ItemStruct _item)
    {
        float addPrice = _item.price * Game_Manager.current.currentStatus.fishPrice * 0.01f;// 퍼센트 만큼 싸게 구매 
        float price = -_item.price;
        Game_Manager.current.GetMainUI.MoveMoney(price);
        Debug.Log($"아이템 구매: {_item.id} for {_item.price} + {addPrice} = {price}");
    }

    void SetEmptySlot(UI_Inventory_Slot _slot)// 슬롯 비우기
    {
        UI_Inventory_Base getInventory = GetInventory(enterSlotType);
        getInventory.SlotEmpty(_slot);
        Debug.Log($"아이템 비우기: {_slot.slotNum}");
    }

    public void SetQuestResult()
    {
        // 아이템 채워 넣으면 열리게
        shop.QuestResultUnlock();
    }

    //===========================================================================================================================
    // 인풋 컨트롤
    //===========================================================================================================================
    public void OnPointerLeftClick(UI_Inventory_Slot _slot)
    {
        // 아이템 체크
        if (onDrag == true)// 드래그 중일 때 드랍
        {
            DragEnd();
        }
        else// 드래그 중이 아닐 때
        {
            DragStart(_slot);
        }
    }

    private void DragStart(UI_Inventory_Slot _slot)// 픽업
    {
        if (onRepair == true)// 수리 모드에서 파괴 슬롯 클릭
        {
            RepairMode(false);
            if (_slot.destroy == true)
            {
                // 클릭
                myBox.FixSlot(_slot);    // 하나씩 수리
                if (selectSlot != null)// 선택된 슬롯이 있으면
                {
                    SetEmptySlot(selectSlot);// 선택된 슬롯 비우기
                }
                return;
            }
        }
        else
        {
            if (_slot.empty == true)
                return;

            if (enterSlotType == SlotType.Submit && shop.CheckMy(_slot.slotNum) == false)// 내가 넣은거 아니라면
            {
                Debug.LogWarning("퀘스트 아이템이 필요");
                return;
            }
            onDrag = true;
            // 픽업
            selectSlot = _slot.GetLinkSlot;
            selectSlotType = enterSlotType;
            selectItemClass = selectSlot.itemInInventory;
            if (enterSlotType == SlotType.MyBox)
                myBox.OnRemoveBox(true);

            SetOriginItemClass();// 기존 위치 저장
            SetEmptySlot(selectSlot);// 위치 비우기
            DragSlot();// 드래그 시작
        }
    }

    public void AddPickUpItem(string _id)
    {
        ItemStruct item = Singleton_Data.INSTANCE.GetItemStruct(_id);
        if (item.id == null)
            return;

        onDrag = true;
        ItemInInventory itemInInventory = myBox.SetItemClass(item);// 테스트 아이템 추가
        selectItemClass = itemInInventory;
        SetIconImage(item);
        DragSlot();// 아이템 추가
    }

    private void DragEnd()// 드랍
    {
        if (onCheck == true)// 놓을 수 있다.
        {
            if (enterSlotType == selectSlotType)// 같은 인벤토리 안이라면
            {
                UI_Inventory_Base tempEnter = GetInventory(enterSlotType);
                tempEnter.SetSlot(enterSlot, selectItemClass);// 놓기
                originItemClass = null;
                OffDragReset();
                return;
            }
            else // 같은 타입이 아닐 때
            {
                // 현재 열린 타입
                switch (currentType)
                {
                    case SlotType.Shop:// 샵이 열려있을 때 드래그
                        DragShopType();
                        break;

                    case SlotType.Shipyard:
                        DragShipyardType();
                        break;

                    case SlotType.Submit:// 퀘스트 아이템 제출
                        DragSubmitType();
                        break;

                    default:
                        UI_Inventory_Base tempEnter = GetInventory(enterSlotType);
                        tempEnter.SetSlot(enterSlot, selectItemClass);// 놓기
                        break;
                }
            }
        }
        else// 놓을 수 없다.
        {
            // 원래 위치로 돌리기
            MoveOriginalSlot();
        }
        originItemClass = null;
        OffDragReset();
    }

    void DragShopType()
    {
        if (enterSlotType == SlotType.MyBox)// 드랍 구매
        {
            DragBuy();
        }
        else if (selectSlotType == SlotType.MyBox)// 드랍 판매
        {
            if (selectItemClass.item.itemType == ItemStruct.ItemType.Fish)
            {
                SellItem(selectItemClass.item);// 생선만 드래그 판매
            }
            else
            {
                // 원래 위치로 돌리기
                MoveOriginalSlot();
            }
        }
    }

    void DragShipyardType()
    {
        if (enterSlotType == SlotType.MyBox)// 드랍 구매
        {
            DragBuy();
        }
        else if (selectSlotType == SlotType.MyBox)// 드랍 판매
        {
            if (selectItemClass.item.itemType != ItemStruct.ItemType.Fish)
            {
                SellItem(selectItemClass.item);// 생선외 드래그 판매
            }
            else
            {
                // 원래 위치로 돌리기
                MoveOriginalSlot();
            }
        }
    }

    void DragSubmitType()
    {
        if (selectSlotType == SlotType.MyBox)// 드랍 제출
        {
            DragSubmit();
        }
    }

    void DragBuy()
    {
        if (Game_Manager.current.CheckMoney(selectItemClass.item.price) == true // 돈이 충분하면
            && myBox.CheckWeight(selectItemClass.item.weight) == true)// 무게가 충분하면
        {
            BuyItem(selectItemClass.item);// 드래그 구매
            UI_Inventory_Base tempEnter = GetInventory(enterSlotType);
            tempEnter.SetSlot(enterSlot, selectItemClass);// 놓기
        }
        else
        {
            // 원래 위치로 돌리기
            MoveOriginalSlot();
        }
    }

    void MoveOriginalSlot()// 원래 위치로 돌리기
    {
        UI_Inventory_Base getInventory = GetInventory(selectSlotType);
        getInventory.SetSlot(selectSlot, originItemClass);
        Debug.LogWarning($"{selectItemClass.item.id} > 원래 위치로 돌리기");
    }

    void DragSubmit()
    {
        QuestStruct questStruct = Game_Manager.current.GetQuest.GetSelectQuest;
        int index = System.Array.IndexOf(questStruct.needItem, selectItemClass.item.id);
        Debug.LogWarning($"{selectItemClass.item.id} = {index} 퀘스트 아이템 넣기");
        if (index < 0)
        {
            MoveOriginalSlot();
            originItemClass = null;
            OffDragReset();
            return;
        }

        // 슬롯 교환
        UI_Inventory_Base tempEnter = GetInventory(enterSlotType);
        tempEnter.SetSlot(enterSlot, selectItemClass);// 놓기

        bool checkQuestItem = shop.CheckQuestItem(questStruct.needItem);// Submit 넣기
        Debug.LogWarning($"체크 : {checkQuestItem}");
        Game_Manager.current.GetQuest.ActiveActionButton(checkQuestItem);
    }

    public void OnPointerRightClick(UI_Inventory_Slot _slot)// 우클릭 액션
    {
        if (onDrag == true)// 드래그 중일 때
        {
            SetDragRotate();// 회전
        }
        else if (onRepair == true)// 수리 모드일 때
        {
            RepairMode(false);
        }
        else if (_slot.empty == false)
        {
            selectSlot = _slot.GetLinkSlot;
            ItemStruct item = selectSlot.itemInInventory.item;
            Debug.Log($"{currentType} -> 오른 클릭 타입 : {enterSlotType}, 아이템 타입 {item.itemType}");
            switch (currentType)
            {
                case SlotType.Shop:// 샵이 열려있을 때 우클릭
                    if (enterSlotType == SlotType.MyBox)// 내 인벤토리일때 판매
                    {
                        if (item.itemType != ItemStruct.ItemType.Fish)// 생선만 판매
                            return;
                        SellItem(item);// Shop 우클릭 판매
                    }
                    else// 구매
                    {
                        if (Game_Manager.current.CheckMoney(item.price) == false || myBox.CheckWeight(item.weight) == false || myBox.AddItem(item) == false)
                            return;

                        Debug.LogWarning($"우클릭으로 구매 : {Singleton_Data.INSTANCE.GetLanguage(item.id)}");
                        BuyItem(item);// 클릭 구매
                    }
                    SetEmptySlot(selectSlot);// 슬롯 비우기
                    break;
                case SlotType.Shipyard:
                    if (enterSlotType == SlotType.MyBox)// 내 인벤토리일때 판매
                    {
                        if (item.itemType == ItemStruct.ItemType.Fish)// 생선은 판매 불가
                            return;
                        SellItem(item);// Shipyard 우클릭 판매
                    }
                    else// 구매
                    {
                        if (Game_Manager.current.CheckMoney(item.price) == false || myBox.CheckWeight(item.weight) == false || myBox.AddItem(item) == false)
                            return;

                        Debug.LogWarning($"우클릭으로 구매 : {Singleton_Data.INSTANCE.GetLanguage(item.id)}");
                        BuyItem(item);// 클릭 구매
                    }
                    SetEmptySlot(selectSlot);// 슬롯 비우기
                    break;

                case SlotType.MyBox:
                    ItemAction();// 사용하기
                    break;

                case SlotType.Submit:
                    if (enterSlotType != SlotType.MyBox && myBox.CheckWeight(item.weight) == false)// 가방으로 옮길때 가방의 무게 체크
                        return;

                    if (enterSlotType == SlotType.Submit && shop.CheckMy(_slot.slotNum) == false)// 내가 넣은거 아니라면
                    {
                        Debug.LogWarning("퀘스트 아이템이 필요");
                        return;
                    }

                    QuestStruct questStruct = Game_Manager.current.GetQuest.GetSelectQuest;
                    Debug.LogWarning($"{item.id} 퀘스트 아이템 넣기");
                    List<string> temp = new List<string>(questStruct.needItem);
                    if (temp.Contains(item.id) == false)
                    {
                        return;
                    }
                    //int index = System.Array.IndexOf(questStruct.needItem, item.id);
                    //Debug.LogWarning($"{item.id} = {index} 퀘스트 아이템 넣기");
                    //if (index < 0)
                    //{
                    //    return;
                    //}

                    UI_Inventory_Base getInventory = enterSlotType == SlotType.MyBox ? shop : myBox;
                    if (getInventory.AddItem(item) == true)// 공간이 있으면 슬롯세팅
                    {
                        SetEmptySlot(selectSlot);// 슬롯 비우기
                    }

                    bool checkQuestItem = shop.CheckQuestItem(questStruct.needItem);// Submit 넣기
                    Debug.LogWarning($"퀘스트 아이템 체크 : {checkQuestItem}");
                    Game_Manager.current.GetQuest.ActiveActionButton(checkQuestItem);
                    break;

                default:
                    if (enterSlotType != SlotType.MyBox && myBox.CheckWeight(item.weight) == false)// 가방으로 옮길때 가방의 무게 체크
                        return;

                    getInventory = enterSlotType == SlotType.MyBox ? shop : myBox;
                    if (getInventory.AddItem(item) == true)// 공간이 있으면 슬롯세팅
                    {
                        SetEmptySlot(selectSlot);// 슬롯 비우기
                    }
                    break;
            }
        }
    }

    void ItemAction()
    {
        ItemStruct item = selectSlot.itemInInventory.item;
        Debug.LogWarning($"아이템 사용 (타입 : {item.itemType})");
        // 아이템 사용
        switch (item.itemType)
        {
            case ItemStruct.ItemType.Fish:
                UseFish();
                // 물고기와 같은 등급의 물고기가 나올 확률 버프
                // 등급은 같은 등급인데...버프 지속시간과 확률은 어디서 따올까?
                FishStruct fishStruct = Singleton_Data.INSTANCE.Dict_Fish[item.id];
                Game_Manager.current.AddBuff(fishStruct);
                Debug.LogWarning($"버프 {fishStruct.itemStruct.itemClass} +{fishStruct.addValue}");
                SetEmptySlot(selectSlot);
                break;

            case ItemStruct.ItemType.Fuel:
                UsedStruct usedStruct = Singleton_Data.INSTANCE.Dict_Used[item.id];
                Game_Manager.current.GetPlayer.AddEnergy(usedStruct.etcValue);
                Debug.LogWarning($"에너지 {usedStruct.etcValue}만큼 회복");
                SetEmptySlot(selectSlot);// 사용한 아이템 비우기
                break;

            case ItemStruct.ItemType.Repare:
                RepairMode(true);// 수리 모드 켜기
                break;

            case ItemStruct.ItemType.Buff:
                usedStruct = Singleton_Data.INSTANCE.Dict_Used[item.id];
                Game_Manager.current.AddBuff(usedStruct);
                SetEmptySlot(selectSlot);
                break;

            case ItemStruct.ItemType.Lottery:// 복권
                //Game_Manager.current.GetMainUI.CloseInventory();
                usedStruct = Singleton_Data.INSTANCE.Dict_Used[item.id];
                Game_Manager.current.GetLottery.SetLottery((int)usedStruct.etcValue);// 복권 열기
                SetEmptySlot(selectSlot);// 사용한 아이템 비우기
                break;

            case ItemStruct.ItemType.Money:// 돈
                usedStruct = Singleton_Data.INSTANCE.Dict_Used[item.id];
                Game_Manager.current.GetMainUI.MoveMoney(usedStruct.etcValue);// 돈 추가 테스트
                SetEmptySlot(selectSlot);// 사용한 아이템 비우기
                break;

            case ItemStruct.ItemType.Bait:// 미끼 아이템
                if (Game_Manager.current.CheckLicense() == false)// 낚시 면허가 없으면 사용 불가
                    return;

                // 미끼 사용
                usedStruct = Singleton_Data.INSTANCE.Dict_Used[item.id];
                ItemStruct.ItemClass itemClass = usedStruct.itemStruct.itemClass;
                Game_Manager.current.GetMainUI.CloseInventory();// 인벤토리 닫기
                Game_Manager.current.GetFishing.SetBait(itemClass);
                SetEmptySlot(selectSlot);// 사용한 아이템 비우기
                break;

            case ItemStruct.ItemType.Etc:// 기타 아이템

                break;
        }
    }

    void UseFish()
    {
        Debug.LogWarning($"Fish : {selectSlot.itemInInventory.item.id}");
    }

    public void RepairMode(bool _repair)// 수리 모드
    {
        if (onRepair == true)// 이미 수리 모드라면 끄기
        {
            onRepair = false;
        }
        else// 수리 모드를 켜든 끄든 상관없음
        {
            onRepair = _repair;
        }
        //// 하나씩 수리 모드
        //Game_Manager.current.GetMainUI.SetWarnningText("수리할 아이템을 선택하세요.");
        Cursor_Manager.current?.OnMouseRepair(_repair);// 커서 변경
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
        myBox.OnRemoveBox(false);// 쓰레기통 제거

        onDrag = false;
    }

    //===========================================================================================================================
    // 액션
    //===========================================================================================================================

    void DragSlot()
    {
        SetInfomation(null);// 켜져 있던 정보 끄기
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
        iconImage.sprite = Singleton_Data.INSTANCE.Dict_Sprite[_itemStruct.icon];
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
            originItemClass = new ItemInInventory();
        originItemClass.SetSaveItem(selectItemClass);
    }

    UI_Inventory_Base GetInventory(SlotType _dragSlotType)
    {
        switch (_dragSlotType)
        {
            case SlotType.None:
                return null;

            case SlotType.MyBox:
                return myBox;

            default:
                return shop;
        }
    }

    public void SetInfomation(UI_Inventory_Slot _slot)
    {
        if (onDrag == true)
            return;

        infomation.SetStart(_slot);
    }

    public void DestroySlot()
    {
        myBox.DestroySlot();
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

    //List<Vector2Int> checkSlot = new List<Vector2Int>();
    //public bool CheckQuestItem(string[] _needItems)// 퀘스트 아이템이 있는지 확인
    //{
    //    checkSlot.Clear();
    //    for (int i = 0; i < _needItems.Length; i++)
    //    {
    //        UI_Inventory_Slot _slot = shop.CheckItem(_needItems[i], checkSlot);
    //        if (_slot == null)// 하나라도 없으면 실패
    //            return false;

    //        Debug.LogWarning(_slot.slotNum);
    //        if (checkSlot.Contains(_slot.slotNum) == false)// 같은게 없으면
    //            checkSlot.Add(_slot.slotNum);

    //        if (_needItems.Length == checkSlot.Count)// 개수가 채워지면 트루
    //            return true;
    //    }
    //    return false;
    //}

    //public void RemoveQuestItem()
    //{
    //    // 아이템이 모두 있는 경우
    //    for (int i = 0; i < checkSlot.Count; i++)
    //    {
    //        UI_Inventory_Slot slot = checkSlot[i];
    //        myBox.SlotEmpty(slot);// 해당 아이템을 비우기
    //    }
    //}
}
