using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_FishCard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject card;
    public Data_Manager.FishStruct fishStruct;
    public TMPro.TMP_Text text_Name, amount, minSize, maxSize;
    public Image iconImage;

    public delegate void DeleSelectCard(UI_FishCard _card);
    public DeleSelectCard deleSelectCard;

    public void SetCard(Data_Manager.FishStruct _fishStruct, FishGuide.SaveFishClass _fishClass)
    {
        fishStruct = _fishStruct;

        text_Name.gameObject.SetActive(_fishClass != null);
        amount.gameObject.SetActive(_fishClass != null);
        minSize.gameObject.SetActive(_fishClass != null);
        maxSize.gameObject.SetActive(_fishClass != null);

        if (_fishClass != null)
        {
            text_Name.text = _fishStruct.itemStruct.name;
            amount.text = _fishClass.amount.ToString();
            minSize.text = _fishClass.minSize.ToString();
            maxSize.text = _fishClass.maxSize.ToString();
        }
        iconImage.sprite = _fishStruct.itemStruct.icon;
        iconImage.color = _fishClass != null ? Color.white : Color.black;
    }

    public void CardDisplay(bool _onDisplay)
    {
        card.SetActive(_onDisplay);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        deleSelectCard?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        deleSelectCard?.Invoke(default);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:

                break;

            case PointerEventData.InputButton.Right:

                break;

            case PointerEventData.InputButton.Middle:

                break;
        }
    }
}
