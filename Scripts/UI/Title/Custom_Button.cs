using System;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Custom_Button : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image buttonImage;
    Action actionClick, actionExit;
    Action<GameObject> actionEnter;

    public void SetButton(Action _click, Action<GameObject> _enter = null,Action _exit = null)
    {
        actionClick = _click;
        actionEnter = _enter;
        actionExit= _exit;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        actionClick?.Invoke();
        transform.localScale = Vector3.one;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        actionEnter?.Invoke(this.gameObject);
        transform.localScale = Vector3.one * 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        actionExit?.Invoke();
        transform.localScale = Vector3.one;
    }
}
