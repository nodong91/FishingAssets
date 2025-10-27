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
        DownTown,
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

    LandingStruct landingData;
    public LandingStruct GetLandingData { get { return landingData; } }

    public delegate void DeleOutLanding();
    public DeleOutLanding outLanding;
    Coroutine opening;

    [Header("Buttons")]
    public Custom_Button outButton;
    public Custom_Button restButton;
    public Custom_Button fuelButton;
    public Custom_Button storageButton;
    public Custom_Button shopButton;
    public Custom_Button shipyardButton;
    public Custom_Button downTownButton;
    public Custom_Button boardButton;
    bool inlanding, onDialog;

    Dictionary<GameObject, GameObject> dictLandingUI = new Dictionary<GameObject, GameObject>();

    public void SetStart()
    {
        canvasGroup.gameObject.SetActive(false);
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera_Manager.current.UICamera;
        nothingBoard.gameObject.SetActive(false);

        outButton.SetButton(OutButton);
        fuelButton.SetButton(FuelButton);
        restButton.SetButton(RestButton);
        shopButton.SetButton(ShopButton);
        shipyardButton.SetButton(ShipyardButton);
        storageButton.SetButton(StorageButton);
        downTownButton.SetButton(DownTownButton);
        boardButton.SetButton(BoardButton);
        //backButton.SetButton(BackButton);
        //backCanvas = backButton.GetComponent<CanvasGroup>();
    }

    public void SetLanding(LandingStruct _landingData)
    {
        inlanding = true;
        // 어떤 섬인지 확인
        landingData = _landingData;// 개별 섬의 정보
        for (int i = 0; i < _landingData.landingSetting.Length; i++)
        {
            GameObject targetPoint = _landingData.landingSetting[i].landingPoint;
            GameObject followUI = GetFollowUI(_landingData.landingSetting[i].landingType);
            dictLandingUI[targetPoint] = followUI;
            followUI.SetActive(true);
            Game_Manager.current.GetFollow.AddFollowUI(targetPoint, followUI);
        }
        SetLandingCanvas(true);// 시작
        Game_Manager.current.OutOfControll(true);
    }

    void RemoveUI()
    {
        for (int i = 0; i < landingData.landingSetting.Length; i++)
        {
            GameObject targetPoint = landingData.landingSetting[i].landingPoint;
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

    void FuelButton()// 휴식
    {
        currentType = LandingType.Energy;
        onDialog = false;
        SetLandingCanvas(false);// 창고 누르면 랜드 UI 제거
        Game_Manager.current.GetEnergyUI.OpenEnergy();
        Game_Manager.current.currentLand.CameraOutFouce(true);
    }

    void RestButton()// 휴식
    {
        currentType = LandingType.Rest;
        onDialog = false;
        SetLandingCanvas(false);// 랜드 UI 제거
        Game_Manager.current.GetRestManager.OpenCanvas(true);
        Game_Manager.current.currentLand.CameraOutFouce(true);
    }

    void ShopButton()
    {
        currentType = LandingType.Shop;
        onDialog = true;
        SetLandingCanvas(false);        // 샵 버튼 누르면 랜드 UI 제거
        Option_Manager.current.SetThemeMusic(landingData.shopNPC.themeMusic);
        Game_Manager.current.GetDialog.DialogStart(landingData.shopNPC);
        Game_Manager.current.currentLand.CameraOutFouce(true);
    }

    void ShipyardButton()// 조선소
    {
        currentType = LandingType.Shipyard;
        onDialog = true;
        SetLandingCanvas(false);        // 조선소 버튼 누르면 랜드 UI 제거
        Option_Manager.current.SetThemeMusic(landingData.shipyardNPC.themeMusic);
        Game_Manager.current.GetDialog.DialogStart(landingData.shipyardNPC);
        Game_Manager.current.currentLand.CameraOutFouce(true);
    }
    public CanvasGroup nothingBoard;
    int Hour => Game_Manager.current.GetMainUI.timeUI.hour;
    void DownTownButton()
    {
        if (Hour >= 5f && Hour < 18f)
        {
            // 낮
            onDialog = false;
            StartCoroutine(OpenNothingBoard());
            Debug.LogWarning("이벤트 시간 : " + Hour);
        }
        else
        {
            // 밤
            currentType = LandingType.DownTown;
            onDialog = true;
            SetLandingCanvas(false);        // 랜드 UI 제거
            Game_Manager.current.currentLand.CameraOutFouce(true);
            Option_Manager.current.SetThemeMusic(landingData.smugglerNPC.themeMusic);
            Game_Manager.current.GetDialog.DialogStart(landingData.smugglerNPC);
        }
    }

    IEnumerator OpenNothingBoard()
    {
        nothingBoard.gameObject.SetActive(true);
        nothingBoard.alpha = 1.0f;
        float normalize = 0;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 0.3f;
            nothingBoard.alpha = Mathf.Clamp((1.0f - normalize) * 3f, 0f, 1f);
            yield return null;
        }
        nothingBoard.gameObject.SetActive(false);
        currentType = LandingType.None;
    }

    void StorageButton()
    {
        currentType = LandingType.Storage;
        onDialog = false;
        SetLandingCanvas(false);// 창고 누르면 랜드 UI 제거

        Game_Manager.current.GetMainUI.OpenShop();// 창고
        Game_Manager.current.GetInventory.OpenStorage(true);
        Game_Manager.current.currentLand.CameraOutFouce(true);
    }

    void BoardButton()
    {
        currentType = LandingType.Board;
        onDialog = false;
        SetLandingCanvas(false);// 창고 누르면 랜드 UI 제거

        Game_Manager.current.GetNews.OpenNewsPaper();// 신문 열기
        Game_Manager.current.currentLand.CameraOutFouce(true);
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
                OutDialog();// 대화창 닫기
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
        Game_Manager.current.currentLand.CameraOutFouce(false);
        SetLandingCanvas(true);// 랜드 UI 열기
    }

    public void OutDialog()
    {
        if (onDialog == true)
        {
            onDialog = false;
            Game_Manager.current.GetDialog.OutDialog();
        }
    }

    public void OpenLandingUI()
    {
        SetLandingCanvas(true);// 랜드 UI 열기
    }
}
