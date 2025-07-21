using System.Collections;
using UnityEngine;

public class Fishing_Sub_Strength : Fishing_Sub
{
    public TMPro.TMP_Text type3Text;
    Coroutine playing;
    public float testSpeed = 1f;
    //==================================================================================================================================
    // 3
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
        float addValue = keyCode == 0 ? 1f : -1f;
        FillAmount(addValue);
    }

    public override void InputMouseRight(bool _input)
    {
        float addValue = keyCode == 1 ? 1f : -1f;
        FillAmount(addValue);
    }

    void FillAmount(float _value)
    {
        EndGame();
        //if (fillAmount > 0f || fillAmount < 1f)
        //{
        //    fillAmount += 0.05f * _value;
        //}
        AddAmount(0.05f * _value);
    }

    void SetKeyCode()
    {
        keyCode = Random.Range(0, 2);
        type3Text.text = (keyCode == 0) ? "Left" : "Right";
    }

    IEnumerator Playing()
    {
        fillAmount = 0f;
        float runningTime = 0f;
        float randomTime = 0f;
        bool setStart = true;
        while (setStart == true)
        {
            runningTime += testSpeed * Time.deltaTime;
            //if (fillAmount > 1f)
            //{
            //    setStart = false;
            //}
            //else if (fillAmount > 0f)
            //{
            //    fillAmount -= Time.deltaTime * 0.1f;
            //}
            if (AddAmount(-Time.deltaTime * 0.1f) == true)
            {
                setStart = false;
            }
            gageImage.material.SetFloat("_FillAmount", fillAmount);
            if (runningTime > randomTime)
            {
                randomTime += Random.Range(0.5f, 2f);
                SetKeyCode();
            }
            yield return null;
        }
        EndGame();
    }
}
