using System.Collections;
using TMPro;
using UnityEngine;
using static Data_Manager;

public class Skill_Infomation : MonoBehaviour
{
    public RectTransform rect;
    public TMP_Text text_Neme;
    public TMP_Text text_Price;

    private void Start()
    {
        SetPosition(null);
    }

    public void SetPosition(StatusStruct _status, Vector2 _position = default)
    {
        //rect.gameObject.SetActive(_status != null);

        if (moving != null)
            StopCoroutine(moving);
        moving = StartCoroutine(SetMoving(_position));

        if (_status == null)
            return;

        text_Neme.text = _status.name;
        text_Price.text = _status.price.ToString();

    }
    Coroutine moving;
    public CanvasGroup canvasGroup;
    IEnumerator SetMoving(Vector2 _position)
    {
        Vector2 viewportPoint = Camera.main.ScreenToViewportPoint(_position);
        float viewX = viewportPoint.x;
        float viewY = Mathf.Round(viewportPoint.y);
        viewportPoint = new Vector2(viewX, viewY);

        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 5f;
            yield return null;
            if (_position != default)
            {
                rect.pivot = Vector2.Lerp(rect.pivot, viewportPoint, normalize);
                rect.position = Vector3.Lerp(rect.position, _position, normalize);
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1f, normalize);
            }
            else
            {
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0f, normalize);
            }
        }
    }
}
