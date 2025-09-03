using System.Collections;
using UnityEngine;
using static Data_Manager;
using static Fishing_Action;

public class PositionTeset : MonoBehaviour
{
    public GameObject fish, target;
    float dist;

    void Update()
    {
        fish.transform.position = CatchRayCast();
        Vector3 pos = Vector3.Lerp(fish.transform.position, target.transform.position, 0.5f);
        dist = (pos - transform.position).magnitude;
        angle = Vector3.Angle(transform.position, fish.transform.position);
    }

    Vector3 CatchRayCast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int downhillLayer = 1 << LayerMask.NameToLayer("Water");
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, downhillLayer))
        {
            Debug.LogWarning(hit.transform.name);
            Vector3 hitOffset = (hit.point - transform.position);
            Vector3 direction = hitOffset.normalized;
            return transform.position + direction * Mathf.Clamp(hitOffset.magnitude, 0f, 10f);
        }
        return default;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, fieldSize);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, shipSize);

        Gizmos.color = dist > 2f ? Color.blue : Color.red;
        Gizmos.DrawSphere(fish.transform.position, 0.3f);
        Gizmos.DrawLine(fish.transform.position, target.transform.position);
        Gizmos.DrawLine(fish.transform.position, transform.position);

        float currentAngle = Vector3.Angle(transform.forward, fish.transform.position);

        Vector3 direction = (fish.transform.position - target.transform.position);
        Vector3 lerfVector = Vector3.Lerp(fish.transform.position, target.transform.position, 0.5f);
        UnityEditor.Handles.Label(lerfVector, $"{currentAngle} : {direction.magnitude}");



        Gizmos.color = Color.green;
        Vector3 tempAngle = DirFromAngle(angle, false);
        Vector3 pos = transform.position + tempAngle * range;
        Gizmos.DrawSphere(pos, 0.3f);
    }
#endif

    private void Start()
    {
        StartCoroutine(RandomTest());
    }

    IEnumerator RandomTest()
    {

        while (true)
        {
            Vector3 test = SetRandomPosition();
            target.transform.position = test;
            yield return null;
        }
    }

    Vector3 SetRandomPosition()
    {
        float currentAngle = Vector3.Angle(transform.forward, fish.transform.position);
        float randomAngle = Random.Range(-45f, 45f) + currentAngle;
        float randomRange = Random.Range(shipSize, fieldSize);
        Vector3 tempAngle = DirFromAngle(randomAngle, false);
        Vector3 position = transform.position + tempAngle * range;
        return position;
    }

    public float angle;
    public float range;
    public float shipSize = 2f;
    public float fieldSize = 10f;

    Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
            angleInDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
