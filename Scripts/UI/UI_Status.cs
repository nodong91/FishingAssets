using UnityEngine;
using UnityEngine.EventSystems;
using static Data_Manager;
using static UI_Main;

public class UI_Status : MonoBehaviour, IPointerClickHandler
{
    public CanvasStruct[] canvasStructs;

    public PartsStruct boatStruct;
    public PartsStruct engineStruct;
    public PartsStruct boxStruct;
    public EquipStruct equipStruct;// 낚시 관련 스탯
    public BaitStruct bait;// 미끼 (해당 물고기 )

    [Header("[ 스테이터스 ]")]
    public SetStatus totalStatus;
    public TMPro.TMP_Text maxSpeedText, maxWeightText, maxEnergyText, maxBoxSizeText, freshnessText;
    public TMPro.TMP_Text fishingAreaText, LodPowerText, ReelingSpeedText, ReelingAccText, HitPointText, HitSpeedText;

    private void Start()
    {
        PartsStruct test = Singleton_Data.INSTANCE.Dict_Parts["Pb_0001"];
        AddParts(test);
        test = Singleton_Data.INSTANCE.Dict_Parts["Pe_0001"];
        AddParts(test);
        test = Singleton_Data.INSTANCE.Dict_Parts["Px_0001"];
        AddParts(test);

        OpenCanvas(false);
    }

    public void OpenCanvas(bool _open)
    {
        StartCoroutine(OpenCanvasMoving(canvasStructs, _open, 10f));
    }

    public void AddParts(PartsStruct _struct)
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
        SetStatus(boatStruct);
        SetStatus(engineStruct);
        SetStatus(boxStruct);

        SetStutusText();
    }

    void SetStatus(PartsStruct _struct)
    {
        totalStatus.maxSpeed += _struct.addStatus.maxSpeed;
        totalStatus.maxWeight += _struct.addStatus.maxWeight;
        totalStatus.maxEnergy += _struct.addStatus.maxEnergy;
        totalStatus.maxBoxSize += _struct.addStatus.maxBoxSize;
        totalStatus.freshness += _struct.addStatus.freshness;
    }

    void SetStutusText()
    {
        maxSpeedText.text = totalStatus.maxSpeed.ToString();
        maxWeightText.text = totalStatus.maxWeight.ToString();
        maxEnergyText.text = totalStatus.maxEnergy.ToString();
        maxBoxSizeText.text = totalStatus.maxBoxSize.ToString();
        freshnessText.text = totalStatus.freshness.ToString();
    }

    public void AddEquip(string _id)
    {
        equipStruct = Singleton_Data.INSTANCE.Dict_Equip[_id];
        SetLodText();
    }

    void SetLodText()
    {
        fishingAreaText.text = equipStruct.fishingArea.ToString();
        LodPowerText.text = equipStruct.lodPower.ToString();
        ReelingSpeedText.text = equipStruct.reelingSpeed.ToString();
        ReelingAccText.text = equipStruct.reelingAcceleration.ToString();
        HitPointText.text = equipStruct.hitPoint.ToString();
        HitSpeedText.text = equipStruct.hitSpeed.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OpenCanvas(false);
    }
}
