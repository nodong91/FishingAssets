using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FishingGame : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public enum FishingState
    {
        None,
        Bobber,
        Hit,
        Complate
    }
    public FishingState state;
    public TMPro.TMP_Text stateText;
    public Image fishingRod, hitPointImage, fishObject, health;
    public Transform fish;
    public ParticleSystem targetParticles;
    public Image bobber;// 낚시찌
    public GameObject hitImage;

    public bool hitBool = false;
    float fillAmount;
    float fillAngle;
    float targetAngle;
    Coroutine actionCoroutine;

    bool fishingAction;
    float hitPoint;
    float bobberSpeed;
    float currentSpeed = 0f;
    float fishPower;
    float complatePoint;// 완료 퍼센트

    [Header("[ 변경 가능한 수치 ]")]
    public FishingLodStruct fishingLodStruct;
    [System.Serializable]
    public struct FishingLodStruct
    {
        public float fishingAmount;
        public float lodPower;// 초당 끌려가는 힘 - 높을 수록 쉽게 끌려감
        public float reelingSpeed;// 낚시 회전 속도
        public float reelingSlip;// 릴링 정지 시 밀림
        public float hitPoint;// 물고기 잡을 위치
        public float hitBobberSpeed;// 물고기 찌 움직임
    }

    public FishStruct fishStruct;
    [System.Serializable]
    public struct FishStruct
    {
        public int fishLevel;
        public float fishPower;// 초당 끌려가는 힘 - 높을 수록 쉽게 끌려감
        public float fishAddAngle;// 물고기 이동 각도
        public float fishSpeed;
        public Vector2 fishDelay;// 방향 바뀌는 딜레이 시간
        public float fishBobberLength;// 최소 0~1까지
    }

    void Start()
    {
        fishingRod.material = Instantiate(fishingRod.material);
        hitPointImage.material = Instantiate(hitPointImage.material);
        health.material = Instantiate(health.material);
        bobber.material = Instantiate(bobber.material);
        StateMachine(FishingState.None);
        SetMouse();
    }

    void SetMouse()
    {
        Singleton_Controller.INSTANCE.key_MouseLeft += InputMouseLeft;
        Singleton_Controller.INSTANCE.key_MouseRight += InputMouseRight;
    }

    public void StartFishing()
    {
        if (state != FishingState.None)
            return;

        // 낚시 가능한 위치
        StateMachine(FishingState.Bobber);
        Debug.LogWarning("poujpok");
    }

    void InputMouseLeft(bool _input)
    {
        switch (state)
        {
            case FishingState.None:
                //if (_input == true)
                //{
                //    // 낚시 가능한 위치
                //    StateMachine(FishingState.Bobber);
                //}
                break;

            case FishingState.Bobber:
                if (_input == true)
                {
                    if (hitBool == true)
                    {
                        StateMachine(FishingState.Hit);
                    }
                    else
                    {
                        StateMachine(FishingState.None);
                    }
                }
                break;

            case FishingState.Hit:
                if (_input == true)
                {
                    RotateTarget(fishingLodStruct.reelingSpeed);
                }
                else
                {
                    RotateTarget(0);
                }
                break;
        }
    }

    void InputMouseRight(bool _input)
    {
        switch (state)
        {
            case FishingState.Hit:

                if (_input == true)
                {
                    RotateTarget(-fishingLodStruct.reelingSpeed);
                }
                else
                {
                    RotateTarget(0);
                }
                break;
        }
    }

    public void ResetGame(FishingLodStruct _fishingLodStruct, FishStruct _fishStruct)
    {
        fishingLodStruct = _fishingLodStruct;
        fishStruct = _fishStruct;
        StateMachine(FishingState.None);
    }

    void StateMachine(FishingState _state)
    {
        state = _state;
        stateText.text = _state.ToString();
        if (actionCoroutine != null)
            StopCoroutine(actionCoroutine);

        switch (state)
        {
            case FishingState.None:
                StateNone();
                break;

            case FishingState.Bobber:
                StateBobber();
                break;

            case FishingState.Hit:
                StateFishing();
                break;

            case FishingState.Complate:
                StateComplate();
                break;
        }
    }

    void StateNone()
    {
        SetFishing();
        hitPointImage.material.SetFloat("_FillAmount", 1f - hitPoint);
        canvasGroup.gameObject.SetActive(false);
    }

    void SetFishing()
    {
        fillAmount = fishingLodStruct.fishingAmount;
        hitPoint = fishingLodStruct.hitPoint;
        bobberSpeed = fishingLodStruct.hitBobberSpeed - (fishStruct.fishLevel * 0.1f);
        fishPower = fishingLodStruct.lodPower + fishStruct.fishPower;
    }

    //==================================================================================================================================
    // 히트포인트
    //==================================================================================================================================

    void StateBobber()
    {
        actionCoroutine = StartCoroutine(BobberMovement());
        canvasGroup.gameObject.SetActive(true);
    }

    IEnumerator BobberMovement()
    {
        hitImage.SetActive(false);
        hitBool = false;
        float runningTime = 0f;
        float yPos = 0f;
        bool bobbering = true;
        float length = 0f;

        while (bobbering == true)
        {
            runningTime += bobberSpeed * Time.deltaTime;
            float mathfSin = (Mathf.Sin(runningTime) + 1f) / 2f;
            yPos = mathfSin * length;
            if (yPos > hitPoint)
            {
                hitBool = true;
                hitImage.SetActive(true);
            }
            bobber.material.SetFloat("_FillAmount", yPos);
            yield return null;

            Debug.LogWarning(mathfSin);
            if (yPos < 0.0001f)
            {
                length = Random.Range(fishStruct.fishBobberLength, 1f);
            }

            if (hitBool == true && yPos < hitPoint)
            {
                StateMachine(FishingState.None);
            }
        }
    }

    //==================================================================================================================================
    // 릴링
    //==================================================================================================================================

    private void StateFishing()
    {
        complatePoint = 0.1f;// 기본 포인트 10%
        fish.rotation = Quaternion.Euler(0f, 0f, 0f);
        fishingRod.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        StartCoroutine(FishPosition());
        RotateTarget(0);
    }

    void RotateTarget(float _targetSpeed)
    {
        if (actionCoroutine != null)
            StopCoroutine(actionCoroutine);
        actionCoroutine = StartCoroutine(RotateTargeting(_targetSpeed));
    }

    IEnumerator RotateTargeting(float _targetSpeed)
    {
        while (fishingAction == true)
        {
            fish.rotation = Quaternion.Slerp(fish.rotation, Quaternion.Euler(0, 0, targetAngle), fishStruct.fishSpeed * Time.deltaTime);// 물고기 움직임 추적

            currentSpeed = Mathf.Lerp(currentSpeed, _targetSpeed, fishingLodStruct.reelingSlip * Time.deltaTime);// 가속도
            fishingRod.transform.rotation = Quaternion.Euler(0, 0, fishingRod.transform.rotation.eulerAngles.z + currentSpeed * Time.deltaTime);

            health.material.SetFloat("_FillAmount", complatePoint);

            fillAngle = 180f * fillAmount;
            fishingRod.material.SetFloat("_FillAmount", fillAmount);
            fishingRod.material.SetFloat("_RotateAngle", fillAngle + 180f);

            var main = targetParticles.main;
            if (VisibleTarget() == true)
            {
                main.startColor = Color.green;
                if (complatePoint < 1f)
                {
                    complatePoint += fishPower * Time.deltaTime;
                }
                else
                {
                    StateMachine(FishingState.Complate);
                }
            }
            else
            {
                main.startColor = Color.red;
                if (complatePoint > 0f)
                {
                    complatePoint -= fishPower * Time.deltaTime;
                }
                //else
                //{
                //    fishingAction = false;
                //    StateMachine(FishingState.None);
                //}

                //if (fillAmount > 0.0f)
                //{
                //    fillAmount -= 0.01f * Time.deltaTime;
                //}
                //else
                //{
                //    fishingAction = false;
                //    StateMachine(FishingState.None);
                //}
            }
            complatePoint = Mathf.Clamp01((float)complatePoint);
            yield return null;
        }
    }

    IEnumerator FishPosition()
    {
        fishingAction = true;
        while (fishingAction == true)
        {
            float addAngle = Random.Range(-fishStruct.fishAddAngle, fishStruct.fishAddAngle);
            targetAngle += addAngle;
            float random = Random.Range(fishStruct.fishDelay.x, fishStruct.fishDelay.y);
            yield return new WaitForSeconds(random);
        }
    }

    bool VisibleTarget()// 보이는지 확인
    {
        Vector3 offset = (fishObject.transform.position - fishingRod.transform.position);
        //Debug.LogWarning(Vector3.Angle(test.transform.up, offset.normalized));
        if (Vector3.Angle(fishingRod.transform.up, offset.normalized) < fillAngle)// 앵글 안에 포함 되는지
        {
            return true;
        }
        return false;
    }

    void StateComplate()
    {
        actionCoroutine = StartCoroutine(FishingComplate());
    }

    IEnumerator FishingComplate()
    {
        int index = 0;
        while (index < 3)
        {
            index++;
            stateText.text = index.ToString();
            yield return new WaitForSeconds(1f);
        }
        StateMachine(FishingState.None);
    }

    //==================================================================================================================================
    // 기즈모
    //==================================================================================================================================

    public float arcSize;
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.green;
        Handles.DrawWireArc(fishingRod.transform.position, Vector3.forward, Vector3.up, 360f, arcSize);

        Vector3 viewAngleA = DirFromAngle(fillAngle, fishingRod.transform);
        Vector3 viewAngleB = DirFromAngle(-fillAngle, fishingRod.transform);

        Handles.DrawLine(fishingRod.transform.position, fishingRod.transform.position + viewAngleA * arcSize);
        Handles.DrawLine(fishingRod.transform.position, fishingRod.transform.position + viewAngleB * arcSize);

        Handles.DrawLine(fishObject.transform.position, fishingRod.transform.position);
    }

    Vector3 DirFromAngle(float _angleInDegrees, Transform _trans = null)
    {
        if (_trans != null)
        {
            // 로컬 기준
            _angleInDegrees += _trans.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(_angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(_angleInDegrees * Mathf.Deg2Rad), 0);
    }
#endif
}