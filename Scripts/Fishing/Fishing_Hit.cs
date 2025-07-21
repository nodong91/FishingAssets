using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fishing_Hit : MonoBehaviour
{
    //==================================================================================================================================
    // 
    //==================================================================================================================================
    public CanvasGroup canvasGroup;
    public float runningSpeed = 360f;
    public RectTransform spinTrans;
    public Image criticalImage, catchImage, hookImage;
    public Image resultImage;
    public TMPro.TMP_Text resultText;
    public float currentAngle;
    Coroutine playing;
    bool spinStop;

    public delegate void DeleEndGame(Fishing_Manager.FishingState _state);
    public DeleEndGame deleEndGame;

    public void SetStart()
    {
        criticalImage.material = Instantiate(criticalImage.material);
        catchImage.material = Instantiate(catchImage.material);
        resultImage.material = Instantiate(resultImage.material);

        canvasGroup.gameObject.SetActive(false);
        SetFillAmount();
    }

    void SetFillAmount()
    {
        float fillAmount = 0.1f;
        criticalAngle = fillAmount * 360f * 0.5f;
        criticalImage.material.SetFloat("_FillAmount", fillAmount);
        criticalImage.material.SetFloat("_RotateAngle", criticalAngle + 180f);

        fillAmount = 0.4f;
        catchAngle = fillAmount * 360f * 0.5f;
        catchImage.material.SetFloat("_FillAmount", fillAmount);
        catchImage.material.SetFloat("_RotateAngle", catchAngle + 180f);
    }

    public void StartGame()
    {
        resultText.gameObject.SetActive(false);
        canvasGroup.gameObject.SetActive(true);

        spinStop = false;
        if (playing != null)
            StopCoroutine(playing);
        playing = StartCoroutine(Playing());
    }

    void EndGame()
    {
        switch (hitResult)
        {
            case HitResult.Critical:
                deleEndGame?.Invoke(Fishing_Manager.FishingState.Main);// 메인으로 이동
                break;

            case HitResult.Hit:
                deleEndGame?.Invoke(Fishing_Manager.FishingState.Main);// 메인으로 이동
                break;

            case HitResult.Fail:
                deleEndGame?.Invoke(Fishing_Manager.FishingState.Ready);// 실패
                break;
        }
        canvasGroup.gameObject.SetActive(false);
    }
    public float getAngle;
    public float criticalAngle, catchAngle;


    public void InputMouseLeft(bool _input)
    {
        if (_input == true)
        {
            spinStop = true;
        }
    }

    public void InputMouseRight(bool _input)
    {
        if (_input == true)
        {
            spinStop = true;
        }
    }

    public void Action()
    {
        spinStop = true;
    }

    public enum HitResult
    {
        Critical,
        Hit,
        Fail
    }
    public HitResult hitResult;
    void Result()
    {
        Vector3 offset = hookImage.transform.position - spinTrans.transform.position;
        getAngle = Vector3.Angle(spinTrans.transform.up, offset.normalized);

        resultText.gameObject.SetActive(true);
        if (getAngle < criticalAngle)
        {
            // 크리티컬
            hitResult = HitResult.Critical;
            resultText.text = "Critical Hit";
        }
        else if (getAngle < catchAngle)
        {
            // 잡았다.
            hitResult = HitResult.Hit;
            resultText.text = "Hit";
        }
        else
        {
            // 실패
            hitResult = HitResult.Fail;
            resultText.text = "Fail";
        }
    }

    IEnumerator Playing()
    {
        float runningTime = 0f;
        float rotateSpeed = runningSpeed;
        float randomStop = runningSpeed * Random.Range(1f, 3f);
        bool spinStart = true;
        while (spinStart == true)
        {
            runningTime += Time.deltaTime * rotateSpeed;
            spinTrans.localRotation = Quaternion.Euler(0f, 0f, -runningTime);
            currentAngle = runningTime % 360f;
            yield return null;

            if (spinStop == true)
            {
                if (rotateSpeed > 0)
                {
                    rotateSpeed -= Time.deltaTime * randomStop;
                }
                else
                {

                    spinStart = false;
                }
            }
        }
        Result();
        runningTime = 0f;
        while (runningTime < 1f)
        {
            runningTime += Time.deltaTime * 2f;
            resultImage.material.SetFloat("_Amount", runningTime);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        EndGame();
    }
}
