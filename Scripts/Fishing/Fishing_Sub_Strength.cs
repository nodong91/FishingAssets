using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fishing_Sub_Strength : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Image test3Image;
    public float type3FillAmount;
    public TMPro.TMP_Text type3Text;
    Coroutine playing;
    public float testSpeed = 1f;
    //==================================================================================================================================
    // 3
    //==================================================================================================================================
    int keyCode;

    public delegate void DeleEndGame();
    public DeleEndGame deleEndGame;

    public void StartGame()
    {
        canvasGroup.gameObject.SetActive(true);
        SetKeyCode();
        if (playing != null)
            StopCoroutine(playing);
        playing = StartCoroutine(Playing());
    }

    void EndGame()
    {
        deleEndGame?.Invoke();
        canvasGroup.gameObject.SetActive(false);
    }

    public void Action_Left()
    {
        if (keyCode == 0)
        {
            if (type3FillAmount < 1f)
                type3FillAmount += 0.05f;
        }
    }

    public void Action_Right()
    {
        if (keyCode == 1)
        {
            if (type3FillAmount > 0f)
                type3FillAmount -= 0.05f;
        }
    }

    void SetKeyCode()
    {
        keyCode = Random.Range(0, 2);
        type3Text.text = (keyCode > 0) ? "Left" : "Right";
    }

    IEnumerator Playing()
    {
        type3FillAmount = 0f;
        float runningTime = 0f;
        float randomTime = 0f;
        bool setStart = true;
        while (setStart == true)
        {
            runningTime += testSpeed * Time.deltaTime;
            if (type3FillAmount > 1f)
            {
                setStart = false;
            }
            else if (type3FillAmount > 0f)
            {
                type3FillAmount -= Time.deltaTime * 0.1f;
            }
            test3Image.material.SetFloat("_FillAmount", type3FillAmount);
            if (runningTime > randomTime)
            {
                randomTime += Random.Range(1f, 3f);
                SetKeyCode();
            }
            yield return null;
        }
        EndGame();
    }
}
