using UnityEngine;
using static Data_Manager;

public class Trigger_Fish : MonoBehaviour
{
    public Trigger_Setting triggerSetting;
    public string id;
    public FishStruct fishStruct;
    //public FishStruct.FishType GetFishType { get { return fishStruct.fishType; } }
    //public FishStruct.RandomSize randomSize;

    //public override Sprite GetIconSprite
    //{
    //    get { return fishStruct.itemStruct.icon; }
    //}

    void Start()
    {
        SetFish(id);
        triggerSetting.deleTriggerAction = FishingStart;
        triggerSetting.GetIconSprite = fishStruct.itemStruct.icon;
    }

    public void SetFish(string _id)
    {
        id = _id;
        fishStruct = Singleton_Data.INSTANCE.Dict_Fish[id];
        //randomSize = fishStruct.GetRandom();
    }

    void FishingStart()
    {
        Game_Manager.current.fishingManager.StartGame(fishStruct);
    }
}
