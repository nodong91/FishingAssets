using UnityEngine;
using static Data_Manager;

public class Trigger_Setting : MonoBehaviour
{
    public enum TriggerType
    {
        Fishing,// ³¬½Ã
        Landing,// ºÎµÎ
    }
    public TriggerType triggerType;

    public string id;
    public FishStruct fishStruct;
    public FishStruct.FishType fishType;
    public FishStruct.RandomSize randomSize;

    void Start()
    {
        RandomFish();
    }

    public void SetFish(string _id)
    {
        id = _id;
        RandomFish();
    }

    void RandomFish()
    {
        fishStruct = Singleton_Data.INSTANCE.Dict_Fish[id];
        fishType = fishStruct.fishType;
        randomSize = fishStruct.GetRandom();
    }

    public Sprite GetIconSprite
    {
        get { return fishStruct.itemStruct.icon; }
    }
}
