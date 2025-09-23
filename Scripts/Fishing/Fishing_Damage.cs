using System.Collections;
using UnityEngine;

public class Fishing_Damage : MonoBehaviour
{
    public RectTransform rect;
    public Animator animator;
    public TMPro.TMP_Text damageText;
    public GameObject critical;
    AnimatorStateInfo animStateInfo;
    public delegate void Delegate_Damage(Fishing_Damage _damage);
    public Delegate_Damage deleDamage;

    public void SetStart(string _damage, bool _cri)
    {
        animator.Play(_cri ? "Critical" : "Normal", -1, 0f);
        damageText.text = _damage;
        critical.SetActive(_cri);
        StartCoroutine(EndDamageAction());
    }

    IEnumerator EndDamageAction()
    {
        if (animator != null)
            animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(animStateInfo.length);
        deleDamage(this);
    }
}
