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
    public VerticalLayoutGroup layoutGroup;
    public GameObject fixGroup;
    public Custom_Button fixButton, fixAllButton;

    public override void SetStart()
    {
        base.SetStart();
        SetToggle(0);
        fixButton.SetButton(FixButton);
        fixAllButton.SetButton(FixAllButton);
    }

    void SetToggle(int _index)
    {
        if (currentIndex != _index)
        {
            Static_JsonManager.SaveInventory(saveData, GetSaveInventoryData); ;  // 토글 변경 시 저장
            currentIndex = _index;
            Debug.LogWarning($"토글 변경 {currentIndex}");
        }
        SetStorageItem();// SetToggle 토글 변경
    }

    void FixButton()
    {
        Game_Manager.current.GetInventory.RepairMode(true);
    }

    void FixAllButton()
    {
        Game_Manager.current.GetInventory.AllRepair(false);
    }

    public override void OpenCanvas(bool _open)
    {
        base.OpenCanvas(_open);
    }

    public void SetShop(bool _open, Data_ItemList _itemList)
    {
        Data_NPC data_NPC = Singleton_Data.INSTANCE.Dict_NPC[Const_NPC._shop];
        inventoryID = data_NPC.npc_ID;
        currentIndex = 0;
        saveData = inventoryID + currentIndex;
        if (_open)
        {
            Debug.LogWarning($"{Game_Manager.current.shopReset}  : {saveData}");
            OpenCanvas(true);

            layoutGroup.padding.bottom = 15;

            slotType = SlotType.Shop;// SetShop
            fixGroup.gameObject.SetActive(false);

            if (Game_Manager.current.shopReset == true)
            {
                Game_Manager.current.shopReset = false;
                SetItemDisplay(_itemList);// 생선상점 물건 리셋
            }
            else
            {
                // 저장된 내용 불러오기
                SetInventoryItem(saveData);// Shop 세팅
            }
        }
        else
        {
            OpenCanvas(false);
        }
    }

    public void SetShipyard(bool _open, Data_ItemList _itemList)
    {
        Data_NPC data_NPC = Singleton_Data.INSTANCE.Dict_NPC[Const_NPC._shipyard];
        inventoryID = data_NPC.npc_ID;
        currentIndex = 0;
        saveData = inventoryID + currentIndex;
        if (_open)
        {
            OpenCanvas(true);

            layoutGroup.padding.bottom = 40;

            slotType = SlotType.Shipyard;// SetShipyard
            fixGroup.gameObject.SetActive(true);
            if (Game_Manager.current.shipyardReset == true)
            {
                Game_Manager.current.shipyardReset = false;
                SetItemDisplay(_itemList);// 조선소 상점 물건 리셋
            }
            else
            {
                // 저장된 내용 불러오기
                SetInventoryItem(saveData);//  Shipyard 세팅
            }
        }
        else
        {
            OpenCanvas(false);
        }
    }

    public void SetInn(bool _open, Data_ItemList _itemList)
    {
        Data_NPC data_NPC = Singleton_Data.INSTANCE.Dict_NPC[Const_NPC._inn];
        inventoryID = data_NPC.npc_ID;
        currentIndex = 0;
        saveData = inventoryID + currentIndex;
        if (_open)
        {
            OpenCanvas(true);

            layoutGroup.padding.bottom = 40;

            slotType = SlotType.Shipyard;// SetShipyard
            fixGroup.gameObject.SetActive(true);
            if (Game_Manager.current.innReset == true)
            {
                Game_Manager.current.innReset = false;
                SetItemDisplay(_itemList);// 조선소 상점 물건 리셋
            }
            else
            {
                // 저장된 내용 불러오기
                SetInventoryItem(saveData);//  Shipyard 세팅
            }
        }
        else
        {
            OpenCanvas(false);
        }
    }

    public void SetSmuggler(bool _open, Data_ItemList _itemList)
    {
        Data_NPC data_NPC = Singleton_Data.INSTANCE.Dict_NPC[Const_NPC._smuggler];
        inventoryID = data_NPC.npc_ID;
        currentIndex = 0;
        saveData = inventoryID + currentIndex;
        if (_open)
        {
            OpenCanvas(_open);

            layoutGroup.padding.bottom = 40;

            slotType = SlotType.Shipyard;// SetShipyard
            fixGroup.gameObject.SetActive(true);
            if (Game_Manager.current.smugglerReset == true)
            {
                Game_Manager.current.smugglerReset = false;
                SetItemDisplay(_itemList);// 밀수꾼 상점 물건 리셋
            }
            else
            {
                // 저장된 내용 불러오기
                SetInventoryItem(saveData);// SetSmuggler 세팅
            }
        }
    }

    public void SetStorage(bool _open)
    {
        inventoryID = "MyStorage";
        currentIndex = 0;
        saveData = inventoryID + currentIndex;
        OpenCanvas(_open);

        layoutGroup.padding.bottom = 15;

        slotType = SlotType.Storage;
        fixGroup.gameObject.SetActive(false);

        SetToggle(currentIndex);
    }

    public void SetResult(bool _open, ResultStruct _result = default)
    {
        inventoryID = "Result";
        currentIndex = 0;
        saveData = inventoryID + currentIndex;
        OpenCanvas(_open);

        layoutGroup.padding.bottom = 15;

        slotType = SlotType.Result;
        fixGroup.gameObject.SetActive(false);

        if (_open)
        {
            SetResultItem(_result);
        }
    }

    public void SetGhoset(bool _open)
    {
        inventoryID = "GhostInventory";
        currentIndex = 0;
        saveData = inventoryID + currentIndex;
        OpenCanvas(_open);

        layoutGroup.padding.bottom = 15;

        slotType = SlotType.Result;
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

        layoutGroup.padding.bottom = 15;

        slotType = SlotType.Submit;
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

    public int currentIndex = 0;
    public string inventoryID;
    public Data_ItemList itemList;
    Vector2Int invenSize = new Vector2Int(7, 7);

    void SetStorageItem()
    {
        saveData = inventoryID + currentIndex;
        // 저장된 내용 불러오기
        SetInventoryItem(saveData);// Storage 세팅
    }

    void SetItemDisplay(Data_ItemList _itemList)
    {
        itemList = _itemList;
        Debug.LogWarning($"{itemList.inventoryType}");
        StartCoroutine(DisplayItem());
    }

    IEnumerator DisplayItem()
    {
        EmptyInventoryAllSlot();// 비우기
        SetInventorySlot(invenSize);// 인벤토리 세팅
        yield return null;

        SetItem();
    }

    void SetItem()// 상점 아이템 세팅
    {
        int randomCount = Random.Range(itemList.randomAmount.x, itemList.randomAmount.y);// 랜덤 아이템 개수
        Debug.LogWarning($"상점 아이템 세팅 : {randomCount}");
        string[] items = itemList.GetRandomItems(randomCount);
        System.Array.Sort(items);// 아이템 정렬
        // 아이템 배치
        for (int i = 0; i < items.Length; i++)
        {
            ItemStruct item = Singleton_Data.INSTANCE.GetItemStruct(items[i]);
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


    //===========================================================================================================================
    // 상점 시세
    //===========================================================================================================================

    //public int GetShopPrice(ItemStruct _item)
    //{
    //    int price = _item.price;
    //    // 시세 변동
    //    float fluctuation = Singleton_Data.INSTANCE.shopFluctuation;
    //    float randomFactor = Random.Range(-fluctuation, fluctuation);
    //    price = Mathf.RoundToInt(price * (1 + randomFactor));
    //    // 최소 가격 보장
    //    if (price < 1)
    //    {
    //        price = 1;
    //    }
    //    return price;
    //}
}
