using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static Data_Dialog;
using static Data_Dialog.SelectStruct;
using static Trigger_Landing;
using static UI_Inventory_Base;

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
                // 섬 나가기
                Game_Manager.current.GetLanding.BackButton();
                break;
            case SelectType.OpenShop:
                // 상점 열기
                LandingStruct getLandingData = Game_Manager.current.GetLanding.GetLandingData;
                Game_Manager.current.GetInventory.OpenShop(getLandingData.shopNPC);
                break;
            case SelectType.OpenShipyard:
                // 조선소 열기
                getLandingData = Game_Manager.current.GetLanding.GetLandingData;
                Game_Manager.current.GetInventory.OpenShipyard(getLandingData.shipyardNPC);
                break;

            case SelectType.Upgrade:
                if (Game_Manager.current.GetPlayer.FullHealth == false)
                {
                    getLandingData = Game_Manager.current.GetLanding.GetLandingData;
                    Data_Dialog warnDialog = getLandingData.shipyardNPC.dataDialogs[1];
                    Game_Manager.current.GetDialog.DialogStart(warnDialog);
                    Debug.LogWarning("체력이 가득 차지 않았으면 스킬창 못열게");
                    return;
                }
                Debug.LogWarning("인벤토리 닫아야");
                Game_Manager.current.GetInventory.CloseShop();
                Game_Manager.current.GetSkill.OpenCanvas(true);
                Game_Manager.current.GetLanding.OutDialog();
                break;

            case SelectType.Quest:
                // 퀘스트 열기
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

                Game_Manager.current.GetQuestUI.ComplateQuest(questData);
                Game_Manager.current.GetInventory.SetResult(questData.resultData);// 퀘스트 완료 후 결과 아이템 설정
                Game_Manager.current.GetInventory.RemoveQuestItem();

                Game_Manager.current.GetDialog.DialogStart(questData.successDialogData);// 퀘스트 성공 대화 시작
            }
            else
            {
                Game_Manager.current.GetDialog.DialogStart(questData.failDialogData);// 퀘스트 실패 대화 시작
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
