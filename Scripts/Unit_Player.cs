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

    Data_Manager.SetStatus status;
    public float moveSpeed = 1f;
    public int health;
    public float energy;
    public float efficient;// 에너지 효율
    private Vector2 dirction;
    // 물위에서 배의 움직임
    private float shipHight = -0.1f;
    private float waveSpeed = 2f;
    private float targetAngle = 10f;
    float runningTime;
    public GameObject playerObject;
    Transform FocusTarget => Game_Manager.current?.cameraManager.GetFocusTarget;
    Coroutine stateAction;

    private List<Trigger_Setting> triggerGameObject = new List<Trigger_Setting>();
    private Trigger_Setting closestTarget;

    Quaternion prevAngle, setAngle;
    float randomTime, runningRandomTime;

    public AnimationCurve rotateCurve;// 위아래 흔들릴 때 로테이션

    private void Start()
    {
        StateMachine(State.Idle);
        if (FocusTarget == null)
            return;

        FocusTarget.position = transform.position;
    }

    public void SetStatus()
    {
        status = Game_Manager.current.currentStatus;
        moveSpeed = status.shipSpeed;
        health = status.shipHealth;
        energy = status.maxEnergy;
        efficient = status.catchPower;
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
            case State.Destroy:
                StateDestroy();
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

            energy -= status.efficient * Time.deltaTime;// 임시
            Game_Manager.current.GetMainUI.SetEnergy(energy / status.maxEnergy);
            if (energy <= 0)
            {
                StateMachine(State.Destroy);
            }
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
        Game_Manager.current.cameraManager.InputShake();// 카메라 흔들기
        float normalize = 0f;
        while (normalize < 1f)// 뒤로 밀려나기
        {
            normalize += Time.deltaTime;
            float speed = (1f - normalize) * 0.1f;
            transform.position = Vector3.Lerp(transform.position, _target, speed);
            CheckClosestUnit();// 가까운 트리거 체크
            yield return null;
        }

        if (health > 0)
            StateMachine(State.Idle);// 다시 대기 상태
        else
            StateMachine(State.Destroy);
    }

    void StateDestroy()
    {
        Data_Continue continueData = SaveData_Continue.current.continueData;
        // 위치
        transform.position = continueData.playerPosition;
        transform.rotation = continueData.playerRotation;
        transform.localScale = continueData.playerScale;
        FocusTarget.position = transform.position;

        SetStatus();
        StateMachine(State.Idle);// 다시 대기 상태
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
    //================================================================================================================================================
    // 충돌
    //================================================================================================================================================


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
            Debug.LogWarning("배 데미지~~~");
            if (health > 0)
            {
                health--;
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