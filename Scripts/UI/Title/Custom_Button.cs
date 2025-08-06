using UnityEngine;
using UnityEngine.EventSystems;

public class Custom_Button : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void DeleClickedHandler();
    public DeleClickedHandler deleClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        deleClicked?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = Vector3.one * 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
    }
}
