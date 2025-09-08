using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using static Data_Manager;
using static Data_Manager.FishStruct;
using static Data_Quest;

public class Fishing_Manager : MonoBehaviour
{
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
    public GameObject fishingSet;
    public CanvasGroup canvasGroup;
    [Header(" [ Camera ]")]
    public CinemachinePositionComposer positionComposer;
    public float defaultCameraDistance = 15f;
    [Header(" [ Object ]")]
    public GameObject shipPrefab;
    public float shipSize = 1f;

    public GameObject catchPrefab;
    public Renderer catchRanderer;
    private SetStatus catchStatus;
    private float catchHealth, catchMaxHealth;

    public GameObject fishPrefab;
    //private FishStruct fishStruct; 
    FishStruct currentFish;
    RandomSize currentSize;
    private float fishHealth, fishMaxHealth;
    private float fishSpeed = 0f;

    [ColorUsage(true, true)]
    public Color onCatchColor, notCatchColor;
    public Fishing_Canvas fishingCanvas;

    public Transform focusTarget;

    private float cooling;
    public float coolTime = 5f;// 물고기 스탯으로 추가
    public float spellTime = 3f;// 물고기 스탯으로 추가
    public float groggyTime = 1f;// 물고기 스탯으로 추가

    private Vector3 fishTargetPoint;
    private bool shake;
    private bool isFishing = false;
    private bool isCatching = false;
    Coroutine fishAction;

    AreaType areaType;
    DayType dayType;
    Queue<FishStruct> fishQueue = new Queue<FishStruct>();// 나올 물고기 묶음
    Dictionary<string, List<FishStruct>> dictFishStruct = new Dictionary<string, List<FishStruct>>();

    public void SetStart()
    {
        SetDictionary_FishStruct();// 미리 사전 세팅

        fishingCanvas.startButton.SetButton(SetFishingStart);
        fishingCanvas.outButton.SetButton(OutFishing);
        fishingCanvas.SetStart();
    }

    void SetDictionary_FishStruct()// 사전 세팅 
    {
        Dictionary<string, FishStruct> tempDict = Singleton_Data.INSTANCE.Dict_Fish;
        foreach (var child in tempDict)
        {
            FishStruct fish = child.Value;
            string dictType = fish.areaType.ToString() + fish.fishDayType.ToString();
            if (dictFishStruct.ContainsKey(dictType))
            {
                dictFishStruct[dictType].Add(fish);
            }
            else
            {
                dictFishStruct[dictType] = new List<FishStruct> { fish };
            }
        }
    }

    public void SetFishing(AreaType _areaType)
    {
        SetCanvasGroup(0.0f);
        areaType = _areaType;

        SetFishList();// 낚시터 세팅
        SetReady(true);// 낚시 준비
    }

    void SetReady(bool _ready)
    {
        fishingSet.SetActive(true);
        Game_Manager.current.GetMainUI.timeUI.TimePause(true);// 시간 정지
        transform.position = Game_Manager.current.GetPlayer.transform.position;

        float rotateY = Camera.main.transform.rotation.eulerAngles.y;
        positionComposer.transform.rotation = Quaternion.Euler(45f, rotateY, 0f);
        positionComposer.gameObject.SetActive(true);

        // 버튼 활성화
        fishingCanvas.OnStartButton(fishQueue.Count, areaType.ToString(), dayType.ToString());

        Game_Manager.current.GetMainUI.OpenCanvas(false);
        Game_Manager.current.OutOfControll(true);
    }

    void SetFishList()
    {
        int fishingAmount = Random.Range(1, 5);// 낚시 횟수
        dayType = Game_Manager.current.GetMainUI.timeUI.lightMode;
        string cordType = areaType.ToString() + dayType.ToString();
        Debug.LogError($"낚시터 타입 : {cordType}, 낚시 횟수 : {fishingAmount}");
        // 물고기 세팅
        fishQueue.Clear();
        for (int i = 0; i < fishingAmount; i++)
        {
            FishStruct fish = TryFishStruct(cordType);
            fishQueue.Enqueue(fish);
            Debug.LogWarning($"{fish.id} : {dictFishStruct.Count}");
        }
    }

    FishStruct TryFishStruct(string _type)
    {
        if (dictFishStruct.ContainsKey(_type))
        {
            List<FishStruct> fishList = dictFishStruct[_type];
            int randomIndex = Random.Range(0, fishList.Count);
            FishStruct randomFish = fishList[randomIndex];
            return randomFish;
        }
        return default;
    }

    public void SetFishingStart()
    {
        isFishing = true;

        Option_Manager.current.SetThemeMusic("Battle");// 전투 시작
        SetCanvasGroup(1.0f);// 캔버스 오픈
        currentFish = fishQueue.Dequeue();// 물고기 정보
        currentSize = currentFish.GetRandom();
        fishingCanvas.SetFishing();

        SetCatch();// 낚시 영역 초기화
        SetFish();// 물고기 스탯 초기화

        OutReward();// 스타트 버튼
        StartCoroutine(StartCount());
    }

    void SetCanvasGroup(float _alpha)
    {
        canvasGroup.alpha = _alpha;
        canvasGroup.interactable = (_alpha > 0);
        canvasGroup.blocksRaycasts = (_alpha > 0);
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
        catchStatus = Game_Manager.current.currentStatus;
        catchPrefab.transform.position = shipPrefab.transform.position;
        catchPrefab.transform.localScale = Vector3.one * catchStatus.catchRadius;
        //catchRanderer.material.SetFloat("_Thickness", defaultStatus.catchRadius / catchStatus.catchRadius);

        catchMaxHealth = catchStatus.catchMaxHealth;
        catchHealth = catchMaxHealth;
    }

    void SetFish()
    {
        cooling = Time.time + coolTime;

        Vector3 randomPoint = Random.insideUnitSphere * currentFish.fieldRadius;
        fishTargetPoint = new Vector3(randomPoint.x, 0f, randomPoint.z) + transform.position;
        fishPrefab.transform.position = fishTargetPoint;

        fishMaxHealth = currentFish.fishHealth;
        fishHealth = fishMaxHealth;
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
    void PlayingControll()
    {
        fishingCanvas.FollowUI(fishPrefab.transform.position, catchPrefab.transform.position);
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
            float catchDistance = (fishPrefab.transform.position - catchPrefab.transform.position).magnitude;
            isCatching = (catchDistance < catchStatus.catchRadius);
            Color catchColor = isCatching ? onCatchColor : notCatchColor;
            catchRanderer.material.SetColor("_Color", catchColor);

            float fishDistance = (fishPrefab.transform.position - shipPrefab.transform.position).magnitude;
            positionComposer.CameraDistance = (fishDistance * 0.5f) + defaultCameraDistance;
            yield return null;

            PlayingControll();
        }
    }

    Vector3 CatchRayCast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int downhillLayer = 1 << LayerMask.NameToLayer("Water");
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, downhillLayer))
        {
            Vector3 hitOffset = (hit.point - transform.position);
            Vector3 direction = hitOffset.normalized;
            return transform.position + direction * Mathf.Clamp(hitOffset.magnitude, 0f, currentFish.fieldRadius);
        }
        return default;
    }

    IEnumerator CheckingCatch()
    {
        while (isFishing == true)
        {
            if (isCatching == true)
            {
                fishHealth -= catchStatus.catchPower;
                fishingCanvas.SetFishHP(fishHealth / fishMaxHealth);// 물고기에 데미지
                yield return new WaitForSeconds(currentFish.fishAttackSpeed);
            }
            else
            {
                catchHealth -= currentFish.fishPower;
                fishingCanvas.SetCatchHP(catchHealth / catchMaxHealth);// 낚시에 데미지
                yield return new WaitForSeconds(catchStatus.catchAttakSpeed);
            }
            if (catchHealth <= 0f || fishHealth <= 0f)
            {
                FishingComplate(fishHealth <= 0f);
            }
            yield return null;
        }
    }

    //===================================================================================================================
    // 물고기 상태
    //===================================================================================================================

    void IdleState()
    {
        if (cooling < Time.time)
        {
            FishState(FishStateType.Spelling);// 특수 기술
        }
        else
        {
            fishTargetPoint = SetRandomPosition();
            FishState(FishStateType.Moving);
        }
    }

    IEnumerator FishMoving()
    {
        float prevSpeed = fishSpeed;
        float distance = (fishTargetPoint - fishPrefab.transform.position).magnitude / currentFish.fieldRadius;
        float randomSpeed = distance * currentFish.fishSpeed;
        float randomTime = Random.Range(currentFish.fishRange.x, currentFish.fishRange.y) * distance;
        float normalize = 0f;
        while (normalize < randomTime)
        {
            normalize += Time.deltaTime;
            Vector3 fishOffset = (fishTargetPoint - fishPrefab.transform.position);
            Quaternion targetPoint = Quaternion.LookRotation(fishOffset.normalized);

            fishSpeed = Mathf.Lerp(prevSpeed, randomSpeed, normalize);
            fishPrefab.transform.rotation = Quaternion.Slerp(fishPrefab.transform.rotation, targetPoint, Time.deltaTime * currentFish.fishSpeed * 0.5f);// 이동하면서 회전은 약간 느리게
            fishPrefab.transform.Translate(Vector3.forward * Time.deltaTime * fishSpeed, Space.Self);
            yield return null;
        }
        FishState(FishStateType.Idle);
    }

    IEnumerator FishDodge()// 회피기동
    {
        float skillSpeed = currentFish.fishSpeed * 2f;
        while (fishSpeed > currentFish.fishSpeed)
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
        fishingCanvas.OnArrowParent(false);
        yield return null;

        FishState(FishStateType.Attack);
    }

    IEnumerator FishAttack()// 발사
    {
        shake = false;
        float distance = (shipPrefab.transform.position - fishPrefab.transform.position).magnitude;
        float skillSpeed = currentFish.fishSpeed * currentFish.fieldRadius;
        bool destroy = false;
        while (skillSpeed > 0.1f)
        {
            skillSpeed = Mathf.Lerp(skillSpeed, 0f, Time.deltaTime * currentFish.fishSpeed);
            fishPrefab.transform.Translate(Vector3.forward * Time.deltaTime * skillSpeed, Space.Self);
            distance = (shipPrefab.transform.position - fishPrefab.transform.position).magnitude;
            if (distance < shipSize && shake == false)
            {
                shake = true;
                if (shakingObject != null)
                    StopCoroutine(shakingObject);
                shakingObject = StartCoroutine(ShakingObject(shipPrefab));
                destroy = Game_Manager.current.GetPlayer.TakeDamage();// 선박에 데미지
            }
            yield return null;
        }
        // 공격 끝난 이후
        if (destroy == true)
        {
            // 파괴 됐을 때
            FishingComplate(false);
            OutFishing();

            StartCoroutine(DestroyShip());// 배 부셔져서 낚시 실패
        }
        else
        {
            cooling = Time.time + coolTime;
            FishState(FishStateType.Idle);
        }
    }

    IEnumerator DestroyShip()
    {
        Debug.LogWarning("배 부숴짐");
        yield return new WaitForSeconds(1f);
        Game_Manager.current.GetPlayer.FishingDestroy();// 배 부숴짐
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
        float randomRange = Random.Range(shipSize, currentFish.fieldRadius);
        Vector3 position = transform.position + tempAngle * randomRange;
        return position;
    }

    Vector3 DirFromAngle(float angleInDegrees)
    {
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    //===================================================================================================================
    // 흔들기
    //===================================================================================================================

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

    //===================================================================================================================
    // 방어
    //===================================================================================================================

    [Header(" [ Defense ]")]
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
                StartCoroutine(CancelState());// 캔슬 성공
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
        fishingCanvas.OnArrowParent(false);
        fishingCanvas.SetFishSpell(0f);
        FishState(FishStateType.None);

        if (shakingObject != null)
            StopCoroutine(shakingObject);
        shakingObject = StartCoroutine(ShakingObject(fishPrefab));
        yield return new WaitForSeconds(groggyTime);// 그로기 타임

        cooling = Time.time + coolTime;
        FishState(FishStateType.Idle);
    }

    //===================================================================================================================
    // 낚시 완료
    //===================================================================================================================
    void FishingComplate(bool _success)// 낚시 완료
    {
        Option_Manager.current.SetThemeMusic(null);// 테마 음악 초기화

        isFishing = false;
        StopAllCoroutines();
        FishState(FishStateType.None);

        fishingCanvas.OnArrowParent(false);
        fishingCanvas.SetFishSpell(0f);

        // 버튼 활성화
        fishingCanvas.OnStartButton(fishQueue.Count, areaType.ToString(), dayType.ToString());
        fishingCanvas.OnOutButton();

        if (_success == true)
            SetReward();
    }

    public void SetReward()
    {
        ItemStruct fishItem = currentFish.itemStruct;
        float size = currentSize.size;

        ResultStruct fishResult = new ResultStruct
        {
            inventorySize = new Vector2Int(7, 7), // 인벤토리 크기
            money = 0, // 돈
            itemID = new string[1] { fishItem.id }, // 아이템 ID
        };

        Game_Manager.current.GetInventory.SetResult(fishResult);// 퀘스트 완료 후 결과 아이템 설정
        Game_Manager.current.GetInventory.OpenResult();
        Game_Manager.current.GetFishGuide.AddFishClass(fishItem.id, size);// 생선 가이드에 추가
    }

    void OutReward()
    {
        Game_Manager.current.GetInventory.CloseResult();
    }


    //===================================================================================================================
    // 낚시 끝
    //===================================================================================================================

    void OutFishing()
    {
        Game_Manager.current.GetMainUI.timeUI.TimePause(false);// 시간 정지
        positionComposer.gameObject.SetActive(false);

        // 버튼 활성화
        fishingCanvas.OnStartButton(0);

        Game_Manager.current.GetMainUI.OpenCanvas(true);
        Game_Manager.current.OutOfControll(false);

        OutReward();// 아웃버튼

        fishingSet.SetActive(false);
        this.enabled = false;
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEditor.Handles.color = Color.yellow;
        Gizmos.DrawSphere(shipPrefab.transform.position, shipSize);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, currentFish.fieldRadius);

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
