using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Data_Manager;

public class UI_Inventory : MonoBehaviour
{
    public GridLayoutGroup gridLayoutGroup;
    public UI_Inventory_Slot inventorySlot;
    public Vector2Int inventorySize;
    public Image weightSlider;
    public float currentWeight, maxWeight;
    ItemStruct a, b;
    UI_Inventory_Slot[,] allSlots;
    List<UI_Inventory_Slot> checkList = new List<UI_Inventory_Slot>();

    ItemStruct dragItem;
    UI_Inventory_Slot dragSlot, enterSlot;
    public Image iconImage;
    bool onDrag, onCheck;

    void Start()
    {
        a = Singleton_Data.INSTANCE.Dict_Fish["F_0001"].itemStruct;
        b = Singleton_Data.INSTANCE.Dict_Fish["F_0002"].itemStruct;
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
                inst.dele_Click = OnPointerClick;
                inst.dele_Enter = OnPointerEnter;
                inst.dele_Exit = OnPointerExit;
                allSlots[x, y] = inst;
            }
        }
        // 테스트 세팅
        SetSlot(allSlots[1, 1], a);
        SetSlot(allSlots[3, 3], b);
    }

    void SetSlot(UI_Inventory_Slot _slot, ItemStruct _item)
    {
        _slot.SetBase(_item);// 메인
        if (_item.Size == null)
            return;

        // 사이즈
        for (int i = 0; i < _item.Size.Length; i++)
        {
            int slotX = _slot.x + _item.Size[i].x;
            int slotY = _slot.y + _item.Size[i].y;
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
        if (item.Size == null)
            return;
        // 사이즈
        for (int i = 0; i < item.Size.Length; i++)
        {
            int slotX = _slot.x + item.Size[i].x;
            int slotY = _slot.y + item.Size[i].y;
            allSlots[slotX, slotY].SetEmpty();
        }
    }

    UI_Inventory_Slot GetEmptySlot(ItemStruct _item)
    {
        foreach (var child in allSlots)
        {
            bool empty = true;
            if (child.empty == false)
            {
                continue;
            }

            for (int i = 0; i < _item.Size.Length; i++)
            {
                int slotX = child.x + _item.Size[i].x;
                int slotY = child.y + _item.Size[i].y;
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
                return child;
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

        if (item.Size == null)
            return;
        for (int i = 0; i < item.Size.Length; i++)
        {
            int slotX = _slot.x + item.Size[i].x;
            int slotY = _slot.y + item.Size[i].y;
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

    float walletMoney;
    Coroutine moneyCoroutine;
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

    void BuyItem()
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
        moneyCoroutine = StartCoroutine(RemoveWalletMoney(walletMoney - item.Price));
    }

    void SellItem()
    {
        if (selectedSlot == null)
            return;

        ItemStruct item = selectedSlot.item;
        SetWeight(-item.Weight);
        SetEmpty(selectedSlot);

        if (moneyCoroutine != null)
            StopCoroutine(moneyCoroutine);
        moneyCoroutine = StartCoroutine(AddWalletMoney(walletMoney + item.Price));
    }

    IEnumerator AddWalletMoney(float _money)
    {
        while (walletMoney < _money)
        {
            walletMoney = Mathf.Lerp(walletMoney, _money, 0.1f);
            walletText.text = Mathf.Round(walletMoney).ToString();
            yield return null;
        }
    }

    IEnumerator RemoveWalletMoney(float _money)
    {
        while (walletMoney > _money)
        {
            walletMoney = Mathf.Lerp(walletMoney, _money, 0.1f);
            walletText.text = Mathf.Round(walletMoney).ToString();
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
    UI_Inventory_Slot selectedSlot;
    void OnPointerClick(UI_Inventory_Slot _slot)
    {
        // 클릭
        selectedSlot = _slot.baseSlot;
        if (selectedSlot != null)
            SetInfomationDisplay(selectedSlot.item);
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
