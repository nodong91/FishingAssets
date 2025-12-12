using System.Collections;
using TMPro;
using UnityEngine;
using static Data_Manager;

public class Skill_Infomation : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public RectTransform rect;
    public TMP_Text text_Neme;
    public TMP_Text text_Description;
    public TMP_Text text_Price;
    Coroutine moving;
    public AnimationCurve OpeningCurve => Game_Manager.current.GetSkill.openingCurve;

    private void Start()
    {
        SetPosition(null);
    }

    public void SetPosition(Skill_Slot _slot, Vector3 _position = default)
    {
        float alpha = (_slot == null) ? 0f : 1f;
        Vector2 viewportPoint;
        if (alpha == 0f)
        {
            viewportPoint = Vector2.one * 0.5f;
            _position = Camera.main.ViewportToScreenPoint(viewportPoint);
        }
        else
        {
            viewportPoint = Camera_Manager.current.UICamera.ScreenToViewportPoint(_position);
            float viewX = Mathf.Ceil(viewportPoint.x);
            float viewY = Mathf.Ceil(viewportPoint.y);
            viewportPoint = new Vector2(viewX, viewY);
        }

        if (moving != null)
            StopCoroutine(moving);
        moving = StartCoroutine(SetMoving(_position, viewportPoint, alpha));

        if (_slot == null)
            return;

        text_Neme.text = Singleton_Data.INSTANCE.GetLanguage(_slot.Skill.name);
        int fontSize = (int)(text_Description.fontSize * 0.7f);
        //string addStatusString = $"\n<size={fontSize}>{_status.addStatusString}</size>";
        text_Description.text = Singleton_Data.INSTANCE.GetLanguage(_slot.Skill.description);

        text_Price.gameObject.SetActive(!_slot.activeSlot);// È°¼ºÈ­ µÈ ½½·ÔÀÌ¸é °¡°Ý ¼û±è
        if (_slot.activeSlot == true)
            return;
        text_Price.text = Game_Manager.current.GetSkill.GetSkillPrice.ToString();
    }

    IEnumerator SetMoving(Vector3 _position, Vector2 _viewportPoint, float _alpha)
    {
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 5f;
            yield return null;

            if (_alpha > 0)
            {
                rect.pivot = Vector2.Lerp(rect.pivot, _viewportPoint, normalize);
                //rect.transform.position = _position;
                rect.transform.position = Vector3.Lerp(rect.position, _position, normalize);
                float curveValue = OpeningCurve.Evaluate(normalize);
                canvasGroup.transform.rotation = Quaternion.Euler(0f, 0f, curveValue * 10f);
                //canvasGroup.transform.localScale = Vector3.one * (1f + curveValue * 0.3f);
            }
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, _alpha, normalize);
        }
    }
}
