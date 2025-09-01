using System.Collections;
using UnityEngine;

public class Fishing_Action : MonoBehaviour
{
    public Data_Manager manager;
    public Data_Manager.FishStruct fishStruct;
    public GameObject fishPrefab;
    public GameObject shipPrefab;
    public enum FishStateType
    {
        None,
        Idle,
        Spelling,
        Attack,
        Moving,
    }
    public FishStateType State;
    public float coolTime = 3f;
    public float cooling;
    public float randomTime;
    Vector3 fishTargetPoint;
    float spellTime = 2f;
    bool shake;
    public float shipSize = 1f;

    void Start()
    {
        cooling = Time.time + coolTime;
        fishStruct = manager.fishStruct[0];
        FishState(FishStateType.Idle);
    }

    void FishState(FishStateType _state)
    {
        State = _state;
        switch (State)
        {
            case FishStateType.Idle:
                State_Idle();
                break;
            case FishStateType.Spelling:
                // 시전중
                StartCoroutine(Spelling());
                break;
            case FishStateType.Attack:
                // 공격
                StartCoroutine(FishAttack());
                break;
            case FishStateType.Moving:
                StartCoroutine(Moving());
                break;
        }
    }

    void State_Idle()
    {
        float distance = (shipPrefab.transform.position - fishPrefab.transform.position).magnitude;
        float attackRange = shipSize + fishStruct.fieldRadius * 0.5f;
        if (cooling < Time.time && distance < attackRange && distance > attackRange * 0.5f)
        {
            FishState(FishStateType.Spelling);
        }
        else
        {
            FishState(FishStateType.Moving);
        }
    }

    IEnumerator Moving()
    {
        Vector2 range = fishStruct.fishRange;
        randomTime = Time.time + Random.Range(range.x, range.y);

        Vector3 tempPoint = Random.insideUnitSphere * fishStruct.fieldRadius;
        fishTargetPoint = new Vector3(tempPoint.x, 0f, tempPoint.z) + transform.position;

        float setTime = Random.Range(0.5f, 2f);
        float normalize = 0f;
        while (normalize < setTime)
        {
            normalize += Time.deltaTime;
            Vector3 fishDirection = (fishTargetPoint - fishPrefab.transform.position).normalized;
            Quaternion targetPoint = Quaternion.LookRotation(fishDirection);
            fishSpeed = Mathf.Lerp(fishSpeed, fishStruct.fishSpeed, normalize);
            fishPrefab.transform.rotation = Quaternion.Slerp(fishPrefab.transform.rotation, targetPoint, Time.deltaTime * fishStruct.fishSpeed);
            fishPrefab.transform.Translate(Vector3.forward * Time.deltaTime * fishSpeed, Space.Self);
            yield return null;
        }
        FishState(FishStateType.Idle);
    }
    float fishSpeed = 0f;

    IEnumerator Spelling()
    {
        Vector3 direction = (shipPrefab.transform.position - fishPrefab.transform.position).normalized;
        float normalize = 0f;
        while (normalize < spellTime)
        {
            normalize += Time.deltaTime;
            Quaternion rotation = Quaternion.LookRotation(direction);
            fishPrefab.transform.rotation = Quaternion.Slerp(fishPrefab.transform.rotation, rotation, Time.deltaTime * 5f);
            fishSpeed = Mathf.Lerp(fishStruct.fishSpeed, 0f, normalize);
            fishPrefab.transform.Translate(Vector3.forward * Time.deltaTime * fishSpeed, Space.Self);
            yield return null;
        }
        FishState(FishStateType.Attack);
    }

    IEnumerator FishAttack()// 발사
    {
        shake = false;
        float distance = (shipPrefab.transform.position - fishPrefab.transform.position).magnitude;
        float skillSpeed = distance * 5f;
        while (skillSpeed > 0.1f)
        {
            skillSpeed = Mathf.Lerp(skillSpeed, 0f, Time.deltaTime * 2f);
            fishPrefab.transform.Translate(Vector3.forward * Time.deltaTime * skillSpeed, Space.Self);
            distance = (shipPrefab.transform.position - fishPrefab.transform.position).magnitude;
            if (distance < shipSize && shake == false)
            {
                ShakeShip();
            }
            yield return null;
        }
        cooling = Time.time + coolTime;
        FishState(FishStateType.Idle);
    }

    void ShakeShip()
    {
        shake = true;
        StartCoroutine(ShakingShip());
    }

    IEnumerator ShakingShip()
    {
        Vector3 originPosition = shipPrefab.transform.position;
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 3f;
            Vector3 shakePosition = Random.insideUnitSphere * (1f - normalize);
            shipPrefab.transform.position = originPosition + shakePosition;
            yield return null;
        }
        shipPrefab.transform.position = originPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEditor.Handles.color = Color.yellow;
        Gizmos.DrawSphere(shipPrefab.transform.position, shipSize);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, fishStruct.fieldRadius);

        UnityEditor.Handles.color = Color.red;
        float attackRange = shipSize + fishStruct.fieldRadius * 0.5f;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, attackRange);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, attackRange * 0.5f);

        Gizmos.color = Color.blue;
        if (State == FishStateType.Moving)
        {
            Gizmos.DrawSphere(fishTargetPoint, 0.3f);
            Gizmos.DrawLine(fishTargetPoint, fishPrefab.transform.position);
        }
    }
}
