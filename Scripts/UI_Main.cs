using UnityEngine;
using UnityEngine.UI;

public class UI_Main : MonoBehaviour
{
    public Button inventoryButton;
    public void SetStart()
    {
        inventoryButton.onClick.AddListener(InventoryButton);
    }

    // Update is called once per frame
    void InventoryButton()
    {
        Game_Manager.current.inventory.OpenCanvas();
    }
}
