using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using static Data_Manager;

public class Fishing_Manager : MonoBehaviour
{
    public bool immortal;
    public bool spelling;
    public Fishing_Canvas fishingCanvas;
    public Fishing_Fish fishingFish;
    [ColorUsage(true, true)]
    public Color onCatchColor, notCatchColor;
    //public enum FishStateType
    //{
    //    None,
    //    Idle,
    //    Spelling,
    //    Attack,
    //    Dodge,
    //    Moving,
    //}
    //public FishStateType fishState;

    [Header(" [ Camera ]")]
    public CinemachinePositionComposer positionComposer;
    const float defaultCameraDistance = 25f;

    [Header(" [ Object ]")]
    public GameObject fishingSet;
    public GameObject catchaText, failText;
    public GameObject shipPrefab;

    private float FieldRadius => fishingFish.GetFieldRadius;

    public GameObject catchPrefab;
    public Renderer catchRanderer;
    private SetStatus catchStatus;
    float catchRadius = 1f;// 지름이 클수록 데미지가 약해짐 (줄을 느슨하게)

    FishStruct currentFish;

    private float fishHealth = 0f;

    public Transform focusTarget;
    private Vector3 fishTargetPoint;
    private bool isFishing = false;
    private bool isCatching = false;
    public GameObject fishPrefab => fishingFish.fishAgent.gameObject;
    AreaType GetAreaType => Game_Manager.current.GetMainUI.GetAreaType;
    DayType dayType => Game_Manager.current.GetMainUI.timeUI.lightMode;
    Dictionary<string, List<FishStruct>> dictFishStruct = new Dictionary<string, List<FishStruct>>();
    Queue<string> fishQueue = new Queue<string>();
    public TMPro.TMP_Text debugText;

    bool catching;

    public void SetStart()
    {
        SetDictionary_FishStruct();// 미리 사전 세팅
        SetFishingDelegate();
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
            string dictType = fish.areaType.ToString() + fish.fishDayType.ToString();// 키 값 설정 -> 지역 타입 + 낮,밤
            if (dictFishStruct.ContainsKey(dictType))
            {
                dictFishStruct[dictType].Add(fish);
            }
            else
            {
                dictFishStruct[dictType] = new List<FishStruct> { fish };
            }
        }
        fishingSet.SetActive(false);

        bool isCompleted = Tutorial_Manager.current.IsTutorialCompleted(Const_Tutorial._firstFishing);
        if (isCompleted == false)
        {
            Tutorial_Manager.current.CompletedTutorial(Const_Tutorial._firstFishing);// 튜토완료
            Debug.LogWarning("낚시가 처음인지 확인 - 튜토리얼 시작");
            Tutorial_Manager.current.FishingTutorial();// 낚시 튜토리얼 시작
        }
    }

    //===================================================================================================================
    // 물고기 세팅
    //===================================================================================================================

    public void StartFishing_Trigger()
    {
        int addSkillAmount = Game_Manager.current.currentStatus.fishAmount;// 낚시 스킬로 추가 횟수
        int fishingAmount = Random.Range(1, 5) + addSkillAmount;// 낚시 횟수

        fishQueue.Clear();
        for (int i = 0; i < fishingAmount; i++)
        {
            // 물고기 세팅
            FishStruct fishStruct = GetFishStruct();// 낚시터 세팅
            fishQueue.Enqueue(fishStruct.id);
        }
        SetReady(true);// 낚시 준비
    }

    FishStruct GetFishStruct()// 해당 타입 물고기 중 랜덤
    {
        string cordType = GetAreaType.ToString() + dayType.ToString();
        if (dictFishStruct.ContainsKey(cordType))
        {
            List<FishStruct> fishList = dictFishStruct[cordType];// 해당 타입 물고기 리스트
            FishStruct randomFish = FishingChance(fishList);// 물고기 확률로 선택
            return randomFish;
        }
        Debug.LogError($"[Fishing_Manager] 해당 타입 물고기 없음 : {cordType}");
        FishStruct defaultFish = Singleton_Data.INSTANCE.Dict_Fish["fs_1001"];// 기본 물고기
        return defaultFish;
    }

    FishStruct FishingChance(List<FishStruct> _fishList)// 확률로 물고기 선택
    {
        List<float> _floatList = new List<float>();
        float total = 0;
        for (int i = 0; i < _fishList.Count; i++)
        {
            ItemStruct.ItemClass itemClass = _fishList[i].itemStruct.itemClass;
            float fishProbability = GetProbability(itemClass) + GetAddBuff(itemClass);
            _floatList.Add(fishProbability);
            total += fishProbability;
        }

        float randomPoint = Random.value * total;
        for (int i = 0; i < _floatList.Count; i++)
        {
            if (randomPoint < _floatList[i])
            {
                return _fishList[i];
            }
            else
            {
                randomPoint -= _floatList[i];
            }
        }
        return _fishList[^1];
    }

    float GetAddBuff(ItemStruct.ItemClass _class)
    {
        float addValue = 0f;
        Game_Manager.FishBuffStruct fishBuff = Game_Manager.current.GetFishBuff;// 낚시 버프 적용
        if (fishBuff != null && fishBuff.itemClass == _class)
        {
            addValue = fishBuff.addValue;
        }
        return addValue;
    }

    float GetProbability(ItemStruct.ItemClass _class)// 물고기 클래스별 확률
    {
        // 물고기 클래스별 확률
        float classValue = _class switch
        {
            // 필요한 물고기가 안나올 확률????? - 낮은 클래스 물고기 안나올 수 있음
            // 미끼가 특정 클래스의 확률을 조정
            ItemStruct.ItemClass.Common => 1.0f,
            ItemStruct.ItemClass.Uncommon => 0.25f,
            ItemStruct.ItemClass.Rare => 0.1f,
            ItemStruct.ItemClass.Epic => 0.04f,
            ItemStruct.ItemClass.Legendary => 0.01f,
            _ => 0f,
        };
        return classValue;
    }

    //===================================================================================================================
    // 준비
    //===================================================================================================================

    void SetReady(bool _ready)// 준비
    {
        Debug.LogWarning("낚시 시작");
        transform.position = Game_Manager.current.GetPlayer.transform.position;

        float rotateY = Camera.main.transform.rotation.eulerAngles.y;
        positionComposer.transform.rotation = Quaternion.Euler(45f, rotateY, 0f);
        positionComposer.gameObject.SetActive(true);

        // 버튼 활성화
        CheckFishQueue();
        Game_Manager.current.GetMainUI.OpenCanvas(false);// 낚시 시작 MainUI 제거
        Game_Manager.current.OutOfControll(true);
    }

    public void CheckFishQueue()
    {
        // 버튼 활성화
        fishingCanvas.OnStartButton(fishQueue.Count, GetAreaType.ToString(), dayType.ToString());// 스타트 버튼 활성화
    }

    //===================================================================================================================
    // 시작
    //===================================================================================================================

    void FishingStartButton()// 시작 버튼
    {
        isFishing = true;

        string _id = fishQueue.Dequeue();
        currentFish = SetCurrentFish(_id);
        fishingCanvas.SetFishing();
        Debug.LogWarning($"물고기 이름 : {_id}");
        StartCoroutine(StartCount());
    }

    FishStruct SetCurrentFish(string _id)// 스킬 적용 스탯
    {
        FishStruct findFish = Singleton_Data.INSTANCE.Dict_Fish[_id];
        FishStatus addStruct = Game_Manager.current.GetSkill.skill_Setting.AddFishStatus();
        FishStruct addFishStatus = new FishStruct
        {
            itemStruct = findFish.itemStruct,
            areaType = findFish.areaType,
            fishDayType = findFish.fishDayType,
            size = findFish.size,
            id = findFish.id,
            // 스킬 적용 스탯
            fishHealth = findFish.fishHealth + addStruct.fishHealth,
            fishPower = findFish.fishPower + addStruct.fishPower,
            fishSpeed = findFish.fishSpeed + addStruct.fishSpeed,
            fishCoolTime = findFish.fishCoolTime + addStruct.fishCoolTime,
            fishSpellTime = findFish.fishSpellTime + addStruct.fishSpellTime,
            fishGroggyTime = findFish.fishGroggyTime + addStruct.fishGroggyTime,
            fishDefenseCount = findFish.fishDefenseCount + addStruct.fishDefenseCount,
            fishLazy = findFish.fishLazy + addStruct.fishLazy,
            addDuration = findFish.addDuration + addStruct.addDuration,
            addValue = findFish.addValue + addStruct.addValue
        };
        return addFishStatus;
    }

    void SetFishing()// 초기 세팅
    {
        catchStatus = Game_Manager.current.currentStatus;

        Vector3 randomPoint = Random.insideUnitSphere * FieldRadius;
        fishTargetPoint = new Vector3(randomPoint.x, 0f, randomPoint.z) + transform.position;
        fishingFish.SetFish(fishTargetPoint);

        fishHealth = catchStatus.catchMaxHealth / (currentFish.fishHealth + catchStatus.catchMaxHealth);
        fishingCanvas.SetFishHP(fishHealth);

        catchPrefab.transform.position = fishTargetPoint;
        catchRadius = catchStatus.catchRadius;
        catchPrefab.transform.localScale = Vector3.one * catchRadius;
    }

    IEnumerator StartCount()// 카운트
    {
        SetFishing();// 낚시 초기화
        yield return null;

        for (int i = 0; i < 3; i++)
        {
            int index = 3 - i;
            fishingCanvas.SetCount(index);
            Singleton_Audio.INSTANCE.Audio_FX(Const_Audio._countDown);
            yield return new WaitForSeconds(1f);
        }
        fishingCanvas.SetCount(0);// 카운트 완료
        fishingCanvas.SetFishUI();

        //float fishDistance = (fishPrefab.transform.position - shipPrefab.transform.position).magnitude;
        //positionComposer.CameraDistance = (fishDistance * 0.5f) + defaultCameraDistance;
        yield return null;

        //SetCooling();

        fishingSet.SetActive(true);
        StartCoroutine(CatchMovement());
        //FishState(FishStateType.Idle);
        // 물고기 움직이기
        fishingFish.SetStart(currentFish);
    }

    IEnumerator CatchMovement()
    {
        float catchSpeed = catchStatus.catchSpeed * 0.1f;
        while (isFishing == true)
        {
            //Vector3 catchOffset = CatchRayCast() - catchPrefab.transform.position;
            //catchSpeed = Mathf.Lerp(catchSpeed, catchStatus.catchSpeed * Mathf.Clamp01(catchOffset.magnitude), 0.1f * Time.deltaTime);
            //catchPrefab.transform.Translate(catchOffset.normalized * catchSpeed, Space.World);
            catchPrefab.transform.position = Vector3.Lerp(catchPrefab.transform.position, CatchRayCast(), catchSpeed * Time.deltaTime);
            MoveCatch();
            yield return null;

            fishingCanvas.FollowUI(fishPrefab.transform.position);
            DefenseControll();
            FishingControll();

            if (immortal == false)
            {
                CheckingCatch();
                //// 낚시줄 텐셭 체크
                //if (fishingCanvas.TryTention == true)
                //{
                //    FishingComplate(false);// 1초이상 팽팽하게 당기면 끊어짐
                //}
            }
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
            return transform.position + direction * Mathf.Clamp(hitOffset.magnitude, 0f, FieldRadius);
        }
        return default;
    }

    void MoveCatch()
    {
        // 카메라 포커스 위치
        focusTarget.position = Vector3.Lerp(catchPrefab.transform.position, shipPrefab.transform.position, 0.5f);// 카메라 포커스

        // 캐치 영역 안에 있는지 체크
        Vector2 fishPosition = new Vector2(fishPrefab.transform.position.x, fishPrefab.transform.position.z);
        Vector2 catchPosition = new Vector2(catchPrefab.transform.position.x, catchPrefab.transform.position.z);
        float catchDistance = (fishPosition - catchPosition).magnitude;
        isCatching = (catchDistance < catchRadius);// 영역 안에 있는지 체크
        Color catchColor = isCatching == true ? onCatchColor : notCatchColor;
        catchRanderer.material.SetColor("_Color", catchColor);
        fishingCanvas.ReelTention(catchDistance / catchRadius);// 텐션UI 표시

        float fishDistance = (fishPrefab.transform.position - shipPrefab.transform.position).magnitude;
        positionComposer.CameraDistance = Mathf.Lerp(positionComposer.CameraDistance, (fishDistance * 0.5f) + defaultCameraDistance, 0.1f);
    }

    void CheckingCatch()
    {
        if (isCatching == true)
        {
            if (catching == true)
            {
                // 낚시대 힘만큼 데미지
                float damage = catchStatus.catchPower / (currentFish.fishHealth + catchStatus.catchMaxHealth);
                fishHealth += damage * Time.deltaTime;
                fishingCanvas.SetFishIcon(-1f);
            }
        }
        else
        {
            float addDamage = (catching == true) ? 2f : 1f;// 영역외에 계속 누르고 있으면 두배로 깎음
            // 물고기 힘만큼 데미지
            float damage = currentFish.fishPower * addDamage / (currentFish.fishHealth + catchStatus.catchMaxHealth);
            fishHealth -= damage * Time.deltaTime;
            fishingCanvas.SetFishIcon(1f);
        }

        fishingCanvas.SetFishHP(fishHealth);
        if (fishHealth >= 1f || fishHealth <= 0f)
        {
            FishingComplate(fishHealth >= 1f);// 낚시 성공 실패
        }
    }

    //===================================================================================================================
    // 컨트롤
    //===================================================================================================================

    void FishingControll()
    {
        if (Input.GetMouseButton(0))
        {
            if (catching == false)
            {
                catching = true;
                Singleton_Audio.INSTANCE.Audio_LoopFX(Const_Audio._reeling);// 낚시 소리
            }
            if (catchRadius > 0.1f)
                catchRadius -= 0.5f * Time.deltaTime;
        }
        else if (catchRadius <= catchStatus.catchRadius)
        {
            if (catching == true)
            {
                catching = false;
                Singleton_Audio.INSTANCE.Stop_LoopFX();// 낚시 소리
            }
            catchRadius += 1f * Time.deltaTime;
        }
        catchPrefab.transform.localScale = Vector3.one * catchRadius;
    }

    void DefenseControll()
    {
        if (spelling == true)
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


    //===================================================================================================================
    // 물고기 상태
    //===================================================================================================================

    //void FishState(FishStateType _state)
    //{
    //    fishState = _state;

    //    if (fishAction != null)
    //        StopCoroutine(fishAction);
    //    debugText.text = $"{currentFish.itemStruct.name} : {fishState}";

    //    switch (fishState)
    //    {
    //        case FishStateType.Idle:
    //            fishAction = StartCoroutine(IdleState());
    //            break;
    //        case FishStateType.Spelling:
    //            // 시전중
    //            fishAction = StartCoroutine(Spelling());
    //            break;
    //        case FishStateType.Attack:
    //            // 공격
    //            fishAction = StartCoroutine(FishAttack());
    //            break;
    //        case FishStateType.Dodge:
    //            fishAction = StartCoroutine(FishDodge());
    //            break;
    //        case FishStateType.Moving:
    //            fishAction = StartCoroutine(FishMoving());
    //            break;
    //    }
    //}

    //IEnumerator IdleState()
    //{
    //    while (fishState == FishStateType.Idle)
    //    {
    //        Debug.LogWarning($"{currentFish.id} : {currentFish.fishCoolTime}({cooling} < {Time.time})");
    //        if (currentFish.fishCoolTime > 0f && currentFish.fishDefenseCount > 0 && cooling < Time.time)
    //        {
    //            // 쿨타임이 0인 경우 공격하지 않음 디펜스 개수가 0보다 커야
    //            FishState(FishStateType.Spelling);// 스킬 기술
    //        }
    //        else// 이동 능력이 없으면 이동 스테이트에 들어갈 수 없음
    //        if (currentFish.fishSpeed > 0 && currentFish.fishTurnDelay.x + currentFish.fishTurnDelay.y > 0)
    //        {
    //            FishState(FishStateType.Moving);
    //        }
    //        else
    //        {
    //            float randomTime = Random.Range(currentFish.fishTurnDelay.x, currentFish.fishTurnDelay.y);
    //            yield return new WaitForSeconds(randomTime);
    //        }
    //        yield return null;
    //    }
    //}

    //IEnumerator FishMoving()
    //{
    //    fishTargetPoint = SetRandomPosition();

    //    float prevSpeed = fishSpeed;
    //    float distance = (fishTargetPoint - fishPrefab.transform.position).magnitude / fieldRadius;
    //    float randomSpeed = Random.Range(currentFish.fishSpeed * 0.3f, currentFish.fishSpeed);
    //    float randomTime = Random.Range(currentFish.fishTurnDelay.x, currentFish.fishTurnDelay.y) * distance;
    //    float normalize = 0f;
    //    while (normalize < randomTime)
    //    {
    //        normalize += Time.deltaTime;
    //        Vector3 fishOffset = (fishTargetPoint - fishPrefab.transform.position);
    //        Quaternion targetPoint = Quaternion.LookRotation(fishOffset.normalized);

    //        fishSpeed = Mathf.Lerp(prevSpeed, randomSpeed, normalize);
    //        fishPrefab.transform.rotation = Quaternion.Slerp(fishPrefab.transform.rotation, targetPoint, Time.deltaTime * currentFish.fishSpeed * 0.5f);// 이동하면서 회전은 약간 느리게
    //        fishPrefab.transform.Translate(Vector3.forward * Time.deltaTime * fishSpeed, Space.Self);
    //        yield return null;
    //    }
    //    FishState(FishStateType.Idle);
    //}

    //IEnumerator FishDodge()// 회피기동
    //{
    //    float skillSpeed = currentFish.fishSpeed * 2f;
    //    while (fishSpeed > currentFish.fishSpeed)
    //    {
    //        fishSpeed = Mathf.Lerp(skillSpeed, 0f, Time.deltaTime * 0.5f);
    //        Vector3 fishOffset = (fishTargetPoint - fishPrefab.transform.position);
    //        Quaternion targetPoint = Quaternion.LookRotation(fishOffset.normalized);
    //        fishPrefab.transform.rotation = Quaternion.Slerp(fishPrefab.transform.rotation, targetPoint, Time.deltaTime * fishSpeed);
    //        fishPrefab.transform.Translate(Vector3.forward * Time.deltaTime * fishSpeed, Space.Self);
    //        yield return null;
    //    }
    //    FishState(FishStateType.Idle);
    //}

    //IEnumerator Spelling()
    //{
    //    SetSkillCord();

    //    Vector3 targetPosition = shipPrefab.transform.position + (fishPrefab.transform.position - shipPrefab.transform.position).normalized * (shipSize + 1f);
    //    float normalize = 0f;
    //    while (fishState == FishStateType.Spelling)
    //    {
    //        normalize += Time.deltaTime;
    //        Vector3 direction = (fishTargetPoint - fishPrefab.transform.position);
    //        if (direction.sqrMagnitude < 0.1f)
    //        {
    //            fishSpeed = Random.Range(currentFish.fishSpeed * 0.3f, currentFish.fishSpeed);
    //            fishTargetPoint = SetRandomPosition();
    //        }

    //        Quaternion rotation = Quaternion.LookRotation(direction.normalized);
    //        fishPrefab.transform.rotation = Quaternion.Slerp(fishPrefab.transform.rotation, rotation, Time.deltaTime * currentFish.fishSpellTime);
    //        //fishSpeed = Mathf.Lerp(prevSpeed, 0f, normalize);// 서시히 정지
    //        fishPrefab.transform.Translate(Vector3.forward * Time.deltaTime * fishSpeed, Space.Self);
    //        //Debug.LogWarning($"{normalize} / {currentFish.fishSpellTime}");
    //        fishingCanvas.SetFishSpell(normalize / currentFish.fishSpellTime);
    //        if (normalize > currentFish.fishSpellTime)
    //        {
    //            FishState(FishStateType.Attack);
    //        }
    //        yield return null;
    //    }
    //    fishingCanvas.SetFishSpell(0f);
    //    fishingCanvas.OnArrowParent(false);
    //}
    //IEnumerator FishAttack()// 발사
    //{
    //    //float skillSpeed = currentFish.fishSpeed * fieldRadius;
    //    //float skillSpeed = fieldRadius;
    //    bool damaged = false;
    //    bool destroy = false;
    //    float normalize = 0f;
    //    fishPrefab.transform.LookAt(shipPrefab.transform);
    //    while (normalize < 1f)
    //    {
    //        normalize += Time.deltaTime;
    //        float skillSpeed = Mathf.Lerp(fieldRadius * 2f, 0f, normalize);
    //        fishPrefab.transform.Translate(Vector3.forward * Time.deltaTime * skillSpeed, Space.Self);// 이동
    //        float distance = (shipPrefab.transform.position - fishPrefab.transform.position).magnitude;
    //        if (distance < shipSize && damaged == false)
    //        {
    //            damaged = true;
    //            SetShaking();
    //            destroy = Game_Manager.current.GetPlayer.TakeDamage();// 선박에 데미지
    //        }
    //        yield return null;
    //    }

    //    // 공격 끝난 이후
    //    if (destroy == true)
    //    {
    //        // 파괴 됐을 때
    //        //FishingComplate(false);
    //        FishingDestroy();
    //        OutFishing();// 파괴되서 낚시 종료

    //        StartCoroutine(DestroyShip());// 배 부셔져서 낚시 실패
    //    }
    //    else
    //    {
    //        SetCooling();
    //        FishState(FishStateType.Idle);
    //    }
    //}

    void TryDestroy()
    {
        bool destroy = Game_Manager.current.GetPlayer.TakeDamage();// 선박에 데미지 부서졌는지 체크
        if (destroy == true)
        {
            OutFishing();// 파괴되서 낚시 종료
            StartCoroutine(DestroyShip());// 배 부셔져서 낚시 실패
        }
    }

    IEnumerator DestroyShip()
    {
        Debug.LogWarning("배 부숴짐");
        yield return new WaitForSeconds(1f);
        Game_Manager.current.GetPlayer.FishingDestroy();// 배 부숴짐
    }

    //Vector3 SetRandomPosition()
    //{
    //    float currentAngle;
    //    if (Vector3.Angle(transform.right, fishPrefab.transform.position - shipPrefab.transform.position) > 90f)
    //    {
    //        // 왼쪽
    //        currentAngle = 360f - Vector3.Angle(transform.forward, fishPrefab.transform.position);
    //    }
    //    else
    //    {
    //        // 오른쪽
    //        currentAngle = Vector3.Angle(transform.forward, fishPrefab.transform.position);
    //    }
    //    float minMaxAngle = Random.Range(45f, 120f);
    //    int randomIndex = Random.Range((int)0, (int)2) > 0 ? -1 : 1;
    //    float randomAngle = minMaxAngle * randomIndex + currentAngle;
    //    Vector3 tempAngle = DirFromAngle(randomAngle);
    //    float randomRange = Random.Range(shipSize, fieldRadius);
    //    Vector3 position = transform.position + tempAngle * randomRange;
    //    return position;
    //}

    //Vector3 DirFromAngle(float angleInDegrees)
    //{
    //    return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    //}

    //===================================================================================================================
    // 흔들기
    //===================================================================================================================

    Coroutine shakingObject;
    CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
    float shakeTime = 1f, shakeValue = 5f;

    void SetShaking()
    {
        if (Option_Manager.current.optionControl.GetShake == false)
            return;

        if (shakingObject != null)
            StopCoroutine(shakingObject);
        shakingObject = StartCoroutine(Shaking());
    }

    IEnumerator Shaking()
    {
        float shakeDuration = 1f / shakeTime;
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * shakeDuration;
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

    void SetFishingDelegate()
    {
        fishingFish.deleFillAmount = fishingCanvas.SetFishSpell;
        fishingFish.deleStartSpell = StartSpell;
        fishingFish.deleDestroy = TryDestroy;
    }

    void StartSpell(bool _isOn)
    {
        spelling = _isOn;
        if (_isOn == true)// 스펠 시작
        {
            SetSkillCord();
        }
        else
        {
            fishingCanvas.OnArrowParent(false);// 스펠 완료
            fishingCanvas.SetFishSpell(0f);
        }

    }

    void SetSkillCord()
    {
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
                //StartCoroutine(CancelState());
                SetShaking();
                fishingFish.Interfere();// 캔슬 성공
            }
        }
        else
        {
            currentIndex = 0;
            fishingCanvas.InputFail();// 입력 실패
        }
    }
    //// 방어 성공
    //IEnumerator CancelState()
    //{
    //    fishingCanvas.OnArrowParent(false);
    //    fishingCanvas.SetFishSpell(0f);
    //    FishState(FishStateType.None);

    //    SetShaking();
    //    yield return new WaitForSeconds(currentFish.fishGroggyTime);// 그로기 타임

    //    SetCooling();
    //    FishState(FishStateType.Idle);
    //}

    //void SetCooling()
    //{
    //    cooling = Time.time + currentFish.fishCoolTime;
    //}

    //===================================================================================================================
    // 낚시 완료
    //===================================================================================================================
    void FishingDestroy()// 낚시 완료
    {
        fishingSet.SetActive(false);// 물고기, 낚시대 제거

        //Option_Manager.current.SetThemeMusic(null);// 테마 음악 초기화
        Singleton_Audio.INSTANCE.Stop_LoopFX();// 낚시 소리 정지

        isFishing = false;
        StopAllCoroutines();
        cinemachineBasicMultiChannelPerlin.AmplitudeGain = 0f;// 쉐이크 정지
        //FishState(FishStateType.None);
        fishingFish.FishingComplate();

        fishingCanvas.OnArrowParent(false);
        fishingCanvas.SetFishSpell(0f);

        fishingCanvas.FishingOver();// 낚시 유아이 제거
    }

    void FishingComplate(bool _success)// 낚시 완료
    {
        fishingSet.SetActive(false);// 물고기, 낚시대 제거

        Debug.LogWarning($"FishingComplate : {_success}");
        //Option_Manager.current.SetThemeMusic(null);// 테마 음악 초기화
        Singleton_Audio.INSTANCE.Stop_LoopFX();// 낚시 소리 정지

        isFishing = false;
        StopAllCoroutines();
        cinemachineBasicMultiChannelPerlin.AmplitudeGain = 0f;// 쉐이크 정지
        //FishState(FishStateType.None);
        fishingFish.FishingComplate();

        fishingCanvas.OnArrowParent(false);
        fishingCanvas.SetFishSpell(0f);

        fishingCanvas.FishingOver();// 낚시 유아이 제거
        StartCoroutine(CatchaAction(_success));// 낚시 성공
    }

    IEnumerator CatchaAction(bool _success)// 낚시 성공 텍스트
    {
        if (_success == true)
        {
            catchaText.SetActive(true);
            yield return new WaitForSeconds(3f);

            catchaText.SetActive(false);
            SetReward();
        }
        else
        {

            failText.SetActive(true);
            yield return new WaitForSeconds(3f);

            failText.SetActive(false);
            CheckFishQueue();
        }
    }

    public void SetReward()
    {
        if (currentFish.fishDayType == DayType.Night)
        {
            Steam_StatsManager.current.NightFishing();// 밤낚시 체크
        }
        Game_Manager.current.GetFishGuide.AddFishClass(currentFish.id, currentFish.GetRandom().size);// 생선 가이드에 추가
        // 행운의 물고기 두마리 낚을 확률
        float luckValue = Game_Manager.current.currentStatus.luckFish;
        float randomValue = Random.Range(0f, 100f);
        string[] itemIDs;
        if (luckValue > randomValue)// 행운의 물고기 두마리 낚음
        {
            // 물고기 세팅
            FishStruct bonusFish = GetFishStruct();// 보너스 물고기
            itemIDs = new string[2] { currentFish.id, bonusFish.id };
            Game_Manager.current.GetFishGuide.AddFishClass(bonusFish.id, bonusFish.GetRandom().size);// 생선 가이드에 추가
            // 두마리 낚음
            Game_Manager.current.GetMainUI.SetWarnningText(Const_ETC._twoFish);
        }
        else// 일반 낚시
        {
            itemIDs = new string[1] { currentFish.id };
        }

        Game_Manager.current.GetInventory.SetReward(itemIDs);// 낚시 보상 아이템 설정
        Game_Manager.current.GetMainUI.dele_CloseButton = CloseButton;
    }

    void CloseButton()// 인벤토리 닫기 버튼
    {
        //Game_Manager.current.OutOfControll(false);
        Game_Manager.current.GetInventory.CloseShop();// 상점 닫기
        CheckFishQueue();// 낚시 종료
    }

    //===================================================================================================================
    // 낚시 끝
    //===================================================================================================================

    void OutFishing()// 낚시 나가기 버튼
    {
        Singleton_Continue.INSTANCE.SaveContinue();// 낚시 종료 시 저장
        positionComposer.gameObject.SetActive(false);

        // 스타트 버튼 비활성화
        fishingCanvas.OnStartButton(0);
        fishingFish.FishingComplate();// 물고기 제거
        Game_Manager.current.OutOfControll(false);// 컨트롤 복구

        // 보상 닫기
        Game_Manager.current.GetInventory.CloseResult();// 낚시 보상 창 닫기
        Game_Manager.current.GetMainUI.OpenCanvas(true);// 메인 유아이 다시 열기

        fishingCanvas.outButton.gameObject.SetActive(false);

        cinemachineBasicMultiChannelPerlin.AmplitudeGain = 0f;
    }
























    const float baitValue = 100f;

    public void SetBait(ItemStruct.ItemClass _itemClass)
    {
        int addSkillAmount = Game_Manager.current.currentStatus.fishAmount;// 낚시 스킬로 추가 횟수
        int fishingAmount = Random.Range(1, 5) + addSkillAmount;// 낚시 횟수

        fishQueue.Clear();
        for (int i = 0; i < fishingAmount; i++)
        {
            // 물고기 세팅
            FishStruct fishStruct = AddFishClass(_itemClass);// 낚시터 세팅
            fishQueue.Enqueue(fishStruct.id);
        }
        SetReady(true);// 낚시 준비
    }

    FishStruct AddFishClass(ItemStruct.ItemClass _itemClass)// 해당 타입 물고기 중 랜덤
    {
        string cordType = GetAreaType.ToString() + dayType.ToString();
        if (dictFishStruct.ContainsKey(cordType))
        {
            List<FishStruct> fishList = dictFishStruct[cordType];// 해당 타입 물고기 리스트
            FishStruct randomFish = GetFishClassChance(fishList, _itemClass);// 물고기 확률로 선택
            return randomFish;
        }
        FishStruct defaultFish = Singleton_Data.INSTANCE.Dict_Fish["fs_1001"];// 기본 물고기
        return defaultFish;
    }

    FishStruct GetFishClassChance(List<FishStruct> _fishList, ItemStruct.ItemClass _itemClass)// 확률로 물고기 선택
    {
        List<float> _floatList = new List<float>();
        float total = 0;
        for (int i = 0; i < _fishList.Count; i++)
        {
            ItemStruct.ItemClass itemClass = _fishList[i].itemStruct.itemClass;
            float fishProbability = GetProbability(itemClass) + GetAddBuff(itemClass);
            if (itemClass == _itemClass)
            {
                fishProbability += baitValue; // 해당 클래스 확률 대폭 상승
            }
            _floatList.Add(fishProbability);
            total += fishProbability;
        }

        float randomPoint = Random.value * total;
        for (int i = 0; i < _floatList.Count; i++)
        {
            if (randomPoint < _floatList[i])
            {
                return _fishList[i];
            }
            else
            {
                randomPoint -= _floatList[i];
            }
        }
        return _fishList[^1];
    }


    //===================================================================================================================
    // 물고기 치트 테스트
    //===================================================================================================================

    public void SetFishingTest(string _id)
    {
        int fishingAmount = 100;// 낚시 횟수
        fishQueue.Clear();
        for (int i = 0; i < fishingAmount; i++)
        {
            // 물고기 세팅
            if (Singleton_Data.INSTANCE.Dict_Fish.ContainsKey(_id))
            {
                fishQueue.Enqueue(_id);
            }
            else
            {
                Debug.LogError($"[Fishing_Manager] 해당 ID 물고기 없음 : {_id}");
            }
        }
        SetReady(true);// 치트 테스트 낚시 준비
    }
}
