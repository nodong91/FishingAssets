using UnityEngine;
using static Data_Manager;

public class Trigger_Fish : Trigger_Setting
{
    public string id;
    public FishStruct fishStruct;
    public FishStruct.FishType GetFishType { get { return fishStruct.fishType; } }
    public FishStruct.RandomSize randomSize;

    public override Sprite GetIconSprite
    {
        get { return fishStruct.itemStruct.icon; }
    }

    void Start()
    {
        SetFish(id);
    }

    public void SetFish(string _id)
    {
        id = _id;
        RandomFish();
    }

    void RandomFish()
    {
        fishStruct = Singleton_Data.INSTANCE.Dict_Fish[id];
        randomSize = fishStruct.GetRandom();
    }
}
