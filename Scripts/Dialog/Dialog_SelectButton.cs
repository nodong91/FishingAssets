using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static Data_Dialog;
using static Data_Dialog.SelectStruct;

public class Dialog_SelectButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public TMPro.TMP_Text buttonText;
    public GameObject boxImage;

    SelectStruct selectStruct;

    public Action<SelectType> clickAction;

    public void SetStart(SelectStruct _selectStruct, Action<SelectType> _clickAction)
    {
        selectStruct = _selectStruct;
        clickAction = _clickAction;
        buttonText.text = _selectStruct.selectDialog;
        boxImage.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        clickAction?.Invoke(selectStruct.selectType);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        boxImage.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        boxImage.gameObject.SetActive(false);
    }
}
