using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Custom_Button : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    Action actionClick;
    Action actionEnter;

    public void SetButton(Action _click, Action _enter = null)
    {
        actionClick = _click;
        actionEnter = _enter;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        actionClick?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        actionEnter?.Invoke();
        transform.localScale = Vector3.one * 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
    }
}
