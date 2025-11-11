using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_BuffSlot : MonoBehaviour
{
    public Image iconImage;
    public TMPro.TMP_Text coolTimeText;
    Game_Manager.BuffStruct buff;

    public void SetBuffSlot(Game_Manager.BuffStruct _buff)
    {
        buff = _buff;
        Sprite icon = Singleton_Data.INSTANCE.Dict_Sprite[buff.iconSprite];
        iconImage.sprite = icon;
        StartCoroutine(CoolingBuff());
    }

    IEnumerator CoolingBuff()
    {
        float buffTime = Time.time - buff.buffStartTime;
        while (buff.duration > buffTime)
        {
            buffTime = Time.time - buff.buffStartTime;
            coolTimeText.text = Mathf.CeilToInt(buff.duration - buffTime).ToString();
            Debug.LogWarning("Buff Time: " + coolTimeText.text);
            yield return new WaitForSeconds(1f);
        }
        coolTimeText.text = "";
    }
}
