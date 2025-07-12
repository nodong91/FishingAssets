using UnityEngine;
using UnityEngine.UI;

public class UI_Main : MonoBehaviour
{
    public Button inventoryButton;
    public Button equipButton;

    public void SetStart()
    {
        inventoryButton.onClick.AddListener(InventoryButton);
        equipButton.onClick.AddListener(EquipButton);
    }

    // Update is called once per frame
    void InventoryButton()
    {
        Game_Manager.current.inventory.OpenCanvas();
    }

    void EquipButton()
    {
        Game_Manager.current.equip.OpenCanvas();
    }
}
