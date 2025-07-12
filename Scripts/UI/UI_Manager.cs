using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UI_Manager : MonoBehaviour
{
    public UI_Main uiMain;
    public Follow_Manager followManager;

    void Start()
    {
        followManager.SetCamera = Game_Manager.current.cameraManager.UICamera;
        followManager.StartTest();

        uiMain.SetStart();
    }


}
