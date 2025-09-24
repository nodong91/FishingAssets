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
        nameText.text = itemStruct.name;
        idText.text = itemStruct.id;
        classText.text = itemStruct.itemClass.ToString();
        maxSizeText.text = itemStruct.weight.ToString();
        priceText.text = itemStruct.price.ToString();
        discriptionText.text = itemStruct.explanation.ToString();
        iconImage.rectTransform.sizeDelta = UI_FishCard.SetIconImage(itemStruct, 40f);

    }
}
