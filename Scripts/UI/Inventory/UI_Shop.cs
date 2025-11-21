using System.Collections;
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
        Game_Manager.current.GetInventory.AllRepair();
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
            itemList = _itemList;
            OpenCanvas(true);

            layoutGroup.padding.top = 15;
            layoutGroup.padding.bottom = 15;

            slotType = SlotType.Shop;// SetShop
            toggleGroup.gameObject.SetActive(false);
            fixGroup.gameObject.SetActive(false);
            if (Game_Manager.current.GetMainUI.timeUI.shopReset == true)
            {
                Game_Manager.current.GetMainUI.timeUI.shopReset = false;
                SetItemDisplay();// 상점 물건 리셋
            }
            else
            {
                // 저장된 내용 불러오기
                SetInventoryItem(saveData);// Shop, Shipyard 세팅
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
            itemList = _itemList;
            OpenCanvas(true);

            layoutGroup.padding.top = 15;
            layoutGroup.padding.bottom = 40;

            slotType = SlotType.Shipyard;// SetShipyard
            toggleGroup.gameObject.SetActive(false);
            fixGroup.gameObject.SetActive(true);
            if (Game_Manager.current.GetMainUI.timeUI.shipyardReset == true)
            {
                Game_Manager.current.GetMainUI.timeUI.shipyardReset = false;
                SetItemDisplay();// 상점 물건 리셋
            }
            else
            {
                // 저장된 내용 불러오기
                SetInventoryItem(saveData);// Shop, Shipyard 세팅
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
            itemList = _itemList;
            OpenCanvas(_open);

            layoutGroup.padding.top = 15;
            layoutGroup.padding.bottom = 40;

            slotType = SlotType.Shipyard;// SetShipyard
            toggleGroup.gameObject.SetActive(false);
            fixGroup.gameObject.SetActive(true);
            if (Game_Manager.current.GetMainUI.timeUI.smugglerReset == true)
            {
                Game_Manager.current.GetMainUI.timeUI.smugglerReset = false;
                SetItemDisplay();// 상점 물건 리셋
            }
            else
            {
                // 저장된 내용 불러오기
                SetInventoryItem(saveData);// Shop, Shipyard 세팅
            }
        }
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
        inventoryID = "Result";
        currentIndex = 0;
        saveData = inventoryID + currentIndex;
        OpenCanvas(_open);

        layoutGroup.padding.top = 40;
        layoutGroup.padding.bottom = 15;

        slotType = SlotType.Result;
        toggleGroup.gameObject.SetActive(false);
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

    void SetItemDisplay()
    {
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
        Debug.LogWarning("상점 아이템 세팅");
        //List<string> setID = new List<string>();
        int randomCount = Random.Range(10, 20);// 랜덤 아이템 개수
        string[] items = itemList.GetRandomItems(randomCount);
        //for (int i = 0; i < randomCount; i++)
        //{
        //    string itemID = itemList.GetItemID();// 아이템 목록에서 아이템 ID 가져오기
        //    setID.Add(itemID);
        //}
        System.Array.Sort(items);
        //setID.Sort();// 정렬
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
}
