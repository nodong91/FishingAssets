using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChangeShip : MonoBehaviour
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    public List<Data_Ship> shipList = new List<Data_Ship>();
    public int GetShipCount { get { return shipList.Count; } }
    public UI_ChangeShip_Slot shipButton;
    public GridLayoutGroup shipParent;

    public Custom_Button backButton;

    public void SetStart()
    {
        backButton.SetButton(CloseCanvas);
        shipList.Clear();
        OpenCanvas(false);// 세팅 완료
    }

    public void AddShip(Data_Ship _shipData)
    {
        if (shipList.Contains(_shipData) == true)
            return;

        shipList.Add(_shipData);
        Debug.LogWarning($"{_shipData.name} : {_shipData.shipName} : {shipList.Count} : {gameObject.name}");

        UI_ChangeShip_Slot inst = Instantiate(shipButton, shipParent.transform);
        inst.SetSlot(_shipData);
        inst.customButton.SetButton(delegate { ShipClick(inst); }, ShipEnter, ShipExit);

        shipParent.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        shipParent.constraintCount = 1;
    }

    public void OpenCanvas(bool _open)
    {
        Game_Manager.current.FocusShip(_open);
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
    }

    public void CloseCanvas()
    {
        Game_Manager.current.GetLanding.OpenLandingUI();
        OpenCanvas(false);
    }

    public void SelectTutorialShip()
    {
        // 튜토리얼용 배선택
        Game_Manager.current.ChangeStatus(shipList[0]);
    }

    void ShipClick(UI_ChangeShip_Slot _slot)// 배선택
    {
        Singleton_Continue.INSTANCE.SaveContinue();// 배 변경 시 저장
        Game_Manager.current.ChangeStatus(_slot.shipData);
        Debug.LogWarning($"이게 뒤에 오나? : {shipList.Count} : {gameObject.name}");
    }

    void ShipEnter(Custom_Button _button)
    {
        _button.buttonImage.color = Color.white;
    }

    void ShipExit(Custom_Button _button)
    {
        _button.buttonImage.color = Color.gray;
    }
}
