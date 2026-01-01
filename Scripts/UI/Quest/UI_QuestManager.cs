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
    public Custom_Button actionButton;
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
    public GameObject notQuest;
    public UI_QuestSlot selectQuest;
    public QuestStruct GetSelectQuest => selectQuest.GetQuestData;
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
        Camera_Manager.current.CameraFocusOut(_open);
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
        if (_open == true)
        {
            ActiveActionButton(false);
            OpenQuestBoard();
        }
    }

    void OpenQuestBoard()
    {
        if (setQuest.currentQuest == null || setQuest.currentQuest.Count == 0)
        {
            // 정보 비우기
            SelectQuest(null);
            return;
        }

        // 첫번째 퀘스트 정보 열기
        string id = setQuest.currentQuest[0];
        Debug.LogError($"첫번째 퀘스트 정보 열기 ({dictQuest.ContainsKey(id)},{id} : {setQuest.currentQuest.Count})");
        if (dictQuest.ContainsKey(id) == true)
        {
            dictQuest[id].SelectedSlot(true);
        }
    }

    void BackButton()
    {
        Game_Manager.current.GetMainUI?.CloseCanvas();
    }

    void ActionButton()
    {
        Game_Manager.current.GetInventory.SetQuestResult();
        RemoveQuestSlot(selectQuest);
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
        OpenQuestBoard();
    }

    void SubmitButton()
    {
        Game_Manager.current.GetInventory.OpenSubmit();
        Game_Manager.current.GetMainUI.dele_CloseButton = CloseButton;
        HideCanvas(true);
    }

    void CloseButton()
    {
        Game_Manager.current.GetInventory.CloseSubMit();//퀘스트 보상 닫기
    }

    public void AddQuestSlot(QuestStruct _questDatas)
    {
        if (dictQuest.Count >= 10)// 받을 수 있는 최대 한도
        {
            Game_Manager.current.GetMainUI.SetWarnningText(Const_ETC._noQuest);
            return;
        }

        if (dictQuest.ContainsKey(_questDatas.id) == false)
        {
            setQuest.currentQuest.Add(_questDatas.id);// 슬롯 리스트 등록
            SetQuestSlot(_questDatas);
        }
        SaveQuest();
        Debug.LogWarning($"{_questDatas.title} : {dictQuest.ContainsKey(_questDatas.id)} ({dictQuest.Count})");
    }

    public void RemoveQuestSlot(UI_QuestSlot _selectSlot)
    {
        // 슬롯 제거
        questQueue.Enqueue(_selectSlot);

        string id = _selectSlot.GetQuestData.id;
        setQuest.compQuest.Add(id);
        setQuest.currentQuest.Remove(id);
        dictQuest.Remove(id);

        _selectSlot.gameObject.SetActive(false);
        selectQuest = null;
        Debug.LogError($"퀘스트 진행 완료 : {_selectSlot.GetQuestData.id}({dictQuest.Count})");
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
        if (selectQuest != null)// 기존 선택 표시 제거
        {
            selectQuest.SelectedSlot(false);// 선택 표시
        }

        notQuest.SetActive(_slot == null);
        if (_slot == null)
        {
            // 퀘스트 없음
            submitSet.SetActive(false);// 버튼 비활성화

            titleText.text = "";
            npcText.text = "";
            descriptionText.text = "";
            needText.text = "";
            resultText.text = "";
            return;
        }
        selectQuest = _slot;
        titleText.text = GetSelectQuest.title;
        npcText.text = GetSelectQuest.client;
        descriptionText.text = GetSelectQuest.description;

        // 필요한 아이템 출력
        needText.text = "";
        for (int i = 0; i < GetSelectQuest.needItem.Length; i++)
        {
            if (i > 0) needText.text += "\n";
            ItemStruct item = Singleton_Data.INSTANCE.GetItemStruct(GetSelectQuest.needItem[i]);
            needText.text += Singleton_Data.INSTANCE.GetLanguage(item.name);
        }
        // 보상 아이템 출력
        resultText.text = "";
        for (int i = 0; i < GetSelectQuest.result.Length; i++)
        {
            if (i > 0) resultText.text += "\n";
            string key = Singleton_Data.INSTANCE.GetItemStruct(GetSelectQuest.result[i]).name;
            resultText.text += Singleton_Data.INSTANCE.GetLanguage(key);
        }

        if (GetSelectQuest.needItem == null)
            return;

        bool active = Game_Manager.current.GetInventory.myBox.CheckAllSlot(GetSelectQuest.needItem);
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

    void SaveQuest()
    {
        Static_JsonManager.SaveQuestData(Const_Save._quest, setQuest);
    }

    void LoadQuest()
    {
        if (Static_JsonManager.TryLoadQuestData(Const_Save._quest, out SetQuest _data) == true)
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
