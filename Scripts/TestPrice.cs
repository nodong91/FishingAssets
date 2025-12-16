using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;
using static Fishing_News;

public class TestPrice : MonoBehaviour
{
    public Image[] graphPoints;

    public void SetStart(List<SellCountStruct> _sellCountStruct)
    {
        for (int i = 0; i < graphPoints.Length; i++)
        {
            graphPoints[i].rectTransform.anchoredPosition = new Vector2(graphPoints[i].rectTransform.anchoredPosition.x, (_sellCountStruct[i].sellCount - 100f) * 3f);
        }

        for (int i = 0; i < graphPoints.Length - 1; i++)
        {
            // Draw line between graphPoints[i] and graphPoints[i + 1] ¶óÀÎ·»´õ¸µ
        }
    }

    public void SetPrice(AreaType areaType, List<float> _prices)
    {

    }
}
