using System.IO;
using UnityEngine;
using static Data_Manager;

public class UI_Inventory_Infomation : MonoBehaviour
{
    public CanvasGroup canvasGroup;
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
        nameText.text = item.name;
        priceText.text = item.price.ToString();
        typeText.text = item.itemType.ToString();

        canvasGroup.alpha = 1f;
        canvasGroup.transform.position = Input.mousePosition;
    }
}
