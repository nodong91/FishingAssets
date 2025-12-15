using System.Collections.Generic;
using UnityEngine;
using static Data_Manager;

public class TestPrice : MonoBehaviour
{
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

    public int sellCount = 0;
    public float totalCount = 0;
    public List<SellCountStruct> sellList = new List<SellCountStruct>();
    public List<SellCountStruct> pricePercent = new List<SellCountStruct>();

    private void Start()
    {
        ResetAll();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ResetAll();
            for (int i = 0; i < 20; i++)
            {
                int randomFactor = Random.Range(0, (int)AreaType.Abyssal) + 1;// 1~4// 잡은 물고기 타입
                AreaType areaType = (AreaType)randomFactor;
                sellDict[areaType]++;
                sellCount++;
                SetSellList();
            }

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
            SavePercent();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            DisplayGraph();
        }
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
        float test = 0f;
        float perSell = (sellCount * 2f / (float)AreaType.Abyssal);
        pricePercent.Clear();
        for (int i = 0; i < (int)AreaType.Abyssal; i++)// 랜덤 시세
        {
            AreaType areaType = (AreaType)(i + 1);// 1~4
            int randomValue = randomDict[areaType];
            int sellValue = sellDict[areaType];
            float percent = (randomValue + perSell - sellValue) / totalCount * 100f * (float)AreaType.Abyssal;
            SellCountStruct sellCountStruct = new SellCountStruct(areaType, percent);
            pricePercent.Add(sellCountStruct);
            Debug.LogWarning($"AreaType : {areaType} / RandomValue : {randomValue} / SellValue : {sellValue} / Percent : {percent}% -- {test += percent}");
        }
    }

    private void ResetAll()
    {
        ResetAreaDictionary();
        sellCount = 0;
        totalCount = 0;
        sellList.Clear();
        pricePercent.Clear();
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
    [System.Serializable]
    public struct SaveStruct
    {
        public List<SellCountStruct> pricePercent;
        public SaveStruct(List<SellCountStruct> _pricePercent)
        {
            pricePercent = _pricePercent;
        }
    }

    private void DisplayGraph()
    {
        for (int i = 0; i < saveCountStructs.Count; i++)
        {
            List<SellCountStruct> percentStruct = saveCountStructs[i].pricePercent;
            //Debug.LogWarning($"Graph AreaType : {percentStruct.areaType} / Percent : {percentStruct.sellCount}%");
        }
    }

    public List<SaveStruct> saveCountStructs = new List<SaveStruct>();
    void SavePercent()
    {
        SaveStruct temp = new SaveStruct(new List<SellCountStruct>(pricePercent));
        saveCountStructs.Add(temp);
        foreach (var percent in pricePercent)
        {
            Debug.LogWarning($"AreaType : {percent.areaType} / Percent : {percent.sellCount}%");
        }
    }
}
