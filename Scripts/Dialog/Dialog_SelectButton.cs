using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static Data_Dialog;
using static Data_Dialog.SelectStruct;
using static Trigger_Landing;

public class Dialog_SelectButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public TMPro.TMP_Text buttonText;
    public GameObject boxImage;
    public Data_Quest questData;

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
        switch (selectStruct.selectType)
        {
            case SelectType.Out:
                // Handle Out action
                Debug.Log("Out action selected.");
                break;
            case SelectType.OpenShop:
                // Handle OpenShop action
                Debug.Log("OpenShop action selected.");
                LandingStruct getLandingData = Game_Manager.current.GetLanding.GetLandingData;
                Game_Manager.current.GetInventory.OpenShop(getLandingData);
                break;
            case SelectType.OpenShipyard:
                // Handle OpenShipyard action
                Debug.Log("OpenShipyard action selected.");
                getLandingData = Game_Manager.current.GetLanding.GetLandingData;
                Game_Manager.current.GetInventory.OpenShipyard(getLandingData);
                break;
            case SelectType.Quest:
                // Handle Quest action
                if (questData != null)
                {
                    // Assuming you have a method to handle quest selection
                    Debug.Log($"Quest selected: {questData.title}");
                    Option_Manager.current.GetQuestManager.ComplateQuest(questData);
                    Game_Manager.current.GetDialog.DialogStart(questData.dialogData);
                }
                else
                {
                    Debug.LogWarning("Quest data is not set.");
                }
                break;

        }
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
