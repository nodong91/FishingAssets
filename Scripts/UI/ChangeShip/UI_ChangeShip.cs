using UnityEngine;
using UnityEngine.UI;

public class UI_ChangeShip : MonoBehaviour
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    public Data_Ship[] ship;
    public UI_ChangeShip_Slot shipButton;
    public GridLayoutGroup shipParent;

    public Custom_Button backButton;

    public void SetStart()
    {
        backButton.SetButton(CloseCanvas);
        for (int i = 0; i < ship.Length; i++)
        {
            UI_ChangeShip_Slot inst = Instantiate(shipButton, shipParent.transform);
            inst.shipData = ship[i];
            inst.name = i.ToString();
            inst.nameText.text = ship[i].name;
            inst.customButton.SetButton(delegate { ShipClick(inst); }, ShipEnter, ShipExit);
        }
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

    void ShipClick(UI_ChangeShip_Slot _slot)// ¹è¼±ÅÃ
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
