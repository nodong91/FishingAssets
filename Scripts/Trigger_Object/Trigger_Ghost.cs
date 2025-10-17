using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Static_JsonManager;
using static UI_Inventory_Base;
using static UI_Inventory_Slot;

public class Trigger_Ghost : Trigger_Setting
{
    public Sprite iconImage;
    InventoryData setResult;

    public void SetResult()
    {
        Dictionary<Vector2Int, ItemInInventory> dictItem = Game_Manager.current.GetInventory.myBox.GetInventoryItems;

        List<SaveItemClass> setSaveItems = new List<SaveItemClass>();
        foreach (var child in dictItem)
        {
            SaveItemClass dictCheck = new SaveItemClass
            {
                slotNum = child.Key,
                item = child.Value,
            };
            setSaveItems.Add(dictCheck);
        }

        setResult = new InventoryData
        {
            lastSetDay = Game_Manager.current.GetTimeUI.day,
            invenSize = Game_Manager.current.GetInventory.myBox.inventorySize,
            saveItems = setSaveItems,
        }; 
        
        deleTriggerAction = GhostAction;
        GetIconSprite = iconImage;// 트리거 아이콘 설정
    }

    void GhostAction()
    {
        if (setResult == null)
            return;
        //SetResult();
        StartCoroutine(SetItem());
        Debug.LogWarning("Ghost Action Triggered");
    }

    IEnumerator SetItem()
    {
        Game_Manager.current.GetInventory.shop.EmptyInventoryAllSlot();// 상점 인벤토리 초기화
        Game_Manager.current.GetInventory.shop.SetInventorySlot(setResult.invenSize);// 데이터 불러온 이후
        yield return null;

        Game_Manager.current.GetInventory.shop.LoadItem(setResult);
        Game_Manager.current.GetMainUI.GhostResult();// 인벤토리 열기
        gameObject.SetActive(false);// 트리거 오브젝트 비활성화
    }
}
