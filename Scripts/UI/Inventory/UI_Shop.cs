using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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
    public VerticalLayoutGroup layoutGroup;
    public ToggleGroup toggleGroup;
    public Toggle[] groupToggles;
    public GameObject fixGroup;
    public Button fixButton, fixAllButton;

    public override void SetStart()
    {
        base.SetStart();
        shopItem = new Data_Shop[groupToggles.Length];
        for (int i = 0; i < groupToggles.Length; i++)
        {
            int index = i;
            groupToggles[i].onValueChanged.AddListener(delegate { SetToggle(index); });
        }
        fixButton.onClick.AddListener(FixButton);
        fixAllButton.onClick.AddListener(FixAllButton);
    }

    public void CloseButton()
    {
        //Static_JsonManager.SaveInventory(saveData, GetSaveInventoryData); ;// 창닫힐 때 저장
    }

    void SetToggle(int _index)
    {
        if (groupToggles[_index].isOn == true)
        {
            currentIndex = _index;
            SetShopItem();
        }
        else
        {
            // 탭 닫힐때 저장
            Static_JsonManager.SaveInventory(saveData, GetSaveInventoryData); ;// 디폴트로 저장
        }
    }

    void FixButton()
    {
        Game_Manager.current.inventory.OnFix = true;
    }

    void FixAllButton()
    {
        Game_Manager.current.inventory.FixAll();
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

        layoutGroup.padding.top = 15;
        layoutGroup.padding.bottom = 15;

        slotType = SlotType.Shop;
        toggleGroup.gameObject.SetActive(false);
        fixGroup.gameObject.SetActive(false);
        if (_open)
            SetShopItem();// 열릴때 세팅
        OpenCanvas(_open);
    }

    public void SetShipyard(bool _open, LandingStruct _landingStruct)
    {
        landingID = _landingStruct.landingID + "_Shipyard";
        currentIndex = 0;
        shopItem = _landingStruct.shipyardData;

        layoutGroup.padding.top = 40;
        layoutGroup.padding.bottom = 40;

        slotType = SlotType.Shop;
        toggleGroup.gameObject.SetActive(true);
        fixGroup.gameObject.SetActive(true);
        groupToggles[0].isOn = true;// 첫번째 탭 열기

        OpenCanvas(_open);
    }

    public void SetStorage(bool _open)
    {
        landingID = "MyStorage";
        currentIndex = 0;
        //shopItem = _landingStruct.shipyardData;

        layoutGroup.padding.top = 15;
        layoutGroup.padding.bottom = 15;

        slotType = SlotType.Storage;
        toggleGroup.gameObject.SetActive(false);
        fixGroup.gameObject.SetActive(false);

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
        Debug.LogWarning("상점 세팅");
        saveData = landingID + currentIndex;
        LoadInventory();
        if (CheckResetDay() == true)
        {
            SetItemDisplay();
        }
        else
        {
            // 저장된 내용 불러오기
            SetInventoryItem(saveData);
        }
    }

    bool CheckResetDay()// 상점 물건 리셋
    {
        int checkDay = Game_Manager.current.timeUI.day;
        resetDay = GetSaveInventoryData.lastSetDay;
        Debug.LogWarning("날짜 체크!!! " + checkDay + " " + resetDay);
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
        EmptyInventory();
        SetDefault();
        yield return new WaitForEndOfFrame();

        SetFixedItem();
        //SetRandomItem();
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
