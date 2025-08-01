using UnityEngine;

[CreateAssetMenu(fileName = "Data_NPC", menuName = "Scriptable Objects/Data_NPC")]
public class Data_NPC : ScriptableObject
{
    public Texture texture;
    public Data_Dialog[] dataDialogs;
    public Data_Quest[] dataQuests;
}
