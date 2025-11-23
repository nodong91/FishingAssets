using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Data_Manager;

public class UI_NewsManager : MonoBehaviour
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    public int newspaperDay;// 신문 구입 날짜 - 날짜형 퀘스트를 위해

    public string[] questDatas;
    public UI_NewsSlot[] questSlots;
    public CanvasGroup questCanvas, background;
    public Custom_Button closeButton;
    public RectTransform closeRect;
    public UI_NewsInfomation questInfomation;
    Coroutine openCanvas;

    Queue<string> questQueue = new Queue<string>();
    void TestSetting()
    {
        if (questQueue.Count > 0)
            return;
        // 큐 세팅
        questQueue = SetQueue();
        questDatas = new string[3];
        for (int i = 0; i < 3; i++)
        {
            questDatas[i] = questQueue.Dequeue();
        }
    }

    Queue<string> SetQueue()
    {
        List<string> temp = new List<string>();
        foreach (var child in Singleton_Data.INSTANCE.Dict_Quest)
        {
            //// 이미 받은 퀘스트는 나오지 않게
            //if (Game_Manager.current.GetQuest.GetDictQuest.ContainsKey(child.Key) == false)
            temp.Add(child.Key);
        }
        Queue<string> queue = P01_Utility.ShuffleQueue(temp, 0);// 리스트 섞기
        Debug.LogWarning($"큐 세팅 : {temp.Count}, {queue.Count}");
        return queue;
    }

    public void SetStart()
    {
        TestSetting();

        questCanvas.gameObject.SetActive(false);
        background.gameObject.SetActive(false);

        closeButton.SetButton(CloseButton);
        QuestInfomation(null);// 정보창 닫기
    }

    void QuestInfomation(QuestStruct _questDatas)
    {
        questInfomation.gameObject.SetActive(_questDatas != null);
        if (_questDatas != null)
        {
            questInfomation.SetDisplay(_questDatas);
        }
    }

    void DisplayNews()
    {
        for (int i = 0; i < questDatas.Length; i++)
        {
            UI_NewsSlot slot = questSlots[i];
            slot.SetQuest(Singleton_Data.INSTANCE.Dict_Quest[questDatas[i]]);
            slot.deleClick = Game_Manager.current.GetQuest.AddQuestSlot;
            slot.deleMouseOver = QuestInfomation;
            bool setImage = Game_Manager.current.GetQuest.GetDictQuest.ContainsKey(questDatas[i]);
            slot.SetFootImage(setImage);
        }
    }

    public void OpenNewsPaper()
    {
        // 신문 세팅
        DisplayNews();
        if (openCanvas != null)
            StopCoroutine(openCanvas);
        openCanvas = StartCoroutine(OpenCanvas());
    }

    IEnumerator OpenCanvas()
    {
        background.gameObject.SetActive(true);
        questCanvas.gameObject.SetActive(true);
        closeRect.gameObject.SetActive(true);

        questCanvas.interactable = false;
        questCanvas.blocksRaycasts = false;

        Vector2 prevClose = new Vector2(0, -closeRect.sizeDelta.y);
        Vector3 prevPoint = Input.mousePosition;
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 10f;
            float size = Mathf.Lerp(0f, 1f, normalize);
            questCanvas.alpha = size;
            background.alpha = size;
            Vector3 actionPoint = Vector3.Lerp(prevPoint, background.transform.position, normalize);
            float rotate = Mathf.Lerp(45f, 0f, normalize);
            questCanvas.transform.SetPositionAndRotation(actionPoint, Quaternion.Euler(0f, 0f, rotate));
            questCanvas.transform.localScale = Vector3.one * size;
            closeRect.anchoredPosition = Vector2.Lerp(prevClose, Vector2.zero, normalize);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        questCanvas.interactable = true;
        questCanvas.blocksRaycasts = true;
    }

    void CloseButton()
    {
        StaticOpenCanvas.deleEndOpen += EndClose;
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, false));
    }

    void EndClose()
    {
        StaticOpenCanvas.deleEndOpen -= EndClose;
        Game_Manager.current.GetLanding.BackButton();
    }











    private void Start()
    {
        SetSlots();
    }

    public UI_NewsSlot baseSlot;
    Queue<UI_NewsSlot> slotPool = new Queue<UI_NewsSlot>();
    List<UI_NewsSlot> slotList = new List<UI_NewsSlot>();
    public Transform slotParent;

    void SetSlots()
    {
        // 큐 세팅
        questQueue = SetQueue();

        int amount = 3;
        for (int i = 0; i < amount; i++)
        {
            UI_NewsSlot inst = TryNewsSlot();
            slotList.Add(inst);
            inst.SetQuest(Singleton_Data.INSTANCE.Dict_Quest[questDatas[i]]);
        }
    }

    UI_NewsSlot TryNewsSlot()
    {
        if (slotPool.Count > 0)
        {
            var slot = slotPool.Dequeue();
            return slot;
        }
        UI_NewsSlot inst = Instantiate(baseSlot, slotParent);
        inst.deleClick = SlotClick;
        return inst;
    }

    void SlotClick(QuestStruct _questData)
    {
        QuestInfomation(_questData);
    }
}
