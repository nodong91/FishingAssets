using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_NewsManager : MonoBehaviour
{
    public List<Data_Quest> myQuestList = new List<Data_Quest>();
    public int newspaperDay;// 신문 구입 날짜 - 날짜형 퀘스트를 위해

    public Data_Quest[] questData;

    public UI_NewsSlot questSlot;
    public UI_NewsSlot[] questSlots;
    public GridLayoutGroup gridParent;
    public CanvasGroup questCanvas;
    public Button closeButton;
    public GameObject target;
    Coroutine openCanvas;
    public UI_NewsDisplay questDisplay;
    Queue<UI_NewsSlot> queueSlot = new Queue<UI_NewsSlot>();

    public void SetStart()
    {
        questCanvas.gameObject.SetActive(false);
        closeButton.onClick.AddListener(CloseButton);

        DisplayQuest(null);// 정보창 닫기
    }

    UI_NewsSlot TrySlot()
    {
        if (queueSlot.Count > 0)
            return queueSlot.Dequeue();
        UI_NewsSlot inst = Instantiate(questSlot, gridParent.transform);
        return inst;
    }

    void DisplayQuest(Data_Quest _questData)
    {
        questDisplay.gameObject.SetActive(_questData != null);
        if (_questData != null)
        {
            questDisplay.SetDisplay(_questData);
        }
    }

    public void SetQuest(Data_Quest[] _questData)
    {
        questData = _questData;
        Debug.LogWarning(_questData.Length);
        // 비우기
        for (int i = 0; i < questSlots.Length; i++)
        {
            queueSlot.Enqueue(questSlots[i]);
            questSlots[i].gameObject.SetActive(false);
        }

        questSlots = new UI_NewsSlot[_questData.Length];
        for (int i = 0; i < _questData.Length; i++)
        {
            UI_NewsSlot slot = TrySlot();
            slot.gameObject.SetActive(true);
            slot.SetQuest(_questData[i]);
            slot.deleClick = DisplayQuest;
            questSlots[i] = slot;
        }

        if (openCanvas != null)
            StopCoroutine(openCanvas);
        openCanvas = StartCoroutine(OpenCanvas());
    }

    IEnumerator OpenCanvas()
    {
        questCanvas.gameObject.SetActive(true);
        questCanvas.alpha = 1f;
        Vector3 prevPoint = Input.mousePosition;
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 10f;
            Vector3 actionPoint = Vector3.Lerp(prevPoint, target.transform.position, normalize);
            questCanvas.transform.position = actionPoint;
            float rotate = Mathf.Lerp(45f, 0f, normalize);
            questCanvas.transform.rotation = Quaternion.Euler(0f, 0f, rotate);
            float size = Mathf.Lerp(0f, 1f, normalize);
            questCanvas.transform.localScale = Vector3.one * size;
            yield return null;
        }
    }

    void CloseButton()
    {
        if (openCanvas != null)
            StopCoroutine(openCanvas);
        openCanvas = StartCoroutine(CloseCanvas());
    }

    IEnumerator CloseCanvas()
    {
        Vector3 prevPoint = questCanvas.transform.position;
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 10f;
            Vector3 actionPoint = Vector3.Lerp(prevPoint, target.transform.position + Vector3.up * 500f, normalize);
            questCanvas.transform.position = actionPoint;
            float alpha = Mathf.Lerp(0f, 1, normalize);
            questCanvas.alpha = 1f - alpha;
            yield return null;
        }
        questCanvas.gameObject.SetActive(false);
    }

    public void AddQuest(Data_Quest _quest)
    {
        myQuestList.Add(_quest);
    }
}
