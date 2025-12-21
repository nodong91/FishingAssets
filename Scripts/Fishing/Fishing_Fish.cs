using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Fishing_Fish : MonoBehaviour
{
    public Data_Manager.FishStruct currentFish;
    public enum StateType
    {
        None,
        Idle,
        Move,
        Attack,
        Groggy,
        Death
    }
    public StateType state;

    Vector3 fishTargetPoint;
    public float fishSpeed;
    const float fieldRadius = 9f;
    public float GetFieldRadius { get { return fieldRadius; } }

    public NavMeshAgent fishAgent;
    public GameObject formObject;
    public float addAngle;
    const float searchRadius = 1f;
    bool onCooling = false;
    Coroutine coroutineSpell, coroutineState;

    public delegate void DeleFillAmount(float amount);
    public DeleFillAmount deleFillAmount;
    public delegate void DeleHandler(bool _isOn);
    public DeleHandler deleStartSpell;
    public delegate void DeleDestroy();
    public DeleDestroy deleDestroy;

    private void Start()
    {
        fishAgent.updateRotation = false;
        fishAgent.gameObject.SetActive(false);
    }

    public void SetStart(Data_Manager.FishStruct _fishStruct)
    {
        currentFish = _fishStruct;
        fishAgent.gameObject.SetActive(true);
        fishAgent.acceleration = _fishStruct.fishSpeed * 2f;
        FishState(StateType.Idle);
        StateCooling();
    }

    public void SetFish(Vector3 _position)
    {
        fishAgent.transform.position = _position;
    }

    public delegate void DeleGetState(StateType _state);
    public DeleGetState deleGetState;

    void FishState(StateType _state)
    {
        deleGetState?.Invoke(_state);

        if (coroutineState != null)
            StopCoroutine(coroutineState);

        fishAgent.avoidancePriority = (int)_state;
        state = _state;
        switch (_state)
        {
            case StateType.None:
                break;
            case StateType.Idle:
                State_Idle();
                break;
            case StateType.Move:
                State_Move();
                break;
            case StateType.Attack:
                State_Attack();
                break;
            case StateType.Groggy:
                State_Groggy();
                break;
            case StateType.Death:
                break;
        }
    }

    void State_Idle()
    {
        coroutineState = StartCoroutine(FishIdle());
    }
    IEnumerator FishIdle()
    {
        float prevSpeed = fishSpeed;
        while (fishSpeed > 0f)
        {
            if (IsAgentStopped() == true)
            {
                fishSpeed = 0f;
            }
            else
            {
                fishSpeed = Mathf.Clamp(fishSpeed -= Time.deltaTime, 0f, prevSpeed);
                fishAgent.speed = fishSpeed;
            }
            MoveTargetPoint(fishAgent.steeringTarget);
            yield return null;
        }
        yield return new WaitForSeconds(1f);// 얼마나 오래 정지해있는지
        FishState(StateType.Move);

        bool attacking = currentFish.fishCoolTime * currentFish.fishSpellTime > 0f;
        if (attacking && onCooling == true)
        {
            State_Spell();// 움직이면서 스펠
        }
    }

    // 에이전트가 정지했는지 확인하는 함수
    public bool IsAgentStopped()
    {
        // 1. 경로 계산 중이 아닌지 확인
        if (!fishAgent.pathPending)
        {
            // 2. 남은 거리가 stoppingDistance 이하인지 확인
            if (fishAgent.remainingDistance <= fishAgent.stoppingDistance)
            {
                // 3. 경로가 없거나, 속도가 0인지 확인 (도달했거나, 경로를 잃었거나)
                if (!fishAgent.hasPath || fishAgent.velocity.sqrMagnitude == 0f)
                {
                    // 4. 최종적으로 완전히 정지했다고 판단
                    return true;
                }
            }
        }
        return false;
    }

    void State_Move()
    {
        coroutineState = StartCoroutine(FishMoving());
    }

    IEnumerator FishMoving()
    {
        Vector3 transformPoint = new Vector3(formObject.transform.position.x, 0f, formObject.transform.position.z);
        Vector3 randomPoint = SetRandomPosition();

        fishTargetPoint = transformPoint + randomPoint;
        fishTargetPoint = GetNavPosition(fishTargetPoint);
        fishAgent.SetDestination(fishTargetPoint);

        float prevSpeed = fishSpeed;
        float randomSpeed = Random.Range(currentFish.fishSpeed * 0.8f, currentFish.fishSpeed);
        float randomTime = 10f / randomSpeed * (currentFish.fishLazy + 1f);// 최고속도 상수
        //float dist = Vector3.Distance(fishTargetPoint, fishAgent.transform.position);
        bool active = true;
        float normalize = 0f;
        while (active == true)
        {
            normalize += Time.deltaTime * randomSpeed;
            fishSpeed = Mathf.Lerp(prevSpeed, randomSpeed, normalize);
            fishAgent.speed = fishSpeed;
            MoveTargetPoint(fishAgent.steeringTarget);// 에이전트가 현재 조향하고 있는 경로의 다음 지점
            yield return null;

            if (IsAgentStopped() == true || normalize > randomTime)// 정지하거나 랜덤타임일때
            {
                active = false;
            }
        }
        yield return null;

        float randomValue = Random.Range(0f, 1f);
        if (randomValue < currentFish.fishLazy)// 얼마나 자주 제자리에 있는지
            FishState(StateType.Idle);
        else
            FishState(StateType.Move);
    }

    void MoveTargetPoint(Vector3 nextCornerPoint)
    {
        Vector3 fishOffset = (nextCornerPoint - fishAgent.transform.position);
        Quaternion targetPoint = Quaternion.LookRotation(fishOffset.normalized);

        fishAgent.transform.rotation = Quaternion.Slerp(fishAgent.transform.rotation, targetPoint, Time.deltaTime * currentFish.fishSpeed);// 이동하면서 회전은 약간 느리게

    }

    Vector3 SetRandomPosition()
    {
        Vector2 randomPoint = Random.insideUnitCircle * fieldRadius;
        Vector3 position = new Vector3(randomPoint.x, 0f, randomPoint.y);
        return position;
    }

    void State_Spell()
    {
        if (coroutineSpell != null)
            StopCoroutine(coroutineSpell);
        coroutineSpell = StartCoroutine(FishSpell());
    }

    IEnumerator FishSpell()
    {
        deleStartSpell?.Invoke(true);
        onCooling = false;// 스펠 시작하면 공격 실패를 하더라도 쿨타임이 돌아가게
        float normalize = 0f;
        while (normalize < currentFish.fishSpellTime)
        {
            normalize += Time.deltaTime;
            deleFillAmount?.Invoke(normalize / currentFish.fishSpellTime);
            yield return null;
        }
        deleStartSpell?.Invoke(false);
        FishState(StateType.Attack);
    }

    void StateCooling()
    {
        if (coroutineSpell != null)
            StopCoroutine(coroutineSpell);
        coroutineSpell = StartCoroutine(FishCooling());
    }

    IEnumerator FishCooling()
    {
        yield return new WaitForSeconds(currentFish.fishCoolTime);
        onCooling = true;
    }

    void State_Attack()
    {
        coroutineState = StartCoroutine(FishAttack());
    }

    IEnumerator FishAttack()
    {
        Vector2 start = new Vector2(formObject.transform.position.x, formObject.transform.position.z);
        Vector2 end = new Vector2(fishAgent.transform.position.x, fishAgent.transform.position.z);
        float getAngle = GetAngle(start, end) + addAngle;// 각도 구해서
        float getDistance = Vector3.Distance(formObject.transform.position, fishAgent.transform.position);// 반대편에 위치 구하기
        Vector3 targetPoint = GetPosition(getDistance, getAngle);
        targetPoint = GetNavPosition(targetPoint);

        Vector3 prevPoint = fishAgent.transform.position;
        fishAgent.enabled = false;
        bool succeeded = false;
        float normalize = 0f;
        while (normalize < 1f)
        {
            float speed = 1f - normalize;
            normalize += Time.deltaTime * speed * 5f;
            //fishPrefab.transform.Translate(Vector3.forward * Time.deltaTime * fishSpeed, Space.Self);
            fishAgent.transform.position = Vector3.Lerp(prevPoint, targetPoint, normalize);
            MoveTargetPoint(targetPoint);
            yield return null;

            getDistance = Vector3.Distance(formObject.transform.position, fishAgent.transform.position);// 반대편에 위치 구하기
            if (getDistance < 0.5f && succeeded == false)
            {
                succeeded = true;
                deleDestroy?.Invoke();
            }

            if (speed < 0.01f)
                normalize = 1f;
        }
        fishAgent.enabled = true;
        FishState(StateType.Idle);
        StateCooling();// 스킬 쿨링 시작
    }

    Vector3 GetNavPosition(Vector3 _position)
    {
        if (NavMesh.SamplePosition(_position, out NavMeshHit navHit, searchRadius, NavMesh.AllAreas))
            _position = navHit.position;
        return _position;
    }

    float GetAngle(Vector2 start, Vector2 end)
    {
        Vector2 v2 = end - start;
        return Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
    }

    Vector3 GetPosition(float radius, float angle)
    {
        Vector3 center = formObject.transform.position;
        float radian = angle * Mathf.Deg2Rad; // 각도를 라디안으로 변환
        float x = radius * Mathf.Cos(radian);
        float y = radius * Mathf.Sin(radian);
        Vector3 position = center + new Vector3(x, 0, y);
        return position;
    }

    public void Interfere()// 방해성공
    {
        if (coroutineSpell != null)
            StopCoroutine(coroutineSpell);
        FishState(StateType.Groggy);
    }

    void State_Groggy()
    {
        deleStartSpell?.Invoke(false);
        fishTargetPoint = GetNavPosition(fishAgent.transform.position);
        fishAgent.SetDestination(fishTargetPoint);
        coroutineState = StartCoroutine(FishGroggy());
    }

    IEnumerator FishGroggy()
    {
        yield return new WaitForSeconds(currentFish.fishGroggyTime);
        StateCooling();// 스킬 쿨링 시작
        FishState(StateType.Idle);
    }

    public void FishingComplate()
    {
        FishState(StateType.None);
        fishAgent.gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(formObject.transform.position, Vector3.up, fieldRadius);

        UnityEditor.Handles.color = Color.red;

        //Gizmos.DrawSphere( fishAgent.steeringTarget, 0.3f);
        Gizmos.DrawLine(fishTargetPoint, fishAgent.transform.position);
        float distance = (fishTargetPoint - fishAgent.transform.position).magnitude;
        Vector3 lerfVector = Vector3.Lerp(fishAgent.transform.position, fishTargetPoint, 0.5f);
        UnityEditor.Handles.Label(lerfVector, distance.ToString());

        if (fishAgent != null)
        {
            int length = fishAgent.path.corners.Length;
            for (int i = 0; i < length; i++)
            {
                Gizmos.DrawSphere(fishAgent.path.corners[i], 0.3f);
            }
        }

        Gizmos.DrawLine(formObject.transform.position, fishAgent.transform.position);
        Vector2 start = new Vector2(formObject.transform.position.x, formObject.transform.position.z);
        Vector2 end = new Vector2(fishAgent.transform.position.x, fishAgent.transform.position.z);
        float getAngle = GetAngle(start, end) + addAngle;
        float getDistance = Vector3.Distance(formObject.transform.position, fishAgent.transform.position);
        Gizmos.DrawLine(formObject.transform.position, GetPosition(getDistance, getAngle));
    }
#endif
}
