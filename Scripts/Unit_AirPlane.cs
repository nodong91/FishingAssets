using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class Unit_AirPlane : MonoBehaviour
{
    public Camera_Manager manager;
    public float normalSpeed;
    public float busterSpeed;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            manager.InputRotate(true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            manager.InputRotate(false);
        }
        manager.GetFocusTarget.transform.position = transform.position;
        transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward, normalSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, manager.GetFocusTarget.transform.rotation, 2f * Time.deltaTime);

        closestTarget = FindClosestTarget();

        if (closestTarget != null && delayTime < Time.time)
        {
            delayTime = Time.time + 1f;
            StartCoroutine(TargetDamage(closestTarget));
        }
    }

    IEnumerator TargetDamage(GameObject _target)
    {
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 5f;
            _target.transform.position = _target.transform.position + Random.insideUnitSphere * (1f - normalize);
            yield return null;
        }
    }

    float delayTime = 0f;
    GameObject FindClosestTarget()
    {
        // 각도 체크
        List<GameObject> tempTargets = new List<GameObject>();
        for (int i = 0; i < targets.Length; i++)
        {
            Vector3 offset = (targets[i].transform.position - transform.position);
            float angle = Vector3.Angle(offset, transform.forward);
            if (angle < 20f)
            {
                tempTargets.Add(targets[i]);
            }
        }

        GameObject closest = null;
        if (tempTargets.Count > 0)
        {
            float dist = Mathf.Infinity;
            for (int i = 0; i < targets.Length; i++)
            {
                float temp = (transform.position - targets[i].transform.position).sqrMagnitude;
                if (dist > temp)
                {
                    dist = temp;
                    closest = targets[i];
                }
            }
        }
        return closest;
    }

    public GameObject closestTarget;
    public GameObject[] targets;

    private void OnDrawGizmos()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            GameObject target = targets[i];
            if (!target) return;

            Vector3 offset = (target.transform.position - transform.position);
            float angleA = Vector3.Angle(transform.position, target.transform.position);
            float angleB = Vector3.Angle(offset, transform.forward);

            Gizmos.color = angleB > 20f ? Color.white : Color.red;
            Gizmos.DrawLine(transform.position, target.transform.position);
            Handles.Label(target.transform.position + Vector3.up, $"{angleA} : {angleB}");

            Handles.DrawLines(new Vector3[] { transform.position, transform.position + transform.forward });
            Handles.Label(transform.position + transform.forward + Vector3.up * 0.2f, $"Forward");
        }
    }
}
