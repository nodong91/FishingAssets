using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tutorial_Manager : MonoBehaviour, IPointerClickHandler
{
    public CanvasGroup canvasGroup;
    [System.Serializable]
    public class TutorialStruct
    {
        public bool completed;
        [System.Serializable]
        public struct TutorialSet
        {
            public Vector2 position;
            public Vector2 size;
        }
        public TutorialSet[] tutorialSet;
    }
    public TutorialStruct tutorialStruct;
    public int currentTutorial;
    public RectTransform boxRect;
    public bool acting;
    Coroutine ingOpenCanvas;
    public AnimationCurve animationCurveX, animationCurveY;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (acting == true)
            return;
        CurrentAction();
    }

    public void TutorialPause()
    {
        Time.timeScale = 0f;
        currentTutorial = 0;
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
            CurrentAction();
    }

    void CanvasGroupAlpha(float _alpha)
    {
        canvasGroup.alpha = _alpha;
        canvasGroup.blocksRaycasts = _alpha > 0;
        canvasGroup.interactable = _alpha > 0;
    }

    void CurrentAction()
    {
        if (currentTutorial < tutorialStruct.tutorialSet.Length)
        {
            TutorialAction();
            currentTutorial++;
            return;
        }
        // 완료
        Debug.LogWarning("튜토리얼 완료");
        Time.timeScale = 1f;
        OpenCanvas(0f);
    }

    void TutorialAction()
    {
        if (ingOpenCanvas != null)
            StopCoroutine(ingOpenCanvas);
        ingOpenCanvas = StartCoroutine(ING_TutorialAction(tutorialStruct.tutorialSet[currentTutorial]));
    }

    IEnumerator ING_TutorialAction(TutorialStruct.TutorialSet _tutorialSet)
    {
        acting = true;
        boxRect.anchoredPosition = _tutorialSet.position;
        yield return null;

        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.unscaledDeltaTime * 5f;

            float alpha = Mathf.Lerp(0f, 1f, normalize);
            float curveX = animationCurveX.Evaluate(alpha) * _tutorialSet.size.x;
            float curveY = animationCurveY.Evaluate(alpha) * _tutorialSet.size.y;

            boxRect.sizeDelta = new Vector2(curveX, curveY);
            yield return null;
        }
        acting = false;
    }
}
