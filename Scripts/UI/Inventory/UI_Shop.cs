using System.Collections.Generic;
using UnityEngine;
using static Data_Manager;

public class UI_Shop : UI_Inventory
{

    //===========================================================================================================================
    // 상점
    //===========================================================================================================================

    [Header("- Shop")]
    public Data_Shop shopItem;

    public override void SetStart()
    {
        base.SetStart();
        //SetFixedItem();
        //SetRandomItem();
    }
    public override void OpenCanvas(bool _open)
    {
        base.OpenCanvas(_open);
    }

    void SetFixedItem()
    {
        string[] setID = shopItem.fixedID;
        for (int i = 0; i < setID.Length; i++)
        {
            ItemStruct item = Singleton_Data.INSTANCE.GetItemStruct(setID[i]);
            //if (AddItem(item) == false)
            //{
            //    break;// 빈칸이 없으면 그만
            //}
        }
    }

    void SetRandomItem()
    {
        List<string> setID = new List<string>(shopItem.randomID);
        setID = P01_Utility.ShuffleList(setID, 0);

        // 아이템 반복 되지 않게 세팅
        int amount = Random.Range(0, setID.Count);
        for (int i = 0; i < amount; i++)
        {
            ItemStruct item = Singleton_Data.INSTANCE.GetItemStruct(setID[i]);
            //if (AddItem(item) == false)
            //{
            //    break;// 빈칸이 없으면 그만
            //}
        }
    }
}
