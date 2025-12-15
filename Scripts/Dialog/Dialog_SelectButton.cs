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

        string setText = Singleton_Data.INSTANCE.GetLanguage(ButtonName(_selectStruct));
        buttonText.text = setText;
        boxImage.gameObject.SetActive(false);
    }

    string ButtonName(SelectStruct _selectStruct)
    {
        string buttonName;
        if (_selectStruct.scriptableObject as Data_Dialog)
        {
            Data_Dialog data = _selectStruct.scriptableObject as Data_Dialog;
            buttonName = data.selectID;
        }
        else if (_selectStruct.scriptableObject as Data_Dialog_If)
        {
            Data_Dialog_If data = _selectStruct.scriptableObject as Data_Dialog_If;
            Data_Dialog dataDialog = DataDialogIf(data);
            buttonName = dataDialog.selectID;
        }
        else if (_selectStruct.scriptableObject as Data_ItemList)
        {
            Data_ItemList data = _selectStruct.scriptableObject as Data_ItemList;
            buttonName = data.selectID;
        }
        else
        {
            buttonName = DialogSelectType(_selectStruct);
        }
        return buttonName;
    }

    public static Data_Dialog DataDialogIf(Data_Dialog_If _data)
    {
        switch (_data.ifType)
        {
            case Data_Dialog_If.IfType.Loan:
                if (Game_Manager.current.GetMainUI.timeUI.loanActive == false)
                {
                    return _data.onDataDialog;
                }
                return _data.offDataDialog;
        }
        return null;
    }

    string DialogSelectType(SelectStruct _selectStruct)
    {
        switch (_selectStruct.selectType)
        {
            case SelectStruct.SelectType.Out: return Const_Dialog._exit;
            //case SelectStruct.SelectType.FishPrice: return Const_Dialog._1002;
            case SelectStruct.SelectType.Upgrade: return Const_Dialog._upgrade;
            case SelectStruct.SelectType.Rest: return Const_Dialog._rest;
            case SelectStruct.SelectType.Street: return Const_Dialog._street;
            case SelectStruct.SelectType.InLand: return Const_Dialog._inLand;
            case SelectStruct.SelectType.PayBack: return Const_Dialog._payBack;
            case SelectStruct.SelectType.GameOver: return Const_Dialog._gameOver;
            default: return null;
        }
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
