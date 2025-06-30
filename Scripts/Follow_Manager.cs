using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow_Manager : MonoBehaviour
{
    Camera UICamera;
    public Camera SetCamera { set { UICamera = value; } }
    public RectTransform cameraParent;
    Dictionary<GameObject, GameObject> follow_Camera = new Dictionary<GameObject, GameObject>();
    Dictionary<GameObject, GameObject> follow_Overlay = new Dictionary<GameObject, GameObject>();

    public GameObject ttttttt;
    public GameObject xxxxxxx;

    void Start()
    {

    }

    public void StartTest()
    {
        //AddFollowUI(ttttttt);
        //StartFollowing();
        xxxxxxx.gameObject.SetActive(false);
    }

    public void AddClosestTarget(GameObject _target)
    {
        ttttttt = _target;
        xxxxxxx.gameObject.SetActive(_target != null);
        if (followClosestTarget == null)
            followClosestTarget = StartCoroutine(FollowClosestTarget());
    }
    Coroutine followClosestTarget;

    IEnumerator FollowClosestTarget()
    {
        while (ttttttt != null)
        {
            FollowTarget_Camera(ttttttt.transform, xxxxxxx);
            yield return null;
        }
        followClosestTarget = null;
    }

    public void AddFollowUI(GameObject _target)
    {
        follow_Camera[_target] = xxxxxxx;
    }

    public void RemoveFollowUI(GameObject _target)
    {
        follow_Camera.Remove(_target);
    }

    Coroutine followUI;
    void StartFollowing()
    {
        if (followUI != null)
            StopCoroutine(followUI);
        followUI = StartCoroutine(StartFollowing_Camera());
    }

    IEnumerator StartFollowing_Camera()
    {
        while (follow_Camera.Count > 0)
        {
            foreach (var child in follow_Camera)
            {
                Transform target = child.Key.transform;
                GameObject followUI = child.Value;
                //    followUI.transform.localScale = Vector3.one;

                //    Vector3 screenPosition = Camera.main.WorldToScreenPoint(target.position);
                //    switch (followUI.followType)// 카메라 캔버스인 경우
                //    {
                //        case Follow_Target.FollowType.Overlay:
                //            followUI.transform.SetParent(overlayParent);
                //            followUI.transform.position = screenPosition;
                //            break;

                //        case Follow_Target.FollowType.Camera:
                //            Vector3 followPosition = UICamera.ScreenToWorldPoint(screenPosition);
                //    followUI.transform.SetParent(cameraParent);
                //    followUI.transform.position = followPosition;
                //    break;
                //}
                FollowTarget_Camera(target, followUI);
            }
            yield return null;
        }
    }

    void FollowTarget_Camera(Transform _target, GameObject _followUI)
    {
        _followUI.transform.localScale = Vector3.one;

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(_target.position);
        Vector3 followPosition = UICamera.ScreenToWorldPoint(screenPosition);
        _followUI.transform.position = followPosition;
    }
}
