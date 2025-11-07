using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Manager : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TMPro.TMP_Text commentText;

    public Data_Tutorial[] tutorialData;
    public Data_Tutorial currentTutorial;

    public string currentID;
    public int currentIndex;
    public RectTransform boxRect;
    public bool acting;
    Coroutine ingOpenCanvas;
    public AnimationCurve animationCurveX, animationCurveY;
    public Custom_Button activeButton;

    Dictionary<string, Data_Tutorial> dictTutorial = new Dictionary<string, Data_Tutorial>();
    //Dictionary<string, bool> dictComp = new Dictionary<string, bool>();

    public void SetStart()
    {
        LoadTutorial();
        dictTutorial = new Dictionary<string, Data_Tutorial>();
        for (int i = 0; i < tutorialData.Length; i++)
        {
            dictTutorial.Add(tutorialData[i].id, tutorialData[i]);
        }
        canvasGroup.gameObject.SetActive(false);
    }

    public void SetTutorial(string _id)// 튜토리얼 세팅
    {
        currentID = _id;
        currentTutorial = dictTutorial[currentID];
        if (completedTutorial.Contains(currentID) == true)
        {
            // 완료 했음
        }
        else if (currentTutorial.npc != null)
        {
            Game_Manager.current.GetDialog.DialogStart_NPC(currentTutorial.npc, currentTutorial.dialogIndex);
        }
    }

    public void StartTutorial()
    {
        // 섬입장
        Game_Manager.current.CurrentLand.SetLandingAction();
        Game_Manager.current.OutOfControll(true);
        // 튜토리얼
        Time.timeScale = currentTutorial.timeScale;
        currentIndex = 0;
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

    void SetComment(Data_Tutorial.TutorialComment _tutorialComment)
    {
        commentText.gameObject.SetActive(true);
        commentText.text = _tutorialComment.comment;
        commentText.fontSize = _tutorialComment.commentSize;
        commentText.rectTransform.anchoredPosition = _tutorialComment.commentPosition;
    }

    void CanvasGroupAlpha(float _alpha)
    {
        canvasGroup.alpha = _alpha;
        canvasGroup.blocksRaycasts = _alpha > 0;
        canvasGroup.interactable = _alpha > 0;
    }

    void CurrentAction()// 현재 튜토리얼 세팅
    {
        if (currentIndex < currentTutorial.tutorialComment.Length)
        {
            TutorialAction();
            return;
        }
        // 완료
        Debug.LogWarning("튜토리얼 완료");
        Singleton_Continue.INSTANCE.SaveContinue();// 튜토리얼 종료
        Time.timeScale = 1f;
        OpenCanvas(0f);
        SaveTutorial();
    }

    void TutorialAction()
    {
        if (ingOpenCanvas != null)
            StopCoroutine(ingOpenCanvas);
        ingOpenCanvas = StartCoroutine(ING_TutorialAction(currentTutorial.tutorialComment[currentIndex]));
    }

    IEnumerator ING_TutorialAction(Data_Tutorial.TutorialComment _tutorialComment)
    {
        int index = currentIndex;
        boxRect.sizeDelta = new Vector2(0f, 0f);
        commentText.gameObject.SetActive(false);
        yield return new WaitForSeconds(_tutorialComment.intervalTime);

        SetComment(currentTutorial.tutorialComment[index]);
        OnClickSetting(index);
        currentIndex++;

        acting = true;
        boxRect.anchoredPosition = _tutorialComment.boxPosition;
        yield return null;

        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.unscaledDeltaTime * 5f;

            float alpha = Mathf.Lerp(0f, 1f, normalize);
            float curveX = animationCurveX.Evaluate(alpha) * _tutorialComment.boxSize.x;
            float curveY = animationCurveY.Evaluate(alpha) * _tutorialComment.boxSize.y;

            boxRect.sizeDelta = new Vector2(curveX, curveY);
            yield return null;
        }
        acting = false;
    }

    //=========================================================================================================
    // 튜토리얼 라인
    //=========================================================================================================

    void OnClickSetting(int _index)// 액션 버튼
    {
        string cord = currentTutorial.id;
        if (acting == true)
            return;
        switch (cord)
        {
            case String_Tutorial._newGame:
                activeButton.SetButton(delegate { OpenShipyard(_index); });// 액션 세팅
                break;

            case String_Tutorial._fillFuel:
                activeButton.SetButton(delegate { FillFuel(_index); });// 액션 세팅
                break;

            default:
                activeButton.SetButton(delegate { ActiveDefault(_index); });
                break;
        }
    }

    void OpenShipyard(int _index)
    {
        switch (_index)
        {
            case 0:
                // 조선소 입장
                Game_Manager.current.GetLanding.ShipyardButton();
                break;

            case 1:
                // 강화 선택
                Game_Manager.current.GetDialog.Tutorial_Upgrade();
                break;

            case 2:
                // 스킬 길게 누르기
                Skill_Slot skillSlot = Game_Manager.current.GetSkill.startSlot;
                skillSlot.ActiveSlot();
                break;

            case 3:
                // 강화창 닫기
                Game_Manager.current.GetSkill.CloseCanvas();
                break;

            case 4:
                // 배 변경 열기
                Game_Manager.current.GetLanding.ChangeButton();
                break;

            case 5:
                // 튜토리얼용 배 선택
                Game_Manager.current.GetChangeShip.SelectTutorialShip();
                break;

            case 6:
                // 선박 변경 나가기
                Game_Manager.current.GetChangeShip.CloseCanvas();
                break;
        }
        CurrentAction();
    }

    void FillFuel(int _index)
    {
        switch (_index)
        {
            case 0:
                // 연료 선택
                Game_Manager.current.GetLanding.FuelButton();
                break;

            case 1:
                // 연료 채우기 - 슬라이더
                break;

            case 2:
                // 나가기
                Game_Manager.current.GetLanding.BackButton();
                break;
        }
        CurrentAction();
    }

    void ActiveDefault(int _index)
    {
        CurrentAction();
        Debug.LogWarning($"튜토리얼 : {currentIndex}");
    }

    //=========================================================================================================
    // 튜토리얼 저장
    //=========================================================================================================

    const string tutorialKey = "CompletedTutorial";
    public List<string> completedTutorial;
    void SaveTutorial()
    {
        if (completedTutorial == null)
            completedTutorial = new List<string>();
        completedTutorial.Add(currentID);
        Static_JsonManager.SaveTutorialData(tutorialKey, completedTutorial);
    }

    void LoadTutorial()
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
