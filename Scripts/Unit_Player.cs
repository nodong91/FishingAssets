
//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Player : MonoBehaviour
{
    public enum State
    {
        None,
        Idle,
        Move,
        Damage,
    }
    public State state = State.None;

    public float moveSpeed = 0.01f;
    public Vector2 dirction;
    Transform FocusTarget => Game_Manager.current?.cameraManager.GetFocusTarget;

    private void Start()
    {
        StateMachine(State.Idle);
        if (FocusTarget == null)
            return;

        FocusTarget.position = transform.position;
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
            case State.Idle:
                if (dirction.x != 0f || dirction.y != 0f)
                    StateMachine(State.Move);
                break;
            case State.Move:
                stateAction = StartCoroutine(Moving());
                break;
            case State.Damage:

                break;
        }
    }

    //================================================================================================================================================
    // 이동
    //================================================================================================================================================

    public void StateMove(Vector2 _dirction)
    {
        dirction = _dirction;
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
            CheckClosestUnit();
            yield return null;
        }
    }

    void CheckClosestUnit()// 아이템이나 채집 같은거 하기 위한 체크
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
    //================================================================================================================================================
    //public Reflection_Manager reflection_Manager;
    public float shipHight, waveSpeed = 2f;
    float runningTime;
    public GameObject playerObject;

    Quaternion prevAngle, setAngle;
    public float targetAngle = 10f;
    float randomTime, runningRandomTime;

    public AnimationCurve rotateCurve;// 위아래 흔들릴 때 로테이션

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

        if (runningTime >= runningRandomTime)
        {
            randomTime = UnityEngine.Random.Range(5f, 3f);
            runningRandomTime = runningTime + randomTime;
            prevAngle = playerObject.transform.localRotation;
            setAngle = Quaternion.Euler(RandomAngle(targetAngle));
        }

        float curve = rotateCurve.Evaluate(1f - (runningRandomTime - runningTime) / randomTime);
        playerObject.transform.localRotation = Quaternion.Slerp(prevAngle, setAngle, curve / randomTime);// 랜덤 회전

        //// 배 부분 물결 안생기게
        //string shipPosition = "_ShipPosition";
        //reflection_Manager.GetMaterial.SetVector(shipPosition, playerObject.transform.position);
        //reflection_Manager.GetMaterial.SetFloat("_WaveSpeed", waveSpeed);
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
        //Transform focusTarget = Game_Manager.current.cameraManager.GetFocusTarget;
        FocusTarget.position = transform.position;

        Vector3 dir = new Vector3(dirction.x, 0f, dirction.y);
        Vector3 target = transform.position + FocusTarget.TransformDirection(dir).normalized;
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

    public void StateEscape()
    {

    }

    public void StateClash(Vector3 _offset)
    {
        Vector3 target = transform.position + _offset;
        if (state != State.Damage)
            StartCoroutine(MovingClash(target));

        Debug.LogWarning("충돌!!!!!!!!!!!!!!!!!!!!");
    }

    IEnumerator MovingClash(Vector3 _target)
    {
        StateMachine(State.Damage);

        Game_Manager.current.GetInventory.DistroySlot();// 랜덤 슬롯 부수기
        Game_Manager.current.cameraManager.InputShake();
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime;
            float speed = (1f - normalize) * 0.1f;
            transform.position = Vector3.Lerp(transform.position, _target, speed);
            CheckClosestUnit();
            yield return null;
        }
        StateMachine(State.Idle);
    }

    //================================================================================================================================================
    // 액션
    //================================================================================================================================================

    Coroutine stateAction;
    //bool action = false;

    public void EventAction()// 어택 이벤트
    {

    }

    public void State_Action(bool _input)// 클릭 이벤트
    {
        if (_input == true)
        {
            if (closestTarget != null)
            {
                closestTarget.TriggerAction();
                triggerGameObject.Remove(closestTarget);
                closestTarget = null;
                Game_Manager.current.GetFollow.AddClosestTarget(null);// 팔로우 유아이 제거
            }
        }
    }

    //================================================================================================================================================
    // 낚시
    //================================================================================================================================================
    public int HealthPoint;
    //================================================================================================================================================
    // 충돌
    //================================================================================================================================================

    public List<Trigger_Setting> triggerGameObject = new List<Trigger_Setting>();
    public Trigger_Setting closestTarget;

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
            Game_Manager.current.GetFollow.AddClosestTarget(null);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Finish")
        {
            if (HealthPoint > 0)
            {
                HealthPoint--;
                Vector3 offset = (transform.position - collision.transform.position).normalized;
                StateClash(offset);
            }
            else
            {
                Debug.LogWarning("배 파괴!!!!!!!!!!!!!!!!!!!!");
            }
        }
    }
}