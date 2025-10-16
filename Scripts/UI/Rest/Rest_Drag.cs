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
    public Image check;

    public void OnDrag(PointerEventData eventData)
    {
        time = Mathf.Clamp(time + (int)(eventData.delta.y), 0f, 48f);
        minute = (int)(time % 2f) * 30;
        hour = (int)(time / 2f);
        setHour.text = hour.ToString("00");
        setMinute.text = minute.ToString("00");
        dragImage.fillAmount = time / 48f;
    }
}
