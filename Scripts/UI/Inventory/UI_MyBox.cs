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
        weightSlider.material = Instantiate(weightSlider.material); removeBox.deleRemove = RemoveDragItem;
        base.SetStart();
    }

    public override void OpenCanvas(bool _open)
    {
        base.OpenCanvas(_open);
    }

    protected override void SetWeight(float _weight)
    {
        currentWeight += _weight;
        float sliderValue = currentWeight / maxWeight;
        weightSlider.material.SetFloat("_FillAmount", sliderValue);
    }
}
