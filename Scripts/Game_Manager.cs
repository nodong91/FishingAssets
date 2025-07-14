using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    public Unit_Player player;
    public UI_Inventory inventory;
    public Fishing_Manager fishingManager;
    public Camera_Manager cameraManager;
    public FishGuide fishGuide;
    public UI_Equip equip;
    public Follow_Manager followManager;
    public UI_Main uiMain;

    public static Game_Manager current;

    private void Awake()
    {
        current = this;
    }

    void Start()
    {
        followManager.SetCamera = cameraManager.UICamera;
        followManager.StartTest();
        uiMain.SetStart();

        inventory.SetStart();
        fishGuide.SetStart();
    }

    void Update()
    {

    }
}
