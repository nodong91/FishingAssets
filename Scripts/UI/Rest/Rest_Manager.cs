using System.Collections;
using UnityEngine;

public class Rest_Manager : MonoBehaviour
{
    public Custom_Button restButton;
    public CanvasGroup canvasGroup;
    public Rest_Drag restDrag;
    public float Hour => restDrag.hour;
    public float Minute => restDrag.minute;

    void Start()
    {
        restButton.SetButton(Rest);
    }

    void Rest()
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
        restDrag.check.color = Color.red;
        restDrag.gameObject.SetActive(false);
        Debug.LogWarning($"Rest Complete : {Hour} : {Minute}");
        yield return new WaitForSeconds(1f);

        normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime;
            CanvasAlpha(1f - normalize);
            yield return null;
        }
    }

    void CanvasAlpha(float _alpha)
    {
        canvasGroup.alpha = _alpha;
        canvasGroup.blocksRaycasts = _alpha > 0f;
        canvasGroup.interactable = _alpha > 0f;
    }
}
