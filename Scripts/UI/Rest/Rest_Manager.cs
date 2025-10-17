using System.Collections;
using UnityEngine;
using static StaticOpenCanvas;

public class Rest_Manager : MonoBehaviour
{
    public CanvasStruct[] canvasStructs;
    public Rest_Drag restDrag;
    public Custom_Button restButton;
    public CanvasGroup canvasGroup;
    public int Hour => restDrag.hour;
    public int Minute => restDrag.minute;

    public void SetStart()
    {
        restButton.SetButton(RestButton);
    }

    public void OpenCanvas(bool _open)
    {
        restDrag.SetStart();
        canvasGroup.gameObject.SetActive(_open);
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
    }

    void RestButton()
    {
        StartCoroutine(SetRest());
    }

    IEnumerator SetRest()
    {
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
        yield return new WaitForSeconds(1f);

        normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime;
            CanvasAlpha(1f - normalize);
            yield return null;
        }
        canvasGroup.gameObject.SetActive(false);
        Singleton_Continue.INSTANCE.SaveContinue();// ÈÞ½Ä ÈÄ ÀúÀå
        Game_Manager.current.GetLanding.SetLandingCanvas(true);
    }

    void CanvasAlpha(float _alpha)
    {
        canvasGroup.alpha = _alpha;
        canvasGroup.blocksRaycasts = _alpha > 0f;
        canvasGroup.interactable = _alpha > 0f;
    }
}
