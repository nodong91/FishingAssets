using UnityEngine;

[CreateAssetMenu(fileName = "Data_ItemList", menuName = "Scriptable Objects/Data_ItemList")]
public class Data_ItemList : ScriptableObject
{
    public enum InventoryType
    {
        None,
        Shop,
        Shipyard,
        Smuggler,
        Loan,
    }
    public InventoryType inventoryType;

    [System.Serializable]
    public struct ItemIDStruct
    {
        public string itemID;
        [Range(0.1f, 100f)]
        public float chance;
    }
    public ItemIDStruct[] itemIDs;

    public string GetItemID()
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
                return item.itemID;
            }
        }
        return itemIDs[0].itemID; // Fallback, should not reach here
    }
    // È®·ü ±¸ÇÏ±â

    //public string GetItemID()
    //{
    //    float total = 0;
    //    foreach (var elem in itemIDs)
    //    {
    //        total += elem.chance;
    //    }

    //    float randomPoint = Random.value * total;
    //    for (int i = 0; i < itemIDs.Length; i++)
    //    {
    //        if (randomPoint < itemIDs[i].chance)
    //        {
    //            return itemIDs[i].itemID;
    //        }
    //        else
    //        {
    //            randomPoint -= itemIDs[i].chance;
    //        }
    //    }
    //    return itemIDs[^1].itemID;
    //}
}
