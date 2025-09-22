using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Fishing_Canvas : MonoBehaviour
{
    public RectTransform fishUI;
    public Image fishHP;
    public Image fishSpell;

    public TMP_Text countText;

    public Custom_Button startButton, outButton;
    public TMP_Text startTypeText;

    public void SetStart()
    {
        fishHP.material = Instantiate(fishHP.material);
        fishUI.gameObject.SetActive(false);
        fishSpell.material = Instantiate(fishSpell.material);
    }

    public void SetFishing()
    {
        OnStartButton(0);// 끄기
        outButton.gameObject.SetActive(false);

        fishHP.material.SetFloat("_FillAmount", 1f);
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
            fishUI.gameObject.SetActive(true);
        }
    }

    public void FollowUI(Vector3 _fishPoint, Vector3 _catchPoint)
    {
        fishUI.position = Camera.main.WorldToScreenPoint(_fishPoint);//FollowHPUI
        Debug.LogWarning("FollowUI Fishing!!!");
    }

    public void SetFishHP(float _hp)
    {
        fishHP.material.SetFloat("_FillAmount", _hp);
    }

    public void SetFishSpell(float _spell)
    {
        fishSpell.material.SetFloat("_FillAmount", _spell);
    }

    public void FishingOver()
    {
        outButton.gameObject.SetActive(true);
        fishUI.gameObject.SetActive(false);
    }

    //=========================================================================================================
    // 공격
    //=========================================================================================================

    public RectTransform arrowParent;
    public Image arrow;
    Queue<Image> arrowQueue = new Queue<Image>();
    private List<Image> arrowList = new List<Image>();

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
            OnArrow(i, false);
            inst.gameObject.SetActive(true);
            int cordType = int.Parse(_cord[i].ToString());
            float rotation = cordType * 90f;
            inst.transform.rotation = Quaternion.Euler(0f, 0f, rotation);
        }
    }

    public void OnArrow(int _index, bool _enable)
    {
        arrowList[_index].color = _enable == true ? Color.green : Color.gray;
    }

    public void InputFail()// 실패 시 전부 끄기
    {
        for (int i = 0; i < arrowList.Count; i++)
        {
            if (arrowList[i].gameObject.activeSelf == true)
            {
                OnArrow(i, false);
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
}
