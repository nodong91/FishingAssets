using UnityEngine;

public class UI_Popup : MonoBehaviour
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    public Custom_Button yesButton, noButton;
    public delegate void DeleButtonAction(bool _action);
    public DeleButtonAction buttonAction;

    public TMPro.TMP_Text activeText;
    public TMPro.TMP_Text yesText, noText;

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
        activeText.text = Singleton_Data.INSTANCE.GetLanguage(Const_ETC._1025);
        yesText.text = Singleton_Data.INSTANCE.GetLanguage(Const_ETC._1023);
        noText.text = Singleton_Data.INSTANCE.GetLanguage(Const_ETC._1024);
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
