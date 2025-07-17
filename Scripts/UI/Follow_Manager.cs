using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Follow_Manager : MonoBehaviour
{
    public Canvas canvas;
    Camera UICamera;
    Dictionary<GameObject, GameObject> follow_Camera = new Dictionary<GameObject, GameObject>();
    Dictionary<GameObject, GameObject> follow_Overlay = new Dictionary<GameObject, GameObject>();

    private GameObject target;
    public Image followUI;
    Coroutine following;

    public void SetStart()
    {
        followUI.gameObject.SetActive(false);
        UICamera = Game_Manager.current.cameraManager.UICamera;
        canvas.worldCamera = UICamera;
    }

    public void AddClosestTarget(Trigger_Setting _target)// 가까운 닿은 오브젝트
    {
        followUI.gameObject.SetActive(_target != null);
        if (_target == null)
            return;

        target = _target.gameObject;
        followUI.sprite = _target.GetIconSprite;

        if (followClosestTarget == null)
            followClosestTarget = StartCoroutine(FollowClosestTarget());
    }
    Coroutine followClosestTarget;

    IEnumerator FollowClosestTarget()
    {
        while (target != null)
        {
            FollowTarget_Camera(target.transform, followUI.transform);
            yield return null;
        }
        followClosestTarget = null;
    }

    public void AddFollowUI(GameObject _target, GameObject _followUI)
    {
        follow_Camera[_target] = _followUI;
        StartFollowing();
    }

    public void RemoveFollowUI(GameObject _target)
    {
        follow_Camera.Remove(_target);
        StartFollowing();
    }

    void StartFollowing()
    {
        if (following != null)
            StopCoroutine(following);
        following = StartCoroutine(StartFollowing_Camera());
    }

    IEnumerator StartFollowing_Camera()
    {
        while (follow_Camera.Count > 0)
        {
            foreach (var child in follow_Camera)
            {
                Transform target = child.Key.transform;
                Transform followUI = child.Value.transform;
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

    void FollowTarget_Camera(Transform _target, Transform _followUI)
    {
        _followUI.transform.localScale = Vector3.one;

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(_target.position);
        Vector3 followPosition = UICamera.ScreenToWorldPoint(screenPosition);
        _followUI.transform.position = followPosition;
    }
}
