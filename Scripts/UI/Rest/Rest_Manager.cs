using System.Collections;
using UnityEngine;
using static StaticOpenCanvas;

public class Rest_Manager : MonoBehaviour
{
    public CanvasStruct[] canvasStructs;
    public Rest_Drag restDrag;
    public Custom_Button restButton, backButton;
    public CanvasGroup canvasGroup;
    public int Hour => restDrag.hour;
    public int Minute => restDrag.minute;

    public void SetStart()
    {
        restButton.SetButton(RestButton);
        backButton.SetButton(Game_Manager.current.GetLanding.BackButton);
    }

    public void OpenCanvas(bool _open)
    {
        restDrag.SetStart();
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
    }

    void RestButton()
    {
        if (Hour + Minute > 0)
            StartCoroutine(SetRest());
        else
            Game_Manager.current.GetLanding.BackButton();
    }

    IEnumerator SetRest()
    {
        canvasGroup.gameObject.SetActive(true);

        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime;
            CanvasAlpha(normalize);
            yield return null;
        }
        restDrag.gameObject.SetActive(false);
        Debug.LogWarning($"Rest Complete : {Hour} : {Minute}");
        Game_Manager.current.GetTimeUI.SetTime(Hour, Minute);

        OpenCanvas(false);
        Game_Manager.current.currentLand.CameraOutFouce(false);// 포커스 취소
        yield return new WaitForSeconds(1f);

        normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime;
            CanvasAlpha(1f - normalize);
            yield return null;
        }
        canvasGroup.gameObject.SetActive(false);
        Singleton_Continue.INSTANCE.SaveContinue();// 휴식 후 저장
        Game_Manager.current.GetLanding.SetLandingCanvas(true);// 랜드 UI 열기
    }

    void CanvasAlpha(float _alpha)
    {
        canvasGroup.alpha = _alpha;
        canvasGroup.blocksRaycasts = _alpha > 0f;
        canvasGroup.interactable = _alpha > 0f;
    }
}
