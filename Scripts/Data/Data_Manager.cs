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
            else if (csv_Type.Contains("Dialog"))
            {
                SetDialogData(GetCSV_Data[i]);
            }
            else if (csv_Type.Contains("Skill"))
            {
                SetSkill(GetCSV_Data[i]);
            }
            else if (csv_Type.Contains("Unit"))
            {
                SetUnit(GetCSV_Data[i]);
            }
            else if (csv_Type.Contains("Item"))
            {
                SetItem(GetCSV_Data[i]);
            }

        }
    }

    void SetDialogData(TextAsset _textAsset)
    {
        dialogStruct.Clear();
        string[] data = _textAsset.text.Split(new char[] { '\n' });
        for (int i = 1; i < data.Length; i++)// 첫째 라인(목록) 빼고 리스팅
        {
            string[] elements = data[i].Split(new char[] { ',' });
            if (elements[0].Trim().Length == 0)// 아이디 표기가 없으면 제외
                continue;

            DialogStruct tempData = new DialogStruct
            {
                ID = elements[0].Trim(),
                textStyle = elements[1].Trim().Length > 0 ?
            (Data_DialogType.TextStyle)Enum.Parse(typeof(Data_DialogType.TextStyle), elements[1]) : 0,
                size = IntTryParse(elements[2]),
                color = elements[3],
                bold = elements[4] == "TRUE" ? true : false,
                speed = FloatTryParse(elements[5].Trim()),

                //KR = elements[6],
                //EN = elements[7],
                //JP = elements[8],
                //CN = elements[9],
            };
            dialogStruct.Add(tempData);
        }
    }

    void SetSkill(TextAsset _textAsset)
    {
        skillStruct.Clear();
        string[] data = _textAsset.text.Split(new char[] { '\n' });
        for (int i = 1; i < data.Length; i++)// 첫째 라인(목록) 빼고 리스팅
        {
            string[] elements = data[i].Split(new char[] { ',' });
            if (elements[0].Trim().Length == 0)// 아이디 표기가 없으면 제외
                continue;

            SkillStruct tempData = new SkillStruct
            {
                ID = elements[0].Trim(),
                skillName = elements[1],
                skillDescription = elements[2],
                //animationType = (Unit_Animation.AnimationType)Enum.Parse(typeof(Unit_Animation.AnimationType), elements[3]),
                icon = FindSprite(elements[4].Trim()),
                level = IntTryParse(elements[5]),
                skillType = (SkillStruct.SkillType)Enum.Parse(typeof(SkillStruct.SkillType), elements[6]),// 
                ccType = (SkillStruct.CCType)Enum.Parse(typeof(SkillStruct.CCType), elements[7]),// 
                energyType = (SkillStruct.EnergyType)Enum.Parse(typeof(SkillStruct.EnergyType), elements[8]),// 
                energyAmount = FloatTryParse(elements[9]),
                castingTime = FloatTryParse(elements[10]),// 0일 경우 즉시시전
                coolingTime = FloatTryParse(elements[11]),
                range = Parse_Vector2(elements[12]),
                influence = Parse_Vector3(elements[13]),
                aggro = FloatTryParse(elements[14]),
                skillSet = elements[15].Trim(),
                splashRange = FloatTryParse(elements[16]),
                splashAngle = FloatTryParse(elements[17]),
                projectileSpeed = FloatTryParse(elements[18]),
            };
            skillStruct.Add(tempData);
        }
    }

    void SetUnit(TextAsset _textAsset)
    {
        unitStruct.Clear();
        string[] data = _textAsset.text.Split(new char[] { '\n' });
        for (int i = 1; i < data.Length; i++)// 첫째 라인 빼고 리스팅
        {
            string[] elements = data[i].Split(new char[] { ',' });

            UnitStruct tempData = new UnitStruct
            {
                ID = elements[0].Trim(),
                //unitProp = FindUnit(elements[0].Trim()),
                unitName = elements[1].Trim(),
                unitDescription = elements[2].Trim(),
                unitIcon = FindSprite(elements[3].Trim()),
                synergy = Parse_Vector2Int(elements[4].Trim()),// 시너지
                unitType = (UnitStruct.UnitType)Enum.Parse(typeof(UnitStruct.UnitType), elements[5]),
                unitAttribute = (UnitStruct.UnitAttribute)Enum.Parse(typeof(UnitStruct.UnitAttribute), elements[6]),

                unitSize = FloatTryParse(elements[7].Trim()),
                defaultSkill01 = elements[8].Trim(),
                defaultSkill02 = elements[9].Trim(),
                // 능력치
                strength = FloatTryParse(elements[10]),
                agility = FloatTryParse(elements[11]),
                intelligence = FloatTryParse(elements[12]),
                constitution = FloatTryParse(elements[13]),
            };
            unitStruct.Add(tempData);
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
                itemName = elements[1].Trim(),
                itemDescription = elements[2].Trim(),
                itemIcon = FindSprite(elements[3].Trim()),
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
        public SynergyType synergyType;

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
        public string itemName;
        [TextArea]
        public string itemDescription;
        public Sprite itemIcon;
        public SynergyType synergyType;
    }
    public List<ItemStruct> itemStruct = new List<ItemStruct>();

    [System.Serializable]
    public struct UnitStruct
    {
        public string ID;
        public string unitName;
        public string unitDescription;// 설명
        public Sprite unitIcon;
        //public Unit_Animation unitProp;// 아이디와 동일
        public Vector2Int[] synergy;
        public enum UnitType
        {
            Beast,
            Dragon,
            Undead,
            Plant,
            Elemental,
            Machine,
            Devil
        }
        public UnitType unitType;
        public enum UnitAttribute
        {
            Fire,
            Water,
            Earth
        }
        public UnitAttribute unitAttribute;

        public float unitSize;
        public string defaultSkill01;
        public string defaultSkill02;

        //General
        public float strength;
        public float agility;
        public float intelligence;
        public float constitution;
        [System.Serializable]
        public struct UnitStatus
        {
            public float Health;// 체력
            public float Mana;// 마나
            public float Defense;// 방어
            public float AttackPower;// 힘 공격력
            public float SpellPower;// 지능 공격력
            public float RangePower;// 원거리 공격력
            public float MoveSpeed;
        }

        public UnitStatus TryStatus()
        {
            UnitStatus status = new UnitStatus
            {
                Health = constitution * 10f,
                Mana = intelligence,
                Defense = strength,
                AttackPower = strength,
                SpellPower = intelligence,
                RangePower = agility,
                MoveSpeed = agility,
            };
            return status;
        }
    }
    public List<UnitStruct> unitStruct = new List<UnitStruct>();

    private void Awake()
    {
        Singleton_Data.INSTANCE.SetDictionary_Dialog(dialogStruct);
        Singleton_Data.INSTANCE.SetDictionary_DialogString(dialogString);

        Singleton_Data.INSTANCE.SetDictionary_Skill(skillStruct);
        Singleton_Data.INSTANCE.SetDictionary_Unit(unitStruct);
        Singleton_Data.INSTANCE.SetDictionary_Item(itemStruct);
        Singleton_Data.INSTANCE.SetDictionary_TranslationString(translateString);

        Singleton_Data.INSTANCE.SetDictionary_Audio(audioClip);
        //Singleton_Data.INSTANCE.SetSkillSet(skillSet);
    }
}

public enum ItemType
{
    Empty,
    Item,
    Skill,
    Unit
}
[System.Serializable]
public class SynergyType
{
    public ItemType itemType;
    // 스탯 변경 (공격력, 공격 속도, 방어력 등등)
    // 스킬 속성 변경? 단일 공격에서 광역 공격으로?
    // 아이템 이름에 특수한 단어가 들어갈 경우 효과 적용 (Axe, Potion)
    // 스킬 속성은 유닛 속성을 따라 간다
    public float helthPoint;// 체력 추가 %
    public string keyCode;// 단어가 포함되는 경우
    public Data_Manager.SkillStruct.SkillType skillType;
}