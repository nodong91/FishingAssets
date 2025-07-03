using UnityEngine;
using static Data_Manager;

public class Fishing_Setting : MonoBehaviour
{
    void Start()
    {
        RandomFish();
    }

    public FishStruct.RandomSize randomSize;
    void RandomFish()
    {
        FishStruct fishStruct = Singleton_Data.INSTANCE.Dict_Fish["F_0001"];
        randomSize = fishStruct.GetRandom();
    }


}
