using NUnit.Framework.Internal;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NewFishing : MonoBehaviour
{

    void Start()
    {
        StartFishing();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            LineTention(true);
        }
        else
        {
            LineTention(false);
        }
        if (fishPulling == false)
        {
            fishPower = Mathf.Lerp(fishPower, 0f, Time.deltaTime);
        }
    }
    public Vector2 randomTime;
    Coroutine fishingCoroutine;
    public TMPro.TMP_Text testText;
    bool fishPulling = false;
    void StartFishing()
    {
        if (fishingCoroutine != null)
            StopCoroutine(fishingCoroutine);
        fishingCoroutine = StartCoroutine(Fishing());
    }

    IEnumerator Fishing()
    {
        yield return new WaitForSeconds(Random.Range(randomTime.x, randomTime.y));
        fishPulling = true;
        fishPower = 0f;
        float normalizedTime = 0f;
        while (normalizedTime < 1f)
        {
            normalizedTime += Time.deltaTime * 2f;
            fishPower = Mathf.Lerp(0f, 1f, normalizedTime);
            Debug.Log("Fishing");
            yield return null;
        }

        float delayTime = Random.Range(0.5f, 2f);
        normalizedTime = 0f;
        while (normalizedTime < delayTime)
        {
            normalizedTime += Time.deltaTime;
            fishPower += Random.Range(-1f, 1f) * 0.02f;
            yield return null;
        }
        fishPulling = false;
        StartFishing();
    }

    public Image testRect;
    float lodPower = 0f;
    float fishPower = 0f;
    public float clampX = 0f;
    public Vector2 testSize = new Vector2(20f, 100f);
    public float tentionSpeed = 0.1f;
    [ColorUsage(true, true)]
    public Color testColor01, testColor02;
    public void LineTention(bool _pull)
    {
        float targetSize = _pull == true ? 1f : 0f;
        lodPower = Mathf.Lerp(lodPower, targetSize, Time.deltaTime * tentionSpeed);
        clampX = lodPower + fishPower;
        float xSize = Mathf.Lerp(testSize.x, testSize.y, clampX);
        testRect.rectTransform.sizeDelta = new Vector2(xSize, testRect.rectTransform.sizeDelta.y);
        testRect.color = Color.Lerp(testColor01, testColor02, clampX);
    }
}
