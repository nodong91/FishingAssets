using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Main : MonoBehaviour
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    public CanvasGroup mainAll;
    public UI_Time timeUI;
    //public UI_Status statusUI;
    public Custom_Button inventoryButton;
    public Custom_Button fishGuideButton;
    public Custom_Button mapButton;
    public Custom_Button optionButton;
    public Canvas cameraCanvas;
    public TMPro.TMP_Text warnningText;
    Coroutine textActing;

    public Custom_Button backButton;

    [Header("[ FadeScreen ]")]
    public CanvasGroup fadeScreen;
    public TMPro.TMP_Text fadeText;

    [Header("[ Ship ]")]
    public Image shipEnergyMask;
    public Image shipEnergy;
    public RectTransform shipEnergyRect;
    public Image currentHealthImage, maxHealthImage;
    Coroutine openFadeScreen;
    public Vector2 HealthSize;

    Material healthMaterial, energyMaterial;
    bool lowEnergy = false;
    bool lowHP = false;

    [Header("[ Booster ]")]
    public RectTransform boosterRect;
    public Image boosterGage;

    public delegate void Dele_CloseButton();
    public Dele_CloseButton dele_CloseButton;

    public void SetStart()
    {
        SetMoney(Game_Manager.current.GetContinue.money);// 로드 저장
        HealthSize = maxHealthImage.rectTransform.sizeDelta;

        inventoryButton.SetButton(InventoryButton);
        fishGuideButton.SetButton(FishGuideButton);
        mapButton.SetButton(MapButton);
        optionButton.SetButton(OptionButton);

        if (energyMaterial == null)
        {
            energyMaterial = Instantiate(shipEnergy.material);
            shipEnergy.material = Instantiate(energyMaterial);
        }

        warnningText.alpha = 0f;
        SetUICameraCanvas();
        SetFadeScreen(false);
    }

    void SetUICameraCanvas()
    {
        cameraCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        cameraCanvas.worldCamera = Camera_Manager.current.UICamera;
    }

    void InventoryButton()
    {
        OpenCanvas(false);
        dele_CloseButton = CloseInventory;
        //statusUI.OpenCanvas(true);
        Game_Manager.current.GetInventory.OpenInventory(true);
        Game_Manager.current.OutOfControll(true);
    }

    public void CloseInventory()
    {
        OpenCanvas(true);
        //statusUI.OpenCanvas(false);
        Game_Manager.current.GetInventory.OpenInventory(false);
        Game_Manager.current.OutOfControll(false);
    }

    void FishGuideButton()
    {
        OpenCanvas(false);
        dele_CloseButton = CloseFishGrude;
        Game_Manager.current.GetFishGuide.OpenCanvas(true);
        Game_Manager.current.OutOfControll(true);
    }

    void CloseFishGrude()
    {
        Game_Manager.current.OutOfControll(false);
        Game_Manager.current.GetFishGuide.OpenCanvas(false);
        OpenCanvas(true);
    }

    void MapButton()
    {
        OpenCanvas(false);
        dele_CloseButton = CloseMap;
        //Game_Manager.current.GetQuest.OpenCanvas(true);
        Game_Manager.current.OutOfControll(true);
        Game_Manager.current.GetMinimap.OpenCanvas(true);
    }

    void CloseMap()
    {
        Game_Manager.current.OutOfControll(false);
        Game_Manager.current.GetMinimap.OpenCanvas(false);
        OpenCanvas(true);
    }

    public void OptionButton()
    {
        //OpenCanvas(false);
        StartCoroutine(MainAllOpen(false));
        //dele_CloseButton = CloseOption;
        Option_Manager.current.OpenCanvas(true);
        Game_Manager.current.OutOfControll(true);
        Debug.LogWarning("Option Button Clicked");
    }

    IEnumerator MainAllOpen(bool _open)
    {
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 3f;
            mainAll.alpha = _open == true ? normalize : 1f - normalize;
            yield return null;
        }
        mainAll.alpha = (int)(_open == true ? normalize : 1f - normalize);
    }

    public void CloseOption()
    {
        StartCoroutine(MainAllOpen(true));
        Game_Manager.current.OutOfControll(false);
        Option_Manager.current.OpenCanvas(false);
        //OpenCanvas(true);
    }

    //===========================================================================================================================
    // 열기
    //===========================================================================================================================

    public void OpenCanvas(bool _open)// 메인 유아이 캔버스
    {
        Debug.LogWarning("// 메인 유아이 캔버스");
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
        timeUI.TimePause(!_open);
    }

    //===========================================================================================================================
    // 닫기
    //===========================================================================================================================


    public void CloseCanvas()
    {
        dele_CloseButton?.Invoke();
        dele_CloseButton = null;
    }

    public void DelayCloseCanvas()
    {
        if (StaticOpenCanvas.deleEndOpen == null)
        {
            CloseCanvas();
        }
    }

    //===========================================================================================================================
    // 경고 문구
    //===========================================================================================================================

    public void SetWarnningText(string _text)
    {
        warnningText.text = Singleton_Data.INSTANCE.GetLanguage(_text);
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
        fadeText.text = Singleton_Data.INSTANCE.GetLanguage(Const_ETC._1026);

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

    public void SetMaxEnergyPoint(float _energy)
    {
        //Debug.LogWarning($"최대 연료량 : {_energy}");
        shipEnergyRect.gameObject.SetActive(_energy > 0f);
        shipEnergyRect.sizeDelta = new Vector2((1f + _energy) * 2f, shipEnergyRect.sizeDelta.y);
    }

    public void SetEnergy(float _energy)
    {
        shipEnergyMask.fillAmount = _energy;
        if (_energy < 0.2f)
        {
            if (lowEnergy == false)// 에너지   경고
            {
                lowEnergy = true;
                StartCoroutine(LowEnergy(energyMaterial));
            }
        }
        else if (lowEnergy == true)
        {
            lowEnergy = false;
            energyMaterial.SetColor("_MainColor", Color.white);
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

    [Header("[ Money ]")]
    public TMPro.TMP_Text moneyText;
    Coroutine movingMoney;
    float moneyValue;
    public RectTransform moneyRect;
    private Coroutine shakeUI;

    public RectTransform loanRect;
    public TMPro.TMP_Text loanText;

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
        if (_price == 0 || moneyValue + _price < 0f)
            return;

        if (movingMoney != null)
            StopCoroutine(movingMoney);
        movingMoney = StartCoroutine(MoneyMoving(_price));
    }

    IEnumerator MoneyMoving(float _price)
    {
        float prevMoney = moneyValue;
        Steam_StatsManager.current.StatsMoney((int)moneyValue);
        Singleton_Continue.INSTANCE.SaveContinue(); // 팔거나 사면 저장
        Singleton_Audio.INSTANCE.Audio_FX(Const_Audio._money);
        yield return null;

        SetMoney(moneyValue + _price);// 돈 이동
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

    public void NoMoney()
    {
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

    public void SetLoanText(float _loan)
    {
        loanText.text = "-" + _loan.ToString();
        loanRect.gameObject.SetActive(_loan > 0);
    }

    //===========================================================================================================================
    // 버프
    //===========================================================================================================================
    [Header(" [ 버프 ]")]
    public GridLayoutGroup buffGrid;
    public UI_BuffSlot buffSlotPrefab;
    Dictionary<string, UI_BuffSlot> dictBuffSlots = new Dictionary<string, UI_BuffSlot>();
    Queue<UI_BuffSlot> buffSlotPool = new Queue<UI_BuffSlot>();

    public void AddBuffSlot(Game_Manager.FishBuffStruct _buff)
    {
        if (dictBuffSlots.ContainsKey(_buff.id) == false)
        {
            dictBuffSlots[_buff.id] = TryBuffSlot();
            dictBuffSlots[_buff.id].gameObject.SetActive(true);
            dictBuffSlots[_buff.id].OnBuffEnd = RemoveBuffSlot;
        }
        dictBuffSlots[_buff.id].SetBuffSlot(_buff);
    }

    public void AddBuffSlot(Game_Manager.BuffStruct _buff)
    {
        if (dictBuffSlots.ContainsKey(_buff.id) == false)
        {
            dictBuffSlots[_buff.id] = TryBuffSlot();
            dictBuffSlots[_buff.id].gameObject.SetActive(true);
            dictBuffSlots[_buff.id].OnBuffEnd = RemoveBuffSlot;
        }
        dictBuffSlots[_buff.id].SetBuffSlot(_buff);
    }

    public void RemoveBuffSlot(UI_BuffSlot _slot)
    {
        buffSlotPool.Enqueue(_slot);
        _slot.gameObject.SetActive(false);
        if (_slot.buffType == UI_BuffSlot.BuffType.FishBuff)
        {
            Game_Manager.current.RemoveFishBuff();
        }
        else if (_slot.buffType == UI_BuffSlot.BuffType.GeneralBuff)
        {

        }
        if (dictBuffSlots.ContainsKey(_slot.buffID) == false)
            return;
        dictBuffSlots.Remove(_slot.buffID);
    }

    UI_BuffSlot TryBuffSlot()
    {
        if (buffSlotPool.Count > 0)
        {
            return buffSlotPool.Dequeue();
        }
        UI_BuffSlot inst = Instantiate(buffSlotPrefab, buffGrid.transform);
        return inst;
    }


    //===========================================================================================================================
    // 깊이 체크
    //===========================================================================================================================
    Data_Manager.AreaType areaType;
    public Data_Manager.AreaType GetAreaType { get { return areaType; } }
    public TMPro.TMP_Text deepText;
    public void CheckDeep(Data_Manager.AreaType _areaType)
    {
        areaType = _areaType;
        deepText.gameObject.SetActive(_areaType != Data_Manager.AreaType.None);
        deepText.text = _areaType.ToString();
    }



    //===========================================================================================================================
    // 부스터
    //===========================================================================================================================

    public void SetMaxBoosterValue(float _speed, float _value)
    {
        boosterRect.gameObject.SetActive(_speed * _value > 0f);
        boosterRect.sizeDelta = new Vector2(100f + _value * 20f, boosterRect.sizeDelta.y);
        //Debug.LogWarning($"{_speed}  {_value}");
    }

    public void SetBoosterGage(float _value)
    {
        boosterGage.fillAmount = _value;
    }
}
