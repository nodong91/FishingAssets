using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChangeShip : MonoBehaviour
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    public List<Data_Ship> shipList = new List<Data_Ship>();
    public UI_ChangeShip_Slot shipButton;
    public GridLayoutGroup shipParent;

    public Custom_Button backButton;

    public void SetStart()
    {
        backButton.SetButton(CloseCanvas);
        shipList.Clear();
        OpenCanvas(false);
    }

    public void AddShip(Data_Ship _shipData)
    {
        shipList.Add(_shipData);

        UI_ChangeShip_Slot inst = Instantiate(shipButton, shipParent.transform);
        inst.shipData = _shipData;
        inst.nameText.text = _shipData.name;
        inst.customButton.SetButton(delegate { ShipClick(inst); }, ShipEnter, ShipExit);

        shipParent.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        shipParent.constraintCount = 1;
    }

    public void OpenCanvas(bool _open)
    {
        Game_Manager.current.FocusShip(_open);
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
    }

    void CloseCanvas()
    {
        Game_Manager.current.GetLanding.OpenLandingUI();
        OpenCanvas(false);
    }

    void ShipClick(UI_ChangeShip_Slot _slot)// πËº±≈√
    {
        Game_Manager.current.ChangeStatus(_slot.shipData);
    }

    void ShipEnter(Custom_Button _button)
    {
        _button.buttonImage.color = Color.gray;
    }

    void ShipExit(Custom_Button _button)
    {
        _button.buttonImage.color = Color.white;
    }
}
