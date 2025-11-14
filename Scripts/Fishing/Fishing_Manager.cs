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

    private const float fieldRadius = 5f;

    public GameObject catchPrefab;
    public Renderer catchRanderer;
    private SetStatus catchStatus;
    float catchRadius = 1f;// 지름이 클수록 데미지가 약해짐 (줄을 느슨하게)

    FishStruct currentFish;
    RandomSize currentSize;
    public GameObject fishPrefab;
    Coroutine fishAction;

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

    AreaType areaType;
    DayType dayType;
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

        // 물고기 시간, 영역 세팅
        dayType = Game_Manager.current.GetMainUI.timeUI.lightMode;
        SetFish();
        SetReady(true);// 낚시 준비
    }
    //===================================================================================================================
    // 물고기 세팅
    //===================================================================================================================

    Queue<string> fishQueue = new Queue<string>();
    void SetFish()
    {
        int addSkillAmount = Game_Manager.current.currentStatus.fishAmount;// 낚시 스킬로 추가 횟수
        int fishingAmount = Random.Range(1, 5) + addSkillAmount;// 낚시 횟수

        fishQueue.Clear();
        for (int i = 0; i < fishingAmount; i++)
        {
            // 물고기 세팅
            FishStruct fishStruct = GetFishStruct();
            fishQueue.Enqueue(fishStruct.id);
        }
        //// 물고기 세팅
        //if (TryFishStruct(out FishStruct _fish) == true)
        //{
        //    fishQueue.Enqueue(_fish.id);
        //    currentFish = _fish;// 물고기 정보
        //    currentSize = currentFish.GetRandom();
        //    Debug.LogWarning($"{_fish.id} : {dictFishStruct.Count}");
        //}
    }

    FishStruct GetFishStruct()// 해당 타입 물고기 중 랜덤
    {
        string cordType = areaType.ToString() + dayType.ToString();
        if (dictFishStruct.ContainsKey(cordType))
        {
            List<FishStruct> fishList = dictFishStruct[cordType];// 해당 타입 물고기 리스트
            FishStruct randomFish = FishingChance(fishList);
            return randomFish;
        }
        Debug.LogError($"[Fishing_Manager] 해당 타입 물고기 없음 : {cordType}");
        FishStruct defaultFish = Singleton_Data.INSTANCE.Dict_Fish["fs_1001"];// 기본 물고기
        return defaultFish;
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

    float GetProbability(ItemStruct.ItemClass _class)// 물고기 클래스별 확률
    {
        // 버프는 중복 적용 안됨
        // 버프는 배스탯 관련만??
        // 생선을 미끼로 사용?
        float addValue = 0f;
        Game_Manager.FishBuffStruct fishBuff = Game_Manager.current.GetFishBuff;// 낚시 버프
        if (fishBuff != null && fishBuff.itemClass == _class)
        {
            addValue = fishBuff.addValue;
        }

        // 물고기 클래스별 확률
        float classValue = _class switch
        {
            // 필요한 물고기가 안나올 확률????? - 낮은 클래스 물고기 안나올 수 있음
            // 미끼가 특정 클래스의 확률을 조정
            ItemStruct.ItemClass.Common => 0.6f,
            ItemStruct.ItemClass.Uncommon => 0.25f,
            ItemStruct.ItemClass.Rare => 0.1f,
            ItemStruct.ItemClass.Epic => 0.04f,
            ItemStruct.ItemClass.Legendary => 0.01f,
            _ => 0f,
        };
        return classValue + addValue;
    }

    //===================================================================================================================
    // 준비
    //===================================================================================================================

    void SetReady(bool _ready)// 준비
    {
        Game_Manager.current.GetMainUI.timeUI.TimePause(true);// 낚시 하는 동안 시간 정지

        fishingSet.SetActive(true);
        transform.position = Game_Manager.current.GetPlayer.transform.position;

        float rotateY = Camera.main.transform.rotation.eulerAngles.y;
        positionComposer.transform.rotation = Quaternion.Euler(45f, rotateY, 0f);
        positionComposer.gameObject.SetActive(true);

        // 버튼 활성화
        fishingCanvas.OnStartButton(fishQueue.Count, areaType.ToString(), dayType.ToString());
        Game_Manager.current.GetMainUI.OpenCanvas(false);// 낚시 시작 MainUI 제거
        Game_Manager.current.OutOfControll(true);
    }

    //===================================================================================================================
    // 시작
    //===================================================================================================================

    void FishingStartButton()// 시작 버튼
    {
        SetFish();
        isFishing = true;

        string _id = fishQueue.Dequeue();
        currentFish = Singleton_Data.INSTANCE.Dict_Fish[_id];
        //Option_Manager.current.SetThemeMusic(bgmBattle);// 전투 시작 음악
        fishingCanvas.SetFishing();

        SetFishing();// 낚시 초기화
        StartCoroutine(StartCount());
    }

    void SetFishing()// 초기 세팅
    {
        catchStatus = Game_Manager.current.currentStatus;
        cooling = Time.time + currentFish.fishCoolTime;

        Vector3 randomPoint = Random.insideUnitSphere * fieldRadius;
        fishTargetPoint = new Vector3(randomPoint.x, 0f, randomPoint.z) + transform.position;
        fishPrefab.transform.position = fishTargetPoint;

        fishHealth = currentFish.fishHealth / (catchStatus.catchMaxHealth + currentFish.fishHealth);
        fishingCanvas.SetFishHP(fishHealth);

        catchPrefab.transform.position = fishPrefab.transform.position;
        catchRadius = catchStatus.catchRadius;
        catchPrefab.transform.localScale = Vector3.one * catchRadius;
        Debug.LogWarning($"SetReady 게이지 : {currentFish.fishHealth} / ({catchStatus.catchMaxHealth} + {currentFish.fishHealth}) = {fishHealth}");
    }

    IEnumerator StartCount()// 카운트
    {
        for (int i = 0; i < 3; i++)
        {
            int index = 3 - i;
            fishingCanvas.SetCount(index);
            yield return new WaitForSeconds(1f);
        }
        fishingCanvas.SetCount(0);// 카운트 완료
        fishingCanvas.SetFishUI();

        Debug.LogWarning("낚시가 처음인지 확인 - 튜토리얼 시작");
        //Game_Manager.current.GetTutorial.SetTutorial(String_Tutorial._fishing);
        //Game_Manager.current.GetTutorial.StartTutorial();
        yield return null;

        StartCoroutine(CatchMovement());
        if (fishHealth > 0)
            FishState(FishStateType.Idle);
    }

    IEnumerator CatchMovement()
    {
        float cutTime = 0f;
        float catchSpeed = catchStatus.catchSpeed;
        while (isFishing == true)
        {
            Vector3 catchOffset = CatchRayCast() - catchPrefab.transform.position;
            catchSpeed = Mathf.Lerp(catchSpeed, catchStatus.catchSpeed * Mathf.Clamp01(catchOffset.magnitude), 0.1f);
            catchPrefab.transform.Translate(catchOffset.normalized * Time.deltaTime * catchSpeed, Space.World);
            MoveCatch();
            yield return null;

            PlayingControll();
            TestControll();
            CheckingCatch();
            if (fishingCanvas.TryTention == true)
            {
                cutTime += Time.deltaTime;
                if (cutTime > 1f)// 1초이상 팽팽하게 당기면 끊어짐
                    FishingComplate(false);
            }
            else
            {
                cutTime = 0f;
            }
        }
    }

    void MoveCatch()
    {
        // 카메라 포커스 위치
        focusTarget.position = Vector3.Lerp(catchPrefab.transform.position, shipPrefab.transform.position, 0.5f);// 카메라 포커스

        // 캐치 영역 안에 있는지 체크
        Vector2 fishPosition = new Vector2(fishPrefab.transform.position.x, fishPrefab.transform.position.z);
        Vector2 catchPosition = new Vector2(catchPrefab.transform.position.x, catchPrefab.transform.position.z);
        float catchDistance = (fishPosition - catchPosition).magnitude;
        isCatching = (catchDistance < catchRadius);
        Color catchColor = isCatching ? onCatchColor : notCatchColor;
        catchRanderer.material.SetColor("_Color", catchColor);
        fishingCanvas.LinTention(catchDistance / catchRadius);

        float fishDistance = (fishPrefab.transform.position - shipPrefab.transform.position).magnitude;
        positionComposer.CameraDistance = (fishDistance * 0.5f) + defaultCameraDistance;
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

    void CheckingCatch()
    {
        if (isCatching == true)
        {
            //bool critical = Random.Range(0f, 1f) > 0.5f;
            //// 낚시대 힘만큼 데미지
            //float setDamage = critical ? catchStatus.catchPower + catchStatus.catchPower * 0.2f : catchStatus.catchPower;
            //float damage = setDamage / (currentFish.fishHealth + catchStatus.catchMaxHealth);
            if (catching == true)
            {
                float damage = catchStatus.catchPower / (currentFish.fishHealth + catchStatus.catchMaxHealth);
                fishHealth -= damage * Time.deltaTime;
            }
            fishingCanvas.SetFishIcon(1f);
        }
        else
        {
            // 물고기 힘만큼 힐
            float damage = currentFish.fishPower / (currentFish.fishHealth + catchStatus.catchMaxHealth);
            fishHealth += damage * Time.deltaTime;
            fishingCanvas.SetFishIcon(-1f);
        }
        fishingCanvas.SetFishHP(fishHealth);

        if (fishHealth >= 1f || fishHealth <= 0f)
        {
            FishingComplate(fishHealth <= 0f);// 낚시 성공 실패
        }
    }

    //===================================================================================================================
    // 컨트롤
    //===================================================================================================================

    bool catching;
    void TestControll()
    {
        if (Input.GetMouseButton(0))
        {
            if (catching == false)
            {
                catching = true;
                Singleton_Audio.INSTANCE.Audio_LoopFX(String_Audio._reeling);// 낚시 소리
            }
            if (catchRadius > 0.1f)
                catchRadius -= 1f * Time.deltaTime;
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


    //===================================================================================================================
    // 물고기 상태
    //===================================================================================================================

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

    void IdleState()
    {
        if (currentFish.fishCoolTime > 0f && cooling < Time.time)
        {
            // 쿨타임이 0인 경우 공격하지 않음
            FishState(FishStateType.Spelling);// 스킬 기술
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
        float randomSpeed = Random.Range(currentFish.fishSpeed * 0.3f, currentFish.fishSpeed);
        //float randomSpeed = distance * currentFish.fishSpeed;
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

        Vector3 targetPosition = shipPrefab.transform.position + (fishPrefab.transform.position - shipPrefab.transform.position).normalized * (shipSize + 1f);
        float prevSpeed = fishSpeed;
        float normalize = 0f;
        while (normalize < currentFish.fishSpellTime)
        {
            normalize += Time.deltaTime / currentFish.fishSpellTime;
            Vector3 direction = (fishTargetPoint - fishPrefab.transform.position);
            if (direction.sqrMagnitude < 0.1f)
            {
                fishSpeed = Random.Range(currentFish.fishSpeed * 0.3f, currentFish.fishSpeed);
                fishTargetPoint = SetRandomPosition();
            }

            Quaternion rotation = Quaternion.LookRotation(direction.normalized);
            fishPrefab.transform.rotation = Quaternion.Slerp(fishPrefab.transform.rotation, rotation, Time.deltaTime * currentFish.fishSpellTime);
            //fishSpeed = Mathf.Lerp(prevSpeed, 0f, normalize);// 서시히 정지
            fishPrefab.transform.Translate(Vector3.forward * Time.deltaTime * fishSpeed, Space.Self);
            fishingCanvas.SetFishSpell(normalize / currentFish.fishSpellTime);
            yield return null;
        }
        fishingCanvas.SetFishSpell(0f);
        fishingCanvas.OnArrowParent(false);
        yield return null;

        FishState(FishStateType.Attack);
    }
    IEnumerator FishAttack()// 발사
    {
        //float skillSpeed = currentFish.fishSpeed * fieldRadius;
        //float skillSpeed = fieldRadius;
        bool damaged = false;
        bool destroy = false;
        float normalize = 0f;
        fishPrefab.transform.LookAt(shipPrefab.transform);
        while (normalize < 1f)
        {
            normalize += Time.deltaTime;
            float skillSpeed = Mathf.Lerp(fieldRadius * 2f, 0f, normalize);
            fishPrefab.transform.Translate(Vector3.forward * Time.deltaTime * skillSpeed, Space.Self);// 이동
            float distance = (shipPrefab.transform.position - fishPrefab.transform.position).magnitude;
            if (distance < shipSize && damaged == false)
            {
                damaged = true;
                //SetShaking();
                destroy = Game_Manager.current.GetPlayer.TakeDamage();// 선박에 데미지
            }
            yield return null;
        }
        // 공격 끝난 이후
        if (destroy == true)
        {
            // 파괴 됐을 때
            FishingComplate(false);
            OutFishing();// 파괴 되서       낚시 종료

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
        //Option_Manager.current.SetThemeMusic(null);// 테마 음악 초기화
        Singleton_Audio.INSTANCE.Stop_LoopFX();// 낚시 소리 정지

        isFishing = false;
        StopAllCoroutines();
        FishState(FishStateType.None);

        fishingCanvas.OnArrowParent(false);
        fishingCanvas.SetFishSpell(0f);

        fishingCanvas.FishingOver();// 낚시 유아이 제거
        if (_success == true)// 낚시 성공
        {
            StartCoroutine(CatchaAction());
        }
        else
        {
            FishingOver();
        }
    }

    public GameObject catchaText;
    IEnumerator CatchaAction()// 낚시 성공 텍스트
    {
        catchaText.SetActive(true);
        yield return new WaitForSeconds(3f);

        catchaText.SetActive(false);
        SetReward();
    }

    public void SetReward()
    {
        string[] itemIDs;

        ItemStruct fishItem = currentFish.itemStruct;
        float size = currentSize.size;
        // 행운의 물고기 두마리 낚을 확률
        float luckValue = Game_Manager.current.currentStatus.luckFish;
        float randomValue = Random.Range(0f, 100f);
        if (luckValue > randomValue)// 행운의 물고기 두마리 낚음
        {
            // 물고기 세팅
            FishStruct bonusFish = GetFishStruct();
            itemIDs = new string[2] { fishItem.id, bonusFish.id };
            // 두마리 낚음
            Debug.LogWarning("축하합니다! 행운의 물고기 두 마리를 낚았습니다!");
        }
        else// 일반 낚시
        {
            itemIDs = new string[1] { fishItem.id };
        }

        ResultStruct fishResult = new ResultStruct
        {
            inventorySize = new Vector2Int(7, 7), // 인벤토리 크기
            money = 0, // 돈
            itemID = itemIDs, // 아이템 ID
        };

        Game_Manager.current.GetInventory.SetResult(fishResult);// 퀘스트 완료 후 결과 아이템 설정
        Game_Manager.current.GetInventory.OpenResult();// 낚시 보상
        Game_Manager.current.GetFishGuide.AddFishClass(fishItem.id, size);// 생선 가이드에 추가
        Game_Manager.current.GetMainUI.dele_CloseButton = CloseButton;
    }

    void CloseButton()// 인벤토리 닫기 버튼
    {
        Game_Manager.current.OutOfControll(false);
        Game_Manager.current.GetInventory.CloseShop();// 상점 닫기
        FishingOver();// 낚시 종료
    }

    public void FishingOver()
    {
        // 버튼 활성화
        fishingCanvas.OnStartButton(fishQueue.Count, areaType.ToString(), dayType.ToString());// 스타트 버튼 활성화
    }

    //===================================================================================================================
    // 낚시 끝
    //===================================================================================================================

    void OutFishing()// 낚시 나가기 버튼
    {
        Singleton_Continue.INSTANCE.SaveContinue();// 낚시 종료 시 저장

        Game_Manager.current.GetMainUI.timeUI.TimePause(false);// 시간 정지 종료
        positionComposer.gameObject.SetActive(false);

        // 스타트 버튼 비활성화
        fishingCanvas.OnStartButton(0);

        Game_Manager.current.OutOfControll(false);// 컨트롤 복구

        // 보상 닫기
        Game_Manager.current.GetInventory.CloseResult();// 낚시 보상 창 닫기
        Game_Manager.current.GetMainUI.OpenCanvas(true);// 메인 유아이 다시 열기

        fishingSet.SetActive(false);// 물고기, 낚시대 제거
        fishingCanvas.outButton.gameObject.SetActive(false);

        cinemachineBasicMultiChannelPerlin.AmplitudeGain = 0f;
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



































    //===================================================================================================================
    // 물고기 테스트
    //===================================================================================================================

    public void SetFishingTest(string _id)
    {
        int addSkillAmount = Game_Manager.current.currentStatus.fishAmount;// 낚시 스킬로 추가 횟수
        int fishingAmount = 100 + addSkillAmount;// 낚시 횟수
        fishQueue.Clear();
        // 물고기 세팅
        if (Singleton_Data.INSTANCE.Dict_Fish.ContainsKey(_id))
        {
            fishQueue.Enqueue(_id);
        }
        else
        {
            Debug.LogError($"[Fishing_Manager] 해당 ID 물고기 없음 : {_id}");
        }
        SetReady(true);// 테스트 낚시 준비
    }
}
