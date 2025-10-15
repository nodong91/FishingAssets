using UnityEngine;
using UnityEngine.UI;

public class Energy_Manager : MonoBehaviour
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    public TMPro.TMP_Text energyText, energyPriceText;
    public Slider energySlider;
    float energyMaxAmount = 150;
    int energyPrice = 13;
    float prevEnergy;
    public int buyPrice;
    public float addEnergy;
    public Custom_Button buyButton, backButton;

    public void SetStart()
    {
        energySlider.onValueChanged.AddListener(SetEnergy);
        buyButton.SetButton(FillUpEnergy);
        backButton.SetButton(Game_Manager.current.GetLanding.BackButton);
        addEnergy = 0f;
    }

    public void OpenEnergy()
    {
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, true));
        SetStartEnergy();
    }

    void SetStartEnergy()
    {
        energyMaxAmount = Game_Manager.current.GetPlayer.GetMaxEnergy;
        float energy = Game_Manager.current.GetPlayer.GetEnergy;
        prevEnergy = (energy / energyMaxAmount) * 100f;// 0~100
        Debug.LogWarning($"{energy}/{energyMaxAmount}={prevEnergy}");
        energySlider.value = prevEnergy;
        SetEnergy(prevEnergy);
    }

    public void CloseEnergy()
    {
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, false));
    }

    void FillUpEnergy()
    {
        if (Game_Manager.current.CheckMoney(buyPrice) == false)
            return;

        float energy = addEnergy * energyMaxAmount / 100f;
        Debug.LogWarning($"에너지 충전 {addEnergy}% , {energy}만큼 충전");
        Game_Manager.current.GetPlayer.AddEnergy(energy);
        Game_Manager.current.GetMainUI.MoveMoney(buyPrice);

        SetStartEnergy();
    }

    void SetEnergy(float _value)
    {
        if (_value < prevEnergy)
        {
            energySlider.value = prevEnergy;
            return;
        }
        energyText.text = $"{(int)_value}<size=10>%</size>";
        // 1%당 비용  energyMaxAmount / energyPrice;
        addEnergy = _value - prevEnergy;
        buyPrice = (int)(addEnergy * (energyMaxAmount / 100f * energyPrice));
        energyPriceText.text = $"{buyPrice}원";
    }
}
