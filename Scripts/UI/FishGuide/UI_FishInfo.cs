using UnityEngine;
using UnityEngine.UI;

public class UI_FishInfo : MonoBehaviour
{
    public Image iconImage;
    public TMPro.TMP_Text nameText, idText, classText;
    public TMPro.TMP_Text maxSizeText, priceText, discriptionText;

    public void SetStart(UI_FishCard _card)
    {
        iconImage.sprite = _card.iconImage.sprite;
        iconImage.color = _card.iconImage.color;

        Data_Manager.ItemStruct itemStruct = _card.fishStruct.itemStruct;
        iconImage.rectTransform.sizeDelta = UI_FishCard.SetIconImage(itemStruct, 40f);

        SetItemStruct(itemStruct, _card.GetUnknown);
    }

    void SetItemStruct(Data_Manager.ItemStruct _itemStruct, bool _unknown)
    {
        string unknown = "???";
        nameText.text = _unknown ? unknown : Singleton_Data.INSTANCE.GetLanguage(_itemStruct.name);
        idText.text = _unknown ? unknown : _itemStruct.id;
        classText.text = _unknown ? unknown : _itemStruct.itemClass.ToString();
        maxSizeText.text = _unknown ? unknown : _itemStruct.weight.ToString();
        priceText.text = _unknown ? unknown : _itemStruct.price.ToString();
        discriptionText.alignment = _unknown ? TMPro.TextAlignmentOptions.Center : TMPro.TextAlignmentOptions.TopLeft;
        discriptionText.text = _unknown ? unknown : Singleton_Data.INSTANCE.GetLanguage(_itemStruct.explanation);
    }
}
