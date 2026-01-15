using UnityEngine;
using static Data_Manager;

public class UI_Inventory_Infomation : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public RectTransform rectTransform;
    public TMPro.TMP_Text nameText;
    public TMPro.TMP_Text weightText, priceText, classText, typeText;

    public void SetStart(UI_Inventory_Slot _slot)
    {
        bool HideInfo = _slot == null || _slot.empty == true;
        if (HideInfo == true)
        {
            canvasGroup.alpha = 0f;
            return;
        }

        ItemStruct item = _slot.itemInInventory.item;
        nameText.text = Singleton_Data.INSTANCE.GetLanguage(item.id);

        UI_Inventory_Base.SlotType currentType = Game_Manager.current.GetInventory.currentType;
        bool isPrice = (currentType == UI_Inventory_Base.SlotType.Shop ||
         currentType == UI_Inventory_Base.SlotType.Shipyard ||
        currentType == UI_Inventory_Base.SlotType.Submit);
        priceText.gameObject.SetActive(isPrice);
        if (isPrice == true)
        {
            float addPrice = 0f;
            if (Game_Manager.current.GetInventory.enterSlotType == UI_Inventory_Base.SlotType.MyBox)// 인벤토리 아이템 가격
                addPrice = item.price * Game_Manager.current.currentStatus.fishPrice * 0.01f;// 퍼센트 만큼 

            float price = Mathf.Round(item.price + addPrice);// 스킬 스탯 추가
            Debug.Log($"{Singleton_Data.INSTANCE.GetLanguage(item.id)} ({item.id}) :" +
                $" {item.price} + {Game_Manager.current.currentStatus.fishPrice} = {price}");
            priceText.text = price.ToString();
        }

        string classColor = P01_Utility.ClassColor(item.itemClass);
        classText.text = $"<color=#{classColor}>{item.itemClass}</color>";
        string typeColor = "B3B3B3";
        switch (item.itemType)
        {
            case ItemStruct.ItemType.Fish:
                typeColor = "30AEEA";
                break;
            case ItemStruct.ItemType.Fuel:
                typeColor = "222222";
                break;
            case ItemStruct.ItemType.Buff:
                typeColor = "1DE02B";
                break;
            case ItemStruct.ItemType.Repair:
                typeColor = "FFC300";
                break;
            case ItemStruct.ItemType.Bait:
                typeColor = "FF5733";
                break;
            case ItemStruct.ItemType.Etc:
                typeColor = "B3B3B3";
                break;
        }
        typeText.text = $"<color=#{typeColor}>{item.itemType}</color>";
        if (Game_Manager.current.GetInventory.enterSlotType == UI_Inventory_Base.SlotType.MyBox)
        {
            rectTransform.pivot = new Vector2(1f, 0.5f);
            rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(0f, 0.5f);
            RectTransform infoRect = Game_Manager.current.GetInventory.myBox.infomationRect;
            rectTransform.transform.position = new Vector2(infoRect.transform.position.x, _slot.GetLinkSlot.transform.position.y);
        }
        else
        {
            rectTransform.pivot = new Vector2(0f, 0.5f);
            rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(1f, 0.5f);
            RectTransform infoRect = Game_Manager.current.GetInventory.shop.infomationRect;
            rectTransform.transform.position = new Vector2(infoRect.transform.position.x, _slot.GetLinkSlot.transform.position.y);
        }
        canvasGroup.alpha = 1f;
    }
}
