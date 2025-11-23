using UnityEngine;
using UnityEngine.UI;

public class UI_ChangeShip_Slot : MonoBehaviour
{
    public Image iconImage;
    public Custom_Button customButton;

    public Data_Ship shipData;

    public void SetSlot(Data_Ship _shipData)
    {
        shipData = _shipData;
        iconImage.sprite = _shipData.icon;
    }
}
