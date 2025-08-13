using UnityEngine;

[CreateAssetMenu(fileName = "Data_NPC", menuName = "Scriptable Objects/Data_NPC")]
public class Data_NPC : ScriptableObject
{
    public string npc_ID; // NPC의 고유 ID
    public Texture texture;
    public string themeMusic; // NPC의 테마 음악 파일 이름   
    public string voice;// NPC의 음성 파일 이름
    public Data_Dialog[] dataDialogs;
    public Data_Quest[] dataQuests;
}
