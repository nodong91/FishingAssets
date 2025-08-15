using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FishingTest : MonoBehaviour
{
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

    private void Start()
    {
        SetFishing();
    }

    public void SetFishing()
    {
        catchImage.material = Instantiate(catchImage.material);
        fishImage.material = Instantiate(fishImage.material);

        StartCoroutine(Fishing());
        StartCoroutine(FishingDamage());
    }

    bool isFishing = true;
    bool isCatching = false;
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
                yield return new WaitForSeconds(catchAttakSpeed);
            }
            else
            {
                // 물고기가 공격하는 빈도에 따라 대기
                catchAmount -= 0.1f;
                catchImage.material.SetFloat("_FillAmount", catchAmount);
                yield return new WaitForSeconds(fishAttackSpeed);
            }
        }
    }

    void OnRayCast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int downhillLayer = 1 << LayerMask.NameToLayer("Water");
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, downhillLayer))
        {
            catchPoint = hit.point;
            Debug.LogWarning(hit.transform.name);
        }
        catchPrefab.transform.position = Vector3.Lerp(catchPrefab.transform.position, catchPoint, Time.deltaTime * moveSpeed);
    }

    void FishMovement()
    {
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

    void OnDrawGizmos()
    {
        // Gizmos를 사용하여 물고기의 이동 범위를 시각화
        Handles.color = Gizmos.color = isCatching == true ? Color.green : Color.red;
        Gizmos.DrawSphere(fishPoint, 1f);
        Handles.DrawWireDisc(transform.position, Vector3.up, fieldRadius);
        Handles.DrawWireDisc(catchPrefab.transform.position, Vector3.up, catchRadius);
    }
}
