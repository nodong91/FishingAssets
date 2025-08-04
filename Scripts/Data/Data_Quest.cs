using UnityEngine;

[CreateAssetMenu(fileName = "Data_Quest", menuName = "Scriptable Objects/Data_Quest")]
public class Data_Quest : ScriptableObject
{
    public string title;
    [TextArea]
    public string description;
    public string npc_ID;
    public int deadLine;
    public string[] resultID;

    //public int GetLasDay()
    //{

    //}
}
