using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class Energy_Manager : MonoBehaviour
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    public TMPro.TMP_Text energyText, energyPriceText;
    public Slider energySlider;
    float EnergyMaxAmount => Game_Manager.current.GetPlayer.GetMaxEnergy;
    int energyPrice = 13;// 1%당 가격
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
        float energy = Game_Manager.current.GetPlayer.GetEnergy;
        prevEnergy = Mathf.Max(energy / EnergyMaxAmount, 0);// 0~1
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

        float energy = addEnergy * EnergyMaxAmount / 100f;
        Debug.LogWarning($"에너지 충전 {addEnergy}% , {energy}만큼 충전");
        Game_Manager.current.GetPlayer.AddEnergy(energy);
        Game_Manager.current.GetMainUI.MoveMoney(-buyPrice);

        SetStartEnergy();
    }

    void SetEnergy(float _value)
    {
        if (_value < prevEnergy)// 감소 불가
        {
            energySlider.value = prevEnergy;
            return;
        }
        addEnergy = (_value - prevEnergy) * 100f;
        energyText.text = $"{(int)(_value * 100f)}<size=75%>%</size>";
        // 1%당 비용  energyMaxAmount / energyPrice;
        buyPrice = (int)(addEnergy * (EnergyMaxAmount / 100f * energyPrice));
        buyPrice = Mathf.Max(buyPrice, 0);
        energyPriceText.text = $"{buyPrice}원";
        buyButton.gameObject.SetActive(buyPrice > 0);// 충전량 있을때만 활성
    }
}
