using UnityEngine;

[CreateAssetMenu(fileName = "Data_ItemList", menuName = "Scriptable Objects/Data_ItemList")]
public class Data_ItemList : ScriptableObject
{
    public enum InventoryType
    {
        Fix,
        Random,
        Shop,
        Shipyard,
        Smuggler,
        Fix_Loan,
    }
    public InventoryType inventoryType;

    [System.Serializable]
    public struct ItemIDStruct
    {
        public string itemID;
        [Range(0.1f, 100f)]
        public float chance;
    }
    public Vector2Int itemAmount;
    public ItemIDStruct[] itemIDs;

    public string[] GetFixItems()
    {
        string[] items = new string[itemIDs.Length];
        for (int i = 0; i < items.Length; i++)
        {
            string itemString = itemIDs[i].itemID;// 아이템 목록에서 아이템 ID 가져오기
            items[i] = itemString;
            Debug.LogWarning(itemString);
        }
        return items;
    }

    public string[] GetRandomItems(int _amount)
    {
        string[] items = new string[_amount];
        for (int i = 0; i < items.Length; i++)
        {
            string itemString = GetItemID();// 랜덤 아이템
            items[i] = itemString;
            Debug.LogWarning(itemString);
        }
        return items;
    }

    // 랜덤 아이템
    string GetItemID()
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
}
