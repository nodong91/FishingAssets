using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Rest_Drag : MonoBehaviour, IDragHandler
{
    float time;
    public int hour, minute;
    public TMPro.TMP_Text setHour;
    public TMPro.TMP_Text setMinute;
    public Image dragImage;

    public void OnDrag(PointerEventData eventData)
    {
        time = Mathf.Clamp(time + (int)(eventData.delta.y), 0f, 48f);
        minute = (int)(time % 2f) * 3;
        hour = (int)(time / 2f);
        setHour.text = hour.ToString("00");
        setMinute.text = (minute * 10).ToString("00");
        dragImage.fillAmount = time / 48f;
    }

    public void SetStart()
    {
        time = 0f;
        hour = 0;
        minute = 0;
        setHour.text = "00";
        setMinute.text = "00";
        dragImage.fillAmount = 0f;
    }
}
