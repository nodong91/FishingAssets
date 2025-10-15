using UnityEngine;
using UnityEngine.UI;

public class UI_MyBox : UI_Inventory_Base
{
    [Header("- Box")]
    public Slider weightSlider;
    public float currentWeight, maxWeight;
    public UI_Inventory_Remove_Box removeBox;
    public TMPro.TMP_Text weightText;

    public override void SetStart()
    {
        slotType = SlotType.MyBox;
        saveData = "MyBag";
        removeBox.deleRemove = RemoveDragItem;
        base.SetStart();
        // 저장된 내용 불러오기
        SetInventoryItem(saveData);
    }

    public void AddInventory(Vector2Int _inventorySize)
    {
        if (inventorySize == _inventorySize)
            return;
        Debug.LogWarning($"인벤토리 사이즈 변경 {inventorySize} -> {_inventorySize}");
        GetSaveInventoryData.invenSize = _inventorySize;
        SetInventoryItem(saveData);
    }

    public override void OpenCanvas(bool _open)
    {
        base.OpenCanvas(_open);
    }

    protected override void SetWeight(float _weight)
    {
        currentWeight += _weight;
        float sliderValue = currentWeight / maxWeight;
        weightSlider.value = sliderValue;
        weightText.text = $"{Mathf.Round(currentWeight * 10f) * 0.1f}/{maxWeight}kg";
        //Static_JsonManager.SaveInventory(saveData, GetSaveInventoryData); ;   // 내용물에 변경이 있으면 저장
    }

    public bool CheckWeight(float _weight)
    {
        return currentWeight + _weight <= maxWeight;
    }
}
