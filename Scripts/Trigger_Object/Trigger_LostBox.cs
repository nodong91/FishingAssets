using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Static_JsonManager;
using static UI_Inventory_Base;
using static UI_Inventory_Slot;

public class Trigger_LostBox : Trigger_Setting
{
    public Sprite iconImage;
    InventoryData setResult;

    public void SetResult()
    {
        Dictionary<Vector2Int, ItemInInventory> dictItem = Game_Manager.current.GetInventory.myBox.GetDictItems;
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
        //Vector2Int inventorySize = Game_Manager.current.GetInventory.myBox.inventorySize;
        Vector2Int inventorySize = Game_Manager.current.currentStatus.maxBoxSize;
        setResult = new InventoryData
        {
            invenSize = inventorySize,
            saveItems = setSaveItems,
        };

        deleTriggerAction = GhostAction;
        GetIconSprite = iconImage;// 트리거 아이콘 설정
    }

    void GhostAction()
    {
        if (setResult == null)
            return;

        StartCoroutine(SetItem());
        Debug.LogWarning("Ghost Action Triggered");
    }

    IEnumerator SetItem()
    {
        Game_Manager.current.GetInventory.shop.EmptyInventoryAllSlot();// 상점 인벤토리 초기화
        Game_Manager.current.GetInventory.shop.SetInventorySlot(setResult.invenSize);// 데이터 불러온 이후
        yield return null;

        Game_Manager.current.GetInventory.shop.LoadItem(setResult);// 상자 세팅
        Game_Manager.current.GetInventory.OpenGhost(true);// 상자 열기
        Game_Manager.current.GetMainUI.dele_CloseButton = CloseButton;
        Game_Manager.current.GetMinimap.RemoveLostBox();

        gameObject.SetActive(false);// 트리거 오브젝트 비활성화
    }

    void CloseButton()
    {
        Game_Manager.current.OutOfControll(false);
        Game_Manager.current.GetInventory.OpenGhost(false);//유령 보상 닫기
    }
}
