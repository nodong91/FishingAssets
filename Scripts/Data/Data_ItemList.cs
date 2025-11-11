using UnityEngine;

[CreateAssetMenu(fileName = "Data_ItemList", menuName = "Scriptable Objects/Data_ItemList")]
public class Data_ItemList : ScriptableObject
{
    [System.Serializable]
    public struct ItemIDStruct
    {
        public string itemID;
        [Range(0f, 100f)]
        public float chance;
    }
    public ItemIDStruct[] itemIDs;

    public ItemIDStruct GetItemID()
    {
        float totalChance = 0;
        foreach (var item in itemIDs)
        {
            totalChance += item.chance;
        }
        float randomValue = Random.Range(1, totalChance + 1);
        float cumulativeChance = 0;
        foreach (var item in itemIDs)
        {
            cumulativeChance += item.chance;
            if (randomValue <= cumulativeChance)
            {
                return item;
            }
        }
        return itemIDs[0]; // Fallback, should not reach here
    }
}
