using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UI_NewsManager : MonoBehaviour
{
    public int newspaperDay;// 신문 구입 날짜 - 날짜형 퀘스트를 위해

    public Data_Quest[] questDatas;
    public UI_NewsSlot[] questSlots;
    public CanvasGroup questCanvas, background;
    public Custom_Button closeButton;
    Coroutine openCanvas;
    public UI_NewsDisplay questDisplay;
    Queue<UI_NewsSlot> queueSlot = new Queue<UI_NewsSlot>();

    public void SetStart()
    {
        questCanvas.gameObject.SetActive(false);
        closeButton.SetButton(CloseButton);

        SetQuest();
        DisplayQuest(null);// 정보창 닫기
    }

    void DisplayQuest(Data_Quest _questDatas)
    {
        questDisplay.gameObject.SetActive(_questDatas != null);
        if (_questDatas != null)
        {
            questDisplay.SetDisplay(_questDatas);
        }
    }

    void DisplaynNews()
    {
        for (int i = 0; i < questDatas.Length; i++)
        {
            UI_NewsSlot slot = questSlots[i];
            slot.SetQuest(questDatas[i]);
            slot.deleClick = DisplayQuest;
        }
    }

    public void OpenNewsPaper()
    {
        DisplaynNews();
        if (openCanvas != null)
            StopCoroutine(openCanvas);
        openCanvas = StartCoroutine(OpenCanvas());
    }

    IEnumerator OpenCanvas()
    {
        background.gameObject.SetActive(true);
        questCanvas.gameObject.SetActive(true);
        questCanvas.alpha = 1f;
        Vector3 prevPoint = Input.mousePosition;
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 10f;
            background.alpha = Mathf.Lerp(0f, 1f, normalize);
            Vector3 actionPoint = Vector3.Lerp(prevPoint, background.transform.position, normalize);
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
            Vector3 actionPoint = Vector3.Lerp(prevPoint, background.transform.position + Vector3.up * 500f, normalize);
            questCanvas.transform.position = actionPoint;
            float alpha = Mathf.Lerp(1f, 0f, normalize);
            questCanvas.alpha = alpha;
            background.alpha = alpha;
            yield return null;
        }
        questCanvas.gameObject.SetActive(false);
        background.gameObject.SetActive(false);
    }

    //public void AddQuest(Data_Quest _quest)
    //{
    //    myQuestList.Add(_quest);
    //}







    void SetQuest()
    {
        Debug.LogError("신문 퀘스트 세팅");
        Game_Manager.current.GetQuestUI.GetNewspaper(questDatas);
        //Option_Manager.current.GetQuestManager.SetStart();
        //Option_Manager.current.GetQuestManager.OpenQuestCanvas();
    }
}
