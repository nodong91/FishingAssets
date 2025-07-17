
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Player : MonoBehaviour
{

    public enum State
    {
        None,
        Dead,
        Action,
        Idle,
        Escape,
        Move,
        Patrol,
        Damage,
        End,
    }
    public State state = State.None;

    public float moveSpeed = 0.01f;

    private void Start()
    {
        SetControll();
    }

    //================================================================================================================================================
    // 컨트롤 세팅
    //================================================================================================================================================

    void SetMouse()
    {
        //Singleton_Controller.INSTANCE.key_MouseLeft += InputMousetLeft;
        //Singleton_Controller.INSTANCE.key_MouseRight += InputMouseRight;
        //Singleton_Controller.INSTANCE.key_MouseWheel += InputMouseWheel;
    }

    void RemoveMouse()
    {
        //Singleton_Controller.INSTANCE.key_MouseLeft -= InputMousetLeft;
        //Singleton_Controller.INSTANCE.key_MouseRight += InputMouseRight;
        //Singleton_Controller.INSTANCE.key_MouseWheel += InputMouseWheel;
    }

    //================================================================================================================================================
    // 컨트롤
    //================================================================================================================================================


    public void StateMachine(State _state)
    {
        state = _state;

        if (stateAction != null)
            StopCoroutine(stateAction);

        switch (state)
        {
            case State.None:
                break;
            case State.Dead:
                //RemoveKeyCode();
                //DeadState();
                //OutOfControll(true);
                break;
            case State.Action:
                break;
            case State.Idle:
                if (dirction.x != 0f || dirction.y != 0f)
                    StateMachine(State.Move);
                break;
            case State.Move:
                stateAction = StartCoroutine(Moving());
                break;
            case State.Escape:
                //stateAction = StartCoroutine(MoveEscape());
                break;
            case State.Damage:
                break;
        }
    }

    //================================================================================================================================================
    // 이동
    //================================================================================================================================================

    public Vector2 dirction;
    public void StateMove(Vector2 _dirction)
    {
        dirction = _dirction;
        //SetDirection();

        //if (outOfControll == true)
        //    return;

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
            yield return null;
            CheckClosestUnit();
        }
    }

    void CheckClosestUnit()// 아이템이나 채집 같은거 하기 위한 체크
    {
        if (triggerGameObject.Count == 0)
            return;

        closestDistance = float.MaxValue;
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
        Game_Manager.current.followManager.AddClosestTarget(closestTarget);
    }
    public Reflection_Manager reflection_Manager;
    public float shipHight, waveSpeed = 2f;
    float runningTime;
    public GameObject playerObject;

    Quaternion prevAngle, setAngle;
    public float targetAngle = 10f;
    float randomTime, runningRandomTime;

    public AnimationCurve rotateCurve;

    private void Update()
    {
        SetOceanRenderer();
    }

    void SetOceanRenderer()
    {
        runningTime += Time.deltaTime * waveSpeed;

        float moveHight = (Mathf.Sin(runningTime) + 1f) * 0.5f;// 위아래 움직임
        Vector3 localPosition = Vector3.up * moveHight * shipHight;
        playerObject.transform.localPosition = localPosition;

        if (runningTime > runningRandomTime)
        {
            randomTime = UnityEngine.Random.Range(5f, 3f);
            runningRandomTime = runningTime + randomTime;
            prevAngle = playerObject.transform.localRotation;
            setAngle = Quaternion.Euler(RandomAngle(targetAngle));
        }

        float curve = rotateCurve.Evaluate(1f - (runningRandomTime - runningTime) / randomTime);
        playerObject.transform.localRotation = Quaternion.Slerp(prevAngle, setAngle, curve / randomTime);// 랜덤 회전

        string shipPosition = "_ShipPosition";
        reflection_Manager.GetMaterial.SetVector(shipPosition, playerObject.transform.position);
        reflection_Manager.GetMaterial.SetFloat("_WaveSpeed", waveSpeed);
    }

    Vector3 RandomAngle(float _maxAngle)
    {
        float x = UnityEngine.Random.Range(-_maxAngle, _maxAngle);
        float y = UnityEngine.Random.Range(-_maxAngle, _maxAngle);
        float z = UnityEngine.Random.Range(-_maxAngle, _maxAngle);
        return new Vector3(x, y, z);
    }

    void SetMoving()
    {
        float speed = moveSpeed * Time.deltaTime;
        Game_Manager.current.cameraManager.transform.position = transform.position;
        Vector3 dir = new Vector3(dirction.x, 0f, dirction.y);
        Vector3 target = transform.position + Game_Manager.current.cameraManager.transform.TransformDirection(dir).normalized;
        transform.position = Vector3.Lerp(transform.position, target, speed);

        Vector3 offset = (target - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(offset), speed * 5f);
    }

    void RotateMousePosition()
    {
        float speed = moveSpeed * Time.deltaTime;
        Vector3 playerPosition = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 mousePosition = Input.mousePosition;

        Vector3 uiOffset = (mousePosition - playerPosition).normalized;
        Vector3 dir = new Vector3(uiOffset.x, 0f, uiOffset.y);

        Vector3 target = transform.position + Game_Manager.current.cameraManager.transform.TransformDirection(dir).normalized;
        Vector3 offset = (target - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(offset), speed * 5f);
    }

    //================================================================================================================================================
    // 충돌
    //================================================================================================================================================

    public List<Trigger_Setting> triggerGameObject = new List<Trigger_Setting>();
    public Trigger_Setting closestTarget;
    public float closestDistance;

    private void OnTriggerEnter(Collider other)
    {
        Trigger_Setting fishing = other.GetComponent<Trigger_Setting>();
        if (fishing == null)
            return;
        triggerGameObject.Add(fishing);
    }

    private void OnTriggerExit(Collider other)
    {
        Trigger_Setting fishing = other.GetComponent<Trigger_Setting>();
        if (fishing == null)
            return;

        triggerGameObject.Remove(fishing);
        if (triggerGameObject.Count == 0)
        {
            closestTarget = null;
            Game_Manager.current.followManager.AddClosestTarget(null);
        }
    }

    //================================================================================================================================================
    // 회피
    //================================================================================================================================================

    public void StateEscape()
    {
        StateMachine(State.Escape);
    }

    //IEnumerator MoveEscape()// 탈출 (회피)
    //{
    //    float normalize = 0f;
    //    float actionTime = unitAnimation.PlayAnimation(4);// 애니메이션 길이만큼 대기
    //    OutOfControll(actionTime + 0.5f);// 대기 시간 0.5f
    //    Vector3 dir = new Vector3(dirction.x, 0f, dirction.y);
    //    while (normalize < actionTime)
    //    {
    //        normalize += Time.deltaTime;
    //        float escapeSpeed = Mathf.Lerp(0.3f, 0f, normalize * 5f);
    //        SetMoveEscape(dir, escapeSpeed);
    //        yield return null;
    //    }
    //    StateMachine(State.Idle);
    //}

    void SetMoveEscape(Vector3 _dir, float _escapeSpeed)
    {
        Game_Manager.current.cameraManager.transform.position = transform.position;
        Vector3 target = transform.position + Game_Manager.current.cameraManager.transform.TransformDirection(_dir).normalized;
        transform.position = Vector3.Lerp(transform.position, target, _escapeSpeed);

        Vector3 offset = (target - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(offset), _escapeSpeed * 5f);
    }

    //================================================================================================================================================
    // 공격
    //================================================================================================================================================

    Coroutine stateAction;
    bool action = false;

    public void EventAction()// 어택 이벤트
    {

    }

    public void State_Action(bool _input)// 클릭 이벤트
    {
        if (_input == true)
        {
            //if (outOfControll == true)
            //    return;

            StateMachine(State.Action);
            stateAction = StartCoroutine(State_Acting());
        }
        else
        {
            if (closestTarget != null)
            {
                triggerGameObject.Remove(closestTarget);
                switch (closestTarget.triggerType)
                {
                    case Trigger_Setting.TriggerType.Fishing:
                        // 낚시 시작
                        Game_Manager.current.fishingManager.StartGame(closestTarget);
                        Destroy(closestTarget.gameObject);
                        OutOfControll(true);
                        break;

                    case Trigger_Setting.TriggerType.Landing:
                        Game_Manager.current.OutOfControll(true);
                        closestTarget.GetTriggerLanding.SetLanding(this);
                        break;
                }
                closestTarget = null;
                Game_Manager.current.followManager.AddClosestTarget(null);// 팔로우 유아이 제거
            }
            stateAction = StartCoroutine(State_StopActing());
        }
    }

    IEnumerator State_Acting()
    {
        action = true;
        while (action == true)
        {
            float coolingTime = 1f;
            //float castingTime = currentSkill.skillStruct.castingTime;
            //if (castingTime > 0f)
            //    yield return StartCoroutine(SkillCasting(castingTime));// 캐스팅

            //float coolingTime = currentSkill.skillStruct.coolingTime;
            //currentSkill.startTime = Time.time + coolingTime;
            //Debug.LogWarning("State_Attacking!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            //float actionTime = unitAnimation.PlayAnimation(3);// 애니메이션
            //OutOfControll(actionTime);// 공격하는 동안 대기
            yield return new WaitForSeconds(coolingTime);

            //float coolingTime = currentSkill.skillStruct.coolingTime;
            //yield return new WaitForSeconds(coolingTime);
        }
    }

    IEnumerator State_StopActing()
    {
        action = false;
        while (state == State.Action)
        {
            //if (outOfControll == false)
                StateMachine(State.Idle);
            yield return null;
        }
    }
    //================================================================================================================================================
    // 홀드
    //================================================================================================================================================

    //bool outOfControll = false;

    public void OutOfControll(bool _isOn)
    {
        //outOfControll = _isOn;
    }

    //void OutOfControllTimer(float _time)
    //{
    //    StartCoroutine(HoldControllTimer(_time));
    //}

    //IEnumerator HoldControllTimer(float _time)
    //{
    //    outOfControll = true;
    //    yield return new WaitForSeconds(_time);
    //    outOfControll = false;
    //}
















    //================================================================================================================================================
    // 낚시
    //================================================================================================================================================

    public void SetControll()
    {
        state = State.Idle;
        //dirction = Vector2Int.zero;

        //SetKeyCode();
    }

    public void RemoveControll()
    {
        state = State.Idle;
        //dirction = Vector2Int.zero;

        //RemoveKeyCode();
    }
}