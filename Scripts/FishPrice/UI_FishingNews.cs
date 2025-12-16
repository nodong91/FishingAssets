using System.Collections.Generic;
using UnityEngine;
using static Data_Manager;

public class Fishing_News : MonoBehaviour
{
    public AreaType areaType;
    public TMPro.TMP_Text areaText;
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
    Dictionary<AreaType, int> sellDict = new Dictionary<AreaType, int>();
    Dictionary<AreaType, int> randomDict = new Dictionary<AreaType, int>();
    public UI_FishingNews_Point[] lineRects;
    public Custom_Button[] areaButtons;

    public int sellCount = 0;
    public float totalCount = 0;
    public List<SellCountStruct> sellList = new List<SellCountStruct>();

    public List<float> shallowPrices = new List<float>();
    public List<float> coastalPrices = new List<float>();
    public List<float> oceanicPrices = new List<float>();
    public List<float> abyssalPrices = new List<float>();

    private void Start()
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
        SettestPrices();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ResetAll();
            SetPrice();
            for (int i = 0; i < 20; i++)// 잡은 물고기 수
            {
                int randomFactor = Random.Range(0, (int)AreaType.Abyssal) + 1;// 1~4// 잡은 물고기 타입
                randomFactor = 1;
                AreaType areaType = (AreaType)randomFactor;
                sellDict[areaType]++;
                sellCount++;
            }
            SetSellList();

            int randomCount = (10 + sellCount) * (int)AreaType.Abyssal;// 팔린 물고기 수 * 깊이 타입
            totalCount = randomCount + sellCount;
            for (int i = 0; i < randomCount; i++)// 랜덤 시세
            {
                int randomFactor = Random.Range(0, (int)AreaType.Abyssal) + 1;// 1~4
                AreaType areaType = (AreaType)randomFactor;
                randomDict[areaType]++;
            }
            SetTest();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            SettestPrices();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetAll();
            SetPrice();
            SetSellList();
        }
    }

    void SetPrice()
    {
        for (int i = 0; i < (int)AreaType.Abyssal; i++)// 시세 합
        {
            int randomFactor = i + 1;// 1~4
            AreaType areaType = (AreaType)randomFactor;
            int addValue = 0;
            switch (areaType)
            {
                case AreaType.Shallow:
                    addValue = TryAddValue(shallowPrices);
                    break;
                case AreaType.Coastal:
                    addValue = TryAddValue(coastalPrices);
                    break;
                case AreaType.Oceanic:
                    addValue = TryAddValue(oceanicPrices);
                    break;
                case AreaType.Abyssal:
                    addValue = TryAddValue(abyssalPrices);
                    break;
            }
            sellDict[areaType] = addValue;
        }
    }

    int TryAddValue(List<float> _addList)
    {
        int addValue = 0;
        for (int i = 0; i < _addList.Count; i++)
        {
            addValue += (int)_addList[i];
        }
        return addValue;
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

    void SetTest()
    {
        float perSell = (sellCount * 2f / (float)AreaType.Abyssal);
        for (int i = 0; i < (int)AreaType.Abyssal; i++)// 랜덤 시세
        {
            AreaType areaType = (AreaType)(i + 1);// 1~4
            int randomValue = randomDict[areaType];
            int sellValue = sellDict[areaType];
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
        }
        SettestPrices();
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
        sellCount = 0;
        totalCount = 0;
        sellList.Clear();
    }

    void ResetAreaDictionary()
    {
        sellDict.Clear();
        randomDict.Clear();
        int areaCount = (int)AreaType.Abyssal;
        for (int i = 0; i < areaCount; i++)// 사전 리셋
        {
            AreaType areaType = (AreaType)i + 1;
            sellDict.Add(areaType, 0);// 기본 수치
            randomDict.Add(areaType, 0);// 기본 수치
        }
    }
}
