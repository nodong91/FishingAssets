using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;

public class UI_Time : MonoBehaviour
{
    public TMPro.TMP_Text hourText, minuteText;
    public TMPro.TMP_Text weekText;
    Light DayLight => Game_Manager.current.dayLight;
    Color DayColor => Game_Manager.current.dayColor;
    Color NightColor => Game_Manager.current.nightColor;
    Color setEmissionColor;
    Color emissionColor => Game_Manager.current.emissionColor;
    public Image dayIcon, nightIcon;
    Coroutine timeUpdate;

    public enum WeatherType
    {
        Sun,
        Rain,
        Cloud
    }
    public WeatherType weatherType;
    public DayType lightMode = DayType.Any;
    public float timeSpeed = 10f;
    public float minute = 0;
    public int hour = 0;
    public int day = 0;
    public bool paused = false;

    public void SetStart(Data_Continue _data)
    {
        timeSpeed = _data.timeSpeed;
        minute = _data.minute;
        hour = _data.hour;
        day = _data.day;
        SetSkyBox();
        TimePause(false);
        SetResetTime();
    }

    void SetSkyBox()
    {
        if (hour >= 5f && hour < 18f)
        {
            // ³·
            lightMode = DayType.Day;
            DayLight.color = DayColor;
            dayIcon.rectTransform.anchoredPosition = new Vector2(dayIcon.rectTransform.anchoredPosition.x, 0f);
            nightIcon.rectTransform.anchoredPosition = new Vector2(nightIcon.rectTransform.anchoredPosition.x, -30f);
            setEmissionColor = Color.black;
            RenderSettings.skybox.SetFloat("_Amount", 0f);
        }
        else
        {
            // ¹ã
            lightMode = DayType.Night;
            DayLight.color = NightColor;
            dayIcon.rectTransform.anchoredPosition = new Vector2(dayIcon.rectTransform.anchoredPosition.x, 30f);
            nightIcon.rectTransform.anchoredPosition = new Vector2(nightIcon.rectTransform.anchoredPosition.x, 0f);
            setEmissionColor = emissionColor;
            RenderSettings.skybox.SetFloat("_Amount", 1f);
        }
        Shader.SetGlobalColor("_EmissionColor", setEmissionColor);
    }

    IEnumerator TimeUpdate()
    {
        while (paused == false)
        {
            string minuteStr = ((int)minute * 10).ToString("D2");
            string hourStr = hour.ToString("D2");
            hourText.text = hourStr;
            minuteText.text = minuteStr;
            WeekPosition(day % 7);
            yield return new WaitForSeconds(timeSpeed);

            minute++;
            if (minute >= 6f)
            {
                minute = 0f;
                hour++;
                if (hour >= 24)
                {
                    hour = 0;
                    day++;
                    SetResetTime();
                }
                CheckLoanTime();
            }
            yield return null;
            DayChage();
        }
    }

    void DayChage()
    {
        if (hour == 18 && lightMode == DayType.Day)
        {
            StartCoroutine(DayChange(DayType.Night));
        }
        else if (hour == 5 && lightMode == DayType.Night)
        {
            StartCoroutine(DayChange(DayType.Day));
        }
    }

    IEnumerator DayChange(DayType _dayType)
    {
        Debug.LogWarning($"¹ã³· : {_dayType}");
        lightMode = _dayType;
        Color prevColor = DayLight.color;
        Color targetColor = lightMode == DayType.Day ? DayColor : NightColor;
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime;
            RenderSettings.skybox.SetFloat("_Amount", lightMode == DayType.Day ? 1f - normalize : normalize);
            // ¶óÀÌÆ® º¯°æ
            DayLight.color = Color.Lerp(prevColor, targetColor, normalize);
            float active = Mathf.Lerp(-30f, 0f, normalize);
            float remove = Mathf.Lerp(0f, 30f, normalize);
            switch (lightMode)
            {
                case DayType.Day:

                    dayIcon.rectTransform.anchoredPosition = new Vector2(dayIcon.rectTransform.anchoredPosition.x, active);
                    nightIcon.rectTransform.anchoredPosition = new Vector2(nightIcon.rectTransform.anchoredPosition.x, remove);
                    break;

                case DayType.Night:
                    nightIcon.rectTransform.anchoredPosition = new Vector2(nightIcon.rectTransform.anchoredPosition.x, active);
                    dayIcon.rectTransform.anchoredPosition = new Vector2(dayIcon.rectTransform.anchoredPosition.x, remove);
                    break;
            }
            yield return null;
            Color targetEmissionColor = lightMode == DayType.Day ? Color.black : emissionColor;
            setEmissionColor = Color.Lerp(setEmissionColor, targetEmissionColor, normalize);
            Shader.SetGlobalColor("_EmissionColor", setEmissionColor);
        }
    }

    void WeekPosition(int _index)
    {
        //float targetX = Mathf.Lerp(-60f, 60f, _index / 6f);
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
    //==========================================================================================================
    // ¿ÜºÎ ÄÁÆ®·Ñ
    //==========================================================================================================
    public void TimePause(bool _pause)
    {
        paused = _pause;
        if (timeUpdate != null)
            StopCoroutine(timeUpdate);
        timeUpdate = StartCoroutine(TimeUpdate());
    }

    public void SetRestTime(int _hour, float _minute)
    {
        hour += _hour;
        minute += _minute;
        if (hour >= 24)
        {
            hour -= 24;
            day++;
        }
        string minuteStr = ((int)minute * 10).ToString("D2");
        string hourStr = hour.ToString("D2");
        hourText.text = hourStr;
        minuteText.text = minuteStr;
        WeekPosition(day % 7);
        SetSkyBox();
    }
    [Header(" [ ´ëÃâ ]")]
    public GameObject loanObject;
    public TMPro.TMP_Text loanText;
    int loanTime = 0;
    public void StartLoanTimer()
    {
        loanObject.SetActive(true);
        loanTime = 24;
    }

    void CheckLoanTime()
    {
        if (loanTime > 0)
        {
            loanTime--;
            loanText.text = $"Loan Due Time: {loanTime}h";
            Debug.LogWarning(loanText.text);
            if (loanTime == 0)
            {
                // °ÔÀÓ ¿À¹ö
                Game_Manager.current.GameOver();
            }
        }
    }
    public bool shopReset = false;
    public bool shipyardReset = false;
    public bool smugglerReset = false;
    void SetResetTime()
    {
        shopReset = true;
        shipyardReset = true;
        smugglerReset = true;
    }
}
