using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_FishCard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform card;
    bool unknown = false;
    public bool GetUnknown { get { return unknown; } }
    public Data_Manager.FishStruct fishStruct;
    public Image iconImage;
    public TMPro.TMP_Text nameText, idText;
    //public TMPro.TMP_Text  amount, minSize, maxSize;

    public delegate void DeleSelectCard(UI_FishCard _card);
    public DeleSelectCard deleSelectCard;

    public void SetCard(Data_Manager.FishStruct _fishStruct, FishGuide.SaveFishClass _fishClass)
    {
        fishStruct = _fishStruct;
        unknown = (_fishClass == null);
        Data_Manager.ItemStruct itemStruct = _fishStruct.itemStruct;
        nameText.text = (_fishClass != null) ? Singleton_Data.INSTANCE.GetLanguage(itemStruct.name) : "???";
        idText.text = itemStruct.id;
        iconImage.sprite = itemStruct.icon;
        iconImage.color = _fishClass != null ? Color.white : P01_Utility.HexToColor("000000CC");
        iconImage.rectTransform.sizeDelta = SetIconImage(itemStruct, 20f);
    }

    public static Vector2 SetIconImage(Data_Manager.ItemStruct _itemStruct, float _size)
    {
        return new Vector2(_itemStruct.iconSize.x, _itemStruct.iconSize.y) * _size;
    }

    public void CardDisplay(bool _onDisplay)
    {
        card.gameObject.SetActive(_onDisplay);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        card.localScale = Vector3.one * 1.1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        card.localScale = Vector3.one;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                deleSelectCard?.Invoke(this);
                break;

            case PointerEventData.InputButton.Right:

                break;

            case PointerEventData.InputButton.Middle:

                break;
        }
    }
}
