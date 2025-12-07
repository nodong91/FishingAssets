using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static Data_Manager;

public class Game_Manager : MonoBehaviour
{
    public Unit_Player player;
    public Controll_Manager controllManager;

    [Header("[ Managers ]")]
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
    public UI_ChangeShip changeShip;
    public Event_Manager event_Manager;
    public Map_Minimap mapMinimap;

    [Header(" [ 밤낮 ]")]
    public Material skyboxMatial;
    public Light dayLight;
    public Color dayColor, nightColor;
    [ColorUsage(true, true)]
    public Color emissionColor;
    public Trigger_Landing CurrentLand { get; set; }
    [Header(" [ 플레이어 파괴 ]")]
    public Trigger_LostBox lostBox;
    Trigger_LostBox instLostBox;

    [Header(" [ 빈 오브젝트 ]")]
    public Data_Ship shipData;
    public SetStatus currentStatus;
    public string addItemTest;
    public SetStatus GetAddStatus => GetSkill.skill_Setting.AddShipStatus();

    [Header(" [ 이어하기 ]")]
    public float loanTime = 0f;
    Data_Continue continueData;
    public Data_Continue GetContinue { get { return continueData; } }

    public static Game_Manager current;

    private void Awake()
    {
        current = this;
    }

    void Start()
    {
        if (LoadingManager.current != null)
            LoadingManager.current.deleComplate = LoadingComplate;// 로딩 완료 딜리게이트 등록

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
    }

    public void InputSpacebar(bool _input)
    {
        GetPlayer.ActiveBooster(_input);
    }

    public void SetEtc()
    {
        SetBooster();
    }

    public void SetBooster()
    {
        GetSkill.skill_Setting.GetBooster(out float _boosterSpeed, out float _boosterValue);
        player.SetBooster(_boosterSpeed, _boosterValue);
        // 유아이 세팅
        GetMainUI.SetMaxBoosterValue(_boosterSpeed, _boosterValue);
    }

    public bool TryCrashChance()
    {
        float crashChance = GetSkill.skill_Setting.GetCrashChance();
        float randomChance = Random.Range(0, 100f);
        Debug.LogWarning($"회피 확률 : {crashChance} > {randomChance}");
        return crashChance > randomChance;
    }
    public string[] bgms;
    Coroutine randomTheme;
    IEnumerator SetStart()
    {

        Camera_Manager.current.SetCameraManager();
        continueData = Singleton_Continue.INSTANCE.LoadContinue();

        while (CurrentLand == null)// 맵이 있는지 체크
            yield return null;

        GetTimeUI.SetStart(continueData);// 시간
        GetMainUI.SetMoney(continueData.money);// 돈

        SetRendererFeature();
        yield return null;

        GetMainUI.SetStart();
        GetDialog.SetStart();
        GetFishGuide.SetStart();
        GetQuest.SetStart();
        GetChangeShip.SetStart();
        GetSkill.SetStart();
        GetMinimap.SetStart();

        string shipID = continueData.shipData;// 배 세팅
        if (Singleton_Data.INSTANCE.Dict_Ship.ContainsKey(shipID) == true)
        {
            shipData = Singleton_Data.INSTANCE.Dict_Ship[shipID];
            GetInventory.TryDestroySlot = continueData.destroySlot;// 부서진 슬롯
            ChangeStatus(shipData);
        }
    }

    void LoadingComplate()
    {
        SetThemeMusic();
        bool isCompleted = Tutorial_Manager.current.IsTutorialCompleted(Const_Tutorial._newGame);
        Debug.LogWarning($"튜토리얼 완료? {Const_Tutorial._newGame} {isCompleted}----------{continueData.shipData}");
        // 튜토리얼 시작
        if (isCompleted == true)
        {
            string shipID = continueData.shipData;
            if (Singleton_Data.INSTANCE.Dict_Ship.ContainsKey(shipID) == true)// 배가 있다면
            {
                // 튜토리얼 완료 했으면
                OutOfControll(false);
                GetPlayer.CheckClosestUnit();// 생성 시 가까운 오브젝트 찾기
            }
            else
            {
                GetMainUI.OpenCanvas(false);
                Data_NPC npc = Singleton_Data.INSTANCE.Dict_NPC[Const_NPC._player];
                GetDialog.DialogStart_NPC(npc, Const_Dialog._0001);// 튜토리얼 대화 시작
            }
        }
        else
        {
            GetMainUI.OpenCanvas(false);
            Data_NPC npc = Singleton_Data.INSTANCE.Dict_NPC[Const_NPC._player];
            GetDialog.DialogStart_NPC(npc, Const_Dialog._0002);// 튜토리얼 대화 시작
            return;
        }
    }

    public void SetThemeMusic()
    {
        SetTheme();
        Singleton_Audio.INSTANCE.Audio_Environment(Const_Audio._oceanSound);
    }

    IEnumerator RandomTheme()
    {
        string bgmID = bgms[Random.Range(0, bgms.Length - 1)];
        float length = Singleton_Data.INSTANCE.Dict_Audio[bgmID].clip.length;
        Option_Manager.current.SetThemeMusic(bgmID);
        yield return new WaitForSeconds(length);
        SetTheme();
    }

    void SetTheme()
    {
        if (randomTheme != null)
            StopCoroutine(randomTheme);
        randomTheme = StartCoroutine(RandomTheme());
    }

    public void ChangeStatus(Data_Ship _shipData)// 선박 변경
    {
        bool isCompleted = Tutorial_Manager.current.IsTutorialCompleted(Const_Tutorial._newGame);
        if (isCompleted == false)
        {
            Tutorial_Manager.current.CompletedTutorial(Const_Tutorial._newGame);// 튜토완료
        }

        continueData = Singleton_Continue.INSTANCE.LoadContinue();
        shipData = _shipData;
        GetPlayer.SetShip(_shipData);
        AddStatus();// 선박 변경 스테이트 세팅

        Singleton_Continue.INSTANCE.SaveContinue();
    }

    public void AddStatus()
    {
        if (shipData == null)
            return;

        bool fullHealth = GetPlayer.FullHealth;// 스탯 적용 하기 전 풀피 체크
        currentStatus.SettingStatus(shipData.status);// 디폴트 스탯 적용
        currentStatus.AddStatus(GetAddStatus);// 추가 스탯 적용
        //for (int i = 0; i < buffList.Count; i++)
        //{
        //    currentStatus.AddStatus(buffList[i].addStatus);// 버프 스탯 적용
        //}

        GetInventory.myBox.AddMaxWeight(currentStatus.maxWeight);// 인벤토리 무게 적용
        GetInventory.myBox.AddInventory(currentStatus.maxBoxSize);// 인벤토리 사이즈 적용
        GetPlayer.SetStatus();// 플레이어에 스탯 적용
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
        Debug.Log($"돈 체크 : {GetMainUI.TryMoney} / {_price}");
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
                instLottery.SetStart();
            }
            return instLottery;
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

    private Map_Minimap instMinimap;
    public Map_Minimap GetMinimap
    {
        get
        {
            if (instMinimap == null)
            {
                instMinimap = Instantiate(mapMinimap, transform);
            }
            return instMinimap;
        }
    }

    //====================================================================================================================
    // 풀스크린 렌더러 피쳐 관련
    //====================================================================================================================
    [Header(" [ 풀스크린 ]")]
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
        Debug.LogWarning($"랜더피쳐 카운트 : {scriptableRendererData.rendererFeatures.Count}");

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
    // 플레이어 파괴
    //====================================================================================================================

    public void PlayerDestroy()
    {
        Vector3 setPosition = GetPlayer.transform.position;
        if (instLostBox == null)
            instLostBox = Instantiate(lostBox, setPosition, Quaternion.identity, transform);
        instLostBox.gameObject.SetActive(true);
        instLostBox.SetResult();

        GetInventory.myBox.EmptyInventoryAllSlot();// 인벤토리 초기화
        Debug.LogWarning($"고스트 오브젝트 : {instLostBox.name}");
        GetMinimap.SetLostBox(instLostBox.gameObject);// 미니맵에 표시
        Singleton_Continue.INSTANCE.SaveContinue();// 상황 저장
    }

    public void FocusShip(bool _isOn)
    {
        CurrentLand.focusShip.gameObject.SetActive(_isOn);
    }















    //====================================================================================================================
    // 버프 관련
    //====================================================================================================================

    public class FishBuffStruct
    {
        public string id;
        public float buffStartTime;
        public float duration;

        [Header(" [ 미끼 ]")]
        public ItemStruct.ItemClass itemClass;
        public float addValue;
    }
    public FishBuffStruct fishBuff;
    public FishBuffStruct GetFishBuff { get { return fishBuff; } }

    public struct BuffStruct
    {
        public string id;
        public string iconSprite;
        public float buffStartTime;
        public float duration;

        [Header(" [ 기타 ]")]
        public float etcValue;
        public string skillID;
    }
    public Dictionary<string, BuffStruct> dictBuff = new Dictionary<string, BuffStruct>();

    public void AddBuff(FishStruct _fishStruct)
    {
        // 물고기를 사용하여 버프 추가
        FishBuffStruct buff = new FishBuffStruct
        {
            id = _fishStruct.itemStruct.itemType.ToString(),
            itemClass = _fishStruct.itemStruct.itemClass,// 미끼 효과 종류
            duration = _fishStruct.addDuration,
            addValue = _fishStruct.addValue,// 미끼 효과 퍼센트
        };
        GetMainUI.AddBuffSlot(buff);
        fishBuff = buff;
    }

    public void RemoveFishBuff()
    {
        fishBuff = null;
    }

    public void AddBuff(UsedStruct _usedStruct)
    {
        BuffStruct buff = new BuffStruct
        {
            id = _usedStruct.itemStruct.id,
            iconSprite = _usedStruct.itemStruct.icon,
            buffStartTime = Time.time,
            duration = _usedStruct.buffDuration,
            etcValue = _usedStruct.etcValue,
            skillID = _usedStruct.skillID
        };
        GetMainUI.AddBuffSlot(buff);
        dictBuff[buff.id] = buff;
        //AddStatus();
    }
    // 커맨드가 점점 짧아져서 실력이 늘고 있다는 것을 암시하는 효과

    //====================================================================================================================
    // 게임 오버 관련
    //====================================================================================================================
    public float loanPrice = 0f;
    public float loanInterest = 0f;
    public void LoanStart()
    {
        GetMainUI.timeUI.StartLoanTimer(true);// 대출금 상환 타이머 시작
        loanPrice = 1000;
        GetMainUI.SetLoanText(loanPrice);
        Debug.LogWarning(" 대출금 상환 타이머 시작.");
    }

    public void LoanEnd()// 대출 상환
    {
        Data_NPC npc = Singleton_Data.INSTANCE.Dict_NPC[Const_NPC._inn];
        if (CheckMoney(loanPrice + loanInterest) == true)
        {
            GetMainUI.timeUI.StartLoanTimer(false);// 대출금 상환 타이머 종료
            GetMainUI.MoveMoney(-loanPrice);
            loanPrice = 0;
            GetMainUI.SetLoanText(loanPrice);
            if (gameOver == true)
            {
                GetDialog.DialogStart_NPC(npc, Const_Dialog._3009);
            }
            else
            {
                GetDialog.DialogStart_NPC(npc, Const_Dialog._3006);
            }
            Debug.LogWarning(" 대출금 상환 타이머 종료.");
        }
        else
        {
            // 상환 날짜가 다가왔는지 확인
            if (gameOver == true)
            {
                GetDialog.DialogStart_NPC(npc, Const_Dialog._3008);// 돈이 모자란거 같아요 대화
            }
            else
            {
                GetDialog.DialogStart_NPC(npc, Const_Dialog._3007);// 돈이 모자란거 같아요 대화
                Debug.LogWarning(" 대출금 상환 실패.");
            }
        }
    }
    bool gameOver = false;
    public void GameOver()
    {
        gameOver = true;
        loanInterest += loanPrice * 0.1f;// 빌린 돈의 10%
        if (loanInterest > loanPrice)
        {
            Debug.LogWarning("대출금 상환 시간이 도래했습니다!\n모든 세이브 파일이 삭제됩니다.");
            OutOfControll(true);
            GetMainUI.OpenCanvas(false);

            Data_NPC npc = Singleton_Data.INSTANCE.Dict_NPC[Const_NPC._inn];
            GetDialog.DialogStart_NPC(npc, Const_Dialog._3003);// 튜토리얼 대화 시작
        }
        else
        {
            int loanText = (int)(loanPrice + loanInterest);
            GetMainUI.SetLoanText(loanText);
        }
    }
}
