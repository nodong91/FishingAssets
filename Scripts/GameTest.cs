using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTest : MonoBehaviour
{
    public float fillAmount, rotateAngle;
    public RectTransform rectParent;
    public Image baseImage;
    //public Image[] centerImage;
    public bool visible = false, checkIndex = false;
    public GameObject fishObject, target;
    public int currentCenter;
    public float runningTime;
    public int setAmount;

    void Start()
    {
        //centerImage = new Image[setAmount];

        ResetGame();
    }
    public float rotateSpeed = 100f;
    void Update()
    {
        if (runningTime < 3.6f)
        {
            runningTime += Time.deltaTime;
        }
        else
        {
            ResetGame();
        }
        fishObject.transform.localRotation = Quaternion.Euler(0f, 0f, runningTime * rotateSpeed);
        CheckInAngle();
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        ResetGame2();
    //    }
    //}

    void InstnaceCenter()
    {
        for (int i = 0; i < centerList.Count; i++)
        {
            centerList[i].gameObject.SetActive(false);
            imageQueue.Enqueue(centerList[i]);
        }
        centerList.Clear();
        for (int i = 0; i < setAmount; i++)
        {
            Image inst = TryCircleImage();
            inst.gameObject.SetActive(true);
            inst.transform.localPosition = Vector3.zero;
            centerList.Add(inst);
        }
    }

    void ResetGame()
    {
        InstnaceCenter();

        runningTime = 0f;
        currentCenter = 0;

        fillAmount = 1f / setAmount * 0.5f;
        rotateAngle = fillAmount * 360f * 0.5f;
        for (int i = 0; i < centerList.Count; i++)
        {
            centerList[i].color = Color.red;
            centerList[i].material.SetFloat("_FillAmount", fillAmount);
            centerList[i].material.SetFloat("_RotateAngle", rotateAngle + 180f);

            float amountAngle = 360f * fillAmount;
            float ringAngle = 360f / centerList.Count;
            float randomAmount = Random.Range((ringAngle * i) + amountAngle * 0.5f, (ringAngle * (i + 1)) - amountAngle * 0.5f);

            centerList[i].transform.localRotation = Quaternion.Euler(0f, 0f, randomAmount);
        }
    }
    public List<Image> centerList = new List<Image>();
    Queue<Image> imageQueue = new Queue<Image>();

    Image TryCircleImage()
    {
        if (imageQueue.Count > 0)
            return imageQueue.Dequeue();

        Image inst = Instantiate(baseImage, rectParent);
        inst.material = Instantiate(baseImage.material);
        return inst;
    }

    void CheckInAngle()
    {
        visible = VisibleTarget();

        Color color = visible == true ? Color.white : Color.red;
        centerList[currentCenter].color = color;

        if (visible == true)
            checkIndex = true;
        if (checkIndex == true)
        {
            if (visible == false)
            {
                checkIndex = false;
                if (currentCenter + 1 < centerList.Count)
                    currentCenter++;
            }
        }
    }
    bool VisibleTarget()// 보이는지 확인
    {
        Transform center = centerList[currentCenter].transform;
        Vector3 offset = (target.transform.position - center.position);
        float getAngle = Vector3.Angle(center.up, offset.normalized);
        if (getAngle < rotateAngle)// 앵글 안에 포함 되는지
        {
            return true;
        }
        return false;
    }
}
