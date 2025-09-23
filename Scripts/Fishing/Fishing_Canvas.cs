using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Fishing_Canvas : MonoBehaviour
{
    public Canvas canvas;

    public RectTransform fishUI;
    public Image fishHP;
    public Image fishSpell;

    public TMP_Text countText;

    public Custom_Button startButton, outButton;
    public TMP_Text startTypeText;
    public Animator animator;

    public void SetStart()
    {
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera_Manager.current.UICamera;
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
        animator.Play("Critical", -1, 0f);
        countText.text = _index.ToString();
        countText.gameObject.SetActive(_index > 0);
    }

    public void SetFishUI()
    {
        fishUI.gameObject.SetActive(true);
    }

    public void FollowUI(Vector3 _fishPoint)
    {
        fishUI.position = UIPosition(_fishPoint);
    }

    Vector3 UIPosition(Vector3 _fishPoint)
    {
        Debug.LogWarning("FollowUI Fishing!!!");
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(_fishPoint);
        Camera UICamera = canvas.worldCamera;
        Vector3 followPosition = UICamera.ScreenToWorldPoint(screenPosition);
        return followPosition;
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
        arrowList[_index].color = _enable == true ? Color.red : Color.white;
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

    public Fishing_Damage fishingDamage;
    public Queue<Fishing_Damage> QueueDamage = new Queue<Fishing_Damage>();
    public void SetDamage(float _damage, bool _cri)
    {
        Fishing_Damage inst = TryFishingDamage();
        inst.gameObject.SetActive(true);
        inst.rect.position = fishUI.position;
        inst.SetStart(_damage.ToString(), _cri);
    }

    Fishing_Damage TryFishingDamage()
    {
        if (QueueDamage.Count > 0)
            return QueueDamage.Dequeue();
        Fishing_Damage inst = Instantiate(fishingDamage, canvas.transform);
        inst.deleDamage = AddPool;
        return inst;
    }

    void AddPool(Fishing_Damage _damage)
    {
        QueueDamage.Enqueue(_damage);
        _damage.gameObject.SetActive(false);
    }
}
