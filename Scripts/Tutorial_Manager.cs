using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Tutorial_Manager.TutorialStruct;

public class Tutorial_Manager : MonoBehaviour, IPointerClickHandler
{
    public CanvasGroup canvasGroup;
    public TMPro.TMP_Text commentText;

    [System.Serializable]
    public class TutorialStruct
    {
        public string id;
        public bool completed;
        [System.Serializable]
        public struct TutorialSet
        {
            public string comment;
            public int commentSize;
            public Vector2 commentPosition;
            public Vector2 boxPosition;
            public Vector2 boxSize;
        }
        public TutorialSet[] tutorialSet;
    }
    public TutorialStruct[] testTutorialStruct;
    public TutorialStruct currentTutorial;
    public string currentID;
    public int currentIndex, currentSetIndex;
    public RectTransform boxRect;
    public bool acting;
    Coroutine ingOpenCanvas;
    public AnimationCurve animationCurveX, animationCurveY;

    Dictionary<string, TutorialStruct> dictTutorial = new Dictionary<string, TutorialStruct>();

    public void SetStart()
    {
        LoadTutorial();

        dictTutorial = new Dictionary<string, TutorialStruct>();
        for (int i = 0; i < testTutorialStruct.Length; i++)
        {
            if(completedTutorial.Contains(testTutorialStruct[i].id) == true)
            {
                testTutorialStruct[i].completed = true;
            }
            dictTutorial.Add(testTutorialStruct[i].id, testTutorialStruct[i]);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (acting == true)
            return;
        CurrentAction();
    }

    public void StartTutorial(string _id)
    {
        Debug.LogWarning($"TutorialPause : {_id}");
        currentID = _id;
        currentTutorial = dictTutorial[currentID];
        //currentIndex = _index;
        //currentTutorial = testTutorialStruct[_index];
        //Debug.LogWarning($"TutorialPause Index : {_index}, completed : {currentTutorial.completed}");
        if (currentTutorial.completed == true)
        {
            canvasGroup.gameObject.SetActive(false);
            return;
        }

        // 튜토리얼
        Time.timeScale = 0f;
        currentSetIndex = 0;
        OpenCanvas(1f);
    }

    void OpenCanvas(float _targetAlpha)
    {
        if (ingOpenCanvas != null)
            StopCoroutine(ingOpenCanvas);
        ingOpenCanvas = StartCoroutine(ING_OpenCanvas(_targetAlpha));
    }

    IEnumerator ING_OpenCanvas(float _targetAlpha)
    {
        if (_targetAlpha > 0)
            canvasGroup.gameObject.SetActive(true);
        acting = true;
        boxRect.sizeDelta = Vector2.zero;

        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.unscaledDeltaTime * 3f;
            float alpha = Mathf.Lerp(1f - _targetAlpha, _targetAlpha, normalize);
            CanvasGroupAlpha(alpha);
            yield return null;
        }
        acting = false;
        if (_targetAlpha > 0)// 열림일때
            CurrentAction();// 액션 시작
        else
            canvasGroup.gameObject.SetActive(false);
    }

    void SetComment(TutorialSet _tutorialSet)
    {
        commentText.text = _tutorialSet.comment;
        commentText.fontSize = _tutorialSet.commentSize;
        commentText.rectTransform.anchoredPosition = _tutorialSet.commentPosition;
    }

    void CanvasGroupAlpha(float _alpha)
    {
        canvasGroup.alpha = _alpha;
        canvasGroup.blocksRaycasts = _alpha > 0;
        canvasGroup.interactable = _alpha > 0;
    }

    void CurrentAction()
    {
        if (currentSetIndex < currentTutorial.tutorialSet.Length)
        {
            TutorialAction();
            currentSetIndex++;
            return;
        }
        // 완료
        Debug.LogWarning("튜토리얼 완료");
        Time.timeScale = 1f;
        OpenCanvas(0f);
        SaveTutorial();
    }

    void TutorialAction()
    {
        if (ingOpenCanvas != null)
            StopCoroutine(ingOpenCanvas);
        ingOpenCanvas = StartCoroutine(ING_TutorialAction(currentTutorial.tutorialSet[currentSetIndex]));
    }

    IEnumerator ING_TutorialAction(TutorialSet _tutorialSet)
    {
        SetComment(currentTutorial.tutorialSet[currentSetIndex]);

        acting = true;
        boxRect.anchoredPosition = _tutorialSet.boxPosition;
        yield return null;

        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.unscaledDeltaTime * 5f;

            float alpha = Mathf.Lerp(0f, 1f, normalize);
            float curveX = animationCurveX.Evaluate(alpha) * _tutorialSet.boxSize.x;
            float curveY = animationCurveY.Evaluate(alpha) * _tutorialSet.boxSize.y;

            boxRect.sizeDelta = new Vector2(curveX, curveY);
            yield return null;
        }
        acting = false;
    }

    const string tutorialKey = "CompletedTutorial";
    public List<string> completedTutorial;
    public void SaveTutorial()
    {
        currentTutorial.completed = true;
        if (completedTutorial == null)
            completedTutorial = new List<string>();
        completedTutorial.Add(currentID);
        Static_JsonManager.SaveTutorialData(tutorialKey, completedTutorial);
    }

    public void LoadTutorial()
    {
        if (Static_JsonManager.TryLoadTutorialData(tutorialKey, out List<string> _completedTutorial))
        {
            completedTutorial = _completedTutorial;
        }
        else
        {
            completedTutorial = new List<string>();
        }
    }
}
