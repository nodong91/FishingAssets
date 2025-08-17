using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;
using static Data_Quest;

public class FishingTest : MonoBehaviour
{
    public CinemachineCamera[] cameraPoint;
    public GameObject setObject;

    public void SetCamera()
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

        transform.position = transform.position;
        SetFishing();
    }

    public void OffCamera()
    {
        for (int i = 0; i < cameraPoint.Length; i++)
        {
            cameraPoint[i].gameObject.SetActive(false);
        }
    }

    public GameObject fishPrefab;
    // 원형 바운더리 안에서 랜덤 이동
    // fishPrefab의 위치를 기준으로 반지름 5의 원형 바운더리 안에서 랜덤하게 이동하는 스크립트
    public GameObject catchPrefab;
    public float catchRadius = 5f;
    public float moveSpeed = 2f;
    private float catchAttakSpeed = 1.5f;// 물고기를 공격하는 빈도
    private Vector3 catchPoint;

    public float fieldRadius = 10f;
    private float fishAttackSpeed = 0.5f; // 물고기가 공격하는 빈도

    private float randomTime = 0f;
    private Vector3 fishPoint = Vector3.zero;
    private Vector2 fishRange = new Vector2(0.5f, 1.5f);
    public float fishSpeed = 10f;

    public float catchAmount;
    public float fishAmount;
    public Image catchImage, fishImage;

    bool isFishing = false;
    bool isCatching = false;
    public RectTransform followUI;
    public Vector3 offset = new Vector3(0f, 0f, 0f);

    public delegate void DeleEndFishing(bool _comp);
    public DeleEndFishing deleEndFishing;

    private void Start()
    {
        catchImage.material = Instantiate(catchImage.material);
        fishImage.material = Instantiate(fishImage.material);
    }

    public void SetFishing()
    {
        ResetFishing();
        StartCoroutine(Fishing());
        StartCoroutine(FishingDamage());
    }

    void ResetFishing()
    {
        setObject.SetActive(true);

        Vector3 tempPoint = Random.insideUnitSphere * fieldRadius;
        fishPoint = new Vector3(tempPoint.x, 0f, tempPoint.z) + transform.position;
        fishPrefab.transform.position = fishPoint;
        catchPrefab.transform.position = transform.position;

        catchAmount = 1f;
        fishAmount = 1f;
        fishImage.material.SetFloat("_FillAmount", fishAmount);
        catchImage.material.SetFloat("_FillAmount", catchAmount);
        isFishing = true;
    }

    IEnumerator Fishing()
    {
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
                fishAmount -= 0.1f;
                fishImage.material.SetFloat("_FillAmount", fishAmount);
                if (fishAmount <= 0f)
                {
                    FishingComplate(true);
                }
                yield return new WaitForSeconds(catchAttakSpeed);
            }
            else
            {
                // 물고기가 공격하는 빈도에 따라 대기
                catchAmount -= 0.1f;
                catchImage.material.SetFloat("_FillAmount", catchAmount);
                if (catchAmount <= 0f)
                {
                    FishingComplate(false);
                }
                yield return new WaitForSeconds(fishAttackSpeed);
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
            catchPoint = transform.position + direction * Mathf.Clamp(offset.magnitude, 0f, fieldRadius);
        }
        catchPrefab.transform.position = Vector3.Lerp(catchPrefab.transform.position, catchPoint, Time.deltaTime * moveSpeed);
    }

    void FishMovement()
    {
        FollowHPUI();
        if (randomTime < Time.time)
        {
            randomTime = Time.time + Random.Range(fishRange.x, fishRange.y);
            Vector3 tempPoint = Random.insideUnitSphere * fieldRadius;
            fishPoint = new Vector3(tempPoint.x, 0f, tempPoint.z) + transform.position;
        }
        // fishPrefab의 위치를 기준으로 반지름 5의 원형 바운더리 안에서 랜덤하게 이동
        fishPrefab.transform.position = Vector3.Lerp(fishPrefab.transform.position, fishPoint, Time.deltaTime * fishSpeed);

        Vector3 offset = fishPrefab.transform.position - catchPrefab.transform.position;
        isCatching = (offset.magnitude < catchRadius);
    }

    void FollowHPUI()
    {
        followUI.position = Camera.main.WorldToScreenPoint(fishPrefab.transform.position + offset);
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
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, fieldRadius);
        UnityEditor.Handles.DrawWireDisc(catchPrefab.transform.position, Vector3.up, catchRadius);

        UnityEditor.Handles.color = Gizmos.color = Color.blue;
        Gizmos.DrawSphere(catchPoint, 1f);
    }
#endif
}
