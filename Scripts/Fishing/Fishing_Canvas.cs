using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Fishing_Canvas : MonoBehaviour
{
    //public RectTransform catchUI;
    //public Image catchHP;

    public RectTransform fishUI;
    public Image fishHP;
    public Image fishSpell;

    [ColorUsage(true, true)]
    public Color catchColor, fishColor, spellColor;

    public Vector3 fishOffset, shipOffset;
    public TMP_Text countText;

    public Custom_Button closeButton, startButton, outButton;
    public TMP_Text startTypeText;

    public void SetStart()
    {
        //catchHP.material = Instantiate(catchHP.material);
        //catchUI.gameObject.SetActive(false);
        fishHP.material = Instantiate(fishHP.material);
        fishUI.gameObject.SetActive(false);
        fishSpell.material = Instantiate(fishSpell.material);
    }

    public void SetFishing()
    {
        OnStartButton(0);// ²ô±â
        outButton.gameObject.SetActive(false);

        //catchHP.material.SetColor("_MainColor", catchColor);
        //catchHP.material.SetFloat("_FillAmount", 1f);
        fishHP.material.SetColor("_MainColor", fishColor);
        fishHP.material.SetFloat("_FillAmount", 1f);
        fishSpell.material.SetColor("_MainColor", spellColor);
        fishSpell.material.SetFloat("_FillAmount", 0f);
    }

    public void OnStartButton(int _count, string _areaType = "", string _dayType = "")
    {
        startButton.gameObject.SetActive(_count > 0);
        startTypeText.gameObject.SetActive(_count > 0);
        startTypeText.text = $"{_areaType}\n<size=15>{_dayType}\nCount : {_count}</size>";
    }

    public void SetCount(int _index)
    {
        countText.text = _index.ToString();
        countText.gameObject.SetActive(_index > 0);
        if (_index == 0)
        {
            //catchUI.gameObject.SetActive(true);
            fishUI.gameObject.SetActive(true);
        }
    }

    public void FollowUI(Vector3 _fishPoint, Vector3 _catchPoint)
    {
        fishUI.position = Camera.main.WorldToScreenPoint(_fishPoint + fishOffset);//FollowHPUI
        //catchUI.position = Camera.main.WorldToScreenPoint(_catchPoint + shipOffset);//FollowHPUI
        Debug.LogWarning("FollowUI Fishing!!!");
    }

    //public void SetCatchHP(float _hp)
    //{
    //    catchHP.material.SetFloat("_FillAmount", _hp);
    //}

    public void SetFishHP(float _hp)
    {
        fishHP.material.SetFloat("_FillAmount", _hp);
    }

    public void SetFishSpell(float _spell)
    {
        fishSpell.material.SetFloat("_FillAmount", _spell);
    }

    public void OnOutButton()
    {
        outButton.gameObject.SetActive(true);
    }

    //=========================================================================================================
    // °ø°Ý
    //=========================================================================================================

    public RectTransform arrowParent;
    public Image arrow;
    Queue<Image> arrowQueue = new Queue<Image>();
    List<Image> arrowList = new List<Image>();
    public void SetArrow(string _cord)
    {
        arrowParent.gameObject.SetActive(true);
        arrowQueue = new Queue<Image>();
        for (int i = 0; i < arrowList.Count; i++)
        {
            arrowQueue.Enqueue(arrowList[i]);
            arrowList[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < _cord.Length; i++)
        {
            Image inst = TryArrow();
            OnArrow(i, 0f);
            inst.gameObject.SetActive(true);
            int cordType = int.Parse(_cord[i].ToString());
            float rotation = cordType * 90f;
            inst.transform.rotation = Quaternion.Euler(0f, 0f, rotation);
        }
    }

    public void OnArrow(int _index, float _fill)
    {
        arrowList[_index].material.SetFloat("_FillAmount", _fill);
    }

    public void OffArrowAll()
    {
        for (int i = 0; i < arrowList.Count; i++)
        {
            if (arrowList[i].gameObject.activeSelf == true)
            {
                arrowList[i].material.SetFloat("_FillAmount", 0f);
            }
        }
    }

    public void OnArrowParent(bool _on)
    {
        arrowParent.gameObject.SetActive(_on);
    }

    Image TryArrow()
    {
        if (arrowQueue.Count > 0)
            return arrowQueue.Dequeue();
        Image inst = Instantiate(arrow, arrowParent);
        inst.material = Instantiate(inst.material);
        arrowList.Add(inst);
        return inst;
    }











    //public Image fishingImage;
    //public void SetFishingImage(float _test)
    //{
    //    fishingImage.material = Instantiate(fishingImage.material);
    //    fishingImage.material.SetFloat("_FillAmount", _test);
    //}
}
