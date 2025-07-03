using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Fishing_Main : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public enum FishingState
    {
        None,
        Hit,
        MainGame,
        SubGame,
        Complate
    }
    public FishingState state;
    public TMPro.TMP_Text stateText;
    public Image fishingRod, fishObject, health;
    public Transform fish;
    public ParticleSystem targetParticles;
    public GameObject hitImage;

    public bool hitBool = false;
    float fillAmount;
    float fillAngle;
    float targetAngle;
    Coroutine actionCoroutine;

    bool fishingAction;
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

    public Data_Manager.FishStruct fishStruct;

    public delegate void DeleEndGame();
    public DeleEndGame deleEndGame;

    public void StartGame()
    {
        canvasGroup.gameObject.SetActive(true);
    }

    void EndGame()
    {
        deleEndGame?.Invoke();
        canvasGroup.gameObject.SetActive(false);
    }

    public void Action()
    {
        EndGame();
    }

    void Start()
    {
        fishingRod.material = Instantiate(fishingRod.material);
        health.material = Instantiate(health.material);
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
        StateMachine(FishingState.MainGame);
        Debug.LogWarning("poujpok");
    }

    void InputMouseLeft(bool _input)
    {
        switch (state)
        {
            case FishingState.None:

                break;

            case FishingState.Hit:

                break;

            case FishingState.MainGame:
                if (_input == true)
                {
                    RotateTarget(fishingLodStruct.reelingSpeed);
                }
                else
                {
                    RotateTarget(0);
                }
                break;

            case FishingState.SubGame:

                break;
        }
    }

    void InputMouseRight(bool _input)
    {
        switch (state)
        {
            case FishingState.MainGame:

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

    public void ResetGame(FishingLodStruct _fishingLodStruct, Data_Manager.FishStruct _fishStruct)
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

            case FishingState.Hit:

                break;

            case FishingState.MainGame:
                canvasGroup.gameObject.SetActive(true);
                StateFishing();
                break;

            case FishingState.SubGame:

                break;

            case FishingState.Complate:
                StateComplate();
                break;
        }
    }

    void StateNone()
    {
        SetFishing();
        canvasGroup.gameObject.SetActive(false);
    }

    void SetFishing()
    {
        fillAmount = fishingLodStruct.fishingAmount;
        fishPower = fishingLodStruct.lodPower + fishStruct.fishPower;
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
