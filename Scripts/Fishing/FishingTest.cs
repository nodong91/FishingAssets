using System.Collections;
using Unity.Cinemachine;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;

public class FishingTest : MonoBehaviour
{
    public CinemachineCamera[] cameraPoint;
    public GameObject setObject;

    [Header(" [ Catch ]")]
    // 원형 바운더리 안에서 랜덤 이동
    public GameObject catchPrefab;
    private Vector3 catchPoint = Vector3.zero;

    public SetStatus catchStatus;
    public float catchHealth;// 낚시대의 체력
    public Image catchImage;

    [Header(" [ Fish ]")]
    public GameObject fishPrefab;
    private float randomTime = 0f;
    private Vector3 fishPoint = Vector3.zero;

    public FishStruct fishStatus;
    public float fishHealth;
    public Image fishImage;

    bool isFishing = false;
    bool isCatching = false;
    public Vector3 offset = new Vector3(0f, 0f, 0f);

    public delegate void DeleEndFishing(bool _comp);
    public DeleEndFishing deleEndFishing;

    private void Start()
    {
        catchImage.material = Instantiate(catchImage.material);
        fishImage.material = Instantiate(fishImage.material);
    }

    public void SetStart(FishStruct _fishStruct)
    {
        SetFishStruct(_fishStruct);// 물고기 정보 설정
        SetFishingLod(Game_Manager.current.currentStatus);

        ResetFishing();
        SetCamera();
        StartCoroutine(Fishing());
        StartCoroutine(FishingDamage());
    }

    void SetFishStruct(FishStruct _fishStruct)
    {
        fishStatus = _fishStruct;
    }

    void SetFishingLod(SetStatus _catchStatus)
    {
        catchStatus = _catchStatus;
        catchPrefab.transform.localScale = Vector3.one * 0.2f * catchStatus.catchRadius;
    }

    void SetCamera()
    {
        float distance = 10000f;
        Transform cameraPosition = Game_Manager.current.cameraManager.cinemachineCamera.transform;
        CinemachineCamera closestCamera = null;
        for (int i = 0; i < cameraPoint.Length; i++)
        {
            float dist = (cameraPosition.position - cameraPoint[i].transform.position).sqrMagnitude;
            if (distance > dist)
            {
                closestCamera = cameraPoint[i];
                distance = dist;
            }
        }
        closestCamera.gameObject.SetActive(true);
        closestCamera.Follow = catchPrefab.transform;
    }

    public void OffCamera()
    {
        for (int i = 0; i < cameraPoint.Length; i++)
        {
            cameraPoint[i].gameObject.SetActive(false);
        }
    }

    void ResetFishing()
    {
        setObject.SetActive(true);

        Transform player = Game_Manager.current.GetPlayer.transform;
        transform.SetPositionAndRotation(player.position, player.rotation);

        fishPrefab.transform.position = transform.position;
        catchPrefab.transform.position = transform.position;

        catchHealth = catchStatus.catchMaxHealth;
        fishHealth = fishStatus.fishHealth;
        fishImage.material.SetFloat("_FillAmount", fishHealth);
        catchImage.material.SetFloat("_FillAmount", catchHealth);
    }

    IEnumerator Fishing()
    {
        isFishing = true;
        while (isFishing)
        {
            OnRayCast();
            FishMovement();
            yield return null;
        }
    }

    IEnumerator FishingDamage()
    {
        while (isFishing)
        {
            if (isCatching == true)
            {
                // 물고기가 잡히는 상태일 때 공격 빈도에 따라 대기
                fishHealth -= catchStatus.catchPower;
                fishImage.material.SetFloat("_FillAmount", fishHealth / fishStatus.fishHealth);
                if (fishHealth <= 0f)
                {
                    FishingComplate(true);// 성공
                }
                yield return new WaitForSeconds(catchStatus.catchAttakSpeed);
            }
            else
            {
                // 물고기가 공격하는 빈도에 따라 대기
                catchHealth -= fishStatus.fishPower;
                catchImage.material.SetFloat("_FillAmount", catchHealth / catchStatus.catchMaxHealth);
                if (catchHealth <= 0f)
                {
                    FishingComplate(false);// 실패
                }
                yield return new WaitForSeconds(fishStatus.fishAttackSpeed);
            }
        }
    }

    void FishingComplate(bool _comp)
    {
        isFishing = false;
        deleEndFishing?.Invoke(_comp);
        Debug.Log("낚시 완료");

        setObject.SetActive(false);
    }

    void OnRayCast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int downhillLayer = 1 << LayerMask.NameToLayer("Water");
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, downhillLayer))
        {
            Debug.LogWarning(hit.transform.name);
            Vector3 offset = (hit.point - transform.position);
            Vector3 direction = offset.normalized;
            catchPoint = transform.position + direction * Mathf.Clamp(offset.magnitude, 0f, fishStatus.fieldRadius);
        }
        catchPrefab.transform.position = Vector3.Lerp(catchPrefab.transform.position, catchPoint, Time.deltaTime * catchStatus.catchSpeed);
    }

    void FishMovement()
    {
        FollowHPUI();
        if (randomTime < Time.time)
        {
            randomTime = Time.time + Random.Range(fishStatus.fishRange.x, fishStatus.fishRange.y);
            Vector3 tempPoint = Random.insideUnitSphere * fishStatus.fieldRadius;
            fishPoint = new Vector3(tempPoint.x, 0f, tempPoint.z) + transform.position;
        }
        // fishPrefab의 위치를 기준으로 반지름 5의 원형 바운더리 안에서 랜덤하게 이동
        fishPrefab.transform.position = Vector3.Lerp(fishPrefab.transform.position, fishPoint, Time.deltaTime * fishStatus.fishSpeed);

        Vector3 offset = fishPrefab.transform.position - catchPrefab.transform.position;
        isCatching = (offset.magnitude < catchStatus.catchRadius);
    }

    void FollowHPUI()
    {
        fishImage.rectTransform.position = Camera.main.WorldToScreenPoint(fishPrefab.transform.position + offset);
    }
    //==================================================================================================================================
    // 액션
    //==================================================================================================================================

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // Gizmos를 사용하여 물고기의 이동 범위를 시각화
        UnityEditor.Handles.color = Gizmos.color = isCatching == true ? Color.green : Color.red;
        Gizmos.DrawSphere(fishPoint, 1f);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, fishStatus.fieldRadius);
        UnityEditor.Handles.DrawWireDisc(catchPrefab.transform.position, Vector3.up, catchStatus.catchRadius);

        UnityEditor.Handles.color = Gizmos.color = Color.blue;
        Gizmos.DrawSphere(catchPoint, 1f);
    }
#endif
}
