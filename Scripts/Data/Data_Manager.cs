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
        for (int i = 0; i < CSV_Data.Count; i++)
        {
            string csv_Type = CSV_Data[i].name;
            if (csv_Type.Contains("Fish"))
            {
                SetFish(CSV_Data[i]);
            }
            else if (csv_Type.Contains("Used"))
            {
                SetUsed(CSV_Data[i]);
            }
            else if (csv_Type.Contains("Language"))
            {
                SetLanguageStruct(CSV_Data[i]);
            }
            else if (csv_Type.Contains("Audio"))
            {
                SetAudioStruct(CSV_Data[i]);
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
            FishStruct tempData = new FishStruct
            {
                id = tempItem.id,
                itemStruct = tempItem,
                areaType = (AreaType)System.Enum.Parse(typeof(AreaType), elements[9]),
                fishDayType = (DayType)System.Enum.Parse(typeof(DayType), elements[10]),
                size = Parse_Vector2(elements[11]),
                fishHealth = Parse_Float(elements[12]),
                fishPower = Parse_Float(elements[13]),
                fishSpeed = Parse_Float(elements[14]),
                fishCoolTime = Parse_Float(elements[15]),
                fishAttackSpeed = Parse_Float(elements[16]),
                fishSpellTime = Parse_Float(elements[17]),
                fishGroggyTime = Parse_Float(elements[18]),
                fishDefenseCount = Parse_Int(elements[19]),
                fishTurnDelay = Parse_Vector2(elements[20]),
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
            UsedStruct tempData = new UsedStruct
            {
                id = tempItem.id,
                itemStruct = tempItem,
                value = Parse_Float(elements[9])
            };
            usedStruct.Add(tempData);
        }
    }

    ItemStruct GetItemStruct(string[] _elements)
    {
        Vector2Int[] tempShape = Parse_Vector2IntArray(_elements[5].Trim());// 모양
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
            itemType = (ItemStruct.ItemType)System.Enum.Parse(typeof(ItemStruct.ItemType), _elements[8]),
        };
        return tempItem;
    }

    void SetLanguageStruct(TextAsset _textAsset)
    {
        languageStruct.Clear();
        string[] data = _textAsset.text.Split(new char[] { '\n' });
        for (int i = 1; i < data.Length; i++)// 첫째 라인 빼고 리스팅
        {
            string[] elements = data[i].Split(new char[] { ',' });
            LanguageStruct tempData = new LanguageStruct
            {
                id = elements[0].Trim(),
                english = elements[1],
                korean = elements[2],
                japanese = elements[3],
                chinese = elements[4],
            };
            languageStruct.Add(tempData);
        }
    }

    void SetAudioStruct(TextAsset _textAsset)
    {
        audioStruct.Clear();
        string[] data = _textAsset.text.Split(new char[] { '\n' });
        for (int i = 1; i < data.Length; i++)// 첫째 라인 빼고 리스팅
        {
            string[] elements = data[i].Split(new char[] { ',' });
            AudioStruct tempData = new AudioStruct
            {
                id = elements[0].Trim(),
                clip = Parse_AudioClip(elements[1]),
                type = (AudioStruct.AudioType)System.Enum.Parse(typeof(AudioStruct.AudioType), elements[2]),
            };
            audioStruct.Add(tempData);
        }
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

        float centerX = (1f - (minX + maxX) * (1f / x)) * 0.5f;
        float centerY = (1f + (minY + maxY) * (1f / y)) * 0.5f;

        Vector4 temp = new Vector4(x, y, centerX, centerY);
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

        public int health;
        public float energy;
        public float money;
        public List<Vector2Int> destroySlot;
    }

    [System.Serializable]
    public struct Data_Option
    {
        public int language;
        // 그래픽 관련
        public bool fullScreen;
        public int qualityLevel;
        public int resolutionIndex;
        public int frameRateIndex;

        [System.Serializable]
        public struct AudioStruct
        {
            // 사운드 관련
            public bool masterMute;
            public float masterVolume;
            public bool bgmMute;
            public float bgmVolume;
            public bool fxMute;
            public float fxVolume;
            public bool envMute;
            public float envVolume;
        }
        public AudioStruct audioStruct;

        public void DefaultOption()
        {
            fullScreen = true;
            qualityLevel = 2;
            resolutionIndex = 6;
            frameRateIndex = 2;
            audioStruct = new AudioStruct
            {
                masterMute = false,
                masterVolume = 1f,
                bgmMute = false,
                bgmVolume = 1f,
                fxMute = false,
                fxVolume = 1f,
                envMute = false,
                envVolume = 1f,
            };
        }
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
        public float efficient;// 에너지 효율
        public Vector2Int maxBoxSize;// 인벤토리 크기
        public int shipHealth;// 배 체력
        public float freshness;// 신선도 유지 - 꼭 필요한가??????  

        [Header(" [ Fish ]")]
        public float LuckFish;// 희귀 물고기 확률
        public int FishAmount;// 낚시 횟수 증가
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
            efficient = _status.efficient;
            maxBoxSize = _status.maxBoxSize;
            shipHealth = _status.shipHealth;
            freshness = _status.freshness;
            LuckFish = _status.LuckFish;
            FishAmount = _status.FishAmount;
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
            efficient += _status.efficient * _remove;
            maxBoxSize += _status.maxBoxSize * _remove;
            shipHealth += _status.shipHealth * _remove;
            freshness += _status.freshness * _remove;
            LuckFish += _status.LuckFish * _remove;
            FishAmount += _status.FishAmount * _remove;
            FishPrice += _status.FishPrice * _remove;
        }
    }

    [System.Serializable]
    public class SkillStatus
    {
        public string name;
        [TextArea]
        public string description;
        public string addStatusString;
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
        public float value;// 효과 수치
    }

    [System.Serializable]
    public struct ItemStruct
    {
        public string id;
        public string name;
        public enum ItemType
        {
            Fish,
            Fuel,
            Repare,
            Quest,
            Lottery,
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

    public enum AreaType
    {
        None = 0,
        Shallow = 1,
        Coastal = 2,
        Oceanic = 3,
        Abyssal = 4,
        Hadal = 5,
    }

    public enum DayType
    {
        Any,
        Day,
        Night,
    }

    [System.Serializable]
    public struct FishStruct// 물고기 정보
    {
        [HideInInspector]
        public string id;
        public ItemStruct itemStruct;
        public AreaType areaType;
        public DayType fishDayType;// 등장 시간
        public Vector2 size;

        // 낚시 관련
        public float fishHealth;// 물고기 체력
        public float fishPower;// 물고기 공격력
        public float fishSpeed;// 물고기 이동 속도
        public float fishCoolTime;// 물고기 공격 쿨타임
        public float fishAttackSpeed;// 물고기 공격 속도
        public float fishSpellTime;// 공격할 때 딜레이 시간
        public float fishGroggyTime;// 방어 성공 시 그로기 시간
        public int fishDefenseCount;// 공격시 입력 개수
        public Vector2 fishTurnDelay;// 방향 바뀌는 딜레이 시간

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

    [System.Serializable]
    public struct LanguageStruct
    {
        public string id;
        [TextArea] public string english;
        [TextArea] public string korean;
        [TextArea] public string japanese;
        [TextArea] public string chinese;
    }

    [System.Serializable]
    public struct AudioStruct
    {
        public string id;
        public AudioClip clip;
        public enum AudioType
        {
            BGM = 0,
            FX = 1,
            Environment=2,
        }
        public AudioType type;
    }

    //==================================================================================
    // Data
    //==================================================================================

    [Header(" [ Data ]")]
    public List<UsedStruct> usedStruct = new List<UsedStruct>();
    public List<FishStruct> fishStruct = new List<FishStruct>();
    public List<LanguageStruct> languageStruct = new List<LanguageStruct>();
    public List<AudioStruct> audioStruct = new List<AudioStruct>();

    private void Awake()
    {
        Singleton_Data.INSTANCE.SetDictionary_Fish(fishStruct);
        Singleton_Data.INSTANCE.SetDictionary_Used(usedStruct);
        Singleton_Data.INSTANCE.SetDictionary_Language(languageStruct);
        Singleton_Data.INSTANCE.SetDictionary_Audio(audioStruct);
        Singleton_Data.INSTANCE.SetDictionary_Sprite(sprites);
    }
}