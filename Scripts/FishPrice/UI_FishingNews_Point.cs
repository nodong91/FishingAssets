using UnityEngine;
using UnityEngine.UI;

public class UI_FishingNews_Point : MonoBehaviour
{
    public Image point;
    public Image lineRect;
    public RectTransform next;
    public TMPro.TMP_Text valueText;

    public void SetStart(UI_FishingNews_Point _next, Color _color)
    {
        point.color = _color;
        lineRect.color = _color;
        if (_next != null)
        {
            next = _next.point.rectTransform;
        }
        else
        {
            next = null;
        }
        lineRect.gameObject.SetActive(next != null);
    }

    public void SetPoint(float _point)
    {
        float x = 0f;
        float y = (_point - 100f) * 1.5f;
        point.rectTransform.anchoredPosition = new Vector2(x, y);
        valueText.text = $"{_point:0.0}";// 소수점 한자리
    }

    public void UpdateLine()
    {
        if (point != null && next != null)
        {
            UpdateLine(point.transform.position, next.position);
        }
    }

    void UpdateLine(Vector3 pointA, Vector3 pointB)
    {
        //PointA와 PointB는 이전에 결정됨
        Vector3 midpoint = (pointA + pointB) / 2; //선을 배치하는 데 사용

        float angle = Mathf.Atan2(pointB.x - pointA.x, pointA.y - pointB.y);
        if (angle < 0.0) { angle += Mathf.PI * 2; }
        angle *= Mathf.Rad2Deg;
        angle += 90;

        lineRect.rectTransform.position = midpoint; //중간점으로 이동한 다음, 그 주위로 올바른 크기로 확장
        lineRect.rectTransform.sizeDelta = new Vector2(Vector2.Distance(pointA, pointB) * 0.5f, 5); //10은 선의 두께
        lineRect.rectTransform.rotation = Quaternion.Euler(0, 0, angle); //중간점을 중심으로 회전
    }
}
