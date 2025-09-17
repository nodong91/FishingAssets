using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Custom_Button : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image buttonImage;
    public Image GetButtonImage { get { return buttonImage; } }
    Action actionClick;
    Action<Custom_Button> actionEnter, actionExit;

    public void SetButton(Action _click, Action<Custom_Button> _enter = null, Action<Custom_Button> _exit = null)
    {
        actionClick = _click;
        actionEnter = _enter;
        actionExit = _exit;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        actionClick?.Invoke();
        transform.localScale = Vector3.one;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor_Manager.current?.OnMouseOver();
        actionEnter?.Invoke(this);
        transform.localScale = Vector3.one * 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor_Manager.current?.OnMouseExit();
        actionExit?.Invoke(this);
        transform.localScale = Vector3.one;
    }
}
