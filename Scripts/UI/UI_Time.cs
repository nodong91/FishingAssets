using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_Time : MonoBehaviour
{
    public TMPro.TMP_Text hourText, minuteText;
    public TMPro.TMP_Text weekText;
    Light DayLight => Game_Manager.current.dayLight;
    Color DayColor => Game_Manager.current.dayColor;
    Color NightColor => Game_Manager.current.nightColor;
    public Image dayIcon, nightIcon;
    Coroutine timeUpdate;
    public Material skyboxMatial;

    public enum WeatherType
    {
        Sun,
        Rain,
        Cloud
    }
    public WeatherType weatherType;
    public Data_Manager.DayType lightMode = Data_Manager.DayType.Any;
    public float timeSpeed = 10f;
    public float minute = 0;
    public int hour = 0;
    public int day = 0;
    bool paused = false;

    public void SetStart(float _timeSpeed, float _minute, int _hour, int _day)
    {
        skyboxMatial = RenderSettings.skybox;
        timeSpeed = _timeSpeed;
        minute = _minute;
        hour = _hour;
        day = _day;
        if (hour >= 5f && hour < 18f)
        {
            // ³·
            lightMode = Data_Manager.DayType.Day;
            DayLight.color = DayColor;
            dayIcon.rectTransform.anchoredPosition = new Vector2(dayIcon.rectTransform.anchoredPosition.x, 0f);
            nightIcon.rectTransform.anchoredPosition = new Vector2(nightIcon.rectTransform.anchoredPosition.x, -30f);
        }
        else
        {
            // ¹ã
            lightMode = Data_Manager.DayType.Night;
            DayLight.color = NightColor;
            dayIcon.rectTransform.anchoredPosition = new Vector2(dayIcon.rectTransform.anchoredPosition.x, 30f);
            nightIcon.rectTransform.anchoredPosition = new Vector2(nightIcon.rectTransform.anchoredPosition.x, 0f);
        }
        TimePause(false);
    }

    IEnumerator TimeUpdate()
    {
        while (paused == false)
        {
            yield return new WaitForSeconds(timeSpeed);
            //minute += Time.deltaTime * timeSpeed;
            minute++;
            if (minute >= 6f)
            {
                minute = 0f;
                hour++;
                if (hour >= 24)
                {
                    hour = 0;
                    day++;
                }
            }

            string minuteStr = ((int)minute * 10).ToString("D2");
            string hourStr = hour.ToString("D2");
            hourText.text = hourStr;
            minuteText.text = minuteStr;
            WeekPosition(day % 7);
            yield return null;

            if (hour == 18 && lightMode == Data_Manager.DayType.Day)
            {
                StartCoroutine(DayChange(Data_Manager.DayType.Night));
            }
            else if (hour == 5 && lightMode == Data_Manager.DayType.Night)
            {
                StartCoroutine(DayChange(Data_Manager.DayType.Day));
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            paused = !paused;
            TimePause(paused);
        }
    }

    IEnumerator DayChange(Data_Manager.DayType _dayType)
    {
        Debug.LogWarning($"j  {_dayType}");
        lightMode = _dayType;
        Color prevColor = DayLight.color;
        Color targetColor = lightMode == Data_Manager.DayType.Day ? DayColor : NightColor;
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime;
            skyboxMatial.SetFloat("_Amount", lightMode == Data_Manager.DayType.Day ? 1f - normalize : normalize);
            // ¶óÀÌÆ® º¯°æ
            DayLight.color = Color.Lerp(prevColor, targetColor, normalize);
            float active = Mathf.Lerp(-30f, 0f, normalize);
            float remove = Mathf.Lerp(0f, 30f, normalize);
            switch (lightMode)
            {
                case Data_Manager.DayType.Day:

                    dayIcon.rectTransform.anchoredPosition = new Vector2(dayIcon.rectTransform.anchoredPosition.x, active);
                    nightIcon.rectTransform.anchoredPosition = new Vector2(nightIcon.rectTransform.anchoredPosition.x, remove);
                    break;

                case Data_Manager.DayType.Night:
                    nightIcon.rectTransform.anchoredPosition = new Vector2(nightIcon.rectTransform.anchoredPosition.x, active);
                    dayIcon.rectTransform.anchoredPosition = new Vector2(dayIcon.rectTransform.anchoredPosition.x, remove);
                    break;
            }
            yield return null;
        }
    }

    void WeekPosition(int _index)
    {
        float targetX = Mathf.Lerp(-60f, 60f, _index / 6f);
        switch (_index)
        {
            case 0:
                weekText.text = "Sun";
                break;
            case 1:
                weekText.text = "Mon";
                break;
            case 2:
                weekText.text = "Tue";
                break;
            case 3:
                weekText.text = "Wed";
                break;
            case 4:
                weekText.text = "Thu";
                break;
            case 5:
                weekText.text = "Fri";
                break;
            case 6:
                weekText.text = "Sat";
                break;
            default:
                break;
        }
    }

    public void TimePause(bool _pause)
    {
        paused = _pause;

        if (timeUpdate != null)
            StopCoroutine(timeUpdate);
        timeUpdate = StartCoroutine(TimeUpdate());
    }
}
