using UnityEngine;
using static Data_Manager;

public class Trigger_Fish : MonoBehaviour
{
    public Trigger_Setting triggerSetting;
    public string id;
    public FishStruct fishStruct;

    void Start()
    {
        SetFish(id);
        triggerSetting.deleTriggerAction = FishingStart;// 낚시 게임 스타트
        triggerSetting.GetIconSprite = fishStruct.itemStruct.icon;// 트리거 아이콘 설정
    }

    void SetFish(string _id)
    {
        if (fishingAmount <= 0)
            return;

        id = _id;
        fishStruct = Singleton_Data.INSTANCE.Dict_Fish[id];
        //randomSize = fishStruct.GetRandom();
    }

    public FishStruct[] fishStructs;
    public int fishingAmount = 2; // 낚시 횟수

    void FishingStart()
    {
        Game_Manager.current.GetFishing.SetFishingStart(fishStructs, fishingAmount);
        triggerSetting.gameObject.SetActive(false);// 트리거 오브젝트 비활성화
    }
}
