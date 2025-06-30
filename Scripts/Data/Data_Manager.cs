using UnityEngine;
using System.Collections.Generic;
using System;

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

            ItemStruct tempItem = new ItemStruct
            {
                ID = elements[0].Trim(),
                Name = elements[1].Trim(),
                Explanation = elements[2].Trim(),
                Icon = FindSprite(elements[3].Trim()),
                Size = Parse_Vector2Int(elements[4].Trim()),
                Weight = Parse_Float(elements[5].Trim()),
                Price = Parse_Float(elements[6].Trim()),
            };

            FishStruct tempData = new FishStruct
            {
                itemStruct = tempItem,
                fishType = (FishStruct.FishType)Enum.Parse(typeof(FishStruct.FishType), elements[7]),
            };
            fishStruct.Add(tempData);
        }
    }

    void SetItem(TextAsset _textAsset)
    {
        itemStruct.Clear();
        string[] data = _textAsset.text.Split(new char[] { '\n' });
        for (int i = 1; i < data.Length; i++)// 첫째 라인 빼고 리스팅
        {
            string[] elements = data[i].Split(new char[] { ',' });

            ItemStruct tempData = new ItemStruct
            {
                ID = elements[0].Trim(),
                Name = elements[1].Trim(),
                Explanation = elements[2].Trim(),
                Icon = FindSprite(elements[3].Trim()),
            };
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

    [System.Serializable]
    public struct SkillStruct
    {
        public string ID;
        public string skillName;
        [TextArea]
        public string skillDescription;
        //public Unit_Animation.AnimationType animationType;
        public Sprite icon;
        public int level;

        public enum SkillType
        {
            Attack,
            Defense,
            Heal,
        }
        public SkillType skillType;
        public enum CCType
        {
            Normal,
            KnockBack,
        }
        public CCType ccType;
        public enum EnergyType
        {
            Mana,
            Rage,
            Stamina
        }
        public EnergyType energyType;// 기본 에너지의 몇%
        public float energyAmount;
        public float castingTime;// 0일 경우 즉시시전
        public float coolingTime;
        public Vector2 range;
        public Vector3 influence;// 힘,민,지 영향
        public float aggro;
        public string skillSet;
        public float splashRange;
        public float splashAngle;// 타격 했을 때 각도
        public float projectileSpeed;// 탄이 있을 때 탄 속도

        public float GetDamage(float _ap, float _sp, float _rp)
        {
            float damage = _ap * influence.x + _sp * influence.y + _rp * influence.z;
            return damage;
        }
    }
    [Header(" [ Data ]")]
    public List<SkillStruct> skillStruct = new List<SkillStruct>();
    [System.Serializable]
    public struct ItemStruct
    {
        public string ID;
        public string Name;
        [TextArea]
        public string Explanation;// 설명
        public Sprite Icon;

        public Vector2Int[] Size;
        public float Weight;
        public float Price;
    }
    public List<ItemStruct> itemStruct = new List<ItemStruct>();

    [System.Serializable]
    public struct FishStruct
    {
        public ItemStruct itemStruct;
        //public string ID;
        //public string Name;
        //public string Explanation;// 설명
        //public Sprite Icon;
        public enum FishType
        {
            Strength,
            Agility,
            Health,
        }
        public FishType fishType;
    }
    public List<FishStruct> fishStruct = new List<FishStruct>();

    private void Awake()
    {
        Singleton_Data.INSTANCE.SetDictionary_Fish(fishStruct);
    }
}