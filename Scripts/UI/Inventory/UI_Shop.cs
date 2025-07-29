using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;
using static Trigger_Landing;

public class UI_Shop : UI_Inventory_Base
{

    //===========================================================================================================================
    // 상점
    //===========================================================================================================================

    [Header("- Shop")]
    public Data_Shop[] shopItem;
    public ToggleGroup toggleGroup;
    public Toggle[] groupToggles;

    public override void SetStart()
    {
        base.SetStart();
        shopItem = new Data_Shop[groupToggles.Length];
        for (int i = 0; i < groupToggles.Length; i++)
        {
            int index = i;
            groupToggles[i].onValueChanged.AddListener(delegate { SetToggle(index); });
        }
        //groupToggles[0].isOn = true;
    }

    void SetToggle(int _index)
    {
        if (currentIndex != _index)
        {
            currentIndex = _index;
            SetShopItem();
        }
    }

    public override void OpenCanvas(bool _open)
    {
        base.OpenCanvas(_open);
    }

    public void SetShop(bool _open, LandingStruct _landingStruct)
    {
        landingID = _landingStruct.landingID + "_Shop";
        currentIndex = 0;
        shopItem[currentIndex] = _landingStruct.shopData;

        slotType = SlotType.Shop;
        toggleGroup.gameObject.SetActive(false);
        if (_open)
            SetShopItem();// 열릴때 세팅
        OpenCanvas(_open);
    }

    public void SetShipyard(bool _open, LandingStruct _landingStruct)
    {
        groupToggles[0].isOn = true;// 첫번째 탭 열기
        landingID = _landingStruct.landingID + "_Shipyard";
        currentIndex = 0;
        shopItem = _landingStruct.shipyardData;

        slotType = SlotType.Shop;
        toggleGroup.gameObject.SetActive(true);
        if (_open)
            SetShopItem();
        OpenCanvas(_open);
    }

    public void SetStorage(bool _open)
    {
        landingID = "MyStorage";
        slotType = SlotType.Storage;
        toggleGroup.gameObject.SetActive(false);
        OpenCanvas(_open);
    }

    //===========================================================================================================================
    // 상점 물건 배치
    //===========================================================================================================================
    public int resetDay = 0;
    public int currentIndex = 0;
    public string landingID;
    void SetShopItem()
    {
        string saveData = landingID + currentIndex;
        if (CheckResetDay() == true)
        {
            EmptyInventory();
            SetFixedItem();
            SetRandomItem();
            //Static_JsonManager.SaveInventory(saveData, GetSaveInventoryData); ;// 디폴트로 저장
        }
        else
        {
            // 저장된 내용 불러오기
            SetInventoryItem(saveData);
        }
    }

    bool CheckResetDay()
    {
        if (SaveData_Continue.current.setSaveContinue == null )
            return false;

        int checkDay = Game_Manager.current.timeUI.day;
        resetDay = GetSaveInventoryData.lastSetDay;
        Debug.LogWarning(SaveData_Continue.current.setSaveContinue + " " + checkDay + " " + resetDay);
        if (resetDay != checkDay)
        {
            resetDay = checkDay;
            return true;
        }
        return false;
    }

    void SetFixedItem()
    {
        string[] setID = shopItem[currentIndex].fixedID;
        for (int i = 0; i < setID.Length; i++)
        {
            ItemStruct item = Singleton_Data.INSTANCE.GetItemStruct(setID[i]);
            if (AddItem(item) == false)
            {
                break;// 빈칸이 없으면 그만
            }
        }
    }

    void SetRandomItem()
    {
        List<string> setID = new List<string>(shopItem[currentIndex].randomID);
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
}
