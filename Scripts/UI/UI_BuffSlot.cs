using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class UI_BuffSlot : MonoBehaviour
{
    public string buffID;
    public Image iconImage;
    public TMPro.TMP_Text coolTimeText;
    Coroutine coolingCoroutine;

    public delegate void DeleBuffEnd(UI_BuffSlot _slot);
    public DeleBuffEnd OnBuffEnd;

    public float buffStartTime;
    public float buffDuration;

    public enum BuffType
    {
        FishBuff,
        GeneralBuff
    }
    public BuffType buffType;

    public void SetBuffSlot(Game_Manager.FishBuffStruct _buff)
    {
        buffType = BuffType.FishBuff;

        buffID = _buff.id;
        buffStartTime = _buff.buffStartTime;
        buffDuration = _buff.duration;

        Sprite icon = Singleton_Data.INSTANCE.Dict_Sprite[_buff.iconSprite];
        iconImage.sprite = icon;

        iconImage.SetNativeSize();
        float iconWidth = 40f;
        float x = iconWidth / iconImage.rectTransform.sizeDelta.x;
        float y = iconImage.rectTransform.sizeDelta.y * x;
        iconImage.rectTransform.sizeDelta = new Vector2(iconWidth, y);

        if (coolingCoroutine != null)
            StopCoroutine(coolingCoroutine);
        coolingCoroutine = StartCoroutine(CoolingBuff());
    }

    public void SetBuffSlot(Game_Manager.BuffStruct _buff)
    {
        buffType = BuffType.GeneralBuff;

        buffID = _buff.id;
        buffStartTime = _buff.buffStartTime;
        buffDuration = _buff.duration;

        Sprite icon = Singleton_Data.INSTANCE.Dict_Sprite[_buff.iconSprite];
        iconImage.sprite = icon;

        iconImage.SetNativeSize();
        float iconWidth = 40f;
        float x = iconWidth / iconImage.rectTransform.sizeDelta.x;
        float y = iconImage.rectTransform.sizeDelta.y * x;
        iconImage.rectTransform.sizeDelta = new Vector2(iconWidth, y);

        if (coolingCoroutine != null)
            StopCoroutine(coolingCoroutine);
        coolingCoroutine = StartCoroutine(CoolingBuff());
    }

    IEnumerator CoolingBuff()
    {
        float buffTime = Time.time - buffStartTime;
        while (buffDuration > buffTime)
        {
            buffTime = Time.time - buffStartTime;
            coolTimeText.text = Mathf.CeilToInt(buffDuration - buffTime).ToString();
            if (buffDuration <= buffTime)
                break;
            yield return new WaitForSeconds(1f);
        }
        OnBuffEnd?.Invoke(this);
    }
}
