using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    public Unit_Player player;
    public Fishing_Manager fishingManager;
    public Camera_Manager cameraManager;
    public FishGuide fishGuide;
    public Follow_Manager followManager;
    public Controll_Manager controllManager;
    [Header("[ UI ]")]
    public UI_Main mainUI;
    public UI_Inventory inventory;
    public UI_Landing landingUI;
    public UI_Status statusUI;
    public UI_Time timeUI;

    public static Game_Manager current;

    private void Awake()
    {
        current = this;
    }

    void Start()
    {
        followManager.SetStart();
        mainUI.SetStart();

        inventory.SetStart();
        fishGuide.SetStart();
        landingUI.SetStart();

        SaveData_Continue.current.GetContinue();
        PlayerMove();
        //player.deleControll = controllManager.controllDirection;
    }

    public void InputLeftMouse(bool _input)
    {
        player.State_Action(_input);
        //player.InputMousetLeft(_input);
    }

    public void InputRightMouse(bool _input)
    {
        cameraManager?.InputRotate(_input);
    }

    public void PlayerMove()
    {
        controllManager.SetDirection();
        player.StateMove(controllManager.dirction);
    }

    public void PlayerEscape()
    {
        player.StateEscape();
    }

    public void OutOfControll(bool _isOn)
    {
        Singleton_Controller.INSTANCE.outOfControll = _isOn;
        controllManager.ResetControll();
    }
}
