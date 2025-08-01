using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Data_Dialog;

public class Dialog_SelectButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public TMPro.TMP_Text buttonText;
    public Image boxImage;

    SelectStruct selectStruct;

    public delegate void DeleSelect(SelectStruct.SelectType _type);
    public DeleSelect deleSelect;

    public void SetStart(SelectStruct _selectStruct)
    {
        selectStruct = _selectStruct;
        buttonText.text = _selectStruct.selectDialog;
        boxImage.color = Color.gray;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        deleSelect?.Invoke(selectStruct.selectType);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        boxImage.color = Color.white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        boxImage.color = Color.gray;
    }
}
