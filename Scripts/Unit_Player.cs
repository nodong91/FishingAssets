
using System;
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
    }
    public State state = State.None;

    public float moveSpeed = 0.01f;
    public Vector2 dirction;

    private void Start()
    {
        StateMachine(State.Idle);
    }

    //================================================================================================================================================
    // ��Ʈ��
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
    // �̵�
    //================================================================================================================================================

    public void StateMove(Vector2 _dirction)
    {
        dirction = _dirction;
        if (state == State.Idle)
        {
            StateMachine(State.Move);
        }
        if (state == State.Move)// �����̳� ȸ�ǰ� ���� �� ������
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

    void CheckClosestUnit()// �������̳� ä�� ������ �ϱ� ���� üũ
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

    public AnimationCurve rotateCurve;// ���Ʒ� ��鸱 �� �����̼�

    private void Update()
    {
        SetOceanRenderer();
    }

    void SetOceanRenderer()
    {
        runningTime += Time.deltaTime * waveSpeed;

        float moveHight = (Mathf.Sin(runningTime) + 1f) * 0.5f;// ���Ʒ� ������
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
        playerObject.transform.localRotation = Quaternion.Slerp(prevAngle, setAngle, curve / randomTime);// ���� ȸ��

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
        GameObject focusTarget = Game_Manager.current.cameraManager.focusTarget;

        focusTarget.transform.position = transform.position;
        Vector3 dir = new Vector3(dirction.x, 0f, dirction.y);
        Vector3 target = transform.position + focusTarget.transform.TransformDirection(dir).normalized;

        float speed = moveSpeed * Time.deltaTime;
        Vector3 offset = (target - transform.position).normalized;
        transform.position = Vector3.Lerp(transform.position, target, speed);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(offset), speed * 5f);
    }
    //================================================================================================================================================
    // �浹
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
    // ȸ��
    //================================================================================================================================================

    public void StateEscape()
    {

    }
    //================================================================================================================================================
    // �׼�
    //================================================================================================================================================

    Coroutine stateAction;
    //bool action = false;

    public void EventAction()// ���� �̺�Ʈ
    {

    }

    public void State_Action(bool _input)// Ŭ�� �̺�Ʈ
    {
        if (_input == true)
        {
            if (closestTarget == null)// ����
            {
                string id = "Fs_1001";
                FishStruct fishStruct = Singleton_Data.INSTANCE.Dict_Fish[id];
                Game_Manager.current.fishingManager.StartGame(fishStruct);
            }
            else
            {
                closestTarget.TriggerAction();
                triggerGameObject.Remove(closestTarget);
                closestTarget = null;
                Game_Manager.current.followManager.AddClosestTarget(null);// �ȷο� ������ ����
            }
        }
    }

    //================================================================================================================================================
    // ����
    //================================================================================================================================================

}