using System.Collections;
using TMPro;
using UnityEngine;
using static Data_Manager;

public class Skill_Infomation : MonoBehaviour
{
    public RectTransform rect;
    public TMP_Text text_Neme;
    public TMP_Text text_Description;
    public TMP_Text text_Price;
    public CanvasGroup canvasGroup;
    Coroutine moving;

    private void Start()
    {
        SetPosition(null);
    }

    public void SetPosition(StatusStruct _status, Vector2 _position = default)
    {
        float alpha = (_status == null) ? 0f : 1f;
        Vector2 viewportPoint;
        if (alpha == 0f)
        {
            viewportPoint = Vector2.one * 0.5f;
            _position = Camera.main.ViewportToScreenPoint(viewportPoint);
        }
        else
        {
            viewportPoint = Camera.main.ScreenToViewportPoint(_position);
            float viewX = viewportPoint.x;
            float viewY = Mathf.Round(viewportPoint.y);
            viewportPoint = new Vector2(viewX, viewY);
        }

        if (moving != null)
            StopCoroutine(moving);
        moving = StartCoroutine(SetMoving(_position, viewportPoint, alpha));

        if (_status == null)
            return;

        text_Neme.text = _status.name;
        text_Description.text = _status.description;
        text_Price.text = _status.price.ToString();

    }

    IEnumerator SetMoving(Vector2 _position, Vector2 _viewportPoint, float _alpha)
    {
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 5f;
            yield return null;

            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, _alpha, normalize);
            rect.pivot = Vector2.Lerp(rect.pivot, _viewportPoint, normalize);
            rect.position = Vector3.Lerp(rect.position, _position, normalize);
        }
    }
}
