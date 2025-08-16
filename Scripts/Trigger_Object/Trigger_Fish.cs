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
        id = _id;
        fishStruct = Singleton_Data.INSTANCE.Dict_Fish[id];
        //randomSize = fishStruct.GetRandom();
    }

    void FishingStart()
    {
        //string id = "Fs_1001";
        //Data_Manager.FishStruct fishStruct = Singleton_Data.INSTANCE.Dict_Fish[id];
        triggerSetting.gameObject.SetActive(false);// 트리거 오브젝트 비활성화
        Game_Manager.current.GetFishing.StartGame(fishStruct);
    }
}
