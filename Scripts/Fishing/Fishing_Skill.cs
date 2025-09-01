using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;

public class Fishing_Skill : MonoBehaviour
{
    public class StatusEffect
    {
        public enum EffectType
        {
            None = 0,
            Critical = 1,// 강화 데미지
            Frozon = 2,// fishSpeed 감소
            Poison = 3,// 독 데미지
            Burn = 4,// 화염 데미지
            Slow,// fishAttackSpeed 감소
            WeakensAttack,// fishAttack 감소
            WeakensDefense,// 받는 데미지 증가
            Heist,// fishAttackSpeed 증가
            StrengthenAttack,// 주는 데미지 증가
            StrengthenDefense,// 받는데미지 감소
        }
        public EffectType effectType = EffectType.None;
        public int level = 0;
        public float startTime;// 버프 걸린 시작시간
        public float duration;// 지속 시간
        public float value;// 적용 숫자
    }
    public List<StatusEffect> statusEffectClasses = new List<StatusEffect>();
    Coroutine timerCoroutine;

    public void TakeSkill(StatusEffect _addDamage)
    {
        StatusEffect temp = _addDamage;
        temp.startTime = Time.time;
        statusEffectClasses.Add(_addDamage);

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(StatusEffectTimer());
    }

    IEnumerator StatusEffectTimer()
    {
        while (statusEffectClasses.Count > 0)
        {
            for (int i = 0; i < statusEffectClasses.Count; i++)
            {

            }
            yield return null;
        }
    }

    [Header(" [ Fish ]")]
    public GameObject fishPrefab;
    private float randomTime = 0f;
    public Vector3 fishTargetPoint = Vector3.zero;
    public float fishHealth;
    public float fishSpeed;
    public Image fishImage;
    public Vector3 offset = new Vector3(0f, 0f, 0f);

    public FishStruct fishStatus;

    public void FishMovement()
    {
        fishImage.rectTransform.position = Camera.main.WorldToScreenPoint(fishPrefab.transform.position + offset);//FollowHPUI
    }

    public void RandomMove()
    {
        if (randomTime < Time.time)
        {
            randomTime = Time.time + Random.Range(fishStatus.fishRange.x, fishStatus.fishRange.y);
            Vector3 tempPoint = Random.insideUnitSphere * fishStatus.fieldRadius;
            fishTargetPoint = new Vector3(tempPoint.x, 0f, tempPoint.z) + transform.position;
        }
        // fishPrefab의 위치를 기준으로 Field Radius의 원형 바운더리 안에서 랜덤하게 이동
        Vector3 fishDirection = (fishTargetPoint - fishPrefab.transform.position).normalized;
        float fishSpeed = Time.deltaTime * fishStatus.fishSpeed;
        fishPrefab.transform.position = Vector3.Lerp(fishPrefab.transform.position, fishTargetPoint, fishSpeed);
        fishPrefab.transform.rotation = Quaternion.Slerp(fishPrefab.transform.rotation, Quaternion.LookRotation(fishDirection), fishSpeed);
    }

    public void AttackState()
    {
        StartCoroutine(Attacking());
    }

    IEnumerator Attacking()
    {
        yield return null;
    }
}
