
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Data_Manager;


public class Skill_Slot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public bool startSlot;
    public Vector2Int slotNode;
    public bool onSlot, hide;
    public List<Vector2Int> nearbySlot = new List<Vector2Int>();
    public RectTransform rect;
    //public Custom_Button slotButton;
    public Image boxImage;

    public StatusStruct statusList;

    public delegate void DeleSlotAction(Vector2Int _grid);
    public DeleSlotAction deleSlotAction;

    public void SetStart()
    {
        //slotButton.SetButton(SlotButton);
        gageImage.fillAmount = 0f;
        if (hide == true)
            boxImage.gameObject.SetActive(false);
        StatusStruct statusStruct = new StatusStruct
        {
            setStatus = new List<StatusStruct.SetStruct>()
            {
                new StatusStruct.SetStruct
                {
                    statusType = StatusStruct.StatusType.CatchRadius,
                    value = 0.5f, // 임시로 CatchRadius 값 설정
                }
            },
        };
        statusList = statusStruct;// 임시로 CatchRadius만 설정
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
    }

    //==================================================================================================================================
    // Input
    //==================================================================================================================================

    public void OnPointerClick(PointerEventData eventData)
    {
        // 인포메이션
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = Vector3.one * 1.1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (inputSlotCoroutine != null)
            StopCoroutine(inputSlotCoroutine);

        transform.localScale = Vector3.one;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 누르고 있기
        inputSlotCoroutine = StartCoroutine(InputSlot());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (inputSlotCoroutine != null)
            StopCoroutine(inputSlotCoroutine);

        if (onSlot == false)
        {
            gageImage.fillAmount = 0f;
        }
    }

    void SlotButton()
    {
        if (onSlot == false)
        {
            onSlot = true;
            deleSlotAction?.Invoke(slotNode);
        }
    }

    Coroutine inputSlotCoroutine;
    public Image gageImage;
    IEnumerator InputSlot()
    {
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime;
            gageImage.fillAmount = Mathf.Lerp(0f, 1f, normalize);
            yield return null;
        }
        SlotButton();
    }
}
