using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;

public class UI_Inventory : MonoBehaviour
{
    public GridLayoutGroup gridLayoutGroup;
    public UI_Inventory_Slot inventorySlot;
    public Vector2Int inventorySize;
    public Image weightSlider;
    public float currentWeight, maxWeight;
    UI_Inventory_Slot[,] allSlots;
    List<UI_Inventory_Slot> checkList = new List<UI_Inventory_Slot>();

    ItemStruct dragItem;
    UI_Inventory_Slot dragSlot, enterSlot, selectedSlot;
    public Image iconImage;
    bool onDrag, onCheck;

    public FishStruct.RandomSize randomSize;

    void Start()
    {
        weightSlider.material = Instantiate(weightSlider.material);
        SetInventory();
        SetInfomation();
    }

    void SetInventory()
    {
        gridLayoutGroup.cellSize = new Vector2(50f, 50f);
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
                inst.dele_Begin = OnBeginDrag;
                inst.dele_Drag = OnDrag;
                inst.dele_End = OnEndDrag;
                inst.dele_LeftClick = OnPointerLeftClick;
                inst.dele_RightClick = OnPointerRightClick;
                inst.dele_Enter = OnPointerEnter;
                inst.dele_Exit = OnPointerExit;
                allSlots[x, y] = inst;
            }
        }
        // 테스트 세팅
        ItemStruct a = Singleton_Data.INSTANCE.Dict_Fish["F_0001"].itemStruct;
        ItemStruct b = Singleton_Data.INSTANCE.Dict_Fish["F_0002"].itemStruct;
        SetSlot(GetEmptySlot(a), a);
        SetSlot(GetEmptySlot(b), b);
        SetSlot(GetEmptySlot(a), a);

        RandomFish();
    }
    
    void RandomFish()
    {
        FishStruct fishStruct = Singleton_Data.INSTANCE.Dict_Fish["F_0001"];
        randomSize = fishStruct.GetRandom();
        Debug.LogWarning($"  {randomSize.size},  {randomSize.weight},  {randomSize.price}");
    }

    void SetSlot(UI_Inventory_Slot _slot, ItemStruct _item)
    {
        _slot.SetBase(_item);// 메인
        if (_item.Shape == null)
            return;

        // 사이즈
        for (int i = 0; i < _item.Shape.Length; i++)
        {
            int slotX = _slot.x + _item.Shape[i].x;
            int slotY = _slot.y + _item.Shape[i].y;
            allSlots[slotX, slotY].SetLink(_slot);
        }
        SetWeight(_item.Weight);
    }

    void SetWeight(float _weight)
    {
        currentWeight += _weight;
        float sliderValue = currentWeight / maxWeight;
        weightSlider.material.SetFloat("_FillAmount", sliderValue);
    }

    void SetEmpty(UI_Inventory_Slot _slot)// 비우기
    {
        ItemStruct item = _slot.item;
        _slot.SetEmpty();// 메인
        if (item.Shape == null)
            return;
        // 사이즈
        for (int i = 0; i < item.Shape.Length; i++)
        {
            int slotX = _slot.x + item.Shape[i].x;
            int slotY = _slot.y + item.Shape[i].y;
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
                    int slotX = slot.x + _item.Shape[i].x;
                    int slotY = slot.y + _item.Shape[i].y;
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
    // 드래그
    //===========================================================================================================================

    void SetDragStart(UI_Inventory_Slot _slot)
    {
        if (_slot.empty == true)
            return;

        UI_Inventory_Slot slot = _slot.baseSlot;
        ItemStruct item = _slot.item;
        SetWeight(-item.Weight);
        SetEmpty(slot);
    }

    void SetDragMove(UI_Inventory_Slot _slot)
    {
        // 이동 중 놓을 수 잇는 곳인지 체크
        SetCheck(_slot);
    }

    //===========================================================================================================================
    // 체크
    //===========================================================================================================================

    void SetCheck(UI_Inventory_Slot _slot)
    {
        ClearCheckList();

        ItemStruct item = dragItem;
        _slot.CheckSlot();// 메인
        onCheck = _slot.CheckSlot();
        checkList.Add(_slot);

        if (item.Shape == null)
            return;
        for (int i = 0; i < item.Shape.Length; i++)
        {
            int slotX = _slot.x + item.Shape[i].x;
            int slotY = _slot.y + item.Shape[i].y;
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

    void BuyItem()// 구매
    {
        if (selectedSlot == null || selectedSlot.empty == true)
        {
            Debug.LogWarning("선택된 아이템 없음");
            return;
        }

        ItemStruct item = selectedSlot.item;
        UI_Inventory_Slot slot = GetEmptySlot(item);
        if (slot == null)
        {
            // 넣을만한 빈 슬롯 없음
            Debug.LogWarning("넣을만한 빈 슬롯 없음");
            return;
        }
        Debug.LogWarning("슬롯 이름 : " + slot.name);
        SetSlot(slot, item);

        if (moneyCoroutine != null)
            StopCoroutine(moneyCoroutine);
        moneyCoroutine = StartCoroutine(WalletMoney(-item.Price));
    }

    void SellItem()// 판매
    {
        if (selectedSlot == null)
            return;

        ItemStruct item = selectedSlot.item;
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

    void OnBeginDrag(UI_Inventory_Slot _slot)
    {
        if (_slot.empty == true)
            return;

        onDrag = true;
        dragSlot = _slot.baseSlot;
        dragItem = dragSlot.item;
        iconImage.sprite = dragSlot.iconImage.sprite;
        iconImage.gameObject.SetActive(dragSlot.empty == false);

        SetDragStart(_slot);
    }

    void OnDrag()
    {
        if (onDrag == true)
            iconImage.transform.position = Input.mousePosition;
    }

    void OnEndDrag()
    {
        if (onDrag == false)
            return;

        ClearCheckList();
        onDrag = false;
        if (dragSlot != null && enterSlot != null && onCheck == true)
        {
            // 교체
            SetSlot(dragSlot, enterSlot.item);
            SetSlot(enterSlot, dragItem);
        }
        else
        {
            // 기존 위치로
            SetSlot(dragSlot, dragItem);
        }
        onCheck = false;
        iconImage.gameObject.SetActive(false);
    }

    void OnPointerLeftClick(UI_Inventory_Slot _slot)
    {
        // 클릭
        selectedSlot = _slot.baseSlot;
        if (selectedSlot != null)
            SetInfomationDisplay(selectedSlot.item);
    }

    void OnPointerRightClick(UI_Inventory_Slot _slot)
    {
        // 클릭
        selectedSlot = _slot.baseSlot;
        if (selectedSlot != null)
            SellItem();
    }

    void OnPointerEnter(UI_Inventory_Slot _slot)
    {
        enterSlot = _slot;
        if (onDrag == true)
            SetDragMove(enterSlot);
    }

    void OnPointerExit()
    {
        enterSlot = null;
    }
}
