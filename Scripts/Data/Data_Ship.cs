using UnityEngine;
using static Data_Manager;

[CreateAssetMenu(fileName = "Data_Ship", menuName = "Scriptable Objects/Data_Ship")]
public class Data_Ship : ScriptableObject
{
    public string id;
    public string shipName;
    public GameObject shipObject;
    public SetStatus status;
}
