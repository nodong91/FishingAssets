using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_QuestManager : MonoBehaviour
{
    public Data_Quest[] questDataTest;
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    public Dictionary<string, List<Data_Quest>> questDictionary = new Dictionary<string, List<Data_Quest>>();

    [System.Serializable]
    public class CustomButtonStruct
    {
        public bool isComplate;
        public Custom_Button customButton;
        public TMP_Text titleText;
        public Data_Quest questData;
        public CustomButtonStruct(Custom_Button _customButton)
        {
            isComplate = false;
            customButton = _customButton;
            questData = null;
            titleText = _customButton.GetComponentInChildren<TMP_Text>();
        }

        public void SetQuestData(Data_Quest _questData)
        {
            questData = _questData;
            titleText.text = _questData.title;
        }
        public void IsComplate()
        {
            isComplate = true;
        }
    }
    private List<CustomButtonStruct> customButtonList = new List<CustomButtonStruct>();
    public RectTransform questParent;
    public Custom_Button questButtonPrefab;
    public Custom_Button closeButton;

    [Header("Quest List")]
    private List<Data_Quest> questList = new List<Data_Quest>();
    private List<Data_Quest> complateList = new List<Data_Quest>();
    private Queue<CustomButtonStruct> toggleQueue = new Queue<CustomButtonStruct>();
    CustomButtonStruct currentButton;
    public TMP_Text questInfoText;

    public void SetStart()
    {
        // 초기화
        questDictionary.Clear();
        for (int i = 0; i < questDataTest.Length; i++)
        {
            Data_Quest quest = questDataTest[i];
            int index = i;
            AddQuest(quest);
        }
        closeButton.SetButton(delegate { OpenCanvas(false); });
        ToggleClick(customButtonList[0]);
        OpenCanvas(false);
    }

    public void OpenCanvas(bool _open)
    {
        if (_open)
        {
            ToggleClick(customButtonList[0]);
            StaticOpenCanvas.deleEndOpen = null;
        }
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
        Debug.LogWarning($"OpenQuestCanvas : {_open}");
    }

    void ToggleClick(CustomButtonStruct _customToggle)
    {
        SaveQuestData();
        if (currentButton?.customButton == _customToggle.customButton)
            return;

        // 퀘스트 상세 정보 표시
        SetQuestDisplay(_customToggle);
        CustomButtonStruct button = _customToggle;
        // 현재 버튼을 활성화하고 나머지는 비활성화
        button.titleText.color = Color.white;
        if (currentButton?.customButton != null)
            currentButton.titleText.color = Color.gray;

        currentButton = button;
    }

    void ToggleEnter(GameObject _button)
    {
        Debug.Log($"ToggleEnter");
    }

    CustomButtonStruct GetToggle()
    {
        if (toggleQueue.Count > 0)
            return toggleQueue.Dequeue();

        // Toggle 생성
        Custom_Button toggle = Instantiate(questButtonPrefab, questParent);
        CustomButtonStruct customButton = new CustomButtonStruct(toggle);
        return customButton;
    }

    public void AddQuest(Data_Quest _questData)
    {
        CustomButtonStruct customToggle = GetToggle();
        customToggle.titleText.color = Color.gray;
        customToggle.SetQuestData(_questData);
        customToggle.customButton.gameObject.SetActive(true);
        customToggle.customButton.SetButton(delegate { ToggleClick(customToggle); }, ToggleEnter);
        customButtonList.Add(customToggle);

        questList.Add(_questData);
        AddDictionary(_questData);
    }

    void FindQuest(Data_Quest _questData)
    {
        for (int i = 0; i < customButtonList.Count; i++)
        {
            if (_questData == customButtonList[i].questData)
            {
                customButtonList[i].IsComplate();
                return;
            }
        }
    }

    void AddDictionary(Data_Quest _questData)
    {
        string questKey = _questData.npc_ID;
        if (questDictionary.ContainsKey(questKey) == false)
        {
            // 퀘스트가 없으면 새로 추가
            List<Data_Quest> tempQuestList = new List<Data_Quest> { _questData };
            questDictionary[questKey] = tempQuestList;
        }
        else
        {
            // 퀘스트가 이미 있으면 리스트에 추가
            questDictionary[questKey].Add(_questData);
            Debug.Log($"Quest with title {questKey} already exists.");
        }
    }

    public List<Data_Quest> CheckNPC(string _npcID)
    {
        // 해당 엔피씨가 퀘스트를 가지고 있는지 확인
        if (questDictionary.ContainsKey(_npcID) == true)
        {
            // 퀘스트가 있으면 해당 퀘스트 리스트 반환
            return questDictionary[_npcID];
        }
        return new List<Data_Quest>();
    }

    //===================================================================================================
    // 퀘스트 완료
    //===================================================================================================
    public void ComplateQuest(Data_Quest _questData)
    {
        if (questDictionary.ContainsKey(_questData.npc_ID) == true)
        {
            // 퀘스트 완료 처리
            complateList.Add(_questData);
            questDictionary[_questData.npc_ID].Remove(_questData);
            FindQuest(_questData);
            Debug.LogWarning($"Quest '{_questData.title}' completed.");
        }
        else
        {
            Debug.LogWarning($"Quest with title {_questData.title} does not exist in the dictionary.");
        }
    }

    //===================================================================================================
    // 신문 정보 가져오기
    //===================================================================================================

    public void GetNewspaper(Data_Quest[] _questDatas)
    {
        for (int i = 0; i < _questDatas.Length; i++)
        {
            AddDictionary(_questDatas[i]);
        }
    }

    //===================================================================================================
    // 퀘스트 정보 표시
    //===================================================================================================

    void SetQuestDisplay(CustomButtonStruct _quest)
    {
        // 퀘스트가 있는지 확인
        string questInfo = $"{_quest.questData.title} : {_quest.questData.npc_ID}\n완료? ({_quest.isComplate})";
        questInfo += $"\n\n{_quest.questData.description}";
        if (_quest.questData.needItems != null)
        {
            questInfo += $"\n\n필요한 아이템 : ";
            for (int i = 0; i < _quest.questData.needItems.Length; i++)
            {
                questInfo += $"\n- {_quest.questData.needItems[i]}";
            }
        }
        questInfoText.text = questInfo;
    }

    //===================================================================================================
    // 퀘스트 내용 저장
    //===================================================================================================

    void SaveQuestData()
    {
        // 퀘스트 데이터를 저장하는 로직
        // 예: PlayerPrefs, 파일 저장 등
        Debug.LogError("현재 퀘스트 진행 상황 저장 해야함");
    }
}
