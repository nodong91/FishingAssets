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
    bool landUIOpen = false;

    public GameObject landingPointUI;
    public GameObject shopUI;
    public GameObject shipyardUI;
    public GameObject downTownUI;
    //public GameObject boardUI;

    LandingSetting[] landingData;
    public LandingSetting[] GetLandingData { get { return landingData; } }

    public delegate void DeleOutLanding();
    public DeleOutLanding outLanding;
    Coroutine opening;

    [Header(" [ Buttons ]")]
    public Custom_Button outButton;
    public Custom_Button fuelButton;
    public Custom_Button storageButton;
    public Custom_Button changeButton;

    public Custom_Button shopButton;
    public Custom_Button shipyardButton;
    public Custom_Button downTownButton;
    public Custom_Button boardButton;
    bool inlanding;

    public TMPro.TMP_Text fishshopInfo;
    public TMPro.TMP_Text shipyardInfo;
    public TMPro.TMP_Text villageInfo;
    public TMPro.TMP_Text noticeInfo;
    Dictionary<GameObject, GameObject> dictLandingUI = new Dictionary<GameObject, GameObject>();
    Custom_Button currentButton = null;

    public void SetStart()
    {
        OpenCanvasUI(canvasGroup, 0f);
        //canvasGroup.gameObject.SetActive(false);
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera_Manager.current.UICamera;

        outButton.SetButton(OutButton);
        fuelButton.SetButton(FuelButton);
        storageButton.SetButton(StorageButton);
        changeButton.SetButton(ChangeButton);

        shopButton.SetButton(ShopButton, EnterButton, ExitButton);
        shipyardButton.SetButton(ShipyardButton, EnterButton, ExitButton);
        downTownButton.SetButton(DownTownButton, EnterButton, ExitButton);
        boardButton.SetButton(BoardButton, EnterButton, ExitButton);
    }

    public void SetLanguage()
    {
        fishshopInfo.text = Singleton_Data.INSTANCE.GetLanguage(Const_ETC._fishshop);
        shipyardInfo.text = Singleton_Data.INSTANCE.GetLanguage(Const_ETC._shipyard);
        villageInfo.text = Singleton_Data.INSTANCE.GetLanguage(Const_ETC._village);
        noticeInfo.text = Singleton_Data.INSTANCE.GetLanguage(Const_ETC._notice);
    }

    void EnterButton(Custom_Button _button)
    {
        SetLanguage();
        currentButton = _button;
        _button.buttonImage.gameObject.SetActive(true);
    }

    void ExitButton(Custom_Button _button)
    {
        currentButton = null;
        _button.buttonImage.gameObject.SetActive(false);
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

        //SetLanguage();
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

                //case LandingSetting.LandingType.Board:
                //    return boardUI;
        }
        return null;
    }

    public void SetLandingCanvas(bool _open)
    {
        if (landUIOpen == _open)
            return;

        landUIOpen = _open;

        if (opening != null)
            StopCoroutine(opening);
        opening = StartCoroutine(SetCanvasAlpha());
    }

    IEnumerator SetCanvasAlpha()
    {
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 3f;
            float alpha = (landUIOpen == true) ? normalize : 1f - normalize;
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

    bool CheckShip()
    {
        if (currentButton != null)
            ExitButton(currentButton);
        if (Game_Manager.current.shipData == null)
        {
            Data_NPC data_NPC = Singleton_Data.INSTANCE.Dict_NPC[Const_NPC._player];
            if (Game_Manager.current.GetChangeShip.GetShipCount > 0)
            {
                Game_Manager.current.GetDialog.DialogStart_NPC(data_NPC, Const_Dialog._0009);// 배가 없다는 대사
            }
            else
            {
                Game_Manager.current.GetDialog.DialogStart_NPC(data_NPC, Const_Dialog._0003);// 배가 없다는 대사
            }
            SetLandingCanvas(false);// 랜드 UI 제거
            StaticOpenCanvas.deleEndOpen = EndDialog;
            return false;
        }
        return true;
    }

    bool CheckEnergy()
    {
        Unit_Player player = Game_Manager.current.GetPlayer;
        if (player.GetMaxEnergy > 0 && player.energy <= 0)
        {
            SetLandingCanvas(false);// 랜드 UI 제거
            Data_NPC data_NPC = Singleton_Data.INSTANCE.Dict_NPC[Const_NPC._player];
            Game_Manager.current.GetDialog.DialogStart_NPC(data_NPC, Const_Dialog._0010);// 연료가 필요하다는 대사
            StaticOpenCanvas.deleEndOpen = EndDialog;
            return false;
        }
        return true;
    }

    bool CheckFix()
    {
        Unit_Player player = Game_Manager.current.GetPlayer;
        if (player.health <= 0)
        {
            SetLandingCanvas(false);// 랜드 UI 제거
            Data_NPC data_NPC = Singleton_Data.INSTANCE.Dict_NPC[Const_NPC._player];
            Game_Manager.current.GetDialog.DialogStart_NPC(data_NPC, Const_Dialog._0004);// 수리가 필요하다는 대사
            StaticOpenCanvas.deleEndOpen = EndDialog;
            return false;
        }
        return true;
    }

    void EndDialog()
    {
        StaticOpenCanvas.deleEndOpen = null;
        SetLandingCanvas(true);
    }

    void OutButton()
    {
        // 나가기
        if (CheckShip() == false || CheckEnergy() == false || CheckFix() == false)
            return;

        //currentType = LandingType.None;
        inlanding = false;
        SetLandingCanvas(false);// 섬에서 나가기

        outLanding?.Invoke();
        //Game_Manager.current.GetInventory.CloseShop();
        Game_Manager.current.OutOfControll(false);
        RemoveUI();
    }

    public void FuelButton()// 연료 채우기
    {
        if (CheckShip() == false || currentType == LandingType.Energy)
            return;

        currentType = LandingType.Energy;
        SetLandingCanvas(false);// 창고 누르면 랜드 UI 제거
        Game_Manager.current.GetEnergyUI.OpenEnergy();
        Game_Manager.current.CurrentLand.CameraOutFouce(true);
    }

    public void RestButton()// 휴식
    {
        if (currentType == LandingType.Energy)
            return;

        currentType = LandingType.Rest;
        Game_Manager.current.GetRestManager.OpenCanvas(true);
        //Game_Manager.current.currentLand.CameraOutFouce(true);
    }

    void ShopButton()
    {
        if (CheckShip() == false || currentType == LandingType.Shop)
            return;

        currentType = LandingType.Shop;
        SetLandingCanvas(false);        // 샵 버튼 누르면 랜드 UI 제거
        Data_NPC data_NPC = Singleton_Data.INSTANCE.Dict_NPC[Const_NPC._shop];
        Option_Manager.current.SetThemeMusic(data_NPC.themeMusic);// NPC 테마 음악 설정
        Game_Manager.current.GetDialog.DialogStart_NPC(data_NPC, Const_Dialog._1001);
        Game_Manager.current.CurrentLand.CameraOutFouce(true);
    }

    public void ShipyardButton()// 조선소
    {
        if (currentType == LandingType.Shipyard)
            return;

        currentType = LandingType.Shipyard;
        SetLandingCanvas(false);        // 조선소 버튼 누르면 랜드 UI 제거
        Data_NPC data_NPC = Singleton_Data.INSTANCE.Dict_NPC[Const_NPC._shipyard];
        Option_Manager.current.SetThemeMusic(data_NPC.themeMusic);// NPC 테마 음악 설정
        Game_Manager.current.CurrentLand.CameraOutFouce(true);

        if (Game_Manager.current.shipData == null)// 배가 없다면
        {
            // 튜토리얼 시작
            Game_Manager.current.GetDialog.DialogStart_NPC(data_NPC, Const_Dialog._2003);
        }
        else
        {
            Game_Manager.current.GetDialog.DialogStart_NPC(data_NPC, Const_Dialog._2001);
        }
    }

    void DownTownButton()
    {
        Debug.LogWarning("다운타운 버튼");
        if (CheckShip() == false || currentType == LandingType.DownTown)
            return;

        currentType = LandingType.DownTown;

        SetLandingCanvas(false);        // 랜드 UI 제거
        Game_Manager.current.CurrentLand.CameraOutFouce(true);
        Data_NPC data_NPC = Singleton_Data.INSTANCE.Dict_NPC[Const_NPC._player];
        Game_Manager.current.GetDialog.DialogStart_NPC(data_NPC, Const_Dialog._0008);// 플레이어 대화

        //if (lightMode == Data_Manager.DayType.Night)       // 밤
        //{
        //    // 추가
        //    data_NPC = Singleton_Data.INSTANCE.Dict_NPC[Const_NPC._smuggler];// 밀수꾼 추가
        //    Game_Manager.current.GetDialog.AddNPC(data_NPC, Const_Dialog._4001);
        //}
    }

    void StorageButton()
    {
        if (CheckShip() == false || currentType == LandingType.Storage)
            return;

        currentType = LandingType.Storage;
        SetLandingCanvas(false);// 창고 누르면 랜드 UI 제거

        Game_Manager.current.GetMainUI.dele_CloseButton = BackButton;
        //Game_Manager.current.GetMainUI.OpenShop();// 창고
        Game_Manager.current.GetInventory.OpenStorage(true);
        Game_Manager.current.CurrentLand.CameraOutFouce(true);
    }

    public void ChangeButton()
    {
        Debug.LogWarning(Game_Manager.current.GetChangeShip.GetShipCount);
        if (Game_Manager.current.GetChangeShip.GetShipCount == 0)
        {
            CheckShip();
            return;
        }

        SetLandingCanvas(false);        // 랜드 UI 제거
        Game_Manager.current.GetChangeShip.OpenCanvas(true);
    }

    public void BoardButton()
    {
        if (CheckShip() == false || currentType == LandingType.Board)
            return;

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
                //Option_Manager.current.SetThemeMusic(null);
                break;

            case LandingType.Storage:
                Game_Manager.current.GetInventory.CloseShop();
                break;

            case LandingType.Energy:
                if (Game_Manager.current.GetEnergyUI.CloseEnergy() == true)
                {
                    return;
                }
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
        OpenIslandUI();
    }

    public void OpenIslandUI()
    {
        currentType = LandingType.None;
        SetLandingCanvas(true);// 랜드 UI 열기
        Game_Manager.current.CurrentLand.CameraOutFouce(false);
    }
}
