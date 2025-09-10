using UnityEngine;

public class UI_Popup : MonoBehaviour
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    public Custom_Button yesButton, noButton;
    public delegate void DeleButtonAction(bool _action);
    public DeleButtonAction buttonAction;

    private void Start()
    {
        OpenCanvas(false);
    }

    public void SetStart()
    {
        yesButton.SetButton(YesButton);
        noButton.SetButton(NoButton);
        OpenCanvas(true);
    }

    void OpenCanvas(bool _open)
    {
        StaticOpenCanvas.deleEndOpen = null;
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
    }

    void YesButton()
    {
        buttonAction?.Invoke(true);
        OpenCanvas(false);
    }

    void NoButton()
    {
        buttonAction?.Invoke(false);
        OpenCanvas(false);
    }
}
