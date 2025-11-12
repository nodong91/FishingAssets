using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_BuffSlot : MonoBehaviour
{
    public Image iconImage;
    public TMPro.TMP_Text coolTimeText;
    Game_Manager.BuffStruct buff;
    Coroutine coolingCoroutine;

    public delegate void DeleBuffEnd(Game_Manager.BuffStruct _buff);
    public DeleBuffEnd OnBuffEnd;

    public void SetBuffSlot(Game_Manager.BuffStruct _buff)
    {
        buff = _buff;
        Sprite icon = Singleton_Data.INSTANCE.Dict_Sprite[buff.iconSprite];
        iconImage.sprite = icon;

        iconImage.SetNativeSize();
        float x = 40f/iconImage.rectTransform.sizeDelta.x ;
        float y = iconImage.rectTransform.sizeDelta.y *x;
        iconImage.rectTransform.sizeDelta = new Vector2(40f, y);

        //if (coolingCoroutine != null)
        //    StopCoroutine(coolingCoroutine);
        //coolingCoroutine = StartCoroutine(CoolingBuff());
    }

    IEnumerator CoolingBuff()
    {
        float buffTime = Time.time - buff.buffStartTime;
        while (buff.duration > buffTime)
        {
            buffTime = Time.time - buff.buffStartTime;
            coolTimeText.text = Mathf.CeilToInt(buff.duration - buffTime).ToString();
            if (buff.duration <= buffTime)
                break;
            yield return new WaitForSeconds(1f);
        }
        OnBuffEnd?.Invoke(buff);
    }
}
