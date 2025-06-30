using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UI_Manager : MonoBehaviour
{
    public Camera UICamera;
    public Follow_Manager followManager;

    public static UI_Manager current;

    private void Awake()
    {
        current = this;
    }

    void Start()
    {
        SetUICamera();
        followManager.SetCamera = UICamera;
        followManager.StartTest();
    }

    void SetUICamera()
    {
        Camera mainCamera = Camera.main;
        var cameraData = mainCamera.GetUniversalAdditionalCameraData();
        if (cameraData.cameraStack.Contains(UICamera) == false)
        {
            UICamera.fieldOfView = mainCamera.fieldOfView;
            cameraData.cameraStack.Add(UICamera);
            Debug.LogWarning(UICamera.fieldOfView +"       "+ mainCamera.fieldOfView);
        }
    }
}
