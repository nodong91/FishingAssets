using UnityEngine;
using System.Collections.Generic;
using static Data_Manager.PartsStruct;


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
        translateString = new List<TranslateString>();
        dialogString = new List<TranslateString>();
        for (int i = 0; i < GetCSV_Data.Count; i++)
        {
            string csv_Type = GetCSV_Data[i].name;
            if (csv_Type.Contains("Translate"))
            {
                if (csv_Type.Contains("Dialog"))
                {
                    dialogString = SetTranslateString(dialogString, GetCSV_Data[i]);
                }
                else
                {
                    translateString = SetTranslateString(translateString, GetCSV_Data[i]);
                }
            }
            else if (csv_Type.Contains("Fish"))
            {
                SetFish(GetCSV_Data[i]);
            }
            else if (csv_Type.Contains("Parts"))
            {
                SetParts(GetCSV_Data[i]);
            }
            else if (csv_Type.Contains("Equip"))
            {
                SetEquip(GetCSV_Data[i]);
            }
            else if (csv_Type.Contains("Item"))
            {
                SetItem(GetCSV_Data[i]);
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
                fishType = (FishStruct.FishType)System.Enum.Parse(typeof(FishStruct.FishType), elements[7]),
                size = Parse_Vector2(elements[8]),
                fishStamina = Parse_Float(elements[9]),
                fishPower = Parse_Float(elements[10]),
                fishROA = Parse_Float(elements[11]),
                fishSpeed = Parse_Float(elements[12]),
                fishTurnDelay = Parse_Vector2(elements[13]),
                hitValue = Parse_Vector2(elements[14]),
            };
            fishStruct.Add(tempData);
        }
    }

    void SetEquip(TextAsset _textAsset)
    {
        equipStruct.Clear();
        string[] data = _textAsset.text.Split(new char[] { '\n' });
        for (int i = 1; i < data.Length; i++)// 첫째 라인 빼고 리스팅
        {
            string[] elements = data[i].Split(new char[] { ',' });
            ItemStruct tempItem = GetItemStruct(elements);
            tempItem.itemType = ItemStruct.ItemType.Equip;// 타입 세팅
            EquipStruct tempData = new EquipStruct
            {
                id = tempItem.id,
                itemStruct = tempItem,
                fishingArea = Parse_Float(elements[7]),
                lodPower = Parse_Float(elements[8]),
                reelingSpeed = Parse_Float(elements[9]),
                reelingAcceleration = Parse_Float(elements[10]),
                hitPoint = Parse_Float(elements[11]),
                hitSpeed = Parse_Float(elements[12]),
            };
            equipStruct.Add(tempData);
        }
    }
    ItemStruct GetItemStruct(string[] _elements)
    {
        ItemStruct tempItem = new ItemStruct
        {
            id = _elements[0].Trim(),
            name = _elements[1],
            explanation = _elements[2],
            icon = FindSprite(_elements[3]),
            shape = Parse_Vector2IntArray(_elements[4].Trim()),
            weight = Parse_Float(_elements[5]),
            price = Parse_Float(_elements[6]),
        };
        return tempItem;
    }

    void SetParts(TextAsset _textAsset)
    {
        partsStruct.Clear();
        string[] data = _textAsset.text.Split(new char[] { '\n' });
        for (int i = 1; i < data.Length; i++)// 첫째 라인 빼고 리스팅
        {
            string[] elements = data[i].Split(new char[] { ',' });
            PartsStruct tempData = new PartsStruct
            {
                id = elements[0].Trim(),
                name = elements[1],
                partsType = GetPartsType(elements[0].Trim()),
                explanation = elements[2],
                icon = FindSprite(elements[3]),
                price = Parse_Float(elements[4]),
                addStatus = new SetStatus
                {
                    maxSpeed = Parse_Float(elements[5]),
                    maxWeight = Parse_Float(elements[6]),
                    maxEnergy = Parse_Float(elements[7]),
                    maxBoxSize = Parse_Vector2Int(elements[8]),
                    freshness = Parse_Float(elements[9]),
                },
            };
            partsStruct.Add(tempData);
        }
    }

    PartsType GetPartsType(string _id)
    {
        if (_id.Contains("Pb"))
        {
            return PartsType.Body;
        }
        else if (_id.Contains("Pe"))
        {
            return PartsType.Engine;
        }
        return PartsType.Box;
    }

    public PartsType partsType;
    [TextArea]
    public string explanation;// 설명
    public Sprite icon;
    public float price;
    public SetStatus addStatus;
    void SetItem(TextAsset _textAsset)
    {
        itemStruct.Clear();
        string[] data = _textAsset.text.Split(new char[] { '\n' });
        for (int i = 1; i < data.Length; i++)// 첫째 라인 빼고 리스팅
        {
            string[] elements = data[i].Split(new char[] { ',' });

            ItemStruct tempData = GetItemStruct(elements);
            itemStruct.Add(tempData);
        }
    }

    List<TranslateString> SetTranslateString(List<TranslateString> _tempString, TextAsset _textAsset)
    {
        string[] data = _textAsset.text.Split(new char[] { '\n' });
        for (int i = 1; i < data.Length; i++)// 첫째 라인(목록) 빼고 리스팅
        {
            string[] elements = data[i].Split(new char[] { ',' });
            if (elements[0].Trim().Length == 0)// 아이디 표기가 없으면 제외
                continue;

            TranslateString tempData = new TranslateString
            {
                ID = elements[0].Trim(),
                KR = elements[1],
                EN = elements[2],
                JP = elements[3],
                CN = elements[4],
            };
            _tempString.Add(tempData);
        }
        return _tempString;
    }
#endif

    [System.Serializable]
    public class DialogStruct
    {
        public string ID;
        public string color;
        public int size;
        public bool bold;
        public Data_DialogType.TextStyle textStyle;
        public float speed;
    }
    [Header(" [ String ]")]
    public List<DialogStruct> dialogStruct = new List<DialogStruct>();

    [System.Serializable]
    public class TranslateString
    {
        public string ID;
        public string KR;
        public string EN;
        public string JP;
        public string CN;
    }
    public List<TranslateString> dialogString = new List<TranslateString>();
    public List<TranslateString> translateString = new List<TranslateString>();



















    [Header(" [ Data ]")]
    public List<EquipStruct> equipStruct = new List<EquipStruct>();
    public List<ItemStruct> itemStruct = new List<ItemStruct>();
    public List<FishStruct> fishStruct = new List<FishStruct>();
    public List<PartsStruct> partsStruct = new List<PartsStruct>();

    [System.Serializable]
    public class SetStatus
    {
        public float maxSpeed;// 속도
        public float maxWeight;// 인벤토리 중량
        public float maxEnergy;// 연료통 크기
        public Vector2Int maxBoxSize;// 인벤토리 크기
        public float freshness;// 신선도 유지
    }

    [System.Serializable]
    public struct PartsStruct
    {
        public string id;
        public string name;
        public enum PartsType
        {
            Body,
            Engine,
            Box
        }
        public PartsType partsType;
        [TextArea]
        public string explanation;// 설명
        public Sprite icon;
        public float price;
        public SetStatus addStatus;
    }

    [System.Serializable]
    public struct EquipStruct
    {
        [HideInInspector]
        public string id;
        public ItemStruct itemStruct;

        public float fishingArea;// 공격 영역
        public float lodPower;// 초당 끌려가는 힘 - 높을 수록 쉽게 끌려감
        public float reelingSpeed;// 낚시 회전 속도
        public float reelingAcceleration;// 릴링 가속도
        public float hitPoint;// 물고기 잡을 위치
        public float hitSpeed;// 물고기 찌 움직임
    }

    public struct BaitStruct
    {
        [HideInInspector]
        public string id;
        public ItemStruct itemStruct;
        public enum CatchType// 잡을 수 있는 어종
        {
            Coast,// 연안
            Shallow,// 얕은
            Ocean,// 대양
        }
        public CatchType catchType;
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
        }
        public ItemType itemType;
        [TextArea]
        public string explanation;// 설명
        public Sprite icon;

        public Vector2Int[] shape;
        public float weight;
        public float price;
    }

    [System.Serializable]
    public struct FishStruct
    {
        [HideInInspector]
        public string id;
        public ItemStruct itemStruct;
        public enum FishType
        {
            Strength,
            Agility,
            Health,
        }
        public FishType fishType;
        public Vector2 size;
        public float freshness;// 신선도

        // 낚시 관련
        public float fishStamina;
        public float fishPower;// 물고기 방어력
        public float fishROA;// 물고기 활동 범위 (다음 이동 각도) range of activity 
        public float fishSpeed;
        public Vector2 fishTurnDelay;// 방향 바뀌는 딜레이 시간
        public Vector2 hitValue; // 크리티컬 ; 히트 0~1

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

    private void Awake()
    {
        Singleton_Data.INSTANCE.SetDictionary_Fish(fishStruct);
        Singleton_Data.INSTANCE.SetDictionary_Parts(partsStruct);
        Singleton_Data.INSTANCE.SetDictionary_Equip(equipStruct);
    }
}