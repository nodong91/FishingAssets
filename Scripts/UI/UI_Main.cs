using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Game_Manager;

public class UI_Main : MonoBehaviour
{
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
    public Image shipEnergyMask;
    public Image shipEnergy;
    public RectTransform shipEnergyRect;
    public Image currentHealthImage, maxHealthImage;
    Coroutine openFadeScreen;
    public Vector2 HealthSize;

    Material healthMaterial, energyMaterial;
    bool lowEnergy = false;
    bool lowHP = false;

    public void SetStart()
    {
        SetMoney(Game_Manager.current.GetContinue.money);
        HealthSize = maxHealthImage.rectTransform.sizeDelta;

        inventoryButton.SetButton(InventoryButton);
        fishGuideButton.SetButton(FishGuideButton);
        questButton.SetButton(QuestButton);
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
        statusUI.OpenCanvas(true);
        Game_Manager.current.GetInventory.OpenInventory(true);
        Game_Manager.current.OutOfControll(true);
    }

    void CloseInventory()
    {
        Game_Manager.current.OutOfControll(false);
        Game_Manager.current.GetInventory.OpenInventory(false);
        statusUI.OpenCanvas(false);
        OpenCanvas(true);
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

    void QuestButton()
    {
        OpenCanvas(false);
        dele_CloseButton = CloseQuest;
        Game_Manager.current.GetQuest.OpenCanvas(true);
        Game_Manager.current.OutOfControll(true);
    }

    void CloseQuest()
    {
        Game_Manager.current.OutOfControll(false);
        Game_Manager.current.GetQuest.OpenCanvas(false);
        OpenCanvas(true);
    }

    public void OptionButton()
    {
        OpenCanvas(false);
        dele_CloseButton = CloseOption;
        Option_Manager.current.OpenCanvas(true);
        Game_Manager.current.OutOfControll(true);
        Debug.LogWarning("Option Button Clicked");
    }

    void CloseOption()
    {
        Game_Manager.current.OutOfControll(false);
        Option_Manager.current.OpenCanvas(false);
        OpenCanvas(true);
    }

    //===========================================================================================================================
    // 열기
    //===========================================================================================================================

    public void OpenCanvas(bool _open)// 메인 유아이 캔버스
    {
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
    }

    //===========================================================================================================================
    // 닫기
    //===========================================================================================================================
    public delegate void Dele_CloseButton();
    public Dele_CloseButton dele_CloseButton;
    public void CloseCanvas()
    {
        dele_CloseButton?.Invoke();
        dele_CloseButton = null;
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

    public void SetMaxEnergyPoint(float _energy)
    {
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
        if (_price == 0 || moneyValue + _price < 0f)
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










    //===========================================================================================================================
    // 버프
    //===========================================================================================================================
    [Header(" [ 버프 ]")]
    public GridLayoutGroup buffGrid;
    public UI_BuffSlot buffSlotPrefab;
    Dictionary<string, UI_BuffSlot> dictBuffSlots = new Dictionary<string, UI_BuffSlot>();
    Queue<UI_BuffSlot> buffSlotPool = new Queue<UI_BuffSlot>();

    public void AddBuffSlot(FishBuffStruct _buff)
    {
        if (dictBuffSlots.ContainsKey(_buff.id) == false)
        {
            dictBuffSlots[_buff.id] = TryBuffSlot();
            dictBuffSlots[_buff.id].gameObject.SetActive(true);
            dictBuffSlots[_buff.id].OnBuffEnd = RemoveBuffSlot;
        }
        dictBuffSlots[_buff.id].SetBuffSlot(_buff);
    }

    public void AddBuffSlot(BuffStruct _buff)
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
}
