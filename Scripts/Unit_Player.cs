using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Unit_Player : MonoBehaviour
{
    [Flags]
    public enum ControllDirection
    {
        None = 0, W = 1 << 0, A = 1 << 1, S = 1 << 2, D = 1 << 3
    }
    public ControllDirection controllDirection = ControllDirection.None;

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

    public Vector2Int dirction;
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
        Singleton_Controller.INSTANCE.key_MouseLeft += InputMousetLeft;
        //Singleton_Controller.INSTANCE.key_MouseRight += InputMouseRight;
        //Singleton_Controller.INSTANCE.key_MouseWheel += InputMouseWheel;
    }

    void RemoveMouse()
    {
        Singleton_Controller.INSTANCE.key_MouseLeft -= InputMousetLeft;
        //Singleton_Controller.INSTANCE.key_MouseRight += InputMouseRight;
        //Singleton_Controller.INSTANCE.key_MouseWheel += InputMouseWheel;
    }

    void SetKeyCode()
    {
        Singleton_Controller.INSTANCE.key_W += Direction_UP;
        Singleton_Controller.INSTANCE.key_A += Direction_Left;
        Singleton_Controller.INSTANCE.key_S += Direction_Down;
        Singleton_Controller.INSTANCE.key_D += Direction_Right;

        Singleton_Controller.INSTANCE.key_SpaceBar += Key_SpaceBar;
        Singleton_Controller.INSTANCE.key_1 += Key_1;
        Singleton_Controller.INSTANCE.key_2 += Key_2;
    }

    void RemoveKeyCode()
    {
        Singleton_Controller.INSTANCE.key_W -= Direction_UP;
        Singleton_Controller.INSTANCE.key_A -= Direction_Left;
        Singleton_Controller.INSTANCE.key_S -= Direction_Down;
        Singleton_Controller.INSTANCE.key_D -= Direction_Right;

        Singleton_Controller.INSTANCE.key_SpaceBar -= Key_SpaceBar;
        Singleton_Controller.INSTANCE.key_1 -= Key_1;
        Singleton_Controller.INSTANCE.key_2 -= Key_2;
    }

    //================================================================================================================================================
    // 컨트롤
    //================================================================================================================================================

    void InputMousetLeft(bool _input)
    {
        State_Action(_input);
    }

    void Direction_UP(bool _input)
    {
        if (_input == true)
        {
            controllDirection |= ControllDirection.W;// 넣기
            //dirction.y += 1;
        }
        else
        {
            controllDirection &= ~ControllDirection.W;// 제외
            //dirction.y -= 1;
        }
        StateMove();
    }

    void Direction_Left(bool _input)
    {
        if (_input == true)
        {
            controllDirection |= ControllDirection.A;
            //dirction.x -= 1;
        }
        else
        {
            controllDirection &= ~ControllDirection.A;
            //dirction.x += 1;
        }
        StateMove();
    }

    void Direction_Down(bool _input)
    {
        if (_input == true)
        {
            controllDirection |= ControllDirection.S;
            //dirction.y -= 1;
        }
        else
        {
            controllDirection &= ~ControllDirection.S;
            //dirction.y += 1;
        }
        StateMove();
    }

    void Direction_Right(bool _input)
    {
        if (_input == true)
        {
            controllDirection |= ControllDirection.D;
            //dirction.x += 1;
        }
        else
        {
            controllDirection &= ~ControllDirection.D;
            //dirction.x -= 1;
        }
        StateMove();
    }

    void Key_1(bool _input)
    {
        if (_input == true)
            QuickSlot(0);
    }

    void Key_2(bool _input)
    {
        if (_input == true)
            QuickSlot(1);
    }

    public void QuickSlot(int _index)
    {
        //Game_Manager.current.QuickSlotAction(_index);
    }

    void Key_SpaceBar(bool _input)
    {
        // 방어 스킬
        // 방패를 가진 무기는 방패막기
        // 양손검은 패링
        // 한손검은 구르기
        // 무기 특징
        if (_input == true)
        {
            StateEscape();
        }
    }

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
                RemoveKeyCode();
                //DeadState();
                outOfControll = true;
                break;
            case State.Action:
                break;
            case State.Idle:
                if (controllDirection != ControllDirection.None)
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

    void SetDirection()
    {
        if (controllDirection.HasFlag(ControllDirection.W) == false && controllDirection.HasFlag(ControllDirection.S) == false)
        {
            dirction.y = 0;
        }
        else if (controllDirection.HasFlag(ControllDirection.W))
        {
            dirction.y = 1;
        }
        else if (controllDirection.HasFlag(ControllDirection.S))
        {
            dirction.y = -1;
        }
        if (controllDirection.HasFlag(ControllDirection.A) == false && controllDirection.HasFlag(ControllDirection.D) == false)
        {
            dirction.x = 0;
        }
        else if (controllDirection.HasFlag(ControllDirection.A))
        {
            dirction.x = -1;
        }
        else if (controllDirection.HasFlag(ControllDirection.D))
        {
            dirction.x = 1;
        }
    }

    void StateMove()
    {
        SetDirection();

        if (outOfControll == true)
            return;

        if (state == State.Idle)
        {
            StateMachine(State.Move);
        }
        if (state == State.Move)// 공격이나 회피가 있을 수 있으니
        {
            if (controllDirection == ControllDirection.None)
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

    void CheckClosestUnit()// 아이템이나 채집 같은거 하기 위한 테스트
    {
        if (triggerGameObject.Count == 0)
            return;

        closestDistance = float.MaxValue;
        Fishing_Setting tempTarget = null;
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
        Game_Manager.current.uiManager.followManager.AddClosestTarget(closestTarget);
    }

    void SetMoving()
    {
        if (outOfControll == true)
            return;

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
    public List<Fishing_Setting> triggerGameObject = new List<Fishing_Setting>();
    public Fishing_Setting closestTarget;
    public float closestDistance;

    private void OnTriggerEnter(Collider other)
    {
        Fishing_Setting fishing = other.GetComponent<Fishing_Setting>();
        if (fishing == null)
            return;
        triggerGameObject.Add(fishing);
    }

    private void OnTriggerExit(Collider other)
    {
        Fishing_Setting fishing = other.GetComponent<Fishing_Setting>();
        if (fishing == null)
            return;

        triggerGameObject.Remove(fishing);
        if (triggerGameObject.Count == 0)
        {
            closestTarget = null;
            Game_Manager.current.uiManager.followManager.AddClosestTarget(null);
        }
    }

    //================================================================================================================================================
    // 회피
    //================================================================================================================================================

    void StateEscape()
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

    void State_Action(bool _input)
    {
        if (_input == true)
        {
            if (outOfControll == true)
                return;

            StateMachine(State.Action);
            stateAction = StartCoroutine(State_Acting());
        }
        else
        {
            if (closestTarget != null)
            {
                // 낚시 시작
                Game_Manager.current.uiManager.followManager.AddClosestTarget(null);
                Game_Manager.current.fishingManager.StartGame(closestTarget);
                triggerGameObject.Remove(closestTarget);
                closestTarget = null;

                RemoveControll();
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
            if (outOfControll == false)
                StateMachine(State.Idle);
            yield return null;
        }
    }
    //================================================================================================================================================
    // 홀드
    //================================================================================================================================================

    public bool outOfControll = false;
    void OutOfControll(float _time)
    {
        StartCoroutine(HoldControll(_time));
    }

    IEnumerator HoldControll(float _time)
    {
        outOfControll = true;
        yield return new WaitForSeconds(_time);
        outOfControll = false;
    }
















    //================================================================================================================================================
    // 낚시
    //================================================================================================================================================

    public void SetControll()
    {
        state = State.Idle;
        dirction = Vector2Int.zero;

        SetMouse();
        SetKeyCode();
    }

    public void RemoveControll()
    {
        state = State.Idle;
        dirction = Vector2Int.zero;

        RemoveMouse();
        RemoveKeyCode();
    }
}