using UnityEngine;
using UnityEngine.UI;

public class UI_MyBox : UI_Inventory_Base
{
    [Header("- Box")]
    public Slider weightSlider;
    public float currentWeight;
    public float maxWeight;
    public UI_Inventory_Remove_Box removeBox;
    public TMPro.TMP_Text weightText;

    public override void SetStart()
    {
        slotType = SlotType.MyBox;
        saveData = "MyBag";
        removeBox.deleRemove = RemoveDragItem;
        OnRemoveBox(false);

        base.SetStart();
        // 저장된 내용 불러오기
        SetInventoryItem(saveData);// 마이박스
    }

    public void AddMaxWeight(float _weight)
    {
        maxWeight = _weight;
        SetWeight(0f);
    }

    public void AddInventory(Vector2Int _inventorySize)
    {
        if (inventorySize == _inventorySize)
            return;

        //Debug.LogWarning($"인벤토리 사이즈 변경 {inventorySize} -> {_inventorySize}");
        if (GetSaveInventoryData == null)
        {
            LoadInventory();
        }

        GetSaveInventoryData.invenSize = _inventorySize;
        SetInventoryItem(saveData);// 마이박스AddInventory
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
        weightText.text = $"{currentWeight.ToString("N1")}/{maxWeight}kg";
        //Debug.LogWarning($"무게 {weightText.text}");
    }

    public bool CheckWeight(float _weight)
    {
        bool check = currentWeight + _weight <= maxWeight;
        if (check == false)
        {
            Game_Manager.current.GetMainUI.SetWarnningText(Const_ETC._overWeight);
            return false;
        }
        return true;
    }

    public void OnRemoveBox(bool _isOn)
    {
        removeBox.gameObject.SetActive(_isOn);
    }
}
