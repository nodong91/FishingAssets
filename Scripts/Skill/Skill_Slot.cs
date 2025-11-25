using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Data_Manager;


public class Skill_Slot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public bool startSlot;
    public Vector2Int slotNode;
    public bool onSlot;
    public bool hide = true;
    public List<Vector2Int> nearbySlot = new List<Vector2Int>();
    public Sprite[] levelIcons;
    public RectTransform rect;
    public Image iconImage, levelImage, boxImage;

    Coroutine inputSlotCoroutine;
    public Image gageImage;
    public AnimationCurve OpeningCurve => Game_Manager.current.GetSkill.openingCurve;
    public SkillStruct Skill { get; set; }

    public delegate void DeleSlotAction(Vector2Int _grid);
    public DeleSlotAction deleSlotAction;

    public delegate void DeleSlotPosition(SkillStruct _status, Vector3 _position = default);
    public DeleSlotPosition deleSlotPosition;

    Dictionary<string, Sprite> sprites => Singleton_Data.INSTANCE.Dict_Sprite;

    public void SetStart()
    {
        if (Skill == null || System.String.IsNullOrEmpty(Skill.id) == true)
            return;
        Debug.LogWarning(Skill.id);

        gageImage.fillAmount = 0f;
        if (Skill.icon != null && sprites.ContainsKey(Skill.icon))
        {
            iconImage.gameObject.SetActive(true);
            iconImage.sprite = sprites[Skill.icon];
            iconImage.material = Instantiate(iconImage.material);
            iconImage.material.SetFloat(Const_Shader._fillAmount, 0f);

            int level = int.Parse(Skill.id.Substring(Skill.id.Length - 2, 2));
            levelImage.sprite = levelIcons[level];
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

    public void SetHide(bool _hide, Vector3 _prev = default)
    {
        boxImage.gameObject.SetActive(!_hide);
        if (hide == true && _prev != default)
        {
            hide = false;
            StartCoroutine(OpeningSlot(_prev));
        }
    }

    public void ResetSlot()
    {
        EnableSlot(false);
        if (startSlot == true)
            return;

        hide = true;
        boxImage.gameObject.SetActive(false);
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
        deleSlotPosition?.Invoke(Skill, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (inputSlotCoroutine != null)
            StopCoroutine(inputSlotCoroutine);

        transform.localScale = Vector3.one;
        deleSlotPosition?.Invoke(null);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Skill == null || System.String.IsNullOrEmpty(Skill.id) == true)
            return;

        if (Game_Manager.current.CheckMoney(Skill.price) == false)// 돈이 없으면
            return;

        if (onSlot == false)
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
            iconImage.material.SetFloat(Const_Shader._fillAmount, gageImage.fillAmount);
        }
    }

    public void EnableSlot(bool _enable)
    {
        onSlot = _enable;
        gageImage.fillAmount = _enable == true ? 1f : 0f;
        iconImage.material.SetFloat(Const_Shader._fillAmount, gageImage.fillAmount);
    }

    IEnumerator OnSlotAction()
    {
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 5f;
            float curveValue = OpeningCurve.Evaluate(normalize);
            boxImage.transform.localScale = Vector3.one * (1f + curveValue * 0.3f);
            yield return null;
        }
    }

    IEnumerator InputSlot()// 슬롯 활성화
    {
        float normalize = 0f;
        while (normalize < 1f)
        {
            // 누르고 있기
            normalize += Time.deltaTime;
            gageImage.fillAmount = Mathf.Lerp(0f, 1f, normalize);
            iconImage.material.SetFloat(Const_Shader._fillAmount, gageImage.fillAmount);
            yield return null;
        }
        ActiveSlot();
    }

    public void ActiveSlot()
    {
        // 스킬 활성화
        float price = -Skill.price;
        Game_Manager.current.GetMainUI.MoveMoney(price);// 돈 이동

        deleSlotAction?.Invoke(slotNode);
        StartCoroutine(OnSlotAction());// 슬롯 열기 액션
    }

    IEnumerator OpeningSlot(Vector3 _prev)// 슬롯 열리기
    {
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 5f;
            float curveValue = OpeningCurve.Evaluate(normalize);
            boxImage.transform.position = Vector3.Lerp(_prev, transform.position, normalize * 5f);
            boxImage.transform.rotation = Quaternion.Euler(0f, 0f, curveValue * 90f);
            yield return null;
        }
    }
}
