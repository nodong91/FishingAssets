using UnityEngine;
using static Data_Manager;

public class Fishing_Setting : MonoBehaviour
{
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
