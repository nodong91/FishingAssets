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
    public Button inventoryButton;
    public Button fishGuideButton;
    public Button questButton;
    public Button optionButton;

    public CanvasGroup fadeScreen;
    public Canvas cameraCanvas;
    public TMPro.TMP_Text warnningText;
    Coroutine textActing;
    [Header("[ Ship ]")]
    public Image shipEnergy;
    public Image currentHealthImage, maxHealthImage;
    Coroutine openFadeScreen;
    public Vector2 HealthSize;

    public void SetStart()
    {
        SetMoney(SaveData_Continue.current.continueData.money);
        moneyPosition = moneyTextObject.transform.position;
        HealthSize = maxHealthImage.rectTransform.sizeDelta;

        inventoryButton.onClick.AddListener(InventoryButton);
        fishGuideButton.onClick.AddListener(FishGuideButton);
        questButton.onClick.AddListener(QuestButton);
        optionButton.onClick.AddListener(OptionButton);

        SetCameraCanvas();
        SetFadeScreen(false);
    }

    void SetCameraCanvas()
    {
        warnningText.alpha = 0f;
        cameraCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        cameraCanvas.worldCamera = Camera_Manager.current.UICamera;
    }

    void InventoryButton()
    {
        if ((menuState & MenuState.Inventory) == 0)
        {
            menuState |= MenuState.Inventory;// 넣기
        }
        else
        {
            menuState &= ~MenuState.Inventory;
        }
        bool onInventory = (menuState & MenuState.Inventory) != 0;
        Game_Manager.current.GetInventory.OpenInventory(onInventory);
        statusUI.OpenCanvas(onInventory);
    }

    void FishGuideButton()
    {
        menuState |= MenuState.Fishing;// 넣기
        Game_Manager.current.GetFishGuide.OpenCanvas(true);
    }

    void QuestButton()
    {
        menuState |= MenuState.Quest;// 넣기
        Game_Manager.current.GetQuestUI.OpenCanvas(true);
    }

    void OptionButton()
    {
        menuState |= MenuState.Option;// 넣기
        Option_Manager.current.OpenCanvas(true);
        Debug.LogWarning("Option Button Clicked");
    }

    public void OpenCanvas(bool _open)
    {
        StaticOpenCanvas.deleEndOpen = null;
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
        shipEnergy.fillAmount = _energy;
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
        //RectTransform rectTransform = currentHealthImage.rectTransform;
        //rectTransform.sizeDelta = new Vector2(HealthSize.x * _point, HealthSize.y);
        float maxHealthPoint = Game_Manager.current.currentStatus.shipHealth;
        currentHealthImage.fillAmount = _point / maxHealthPoint;
    }
    //float maxHealthPoint;
    public void SetMaxHealthPoint(int _point)
    {
        //maxHealthPoint = _point;
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
        set
        {
            float money = value;
            moneyText.text = money.ToString();
            moneyValue = money;
        }
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
        SaveData_Continue.current.SetContinue(); // 팔거나 사면 저장
        yield return null;

        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime;
            float textValue = Mathf.Lerp(prevMoney, moneyValue, normalize);
            moneyText.text = Mathf.Round(textValue).ToString();

            //if (_price < 0f)// 판매인 경우
            //{
            //    if (moneyValue <= prevMoney)
            //        moveMoney = false;
            //}
            //else if (_price > 0f)// 구매인 경우
            //{
            //    if (moneyValue >= prevMoney)
            //        moveMoney = false;
            //}
            yield return null;
        }
    }

    public void NoMoney()
    {
        moneyTextObject.transform.position = moneyPosition;
        Debug.LogError("돈이 부족합니다.");
        //StartCoroutine(ShakeUI());
    }
    public GameObject moneyTextObject;
    public Vector3 moneyPosition;
    IEnumerator ShakeUI()
    {
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 5f;
            Vector3 shakePosition = Random.insideUnitSphere * 0.3f * (1f - normalize);
            moneyTextObject.transform.position = moneyPosition + shakePosition;
            yield return null;
        }
        moneyTextObject.transform.position = moneyPosition;
    }
}
