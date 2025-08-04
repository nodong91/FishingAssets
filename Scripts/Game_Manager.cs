using System.Collections;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    public int frameRate = 60;

    public Unit_Player player;
    public Camera_Manager cameraManager;
    public Controll_Manager controllManager;
    [Header("[ UI ]")]
    public UI_Main mainUI;
    public Follow_Manager followManager;
    public UI_Inventory inventory;
    public UI_Landing landingUI;
    public Dialog_Manager dialogManager;
    public UI_QuestManager questUI;
    public Fishing_Manager fishingManager;
    public FishGuide fishGuide;

    public static Game_Manager current;

    private void Awake()
    {
        current = this;
    }

    void Start()
    {
        Application.targetFrameRate = frameRate;

        SceneLoader.OnSceneLoaded("Fishing", UnityEngine.SceneManagement.LoadSceneMode.Additive);
        SaveData_Continue.current.GetContinue();

        //followManager.SetStart();

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
    private UI_Main instMain;
    public UI_Main GetMainUI
    {
        get
        {
            if (instMain == null)
            {
                instMain = Instantiate(mainUI);
                instMain.SetStart();
            }
            return instMain;
        }
    }

    public UI_Time GetTimeUI
    {
        get
        {
            return GetMainUI.timeUI;
        }
    }

    private Follow_Manager instFollow;
    public Follow_Manager GetFollow
    {
        get
        {
            if (instFollow == null)
            {
                instFollow = Instantiate(followManager);
                instFollow.SetStart();
            }
            return instFollow;
        }
    }

    private UI_Inventory instInventory;
    public UI_Inventory GetInventory
    {
        get
        {
            if (instInventory == null)
            {
                instInventory = Instantiate(inventory);
                instInventory.SetStart();
            }
            return instInventory;
        }
    }

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

    private UI_QuestManager instQuest;
    public UI_QuestManager GetQuest
    {
        get
        {
            if (instQuest == null)
            {
                instQuest = Instantiate(questUI);
                instQuest.SetStart();
            }
            return instQuest;
        }
    }

    private Fishing_Manager instFishing;
    public Fishing_Manager GetFishing
    {
        get
        {
            if (instFishing == null)
            {
                instFishing = Instantiate(fishingManager);
                instFishing.SetStart();
            }
            return instFishing;
        }
    }

    private Dialog_Manager instDialog;
    public Dialog_Manager GetDialog
    {
        get
        {
            if (instDialog == null)
            {
                instDialog = Instantiate(dialogManager);
                instDialog.SetStart();
            }
            return instDialog;
        }
    }
}
