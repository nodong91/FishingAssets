using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static StaticOpenCanvas;

public class Rest_Manager : MonoBehaviour
{
    public CanvasStruct[] canvasStructs;
    public Custom_Button restButton, backButton;
    public CanvasGroup canvasGroup;
    public Slider timeSlider;
    public TMPro.TMP_Text timeText;
    public TMPro.TMP_Text beforeTimeText, afterTimeText;

    public int hour;
    public int minute;
    public int day;
    public GameObject dayPlus;
    public void SetStart()
    {
        restButton.SetButton(RestButton);
        backButton.SetButton(Game_Manager.current.GetLanding.BackButton);
        timeSlider.onValueChanged.AddListener(TimeSliderChange);
    }

    void TimeSliderChange(float _value)
    {
        //time = Mathf.Clamp(time + (int)(eventData.delta.y), 0f, 48f);
        minute = (int)(_value % 2f) * 3;
        hour = (int)(_value / 2f);
        timeText.text = $"{hour.ToString("00")}:{(minute * 10).ToString("00")}";
        //setMinute.text = (minute * 10).ToString("00");
        //dragImage.fillAmount = _value / 48f;
        int currentHour = Game_Manager.current.GetMainUI.timeUI.hour;
        int currentMinute = (int)Game_Manager.current.GetMainUI.timeUI.minute * 10;
        beforeTimeText.text = $"{currentHour.ToString("00")}:{currentMinute.ToString("00")}";
        AddTime(currentHour, currentMinute);
    }

    void AddTime(int _currentHour, int _currentMinute)
    {
        day = 0;
        int addHour = _currentHour + hour;
        int addMinute = _currentMinute + minute * 10;
        if (addMinute >= 60)
        {
            addMinute -= 60;
            addHour += 1;
        }
        if (addHour >= 24)
        {
            addHour -= 24;
            day = 1;
        }
        dayPlus.gameObject.SetActive(day > 0);
        afterTimeText.text = $"{addHour.ToString("00")}:{addMinute.ToString("00")}";
    }

    public void OpenCanvas(bool _open)
    {
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
        if (_open == true)
        {
            timeSlider.value = 0;
            TimeSliderChange(0f);
        }
    }

    void RestButton()
    {
        if (hour + minute > 0)
            StartCoroutine(SetRest());
        else
            Game_Manager.current.GetLanding.BackButton();
    }

    IEnumerator SetRest()
    {
        canvasGroup.gameObject.SetActive(true);

        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime;
            CanvasAlpha(normalize);
            yield return null;
        }
        Debug.LogWarning($"Rest Complete : {hour} : {minute}");
        Game_Manager.current.GetTimeUI.SetRestTime(hour, minute);// 시간 적용

        OpenCanvas(false);
        Game_Manager.current.CurrentLand.CameraOutFouce(false);// 포커스 취소
        yield return new WaitForSeconds(1f);

        normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime;
            CanvasAlpha(1f - normalize);
            yield return null;
        }
        canvasGroup.gameObject.SetActive(false);
        Singleton_Continue.INSTANCE.SaveContinue();// 휴식 후 저장
        Game_Manager.current.GetLanding.SetLandingCanvas(true);// 랜드 UI 열기
    }

    void CanvasAlpha(float _alpha)
    {
        canvasGroup.alpha = _alpha;
        canvasGroup.blocksRaycasts = _alpha > 0f;
        canvasGroup.interactable = _alpha > 0f;
    }
}
