using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;

public class UI_Time : MonoBehaviour
{
    public bool paused = false;

    public enum WeatherType
    {
        Sun,
        Rain,
        Cloud
    }
    public WeatherType weatherType;
    public DayType lightMode = DayType.Any;
    public float timeSpeed = 0.1f;
    public float minute = 0;
    public int hour = 0;
    public int day = 0;

    public TMPro.TMP_Text hourText, minuteText;
    public TMPro.TMP_Text weekText;
    Light DayLight => Game_Manager.current.dayLight;
    Color DayColor => Game_Manager.current.dayColor;
    Color NightColor => Game_Manager.current.nightColor;
    Color setEmissionColor;
    Color emissionColor => Game_Manager.current.emissionColor;
    public Image dayIcon, nightIcon;
    Coroutine timeUpdate;

    public void SetStart(Data_Continue _data)
    {
        timeSpeed = _data.timeSpeed;
        minute = _data.minute;
        hour = _data.hour;
        day = _data.day;

        loanActive = _data.loanActive;
        loanTime = _data.loanTime;

        SetSkyBox();
        TimePause(false);

        StartLoanTimer(false);// 대출 타이머
        SetResetTime();// 게임 시작 시 초기화
    }

    void SetSkyBox()
    {
        if (hour >= 5f && hour < 18f)
        {
            // 낮
            lightMode = DayType.Day;
            DayLight.color = DayColor;
            dayIcon.rectTransform.anchoredPosition = new Vector2(dayIcon.rectTransform.anchoredPosition.x, 0f);
            nightIcon.rectTransform.anchoredPosition = new Vector2(nightIcon.rectTransform.anchoredPosition.x, -30f);
            setEmissionColor = Color.black;
            RenderSettings.skybox.SetFloat("_Amount", 0f);
        }
        else
        {
            // 밤
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
                    SetResetTime();// 날이 지나면
                    hour = 0;
                    day++;
                }
            }
            CheckLoanTime();
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
        Debug.Log($"밤낮 : {_dayType}");
        lightMode = _dayType;
        Color prevColor = DayLight.color;
        Color targetColor = lightMode == DayType.Day ? DayColor : NightColor;
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime;
            RenderSettings.skybox.SetFloat("_Amount", lightMode == DayType.Day ? 1f - normalize : normalize);
            // 라이트 변경
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
        switch (_index)
        {
            case 0:
                weekText.text = "Sun";
                break;
            case 1:
                weekText.text = "Mon";
                SetWeeklyReset();
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
                weekText.text = "Error";
                break;
        }
    }

    public void SetWeeklyReset()
    {
        Game_Manager.current.fishingNews.SetWeeklyReset();
    }
    //==========================================================================================================
    // 외부 컨트롤
    //==========================================================================================================
    public void TimePause(bool _pause)
    {
        paused = _pause;
        if (timeUpdate != null)
            StopCoroutine(timeUpdate);
        timeUpdate = StartCoroutine(TimeUpdate());
    }

    public void SetRestTime(int _hour, float _minute)// 시간 이동
    {
        hour += _hour;
        minute += _minute;
        if (hour >= 24)
        {
            SetResetTime();// 모든 데이터 리셋
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

    //==========================================================================================================
    // 대출
    //==========================================================================================================

    [Header(" [ 대출 ]")]
    public bool loanActive = false;
    public int loanTime;

    public void StartLoanTimer(bool _loanActive)
    {
        loanActive = _loanActive;
        loanTime = 0;
    }

    void CheckLoanTime()
    {
        if (loanActive == true)
        {
            loanTime++;
            if (loanTime >= 144)// 10분 * 6 *24 = 1일 = 144
            {
                // 게임 오버
                Game_Manager.current.GameOver();
                loanTime = 0;
                //StopAllCoroutines();
            }
        }
    }

    //==========================================================================================================
    // 하루 초기화
    //==========================================================================================================

    public delegate void DeleDayReset();
    public DeleDayReset deleDayReset;

    void SetResetTime()
    {
        deleDayReset?.Invoke();
    }
}
