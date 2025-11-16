using System;
using static Data_Event;

public class Event_SelectButton : Custom_Button
{
    public Action<EventSelect> clickAction;
    EventSelect eventSelect;
    public TMPro.TMP_Text selectText;
    public void SetStart(EventSelect _eventSelect, Action<EventSelect> _clickAction)
    {
        eventSelect = _eventSelect;
        selectText.text = Singleton_Data.INSTANCE.GetLanguage(_eventSelect.selectDialog);
        clickAction = _clickAction;
        buttonImage.gameObject.SetActive(false);
        SetButton(ClickAction, EnterAction, ExitAction);
    }

    void ClickAction()
    {
        clickAction?.Invoke(eventSelect);
    }

    void EnterAction(Custom_Button _customButton)
    {
        buttonImage.gameObject.SetActive(true);
    }

    void ExitAction(Custom_Button _customButton)
    {
        buttonImage.gameObject.SetActive(false);
    }
}
