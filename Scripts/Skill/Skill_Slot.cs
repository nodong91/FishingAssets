
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;

public class Skill_Slot : MonoBehaviour
{
    public bool startSlot;
    public Vector2Int slotNode;
    public bool onSlot, hide;
    public List<Vector2Int> nearbySlot = new List<Vector2Int>();
    public RectTransform rect;
    public Custom_Button slotButton;
    public Image boxImage;

    public StatusStruct statusList;

    public delegate void DeleSlotAction(Vector2Int _grid);
    public DeleSlotAction deleSlotAction;

    public void SetStart()
    {
        slotButton.SetButton(SlotButton);
        boxImage.color = Color.gray;
        if (hide == true)
            boxImage.gameObject.SetActive(false);
        StatusStruct statusStruct = new StatusStruct
        {
            //setStatus = new List<StatusStruct.SetStruct>()
            //{

            //},
        };
        statusList = statusStruct;// 임시로 CatchRadius만 설정
    }

    void SlotButton()
    {
        if (onSlot == false)
        {
            onSlot = true;
            boxImage.color = Color.white;
            deleSlotAction?.Invoke(slotNode);
        }
    }

    public void SetNearBySlot(Vector2Int _map)
    {
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (x == 0 && y == 0)
                    continue;

                int slotX = x + slotNode.x;
                int slotY = y + slotNode.y;
                if (slotX >= 0 && slotX < _map.x && slotY >= 0 && slotY < _map.y)
                {
                    if (x == 0 || y == 0)
                        nearbySlot.Add(new Vector2Int(slotX, slotY));
                }
            }
        }
    }

    public void SetHide(bool _hide)
    {
        if (onSlot == true && hide == false)
            return;

        hide = _hide;
        boxImage.gameObject.SetActive(!hide);
        if (hide == false)
            boxImage.color = Color.gray;
        else
            boxImage.color = Color.white;
    }
}
