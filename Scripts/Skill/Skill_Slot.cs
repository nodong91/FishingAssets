
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill_Slot : MonoBehaviour
{
    public bool startSlot;
    public Vector2Int slotNode;
    public bool onSlot, hide;
    public List<Skill_Slot> nearbySlot;
    public RectTransform rect;
    public Button slotButton;
    public Image boxImage;

    private void Start()
    {
        slotButton.onClick.AddListener(SlotButton);
        boxImage.color = Color.gray;
        if (hide == true)
            boxImage.gameObject.SetActive(false);
    }

    void SlotButton()
    {
        if (onSlot == false)
        {
            onSlot = true;
            boxImage.color = Color.white;
            for (int i = 0; i < nearbySlot.Count; i++)
            {
                if (nearbySlot[i].onSlot == true && nearbySlot[i].hide == false)
                    continue;
                nearbySlot[i].hide = false;
                nearbySlot[i].boxImage.gameObject.SetActive(true);
            }
        }
    }

    int TryCheckNearbySlot()
    {
        int onCheck = 0;
        for (int i = 0; i < nearbySlot.Count; i++)
        {
            if (nearbySlot[i].onSlot == true)
                onCheck++;
        }
        return onCheck;
    }
}
