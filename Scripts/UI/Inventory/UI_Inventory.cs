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

    public ItemInInventory selectItemClass;
    ItemInInventory originItemClass;

    const int slotSize = 40;
    ResultStruct resultItem;

    public string addItemTest;

    public void CloseCanvas() => Game_Manager.current.GetMainUI?.CloseCanvas();

    void Update()// 아이템 추가 테스트
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Game_Manager.current.GetMainUI.MoveMoney(1000f);// 아이템 추가 테스트
            Debug.LogError("머니 치트");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.LogError("아이템 치트");
            string randomID = addItemTest;
            //if (Singleton_Data.INSTANCE.Dict_Fish.ContainsKey(randomID) == false)
            //    return;

            onDrag = true;
            //selectSlotType = SlotType.MyBox;
            //FishStruct fishStruct = Singleton_Data.INSTANCE.Dict_Fish[randomID];
            //FishStruct.RandomSize randomSize = fishStruct.GetRandom();
            //Debug.LogWarning($"{randomID} > {Singleton_Data.INSTANCE.Dict_Fish.ContainsKey(randomID)}");

            ItemStruct item = Singleton_Data.INSTANCE.GetItemStruct(randomID);
            if (item.id == null)
                return;
            ItemInInventory itemInInventory = myBox.SetItemClass(item);// 테스트 아이템 추가
            SetIconImage(item);
            selectItemClass = itemInInventory;
            DragSlot();// 아이템 추가
        }
    }

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
        Camera_Manager.current.CameraFocus(_open);
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
            currentType = SlotType.Result;
            ResultStruct result = resultItem;
            myBox.OpenCanvas(true);
            shop.SetResult(true, result);
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

    public void CloseResult(bool _questReslut)
    {
        if (currentType == SlotType.Result)
        {
            currentType = SlotType.None;
            myBox.OpenCanvas(false);
            shop.SetResult(false);

            resultItem = default; // 퀘스트 결과 아이템 초기화
        }

        if (_questReslut == true)
        {
            Game_Manager.current.GetLanding.OpenLandingUI();
        }
        else
        {
            Game_Manager.current.GetMainUI.OpenCanvas(true);
        }
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

    public void SetResult(ResultStruct _resultItem)
    {
        // 퀘스트 결과 아이템 세팅
        resultItem = _resultItem;
    }

    void SellItem(ItemStruct _item)
    {
        float addPrice = _item.price * Game_Manager.current.currentStatus.fishPrice * 0.01f;// 퍼센트 만큼 비싸게 판매 
        float price = Mathf.Round(_item.price + addPrice);// 스킬 스탯 추가
        Game_Manager.current.GetMainUI.MoveMoney(price);
        Debug.LogWarning($"아이템 판매: {_item.name} for {_item.price} + {addPrice} = {price}");
    }

    void BuyItem(ItemStruct _item)
    {
        float addPrice = _item.price * Game_Manager.current.currentStatus.fishPrice * 0.01f;// 퍼센트 만큼 싸게 구매 
        float price = -_item.price;
        Game_Manager.current.GetMainUI.MoveMoney(price);
        Debug.LogWarning($"아이템 구매: {_item.name} for {_item.price} + {addPrice} = {price}");
    }

    void SetEmptySlot(UI_Inventory_Slot _slot)// 슬롯 비우기
    {
        UI_Inventory_Base getInventory = GetInventory(enterSlotType);
        getInventory.SlotEmpty(_slot);
    }

    public void SetQuestResult()
    {
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
        if (onRepair == true && _slot.destroy == true)// 수리 모드에서 파괴 슬롯 클릭
        {
            onRepair = false;
            myBox.FixSlot(_slot);    // 하나씩 수리
            if (selectSlot != null)// 선택된 슬롯이 있으면
            {
                SetEmptySlot(selectSlot);// 선택된 슬롯 비우기
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
            selectItemClass = selectSlot.itemInInventory;
            selectSlotType = enterSlotType;

            SetOriginItemClass();// 기존 위치 저장
            SetEmptySlot(selectSlot);// 위치 비우기
            DragSlot();// 드래그 시작
        }
    }

    private void DragEnd()// 드랍
    {
        if (onCheck == true)// 놓을 수 있다.
        {
            //Debug.LogWarning(""+ selectSlot);
            //if (selectSlot == null)// 같은 슬롯이거나 선택된 슬롯이 없으면
            //    return;

            // 내 인벤토리로 옮길 때 무게 초과
            if (enterSlotType == SlotType.MyBox && myBox.CheckWeight(selectItemClass.item.weight) == false)// 드랍 구매
            {
                MoveOriginalSlot();
                originItemClass = null;
                OffDragReset();
                return;
            }

            switch (currentType)
            {
                // 현재 열린 타입이 상점류인 경우
                case SlotType.Shop:// 샵이 열려있을 때 드래그
                    DragShop();
                    break;
                case SlotType.Shipyard:
                    DragShipyard();
                    break;

                case SlotType.Submit:
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
                    break;

                default:
                    // 슬롯 교환
                    tempEnter = GetInventory(enterSlotType);
                    tempEnter.SetSlot(enterSlot, selectItemClass);// 놓기
                    break;
            }
        }
        else// 놓을 수 없다면
        {
            MoveOriginalSlot();
        }
        originItemClass = null;
        OffDragReset();
    }

    void MoveOriginalSlot()// 원래 위치로 돌리기
    {
        UI_Inventory_Base getInventory = GetInventory(selectSlotType);
        getInventory.SetSlot(selectSlot, originItemClass);
        Game_Manager.current.GetMainUI.SetWarnningText("놓을 수 없음");
    }

    void DragShop()
    {
        if(enterSlotType == selectSlotType)// 같은 타입이면 그냥 이동
        {
            UI_Inventory_Base tempEnter = GetInventory(enterSlotType);
            tempEnter.SetSlot(enterSlot, selectItemClass);// 놓기
            return;
        }

        if (enterSlotType == SlotType.MyBox)// 드랍 구매
        {
            // 돈이 부족
            if (Game_Manager.current.CheckMoney(selectItemClass.item.price) == false)// 실패
            {
                MoveOriginalSlot();
            }
            else
            {
                Debug.LogWarning($"드래그로 구매 : {selectItemClass.item.id}");
                BuyItem(selectItemClass.item);// 드래그 구매
                UI_Inventory_Base tempEnter = GetInventory(enterSlotType);
                tempEnter.SetSlot(enterSlot, selectItemClass);// 놓기
            }
        }
        else if (selectSlotType == SlotType.MyBox)// 판매
        {
            switch (selectItemClass.item.itemType)
            {
                case ItemStruct.ItemType.Fish:
                case ItemStruct.ItemType.Quest:
                case ItemStruct.ItemType.Lottery:
                    SellItem(selectItemClass.item);// 드래그 판매
                    break;

                default:
                    MoveOriginalSlot();
                    break;
            }
        }
    }

    void DragShipyard()
    {
        if (enterSlotType == SlotType.MyBox)// 드랍 구매
        {
            // 돈이 부족
            if (Game_Manager.current.CheckMoney(selectItemClass.item.price) == false)
            {
                MoveOriginalSlot();
            }
            else
            {
                Debug.LogWarning($"드래그로 구매 : {selectItemClass.item.id}");
                BuyItem(selectItemClass.item);// 드래그 구매
                UI_Inventory_Base tempEnter = GetInventory(enterSlotType);
                tempEnter.SetSlot(enterSlot, selectItemClass);// 놓기
            }
        }
        else if (selectSlotType == SlotType.MyBox)// 판매
        {
            switch (selectItemClass.item.itemType)
            {
                case ItemStruct.ItemType.Fish:
                    MoveOriginalSlot();
                    break;

                default:
                    SellItem(selectItemClass.item);// 드래그 판매
                    break;
            }
        }
    }

    public void OnPointerRightClick(UI_Inventory_Slot _slot)// 우클릭 액션
    {
        if (onDrag == true)// 드래그 중일 때
        {
            SetDragRotate();// 회전
        }
        else if (onRepair == true)// 수리 모드일 때
        {
            onRepair = false;
            Game_Manager.current.GetMainUI.SetWarnningText("수리 모드 취소");
        }
        else if (_slot.empty == false)
        {
            selectSlot = _slot.GetLinkSlot;
            ItemStruct item = selectSlot.itemInInventory.item;
            Debug.LogWarning($"{currentType} -> 오른 클릭 타입 : {enterSlotType}, 아이템 타입 {item.itemType}");
            switch (currentType)
            {
                case SlotType.Shop:// 샵이 열려있을 때 우클릭
                    if (enterSlotType == SlotType.MyBox)// 내 인벤토리일때 판매
                    {
                        if (item.itemType != ItemStruct.ItemType.Fish)
                            return;
                        SellItem(item);// 우클릭 판매
                    }
                    else// 구매
                    {
                        if (Game_Manager.current.CheckMoney(item.price) == false || myBox.CheckWeight(item.weight) == false)
                            return;

                        if (myBox.AddItem(item) == true)
                        {
                            Debug.LogWarning($"우클릭으로 구매 : {Singleton_Data.INSTANCE.GetLanguage(item.name)}");
                            BuyItem(item);// 클릭 구매
                        }
                    }
                    SetEmptySlot(selectSlot);// 슬롯 비우기
                    break;
                case SlotType.Shipyard:
                    if (enterSlotType == SlotType.MyBox)// 내 인벤토리일때 판매
                    {
                        if (item.itemType == ItemStruct.ItemType.Fish)
                            return;
                        SellItem(item);// 우클릭 판매
                    }
                    else// 구매
                    {
                        if (Game_Manager.current.CheckMoney(item.price) == false || myBox.CheckWeight(item.weight) == false)
                            return;

                        if (myBox.AddItem(item) == true)
                        {
                            Debug.LogWarning($"우클릭으로 구매 : {Singleton_Data.INSTANCE.GetLanguage(item.name)}");
                            BuyItem(item);// 클릭 구매
                        }
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
        // 아이템 사용
        switch (selectSlot.itemInInventory.item.itemType)
        {
            case ItemStruct.ItemType.Fish:
                UseFish();
                break;

            case ItemStruct.ItemType.Fuel:
                UsedStruct usedStruct = Singleton_Data.INSTANCE.Dict_Used[selectSlot.itemInInventory.item.id];
                Game_Manager.current.GetPlayer.AddEnergy(usedStruct.value);
                Debug.LogWarning($"에너지 {usedStruct.value}만큼 회복");
                SetEmptySlot(selectSlot);// 사용한 아이템 비우기
                break;

            case ItemStruct.ItemType.Repare:
                IndividualRepair();
                break;

            case ItemStruct.ItemType.Quest:
                //Game_Manager.current.GetNews.OpenNewsPaper();// 신문 열기
                break;

            case ItemStruct.ItemType.Lottery:// 복권
                // 사용한 복권인지는 어떻게 체크?
                // 사용한 복권은 열지 못하게
                // 사용한 복권이라면 팔때 당첨 금액을 주는 걸로
                Game_Manager.current.GetLottery.OpenCanas();// 복권 열기
                SetEmptySlot(selectSlot);// 사용한 아이템 비우기
                break;
        }
    }

    void UseFish()
    {
        Debug.LogWarning("Fish");
    }

    public void IndividualRepair()// 수리 모드
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
