using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Custom_Button : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum OverStyle
    {
        None = 0,
        Scale = 1,
    }
    public OverStyle overStyle = OverStyle.Scale;
    public Image buttonImage;
    public Image GetButtonImage { get { return buttonImage; } }

    Action actionClick;
    Action<Custom_Button> actionEnter, actionExit;
    Coroutine action;

    public void SetButton(Action _click, Action<Custom_Button> _enter = null, Action<Custom_Button> _exit = null)
    {
        actionClick = _click;
        actionEnter = _enter;
        actionExit = _exit;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (action == null)
            action = StartCoroutine(ClickAnimation());
    }

    IEnumerator ClickAnimation()
    {
        float prevSize = 1f;
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 10f;
            float currentSize = Mathf.Lerp(prevSize, 1.2f, normalize);
            transform.localScale = Vector3.one * currentSize;
            yield return null;
        }
        actionClick?.Invoke();
        yield return new WaitForSeconds(0.5f);
        action = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor_Manager.current?.OnMouseOver();
        actionEnter?.Invoke(this);
        switch (overStyle)
        {
            case OverStyle.None:

                break;

            case OverStyle.Scale:
                transform.localScale = Vector3.one * 1.2f;
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor_Manager.current?.OnMouseExit();
        actionExit?.Invoke(this);
        switch (overStyle)
        {
            case OverStyle.None:

                break;

            case OverStyle.Scale:
                transform.localScale = Vector3.one;
                break;
        }
    }
}
