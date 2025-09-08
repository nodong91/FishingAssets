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
    public Button fixButton, fixAllButton;

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
        fixButton.onClick.AddListener(FixButton);
        fixAllButton.onClick.AddListener(FixAllButton);
    }

    void SetToggle(int _index)
    {
        toggleButtons[currentIndex].buttonImage.material.SetFloat("_FillAmount", 0f);
        toggleButtons[_index].buttonImage.material.SetFloat("_FillAmount", 1f);

        if (currentIndex != _index)
        {
            // 탭 닫힐때 저장
            Static_JsonManager.SaveInventory(saveData, GetSaveInventoryData); ;// 디폴트로 저장
            currentIndex = _index;
        }
        SetShopItem();
    }

    void FixButton()
    {
        Game_Manager.current.GetInventory.OnFix = true;
    }

    void FixAllButton()
    {
        Game_Manager.current.GetInventory.FixAll();
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
    public Custom_Button[] toggleButtons;
    public void SetStorage(bool _open)
    {
        inventoryID = "MyStorage";
        currentIndex = 0;
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
                    SetInventoryItem(saveData);
                }
                break;

            case SlotType.Storage:
                // 저장된 내용 불러오기
                SetInventoryItem(saveData);
                break;

            case SlotType.Result:

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
        EmptyInventory();// 비우기
        SetInventorySlot(npc.invenSize);// 인벤토리 세팅
        yield return null;

        SetFixedItem();
        //SetRandomItem();
    }

    void SetFixedItem()// 상점 고정 아이템 세팅
    {
        //string[] setID = shopItem[currentIndex].fixedID;
        string[] setID = npc.fixedID;
        for (int i = 0; i < setID.Length; i++)
        {
            ItemStruct item = Singleton_Data.INSTANCE.GetItemStruct(setID[i]);
            if (AddItem(item) == false)
            {
                break;// 빈칸이 없으면 그만
            }
        }
    }

    void SetRandomItem()// 상점 랜덤 아이템 세팅
    {
        //List<string> setID = new List<string>(shopItem[currentIndex].randomID);
        List<string> setID = new List<string>(npc.fixedID);
        setID = P01_Utility.ShuffleList(setID, 0);

        // 아이템 반복 되지 않게 세팅
        int amount = Random.Range(0, setID.Count);
        for (int i = 0; i < amount; i++)
        {
            ItemStruct item = Singleton_Data.INSTANCE.GetItemStruct(setID[i]);
            if (AddItem(item) == false)
            {
                break;// 빈칸이 없으면 그만
            }
        }
    }

    public void SetResultItem(ResultStruct _result)// 상점 고정 아이템 세팅
    {
        //for (int i = 0; i < setID.Length; i++)
        //{
        //    ItemStruct item = Singleton_Data.INSTANCE.GetItemStruct(setID[i]);
        //    if (AddItem(item) == false)
        //    {
        //        break;// 빈칸이 없으면 그만
        //    }
        //}
        StartCoroutine(DisplayResultItem(_result));
    }


    IEnumerator DisplayResultItem(ResultStruct _result)
    {
        EmptyInventory();
        SetInventorySlot(_result.inventorySize);
        yield return null;

        for (int i = 0; i < _result.itemID.Length; i++)
        {
            ItemStruct item = Singleton_Data.INSTANCE.GetItemStruct(_result.itemID[i]);
            if (AddItem(item) == false)
            {
                break;// 빈칸이 없으면 그만
            }
        }
    }
}
