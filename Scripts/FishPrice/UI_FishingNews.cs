using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Data_Manager;

public class UI_FishingNews : MonoBehaviour
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    public AreaType areaType;
    public TMPro.TMP_Text areaText;
    Dictionary<AreaType, int> sellDict = new Dictionary<AreaType, int>();
    Dictionary<AreaType, int> randomDict = new Dictionary<AreaType, int>();
    public UI_FishingNews_Point[] lineRects;
    public Custom_Button[] areaButtons;

    public int sellCount = 0;
    public float totalCount = 0;

    public List<float> shallowPrices = new List<float>();
    public List<float> coastalPrices = new List<float>();
    public List<float> oceanicPrices = new List<float>();
    public List<float> abyssalPrices = new List<float>();


    [System.Serializable]
    public struct SellCountStruct
    {
        [HideInInspector]
        public string areaName;
        public AreaType areaType;
        public float sellCount;

        public SellCountStruct(AreaType _areaType, float _sellCount)
        {
            this.areaName = $"{_areaType.ToString()} : {_sellCount}";
            this.areaType = _areaType;
            this.sellCount = _sellCount;
        }
    }
    public List<SellCountStruct> sellList = new List<SellCountStruct>();

    private void Start()
    {
        shallowPrices = new List<float>() { 100f, 100f, 100f, 100f, 100f, 100f, 100f };
        coastalPrices = new List<float>() { 100f, 100f, 100f, 100f, 100f, 100f, 100f };
        oceanicPrices = new List<float>() { 100f, 100f, 100f, 100f, 100f, 100f, 100f };
        abyssalPrices = new List<float>() { 100f, 100f, 100f, 100f, 100f, 100f, 100f };

        StartCoroutine(SetStart());
    }

    IEnumerator SetStart()
    {
        ResetAll();
        for (int i = 0; i < areaButtons.Length; i++)
        {
            int index = i;
            areaButtons[i].SetButton(() =>
            {
                areaType = (AreaType)(index + 1);
                SettestPrices();
            });
        }
        yield return new WaitForEndOfFrame();
        SettestPrices();
    }

    public void OpenCanvas(bool _open)
    {
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SetPrice(areaType);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            ResetSellDict();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SettestPrices();
        }
    }


    void SetPrice(AreaType areaType)
    {
        ResetAll();
        for (int i = 0; i < 20; i++)// 잡은 물고기 수
        {
            if (sellDict.ContainsKey(areaType) == false)
            {
                sellDict.Add(areaType, 0);
            }
            sellDict[areaType]++;
        }
        sellCount = TrySellCount();
        SetSellList();

        int randomCount = (10 + sellCount) * (int)AreaType.Abyssal;// 팔린 물고기 수 * 깊이 타입
        totalCount = randomCount + sellCount;
        for (int i = 0; i < randomCount; i++)// 랜덤 시세
        {
            int randomFactor = Random.Range(0, (int)AreaType.Abyssal) + 1;// 1~4
            randomDict[(AreaType)randomFactor]++;
        }
        SetPercent();
    }

    int TrySellCount()
    {
        int tempCount = 0;
        foreach (var area in sellDict)
        {
            tempCount += area.Value;
        }
        return tempCount;
    }

    void SetSellList()
    {
        sellList.Clear();
        foreach (var area in sellDict)
        {
            SellCountStruct testClass = new SellCountStruct(area.Key, area.Value);
            sellList.Add(testClass);
        }
    }

    void SetPercent()
    {
        float totalPercent = 0f;
        float perSell = (sellCount * 2f / (float)AreaType.Abyssal);
        for (int i = 0; i < (int)AreaType.Abyssal; i++)// 랜덤 시세
        {
            AreaType areaType = (AreaType)(i + 1);// 1~4
            int randomValue = randomDict[areaType];
            int sellValue = sellDict.ContainsKey(areaType) ? sellDict[areaType] : 0;

            float percent = (randomValue + perSell - sellValue) / totalCount * 100f * (float)AreaType.Abyssal;
            switch (areaType)
            {
                case AreaType.Shallow:
                    if (shallowPrices.Count > 6)
                        shallowPrices.RemoveAt(0);
                    shallowPrices.Add(percent);
                    break;

                case AreaType.Coastal:
                    if (coastalPrices.Count > 6)
                        coastalPrices.RemoveAt(0);
                    coastalPrices.Add(percent);
                    break;

                case AreaType.Oceanic:
                    if (oceanicPrices.Count > 6)
                        oceanicPrices.RemoveAt(0);
                    oceanicPrices.Add(percent);
                    break;

                case AreaType.Abyssal:
                    if (abyssalPrices.Count > 6)
                        abyssalPrices.RemoveAt(0);
                    abyssalPrices.Add(percent);
                    break;
            }
            totalPercent += percent;
        }
        SettestPrices();
        Debug.Log($"총 시세 갯수 : {totalCount}, 팔린 갯수 : {sellCount} = {totalPercent}");
    }

    void SettestPrices()
    {
        areaText.text = areaType.ToString();
        Color areaColor = Color.white;
        List<float> prices = new List<float>();
        switch (areaType)
        {
            case AreaType.Shallow:
                prices = shallowPrices;
                areaColor = new Color(0.4f, 0.8f, 1f); // Light Blue
                break;
            case AreaType.Coastal:
                prices = coastalPrices;
                areaColor = new Color(0.2f, 0.6f, 0.2f); // Green
                break;
            case AreaType.Oceanic:
                prices = oceanicPrices;
                areaColor = new Color(1f, 0.5f, 0f); // Orange
                break;
            case AreaType.Abyssal:
                prices = abyssalPrices;
                areaColor = new Color(0.6f, 0.2f, 0.8f); // Purple
                break;
        }

        for (int i = 0; i < lineRects.Length; i++)
        {
            UI_FishingNews_Point next = (i + 1 < lineRects.Length) ? lineRects[i + 1] : null;
            lineRects[i].SetStart(next, areaColor);
        }
        SetPrice(prices);
    }

    void SetPrice(List<float> _price)
    {
        for (int i = 0; i < _price.Count; i++)
        {
            lineRects[i].SetPoint(_price[i]);
        }

        for (int i = 0; i < lineRects.Length; i++)
        {
            lineRects[i].UpdateLine();
        }
    }

    private void ResetAll()
    {
        ResetAreaDictionary();
        totalCount = 0;
    }

    private void ResetSellDict()
    {
        sellDict.Clear();
        int areaCount = (int)AreaType.Abyssal;
        for (int i = 0; i < areaCount; i++)// 사전 리셋
        {
            AreaType areaType = (AreaType)i + 1;
            sellDict.Add(areaType, 0);// 기본 수치
        }
    }

    void ResetAreaDictionary()
    {
        randomDict.Clear();
        int areaCount = (int)AreaType.Abyssal;
        for (int i = 0; i < areaCount; i++)// 사전 리셋
        {
            AreaType areaType = (AreaType)i + 1;
            randomDict.Add(areaType, 0);// 기본 수치
        }
    }
}
