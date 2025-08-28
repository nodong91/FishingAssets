using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    public Unit_Player player;
    public Camera_Manager cameraManager;
    public Controll_Manager controllManager;

    [Header("[ UI ]")]
    public UI_Main mainUI;
    public Follow_Manager followManager;
    public UI_Inventory inventory;
    public UI_Landing landingUI;
    public Dialog_Manager dialogManager;
    public UI_NewsManager newsUI;
    public Fishing_Manager fishingManager;
    public FishGuide fishGuide;
    public Skill_Manager skillManager;
    public string themeMusic;
    public string oceanSound = "Ocean 02";

    public UI_QuestManager questManager;
    public Data_Manager.SetStatus defaultStatus;
    public Data_Manager.SetStatus currentStatus;
    public Data_Manager.SetStatus GetStatus => GetSkill.addStatus;

    public Light dayLight;
    public Color dayColor, nightColor;

    public static Game_Manager current;

    private void Awake()
    {
        current = this;
    }

    void Start()
    {
        SaveData_Continue.current.GetContinue();

        GetMainUI.SetStart();
        GetQuestUI.SetStart();
        GetSkill.SetStart();
        GetDialog.SetStart();

        SetThemeMusic();
        SetDefaultStatus();// 기본 스테이트 세팅
        AddStatus();// 추가 스테이트 세팅
        PlayerMove();
    }

    void SetDefaultStatus()
    {
        defaultStatus.catchRadius = 1f;// 물고기를 잡는 범위
        defaultStatus.catchSpeed = 0.1f;// 낚시대가 물고기를 향해 이동하는 속도
        defaultStatus.catchPower = 1f;// 낚시대의 힘
        defaultStatus.catchMaxHealth = 10f;// 낚시대의 최대 체력
        defaultStatus.catchAttakSpeed = 1.5f;// 물고기를 공격하는 빈도

        defaultStatus.shipSpeed = 2f;// 배의 이동 속도
        defaultStatus.maxWeight = 1f;// 인벤토리 중량
        defaultStatus.maxEnergy = 10f;// 연료통 크기
        defaultStatus.efficient = 0.1f;// 에너지 효율
        defaultStatus.maxBoxSize = new Vector2Int(0, 0);// 인벤토리 크기
        defaultStatus.shipHealth = 3;// 배 체력
        defaultStatus.freshness = 1f;// 신선도 유지 - 꼭 필요한가??????  

        defaultStatus.LuckFish = 1f;// 희귀 물고기 확률
        defaultStatus.FishAmount = 1f;// 낚시 횟수 증가
        defaultStatus.FishPrice = 1f;// 판매 물고기 가격 증가
    }

    public void AddStatus()
    {
        currentStatus.SettingStatus(defaultStatus);// 디폴트 스탯 적용
        currentStatus.AddStatus(GetStatus);// 추가 스탯 적용

        GetPlayer.SetStatus();
    }

    public void SetThemeMusic()
    {
        Option_Manager.current.SetThemeMusic(themeMusic);
        Singleton_Audio.INSTANCE.Audio_Environment(oceanSound);
    }

    public void InputLeftMouse(bool _input)
    {
        GetPlayer.State_Action(_input);
    }

    public void InputRightMouse(bool _input)
    {
        cameraManager?.InputRotate(_input);
    }

    public void PlayerMove()
    {
        controllManager.SetDirection();
        GetPlayer.StateMove(controllManager.dirction);
    }

    public void PlayerEscape()
    {
        GetPlayer.StateEscape();
    }

    public void OutOfControll(bool _isOn)
    {
        Singleton_Controller.INSTANCE.outOfControll = _isOn;
        controllManager.ResetControll();
    }

    //====================================================================================================================
    // 매니저 가져오기
    //====================================================================================================================

    private Unit_Player instPlayer;
    public Unit_Player GetPlayer
    {
        get
        {
            if (instPlayer == null)
            {
                instPlayer = Instantiate(player, transform);
            }
            return instPlayer;
        }
    }

    private UI_Main instMain;
    public UI_Main GetMainUI
    {
        get
        {
            if (instMain == null)
            {
                instMain = Instantiate(mainUI, transform);
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
                instFollow = Instantiate(followManager, transform);
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
                instInventory = Instantiate(inventory, transform);
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
                instFishGuide = Instantiate(fishGuide, transform);
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
                instLanding = Instantiate(landingUI, transform);
                instLanding.SetStart();
            }
            return instLanding;
        }
    }

    private UI_NewsManager instNews;
    public UI_NewsManager GetNews
    {
        get
        {
            if (instNews == null)
            {
                instNews = Instantiate(newsUI, transform);
                instNews.SetStart();
            }
            return instNews;
        }
    }

    private Fishing_Manager instFishing;
    public Fishing_Manager GetFishing
    {
        get
        {
            if (instFishing == null)
            {
                instFishing = Instantiate(fishingManager, transform);
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
                instDialog = Instantiate(dialogManager, transform);
            }
            return instDialog;
        }
    }

    private Skill_Manager instSkill;
    public Skill_Manager GetSkill
    {
        get
        {
            if (instSkill == null)
            {
                instSkill = Instantiate(skillManager, transform);
            }
            return instSkill;
        }
    }

    private UI_QuestManager instQuest;
    public UI_QuestManager GetQuestUI
    {
        get
        {
            if (instQuest == null)
            {
                instQuest = Instantiate(questManager, transform);
            }
            return instQuest;
        }
    }
}
