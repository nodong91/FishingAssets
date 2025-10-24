using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestManager : MonoBehaviour
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    public GridLayoutGroup gridLayout;
    public UI_QuestSlot questSlot;
    Dictionary<string, UI_QuestSlot> dictQuest = new Dictionary<string, UI_QuestSlot>();
    public Custom_Button backButton;

    public UI_QuestSlot selectSlot;

    public void SetStart()
    {
        backButton.SetButton(BackButton);
        SetQuestSlot();
        OpenCanvas(false);
    }

    void SetQuestSlot()
    {
        slotList.Clear();
        // 로드해서 슬롯 세팅
        foreach (var child in dictQuest)
        {
            //Data_Quest quest = child.Value;

        }
    }

    public void OpenCanvas(bool _open)
    {
        Camera_Manager.current.CameraFocus(_open);
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));

        if (_open == true)
        {
            if (slotList == null || slotList.Count == 0)
                return;
            slotList[0].SelectedSlot(true);
        }
    }

    void BackButton()
    {
        Game_Manager.current.GetMainUI.CloseCanvas();
    }

    public void AddQuestSlot(Data_Quest _questDatas)
    {
        if (dictQuest.ContainsKey(_questDatas.id) == true)
        {

        }
        else
        {
            // 슬롯 추가
            UI_QuestSlot instSlot = TryQuestSlot();
            instSlot.gameObject.SetActive(true);
            instSlot.SetQuestSlot(_questDatas);
            dictQuest[_questDatas.id] = instSlot;// 사전 등록
            slotList.Add(instSlot);// 슬롯 리스트 등록
        }
        Debug.LogWarning($"{_questDatas.title} : {dictQuest.ContainsKey(_questDatas.id)} ({dictQuest.Count})");
    }

    public void RemoveQuestSlot(Data_Quest _questDatas)
    {
        // 슬롯 제거
        UI_QuestSlot findSlot = dictQuest[_questDatas.id];
        findSlot.gameObject.SetActive(false);
        questQueue.Enqueue(findSlot);
        dictQuest.Remove(_questDatas.id);
    }

    public TMPro.TMP_Text titleText;
    public TMPro.TMP_Text npcText;
    public TMPro.TMP_Text descriptionText;
    void DisplayQuest(UI_QuestSlot _slot)
    {
        if (selectSlot != null)
        {
            selectSlot.SelectedSlot(false);
        }
        selectSlot = _slot;
        titleText.text = _slot.questData.title;
        npcText.text = _slot.questData.npc_ID;
        descriptionText.text = _slot.questData.description;
    }

    Queue<UI_QuestSlot> questQueue = new Queue<UI_QuestSlot>();
    List<UI_QuestSlot> slotList = new List<UI_QuestSlot>();
    UI_QuestSlot TryQuestSlot()
    {
        if (questQueue.Count > 0)
            return questQueue.Dequeue();
        UI_QuestSlot inst = Instantiate(questSlot, gridLayout.transform);
        inst.SetStart();
        inst.slotClick = DisplayQuest;
        return inst;
    }
}
