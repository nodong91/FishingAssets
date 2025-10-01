using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_Main : MonoBehaviour
{
    [System.Flags]
    public enum MenuState
    {
        Inventory = 1 << 0,
        Fishing = 1 << 1,
        Quest = 1 << 2,
        Option = 1 << 3,
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

    void AllClose()
    {
        switch (menuState)
        {
            case MenuState.Inventory:
                menuState &= ~MenuState.Inventory;
                Game_Manager.current.GetInventory.OpenInventory(false);
                //statusUI.OpenCanvas(false);
                break;
            case MenuState.Fishing:
                menuState &= ~MenuState.Fishing;
                Game_Manager.current.GetFishGuide.OpenCanvas(false);
                break;
            case MenuState.Quest:
                menuState &= ~MenuState.Quest;
                Game_Manager.current.GetQuestUI.OpenCanvas(false);
                break;
            case MenuState.Option:
                menuState &= ~MenuState.Option;
                Option_Manager.current.OpenCanvas(false);
                break;
        }
    }

    void InventoryButton()
    {
        AllClose();
        //if ((menuState & MenuState.Inventory) == 0)
        //{
            menuState |= MenuState.Inventory;// 넣기
        //}
        //else
        //{
        //    menuState &= ~MenuState.Inventory;
        //}
        //bool onInventory = (menuState & MenuState.Inventory) != 0;
        //Game_Manager.current.GetInventory.OpenInventory(onInventory);
        //statusUI.OpenCanvas(onInventory);
        Game_Manager.current.GetInventory.OpenInventory(true);
    }

    void FishGuideButton()
    {
        AllClose();
        menuState |= MenuState.Fishing;// 넣기
        Game_Manager.current.GetFishGuide.OpenCanvas(true);
    }

    void QuestButton()
    {
        AllClose();
        menuState |= MenuState.Quest;// 넣기
        Game_Manager.current.GetQuestUI.OpenCanvas(true);
    }

    void OptionButton()
    {
        AllClose();
        menuState |= MenuState.Option;// 넣기
        Option_Manager.current.OpenCanvas(true);
        Debug.LogWarning("Option Button Clicked");
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
        yield return new WaitForSeconds(1f);
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 5f;
            warnningText.alpha = 1f - normalize;
            yield return null;
        }
    }

    public void SetEnergy(float _energy)
    {
        shipEnergy.value = _energy;
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

    public void SetHealthPoint(int _point)
    {
        //float maxHealthPoint = Game_Manager.current.currentStatus.shipHealth;
        //currentHealthImage.fillAmount = _point / maxHealthPoint;
        //Debug.LogWarning($"SetHealthPoint : {_point} / {maxHealthPoint}");

        RectTransform rectTransform = currentHealthImage.rectTransform;
        rectTransform.sizeDelta = new Vector2(HealthSize.x * _point, HealthSize.y);
    }

    public void SetMaxHealthPoint(int _point)
    {
        RectTransform rectTransform = maxHealthImage.rectTransform;
        rectTransform.sizeDelta = new Vector2(HealthSize.x * _point, HealthSize.y);
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
        Singleton_Continue.INSTANCE.SetContinue(); // 팔거나 사면 저장
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
