using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fishing_Hit : MonoBehaviour
{
    //==================================================================================================================================
    // 
    //==================================================================================================================================
    public CanvasGroup canvasGroup;
    public float runningSpeed = 1f;
    public RectTransform spinTrans;
    public Image hookImage;
    public float currentAngle;
    Coroutine playing;
    bool spinStop;

    public delegate void DeleEndGame();
    public DeleEndGame deleEndGame;

    public void StartGame()
    {
        canvasGroup.gameObject.SetActive(true);
        spinStop = false;
        if (playing != null)
            StopCoroutine(playing);
        playing = StartCoroutine(Playing());
    }

    void EndGame()
    {
        deleEndGame?.Invoke();
        canvasGroup.gameObject.SetActive(false);
    }

    public void Action()
    {
        spinStop = true;
    }

    IEnumerator Playing()
    {
        float runningTime = 0f;
        float rotateSpeed = runningSpeed;
        //float randomStop = runningSpeed * Random.Range(0.1f, 1f);
        bool spinStart = true;
        while (spinStart == true)
        {
            runningTime += Time.deltaTime * rotateSpeed;
            spinTrans.rotation = Quaternion.Euler(0, 0, runningTime);
            currentAngle = runningTime % 360f;
            yield return null;

            if (spinStop == true)
            {
                if (rotateSpeed > 0)
                {
                    //rotateSpeed -= Time.deltaTime * randomStop;
                    rotateSpeed -= Time.deltaTime * runningSpeed * 10f;
                }
                else
                {

                    spinStart = false;
                }
            }
        }
        for (int i = 0; i < 3; i++)
        {
            // 성공 실패 연출
            yield return new WaitForSeconds(1f);//3초 뒤에
        }
        EndGame();
    }
}
