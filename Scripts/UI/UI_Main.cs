using UnityEngine;
using UnityEngine.UI;

public class UI_Main : MonoBehaviour
{
    public Button inventoryButton;
    public Button fishGuideButton;

    public void SetStart()
    {
        inventoryButton.onClick.AddListener(InventoryButton);
        fishGuideButton.onClick.AddListener(FishGuideButton);
    }

    // Update is called once per frame
    void InventoryButton()
    {
        Game_Manager.current.inventory.OpenCanvas(true);
    }

    void FishGuideButton()
    {
        Game_Manager.current.fishGuide.OpenCanvas(true);
    }
}
