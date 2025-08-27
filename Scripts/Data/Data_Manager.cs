using UnityEngine;
using System.Collections.Generic;



#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Data_Manager))]
public class DataManager_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        GUIStyle fontStyle = new GUIStyle(GUI.skin.button);
        fontStyle.fontSize = 15;
        fontStyle.normal.textColor = Color.yellow;

        Data_Manager Inspector = target as Data_Manager;
        if (GUILayout.Button("Data Parse", fontStyle, GUILayout.Height(30f)))
        {
            Inspector.UpdateData();
            EditorUtility.SetDirty(Inspector);
        }
        GUILayout.Space(10f);
        base.OnInspectorGUI();
    }
}
#endif

public class Data_Manager : Data_Parse
{
#if UNITY_EDITOR
    public void UpdateData()
    {
        DataSetting();
    }

    public override void DataSetting()
    {
        base.DataSetting();
        for (int i = 0; i < GetCSV_Data.Count; i++)
        {
            string csv_Type = GetCSV_Data[i].name;
            if (csv_Type.Contains("Fish"))
            {
                SetFish(GetCSV_Data[i]);
            }
            else if (csv_Type.Contains("Used"))
            {
                SetUsed(GetCSV_Data[i]);
            }
        }
    }

    void SetFish(TextAsset _textAsset)
    {
        fishStruct.Clear();
        string[] data = _textAsset.text.Split(new char[] { '\n' });
        for (int i = 1; i < data.Length; i++)// 첫째 라인 빼고 리스팅
        {
            string[] elements = data[i].Split(new char[] { ',' });
            ItemStruct tempItem = GetItemStruct(elements);
            tempItem.itemType = ItemStruct.ItemType.Fish;// 타입 세팅
            FishStruct tempData = new FishStruct
            {
                id = tempItem.id,
                itemStruct = tempItem,
                fishType = (FishStruct.FishType)System.Enum.Parse(typeof(FishStruct.FishType), elements[8]),
                size = Parse_Vector2(elements[9]),
                fishHealth = Parse_Float(elements[10]),
                fishPower = Parse_Float(elements[11]),
                fieldRadius = Parse_Float(elements[12]),
                fishSpeed = Parse_Float(elements[13]),
                fishAttackSpeed = Parse_Float(elements[14]),
                fishRange = Parse_Vector2(elements[15]),
                fishTime = (FishStruct.FishTime)System.Enum.Parse(typeof(FishStruct.FishTime), elements[16]),
            };
            fishStruct.Add(tempData);
        }
    }

    void SetUsed(TextAsset _textAsset)
    {
        usedStruct.Clear();
        string[] data = _textAsset.text.Split(new char[] { '\n' });
        for (int i = 1; i < data.Length; i++)// 첫째 라인 빼고 리스팅
        {
            string[] elements = data[i].Split(new char[] { ',' });
            ItemStruct tempItem = GetItemStruct(elements);
            tempItem.itemType = ItemStruct.ItemType.Used;// 타입 세팅
            UsedStruct tempData = new UsedStruct
            {
                id = tempItem.id,
                itemStruct = tempItem,
                usedType = (UsedStruct.UsedType)System.Enum.Parse(typeof(UsedStruct.UsedType), elements[8]),
            };
            usedStruct.Add(tempData);
        }
    }

    ItemStruct GetItemStruct(string[] _elements)
    {
        Vector2Int[] tempShape = Parse_Vector2IntArray(_elements[5].Trim());
        ItemStruct tempItem = new ItemStruct
        {
            id = _elements[0].Trim(),
            name = _elements[1],
            explanation = _elements[2],
            icon = FindSprite(_elements[3]),
            itemClass = (ItemStruct.ItemClass)System.Enum.Parse(typeof(ItemStruct.ItemClass), _elements[4]),
            shape = tempShape,
            iconSize = TryIconSize(tempShape),
            weight = Parse_Float(_elements[6]),
            price = Parse_Float(_elements[7]),
        };
        return tempItem;
    }

    Vector4 TryIconSize(Vector2Int[] _shape)
    {
        int minX = 0, minY = 0, maxX = 0, maxY = 0;
        for (int i = 0; i < _shape.Length; i++)
        {
            if (_shape[i].x < minX)
                minX = _shape[i].x;
            if (_shape[i].y < minY)
                minY = _shape[i].y;
            if (_shape[i].x > maxX)
                maxX = _shape[i].x;
            if (_shape[i].y > maxY)
                maxY = _shape[i].y;
        }
        int x = maxX - minX + 1;
        int y = maxY - minY + 1;
        float centerX = 0.5f - (minX + maxX) * 0.25f;
        float centerY = 0.5f + (minY + maxY) * 0.25f;

        Vector4 temp = new Vector4(x, y, centerY, centerX);
        return temp;
    }
#endif

    //==================================================================================
    // Data
    //==================================================================================
    [System.Serializable]
    public class Data_Continue
    {
        public Vector3 playerPosition;
        public Quaternion playerRotation;
        public Vector3 playerScale;

        public float timeSpeed;
        public float minute;
        public int hour;
        public int day;
        public UI_Time.WeatherType weatherType;

        public float energy;
        public float money;
        public List<Vector2Int> destroySlot;
    }

    [System.Serializable]
    public struct Data_Option
    {
        public bool fullScreen;
        public int qualityLevel;
        public int resolutionIndex;
        public int frameRateIndex;

        [System.Serializable]
        public struct AudioStruct
        {
            // 사운드 관련
            public bool bgmMute;
            public float bgmVolume;
            public bool fxMute;
            public float fxVolume;
            public bool envMute;
            public float envVolume;
        }
        public AudioStruct audioStruct;
    }

    [System.Serializable]
    public class SetStatus
    {
        [Header(" [ Catch ]")]
        public float catchRadius;// 물고기를 잡는 범위
        public float catchSpeed;// 낚시대가 물고기를 향해 이동하는 속도
        public float catchPower;// 낚시대의 힘
        public float catchMaxHealth;// 낚시대의 최대 체력
        public float catchAttakSpeed;// 물고기를 공격하는 빈도

        [Header(" [ Ship ]")]
        public float shipSpeed;// 배의 이동 속도
        public float maxWeight;// 인벤토리 중량
        public float maxEnergy;// 연료통 크기
        public Vector2Int maxBoxSize;// 인벤토리 크기
        public int shipHealth;// 배 체력
        public float freshness;// 신선도 유지 - 꼭 필요한가??????  

        [Header(" [ Fish ]")]
        public float LuckFish;// 희귀 물고기 확률
        public float FishAmount;// 낚시 횟수 증가
        public float FishPrice;// 판매 물고기 가격 증가

        public void SettingStatus(SetStatus _status)
        {
            catchRadius = _status.catchRadius;
            catchSpeed = _status.catchSpeed;
            catchPower = _status.catchPower;
            catchMaxHealth = _status.catchMaxHealth;
            catchAttakSpeed = _status.catchAttakSpeed;
            shipSpeed = _status.shipSpeed;
            maxWeight = _status.maxWeight;
            maxEnergy = _status.maxEnergy;
            maxBoxSize = _status.maxBoxSize;
            freshness = _status.freshness;
            LuckFish = _status.LuckFish;
            FishAmount = _status.FishPrice;
            FishPrice = _status.FishPrice;
        }

        public void AddStatus(SetStatus _status, int _remove = 1)
        {
            catchRadius += _status.catchRadius * _remove;
            catchSpeed += _status.catchSpeed * _remove;
            catchPower += _status.catchPower * _remove;
            catchMaxHealth += _status.catchMaxHealth * _remove;
            catchAttakSpeed += _status.catchAttakSpeed * _remove;
            shipSpeed += _status.shipSpeed * _remove;
            maxWeight += _status.maxWeight * _remove;
            maxEnergy += _status.maxEnergy * _remove;
            maxBoxSize += _status.maxBoxSize * _remove;
            freshness += _status.freshness * _remove;
            LuckFish += _status.LuckFish * _remove;
            FishAmount += _status.FishPrice * _remove;
            FishPrice += _status.FishPrice * _remove;
        }
    }

    [System.Serializable]
    public class StatusStruct
    {
        public string name;
        public string description;
        public string icon;
        public int price; // 가격 정보
        public SetStatus addStatus;
    }

    [System.Serializable]
    public struct UsedStruct
    {
        [HideInInspector]
        public string id;
        public ItemStruct itemStruct;
        public enum UsedType// 사용 효과
        {
            Energe, // 연료
            Bait_Coast, // 연안 미끼
            Bait_Shallow,// 얕은
            Bait_Ocean,// 대양
        }
        public UsedType usedType;
    }

    [System.Serializable]
    public struct ItemStruct
    {
        public string id;
        public string name;
        public enum ItemType
        {
            Equip,
            Fish,
            Used,
            Quest,
        }
        public ItemType itemType;
        [TextArea]
        public string explanation;// 설명
        public Sprite icon;
        public enum ItemClass
        {
            Legendary,
            Epic,
            Rare,
            Uncommon,
            Common,
        }
        public ItemClass itemClass;// 아이템 등급
        public Vector2Int[] shape;
        public Vector4 iconSize;
        public float weight;
        public float price;
    }

    [System.Serializable]
    public struct FishStruct// 물고기 정보
    {
        [HideInInspector]
        public string id;
        public ItemStruct itemStruct;
        public enum FishType
        {
            Coastal,
            Shallow,
            Oceanic,
            Abyssal,
            Hadal,
        }
        public FishType fishType;
        public Vector2 size;

        // 낚시 관련
        public float fishHealth;// 물고기 체력
        public float fishPower;// 물고기 공격력
        public float fieldRadius;// 물고기 활동 범위
        public float fishSpeed;// 물고기 이동 속도
        public float fishAttackSpeed;// 물고기 공격 속도
        public Vector2 fishRange;// 방향 바뀌는 딜레이 시간
        public enum FishTime
        {
            Any,
            Day,
            Night,
        }
        public FishTime fishTime;

        [System.Serializable]
        public struct RandomSize
        {
            public string id;
            public float size;
            public float weight;
            public float price;
        }

        // 랜덤 사이즈
        public RandomSize GetRandom()
        {
            float randomSize = Random.Range(size.x, size.y);
            float percent = GetPercent(size.y / randomSize);
            RandomSize randomFish = new RandomSize
            {
                id = itemStruct.id,
                size = GetPercent(size.y / percent),
                weight = GetPercent(itemStruct.weight / percent),
                price = GetPercent(itemStruct.price / percent),
            };
            return randomFish;
        }

        float GetPercent(float _origin)
        {
            float temp = Mathf.Round(_origin * 10f) * 0.1f;
            return temp;
        }
    }

    //[System.Serializable]
    //public struct DialogStruct
    //{
    //    public string id;
    //    public string contents;

    //    public enum ActionType
    //    {
    //        None,
    //        Move,
    //        Wave,
    //        Jitter
    //    }

    //    [System.Serializable]
    //    public struct DialogType
    //    {
    //        public Vector2Int dialogIndex;// 움직일 문장 시작과 끝 인덱스
    //        public float textSize;
    //        public float typingSpeed;
    //        public string textColor;
    //        [Header("Action")]
    //        public ActionType actionType;
    //        public float actionSpeed;
    //        public float actionInterval;
    //        public Vector2 actionAngle;
    //    }
    //    public DialogType[] dialogTypes;
    //}

    //==================================================================================
    // Data
    //==================================================================================

    [Header(" [ Data ]")]
    public List<UsedStruct> usedStruct = new List<UsedStruct>();
    public List<FishStruct> fishStruct = new List<FishStruct>();

    private void Awake()
    {
        Singleton_Data.INSTANCE.SetDictionary_Fish(fishStruct);
        Singleton_Data.INSTANCE.SetDictionary_Used(usedStruct);
        Singleton_Data.INSTANCE.SetDictionary_Audio(audioClip);
        Singleton_Data.INSTANCE.SetDictionary_Sprite(sprites);
    }
}