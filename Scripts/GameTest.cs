using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTest : MonoBehaviour
{
    public float fillArea, fillAngle;
    public Image[] centerImage;
    public bool visible = false, checkIndex = false;
    public GameObject target;
    public int currentCenter;

    void Start()
    {
        fillAngle = fillArea * 180f;
        for (int i = 0; i < centerImage.Length; i++)
        {
            centerImage[i].material = Instantiate(centerImage[i].material);
            centerImage[i].color = Color.red;

            centerImage[i].material.SetFloat("_FillAmount", fillArea);
            centerImage[i].material.SetFloat("_RotateAngle", fillAngle + 180f);
        }
    }

    void Update()
    {
        //fillAngle = fillArea * 180f;
        //centerImage[currentCenter].material.SetFloat("_FillAmount", fillArea);
        //centerImage[currentCenter].material.SetFloat("_RotateAngle", fillAngle + 180f);

        CheckInAngle();
    }

    void CheckInAngle()
    {
        visible = VisibleTarget();

        Color color = visible == true ? Color.white : Color.red;
        centerImage[currentCenter].color = color;

        if (visible == true)
            checkIndex = true;
        if (checkIndex == true)
        {
            if (visible == false && currentCenter + 1 < centerImage.Length)
            {
                checkIndex = false;
                currentCenter++;
            }
        }
    }
    bool VisibleTarget()// ���̴��� Ȯ��
    {
        Transform center = centerImage[currentCenter].transform;
        Vector3 offset = (target.transform.position - center.position);
        float getAngle = Vector3.Angle(center.up, offset.normalized);
        if (getAngle < fillAngle)// �ޱ� �ȿ� ���� �Ǵ���
        {
            return true;
        }
        return false;
    }
}
