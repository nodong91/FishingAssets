using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;
using static Data_Quest;

public class UI_Shop : UI_Inventory_Base
{

    //===========================================================================================================================
    // 상점
    //===========================================================================================================================

    [Header("- Shop")]
    private Data_NPC npc;
    public VerticalLayoutGroup layoutGroup;
    public ToggleGroup toggleGroup;
    public GameObject fixGroup;
    public Custom_Button fixButton, fixAllButton;
    public Custom_Button[] toggleButtons;

    public override void SetStart()
    {
        base.SetStart();
        for (int i = 0; i < toggleButtons.Length; i++)
        {
            int index = i;
            toggleButtons[i].SetButton(delegate { SetToggle(index); });
            toggleButtons[i].buttonImage.material = Instantiate(toggleButtons[i].buttonImage.material);
            toggleButtons[i].buttonImage.material.SetFloat("_FillAmount", 0f);
        }
        SetToggle(0);
        fixButton.SetButton(FixButton);
        fixAllButton.SetButton(FixAllButton);
    }

    void SetToggle(int _index)
    {
        toggleButtons[currentIndex].buttonImage.material.SetFloat("_FillAmount", 0f);
        toggleButtons[_index].buttonImage.material.SetFloat("_FillAmount", 1f);

        if (currentIndex != _index)
        {
            Static_JsonManager.SaveInventory(saveData, GetSaveInventoryData); ;  // 토글 변경 시 저장
            currentIndex = _index;
        }
        SetShopItem();
    }

    void FixButton()
    {
        Game_Manager.current.GetInventory.IndividualRepair();
    }

    void FixAllButton()
    {
        Game_Manager.current.GetInventory.AllRepair();
    }

    public override void OpenCanvas(bool _open)
    {
        base.OpenCanvas(_open);
    }

    public void SetShop(bool _open, Data_NPC _npc)
    {
        npc = _npc;
        inventoryID = _npc.npc_ID;
        currentIndex = 0;
        OpenCanvas(_open);

        layoutGroup.padding.top = 15;
        layoutGroup.padding.bottom = 15;

        slotType = SlotType.Shop;// SetShop
        toggleGroup.gameObject.SetActive(false);
        fixGroup.gameObject.SetActive(false);

        if (_open)
            SetShopItem();// 열릴때 세팅
    }

    public void SetShipyard(bool _open, Data_NPC _npc)
    {
        npc = _npc;
        inventoryID = _npc.npc_ID;
        currentIndex = 0;
        OpenCanvas(_open);

        layoutGroup.padding.top = 15;
        layoutGroup.padding.bottom = 40;

        slotType = SlotType.Shipyard;// SetShipyard
        toggleGroup.gameObject.SetActive(false);
        fixGroup.gameObject.SetActive(true);

        if (_open)
            SetShopItem();// 열릴때 세팅
    }

    public void SetSmuggler(bool _open, Data_NPC _npc)
    {
        npc = _npc;
        inventoryID = _npc.npc_ID;
        currentIndex = 0;
        OpenCanvas(_open);

        layoutGroup.padding.top = 15;
        layoutGroup.padding.bottom = 40;

        slotType = SlotType.Shipyard;// SetShipyard
        toggleGroup.gameObject.SetActive(false);
        fixGroup.gameObject.SetActive(true);

        if (_open)
            SetShopItem();// 열릴때 세팅
    }

    public void SetStorage(bool _open)
    {
        inventoryID = "MyStorage";
        currentIndex = 0;
        saveData = inventoryID + currentIndex;
        OpenCanvas(_open);

        layoutGroup.padding.top = 40;
        layoutGroup.padding.bottom = 15;

        slotType = SlotType.Storage;
        toggleGroup.gameObject.SetActive(true);
        fixGroup.gameObject.SetActive(false);

        SetToggle(currentIndex);
    }

    public void SetResult(bool _open, ResultStruct _result = default)
    {
        inventoryID = "QuestResult";
        currentIndex = 0;
        OpenCanvas(_open);

        layoutGroup.padding.top = 40;
        layoutGroup.padding.bottom = 15;

        slotType = SlotType.Result;
        toggleGroup.gameObject.SetActive(false);
        fixGroup.gameObject.SetActive(false);

        if (_open)
        {
            SetShopItem();// 열릴때 세팅
            SetResultItem(_result);
        }
    }

    public void SetGhoset(bool _open)
    {
        inventoryID = "GhostInventory";
        currentIndex = 0;
        saveData = inventoryID + currentIndex;
        OpenCanvas(_open);

        layoutGroup.padding.top = 15;
        layoutGroup.padding.bottom = 15;

        slotType = SlotType.Result;
        toggleGroup.gameObject.SetActive(false);
        fixGroup.gameObject.SetActive(false);
    }

    public void OpenSubmit(bool _open)
    {
        if (_open == true)
        {
            QuestStruct _quest = Game_Manager.current.GetQuest.GetSelectQuest;
            StartCoroutine(SettingInventory(new Vector2Int(7, 7), _quest.result));
        }
        inventoryID = "OpenSubmit";
        currentIndex = 0;
        saveData = inventoryID;
        OpenCanvas(_open);

        layoutGroup.padding.top = 15;
        layoutGroup.padding.bottom = 15;

        slotType = SlotType.Submit;
        toggleGroup.gameObject.SetActive(false);
        fixGroup.gameObject.SetActive(false);
    }

    IEnumerator SettingInventory(Vector2Int _invenSize, string[] _itemID)
    {
        EmptyInventoryAllSlot();// 비우기
        SetInventorySlot(_invenSize);// 인벤토리 세팅
        yield return null;

        SetQuestItems(_itemID);
    }

    //===========================================================================================================================
    // 상점 물건 배치
    //===========================================================================================================================
    int resetDay = -1;
    public int currentIndex = 0;
    public string inventoryID;

    void SetShopItem()
    {
        Debug.LogWarning($"상점 세팅 {npc?.npc_ID}");
        saveData = inventoryID + currentIndex;
        LoadInventory();
        switch (slotType)
        {
            case SlotType.None:

                break;

            case SlotType.Shop:
            case SlotType.Shipyard:
                if (CheckResetDay() == true)
                {
                    SetItemDisplay();// 상점 물건 리셋
                }
                else
                {
                    // 저장된 내용 불러오기
                    SetInventoryItem(saveData);// Shop, Shipyard 세팅
                }
                break;

            case SlotType.Storage:
                // 저장된 내용 불러오기
                SetInventoryItem(saveData);// Storage 세팅
                break;

            case SlotType.Result:

                break;

            default:

                break;
        }
    }

    bool CheckResetDay()// 상점 물건 리셋
    {
        int checkDay = Game_Manager.current.GetTimeUI.day;
        resetDay = GetSaveInventoryData.lastSetDay;
        Debug.LogWarning($"날짜 체크!!! : {resetDay} = {checkDay}");
        if (resetDay != checkDay)
        {
            resetDay = checkDay;
            return true;
        }
        return false;
    }

    void SetItemDisplay()
    {
        StartCoroutine(DisplayItem());
    }

    IEnumerator DisplayItem()
    {
        EmptyInventoryAllSlot();// 비우기
        SetInventorySlot(npc.invenSize);// 인벤토리 세팅
        yield return null;

        SetItem();
    }

    void SetItem()// 상점 아이템 세팅
    {
        List<string> setID = new List<string>();
        int randomCount = Random.Range(10, 20);// 랜덤 아이템 개수
        for (int i = 0; i < randomCount; i++)
        {
            string itemID = npc.saleItemList.GetItemID().itemID;// 아이템 목록에서 아이템 ID 가져오기
            setID.Add(itemID);
        }
        setID.Sort();// 정렬
        // 아이템 배치
        for (int i = 0; i < setID.Count; i++)
        {
            ItemStruct item = Singleton_Data.INSTANCE.GetItemStruct(setID[i]);
            if (AddItem(item) == false)// 상점 고정 아이템 세팅
            {
                break;// 빈칸이 없으면 그만
            }
        }
    }

    public void SetResultItem(ResultStruct _result)// 상점 고정 아이템 세팅
    {
        StartCoroutine(DisplayResultItem(_result));
    }


    IEnumerator DisplayResultItem(ResultStruct _result)
    {
        EmptyInventoryAllSlot();
        SetInventorySlot(_result.inventorySize);
        yield return null;

        for (int i = 0; i < _result.itemID.Length; i++)
        {
            ItemStruct item = Singleton_Data.INSTANCE.GetItemStruct(_result.itemID[i]);
            if (AddItem(item) == false)// 보상 아이템 세팅
            {
                break;// 빈칸이 없으면 그만
            }
        }
    }
}
