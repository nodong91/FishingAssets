using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;
using static UI_Inventory_Base;
using static UI_Inventory_Slot;

public class UI_Inventory : MonoBehaviour
{
    public UI_MyBox myBox;
    public UI_Shop shop;
    public Image iconImage;

    bool onDrag, onCheck;
    Coroutine slotMoving, movingMoney;
    float moneyValue;
    public float TryMoney { get { return moneyValue; } set { moneyValue = value; } }
    public TMPro.TMP_Text moneyText;
    public UI_Inventory_Infomation infomation;

    public SlotType enterSlotType, selectSlotType;
    private UI_Inventory_Slot enterSlot, selectSlot;

    ItemClass selectItemClass;
    ItemClass originItemClass;

    int slotSize = 40;

    public void SetStart()
    {
        myBox.SetSlotSize = slotSize;
        shop.SetSlotSize = slotSize;

        myBox.SetStart();
        shop.SetStart();
    }

    public void OpenCanvas(bool _open)
    {
        myBox.OpenCanvas(_open);
    }

    public void OpenShop(bool _open)
    {
        myBox.OpenCanvas(_open);
        shop.OpenCanvas(_open);
    }

    void Update()// 아이템 추가 테스트
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            string randomID = "Fs_" + Random.Range(1, 4) + "001";
            FishStruct fishStruct = Singleton_Data.INSTANCE.Dict_Fish[randomID];
            FishStruct.RandomSize randomSize = fishStruct.GetRandom();

            SetIconImage(fishStruct.itemStruct);
            AddItem(fishStruct.itemStruct);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            string randomID = "Eq_000" + Random.Range(1, 4);
            BuyItem(randomID);
            Debug.LogWarning("GetKeyDown2");
        }
    }

    public void AddItem(ItemStruct _itemStruct)
    {
        ItemClass itemClass = new ItemClass
        {
            item = _itemStruct,
            angle = 0,
            shape = _itemStruct.shape,
        };
        selectItemClass = itemClass;
        DragSlot();
    }

    void BuyItem(string _id)// 구매
    {
        ItemStruct item = Singleton_Data.INSTANCE.GetItemStruct(_id);
        if (moneyValue < item.price)
            return;

        if (myBox.BuyItem(item) == true)// 살 공간이 있으면 슬롯세팅
        {
            float price = -item.price;
            if (CheckMoney(price) == true)
                MoveMoney(price);
        }
    }

    void SetEmptySlot(UI_Inventory_Slot _slot)// 슬롯 비우기
    {
        UI_Inventory_Base getInventory = GetInventory(enterSlotType);
        getInventory.SlotEmpty(_slot);
    }

    bool TryTradeItem()// 상점
    {
        if (selectSlotType == SlotType.None || selectSlotType == SlotType.Storage || enterSlotType == SlotType.Storage)
            return true;

        if (enterSlotType != selectSlotType)
        {
            float price = selectItemClass.item.price;
            if (selectSlotType == SlotType.Shop)
                price *= -1f;

            if (CheckMoney(price) == true)
                MoveMoney(price);
            else
                return false;
        }
        return true;
    }

    bool CheckMoney(float _price)
    {
        return (moneyValue + _price >= 0f);
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
                    UI_Inventory_Base getInventory = GetInventory(enterSlotType);
                    getInventory.SetSlot(enterSlot, selectItemClass);
                }
                else
                {
                    // 돈없음
                    UI_Inventory_Base getInventory = GetInventory(selectSlotType);
                    getInventory.SetSlot(selectSlot, originItemClass);
                    Game_Manager.current.mainUI.SetWarnningText("돈 없음");
                }
            }
            else
            {
                if (selectSlot == null)
                {
                    // 낚시 성공등 트레이드가 아닌 경우?
                }
                else
                {
                    // 놓을 수 없다면 원래 위치로 돌리기
                    UI_Inventory_Base getInventory = GetInventory(selectSlotType);
                    getInventory.SetSlot(selectSlot, originItemClass);
                    Game_Manager.current.mainUI.SetWarnningText("놓을 수 없음");
                }
            }
            originItemClass = null;
            OffDragReset();
        }
        else// 드래그 중이 아닐 때 픽업
        {
            if (_slot.empty == true)
                return;

            selectSlot = _slot.GetLinkSlot;
            selectItemClass = selectSlot.itemClass;
            selectSlotType = enterSlotType;

            SetOriginItemClass();
            SetEmptySlot(selectSlot);
            DragSlot();
        }
    }

    public void OnPointerRightClick(UI_Inventory_Slot _slot)
    {
        if (onDrag == true)// 드래그 중일 때
        {
            SetDragRotate();
        }
        else if (_slot.empty == false)
        {
            SetEmptySlot(_slot.GetLinkSlot);
        }
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
        Image inst = iconImage;
        iconImage.gameObject.SetActive(true);
        if (selectSlot != null)
        {
            Debug.LogWarning(selectItemClass);
            Debug.LogWarning(selectItemClass.item);
            ItemStruct itemStruct = selectItemClass.item;

            SetIconImage(itemStruct);
        }

        while (onDrag == true)
        {
            inst.transform.position = Input.mousePosition;
            inst.transform.rotation = Quaternion.Euler(0f, 0f, selectItemClass.angle);
            yield return null;
        }
        iconImage.gameObject.SetActive(false);
    }

    void SetIconImage(ItemStruct _itemStruct)
    {
        iconImage.sprite = _itemStruct.icon;
        iconImage.rectTransform.sizeDelta = new Vector2(_itemStruct.iconSize.x, _itemStruct.iconSize.y) * slotSize;
        iconImage.rectTransform.pivot = new Vector2(_itemStruct.iconSize.w, _itemStruct.iconSize.z);
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
                return shop;

            case SlotType.Storage:
                return shop;
        }
        return null;
    }

    void MoveMoney(float _price)
    {
        if (moneyValue + _price < 0f)
            return;

        if (movingMoney != null)
            StopCoroutine(movingMoney);
        movingMoney = StartCoroutine(MoneyMoving(_price));
    }

    IEnumerator MoneyMoving(float _price)
    {
        float prevMoney = moneyValue;
        moneyValue += _price;
        bool moveMoney = true;
        Debug.LogWarning(_price + "          " + moneyValue);
        while (moveMoney == true)
        {
            prevMoney = Mathf.Lerp(prevMoney, moneyValue, 0.1f);
            moneyText.text = Mathf.Round(prevMoney).ToString();

            if (_price < 0f)// 판매인 경우
            {
                if (prevMoney <= moneyValue)
                    moveMoney = false;
            }
            else if (_price > 0f)// 구매인 경우
            {
                if (prevMoney >= moneyValue)
                    moveMoney = false;
            }
            yield return null;
        }
    }

    public void SetInfomation(UI_Inventory_Slot _slot)
    {
        if (onDrag == true)
            return;

        infomation.SetStart(_slot);
    }

    //===========================================================================================================================
    // 체크 인벤토리
    //===========================================================================================================================

    void CheckSlot(SlotType _dragSlotType)
    {
        if (onDrag == false)
            return;

        UI_Inventory_Base getInventory = GetInventory(_dragSlotType);
        if (getInventory == null)
        {
            // 체크칸 모두 제거
            myBox.ClearCheckList();
            shop.ClearCheckList();
            return;
        }
        onCheck = getInventory.SetCheck(enterSlot, selectItemClass);
    }
}
