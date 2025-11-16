using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Gamble_Lotto_Slot : MonoBehaviour, IPointerEnterHandler
{
    public delegate void DeleEnterSlot(Gamble_Lotto_Slot _slot);
    public DeleEnterSlot deleEnterSlot;

    public Image iconImage;
    public int price;
    public TMPro.TMP_Text priceText;

    public void SetSlot(Sprite _icon, int _price)
    {
        iconImage.sprite = _icon;
        price = _price;
        priceText.gameObject.SetActive(price > 0);
        priceText.text = _price.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        deleEnterSlot?.Invoke(this);
    }
}
