using UnityEngine;
using static Data_Manager;

public class UI_Inventory_Infomation : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public RectTransform rectTransform;
    public TMPro.TMP_Text nameText;
    public TMPro.TMP_Text priceText, typeText;

    public void SetStart(UI_Inventory_Slot _slot)
    {
        bool HideInfo = _slot == null || _slot.empty == true;
        if (HideInfo == true)
        {
            canvasGroup.alpha = 0f;
            return;
        }

        ItemStruct item = _slot.itemClass.item;
        nameText.text = Singleton_Data.INSTANCE.GetLanguage(item.name);

        float addPrice = 0f;
        if (Game_Manager.current.GetInventory.enterSlotType == UI_Inventory_Base.SlotType.MyBox)// 인벤토리 아이템 가격
            addPrice = item.price * Game_Manager.current.currentStatus.FishPrice * 0.01f;// 퍼센트 만큼 

        float price = Mathf.Round(item.price + addPrice);// 스킬 스탯 추가
        Debug.LogWarning($"{Singleton_Data.INSTANCE.GetLanguage(item.name)} ({item.name}) : {item.price} + {Game_Manager.current.currentStatus.FishPrice} = {price}");
        priceText.text = price.ToString();
        typeText.text = item.itemType.ToString();

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
