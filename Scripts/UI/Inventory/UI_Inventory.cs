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

    public SlotType enterSlotType;
    UI_Inventory_Slot enterSlot;

    public SlotType selectSlotType;
    UI_Inventory_Slot selectSlot;

    ItemClass selectItemClass;
    ItemClass originItemClass;

    public void SetStart()
    {
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

        onDrag = true;
        selectItemClass = itemClass;
        DragSlot();
    }

    void BuyItem(string _id)
    {
        ItemStruct item = Singleton_Data.INSTANCE.GetItemStruct(_id);
        if (moneyValue < item.price)
            return;

        if (myBox.BuyItem(item) == true)// 살 공간이 있으면 슬롯세팅
        {
            if (movingMoney != null)
                StopCoroutine(movingMoney);
            movingMoney = StartCoroutine(MoneyWallet(-item.price));
        }
    }

    void RemoveItem(UI_Inventory_Slot _slot)
    {
        UI_Inventory_Base getInventory = GetInventory(enterSlotType);
        getInventory.SetEmpty(_slot);
    }

    void TradeItem()
    {
        if (selectSlotType == SlotType.None)
            return;

        if (enterSlotType != selectSlotType)
        {
            float price = selectItemClass.item.price;
            if (selectSlotType == SlotType.Shop)
                price *= -1f;

            if (movingMoney != null)
                StopCoroutine(movingMoney);
            movingMoney = StartCoroutine(MoneyWallet(price));
        }
    }

    //===========================================================================================================================
    // 인풋 컨트롤
    //===========================================================================================================================
    public void OnPointerLeftClick(UI_Inventory_Slot _slot)
    {
        // 아이템 체크
        if (onDrag == true)// 드래그 중일 때 드랍
        {
            if (onCheck == true)
            {
                UI_Inventory_Base getInventory = GetInventory(enterSlotType);
                getInventory.SetSlot(enterSlot, selectItemClass);
                TradeItem();
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
                }
            }
            originItemClass = null;
            OnTreshBox();
        }
        else// 드래그 중이 아닐 때 픽업
        {
            if (_slot.empty == true)
                return;

            onDrag = true;
            selectSlot = _slot.GetLinkSlot;
            selectItemClass = selectSlot.itemClass;
            selectSlotType = enterSlotType;

            SetOriginItemClass();
            EmptySlot(selectSlotType);
            DragSlot();
        }
    }

    public void OnPointerRightClick(UI_Inventory_Slot _slot)
    {
        if (onDrag == true)// 드래그 중일 때
        {
            SetDragRotate();
        }
        else
        {
            RemoveItem(_slot.GetLinkSlot);
        }
    }

    public void OnPointerEnter(UI_Inventory_Slot _slot, SlotType _dragSlotType)
    {
        enterSlotType = _dragSlotType;
        enterSlot = _slot;
        CheckSlot(enterSlotType);

        SetInfomation(_slot);
    }

    public void OnPointerExit()
    {
        SetInfomation(null);// 켜져 있던 정보 끄기

        enterSlotType = SlotType.None;
        enterSlot = null;
        CheckSlot(SlotType.None);
    }

    public void OnTreshBox()
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
        CheckSlot(enterSlotType);

        if (slotMoving != null)
            StopCoroutine(slotMoving);
        slotMoving = StartCoroutine(DragingSlot());
    }

    IEnumerator DragingSlot()
    {
        iconImage.gameObject.SetActive(true);
        iconImage.sprite = selectItemClass.item.icon;
        while (onDrag == true)
        {
            iconImage.transform.position = Input.mousePosition;
            yield return null;
        }
        iconImage.gameObject.SetActive(false);
    }

    void SetDragRotate()
    {
        if (selectItemClass == null)
            return;

        selectItemClass.SetRotate(90f);
        CheckSlot(enterSlotType);
    }

    void EmptySlot(SlotType _dragSlotType)
    {
        UI_Inventory_Base getInventory = GetInventory(_dragSlotType);
        getInventory.SetEmpty(selectSlot);
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
        }
        return null;
    }

    IEnumerator MoneyWallet(float _money)
    {
        float prevMoney = moneyValue;
        moneyValue += _money;
        bool addMoney = (prevMoney < moneyValue);
        bool moveMoney = true;
        while (moveMoney == true)
        {
            prevMoney = Mathf.Lerp(prevMoney, moneyValue, 0.1f);
            moneyText.text = Mathf.Round(prevMoney).ToString();

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

    public void SetInfomation(UI_Inventory_Slot _slot)
    {
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
