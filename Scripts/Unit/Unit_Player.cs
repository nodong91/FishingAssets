using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Data_Manager;

public class Unit_Player : MonoBehaviour
{
    public enum State
    {
        None,
        Idle,
        Move,
        Damage,
        Destroy
    }
    public State state = State.None;
    public Rigidbody rb = null;
    SetStatus CurrentStatus => Game_Manager.current.currentStatus;
    public float moveSpeed = 1f;
    public int health;
    public bool FullHealth { get { return health >= CurrentStatus?.shipHealth; } }
    public float energy;
    public float GetEnergy { get { return energy; } }
    public float GetMaxEnergy { get { return CurrentStatus.maxEnergy; } }
    public float efficient;// 에너지 효율
    private Vector2 dirction => Game_Manager.current.controllManager.dirction;

    public GameObject playerObject;
    GameObject FocusTarget => Camera_Manager.current?.GetFocusTarget;
    Data_Continue continueData => Game_Manager.current.GetContinue;
    Coroutine stateAction;

    [SerializeField] private List<Trigger_Setting> triggerGameObject = new List<Trigger_Setting>();
    private Trigger_Setting closestTarget;

    //Quaternion prevAngle, setAngle;
    //float randomTime, runningRandomTime;
    //// 물위에서 배의 움직임
    //private float shipHight = -0.1f;
    //private float waveSpeed = 2f;
    //private float targetAngle = 10f;

    //float runningTime;
    //public AnimationCurve rotateCurve;// 위아래 흔들릴 때 로테이션

    public void SetStart()
    {
        rb.useGravity = false;
        if (continueData == null)
            return;

        health = CurrentStatus.shipHealth - continueData.destroySlot.Count;
        energy = continueData.energy;
        StateMachine(State.Idle);

        if (FocusTarget == null)
            return;

        FocusTarget.transform.position = transform.position;
    }

    public void SetStatus()
    {
        moveSpeed = CurrentStatus.shipSpeed;

        Game_Manager.current.GetMainUI.SetMaxEnergyPoint(CurrentStatus.maxEnergy);
        health = (FullHealth == true) ? CurrentStatus.shipHealth : CurrentStatus.shipHealth - continueData.destroySlot.Count;// 스탯 추가 하기  전 풀피 체크
        Game_Manager.current.GetMainUI.SetMaxHealthPoint(CurrentStatus.shipHealth);
        Game_Manager.current.GetMainUI.SetHealthPoint(health);// 시작 세팅
        energy = continueData.energy;
        efficient = CurrentStatus.efficient;
        Debug.LogWarning($"SetStatus - Energy : {energy}/{CurrentStatus.maxEnergy}, SetStatus - Health : {health}/{CurrentStatus.shipHealth}");
        SetEnergyUI();
    }

    public void SetShip(Data_Ship _shipData)// 배 생성 및 변경
    {
        if (playerObject != null)
        {
            Destroy(playerObject);
        }
        rb.useGravity = true;

        GameObject inst = Instantiate(_shipData.shipObject, transform);
        playerObject = inst;
        Debug.LogWarning($"{_shipData.name} : {_shipData.shipObject}");
    }

    //================================================================================================================================================
    // 업데이트
    //================================================================================================================================================

    //private void Update()
    //{
    //    //SetOceanRenderer();// 물 위에서 배의 움직임
    //    //// 배 부분 물결 안생기게
    //    //string shipPosition = "_ShipPosition";
    //    //reflection_Manager.GetMaterial.SetVector(shipPosition, playerObject.transform.position);
    //    //reflection_Manager.GetMaterial.SetFloat("_WaveSpeed", waveSpeed);
    //}

    //void SetOceanRenderer()
    //{
    //    if (playerObject == null)
    //        return;

    //    runningTime += Time.deltaTime * waveSpeed;

    //    float moveHight = (Mathf.Sin(runningTime) + 1f) * 0.5f;// 위아래 움직임
    //    Vector3 localPosition = Vector3.up * moveHight * shipHight;
    //    playerObject.transform.localPosition = localPosition;

    //    if (runningTime >= runningRandomTime)
    //    {
    //        randomTime = Random.Range(5f, 3f);
    //        runningRandomTime = runningTime + randomTime;
    //        prevAngle = playerObject.transform.localRotation;
    //        setAngle = Quaternion.Euler(RandomAngle(targetAngle));
    //    }

    //    float curve = rotateCurve.Evaluate(1f - (runningRandomTime - runningTime) / randomTime);
    //    playerObject.transform.localRotation = Quaternion.Slerp(prevAngle, setAngle, curve / randomTime);// 랜덤 회전

    //}

    //Vector3 RandomAngle(float _maxAngle)
    //{
    //    float x = Random.Range(-_maxAngle, _maxAngle);
    //    float y = Random.Range(-_maxAngle, _maxAngle);
    //    float z = Random.Range(-_maxAngle, _maxAngle);
    //    return new Vector3(x, y, z);
    //}

    //================================================================================================================================================
    // 컨트롤
    //================================================================================================================================================

    void StateMachine(State _state)
    {
        state = _state;
        if (stateAction != null)
            StopCoroutine(stateAction);

        switch (state)
        {
            case State.None:

                break;
            case State.Idle:
                if (dirction.x != 0f || dirction.y != 0f)
                    StateMachine(State.Move);
                break;
            case State.Move:
                stateAction = StartCoroutine(Moving());
                break;
            case State.Damage:
                TakeDamage();
                break;
            case State.Destroy:
                StateDestroy();
                break;
        }
    }

    //================================================================================================================================================
    // 이동
    //================================================================================================================================================

    public void StateMove()
    {
        //dirction = _dirction;
        if (state == State.Idle)
        {
            StateMachine(State.Move);
        }
        if (state == State.Move)// 공격이나 회피가 있을 수 있으니
        {
            if (dirction.x == 0f && dirction.y == 0f)
                StateMachine(State.Idle);
        }
    }

    IEnumerator Moving()
    {
        while (state == State.Move)
        {
            SetMoving();
            CheckClosestUnit();// 무브

            energy -= CurrentStatus.efficient * Time.deltaTime;// 0에 가까울 수록 소비 안함
            SetEnergyUI();
            if (GetMaxEnergy > 0f && energy <= 0f)// 에너지 없으면 파괴
            {
                StateMachine(State.Destroy);
            }
            yield return null;
        }
    }
    public void AddEnergy(float _value)
    {
        energy += _value;
        energy = Mathf.Clamp(energy, 0f, CurrentStatus.maxEnergy);
        SetEnergyUI();
    }

    void SetEnergyUI()
    {
        Game_Manager.current.GetMainUI.SetEnergy(energy / CurrentStatus.maxEnergy);
    }

    public void CheckClosestUnit()// 아이템이나 채집 같은거 하기 위한 체크
    {
        if (triggerGameObject.Count == 0)
            return;

        float closestDistance = float.MaxValue;
        Trigger_Setting tempTarget = null;
        for (int i = 0; i < triggerGameObject.Count; i++)
        {
            float offsetDist = (triggerGameObject[i].transform.position - transform.position).sqrMagnitude;
            if (closestDistance > offsetDist)
            {
                closestDistance = offsetDist;
                tempTarget = triggerGameObject[i];
            }
        }

        if (closestTarget != tempTarget)
        {
            closestTarget = tempTarget;
        }
        Game_Manager.current.GetFollow.AddClosestTarget(closestTarget);
    }

    void SetMoving()
    {
        if (FocusTarget == null)
            return;
        //Transform focusTarget = Game_Manager.current.cameraManager.GetFocusTarget;
        FocusTarget.transform.position = transform.position;

        Vector3 dir = new Vector3(dirction.x, 0f, dirction.y);
        Vector3 target = transform.position + FocusTarget.transform.TransformDirection(dir).normalized;
        //Vector3 target = transform.position + focusTarget.transform.forward;

        float speed = moveSpeed * Time.deltaTime;
        Vector3 offset = (target - transform.position).normalized;
        transform.position = Vector3.Lerp(transform.position, target, speed);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(offset), speed);
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(focusTarget.transform.forward), speed * 5f);
    }

    //================================================================================================================================================
    // 회피
    //================================================================================================================================================

    IEnumerator MovingClash(Vector3 _target)
    {
        Camera_Manager.current.InputShake();// 카메라 흔들기
        float normalize = 0f;
        while (normalize < 1f)// 뒤로 밀려나기
        {
            normalize += Time.deltaTime;
            float speed = (1f - normalize) * 0.1f;
            transform.position = Vector3.Lerp(transform.position, _target, speed);
            CheckClosestUnit();// 가까운 트리거 체크
            yield return null;
        }
        // 밀려난 이후 상태 체크
        if (health > 0)
            StateMachine(State.Idle);// 다시 대기 상태
        else
            StateMachine(State.Destroy);
    }

    int destroyCount => Game_Manager.current.GetInventory.myBox.destroySlot.Count;
    public bool TakeDamage()
    {
        Singleton_Audio.INSTANCE.Audio_FX(String_Audio._clash);
        Game_Manager.current.GetInventory.DistroySlot();// 랜덤 슬롯 부수기

        health = CurrentStatus.shipHealth - destroyCount;
        Game_Manager.current.GetMainUI.SetHealthPoint(health);// 데미지

        Debug.LogWarning($"TakeDamage - {health} ({destroyCount})");
        return health <= 0;
    }

    public void AddHealth(int _health)
    {
        health += _health;
        Game_Manager.current.GetMainUI.SetHealthPoint(health);// 데미지
    }

    void StateDestroy()
    {
        // 완전 파괴
        StartCoroutine(ResetPosition());
    }

    public void FishingDestroy()
    {
        // 낚시 중 파괴
        StateMachine(State.Destroy);
    }

    IEnumerator ResetPosition()
    {
        Game_Manager.current.GetMainUI.SetFadeScreen(true);
        yield return new WaitForSeconds(0.5f);

        Game_Manager.current.PlayerDestroy();// 플레이어 위치에 고스트 놓고 인벤토리 비우기
        Debug.LogError("견인 되는 연출 필요 - 보험 회사 도착");
        // 견인 되는 연출 필요
        // 위치 변경

        GameObject landingPoint = Game_Manager.current.CurrentLand.landingPoint.gameObject;
        Vector3 forwardDirection = landingPoint.transform.rotation * Vector3.forward;
        Vector3 backwardPosition = landingPoint.transform.position - forwardDirection * 3f;
        Vector3 targetPosition = landingPoint.transform.position;

        // 마지막 위치로 이동
        transform.SetPositionAndRotation(backwardPosition, landingPoint.transform.rotation);

        if (FocusTarget != null)
            FocusTarget.transform.position = transform.position;
        yield return new WaitForSeconds(1f);

        Game_Manager.current.GetMainUI.SetFadeScreen(false);
        //yield return new WaitForSeconds(0.5f);

        float noramlize = 0f;
        while (noramlize < 1f)
        {
            noramlize += Time.deltaTime;
            transform.position = Vector3.Lerp(backwardPosition, targetPosition, noramlize);
            yield return null;
        }
        //StartCoroutine(ShipTowed(backwardPosition));
        Debug.LogError("견인 되는 연출 필요 - 마을 도착");
        CheckClosestUnit();// 가까운 트리거 체크
        //// 스탯 리셋
        //SetStatus();
    }

    //================================================================================================================================================
    // 액션
    //================================================================================================================================================

    public void EventAction()// 어택 이벤트
    {

    }

    public void State_Action(bool _input)// 클릭 이벤트
    {
        if (_input == true)
        {
            if (closestTarget != null)
            {
                closestTarget.TriggerAction();// 가까운 트리거 액션
                triggerGameObject.Remove(closestTarget);
                closestTarget = null;
                Game_Manager.current.GetFollow.AddClosestTarget(null);// 팔로우 유아이 제거
            }
        }
    }

    public void OutOfControll(bool _isOn)
    {
        if (_isOn == true)
            StateMachine(State.None);
        else
            StateMachine(State.Idle);// 다시 대기 상태
    }

    //================================================================================================================================================
    // 충돌
    //================================================================================================================================================

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Trigger_Setting>(out var _trigger) == false)
            return;
        if (triggerGameObject.Contains(_trigger) == false)
            triggerGameObject.Add(_trigger);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Trigger_Setting>(out var fishing) == false)
            return;

        triggerGameObject.Remove(fishing);
        if (triggerGameObject.Count == 0)
        {
            closestTarget = null;
            Game_Manager.current.GetFollow.AddClosestTarget(null);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Finish")// 충돌체
        {
            if (state != State.Damage)
            {
                StateMachine(State.Damage);

                Vector3 direction = (transform.position - collision.transform.position).normalized;
                Vector3 target = transform.position + direction;
                StartCoroutine(MovingClash(target));

                Debug.LogWarning($"{collision.gameObject.name} 충돌!!!!!!!!!!!!!!!!!!!! {transform.position}");
            }
        }
    }
}