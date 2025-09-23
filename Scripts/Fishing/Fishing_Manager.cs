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
    [Header(" [ Camera ]")]
    public CinemachinePositionComposer positionComposer;
    public float defaultCameraDistance = 15f;
    [Header(" [ Object ]")]
    public GameObject shipPrefab;
    public float shipSize = 1f;

    private const float fieldRadius = 15f;
    public GameObject catchPrefab;
    public Renderer catchRanderer;
    private SetStatus catchStatus;

    FishStruct currentFish;
    RandomSize currentSize;
    public GameObject fishPrefab;
    private float fishHealth = 0f;
    private float fishSpeed = 0f;
    private float cooling;

    [ColorUsage(true, true)]
    public Color onCatchColor, notCatchColor;
    public Fishing_Canvas fishingCanvas;

    public Transform focusTarget;

    private Vector3 fishTargetPoint;
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
        cinemachineBasicMultiChannelPerlin = positionComposer.GetComponent<CinemachineBasicMultiChannelPerlin>();

        fishingCanvas.startButton.SetButton(FishingStartButton);
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

    public void SetFishing(AreaType _areaType)// 트리거 닿았을 때 낚시 시작
    {
        areaType = _areaType;
        fishingCanvas.SetCount(0);// 카운트 제거

        SetFishList();// 낚시터 세팅
        SetReady(true);// 낚시 준비
    }

    void SetFishList()
    {
        int addSkillAmount = Game_Manager.current.currentStatus.FishAmount;// 낚시 스킬로 추가 횟수
        int fishingAmount = Random.Range(1, 5) + addSkillAmount;// 낚시 횟수
        dayType = Game_Manager.current.GetMainUI.timeUI.lightMode;
        string cordType = areaType.ToString() + dayType.ToString();
        // 물고기 세팅
        fishQueue.Clear();
        while (fishingAmount > fishQueue.Count)
        {
            if (TryFishStruct(cordType, out FishStruct _fish) == true)
            {
                fishQueue.Enqueue(_fish);
                Debug.LogWarning($"{_fish.id} : {dictFishStruct.Count}");
            }
        }
    }

    bool TryFishStruct(string _type, out FishStruct _fish)// 해당 타입 물고기 중 랜덤
    {
        if (dictFishStruct.ContainsKey(_type))
        {
            List<FishStruct> fishList = dictFishStruct[_type];// 해당 타입 물고기 리스트
            FishStruct randomFish = FishingChance(fishList);
            _fish = randomFish;
            return true;
        }
        Debug.LogError($"[Fishing_Manager] 해당 타입 물고기 없음 : {_type}");
        _fish = default;
        return false;
    }

    FishStruct FishingChance(List<FishStruct> _fishList)// 확률로 물고기 선택
    {
        float total = 0;
        for (int i = 0; i < _fishList.Count; i++)
        {
            float fishProbability = GetProbability(_fishList[i].itemStruct.itemClass);
            total += fishProbability;
        }

        float randomPoint = Random.value * total;
        for (int i = 0; i < _fishList.Count; i++)
        {
            float fishProbability = GetProbability(_fishList[i].itemStruct.itemClass);
            if (randomPoint < fishProbability)
            {
                return _fishList[i];
            }
            else
            {
                randomPoint -= fishProbability;
            }
        }
        return _fishList[^1];
    }

    float GetProbability(ItemStruct.ItemClass _class)
    {
        float skillIndex = Game_Manager.current.currentStatus.LuckFish;
        // 물고기 클래스별 확률
        return _class switch
        {
            // 필요한 물고기가 안나올 확률????? - 낮은 클래스 물고기 안나올 수 있음
            // 미끼로 확률 조정하는게???
            ItemStruct.ItemClass.Common => 0.6f + (1f - 0.6f) * 0.5f * skillIndex,
            ItemStruct.ItemClass.Uncommon => 0.25f + (1f - 0.25f) * 0.5f * skillIndex,
            ItemStruct.ItemClass.Rare => 0.1f,
            ItemStruct.ItemClass.Epic => 0.04f,
            ItemStruct.ItemClass.Legendary => 0.01f,
            _ => 0f,
        };
    }

    void SetReady(bool _ready)// 준비
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

    public void FishingStartButton()// 시작 버튼
    {
        isFishing = true;

        Option_Manager.current.SetThemeMusic("Battle");// 전투 시작

        currentFish = fishQueue.Dequeue();// 물고기 정보
        tempText.text = currentFish.itemStruct.name;
        currentSize = currentFish.GetRandom();
        fishingCanvas.SetFishing();

        SetCatch();// 낚시 영역 초기화
        SetFish();// 물고기 스탯 초기화

        OutReward();// 보상 창이 열려있는 경우 스타트 버튼
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
        fishingCanvas.SetCount(0);// 카운트 완료
        fishingCanvas.SetFishUI();
        yield return null;

        StartCoroutine(CatchMovement());
        StartCoroutine(CheckingCatch());

        FishState(FishStateType.Idle);
    }

    void SetCatch()
    {
        catchStatus = Game_Manager.current.currentStatus;
        catchPrefab.transform.position = shipPrefab.transform.position;
        catchPrefab.transform.localScale = Vector3.one * catchStatus.catchRadius;
    }

    void SetFish()
    {
        cooling = Time.time + currentFish.fishCoolTime;

        Vector3 randomPoint = Random.insideUnitSphere * fieldRadius;
        fishTargetPoint = new Vector3(randomPoint.x, 0f, randomPoint.z) + transform.position;
        fishPrefab.transform.position = fishTargetPoint;

        fishHealth = currentFish.fishHealth / (catchStatus.catchMaxHealth + currentFish.fishHealth);
        fishingCanvas.SetFishHP(fishHealth);
        Debug.LogWarning($"SetReady 게이지 : {currentFish.fishHealth} / ({catchStatus.catchMaxHealth} + {currentFish.fishHealth}) = {fishHealth}");
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
        fishingCanvas.FollowUI(fishPrefab.transform.position);
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
            return transform.position + direction * Mathf.Clamp(hitOffset.magnitude, 0f, fieldRadius);
        }
        return default;
    }

    IEnumerator CheckingCatch()
    {
        while (isFishing == true)
        {
            if (isCatching == true)
            {
                bool critical = Random.Range(0f, 1f) > 0.5f;
                // 낚시대 힘만큼 데미지
                float setDamage = critical ? catchStatus.catchPower + catchStatus.catchPower * 0.2f : catchStatus.catchPower;
                float damage = setDamage / (currentFish.fishHealth + catchStatus.catchMaxHealth);
                fishHealth -= damage;

                fishingCanvas.SetDamage(setDamage, critical);
                yield return new WaitForSeconds(currentFish.fishAttackSpeed);
            }
            else
            {
                // 물고기 힘만큼 힐
                fishHealth += currentFish.fishPower / (currentFish.fishHealth + catchStatus.catchMaxHealth);
                yield return new WaitForSeconds(catchStatus.catchAttakSpeed);
            }
            fishingCanvas.SetFishHP(fishHealth);

            if (fishHealth >= 1f || fishHealth <= 0f)
            {
                FishingComplate(fishHealth <= 0f);// 낚시 성공 실패
            }
            yield return null;
        }
    }

    //===================================================================================================================
    // 물고기 상태
    //===================================================================================================================

    void IdleState()
    {
        if (currentFish.fishCoolTime > 0f && cooling < Time.time)
        {
            // 쿨타임이 0인 경우 공격하지 않음
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
        float distance = (fishTargetPoint - fishPrefab.transform.position).magnitude / fieldRadius;
        float randomSpeed = distance * currentFish.fishSpeed;
        float randomTime = Random.Range(currentFish.fishTurnDelay.x, currentFish.fishTurnDelay.y) * distance;
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

        float normalize = 0f;
        while (normalize < currentFish.fishSpellTime)
        {
            normalize += Time.deltaTime / currentFish.fishSpellTime;
            Vector3 direction = (shipPrefab.transform.position - fishPrefab.transform.position);
            Quaternion rotation = Quaternion.LookRotation(direction.normalized);
            fishPrefab.transform.rotation = Quaternion.Slerp(fishPrefab.transform.rotation, rotation, Time.deltaTime * currentFish.fishSpellTime);
            fishSpeed = Mathf.Lerp(prevSpeed, 0f, normalize);// 서시히 정지
            fishPrefab.transform.Translate(Vector3.forward * Time.deltaTime * fishSpeed, Space.Self);
            fishingCanvas.SetFishSpell(normalize / currentFish.fishSpellTime);
            yield return null;
        }
        fishingCanvas.SetFishSpell(0f);
        fishingCanvas.OnArrowParent(false);
        yield return null;

        FishState(FishStateType.Attack);
    }
    public TMPro.TMP_Text tempText;
    IEnumerator FishAttack()// 발사
    {
        //float skillSpeed = currentFish.fishSpeed * fieldRadius;
        //float skillSpeed = fieldRadius;
        bool damaged = false;
        bool destroy = false;
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime;
            float skillSpeed = Mathf.Lerp(fieldRadius * 2f, 0f, normalize);
            fishPrefab.transform.Translate(Vector3.forward * Time.deltaTime * skillSpeed, Space.Self);// 이동
            float distance = (shipPrefab.transform.position - fishPrefab.transform.position).magnitude;
            if (distance < shipSize && damaged == false)
            {
                damaged = true;
                SetShaking();
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
            cooling = Time.time + currentFish.fishCoolTime;
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
        float randomRange = Random.Range(shipSize, fieldRadius);
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
    CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
    float shakeTime = 1f, shakeValue = 5f;

    void SetShaking()
    {
        if (shakingObject != null)
            StopCoroutine(shakingObject);
        shakingObject = StartCoroutine(Shaking());
    }

    IEnumerator Shaking()
    {
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * (1f / shakeTime);
            float shakeAmount = Mathf.Lerp(shakeValue, 0f, normalize);
            cinemachineBasicMultiChannelPerlin.AmplitudeGain = shakeAmount;
            yield return null;
        }
    }

    //===================================================================================================================
    // 방어
    //===================================================================================================================

    private string skillCord;
    private int currentIndex = 0;
    private int cordCount;

    void SetSkillCord()
    {
        fishState = FishStateType.Spelling;
        currentIndex = 0;
        skillCord = string.Empty;
        cordCount = currentFish.fishDefenseCount;
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
            fishingCanvas.OnArrow(currentIndex, true);// 입력 성공
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
            fishingCanvas.InputFail();// 입력 실패
        }
    }
    // 방어 성공
    IEnumerator CancelState()
    {
        fishingCanvas.OnArrowParent(false);
        fishingCanvas.SetFishSpell(0f);
        FishState(FishStateType.None);

        SetShaking();
        yield return new WaitForSeconds(currentFish.fishGroggyTime);// 그로기 타임

        cooling = Time.time + currentFish.fishCoolTime;
        FishState(FishStateType.Idle);
    }

    //===================================================================================================================
    // 낚시 완료
    //===================================================================================================================
    void FishingComplate(bool _success)// 낚시 완료
    {
        Debug.LogWarning($"FishingComplate : {_success}");
        Option_Manager.current.SetThemeMusic(null);// 테마 음악 초기화

        isFishing = false;
        StopAllCoroutines();
        FishState(FishStateType.None);

        fishingCanvas.OnArrowParent(false);
        fishingCanvas.SetFishSpell(0f);

        // 버튼 활성화
        fishingCanvas.OnStartButton(fishQueue.Count, areaType.ToString(), dayType.ToString());// 스타트 버튼 활성화
        fishingCanvas.FishingOver();// 낚시 유아이 제거

        if (_success == true)// 낚시 성공
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
        Game_Manager.current.GetFishGuide.AddFishClass(fishItem.id, size);// 생선 가이드에 추가
        Game_Manager.current.GetInventory.OpenResult();
    }

    void OutReward()
    {
        // 보상 닫기
        Game_Manager.current.GetInventory.CloseResult();
    }


    //===================================================================================================================
    // 낚시 끝
    //===================================================================================================================

    void OutFishing()
    {
        Game_Manager.current.GetMainUI.timeUI.TimePause(false);// 시간 정지 종료
        positionComposer.gameObject.SetActive(false);

        // 스타트 버튼 비활성화
        fishingCanvas.OnStartButton(0);

        Game_Manager.current.GetMainUI.OpenCanvas(true);
        Game_Manager.current.OutOfControll(false);

        OutReward();// 아웃버튼

        fishingSet.SetActive(false);// 물고기, 낚시대 제거
        fishingCanvas.outButton.gameObject.SetActive(false);
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEditor.Handles.color = Color.yellow;
        Gizmos.DrawSphere(shipPrefab.transform.position, shipSize);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, fieldRadius);

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
