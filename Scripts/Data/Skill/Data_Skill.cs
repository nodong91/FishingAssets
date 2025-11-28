using UnityEngine;

[CreateAssetMenu(fileName = "Data_Skill", menuName = "Scriptable Objects/Data_Skill")]
public class Data_Skill : ScriptableObject
{
    public string skill_ID;
    public string skill_Name;
    [TextArea]
    public string skill_Description;
    public enum SkillType
    {
        AddStatus = 0,// 스탯 증가
        ShipUnlocked = 1,// 배 잠금 해제
        Etc = 2,// 기타
    }
    public SkillType skill_Type;
    public string skill_Icon;
    public int skill_Price; // 가격 정보

    [System.Serializable]
    public struct AddShipStatus
    {
        public enum AddType
        {
            None = 0,
            CatchRadius = 1,
            CatchSpeed = 2,
            CatchPower = 3,
            CatchMaxHealth = 4,

            ShipSpeed = 5,
            MaxWeight = 6,
            MaxEnergy = 7,
            Efficient = 8,
            MaxBoxSize = 9,
            ShipHealth = 10,

            LuckFish = 11,
            FishAmount = 12,
            FishPrice = 13,
            StorageSize = 14,
        }
        public AddType addType;
        public string addVaule;
    }
    public AddShipStatus[] addShipStatus;

    [System.Serializable]
    public struct AddFishStatus
    {
        public enum AddType
        {
            None = 0,
            FishHealth = 1,
            FishPower = 2,
            FishSpeed = 3,
            FishCoolTime = 4,
            FishSpellTime = 5,
            FishGroggyTime = 6,
            FishDefensePer = 7,
        }
        public AddType addType;
        public string addVaule;
    }
    public AddFishStatus[] addFishStatus;
}
