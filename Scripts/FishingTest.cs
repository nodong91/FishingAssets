using UnityEngine;

public class FishingTest : MonoBehaviour
{
    public GameObject fishPrefab;
    // 원형 바운더리 안에서 랜덤 이동
    // fishPrefab의 위치를 기준으로 반지름 5의 원형 바운더리 안에서 랜덤하게 이동하는 스크립트
    public float radius = 5f;
    public float speed = 2f;
    private Vector3 centerPosition;
    private Vector3 fishPosition;
    private Vector3 fishDirection;
    private float fishSpeed;
   

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
