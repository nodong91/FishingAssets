using Unity.Cinemachine;
using UnityEngine;

public class Fishing_Camera : MonoBehaviour
{
    public CinemachineCamera[] cameraPoint;

    public void SetCamera()
    {
        float distance = 10000f;
        Transform cameraPosition = Game_Manager.current.cameraManager.cinemachineCamera.transform;
        GameObject closest = null;
        for (int i = 0; i < cameraPoint.Length; i++)
        {
            float dist = (cameraPosition.position - cameraPoint[i].transform.position).sqrMagnitude;
            if (distance > dist)
            {
                closest = cameraPoint[i].gameObject;
                distance = dist;
            }
        }
        closest.gameObject.SetActive(true);
    }

    public void OffCamera()
    {
        for (int i = 0; i < cameraPoint.Length; i++)
        {
            cameraPoint[i].gameObject.SetActive(false);
        }
    }
}
