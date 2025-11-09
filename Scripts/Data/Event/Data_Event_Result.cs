using UnityEngine;

[CreateAssetMenu(fileName = "Data_Event_Result", menuName = "Scriptable Objects/Data_Event_Result")]
public class Data_Event_Result : Data_Event
{
    public enum ResultType
    {
        None,
        Reward,
        Shop,
    }
    public ResultType resultType;
    public string[] itemRewards;
}
