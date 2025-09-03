using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using static Data_Manager;

public class Fishing_Action : MonoBehaviour
{
    public Data_Manager manager;
    public enum FishStateType
    {
        None,
        Idle,
        Spelling,
        Attack,
        Dodge,
        Moving,
    }
    public FishStateType fishState;

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
    public Fishing_Canvas fishingCanvas;

    public Transform focusTarget;

    public float coolTime = 10f;
    private float cooling;

    private Vector3 fishTargetPoint;
    public float spellTime = 3f;
    private bool shake;
    private bool isFishing = false;
    private bool isCatching = false;
    float fishDamage = 0.1f;
    float catchDamage = 0.1f;
    Coroutine fishAction;

    public CinemachinePositionComposer positionComposer;
    public float defaultCameraDistance = 15f;

    void Start()
    {
        //fishingCanvas = GetComponent<Fishing_Canvas>();
        fishingCanvas.deleReStart = ReStart;

        SetStart(manager.fishStruct[0]);
    }

    void Update()
    {
        fishingCanvas.FollowUI();
        if (fishState == FishStateType.Spelling)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                CancelSkill(0);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                CancelSkill(1);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                CancelSkill(2);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                CancelSkill(3);
            }
        }
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
        fishState = _state;
        if (fishAction != null)
            StopCoroutine(fishAction);
        switch (fishState)
        {
            case FishStateType.Idle:
                IdleState();
                break;
            case FishStateType.Spelling:
                // 시전중
                fishAction = StartCoroutine(Spelling());
                break;
            case FishStateType.Attack:
                // 공격
                fishAction = StartCoroutine(FishAttack());
                break;
            case FishStateType.Dodge:
                fishAction = StartCoroutine(FishDodge());
                break;
            case FishStateType.Moving:
                fishAction = StartCoroutine(FishMoving());
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

        fishingCanvas.OnArrowPrent(false);
        fishingCanvas.SetFishSpell(0f);
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

    void IdleState()
    {
        if (cooling < Time.time)
        {
            FishState(FishStateType.Spelling);// 특수 기술
        }
        else
        {
            fishTargetPoint = SetRandomPosition();
            //float distance = (fishPrefab.transform.position - fishTargetPoint).magnitude;
            //if (distance <= fishStruct.fieldRadius * 0.5f)
            //{
            //    FishState(FishStateType.Moving);
            //}
            //else
            //{
            //    FishState(FishStateType.Dodge);
            //}
            FishState(FishStateType.Moving);
        }
    }

    IEnumerator FishMoving()
    {
        float prevSpeed = fishSpeed;
        float distance = (fishTargetPoint - fishPrefab.transform.position).magnitude / fishStruct.fieldRadius;
        float randomSpeed = distance * fishStruct.fishSpeed;
        float randomTime = Random.Range(fishStruct.fishRange.x, fishStruct.fishRange.y) * distance;
        float normalize = 0f;
        while (normalize < randomTime)
        {
            normalize += Time.deltaTime;
            Vector3 fishOffset = (fishTargetPoint - fishPrefab.transform.position);
            Quaternion targetPoint = Quaternion.LookRotation(fishOffset.normalized);

            fishSpeed = Mathf.Lerp(prevSpeed, randomSpeed, normalize);
            fishPrefab.transform.rotation = Quaternion.Slerp(fishPrefab.transform.rotation, targetPoint, Time.deltaTime * fishStruct.fishSpeed * 0.5f);// 이동하면서 회전은 약간 느리게
            fishPrefab.transform.Translate(Vector3.forward * Time.deltaTime * fishSpeed, Space.Self);
            yield return null;
        }
        FishState(FishStateType.Idle);
    }

    IEnumerator FishDodge()// 회피기동
    {
        float skillSpeed = fishStruct.fishSpeed * 2f;
        while (fishSpeed > fishStruct.fishSpeed)
        {
            fishSpeed = Mathf.Lerp(skillSpeed, 0f, Time.deltaTime * 0.5f);
            Vector3 fishOffset = (fishTargetPoint - fishPrefab.transform.position);
            Quaternion targetPoint = Quaternion.LookRotation(fishOffset.normalized);
            fishPrefab.transform.rotation = Quaternion.Slerp(fishPrefab.transform.rotation, targetPoint, Time.deltaTime * fishSpeed);
            fishPrefab.transform.Translate(Vector3.forward * Time.deltaTime * fishSpeed, Space.Self);
            yield return null;
        }
        FishState(FishStateType.Idle);
    }

    IEnumerator Spelling()
    {
        SetSkillCord();

        float prevSpeed = fishSpeed;
        Vector3 direction = (shipPrefab.transform.position - fishPrefab.transform.position).normalized;
        float normalize = 0f;
        while (normalize < spellTime)
        {
            normalize += Time.deltaTime;
            Quaternion rotation = Quaternion.LookRotation(direction);
            fishPrefab.transform.rotation = Quaternion.Slerp(fishPrefab.transform.rotation, rotation, Time.deltaTime * 5f / spellTime);
            fishSpeed = Mathf.Lerp(prevSpeed, 0f, normalize);
            fishPrefab.transform.Translate(Vector3.forward * Time.deltaTime * fishSpeed, Space.Self);
            fishingCanvas.SetFishSpell(normalize / spellTime);
            yield return null;
        }
        fishingCanvas.SetFishSpell(0f);
        fishingCanvas.OnArrowPrent(false);
        yield return null;

        FishState(FishStateType.Attack);
    }

    IEnumerator FishAttack()// 발사
    {
        shake = false;
        float distance = (shipPrefab.transform.position - fishPrefab.transform.position).magnitude;
        float skillSpeed = fishStruct.fishSpeed * fishStruct.fieldRadius;
        while (skillSpeed > 0.1f)
        {
            skillSpeed = Mathf.Lerp(skillSpeed, 0f, Time.deltaTime * fishStruct.fishSpeed);
            fishPrefab.transform.Translate(Vector3.forward * Time.deltaTime * skillSpeed, Space.Self);
            distance = (shipPrefab.transform.position - fishPrefab.transform.position).magnitude;
            if (distance < shipSize && shake == false)
            {
                shake = true;
                if (shakingObject != null)
                    StopCoroutine(shakingObject);
                shakingObject = StartCoroutine(ShakingObject(shipPrefab));
            }
            yield return null;
        }
        cooling = Time.time + coolTime;
        FishState(FishStateType.Idle);
    }

    Coroutine shakingObject;
    IEnumerator ShakingObject(GameObject _object)
    {
        Vector3 originPosition = _object.transform.position;
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 3f;
            Vector3 shakePosition = Random.insideUnitSphere * 0.3f * (1f - normalize);
            _object.transform.position = originPosition + shakePosition;
            yield return null;
        }
        _object.transform.position = originPosition;
    }

    Vector3 SetRandomPosition()
    {
        float currentAngle;
        if (Vector3.Angle(transform.right, fishPrefab.transform.position - shipPrefab.transform.position) > 90f)
        {
            // 왼쪽
            currentAngle = 360f - Vector3.Angle(transform.forward, fishPrefab.transform.position);
        }
        else
        {
            // 오른쪽
            currentAngle = Vector3.Angle(transform.forward, fishPrefab.transform.position);
        }
        float minMaxAngle = Random.Range(45f, 120f);
        int randomIndex = Random.Range((int)0, (int)2) > 0 ? -1 : 1;
        float randomAngle = minMaxAngle * randomIndex + currentAngle;
        Vector3 tempAngle = DirFromAngle(randomAngle);
        float randomRange = Random.Range(shipSize, fishStruct.fieldRadius);
        Vector3 position = transform.position + tempAngle * randomRange;
        return position;
    }

    Vector3 DirFromAngle(float angleInDegrees)
    {
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }















    public string skillCord;
    public int currentIndex = 0;
    public int cordCount;

    void SetSkillCord()
    {
        fishState = FishStateType.Spelling;
        currentIndex = 0;
        skillCord = "";
        cordCount = Random.Range(4, 7);
        for (int i = 0; i < cordCount; i++)
        {
            int cord = Random.Range(0, 4);
            skillCord += cord.ToString();
        }
        fishingCanvas.SetArrow(skillCord);
    }

    void CancelSkill(int _index)
    {
        string cord = skillCord[currentIndex].ToString();
        if (cord == _index.ToString())
        {
            fishingCanvas.OnArrow(currentIndex, 1f);
            currentIndex++;
            if (cordCount == currentIndex)
            {
                // 완료
                StartCoroutine(CancelState());
            }
        }
        else
        {
            currentIndex = 0;
            fishingCanvas.OffArrowAll();
        }
    }

    IEnumerator CancelState()
    {
        fishingCanvas.OnArrowPrent(false);
        fishingCanvas.SetFishSpell(0f);
        FishState(FishStateType.None);

        if (shakingObject != null)
            StopCoroutine(shakingObject);
        shakingObject = StartCoroutine(ShakingObject(fishPrefab));
        yield return new WaitForSeconds(1f);

        cooling = Time.time + coolTime;
        FishState(FishStateType.Idle);
    }











#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEditor.Handles.color = Color.yellow;
        Gizmos.DrawSphere(shipPrefab.transform.position, shipSize);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, fishStruct.fieldRadius);

        UnityEditor.Handles.color = Color.red;
        if (catchPrefab != null && catchStatus != null)
            UnityEditor.Handles.DrawWireDisc(catchPrefab.transform.position, Vector3.up, catchStatus.catchRadius);

        Gizmos.color = fishState == FishStateType.Dodge ? Color.blue : Color.yellow;
        if (fishState == FishStateType.Dodge || fishState == FishStateType.Moving)
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
