using UnityEngine;
using UnityEngine.UI;

public class UI_ChangeShip : MonoBehaviour
{
    public Unit_Player Player => Game_Manager.current?.GetPlayer;
    public Data_Ship[] ship;
    public UI_ChangeShip_Slot shipButton;
    public GridLayoutGroup shipParent;

    void Start()
    {
        for (int i = 0; i < ship.Length; i++)
        {

            UI_ChangeShip_Slot inst = Instantiate(shipButton, shipParent.transform);
            inst.shipObject = ship[i].shipObject;
            inst.name = i.ToString();
            inst.nameText.text = ship[i].name;
            inst.customButton.SetButton(delegate { ShipClick(inst); }, ShipEnter, ShipExit);
        }
        shipParent.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        shipParent.constraintCount = 1;
    }

    void ShipClick(UI_ChangeShip_Slot _slot)
    {
        if (Player == null)
            return;

        if (Player.playerObject != null)
        {
            //player.playerObject.gameObject.SetActive(false);
            Destroy(Player.playerObject);
        }
        GameObject inst = Instantiate(_slot.shipObject, Player.transform);
        Player.playerObject = inst;
        Debug.LogWarning($"{_slot.name} : {_slot.shipObject}");
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
