using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Trigger_Landing;

public class UI_Landing : MonoBehaviour
{
    public enum LandingType
    {
        None,
        Shop,// 생선 가게
        Shipyard,// 조선소
        Storage,// 창고
        Energy,// 주유소
        DownTown,// 다운타운
        Rest,// 휴식
        Board,
        Count
    }
    public LandingType currentType = LandingType.None;
    public Canvas canvas;
    public CanvasGroup canvasGroup;

    public GameObject landingPointUI;
    public GameObject shopUI;
    public GameObject shipyardUI;
    public GameObject downTownUI;
    public GameObject boardUI;

    LandingSetting[] landingData;
    public LandingSetting[] GetLandingData { get { return landingData; } }

    public delegate void DeleOutLanding();
    public DeleOutLanding outLanding;
    Coroutine opening;

    [Header(" [ Buttons ]")]
    public Custom_Button outButton;
    public Custom_Button restButton;
    public Custom_Button fuelButton;
    public Custom_Button storageButton;
    public Custom_Button changeButton;

    public Custom_Button shopButton;
    public Custom_Button shipyardButton;
    public Custom_Button downTownButton;
    public Custom_Button boardButton;
    bool inlanding;
    Data_Manager.DayType lightMode => Game_Manager.current.GetMainUI.timeUI.lightMode;
    Dictionary<GameObject, GameObject> dictLandingUI = new Dictionary<GameObject, GameObject>();

    public void SetStart()
    {
        canvasGroup.gameObject.SetActive(false);
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera_Manager.current.UICamera;

        outButton.SetButton(OutButton);
        fuelButton.SetButton(FuelButton);
        restButton.SetButton(RestButton);
        storageButton.SetButton(StorageButton);
        changeButton.SetButton(ChangeButton);

        shopButton.SetButton(ShopButton);
        shipyardButton.SetButton(ShipyardButton);
        downTownButton.SetButton(DownTownButton);
        boardButton.SetButton(BoardButton);
    }

    public void SetLanding(LandingSetting[] _landingData)
    {
        inlanding = true;
        // 어떤 섬인지 확인
        landingData = _landingData;// 개별 섬의 정보
        for (int i = 0; i < _landingData.Length; i++)
        {
            GameObject targetPoint = _landingData[i].landingPoint;
            GameObject followUI = GetFollowUI(_landingData[i].landingType);
            dictLandingUI[targetPoint] = followUI;
            followUI.SetActive(true);
            Game_Manager.current.GetFollow.AddFollowUI(targetPoint, followUI);
        }
        SetLandingCanvas(true);// 시작
        Game_Manager.current.OutOfControll(true);
    }

    void RemoveUI()
    {
        for (int i = 0; i < landingData.Length; i++)
        {
            GameObject targetPoint = landingData[i].landingPoint;
            dictLandingUI[targetPoint].SetActive(false);
            Game_Manager.current.GetFollow.RemoveFollowUI(targetPoint);
        }
    }

    GameObject GetFollowUI(LandingSetting.LandingType _type)
    {
        switch (_type)
        {
            case LandingSetting.LandingType.LandingPoint:
                return landingPointUI;

            case LandingSetting.LandingType.FishShop:
                return shopUI;

            case LandingSetting.LandingType.DownTown:
                return downTownUI;

            case LandingSetting.LandingType.Shipyard:
                return shipyardUI;

            case LandingSetting.LandingType.Board:
                return boardUI;
        }
        return null;
    }

    public void SetLandingCanvas(bool _open)
    {
        if (opening != null)
            StopCoroutine(opening);
        opening = StartCoroutine(SetCanvasAlpha(_open));
    }

    IEnumerator SetCanvasAlpha(bool _open)
    {
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 3f;
            float alpha = (_open == true) ? normalize : 1f - normalize;
            OpenCanvasUI(canvasGroup, alpha);
            //if (inlanding == true)
            //    OpenCanvasUI(backCanvas, 1f - alpha);
            yield return null;
        }
        if (inlanding == false)
            Singleton_Continue.INSTANCE.SaveContinue();// 섬에서 나갈 때 저장
    }

    void OpenCanvasUI(CanvasGroup _canvas, float _alpha)
    {
        _canvas.alpha = _alpha;
        _canvas.gameObject.SetActive(_alpha > 0);
    }

    void OutButton()
    {
        if (Game_Manager.current.GetPlayer.OutLandingCheck() == false)// 플레이어가 나갈 수 있는지 체크
            return;

        currentType = LandingType.None;
        inlanding = false;
        SetLandingCanvas(false);// 섬에서 나가기

        outLanding?.Invoke();
        Game_Manager.current.OutOfControll(false);
        Game_Manager.current.GetInventory.CloseShop();
        RemoveUI();
    }

    public void FuelButton()// 연료 채우기
    {
        currentType = LandingType.Energy;
        SetLandingCanvas(false);// 창고 누르면 랜드 UI 제거
        Game_Manager.current.GetEnergyUI.OpenEnergy();
        Game_Manager.current.CurrentLand.CameraOutFouce(true);
    }

    public void RestButton()// 휴식
    {
        currentType = LandingType.Rest;
        //SetLandingCanvas(false);// 랜드 UI 제거
        Game_Manager.current.GetRestManager.OpenCanvas(true);
        //Game_Manager.current.currentLand.CameraOutFouce(true);
    }

    void ShopButton()
    {
        currentType = LandingType.Shop;
        SetLandingCanvas(false);        // 샵 버튼 누르면 랜드 UI 제거
        Data_NPC data_NPC = Singleton_Data.INSTANCE.Dict_NPC[String_NPC._shop];
        Option_Manager.current.SetThemeMusic(data_NPC.themeMusic);
        Game_Manager.current.GetDialog.DialogStart_NPC(data_NPC, 0);
        Game_Manager.current.CurrentLand.CameraOutFouce(true);
    }

    public void ShipyardButton()// 조선소
    {
        currentType = LandingType.Shipyard;
        SetLandingCanvas(false);        // 조선소 버튼 누르면 랜드 UI 제거
        Data_NPC data_NPC = Singleton_Data.INSTANCE.Dict_NPC[String_NPC._shipyard];
        Option_Manager.current.SetThemeMusic(data_NPC.themeMusic);
        Game_Manager.current.GetDialog.DialogStart_NPC(data_NPC, 0);
        Game_Manager.current.CurrentLand.CameraOutFouce(true);
    }

    void DownTownButton()
    {
        currentType = LandingType.DownTown;
        if (lightMode == Data_Manager.DayType.Day)
        {
            // 낮
            SetLandingCanvas(false);        // 랜드 UI 제거
            Game_Manager.current.CurrentLand.CameraOutFouce(true);
            Data_NPC data_NPC = Singleton_Data.INSTANCE.Dict_NPC[String_NPC._player];
            Game_Manager.current.GetDialog.DialogStart_NPC(data_NPC, 0);// 플레이어 대화
        }
        else if (lightMode == Data_Manager.DayType.Night)
        {
            // 밤
            SetLandingCanvas(false);        // 랜드 UI 제거
            Game_Manager.current.CurrentLand.CameraOutFouce(true);
            Data_NPC data_NPC = Singleton_Data.INSTANCE.Dict_NPC[String_NPC._player];
            Game_Manager.current.GetDialog.DialogStart_NPC(data_NPC, 0);// 플레이어 대화

            data_NPC = Singleton_Data.INSTANCE.Dict_NPC[String_NPC._smuggler];// 밀수꾼 추가
            Game_Manager.current.GetDialog.AddNPC(data_NPC, 0);

            RandomEvent();
        }
    }
    public Data_Event randomEvent;
    void RandomEvent()
    {
        string title = randomEvent.eventName;
        Game_Manager.current.GetDialog.EventSelectButton(title);
    }

    void StorageButton()
    {
        currentType = LandingType.Storage;
        SetLandingCanvas(false);// 창고 누르면 랜드 UI 제거

        Game_Manager.current.GetMainUI.OpenShop();// 창고
        Game_Manager.current.GetInventory.OpenStorage(true);
        Game_Manager.current.CurrentLand.CameraOutFouce(true);
    }

    public void ChangeButton()
    {
        SetLandingCanvas(false);        // 랜드 UI 제거
        Game_Manager.current.GetChangeShip.OpenCanvas(true);
    }

    public void BoardButton()
    {
        currentType = LandingType.Board;
        SetLandingCanvas(false);// 창고 누르면 랜드 UI 제거

        Game_Manager.current.GetNews.OpenNewsPaper();// 신문 열기
        Game_Manager.current.CurrentLand.CameraOutFouce(true);
    }

    public void BackButton()// 뒤로 가기
    {
        Debug.LogWarning("뒤로 가기 : " + currentType);
        switch (currentType)
        {
            case LandingType.None:

                break;

            case LandingType.Shop:
            case LandingType.Shipyard:
                Game_Manager.current.GetInventory.CloseShop();// 상점 닫기
                Option_Manager.current.SetThemeMusic(null);
                break;

            case LandingType.Storage:
                Game_Manager.current.GetInventory.CloseShop();
                break;

            case LandingType.Energy:
                Game_Manager.current.GetEnergyUI.CloseEnergy();
                break;

            case LandingType.Rest:
                Game_Manager.current.GetRestManager.OpenCanvas(false);
                break;

            case LandingType.Board:
                //Game_Manager.current.GetNews.OpenNewsPaper();// 신문 열기
                Debug.LogWarning("보드 선택");
                break;

            case LandingType.DownTown:
                Game_Manager.current.GetInventory.CloseShop();// 상점 닫기
                break;
        }
        OpenLandingUI();
    }

    public void OpenLandingUI()
    {
        SetLandingCanvas(true);// 랜드 UI 열기
        Game_Manager.current.CurrentLand.CameraOutFouce(false);
    }
}
