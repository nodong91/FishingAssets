using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Trigger_Landing;

public class UI_Landing : MonoBehaviour
{
    public Canvas canvas;
    public CanvasGroup canvasGroup;

    public GameObject landingPointUI;
    public GameObject fishShopUI;
    public GameObject eventUI;
    public GameObject shipyardUI;

    LandingStruct landingData;
    public LandingStruct GetLandingData { get { return landingData; } }

    public delegate void DeleOutLanding();
    public DeleOutLanding outLanding;
    Coroutine opening;
    [Header("Buttons")]
    public Button outButton;
    public Button restButton;
    public Button storageButton;
    public Button shopButton;
    public Button shipyardButton;
    bool inlanding, onDialog;
    public Button backButton;
    private CanvasGroup backCanvas;
    Dictionary<GameObject, GameObject> dictLandingUI = new Dictionary<GameObject, GameObject>();

    public void SetStart()
    {
        canvasGroup.gameObject.SetActive(false);
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Game_Manager.current.cameraManager.UICamera;

        outButton.onClick.AddListener(OutButton);
        shopButton.onClick.AddListener(ShopButton);
        shipyardButton.onClick.AddListener(ShipyardButton);
        storageButton.onClick.AddListener(StorageButton);
        backButton.onClick.AddListener(BackButton);
        backCanvas = backButton.GetComponent<CanvasGroup>();
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
                return fishShopUI;

            case LandingSetting.LandingType.Event:
                return eventUI;

            case LandingSetting.LandingType.Shipyard:
                return shipyardUI;
        }
        return null;
    }

    void SetLandingCanvas(bool _open)
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
            if (inlanding == true)
                OpenCanvasUI(backCanvas, 1f - alpha);
            yield return null;
        }
        if (inlanding == false)
            SaveData_Continue.current.SetContinue();// 섬에서 나갈 때 저장
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

        inlanding = false;
        SetLandingCanvas(false);// 섬에서 나가기

        outLanding?.Invoke();
        Game_Manager.current.OutOfControll(false);
        Game_Manager.current.GetInventory.CloseShop();
        RemoveUI();
    }

    void RestButton()// 휴식
    {

    }

    void ShopButton()
    {
        onDialog = true;
        SetLandingCanvas(false);        // 샵 버튼 누르면 랜드 UI 제거
        Option_Manager.current.SetThemeMusic(landingData.shopNPC.themeMusic);
        Game_Manager.current.GetDialog.DialogStart(landingData.shopNPC);
    }

    void ShipyardButton()// 조선소
    {
        onDialog = true;
        SetLandingCanvas(false);        // 조선소 버튼 누르면 랜드 UI 제거
        Option_Manager.current.SetThemeMusic(landingData.shipyardNPC.themeMusic);
        Game_Manager.current.GetDialog.DialogStart(landingData.shipyardNPC);
    }

    void StorageButton()
    {
        onDialog = false;
        SetLandingCanvas(false);// 창고 누르면 랜드 UI 제거
        Game_Manager.current.GetInventory.OpenStorage(true);
    }

    public void BackButton()
    {
        Game_Manager.current.GetInventory.CloseShop();
        OutDialog();// 대화창 닫기
        Option_Manager.current.SetThemeMusic(null);
        SetLandingCanvas(true);// 백버튼
    }

    public void OutDialog()
    {
        if (onDialog == true)
        {
            onDialog = false;
            Game_Manager.current.GetDialog.OutDialog();
        }
    }
}
