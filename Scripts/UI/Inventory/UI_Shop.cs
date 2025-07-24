using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;

public class UI_Shop : UI_Inventory_Base
{

    //===========================================================================================================================
    // ����
    //===========================================================================================================================

    [Header("- Shop")]
    public Data_Shop shopItem;
    public Toggle[] groupToggles;

    public override void SetStart()
    {
        base.SetStart();
        for (int i = 0; i < groupToggles.Length; i++)
        {
            int index = i;
            groupToggles[i].onValueChanged.AddListener(delegate { SetToggle(index); });
        }
        groupToggles[0].isOn = true;
    }

    void SetToggle(int _index)
    {
        switch (_index)
        {
            case 0:
                SetInventoryItem("Temp1");
                break;
            case 1:
                SetInventoryItem("Temp2");
                break;
            case 2:
                SetInventoryItem("Temp3");
                break;
        }
    }

    public override void OpenCanvas(bool _open)
    {
        base.OpenCanvas(_open);
        if (_open == false)
        {
            // ���� ���� �� ����
            SaveData_Continue.current.SetContinue();
        }
    }

    public void SetShop(bool _open)
    {
        slotType = SlotType.Shop;
        SetShopItem();
        OpenCanvas(_open);
    }

    public void SetStorage(bool _open)
    {
        slotType = SlotType.Storage;
        OpenCanvas(_open);
    }

    //===========================================================================================================================
    // ���� ���� ��ġ
    //===========================================================================================================================
    int resetDay = 0;
    void SetShopItem()
    {
        int checkDay = Game_Manager.current.timeUI.day;
        if (resetDay != checkDay)
        {
            resetDay = checkDay;

            SetFixedItem();
            SetRandomItem();
        }
    }

    void SetFixedItem()
    {
        EmptyInventory();
        string[] setID = shopItem.fixedID;
        for (int i = 0; i < setID.Length; i++)
        {
            ItemStruct item = Singleton_Data.INSTANCE.GetItemStruct(setID[i]);
            if (AddItem(item) == false)
            {
                break;// ��ĭ�� ������ �׸�
            }
        }
    }

    void SetRandomItem()
    {
        List<string> setID = new List<string>(shopItem.randomID);
        setID = P01_Utility.ShuffleList(setID, 0);

        // ������ �ݺ� ���� �ʰ� ����
        int amount = Random.Range(0, setID.Count);
        for (int i = 0; i < amount; i++)
        {
            ItemStruct item = Singleton_Data.INSTANCE.GetItemStruct(setID[i]);
            if (AddItem(item) == false)
            {
                break;// ��ĭ�� ������ �׸�
            }
        }
    }
}
