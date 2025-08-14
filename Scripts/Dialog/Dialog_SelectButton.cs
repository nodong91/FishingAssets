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
                Game_Manager.current.GetLanding.BackButton();
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
                    // 인벤토리 안에 해당 아이템이 있는지 확인
                    TryComplateQuest();
                }
                else
                {
                    Debug.LogWarning("Quest data is not set.");
                }
                break;

        }
    }

    void TryComplateQuest()
    {
        if (questData != null)
        {
            // 인벤토리 안에 해당 아이템이 있는지 확인
            if (Game_Manager.current.GetInventory.CheckQuestItem(questData.needItems) == true)
            {
                Debug.Log($"Quest selected: {questData.title}");
                Option_Manager.current.GetQuestManager.ComplateQuest(questData);
                Game_Manager.current.GetDialog.DialogStart(questData.successDialogData);
                Game_Manager.current.GetInventory.SetQuestResult(questData.resultData);// 퀘스트 완료 후 결과 아이템 설정
            }
            else
            {
                Game_Manager.current.GetDialog.DialogStart(questData.failDialogData);
            }
        }
        else
        {
            Debug.LogWarning("Quest data is not set.");
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
