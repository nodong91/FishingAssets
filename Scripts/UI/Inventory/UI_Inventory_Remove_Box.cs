using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inventory_Remove_Box : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image iconImage;
    public Sprite openSprite, closeSprite;

    public delegate void Dele_Remove();
    public Dele_Remove deleRemove;

    public void OnPointerClick(PointerEventData eventData)
    {
        deleRemove?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 쓰레기통 열린 이미지
        iconImage.sprite = openSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 쓰레기통 닫힌 이미지
        iconImage.sprite = closeSprite;
    }
}
