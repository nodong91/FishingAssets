using System.Collections;
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

    public Button outButton;
    public Button restButton;
    public Button storageButton;
    public Button shopButton;
    public Button shipyardButton;

    public void SetStart()
    {
        canvasGroup.gameObject.SetActive(false);
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Game_Manager.current.cameraManager.UICamera;

        outButton.onClick.AddListener(OutButton);
        shopButton.onClick.AddListener(ShopButton);
        shipyardButton.onClick.AddListener(ShipyardButton);
        storageButton.onClick.AddListener(StorageButton);
    }

    public void SetLanding(LandingStruct _landingData)
    {
        // 어떤 섬인지 확인
        landingData = _landingData;// 개별 섬의 정보
        for (int i = 0; i < _landingData.landingSetting.Length; i++)
        {
            GameObject targetPoint = _landingData.landingSetting[i].landingPoint;
            GameObject followUI = GetFollowUI(_landingData.landingSetting[i].landingType);
            followUI.SetActive(true);
            Game_Manager.current.GetFollow.AddFollowUI(targetPoint, followUI);
        }
        SetLandingCanvas(true);
        Game_Manager.current.OutOfControll(true);
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
        // 열 때 출력
        if (_open == true)
        {
            OpenPointUI(true);
        }

        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 3f;
            float alpha = (_open == true) ? normalize : 1f - normalize;
            canvasGroup.alpha = alpha;
            yield return null;
        }
        // 닫을 때 제거
        if (_open == false)
        {
            OpenPointUI(false);
        }
    }

    void OpenPointUI(bool _open)
    {
        canvasGroup.gameObject.SetActive(_open);
        landingPointUI.SetActive(_open);
        fishShopUI.SetActive(_open);
        eventUI.SetActive(_open);
        shipyardUI.SetActive(_open);
    }

    void OutButton()
    {
        SetLandingCanvas(false);

        Game_Manager.current.OutOfControll(false);
        outLanding?.Invoke();
        Game_Manager.current.GetInventory.shop.deleOutInventory = null;
        Game_Manager.current.GetInventory.CloseShop();
        SaveData_Continue.current.SetContinue();// 섬에서 나갈 때 저장
    }

    void RestButton()// 휴식
    {

    }

    void OutInventory()
    {
        // 닫히면 다시 랜드 유아이 뜨게
        SetLandingCanvas(true);
        Debug.LogWarning("OutInventory");
    }

    void ShopButton()
    {
        SetLandingCanvas(false);
        Game_Manager.current.GetInventory.shop.deleOutInventory = OutInventory;
        // 샵 버튼 누르면
        Game_Manager.current.GetDialog.DialogStart(landingData.shopNPC);
    }

    void ShipyardButton()// 조선소
    {
        SetLandingCanvas(false);
        //Game_Manager.current.inventory.OpenShipyard(true, landingData);
        Game_Manager.current.GetInventory.shop.deleOutInventory = OutInventory;
        // 샵 버튼 누르면
        Game_Manager.current.GetDialog.DialogStart(landingData.shipyardNPC);
    }

    void StorageButton()// 창고
    {
        SetLandingCanvas(false);
        Game_Manager.current.GetInventory.OpenStorage(true);
        Game_Manager.current.GetInventory.shop.deleOutInventory = OutInventory;
    }
}
