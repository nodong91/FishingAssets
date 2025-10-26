using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;

public class UI_QuestManager : MonoBehaviour
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    public GridLayoutGroup gridLayout;
    public GameObject submitSet;
    public Custom_Button backButton, submitButton;
    public UI_QuestSlot questSlot;
    public CanvasGroup canvasGroup;

    Queue<UI_QuestSlot> questQueue = new Queue<UI_QuestSlot>();
    Dictionary<string, UI_QuestSlot> dictQuest = new Dictionary<string, UI_QuestSlot>();
    public Dictionary<string, UI_QuestSlot> GetDictQuest { get { return dictQuest; } }

    public TMPro.TMP_Text titleText;
    public TMPro.TMP_Text npcText;
    public TMPro.TMP_Text descriptionText;
    public TMPro.TMP_Text needText;
    public TMPro.TMP_Text resultText;

    public UI_QuestSlot selectQuest;
    public Custom_Button actionButton;
    public void SetStart()
    {
        backButton.SetButton(BackButton);
        submitButton.SetButton(SubmitButton);
        actionButton.SetButton(ActionButton);
        ActiveActionButton(false);
        SetQuestSlot();
        OpenCanvas(false);
    }

    void SetQuestSlot()
    {
        dictQuest.Clear();
        LoadQuest();
        // 로드해서 슬롯 세팅
        for (int i = 0; i < setQuest.currentQuest.Count; i++)
        {
            string id = setQuest.currentQuest[i];
            QuestStruct questStruct = Singleton_Data.INSTANCE.Dict_Quest[id];
            SetQuestSlot(questStruct);
        }
    }

    public void OpenCanvas(bool _open)
    {
        Camera_Manager.current.CameraFocus(_open);
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
        if (_open == true)
        {
            if (setQuest.currentQuest == null || setQuest.currentQuest.Count == 0)
                return;

            ActiveActionButton(false);
            string id = setQuest.currentQuest[0];
            if (dictQuest.ContainsKey(id))
                dictQuest[id].SelectedSlot(true);
        }
    }

    void BackButton()
    {
        Game_Manager.current.GetMainUI.CloseCanvas();
    }

    void ActionButton()
    {
        Game_Manager.current.GetInventory.SetQuestResult();
        ActiveActionButton(false);
    }

    public void ActiveActionButton(bool _on)
    {
        actionButton.gameObject.SetActive(_on);
    }

    public void HideCanvas(bool _hide)
    {
        canvasGroup.alpha = _hide == true ? 0f : 1f;
        canvasGroup.interactable = canvasGroup.alpha > 0;
        canvasGroup.blocksRaycasts = canvasGroup.alpha > 0;
    }

    public void SubmitBackButton()
    {
        HideCanvas(false);
        // 퀘스트 첫번째 활성화
        string id = setQuest.currentQuest[0];
        if (dictQuest.ContainsKey(id))
            dictQuest[id].SelectedSlot(true);
    }

    void SubmitButton()
    {
        Game_Manager.current.GetMainUI.OpenSubmit();
        HideCanvas(true);
    }

    public void AddQuestSlot(QuestStruct _questDatas)
    {
        if (dictQuest.Count >= 10)// 받을 수 있는 최대 한도
        {
            Game_Manager.current.GetMainUI.SetWarnningText("더 이상 퀘스트를 받을 수 없음");
            return;
        }

        if (dictQuest.ContainsKey(_questDatas.id) == false)
        {
            setQuest.currentQuest.Add(_questDatas.id);// 슬롯 리스트 등록
            SetQuestSlot(_questDatas);
        }
        SaveQuest();
        Debug.LogWarning($"{_questDatas.name} : {dictQuest.ContainsKey(_questDatas.id)} ({dictQuest.Count})");
    }

    public void RemoveQuestSlot()
    {
        // 슬롯 제거
        UI_QuestSlot findSlot = selectQuest;
        findSlot.gameObject.SetActive(false);
        questQueue.Enqueue(findSlot);
        dictQuest.Remove(findSlot.questData.id);
        setQuest.compQuest.Add(findSlot.questData.id);
        selectQuest = null;
    }

    void SetQuestSlot(QuestStruct _questDatas)
    {
        // 슬롯 추가
        UI_QuestSlot instSlot = TryQuestSlot();
        instSlot.gameObject.SetActive(true);
        instSlot.SetQuestSlot(_questDatas);

        dictQuest[_questDatas.id] = instSlot;// 사전 등록
    }

    void SelectQuest(UI_QuestSlot _slot)// 퀘스트 인포메이션 세팅
    {
        if (selectQuest != null)
        {
            selectQuest.SelectedSlot(false);
        }
        selectQuest = _slot;
        titleText.text = _slot.questData.name;
        npcText.text = _slot.questData.client;
        descriptionText.text = _slot.questData.description;
        // 필요한 아이템 출력
        needText.text = "";
        for (int i = 0; i < _slot.questData.needItem.Length; i++)
        {
            if (i > 0) needText.text += "\n";
            ItemStruct item = Singleton_Data.INSTANCE.GetItemStruct(_slot.questData.needItem[i]);
            needText.text += Singleton_Data.INSTANCE.GetLanguage(item.name);
        }
        // 보상 아이템 출력
        resultText.text = "";
        for (int i = 0; i < _slot.questData.result.Length; i++)
        {
            if (i > 0) resultText.text += "\n";
            string key = Singleton_Data.INSTANCE.GetItemStruct(_slot.questData.result[i]).name;
            resultText.text += Singleton_Data.INSTANCE.GetLanguage(key);
        }

        if (_slot.questData.needItem == null)
            return;

        bool active = Game_Manager.current.GetInventory.myBox.CheckAllSlot(_slot.questData.needItem);
        submitSet.SetActive(active);// 버튼 활성화
    }

    UI_QuestSlot TryQuestSlot()
    {
        if (questQueue.Count > 0)
            return questQueue.Dequeue();
        UI_QuestSlot inst = Instantiate(questSlot, gridLayout.transform);
        inst.SetStart();
        inst.slotClick = SelectQuest;
        return inst;
    }

    //===================================================================================================
    // 데이터 저장 앤 로드
    //===================================================================================================
    [System.Serializable]
    public struct SetQuest
    {
        public List<string> currentQuest;
        public List<string> compQuest;// 완료 퀘스트
    }
    public SetQuest setQuest;
    const string questData = "SaveQuestData";

    void SaveQuest()
    {
        Static_JsonManager.SaveQuestData(questData, setQuest);
    }

    void LoadQuest()
    {
        if (Static_JsonManager.TryLoadQuestData(questData, out SetQuest _data) == true)
        {
            setQuest = _data;
        }
        else
        {
            setQuest = new SetQuest
            {
                currentQuest = new List<string>(),
                compQuest = new List<string>()
            };
        }
    }
}
