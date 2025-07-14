using UnityEngine;
using static Data_Manager;

public class UI_Equip : MonoBehaviour
{
    public PartsStruct boatStruct;
    public PartsStruct engineStruct;
    public PartsStruct boxStruct;

    [Header("[ 스테이터스 ]")]
    public SetStatus totalStatus;
    public EquipStruct fishingStatus;// 낚시 관련 스탯
    public BaitStruct bait;// 미끼 (해당 물고기 )

    public void OpenCanvas()
    {

    }

    private void Start()
    {
        PartsStruct test = Singleton_Data.INSTANCE.Dict_Parts["Pb_0001"];
        AddEquip(test);
        test = Singleton_Data.INSTANCE.Dict_Parts["Pe_0001"];
        AddEquip(test);
        test = Singleton_Data.INSTANCE.Dict_Parts["Px_0001"];
        AddEquip(test);
    }

    void AddEquip(PartsStruct _struct)
    {
        switch (_struct.partsType)
        {
            case PartsStruct.PartsType.Body:
                boatStruct = _struct;
                break;

            case PartsStruct.PartsType.Engine:
                engineStruct = _struct;
                break;

            case PartsStruct.PartsType.Box:
                boxStruct = _struct;
                break;
        }
        AddStatus(boatStruct);
        AddStatus(engineStruct);
        AddStatus(boxStruct);
    }

    void AddStatus(PartsStruct _struct)
    {
        totalStatus.maxSpeed += _struct.addStatus.maxSpeed;
        totalStatus.maxWeight += _struct.addStatus.maxWeight;
        totalStatus.maxEnergy += _struct.addStatus.maxEnergy;
        totalStatus.maxBoxSize += _struct.addStatus.maxBoxSize;
        totalStatus.freshness += _struct.addStatus.freshness;
    }
}
