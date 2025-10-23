using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_NewsManager : MonoBehaviour
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    public int newspaperDay;// 신문 구입 날짜 - 날짜형 퀘스트를 위해

    public Data_Quest[] questDatas;
    public UI_NewsSlot[] questSlots;
    public CanvasGroup questCanvas, background;
    public Custom_Button closeButton;
    public RectTransform closeRect;
    public UI_NewsInfomation questInfomation;
    Coroutine openCanvas;

    public void SetStart()
    {
        questCanvas.gameObject.SetActive(false);
        background.gameObject.SetActive(false);

        closeButton.SetButton(CloseButton);
        QuestInfomation(null);// 정보창 닫기
    }

    void QuestInfomation(Data_Quest _questDatas)
    {
        questInfomation.gameObject.SetActive(_questDatas != null);
        if (_questDatas != null)
        {
            questInfomation.SetDisplay(_questDatas);
        }
    }
    Dictionary<string, Data_Quest> questList = new Dictionary<string, Data_Quest>();
    void QuestClick(Data_Quest _questDatas)
    {
        if (questList.ContainsKey(_questDatas.id) == false)
            questList[_questDatas.id] = _questDatas;
        Debug.LogWarning(_questDatas.title);
    }

    void DisplayNews()
    {
        for (int i = 0; i < questDatas.Length; i++)
        {
            UI_NewsSlot slot = questSlots[i];
            slot.SetQuest(questDatas[i]);
            slot.deleClick = QuestClick;
            slot.deleMouseOver = QuestInfomation;
        }
    }

    public void OpenNewsPaper()
    {
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

        questCanvas.alpha = 1f;
        questCanvas.interactable = true;
        questCanvas.blocksRaycasts = true;

        Vector2 prevClose = new Vector2(0, -closeRect.sizeDelta.y);
        Vector3 prevPoint = Input.mousePosition;
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 10f;
            float size = Mathf.Lerp(0f, 1f, normalize);
            background.alpha = size;
            Vector3 actionPoint = Vector3.Lerp(prevPoint, background.transform.position, normalize);
            float rotate = Mathf.Lerp(45f, 0f, normalize);
            questCanvas.transform.SetPositionAndRotation(actionPoint, Quaternion.Euler(0f, 0f, rotate));
            questCanvas.transform.localScale = Vector3.one * size;
            closeRect.anchoredPosition = Vector2.Lerp(prevClose, Vector2.zero, normalize);
            yield return null;
        }
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
}
