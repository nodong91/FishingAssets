using System.Collections;
using UnityEngine;

public class UI_Time : MonoBehaviour
{
    public TMPro.TMP_Text hourText, minuteText, weekText;

    public float timeSpeed = 10f;
    public float minute = 0;
    public int hour = 0;
    public int day = 0;

    public enum WEEK
    {
        Monday = 0,
        Tuesday = 1,
        Wednesday = 2,
        Thursday = 3,
        Friday = 4,
        Saturday = 5,
        Sunday = 6
    }

    public void SetStart(float _timeSpeed, float _minute, int _hour, int _day)
    {
        timeSpeed = _timeSpeed;
        minute = _minute;
        hour = _hour;
        day = _day;
    }

    private void Update()
    {
        minute += Time.deltaTime * timeSpeed;
        if (minute >= 60f)
        {
            minute = 0f;
            hour++;
            if (hour >= 24)
            {
                hour = 0;
                day++;
            }
        }

        string minuteStr = ((int)minute).ToString("D2");
        string hourStr = hour.ToString("D2");
        hourText.text = hourStr;
        minuteText.text = minuteStr;
        weekText.text = ((WEEK)(day % 7)).ToString();
    }
}
