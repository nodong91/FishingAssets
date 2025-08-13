using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestManager : MonoBehaviour
{
    public Dictionary<string, List<Data_Quest>> questDictionary = new Dictionary<string, List<Data_Quest>>();
    public TMP_Text text;
    public ToggleGroup questToggleGroup;
    public Toggle questButtonPrefab;
    public Data_Quest[] questDataTest;
    [Header("Quest List")]
    public List<Data_Quest> questList = new List<Data_Quest>();
    public List<Data_Quest> complateList = new List<Data_Quest>();

    private void Awake()
    {
        // 초기화
        questDictionary.Clear();
        for (int i = 0; i < questDataTest.Length; i++)
        {
            AddQuest(questDataTest[i]);
            GetToggle().onValueChanged.AddListener(delegate { SetQuestDisplay(questDataTest[i]); });
        }
    }
    Queue<Toggle> toggleQueue = new Queue<Toggle>();
    Toggle GetToggle()
    {
        if (toggleQueue.Count > 0)
        {
            // 큐에서 Toggle 가져오기
            Toggle queToggle = toggleQueue.Dequeue();
            queToggle.gameObject.SetActive(true);
            return queToggle;
        }
        // Toggle 생성
        Toggle toggle = Instantiate(questButtonPrefab, questToggleGroup.transform);
        toggle.group = questToggleGroup;
        toggle.gameObject.SetActive(true);
        return toggle;
    }

    public void SetStart()
    {

    }

    public void AddQuest(Data_Quest _questData)
    {
        questList.Add(_questData);
        AddDictionary(_questData);
    }

    public void RemoveQuest(Data_Quest _questData)
    {
        if (questDictionary.ContainsKey(_questData.npc_ID) == true)
        {
            questDictionary[_questData.npc_ID].Remove(_questData);
        }
        else
        {
            Debug.LogWarning($"Quest with title {_questData.title} does not exist in the dictionary.");
        }
    }

    public void ComplateQuest(Data_Quest _questData)
    {
        if (questDictionary.ContainsKey(_questData.npc_ID) == true)
        {
            complateList.Add(_questData);
            questList.Remove(_questData);
            questDictionary[_questData.npc_ID].Remove(_questData);
            // 퀘스트 완료 처리
            Debug.LogWarning($"Quest '{_questData.title}' completed.");
        }
        else
        {
            Debug.LogWarning($"Quest with title {_questData.title} does not exist in the dictionary.");
        }
    }

    public void GetNewspaper(Data_Quest[] _questDatas)
    {
        for (int i = 0; i < _questDatas.Length; i++)
        {
            AddDictionary(_questDatas[i]);
        }
    }

    void AddDictionary(Data_Quest _questData)
    {
        string questKey = _questData.npc_ID;
        if (questDictionary.ContainsKey(questKey) == false)
        {
            // 퀘스트가 없으면 새로 추가
            List<Data_Quest> questList = new List<Data_Quest> { _questData };
            questDictionary[questKey] = questList;
        }
        else
        {
            // 퀘스트가 이미 있으면 리스트에 추가
            questDictionary[questKey].Add(_questData);
            Debug.LogWarning($"Quest with title {questKey} already exists.");
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

    void SetQuestDisplay(Data_Quest _quest)
    {
        // 퀘스트가 있는지 확인
      
    }


    public bool TryComplateQuest()
    {
        // 퀘스트 완료 여부 확인
        return false;
    }
}
