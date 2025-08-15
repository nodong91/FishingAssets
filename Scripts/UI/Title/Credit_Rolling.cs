using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Credit_Rolling : MonoBehaviour, IPointerClickHandler
{
    public CanvasGroup canvasGroup;
    public RectTransform rolling;
    public float startY = 0f; // Initial position on the Y-axis
    public float endY = 1000f; // Final position on the Y-axis
    public float currentY = 0f; // Distance to roll
    public float speed = 0.01f;
    Coroutine rollingCoroutine;

    public void OpenCanvas(bool _open)
    {
        if (rollingCoroutine != null)
            StopCoroutine(rollingCoroutine);

        if (_open == true)
        {
            canvasGroup.alpha = 0f; // Reset alpha to 0
            rolling.anchoredPosition = new Vector3(0, 0, 0);
            rollingCoroutine = StartCoroutine(OpeningCanvas(1f)); // Fade in the canvas
        }
        else
        {
            rollingCoroutine = StartCoroutine(OpeningCanvas(0f)); // Fade in the canvas
        }
    }

    IEnumerator OpeningCanvas(float _targetAlpha)
    {
        float normalizedTime = 0f;
        while (normalizedTime < 1f)
        {
            normalizedTime += Time.deltaTime * 3f;
            canvasGroup.alpha = Mathf.Lerp(1f - _targetAlpha, _targetAlpha, normalizedTime); // Fade in the canvas
            canvasGroup.interactable = (canvasGroup.alpha > 0);
            canvasGroup.blocksRaycasts = (canvasGroup.alpha > 0);
            yield return null; // Wait for the next frame
        }

        if (_targetAlpha > 0)
            rollingCoroutine = StartCoroutine(StartRolling()); // Start the rolling coroutine
    }

    IEnumerator StartRolling()
    {
        float normalizedTime = 0f;
        while (normalizedTime < 1f)
        {
            normalizedTime += Time.deltaTime * speed;
            currentY = Mathf.Lerp(startY, endY, normalizedTime); // Lerp between start and end positions
            rolling.anchoredPosition = new Vector3(0, currentY, 0);
            yield return null; // Wait for the next frame
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OpenCanvas(false);
    }
}
