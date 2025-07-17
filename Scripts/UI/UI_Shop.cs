using UnityEngine;
using UnityEngine.UI;
using static UI_Main;

public class UI_Shop : MonoBehaviour
{
    public CanvasStruct[] canvasStructs;
    public Toggle buyToggle, sellToggle, outToggle;

    public void SetStart()
    {
        buyToggle.onValueChanged.AddListener(delegate { ChangeMode(UI_Inventory.BargainType.Buy); });
        sellToggle.onValueChanged.AddListener(delegate { ChangeMode(UI_Inventory.BargainType.Sell); });
        outToggle.onValueChanged.AddListener(OutCanvas);
        OpenCanvas(false);
    }

    public void OpenCanvas(bool _open)
    {
        Game_Manager.current.inventory.OpenCanvas(_open);
        StartCoroutine(OpenCanvasMoving(canvasStructs, _open, 10f));
        buyToggle.isOn = true;
        if (_open == true)
            ChangeMode(UI_Inventory.BargainType.Buy);
        else
            ChangeMode(UI_Inventory.BargainType.None);
    }

    void ChangeMode(UI_Inventory.BargainType _mode)
    {
        Game_Manager.current.inventory.SetBargainType = _mode;
    }

    void OutCanvas(bool _isOn)
    {
        if (_isOn == true)
        {
            OpenCanvas(false);
        }
    }
}
