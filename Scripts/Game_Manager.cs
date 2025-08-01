using System.Collections;
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
    public Dialog_Manager dialogManager;
    public UI_Quest questUI;

    public static Game_Manager current;

    private void Awake()
    {
        current = this;
    }

    void Start()
    {
        SceneLoader.OnSceneLoaded("Fishing", UnityEngine.SceneManagement.LoadSceneMode.Additive);
        SaveData_Continue.current.GetContinue();

        followManager.SetStart();
        mainUI.SetStart();

        inventory.SetStart();
        questUI.SetStart();

        PlayerMove();
    }

    public void InputLeftMouse(bool _input)
    {
        player.State_Action(_input);
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


    //====================================================================================================================
    // 매니저 가져오기
    //====================================================================================================================

    private FishGuide instFishGuide;
    public FishGuide GetFishGuide
    {
        get
        {
            if (instFishGuide == null)
            {
                instFishGuide = Instantiate(fishGuide);
                instFishGuide.SetStart();
            }
            return instFishGuide;
        }
    }

    private UI_Landing instLanding;
    public UI_Landing GetLanding
    {
        get
        {
            if (instLanding == null)
            {
                instLanding = Instantiate(landingUI);
                instLanding.SetStart();
            }
            return instLanding;
        }
    }
}
