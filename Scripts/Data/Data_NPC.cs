using UnityEngine;

[CreateAssetMenu(fileName = "Data_NPC", menuName = "Scriptable Objects/Data_NPC")]
public class Data_NPC : ScriptableObject
{
    public string npc_ID; // NPC의 고유 ID
    public Texture texture;
    public string themeMusic; // NPC의 테마 음악 파일 이름   
    public string voice;// NPC의 음성 파일 이름
    public Data_Dialog[] dataDialogs;

    public Vector2Int invenSize;
    public string[] fixedID;// 파는 물건
    [System.Serializable]
    public struct RandomItemChance
    {
        public string itemID;
        [Range(0f, 100f)]
        public float chance;
    }
    public RandomItemChance[] randomID;
}
