using UnityEngine;

public class Skill_Setting : MonoBehaviour
{
    // 낚시 관련
    public float constFishHealth;// 물고기 체력
    public float constFishPower;// 물고기 공격력
    public float constFishSpeed;// 물고기 이동 속도
    public float constFishCoolTime;// 물고기 공격 쿨타임
    public float constFishSpellTime;// 공격할 때 딜레이 시간
    public float constFishGroggyTime;// 방어 성공 시 그로기 시간
    public int constFishDefenseCount;// 공격시 입력 개수

    const float constCatchRadius = 0.2f;
    const float constCatchSpeed = 0.2f;
    const float constCatchPower = 1f;
    const float constCatchHealth = 1f;

    const float constShipSpeed = 0.2f;// 배의 이동 속도
    const float constMaxWeight = 1f;// 인벤토리 중량
    const float constMaxEnergy = 1f;// 연료통 크기
    const float constEfficient = 0.1f;// 에너지 효율
    const int constShipHealth = 1;// 배 체력

    const float constLuckFish = 1f;// 낚시 성공 시 한마리 더 낚을 확률 (낚시 시작할 때 정해지고 두마리 중 등급이 높은 물고기가 기준)
    const int constFishAmount = 1;// 낚시 횟수 증가
    const float constFishPrice = 1f;// 판매 물고기 가격 증가

    const float constBoosterSpeed = 0.5f;
    const float constBoosterValue = 1f;
    const float constCrashChance = 5f;

    const string _001 = "_001";// 강철 낚시줄
    const string _002 = "_002";// 강력한 
    const string _003 = "_003";// 재빠른 손놀림
    const string _004 = "_004";// 신들린 컨트롤
    const string _005 = "_005";// 욕심쟁이
    const string _006 = "_006";// 방랑자
    const string _007 = "_007";// 친환경
    const string _008 = "_008";// 인벤토리증가
    const string _009 = "_009";// 두꺼운 피부
    const string _010 = "_010";// 일타쌍어
    const string _011 = "_011";// 낚시 횟수 증가
    const string _012 = "_012";// 수완가
    const string _013 = "_013";// 창고확장
    const string _014 = "_014";// 낚시왕
    const string _015 = "_015";// 손쉬운 낚시
    const string _016 = "_016";// 부스터
    const string _017 = "_017";// 부스터 크기
    const string _018 = "_018";// 물고기 체력감소
    const string _019 = "_019";// 물고기 스피드감소
    const string _020 = "_020";// 물고기 공격력 감소
    const string _021 = "_021";// 물고기 스펠 속도감소
    const string _022 = "_022";// 물고기 쿨타임 증가
    const string _023 = "_023";// 물고기 그로기 타임 증가
    const string _024 = "_024";// 충돌방지 확률

    const string _100 = "_100";// 배 언락
    const string _101 = "_101";// 배 언락
    const string _102 = "_102";// 배 언락
    const string _103 = "_103";// 배 언락
    const string _104 = "_104";// 배 언락
    const string _105 = "_105";// 배 언락
    const string _106 = "_106";// 배 언락

    public void AddLevel(string _id, bool _add)
    {
        //Debug.LogWarning(_id);
        if (_id.Contains(_001)) Level_CatchRadius = _add ? Level_CatchRadius + 1 : Level_CatchRadius - 1;
        else if (_id.Contains(_002)) Level_CatchPower = _add ? Level_CatchPower + 1 : Level_CatchPower - 1;
        else if (_id.Contains(_003)) Level_CatchSpeed = _add ? Level_CatchSpeed + 1 : Level_CatchSpeed - 1;
        else if (_id.Contains(_004)) Level_ShipSpeed = _add ? Level_ShipSpeed + 1 : Level_ShipSpeed - 1;
        else if (_id.Contains(_005)) Level_MaxWeight = _add ? Level_MaxWeight + 1 : Level_MaxWeight - 1;
        else if (_id.Contains(_006)) Level_MaxEnergy = _add ? Level_MaxEnergy + 1 : Level_MaxEnergy - 1;
        else if (_id.Contains(_007)) Level_Efficient = _add ? Level_Efficient + 1 : Level_Efficient - 1;
        else if (_id.Contains(_008)) Level_MaxBoxSize = _add ? Level_MaxBoxSize + 1 : Level_MaxBoxSize - 1;
        else if (_id.Contains(_009)) Level_ShipHealth = _add ? Level_ShipHealth + 1 : Level_ShipHealth - 1;
        else if (_id.Contains(_010)) Level_LuckFish = _add ? Level_LuckFish + 1 : Level_LuckFish - 1;
        else if (_id.Contains(_011)) Level_FishAmount = _add ? Level_FishAmount + 1 : Level_FishAmount - 1;
        else if (_id.Contains(_012)) Level_FishPrice = _add ? Level_FishPrice + 1 : Level_FishPrice - 1;
        else if (_id.Contains(_013)) Level_StorageSize = _add ? Level_StorageSize + 1 : Level_StorageSize - 1;
        else if (_id.Contains(_014)) Level_FishDefenseChance = _add ? Level_FishDefenseChance + 1 : Level_FishDefenseChance - 1;
        else if (_id.Contains(_015)) Level_CatchMaxHealth = _add ? Level_CatchMaxHealth + 1 : Level_CatchMaxHealth - 1;
        else if (_id.Contains(_016)) Level_BusterSpeed = _add ? Level_BusterSpeed + 1 : Level_BusterSpeed - 1;
        else if (_id.Contains(_017)) Level_BusterValue = _add ? Level_BusterValue + 1 : Level_BusterValue - 1;
        else if (_id.Contains(_018)) Level_FishHealth = _add ? Level_FishHealth + 1 : Level_FishHealth - 1;
        else if (_id.Contains(_019)) Level_FishSpeed = _add ? Level_FishSpeed + 1 : Level_FishSpeed - 1;
        else if (_id.Contains(_020)) Level_FishPower = _add ? Level_FishPower + 1 : Level_FishPower - 1;
        else if (_id.Contains(_021)) Level_FishSpellTime = _add ? Level_FishSpellTime + 1 : Level_FishSpellTime - 1;
        else if (_id.Contains(_022)) Level_FishCoolTime = _add ? Level_FishCoolTime + 1 : Level_FishCoolTime - 1;
        else if (_id.Contains(_023)) Level_FishGroggyTime = _add ? Level_FishGroggyTime + 1 : Level_FishGroggyTime - 1;
        else if (_id.Contains(_024)) Level_CrashChance = _add ? Level_CrashChance + 1 : Level_CrashChance - 1;

        else if (_id.Contains(_100)) Level_Ship_00 = _add ? Level_Ship_00 + 1 : Level_Ship_00 - 1;
        else if (_id.Contains(_101)) Level_Ship_01 = _add ? Level_Ship_01 + 1 : Level_Ship_01 - 1;
        else if (_id.Contains(_102)) Level_Ship_02 = _add ? Level_Ship_02 + 1 : Level_Ship_02 - 1;
        else if (_id.Contains(_103)) Level_Ship_03 = _add ? Level_Ship_03 + 1 : Level_Ship_03 - 1;
        else if (_id.Contains(_104)) Level_Ship_04 = _add ? Level_Ship_04 + 1 : Level_Ship_04 - 1;
        else if (_id.Contains(_105)) Level_Ship_05 = _add ? Level_Ship_05 + 1 : Level_Ship_05 - 1;
        else if (_id.Contains(_106)) Level_Ship_06 = _add ? Level_Ship_06 + 1 : Level_Ship_06 - 1;
        SetSkillType(_id);
    }

    public Data_Manager.SetStatus AddShipStatus()
    {
        Data_Manager.SetStatus setStatus = new Data_Manager.SetStatus
        {
            catchRadius = constCatchRadius * Level_CatchRadius,
            catchSpeed = constCatchSpeed * Level_CatchSpeed,
            catchPower = constCatchPower * Level_CatchPower,
            catchMaxHealth = constCatchHealth * Level_CatchMaxHealth,

            shipSpeed = constShipSpeed * Level_ShipSpeed,// 배의 이동 속도
            maxWeight = constMaxWeight * Level_MaxWeight,// 인벤토리 중량
            maxEnergy = constMaxEnergy * Level_MaxEnergy,// 연료통 크기
            efficient = constEfficient * Level_Efficient,// 에너지 효율
            maxBoxSize = GetSize(Level_MaxBoxSize),// 인벤토리 크기
            shipHealth = constShipHealth * Level_ShipHealth,// 배 체력

            luckFish = constLuckFish * Level_LuckFish,// 낚시 성공 시 한마리 더 낚을 확률 (낚시 시작할 때 정해지고 두마리 중 등급이 높은 물고기가 기준)
            fishAmount = constFishAmount * Level_FishAmount,// 낚시 횟수 증가
            fishPrice = constFishPrice * Level_FishPrice,// 판매 물고기 가격 증가

            storageSize = GetSize(Level_StorageSize),// 창고 크기
        };
        return setStatus;
    }

    Vector2Int GetSize(int _level)
    {
        Vector2Int temp = new(0, 0);
        for (int i = 0; i < _level; i++)
        {
            if (temp.x > temp.y)
                temp.y++;
            else
                temp.x++;
        }
        return temp;
    }
    //public Vector2 constFishTurnDelay;// 방향 바뀌는 딜레이 시간
    //[Header(" [ 버프 ]")]
    //public float constAddDuration;// 버프 시간
    //public float constAddValue;// 더해지는 버프 수치 - 버프수치가 높을 수록 해당 등급이 나올 확률이 올라간다.
    public Data_Manager.FishStatus AddFishStatus()
    {
        Data_Manager.FishStatus setStatus = new Data_Manager.FishStatus
        {
            //id = "",
            fishHealth = constFishHealth * Level_FishHealth,
            fishPower = constFishPower * Level_FishPower,
            fishSpeed = constFishSpeed * Level_FishSpeed,
            fishCoolTime = constFishCoolTime * Level_FishCoolTime,
            fishSpellTime = constFishSpellTime * Level_FishSpellTime,
            fishGroggyTime = constFishGroggyTime * Level_FishGroggyTime,
            fishDefenseCount = constFishDefenseCount * Level_FishDefenseChance,
            //fishTurnDelay = Vector2.zero,
            //addDuration = 0,
            //addValue = 0
        };
        return setStatus;
    }

    void SetSkillType(string _id)
    {
        if (Singleton_Data.INSTANCE.Dict_Skill.ContainsKey(_id) == false)
            return;

        Data_Manager.SkillStruct data = Singleton_Data.INSTANCE.Dict_Skill[_id];
        Debug.LogWarning($"{_id} ({data.skillType}): 활성화");
        switch (data.skillType)
        {
            case Data_Manager.SkillStruct.SkillType.AddStatus:
                Game_Manager.current.AddStatus();// 스탯 적용
                break;

            case Data_Manager.SkillStruct.SkillType.ShipUnlocked:
                ShipUnlock(_id);
                break;

            case Data_Manager.SkillStruct.SkillType.Etc:
                SetEtc();
                break;

            case Data_Manager.SkillStruct.SkillType.License:
                Level_License++;
                break;
        }
    }

    void ShipUnlock(string _id)
    {
        Data_Ship data_Ship = Singleton_Data.INSTANCE.Dict_Ship[_id];
        //Debug.LogWarning($"{_id} : {data_Ship.shipName}");
        Game_Manager.current.GetChangeShip.AddShip(data_Ship);
    }

    void SetEtc()
    {
        Game_Manager.current.SetEtc();
    }

    public void GetBooster(out float _boosterSpeed, out float _boosterValue)
    {
        _boosterSpeed = constBoosterSpeed * Level_BusterSpeed;
        _boosterValue = constBoosterValue * Level_BusterValue;
    }

    public float GetCrashChance()
    {
        return constCrashChance * Level_CrashChance;
    }

    public int GetLicenseLevel()
    {
        return Level_License;
    }

    public void ResetLevel()
    {
        Level_Ship_00 = 0;
        Level_Ship_01 = 0;
        Level_Ship_02 = 0;
        Level_Ship_03 = 0;
        Level_Ship_04 = 0;
        Level_Ship_05 = 0;
        Level_Ship_06 = 0;

        Level_CatchRadius = 0;
        Level_CatchSpeed = 0;// 낚시대가 물고기를 향해 이동하는 속도
        Level_CatchPower = 0;// 낚시대의 힘
        Level_CatchMaxHealth = 0;// 낚시대의 최대 체력

        Level_ShipSpeed = 0;// 배의 이동 속도
        Level_MaxWeight = 0;// 인벤토리 중량
        Level_MaxEnergy = 0;// 연료통 크기
        Level_Efficient = 0;// 에너지 효율
        Level_MaxBoxSize = 0;// 인벤토리 크기
        Level_ShipHealth = 0;// 배 체력
        Level_StorageSize = 0;// 창고 크기

        Level_LuckFish = 0;// 낚시 성공 시 한마리 더 낚을 확률 (낚시 시작할 때 정해지고 두마리 중 등급이 높은 물고기가 기준)
        Level_FishAmount = 0;// 낚시 횟수 증가
        Level_FishPrice = 0;// 판매 물고기 가격 증가

        Level_FishHealth = 0;
        Level_FishPower = 0;
        Level_FishSpeed = 0;
        Level_FishCoolTime = 0;
        Level_FishSpellTime = 0;
        Level_FishGroggyTime = 0;
        Level_FishDefenseChance = 0;

        Level_License = 0;
    }

    [Header(" [ Unlock Ship ]")]
    public int Level_Ship_00;// 기본배
    public int Level_Ship_01;// 쾌속선
    public int Level_Ship_02;// 땟목
    public int Level_Ship_03;// 크루즈
    public int Level_Ship_04;// 범선
    public int Level_Ship_05;// 통통배
    public int Level_Ship_06;// 오리배

    [Header(" [ Catch ]")]
    public int Level_CatchRadius;// 물고기를 잡는 범위
    public int Level_CatchSpeed;// 낚시대가 물고기를 향해 이동하는 속도
    public int Level_CatchPower;// 낚시대의 힘
    public int Level_CatchMaxHealth;// 낚시대의 최대 체력

    [Header(" [ Ship ]")]
    public int Level_ShipSpeed;// 배의 이동 속도
    public int Level_MaxWeight;// 인벤토리 중량
    public int Level_MaxEnergy;// 연료통 크기
    public int Level_Efficient;// 에너지 효율
    public int Level_MaxBoxSize;// 인벤토리 크기
    public int Level_ShipHealth;// 배 체력
    public int Level_StorageSize;// 창고 크기

    [Header(" [ Util ]")]
    public int Level_LuckFish;// 낚시 성공 시 한마리 더 낚을 확률 (낚시 시작할 때 정해지고 두마리 중 등급이 높은 물고기가 기준)
    public int Level_FishAmount;// 낚시 횟수 증가
    public int Level_FishPrice;// 판매 물고기 가격 증가

    [Header(" [ Fish ]")]
    public int Level_FishHealth;// 물고기 체력
    public int Level_FishPower;// 물고기 공격력
    public int Level_FishSpeed;//   물고기 이동 속도
    public int Level_FishCoolTime;// 물고기 공격 쿨타임
    public int Level_FishSpellTime;// 공격할 때 딜레이 시간
    public int Level_FishGroggyTime;// 방어 성공 시 그로기 시간
    public int Level_FishDefenseChance;// 공격시 입력 개수

    [Header(" [ Etc ]")]
    public int Level_BusterSpeed;// 부스터 속도
    public int Level_BusterValue;// 부스터 크기
    public int Level_CrashChance;// 충돌방지 확률

    [Header(" [ 자격증 ]")]
    public int Level_License;// 라이센스 레벨
}
