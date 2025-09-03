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
    public Button button;

    public delegate void Dele_ReStart();
    public Dele_ReStart deleReStart;

    public void SetStart(Data_Manager.FishStruct _fishStruct)
    {
        fishStruct = _fishStruct;

        catchHP.material = Instantiate(catchHP.material);
        catchHP.material.SetColor("_MainColor", catchColor);
        catchHP.material.SetFloat("_FillAmount", 1f);

        fishHP.material = Instantiate(fishHP.material);
        fishHP.material.SetColor("_MainColor", fishColor);
        fishHP.material.SetFloat("_FillAmount", 1f);

        fishSpell.material = Instantiate(fishSpell.material);
        fishSpell.material.SetColor("_MainColor", spellColor);
        fishSpell.material.SetFloat("_FillAmount", 0f);
        OpenCanvas(false);
    }

    public void SetCount(int _index)
    {
        countText.text = _index.ToString();
        countText.gameObject.SetActive(_index > 0);
    }

    void Update()
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

    public void SetEnd(bool _success)
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
            button.onClick.AddListener(CloseCanvas);
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
}
