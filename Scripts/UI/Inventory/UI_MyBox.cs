using UnityEngine;
using UnityEngine.UI;

public class UI_MyBox : UI_Inventory_Base
{
    [Header("- Box")]
    public Image weightSlider;
    public float currentWeight, maxWeight;
    public UI_Inventory_Remove_Box removeBox;

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
        weightSlider.fillAmount = sliderValue;
        //Static_JsonManager.SaveInventory(saveData, GetSaveInventoryData); ;   // 내용물에 변경이 있으면 저장
    }
}
