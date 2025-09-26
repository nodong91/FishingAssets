using UnityEngine;

public class UI_Popup : MonoBehaviour
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    public Custom_Button yesButton, noButton;
    public delegate void DeleButtonAction(bool _action);
    public DeleButtonAction buttonAction;

    public void SetStart()
    {
        yesButton.SetButton(YesButton, Button_Enter, Button_Exit);
        noButton.SetButton(NoButton, Button_Enter, Button_Exit);
        Button_Exit(yesButton);
        Button_Exit(noButton);
    }

    public void OpenCanvas(bool _open)
    {
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

    void Button_Enter(Custom_Button _button)
    {
        _button.buttonImage.gameObject.SetActive(true);
    }

    void Button_Exit(Custom_Button _button)
    {
        _button.buttonImage.gameObject.SetActive(false);
    }
}
