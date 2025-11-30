using UnityEngine;
using UnityEngine.UI;

public class UI_FishInfo : MonoBehaviour
{
    public Image iconImage;
    public TMPro.TMP_Text nameText, idText, classText;
    public TMPro.TMP_Text maxSizeText, priceText, discriptionText;
    public GameObject sun, moon;

    public void SetStart(UI_FishCard _card)
    {
        iconImage.sprite = _card.iconImage.sprite;
        iconImage.color = _card.iconImage.color;

        Data_Manager.ItemStruct itemStruct = _card.fishStruct.itemStruct;
        iconImage.rectTransform.sizeDelta = UI_FishCard.SetIconImage(itemStruct, 40f);

        switch (_card.fishStruct.fishDayType)
        {
            case Data_Manager.DayType.Day:
                sun.SetActive(true);
                moon.SetActive(false);
                break;

            case Data_Manager.DayType.Night:
                sun.SetActive(false);
                moon.SetActive(true);
                break;

            case Data_Manager.DayType.Any:
                sun.SetActive(true);
                moon.SetActive(true);
                break;
        }
        //SetItemStruct(itemStruct, _card.GetUnknown);
        SetItemStruct(itemStruct, false);
    }

    void SetItemStruct(Data_Manager.ItemStruct _itemStruct, bool _unknown)
    {
        string unknown = "???";
        nameText.text = _unknown ? unknown : Singleton_Data.INSTANCE.GetLanguage(_itemStruct.id);
        idText.text = _unknown ? unknown : _itemStruct.id;

        string color = P01_Utility.ClassColor(_itemStruct.itemClass);
        classText.text = _unknown ? unknown : $"<color=#{color}>{_itemStruct.itemClass}</color>";

        maxSizeText.text = _unknown ? unknown : _itemStruct.weight.ToString();
        priceText.text = _unknown ? unknown : _itemStruct.price.ToString();

        discriptionText.alignment = _unknown ? TMPro.TextAlignmentOptions.Center : TMPro.TextAlignmentOptions.TopLeft;
        discriptionText.text = _unknown ? unknown : Singleton_Data.INSTANCE.GetLanguage(_itemStruct.explanation);
    }
}
