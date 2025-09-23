using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static Data_Manager;

public class Game_Manager : MonoBehaviour
{
    public Unit_Player player;
    public Controll_Manager controllManager;

    [Header("[ UI ]")]
    public UI_Main mainUI;
    public Follow_Manager followManager;
    public UI_Inventory inventory;
    public UI_Landing landingUI;
    public Dialog_Manager dialogManager;
    public UI_NewsManager newsUI;
    public FishGuide fishGuide;
    public Skill_Manager skillManager;
    public string themeMusic;
    public string oceanSound = "Ocean 02";
    public Fishing_Manager fishingAction;

    public UI_QuestManager questManager;
    public Energy_Manager energyManager;
    public Gamble_Lottery lottery;
    public Tutorial_Manager tutorial;

    public Data_Status_Default defaultStatusData;
    public SetStatus currentStatus;
    public SetStatus GetAddStatus => GetSkill.addStatus;

    public Light dayLight;
    public Color dayColor, nightColor;

    public static Game_Manager current;

    private void Awake()
    {
        current = this;
    }

    void Start()
    {
        StartCoroutine(SetStart());
    }

    IEnumerator SetStart()
    {
        SaveData_Continue.current.GetContinue();
        Camera_Manager.current.SetCameraManager();
        SetRendererFeature();

        SetThemeMusic();
        SetPlayer();
        yield return null;

        GetMainUI.SetStart();
        GetQuestUI.SetStart();
        GetSkill.SetStart();
        GetDialog.SetStart();
        AddStatus();// 추가 스테이트 세팅
    }

    void SetPlayer()
    {
        if (instPlayer == null)
        {
            instPlayer = Instantiate(player, transform);
            GetPlayer.SetStart();
        }
    }

    public void AddStatus()
    {
        bool fullHealth = GetPlayer.FullHealth;// 스탯 적용 하기 전 풀피 체크
        Debug.LogWarning($"AddStatus 시 풀피 체크 : {fullHealth}");
        currentStatus.SettingStatus(defaultStatusData.defaultStatus);// 디폴트 스탯 적용
        currentStatus.AddStatus(GetAddStatus);// 추가 스탯 적용

        GetInventory.myBox.AddInventory(currentStatus.maxBoxSize);// 인벤토리 사이즈 적용
        GetPlayer.SetStatus(fullHealth);// 플레이어에 스탯 적용
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
        Camera_Manager.current?.InputRotate(_input);
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

    public bool CheckMoney(float _price)
    {
        if (GetMainUI.TryMoney < _price)
        {
            GetMainUI.NoMoney();// 구매할 돈없음
            return false;
        }
        return true;
    }

    public void StartFishing(AreaType _areaType)
    {
        // 낚시가 처음인지 확인
        // 공격하는 물고기가 처음인지 확인
        GetTutorial.TutorialPause();
        GetFishing.SetFishing(_areaType);
    }
    //====================================================================================================================
    // 매니저 가져오기
    //====================================================================================================================

    private Unit_Player instPlayer;
    public Unit_Player GetPlayer
    {
        get
        {
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
                instFishing = Instantiate(fishingAction, transform);
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

    private Energy_Manager instEnergy;
    public Energy_Manager GetEnergyUI
    {
        get
        {
            if (instEnergy == null)
            {
                instEnergy = Instantiate(energyManager, transform);
                instEnergy.SetStart();
            }
            return instEnergy;
        }
    }

    private Gamble_Lottery instLottery;
    public Gamble_Lottery GetLottery
    {
        get
        {
            if (instLottery == null)
            {
                instLottery = Instantiate(lottery, transform);
            }
            return instLottery;
        }
    }

    private Tutorial_Manager instTutorial;
    public Tutorial_Manager GetTutorial
    {
        get
        {
            if (instTutorial == null)
            {
                instTutorial = Instantiate(tutorial, transform);
            }
            return instTutorial;
        }
    }



    public int featureIndex = 2;
    public Material fullscreenMaterial;
    public FullScreenPassRendererFeature fullScreenRendererFeature;
    ScriptableRendererData scriptableRendererData;
    FullScreenPassRendererFeature.InjectionPoint injectionPoint = FullScreenPassRendererFeature.InjectionPoint.AfterRenderingPostProcessing;

    void SetRendererFeature()
    {
        var pipeline = ((UniversalRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline);
        if (pipeline == null)
            return;
        var propertyInfo = pipeline.GetType().GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);
        scriptableRendererData = ((ScriptableRendererData[])propertyInfo.GetValue(pipeline))[0];
        Debug.LogWarning($"scriptableRendererData : {scriptableRendererData.rendererFeatures.Count}");

        fullScreenRendererFeature = (FullScreenPassRendererFeature)scriptableRendererData.rendererFeatures[featureIndex];
        //renderObject = (RenderObjects)scriptableRendererData.rendererFeatures[12];

        if (fullScreenRendererFeature != null)
        {
            fullScreenRendererFeature.SetActive(fullscreenMaterial != null);
            fullScreenRendererFeature.passMaterial = fullscreenMaterial;
            fullScreenRendererFeature.injectionPoint = injectionPoint;
        }
    }
}
