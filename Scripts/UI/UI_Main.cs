using System.Collections;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class UI_Main : MonoBehaviour
{
    [System.Flags]
    public enum MenuState
    {
        Inventory = 1 << 0,
        Fishing = 1 << 1,
        Quest = 1 << 2,
        Option = 1 << 3,
        Shop = 1 << 4,
        FishGuide = 1 << 5,
        Ghost = 1 << 6,
        Result = 1 << 7,
        Submit = 1 << 8,
    }
    public MenuState menuState;

    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    public UI_Time timeUI;
    public UI_Status statusUI;
    public Custom_Button inventoryButton;
    public Custom_Button fishGuideButton;
    public Custom_Button questButton;
    public Custom_Button optionButton;

    public CanvasGroup fadeScreen;
    public Canvas cameraCanvas;
    public TMPro.TMP_Text warnningText;
    Coroutine textActing;

    [Header("[ Ship ]")]
    public Slider shipEnergy;
    public Image currentHealthImage, maxHealthImage;
    Coroutine openFadeScreen;
    public Vector2 HealthSize;

    public void SetStart()
    {
        SetMoney(Singleton_Continue.INSTANCE.continueData.money);
        HealthSize = maxHealthImage.rectTransform.sizeDelta;

        inventoryButton.SetButton(InventoryButton);
        fishGuideButton.SetButton(FishGuideButton);
        questButton.SetButton(QuestButton);
        optionButton.SetButton(OptionButton);

        SetCameraCanvas();
        SetFadeScreen(false);
    }

    void SetCameraCanvas()
    {
        warnningText.alpha = 0f;
        cameraCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        cameraCanvas.worldCamera = Camera_Manager.current.UICamera;
    }

    public void CloseCanvas()
    {
        Debug.LogWarning($"캔버스 닫기 : {menuState}");
        if (menuState == 0)
            return;

        switch (menuState)
        {
            case MenuState.Inventory:
                menuState &= ~MenuState.Inventory;
                Game_Manager.current.OutOfControll(false);
                Game_Manager.current.GetInventory.OpenInventory(false);
                statusUI.OpenCanvas(false);
                OpenCanvas(true);
                break;

            case MenuState.Fishing:
                menuState &= ~MenuState.Fishing;
                Game_Manager.current.OutOfControll(false);
                Game_Manager.current.GetInventory.CloseShop();// 상점 닫기
                Game_Manager.current.GetFishing.FishingOver();
                break;

            case MenuState.Quest:
                menuState &= ~MenuState.Quest;
                Game_Manager.current.OutOfControll(false);
                Game_Manager.current.GetQuest.OpenCanvas(false);
                OpenCanvas(true);
                break;

            case MenuState.Option:
                menuState &= ~MenuState.Option;
                Game_Manager.current.OutOfControll(false);
                Option_Manager.current.OpenCanvas(false);
                OpenCanvas(true);
                break;

            case MenuState.Shop:
                menuState &= ~MenuState.Shop;
                Game_Manager.current.GetLanding.BackButton();
                break;

            case MenuState.FishGuide:
                menuState &= ~MenuState.FishGuide;
                Game_Manager.current.OutOfControll(false);
                Game_Manager.current.GetFishGuide.OpenCanvas(false);
                OpenCanvas(true);
                break;
            case MenuState.Ghost:
                menuState &= ~MenuState.Ghost;
                Game_Manager.current.OutOfControll(false);
                Game_Manager.current.GetInventory.OpenGhost(false);//유령 보상 닫기
                break;

            case MenuState.Result:
                menuState &= ~MenuState.Result;
                Game_Manager.current.OutOfControll(false);
                Game_Manager.current.GetInventory.CloseResult(true);//퀘스트 보상 닫기
                break;

            case MenuState.Submit:
                menuState |= MenuState.Quest;// 넣기
                menuState &= ~MenuState.Submit;
                Game_Manager.current.GetInventory.CloseSubMit();//퀘스트 보상 닫기
                break;
        }
    }

    void InventoryButton()
    {
        OpenCanvas(false);
        menuState |= MenuState.Inventory;// 넣기
        statusUI.OpenCanvas(true);
        Game_Manager.current.GetInventory.OpenInventory(true);
        Game_Manager.current.OutOfControll(true);
    }

    void FishGuideButton()
    {
        OpenCanvas(false);
        menuState |= MenuState.FishGuide;// 넣기
        Game_Manager.current.GetFishGuide.OpenCanvas(true);
        Game_Manager.current.OutOfControll(true);
    }

    void QuestButton()
    {
        OpenCanvas(false);
        menuState |= MenuState.Quest;// 넣기
        Game_Manager.current.GetQuest.OpenCanvas(true);
        Game_Manager.current.OutOfControll(true);
    }

    public void OptionButton()
    {
        OpenCanvas(false);
        menuState |= MenuState.Option;// 넣기
        Option_Manager.current.OpenCanvas(true);
        Game_Manager.current.OutOfControll(true);
        Debug.LogWarning("Option Button Clicked");
    }

    public void OpenShop()
    {
        menuState |= MenuState.Shop;// 넣기
        Game_Manager.current.GetLanding.OutDialog();// 대화 끝
    }

    public void FishingGame()
    {
        menuState |= MenuState.Fishing;// 넣기
        Game_Manager.current.GetInventory.OpenResult();// 낚시 보상
        //OpenCanvas(false);
    }

    public void OpenQuestResult()
    {
        menuState |= MenuState.Result;// 넣기
        Game_Manager.current.GetInventory.OpenResult();// 퀘스트 보상
    }

    public void GhostResult()
    {
        menuState |= MenuState.Ghost;
        Game_Manager.current.GetInventory.OpenGhost(true);// 퀘스트 보상
    }

    public void OpenSubmit()
    {
        menuState &= ~MenuState.Quest;
        menuState |= MenuState.Submit;
        Game_Manager.current.GetInventory.OpenSubmit();
    }

    public void OpenCanvas(bool _open)// 메인 유아이 캔버스
    {
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
    }

    //===========================================================================================================================
    // 경고 문구
    //===========================================================================================================================

    public void SetWarnningText(string _text)
    {
        warnningText.text = _text;
        if (textActing != null)
            StopCoroutine(textActing);
        textActing = StartCoroutine(TextActing());
    }

    IEnumerator TextActing()
    {
        warnningText.alpha = 1f;
        warnningText.material.SetFloat("_BurnAmount", 1f);
        yield return new WaitForSeconds(1f);

        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 5f;
            warnningText.alpha = 1f - normalize;
            warnningText.material.SetFloat("_BurnAmount", normalize);
            yield return null;
        }
    }

    public void SetFadeScreen(bool _open)
    {
        if (openFadeScreen != null)
            StopCoroutine(openFadeScreen);
        openFadeScreen = StartCoroutine(OpenFadeScreen(_open));
    }

    IEnumerator OpenFadeScreen(bool _open)
    {
        float prevAlpha = fadeScreen.alpha;
        float targetAlpha = _open == true ? 1f : 0f;
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 3f;
            fadeScreen.alpha = Mathf.Lerp(prevAlpha, targetAlpha, normalize);
            fadeScreen.blocksRaycasts = fadeScreen.alpha > 0f;
            fadeScreen.interactable = fadeScreen.alpha > 0f;
            yield return null;
        }
    }

    //===========================================================================================================================
    // 상태 체크
    //===========================================================================================================================

    Material healthMaterial, energyMaterial;
    bool lowEnergy = false;
    bool lowHP = false;

    public void SetEnergy(float _energy)
    {
        shipEnergy.value = _energy;
        if (_energy < 0.5f)
        {
            if (lowEnergy == false)
            {
                lowEnergy = true;
                if (energyMaterial == null)
                {
                    Image energy = shipEnergy.fillRect.GetComponent<Image>();
                    energyMaterial = Instantiate(energy.material);
                    energy.material = energyMaterial;
                }
                StartCoroutine(LowEnergy(energyMaterial));
            }
        }
        else if (lowEnergy == true)
        {
            lowEnergy = false;
        }
    }

    IEnumerator LowEnergy(Material _material)
    {
        float normalize = 0f;
        while (lowEnergy == true)
        {
            normalize += Time.deltaTime * 5f;
            float alpha = (Mathf.Sin(normalize) + 1f) * 0.5f;// 0~1까지
            _material.SetColor("_MainColor", Color.white * alpha * 3f);
            yield return null;
        }
        _material.SetColor("_MainColor", Color.white);
    }

    public void SetMaxHealthPoint(int _point)
    {
        RectTransform rectTransform = maxHealthImage.rectTransform;
        rectTransform.sizeDelta = new Vector2(HealthSize.x * _point, HealthSize.y);
    }

    public void SetHealthPoint(int _point)
    {
        RectTransform rectTransform = currentHealthImage.rectTransform;
        rectTransform.sizeDelta = new Vector2(HealthSize.x * _point, HealthSize.y);
        if (_point <= 1)
        {
            if (lowHP == false)
            {
                lowHP = true;
                if (healthMaterial == null)
                {
                    healthMaterial = Instantiate(currentHealthImage.material);
                    currentHealthImage.material = healthMaterial;
                }
                StartCoroutine(LowHP(healthMaterial));
            }
        }
        else if (lowHP == true)
        {
            lowHP = false;
        }
    }

    IEnumerator LowHP(Material _material)
    {
        float normalize = 0f;
        while (lowHP == true)
        {
            normalize += Time.deltaTime * 5f;
            float alpha = (Mathf.Sin(normalize) + 1f) * 0.5f;// 0~1까지
            _material.SetColor("_MainColor", Color.white * alpha * 3f);
            yield return null;
        }
        _material.SetColor("_MainColor", Color.white);
    }

    //===========================================================================================================================
    // 돈
    //===========================================================================================================================

    public TMPro.TMP_Text moneyText;
    Coroutine movingMoney;
    float moneyValue;
    public float TryMoney
    {
        get { return moneyValue; }
    }

    public void SetMoney(float _value)
    {
        float money = _value;
        moneyText.text = money.ToString();
        moneyValue = money;
    }

    public void MoveMoney(float _price)
    {
        if (moneyValue + _price < 0f)
            return;

        if (movingMoney != null)
            StopCoroutine(movingMoney);
        movingMoney = StartCoroutine(MoneyMoving(_price));
    }

    IEnumerator MoneyMoving(float _price)
    {
        float prevMoney = moneyValue;
        moneyValue = moneyValue + _price;
        Singleton_Continue.INSTANCE.SaveContinue(); // 팔거나 사면 저장
        yield return null;

        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime;
            float value = Mathf.Lerp(prevMoney, moneyValue, normalize);
            moneyText.text = Mathf.Round(value).ToString();
            yield return null;
        }
        moneyText.text = moneyValue.ToString();
    }

    public RectTransform moneyRect;
    private Coroutine shakeUI;
    public void NoMoney()
    {
        Debug.LogWarning("돈이 부족합니다.");
        if (shakeUI != null)
            StopCoroutine(shakeUI);
        shakeUI = StartCoroutine(ShakeUI());
    }

    IEnumerator ShakeUI()
    {
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 5f;
            Vector3 randomPos = Random.insideUnitSphere * 10f;
            Vector2 shakePosition = randomPos * (1f - normalize);
            moneyRect.anchoredPosition = shakePosition;
            yield return null;
        }
        moneyRect.anchoredPosition = Vector2.zero;
    }
}
