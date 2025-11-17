using UnityEngine;
using UnityEngine.UI;

public class UI_ChangeShip_Slot : MonoBehaviour
{
    public TMPro.TMP_Text nameText;
    public Image iconImage;
    public Custom_Button customButton;

    public Data_Ship shipData;

    public void SetSlot(Data_Ship _shipData)
    {
        shipData = _shipData;
        nameText.text = _shipData.shipName;
        iconImage.sprite = _shipData.icon;
    }
}
