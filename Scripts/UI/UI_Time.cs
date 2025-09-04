using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class UI_Time : MonoBehaviour
{
    public TMPro.TMP_Text hourText, minuteText, weekText;
    Light DayLight => Game_Manager.current.dayLight;
    Color DayColor => Game_Manager.current.dayColor;
    Color NightColor => Game_Manager.current.nightColor;

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
        if (hour >= 5f && hour < 18f)
        {
            // ³·
            lightMode = Data_Manager.DayType.Day;
            DayLight.color = DayColor;
        }
        else
        {
            // ¹ã
            lightMode = Data_Manager.DayType.Night;
            DayLight.color = NightColor;
        }
    }

    private void Update()
    {
        if (paused == true)
            return;

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



        // ¶óÀÌÆ® º¯°æ
        if (hour >= 5f && hour < 6f)
        {
            // ³·
            lightMode = Data_Manager.DayType.Day;
            float normalize = minute * 0.02f;
            DayLight.color = Color.Lerp(NightColor, DayColor, normalize);
        }
        else if (hour >= 18f && hour < 19f)
        {
            // ¹ã
            lightMode = Data_Manager.DayType.Night;
            float normalize = minute * 0.02f;
            DayLight.color = Color.Lerp(DayColor, NightColor, normalize);
        }
    }
    bool paused = false;
    public void TimePause(bool _pause)
    {
        paused = _pause;
    }
}
