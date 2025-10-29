using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static Data_Dialog;

public class Dialog_SelectButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public TMPro.TMP_Text buttonText;
    public GameObject boxImage;

    SelectStruct selectStruct;

    public Action<SelectStruct> clickAction;

    public void SetStart(SelectStruct _selectStruct, Action<SelectStruct> _clickAction)
    {
        selectStruct = _selectStruct;
        clickAction = _clickAction;
        string setText = Singleton_Data.INSTANCE.GetLanguage(_selectStruct.selectDialog);
        buttonText.text = setText;
        boxImage.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        clickAction?.Invoke(selectStruct);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = Vector3.one * 1.1f;
        boxImage.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
        boxImage.gameObject.SetActive(false);
    }
}
