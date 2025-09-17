using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Button_Enable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CanvasGroup canvasGroup;
    public bool isEnabled = true;

    private void Start()
    {
        SetCanvasAlpha(0.3f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor_Manager.current?.OnMouseOver();
        SetCanvasAlpha(1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor_Manager.current?.OnMouseExit();
        SetCanvasAlpha(0.3f);
    }

    void SetCanvasAlpha(float _alpha)
    {
        if (canvasGroup != null)
            canvasGroup.alpha = _alpha;
    }
}
