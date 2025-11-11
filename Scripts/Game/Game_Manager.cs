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
    public Fishing_Manager fishingAction;
    public UI_QuestManager questManager;

    public Energy_Manager energyManager;
    public Gamble_Lottery lottery;
    public Rest_Manager rest_Manager;
    public Tutorial_Manager tutorial;
    public Tutorial_Manager1 tutorial1;
    public UI_ChangeShip changeShip;
    public Event_Manager event_Manager;

    public Data_Ship shipData;
    public SetStatus currentStatus;
    public SetStatus GetAddStatus => GetSkill.addStatus;

    public Light dayLight;
    public Color dayColor, nightColor;
    [ColorUsage(true, true)]
    public Color emissionColor;
    public Material skyboxMatial;
    public Trigger_Landing CurrentLand { get; set; }
    public string addItemTest;

    public static Game_Manager current;

    private void Awake()
    {
        current = this;
    }

    void Start()
    {
        if (LoadingManager.current != null)
            LoadingManager.current.deleComplate = LoadingComplate;// 로딩 완료

        StartCoroutine(SetStart());
    }

    void Update()// 아이템 추가 테스트
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GetMainUI.MoveMoney(1000f);// 아이템 추가 테스트
            Debug.LogError("머니 치트");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GetInventory.AddPickUpItem(addItemTest);
            Debug.LogError("아이템 치트");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GetPlayer.TakeDamage();
            Debug.LogError("충돌");
        }

        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            GetFishing.SetFishingTest(addItemTest);
            Debug.LogError("물고기 치트");
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            Singleton_Continue.INSTANCE.SaveContinue();// Space 시 저장
            Debug.LogError("저장중");
        }
    }

    IEnumerator SetStart()
    {
        Camera_Manager.current.SetCameraManager();
        continueData = Singleton_Continue.INSTANCE.LoadContinue();

        while (CurrentLand == null)
            yield return null;

        GetTimeUI.SetStart(continueData);// 시간
        GetMainUI.SetMoney(continueData.money);// 돈

        SetRendererFeature();
        yield return null;

        GetMainUI.SetStart();
        GetSkill.SetStart();
        GetDialog.SetStart();
        GetFishGuide.SetStart();
        GetQuest.SetStart();
        GetChangeShip.SetStart();

        string shipID = continueData.shipData;
        if (Singleton_Data.INSTANCE.Dict_Ship.ContainsKey(shipID))
        {
            GetInventory.TryDestroySlot = continueData.destroySlot;// 부서진 슬롯

            shipData = Singleton_Data.INSTANCE.Dict_Ship[shipID];
            ChangeStatus(shipData);
            OutOfControll(false);
        }
        else
        {
            Debug.LogWarning("배 데이터 없음");
        }
    }

    void LoadingComplate()
    {
        SetThemeMusic();
        // 튜토리얼 시작
        if (continueData.shipData == "")
        {
            Debug.LogWarning("배가 없으면 튜토리얼 시작");
            // 배가 없으면 튜토리얼 시작
            //GetTutorial.SetTutorial(String_Tutorial._newGame);// 배구입 튜토리얼 세팅
            GetMainUI.OpenCanvas(false);
            Data_NPC npc = Singleton_Data.INSTANCE.Dict_NPC[String_NPC._player];
            GetDialog.DialogStart_NPC(npc, 4);
            return;
        }
        GetPlayer.CheckClosestUnit();// 생성 시 가까운 오브젝트 찾기
    }

    public void ChangeStatus(Data_Ship _shipData)
    {
        shipData = _shipData;
        GetPlayer.SetShip(_shipData);
        AddStatus();// 스테이트 세팅
    }

    public void AddStatus()
    {
        if (shipData == null)
            return;

        bool fullHealth = GetPlayer.FullHealth;// 스탯 적용 하기 전 풀피 체크
        currentStatus.SettingStatus(shipData.status);// 디폴트 스탯 적용
        currentStatus.AddStatus(GetAddStatus);// 추가 스탯 적용

        GetInventory.myBox.AddMaxWeight(currentStatus.maxWeight);// 인벤토리 무게 적용
        GetInventory.myBox.AddInventory(currentStatus.maxBoxSize);// 인벤토리 사이즈 적용

        GetPlayer.SetStatus();// 플레이어에 스탯 적용
    }

    public void SetThemeMusic()
    {
        Option_Manager.current.SetThemeMusic(null);
        Singleton_Audio.INSTANCE.Audio_Environment(String_Audio._oceanSound);
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
        GetPlayer.StateMove();
    }

    public void OutOfControll(bool _isOn)
    {
        Singleton_Controller.INSTANCE.outOfControll = _isOn;
        controllManager.ResetControll();

        GetPlayer.OutOfControll(_isOn);// 플레이어 못움직이게
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
        // 공격하는 물고기가 처음인지 확인
        Debug.LogWarning("낚시가 처음인지 확인 - 튜토리얼 시작");
        //GetTutorial.SetTutorial(String_Tutorial._firstFishing);// 낚시 튜토리얼 세팅
        //GetTutorial.StartTutorial();// 낚시 튜토리얼 시작
        GetFishing.SetFishing(_areaType);
    }
    Data_Continue continueData;
    public Data_Continue GetContinue { get { return continueData; } }
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
                if (GetContinue != null)
                {
                    instPlayer = Instantiate(player, GetContinue.playerPosition, GetContinue.playerRotation, transform);
                }
                else
                {
                    Transform landingPoint = CurrentLand.landingPoint.transform;
                    instPlayer = Instantiate(player, landingPoint.position, landingPoint.rotation, transform);
                }
                instPlayer.SetStart();
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

    private UI_QuestManager instQuest;
    public UI_QuestManager GetQuest
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

    private Rest_Manager instRest;
    public Rest_Manager GetRestManager
    {
        get
        {
            if (instRest == null)
            {
                instRest = Instantiate(rest_Manager, transform);
                instRest.SetStart();
            }
            return instRest;
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
                instTutorial.SetStart();
            }
            return instTutorial;
        }
    }

    private Tutorial_Manager1 instTutorial1;
    public Tutorial_Manager1 GetTutorial1
    {
        get
        {
            if (instTutorial1 == null)
            {
                instTutorial1 = Instantiate(tutorial1, transform);
                //instTutorial1.SetStart();
            }
            return instTutorial1;
        }
    }

    private UI_ChangeShip instChangeShip;
    public UI_ChangeShip GetChangeShip
    {
        get
        {
            if (instChangeShip == null)
            {
                instChangeShip = Instantiate(changeShip, transform);
            }
            return instChangeShip;
        }
    }

    private Event_Manager instEvent;
    public Event_Manager GetEvent
    {
        get
        {
            if (instEvent == null)
            {
                instEvent = Instantiate(event_Manager, transform);
            }
            return instEvent;
        }
    }

    //====================================================================================================================
    // 풀스크린 렌더러 피쳐 관련
    //====================================================================================================================

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

    //====================================================================================================================
    // 퀘스트 관련
    //====================================================================================================================

    //[System.Serializable]
    //public class QuestItem
    //{
    //    public Vector2Int slotNum;
    //    public Data_Quest[] quests;
    //    public QuestItem(Vector2Int _slotNum, Data_Quest[] _quests)
    //    {
    //        slotNum = _slotNum;
    //        quests = _quests;
    //    }
    //}
    //public List<QuestItem> questItems = new List<QuestItem>();
    //Dictionary<Vector2Int, Data_Quest[]> questSlots = new Dictionary<Vector2Int, Data_Quest[]>();// 아이템 위치
    //Dictionary<string, List<Data_Quest>> npcQuest = new Dictionary<string, List<Data_Quest>>();// 완료 확인용
    //public Data_Quest[] testDatas;

    //public void BuyNews(Vector2Int _slotNum)// 신문 구매시 퀘스트 세팅
    //{
    //    Data_Quest[] temp = new Data_Quest[3]
    //    {
    //        testDatas[0],
    //        testDatas[1],
    //        testDatas[2]
    //    };
    //    questSlots[_slotNum] = temp;
    //    questItems.Add(new QuestItem(_slotNum, temp));

    //    for (int i = 0; i < temp.Length; i++)
    //    {
    //        string npcID = temp[i].npc_ID;
    //        if (npcQuest.ContainsKey(npcID) == false)
    //            npcQuest[npcID] = new List<Data_Quest>();
    //        npcQuest[npcID].Add(temp[i]);
    //    }
    //    Debug.LogWarning($"신문 퀘스트 세팅 : {npcQuest.Count}");
    //}

    //public List<Data_Quest> TryQuestDialog(string _npcID)// NPC 퀘스트 리스팅
    //{
    //    if (npcQuest.ContainsKey(_npcID) == false)
    //        return null;

    //    List<Data_Quest> quests = npcQuest[_npcID];
    //    return quests;
    //}

    //public void ComplateQuest(Data_Quest _quest)// 완료 퀘스트 리스팅
    //{
    //    string npcID = _quest.npc_ID;
    //    List<Data_Quest> quests = npcQuest[npcID];
    //    quests.Remove(_quest);
    //}

    //====================================================================================================================
    // 플레이어 파괴
    //====================================================================================================================

    public Trigger_LostBox lostBox;
    Trigger_LostBox instLostBox;
    public void PlayerDestroy()
    {
        Vector3 setPosition = GetPlayer.transform.position;
        if (instLostBox == null)
            instLostBox = Instantiate(lostBox, setPosition, Quaternion.identity, transform);
        instLostBox.gameObject.SetActive(true);
        instLostBox.SetResult();

        GetInventory.myBox.EmptyInventoryAllSlot();// 인벤토리 초기화
        Debug.LogWarning($"고스트 오브젝트 : {instLostBox.name}");
    }

    public void FocusShip(bool _isOn)
    {
        CurrentLand.focusShip.gameObject.SetActive(_isOn);
    }
}
