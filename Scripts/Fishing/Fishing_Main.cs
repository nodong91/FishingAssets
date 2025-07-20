using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;

public class Fishing_Main : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    //public enum FishingState
    //{
    //    None,
    //    Hit,
    //    MainGame,
    //    SubGame,
    //    Complate
    //}
    //public FishingState state;
    public TMPro.TMP_Text stateText;
    public Image fishingRod, fishObject, health;
    public Transform fish;
    public ParticleSystem targetParticles;
    //public GameObject hitImage;

    public bool hitBool = false;
    float fillArea;
    float fillAngle;
    float targetAngle;
    Coroutine actionCoroutine;

    bool fishingAction;
    float currentSpeed = 0f;
    float complatePoint;// 완료 퍼센트

    [Header("[ 변경 가능한 수치 ]")]
    public EquipStruct equipStruct;
    public FishStruct fishStruct;

    public delegate void DeleEndGame(Fishing_Manager.FishingState _state);
    public DeleEndGame deleEndGame;

    public void SetStart()
    {
        canvasGroup.gameObject.SetActive(false);

        fishingRod.material = Instantiate(fishingRod.material);
        health.material = Instantiate(health.material);
        //StateMachine(FishingState.None);
    }

    public void StartGame(Data_Manager.FishStruct _fishStruct)
    {
        Debug.LogWarning("낚시대 테스트");
        equipStruct = Singleton_Data.INSTANCE.Dict_Equip["Eq_0001"];// 낚시대 테스트
        fishStruct = _fishStruct;
        StartGame();
    }

    public void StartGame()
    {
        //if (state != FishingState.None)
        //    return;

        canvasGroup.gameObject.SetActive(true);
        SetMouse();
        //// 낚시 가능한 위치
        //StateMachine(FishingState.MainGame);
        StateMachine();
    }

    void EndGame()
    {
        deleEndGame?.Invoke(Fishing_Manager.FishingState.Sub);// 끝나면 서브로 이동
        canvasGroup.gameObject.SetActive(false);
    }

    public void Action()
    {

    }

    void SetMouse()
    {
        Singleton_Controller.INSTANCE.key_MouseLeft += InputMouseLeft;
        Singleton_Controller.INSTANCE.key_MouseRight += InputMouseRight;
    }

    void RemoveMouse()
    {
        Singleton_Controller.INSTANCE.key_MouseLeft += InputMouseLeft;
        Singleton_Controller.INSTANCE.key_MouseRight += InputMouseRight;
    }

    void InputMouseLeft(bool _input)
    {
        if (_input == true)
        {
            RotateTarget(equipStruct.reelingSpeed);
        }
        else
        {
            RotateTarget(0);
        }
    }

    void InputMouseRight(bool _input)
    {
        if (_input == true)
        {
            RotateTarget(-equipStruct.reelingSpeed);
        }
        else
        {
            RotateTarget(0);
        }
    }

    public void ResetGame(Data_Manager.EquipStruct _equipStruct, Data_Manager.FishStruct _fishStruct)
    {
        equipStruct = _equipStruct;
        fishStruct = _fishStruct;
        //StateMachine(FishingState.None);
    }

    void StateMachine()
    {
        //state = _state;
        //stateText.text = _state.ToString();
        if (actionCoroutine != null)
            StopCoroutine(actionCoroutine);

        canvasGroup.gameObject.SetActive(true);
        SetFishing();
        StateFishing();
    }

    //void StateNone()
    //{
    //    //SetFishing();
    //    canvasGroup.gameObject.SetActive(false);
    //}
    public float lodPower, fishPower;
    void SetFishing()
    {
        fillArea = equipStruct.fishingArea;
        lodPower = (1f + (equipStruct.lodPower / fishStruct.fishPower)) / fishStruct.fishStamina;
        fishPower = (1f + (fishStruct.fishPower / equipStruct.lodPower)) / fishStruct.fishStamina;
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
            fish.localRotation = Quaternion.Slerp(fish.localRotation, Quaternion.Euler(0, 0, targetAngle), fishStruct.fishSpeed * Time.deltaTime);// 물고기 움직임 추적

            currentSpeed = Mathf.Lerp(currentSpeed, _targetSpeed, equipStruct.reelingAcceleration * Time.deltaTime);// 가속도
            fishingRod.transform.localRotation = Quaternion.Euler(0, 0, fishingRod.transform.localRotation.eulerAngles.z + currentSpeed * Time.deltaTime);

            health.material.SetFloat("_FillAmount", complatePoint);

            fillAngle = 180f * fillArea;
            fishingRod.material.SetFloat("_FillAmount", fillArea);
            fishingRod.material.SetFloat("_RotateAngle", fillAngle + 180f);

            var main = targetParticles.main;
            if (VisibleTarget() == true)
            {
                main.startColor = Color.green;
                if (complatePoint < 1f)
                {
                    complatePoint += lodPower * Time.deltaTime;
                }
                else
                {
                    fishingAction = false;
                    EndGame();
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
            float addAngle = Random.Range(-fishStruct.fishROA, fishStruct.fishROA);
            targetAngle += addAngle;
            float random = Random.Range(fishStruct.fishTurnDelay.x, fishStruct.fishTurnDelay.y);
            yield return new WaitForSeconds(random);
        }
    }

    bool VisibleTarget()// 보이는지 확인
    {
        Vector3 offset = (fishObject.transform.position - fishingRod.transform.position);
        if (Vector3.Angle(fishingRod.transform.up, offset.normalized) < fillAngle)// 앵글 안에 포함 되는지
        {
            return true;
        }
        return false;
    }

    //void StateComplate()
    //{
    //    //StateMachine(FishingState.None);
    //    EndGame();
    //    //actionCoroutine = StartCoroutine(FishingComplate());
    //}

    //IEnumerator FishingComplate()
    //{
    //    int index = 0;
    //    while (index < 3)
    //    {
    //        index++;
    //        stateText.text = index.ToString();
    //        yield return new WaitForSeconds(1f);
    //    }
    //    //StateMachine(FishingState.None);
    //    EndGame();
    //}
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
