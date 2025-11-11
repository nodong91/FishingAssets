using UnityEngine;
using UnityEngine.UI;
using static Game_Manager;

public class UI_BuffSlot : MonoBehaviour
{
    public Image iconImage;
    public TMPro.TMP_Text coolTimeText;
    BuffStruct buff;

    public void SetBuffSlot(BuffStruct _buff)
    {
        buff = _buff;
    }
}
