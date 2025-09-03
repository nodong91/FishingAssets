using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using static Data_Manager;

public class Fishing_Action : MonoBehaviour
{
    public Data_Manager manager;

    public GameObject shipPrefab;
    public float shipSize = 1f;

    public GameObject catchPrefab;
    public Renderer catchRanderer;
    private SetStatus catchStatus;
    private float catchHealth, catchMaxHealth;

    public GameObject fishPrefab;
    private FishStruct fishStruct;
    private float fishHealth, fishMaxHealth;
    private float fishSpeed = 0f;

    [ColorUsage(true, true)]
    public Color onCatchColor, notCatchColor;
    private SetStatus defaultStatus;
    private Fishing_Canvas fishingCanvas;

    public Transform focusTarget;

    public enum FishStateType
    {
        None,
        Idle,
        Spelling,
        Attack,
        Moving,
    }
    public FishStateType State;
    public float coolTime = 10f;
    private float cooling;

    private Vector3 fishTargetPoint;
    public float spellTime = 3f;
    private bool shake;
    private bool isFishing = false;
    private bool isCatching = false;

    public CinemachinePositionComposer positionComposer;
    public float defaultCameraDistance = 15f;

    void Start()
    {
        fishingCanvas = GetComponent<Fishing_Canvas>();
        fishingCanvas.deleReStart = ReStart;

        SetStart(manager.fishStruct[0]);
    }

    void ReStart()
    {
        StopAllCoroutines();
        SetStart(manager.fishStruct[0]);
    }

    public void SetStart(FishStruct _fishStruct)
    {
        fishStruct = _fishStruct;
        isFishing = true;

        SetCatch();
        SetFish();

        fishingCanvas.SetStart(fishStruct);
        StartCoroutine(StartCount());
    }

    IEnumerator StartCount()
    {
        for (int i = 0; i < 3; i++)
        {
            int index = 3 - i;
            fishingCanvas.SetCount(index);
            yield return new WaitForSeconds(1f);
        }
        fishingCanvas.SetCount(0);
        StartCoroutine(CatchMovement());
        StartCoroutine(CheckingCatch());
        FishState(FishStateType.Idle);
    }

    void SetCatch()
    {
        SetDefaultStatus();
        catchStatus = defaultStatus;
        catchPrefab.transform.position = shipPrefab.transform.position;
        catchPrefab.transform.localScale = Vector3.one * catchStatus.catchRadius;
        catchRanderer.material.SetFloat("_Thickness", defaultStatus.catchRadius / catchStatus.catchRadius);

        catchMaxHealth = catchStatus.catchMaxHealth;
        catchHealth = catchMaxHealth;
    }

    void SetFish()
    {
        cooling = Time.time + coolTime;

        Vector3 randomPoint = Random.insideUnitSphere * fishStruct.fieldRadius;
        fishTargetPoint = new Vector3(randomPoint.x, 0f, randomPoint.z) + transform.position;
        fishPrefab.transform.position = fishTargetPoint;

        fishMaxHealth = fishStruct.fishHealth;
        fishHealth = fishMaxHealth;
    }

    void SetDefaultStatus()
    {
        defaultStatus = new SetStatus();
        defaultStatus.catchRadius = 1f;// 물고기를 잡는 범위
        defaultStatus.catchSpeed = 3f;// 낚시대가 물고기를 향해 이동하는 속도
        defaultStatus.catchPower = 1f;// 낚시대의 힘
        defaultStatus.catchMaxHealth = 10f;// 낚시대의 최대 체력
        defaultStatus.catchAttakSpeed = 1.5f;// 물고기를 공격하는 빈도

        defaultStatus.shipSpeed = 2f;// 배의 이동 속도
        defaultStatus.maxWeight = 1f;// 인벤토리 중량
        defaultStatus.maxEnergy = 10f;// 연료통 크기
        defaultStatus.efficient = 0.5f;// 에너지 효율
        defaultStatus.maxBoxSize = new Vector2Int(0, 0);// 인벤토리 크기
        defaultStatus.shipHealth = 3;// 배 체력
        defaultStatus.freshness = 1f;// 신선도 유지 - 꼭 필요한가??????  

        defaultStatus.LuckFish = 1f;// 희귀 물고기 확률
        defaultStatus.FishAmount = 1f;// 낚시 횟수 증가
        defaultStatus.FishPrice = 1f;// 판매 물고기 가격 증가
    }

    void FishState(FishStateType _state)
    {
        State = _state;
        switch (State)
        {
            case FishStateType.Idle:
                IdleState();
                break;
            case FishStateType.Spelling:
                // 시전중
                StartCoroutine(Spelling());
                break;
            case FishStateType.Attack:
                // 공격
                StartCoroutine(FishAttack());
                break;
            case FishStateType.Moving:
                MovingState();
                break;
        }
    }

    IEnumerator CatchMovement()
    {
        float catchSpeed = catchStatus.catchSpeed;
        while (isFishing == true)
        {
            Vector3 catchOffset = CatchRayCast() - catchPrefab.transform.position;
            catchSpeed = Mathf.Lerp(catchSpeed, catchStatus.catchSpeed * Mathf.Clamp01(catchOffset.magnitude), 0.1f);
            catchPrefab.transform.Translate(catchOffset.normalized * Time.deltaTime * catchSpeed, Space.World);
            focusTarget.position = Vector3.Lerp(fishPrefab.transform.position, shipPrefab.transform.position, 0.5f);

            // 캐치 영역 안에 있는지 체크
            catchOffset = fishPrefab.transform.position - catchPrefab.transform.position;
            isCatching = (catchOffset.magnitude < catchStatus.catchRadius);
            yield return null;

            catchOffset = fishPrefab.transform.position - shipPrefab.transform.position;
            positionComposer.CameraDistance = (catchOffset.magnitude * 0.5f) + defaultCameraDistance;
        }
    }

    IEnumerator CheckingCatch()
    {
        while (isFishing == true)
        {
            if (isCatching == true)
            {
                catchRanderer.material.SetColor("_Color", onCatchColor);
                fishHealth -= catchDamage;
                fishingCanvas.SetFishHP(fishHealth / fishMaxHealth);// 물고기에 데미지
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                catchRanderer.material.SetColor("_Color", notCatchColor);
                catchHealth -= fishDamage;
                fishingCanvas.SetCatchHP(catchHealth / catchMaxHealth);// 낚시에 데미지
                yield return new WaitForSeconds(0.1f);
            }
            if (catchHealth <= 0f || fishHealth <= 0f)
            {
                FishingEnd(fishHealth <= 0f);
            }
            yield return null;
        }
    }

    void FishingEnd(bool _success)
    {
        StopAllCoroutines();
        isFishing = false;
        FishState(FishStateType.None);
        fishingCanvas.SetEnd(_success);
    }

    Vector3 CatchRayCast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int downhillLayer = 1 << LayerMask.NameToLayer("Water");
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, downhillLayer))
        {
            Debug.LogWarning(hit.transform.name);
            Vector3 hitOffset = (hit.point - transform.position);
            Vector3 direction = hitOffset.normalized;
            return transform.position + direction * Mathf.Clamp(hitOffset.magnitude, 0f, fishStruct.fieldRadius);
        }
        return default;
    }
    float attackRange;
    float fishDamage = 0f;
    float catchDamage = 0f;
    void IdleState()
    {
        float distance = (shipPrefab.transform.position - fishPrefab.transform.position).magnitude;
        attackRange = shipSize + fishStruct.fieldRadius * 0.5f;
        if (cooling < Time.time && distance > attackRange)
        {
            FishState(FishStateType.Spelling);// 특수 기술
        }
        else
        {
            FishState(FishStateType.Moving);
        }
    }

    void MovingState()
    {
        fishTargetPoint = RandomPosition(out float _dist);
        if (_dist > fishStruct.fieldRadius)
        {
            StartCoroutine(FishDodge());
        }
        else
        {
            StartCoroutine(FishMoving());
        }
    }

    IEnumerator FishMoving()
    {
        float randomIndex = Random.Range(0.5f, 1f);
        float randomSpeed = Random.Range(0.5f, 1f) * fishStruct.fishSpeed;
        float randomTime = Random.Range(fishStruct.fishRange.x, fishStruct.fishRange.y) / randomSpeed;
        float normalize = 0f;
        while (normalize < randomTime)
        {
            normalize += Time.deltaTime;
            Vector3 fishOffset = (fishTargetPoint - fishPrefab.transform.position);
            Quaternion targetPoint = Quaternion.LookRotation(fishOffset.normalized);

            fishSpeed = Mathf.Lerp(fishSpeed, randomSpeed, normalize);
            fishPrefab.transform.rotation = Quaternion.Slerp(fishPrefab.transform.rotation, targetPoint, Time.deltaTime * fishStruct.fishSpeed * 0.5f);// 이동하면서 회전은 약간 느리게
            fishPrefab.transform.Translate(Vector3.forward * Time.deltaTime * fishSpeed, Space.Self);
            yield return null;
        }
        FishState(FishStateType.Idle);
    }

    IEnumerator FishDodge()// 회피기동
    {
        float skillSpeed = fishStruct.fishSpeed * 1.5f;
        while (skillSpeed > fishStruct.fishSpeed)
        {
            skillSpeed = Mathf.Lerp(skillSpeed, 0f, Time.deltaTime * 2f);
            Vector3 fishOffset = (fishTargetPoint - fishPrefab.transform.position);
            Quaternion targetPoint = Quaternion.LookRotation(fishOffset.normalized);
            fishPrefab.transform.rotation = Quaternion.Slerp(fishPrefab.transform.rotation, targetPoint, Time.deltaTime * skillSpeed);
            fishPrefab.transform.Translate(Vector3.forward * Time.deltaTime * skillSpeed, Space.Self);
            yield return null;
        }
        FishState(FishStateType.Idle);
    }

    Vector3 RandomPosition(out float _dist)
    {
        Vector3 randomPoint = Random.insideUnitSphere * fishStruct.fieldRadius;
        _dist = (fishPrefab.transform.position - randomPoint).magnitude;
        float distance = (fishPrefab.transform.position - shipPrefab.transform.position).magnitude;
        if (distance > shipSize)// 물고기와 배의 거리가 멀 때
        {
            bool finded = false;
            while (finded == false)
            {
                if (_dist < fishStruct.fieldRadius * 0.5f)
                {
                    randomPoint = Random.insideUnitSphere * fishStruct.fieldRadius;
                    _dist = (fishPrefab.transform.position - randomPoint).magnitude;

                    Vector3 center = Vector3.Lerp(fishPrefab.transform.position, randomPoint, 0.5f);// 물고기와 타겟위치의 중간
                    float toShip = (shipPrefab.transform.position - center).magnitude;// 배랑 거리 체크
                    float toTarget = (shipPrefab.transform.position - randomPoint).magnitude;
                    if (toShip > shipSize && toTarget > shipSize)
                    {
                        finded = true;
                    }
                }
            }
        }
        return new Vector3(randomPoint.x, 0f, randomPoint.z) + transform.position;
    }

    IEnumerator Spelling()
    {
        Vector3 direction = (shipPrefab.transform.position - fishPrefab.transform.position).normalized;
        float normalize = 0f;
        while (normalize < spellTime)
        {
            normalize += Time.deltaTime;
            Quaternion rotation = Quaternion.LookRotation(direction);
            fishPrefab.transform.rotation = Quaternion.Slerp(fishPrefab.transform.rotation, rotation, Time.deltaTime * 5f / spellTime);
            fishSpeed = Mathf.Lerp(fishSpeed, 0f, normalize);
            fishPrefab.transform.Translate(Vector3.forward * Time.deltaTime * fishSpeed, Space.Self);
            fishingCanvas.SetFishSpell(normalize / spellTime);
            yield return null;
        }
        fishingCanvas.SetFishSpell(0f);
        FishState(FishStateType.Attack);
    }

    IEnumerator FishAttack()// 발사
    {
        shake = false;
        float distance = (shipPrefab.transform.position - fishPrefab.transform.position).magnitude;
        float skillSpeed = fishStruct.fishSpeed * distance * 2f;
        while (skillSpeed > 0.1f)
        {
            skillSpeed = Mathf.Lerp(skillSpeed, 0f, Time.deltaTime * fishStruct.fishSpeed);
            fishPrefab.transform.Translate(Vector3.forward * Time.deltaTime * skillSpeed, Space.Self);
            distance = (shipPrefab.transform.position - fishPrefab.transform.position).magnitude;
            if (distance < shipSize && shake == false)
            {
                shake = true;
                StartCoroutine(ShakingShip());
            }
            yield return null;
        }
        cooling = Time.time + coolTime;
        FishState(FishStateType.Idle);
    }

    IEnumerator ShakingShip()
    {
        Vector3 originPosition = shipPrefab.transform.position;
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 3f;
            Vector3 shakePosition = Random.insideUnitSphere * 0.3f * (1f - normalize);
            shipPrefab.transform.position = originPosition + shakePosition;
            yield return null;
        }
        shipPrefab.transform.position = originPosition;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEditor.Handles.color = Color.yellow;
        Gizmos.DrawSphere(shipPrefab.transform.position, shipSize);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, fishStruct.fieldRadius);

        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, attackRange);

        if (catchPrefab != null && catchStatus != null)
            UnityEditor.Handles.DrawWireDisc(catchPrefab.transform.position, Vector3.up, catchStatus.catchRadius);

        Gizmos.color = Color.blue;
        if (State == FishStateType.Moving)
        {
            Gizmos.DrawSphere(fishTargetPoint, 0.3f);
            Gizmos.DrawLine(fishTargetPoint, fishPrefab.transform.position);
            float distance = (fishTargetPoint - fishPrefab.transform.position).magnitude;
            Vector3 lerfVector = Vector3.Lerp(fishPrefab.transform.position, fishTargetPoint, 0.5f);
            UnityEditor.Handles.Label(lerfVector, distance.ToString());
        }
    }
#endif
}
