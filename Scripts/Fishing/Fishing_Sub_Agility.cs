using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fishing_Sub_Agility : Fishing_Sub
{
    public Image arrowImage;
    public float type1Value = 50f;
    public TMPro.TMP_Text type3Text;
    Coroutine playing;
    public float testSpeed = 1f;

    //==================================================================================================================================
    //
    //==================================================================================================================================

    public override void SetStart()
    {
        base.SetStart();
    }

    public override void StartGame()
    {
        base.StartGame();
        SetKeyCode();
        if (playing != null)
            StopCoroutine(playing);
        playing = StartCoroutine(Playing());
    }

    public override void InputMouseLeft(bool _input)
    {
        bool active = (keyCode == 0);
        FillAmount(active);
    }

    public override void InputMouseRight(bool _input)
    {
        bool active = (keyCode == 1);
        FillAmount(active);
    }

    void FillAmount(bool _active)
    {
        EndGame();
        if (_active == false)
            return;

        float areaX = gageImage.rectTransform.anchoredPosition.x;
        float areaSize = gageImage.rectTransform.sizeDelta.x * 0.5f;
        float arrowX = arrowImage.rectTransform.anchoredPosition.x;
        if (areaX - areaSize < arrowX && areaX + areaSize > arrowX)
        {
            // 성공
            SetKeyCode();
            if (AddAmount(0.1f) == true)
            {
                EndGame();
            }
        }
        else
        {
            // 실패
            //return;
        }
    }

    void SetKeyCode()
    {
        keyCode = Random.Range(0, 2);
        type3Text.text = (keyCode == 0) ? "Left" : "Right";

        float randomPosition = Random.Range(-type1Value, type1Value);
        gageImage.rectTransform.anchoredPosition = new Vector3(randomPosition, 0f);

        float randomScale = Random.Range(10f, 30f);
        gageImage.rectTransform.sizeDelta = new Vector2(randomScale, gageImage.rectTransform.sizeDelta.y);
    }

    IEnumerator Playing()
    {
        float runningTime = 0f;
        while (true)
        {
            runningTime += testSpeed * Time.deltaTime;
            float mathfSin = (Mathf.Sin(runningTime) + 1f) / 2f;
            float value = Mathf.Lerp(-type1Value, type1Value, mathfSin);
            arrowImage.rectTransform.anchoredPosition = new Vector2(value, 0f);
            //Debug.LogWarning(mathfSin);
            yield return null;
        }
    }
}
