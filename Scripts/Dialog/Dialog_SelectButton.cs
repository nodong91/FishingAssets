using UnityEngine;
using UnityEngine.EventSystems;
using static Data_Dialog;

public class Dialog_SelectButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public TMPro.TMP_Text buttonText;
    public GameObject boxImage;

    SelectStruct selectStruct;

    public delegate void DeleSelect(SelectStruct.SelectType _type);
    public DeleSelect deleSelect;

    public void SetStart(SelectStruct _selectStruct)
    {
        selectStruct = _selectStruct;
        buttonText.text = _selectStruct.selectDialog;
        boxImage.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        deleSelect?.Invoke(selectStruct.selectType);
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
