using UnityEngine;

[CreateAssetMenu(fileName = "Data_NPC", menuName = "Scriptable Objects/Data_NPC")]
public class Data_NPC : ScriptableObject
{
    public string npc_ID; // NPC의 고유 ID
    public Vector2Int openTime;
    public Texture[] npcTextures;
    public string themeMusic; // NPC의 테마 음악 파일 이름   
    public string voice;// NPC의 음성 파일 이름

    //[Header(" [ 판매 물품 ]")]
    //public Vector2Int invenSize;
    //public Data_ItemList saleItemList;
}
