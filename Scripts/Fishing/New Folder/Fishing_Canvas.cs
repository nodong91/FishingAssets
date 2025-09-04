using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fishing_Canvas : MonoBehaviour
{
    Data_Manager.FishStruct fishStruct;
    public RectTransform catchUI;
    public Image catchHP;
    public RectTransform fishUI;
    public Image fishHP;
    public Image fishSpell;
    [ColorUsage(true, true)]
    public Color catchColor, fishColor, spellColor;

    public Transform fishPrefab, catchPrefab;
    public Vector3 fishOffset, shipOffset;
    public TMPro.TMP_Text countText;

    public TMPro.TMP_Text infoText;
    public GameObject infomation;
    public Button closeButton;
    public Custom_Button startButton;

    public delegate void Dele_ReStart();
    public Dele_ReStart deleReStart;

    public void SetStart()
    {
        catchHP.material = Instantiate(catchHP.material);
        fishHP.material = Instantiate(fishHP.material);
        fishSpell.material = Instantiate(fishSpell.material);
        OpenCanvas(false);
    }

    public void SetFishing(Data_Manager.FishStruct _fishStruct)
    {
        fishStruct = _fishStruct;

        catchHP.material.SetColor("_MainColor", catchColor);
        catchHP.material.SetFloat("_FillAmount", 1f);
        fishHP.material.SetColor("_MainColor", fishColor);
        fishHP.material.SetFloat("_FillAmount", 1f);
        fishSpell.material.SetColor("_MainColor", spellColor);
        fishSpell.material.SetFloat("_FillAmount", 0f);
    }

    public void SetCount(int _index)
    {
        countText.text = _index.ToString();
        countText.gameObject.SetActive(_index > 0);
    }

    public void FollowUI()
    {
        fishUI.position = Camera.main.WorldToScreenPoint(fishPrefab.position + fishOffset);//FollowHPUI
        catchUI.position = Camera.main.WorldToScreenPoint(catchPrefab.position + shipOffset);//FollowHPUI
    }

    public void SetCatchHP(float _hp)
    {
        catchHP.material.SetFloat("_FillAmount", _hp);
    }

    public void SetFishHP(float _hp)
    {
        fishHP.material.SetFloat("_FillAmount", _hp);
    }

    public void SetFishSpell(float _spell)
    {
        fishSpell.material.SetFloat("_FillAmount", _spell);
    }

    public void SetFinish(bool _success)
    {
        //if (_success == true)
        //{
        //    OpenCanvas(true);
        //}
        OpenCanvas(true);
    }

    public void OpenCanvas(bool _open)
    {
        infomation.SetActive(_open);
        if (_open == true)
        {
            closeButton.onClick.AddListener(CloseCanvas);
            SetInfomation();
        }
    }

    public void CloseCanvas()
    {
        infomation.SetActive(false);
        deleReStart?.Invoke();
    }

    void SetInfomation()
    {
        infoText.text = fishStruct.itemStruct.name;
    }









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

    public void OnArrowPrent(bool _on)
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
