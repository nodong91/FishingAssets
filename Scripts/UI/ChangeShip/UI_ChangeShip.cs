using UnityEngine;
using UnityEngine.UI;

public class UI_ChangeShip : MonoBehaviour
{
    public Unit_Player player;
    public GameObject ship, ship2;
    public UI_ChangeShip_Slot shipButton;
    public GridLayoutGroup shipParent;
    public UI_ChangeShip_Slot selectShip;

    void Start()
    {
        for (int i = 0; i < 10; i++)
        {

            UI_ChangeShip_Slot inst = Instantiate(shipButton, shipParent.transform);
            inst.name = (i % 2).ToString();
            inst.nameText.text = (i % 2).ToString();
            inst.customButton.SetButton(delegate { ShipClick(inst); }, ShipEnter, ShipExit);

            GameObject shipObject = (i % 2 == 0) ? ship : ship2;
            inst.shipObject = shipObject;
        }
        shipParent.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        shipParent.constraintCount = 1;
    }

    void ShipClick(UI_ChangeShip_Slot _slot)
    {
        if (player.playerObject != null)
        {
            //player.playerObject.gameObject.SetActive(false);
            Destroy(player.playerObject);
        }
        GameObject inst = Instantiate(_slot.shipObject, player.transform);
        player.playerObject = inst;
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
