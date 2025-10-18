using UnityEngine;
using static Data_Manager;

public class UI_Inventory_Infomation : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public RectTransform rectTransform;
    public TMPro.TMP_Text nameText;
    public TMPro.TMP_Text weightText, priceText, classText;

    public void SetStart(UI_Inventory_Slot _slot)
    {
        bool HideInfo = _slot == null || _slot.empty == true;
        if (HideInfo == true)
        {
            canvasGroup.alpha = 0f;
            return;
        }

        ItemStruct item = _slot.itemInInventory.item;
        nameText.text = Singleton_Data.INSTANCE.GetLanguage(item.name);

        float addPrice = 0f;
        if (Game_Manager.current.GetInventory.enterSlotType == UI_Inventory_Base.SlotType.MyBox)// 인벤토리 아이템 가격
            addPrice = item.price * Game_Manager.current.currentStatus.fishPrice * 0.01f;// 퍼센트 만큼 

        float price = Mathf.Round(item.price + addPrice);// 스킬 스탯 추가
        Debug.LogWarning($"{Singleton_Data.INSTANCE.GetLanguage(item.name)} ({item.name}) :" +
            $" {item.price} + {Game_Manager.current.currentStatus.fishPrice} = {price}");
        weightText.text = $"{item.weight}<size={weightText.fontSize * 0.5f}>kg</size>";
        priceText.text = price.ToString();
        string color = "";
        switch (item.itemClass)
        {
            case ItemStruct.ItemClass.Common:
                color = "FFFFFF";
                break;
            case ItemStruct.ItemClass.Uncommon:
                color = "00FF29";
                break;
            case ItemStruct.ItemClass.Rare:
                color = "005CFF";
                break;
            case ItemStruct.ItemClass.Epic:
                color = "FF1300";
                break;
            case ItemStruct.ItemClass.Legendary:
                color = "FFDD00";
                break;
        }
        classText.text = $"<color=#{color}>{item.itemClass}</color>";

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
