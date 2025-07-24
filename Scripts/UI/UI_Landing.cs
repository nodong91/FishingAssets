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

    public void SetStart(LandingSetting[] _landings)
    {
        for (int i = 0; i < _landings.Length; i++)
        {
            GameObject targetPoint = _landings[i].landingPoint;
            GameObject followUI = GetFollowUI(_landings[i].landingType);
            followUI.SetActive(true);
            Game_Manager.current.followManager.AddFollowUI(targetPoint, followUI);
        }
        SetOpenCanvas(true);
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

    void SetOpenCanvas(bool _open)
    {
        if (opening != null)
            StopCoroutine(opening);
        opening = StartCoroutine(SetCanvasAlpha(_open));
    }

    IEnumerator SetCanvasAlpha(bool _open)
    {
        if (_open == true)
            canvasGroup.gameObject.SetActive(true);

        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 3f;
            float alpha = (_open == true) ? normalize : 1f - normalize;
            canvasGroup.alpha = alpha;
            yield return null;
        }

        if (_open == false)
        {
            canvasGroup.gameObject.SetActive(false);
            // 모든 UI 제거
            landingPointUI.SetActive(false);
            fishShopUI.SetActive(false);
            eventUI.SetActive(false);
            shipyardUI.SetActive(false);
        }
    }

    void OutButton()
    {
        SetOpenCanvas(false);

        Game_Manager.current.OutOfControll(false);
        outLanding?.Invoke();
    }
    void RestButton()// 휴식
    {

    }

    void StorageButton()// 창고
    {
        Game_Manager.current.inventory.OpenStorage(true);
        
    }

    void ShopButton()
    {
        // 샵 버튼 누르면
        Game_Manager.current.inventory.OpenShop(true);
    }

    void ShipyardButton()// 조선소
    {

    }
}
