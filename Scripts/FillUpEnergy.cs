using UnityEngine;
using UnityEngine.UI;

public class FillUpEnergy : MonoBehaviour
{
    public TMPro.TMP_Text energyText, energyPriceText;
    public Slider energySlider;
    int energyMaxAmount = 150;
    int energyPrice = 13;
    int prevEnergy;
    public int buyEnergy;

    void Start()
    {
        energySlider.onValueChanged.AddListener(SetEnergy);
        prevEnergy = 5;
        energySlider.value = prevEnergy;
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
        float addEnergy = _value - prevEnergy;
        buyEnergy = (int)(addEnergy * (energyMaxAmount / energyPrice));
        energyPriceText.text = $"{buyEnergy}원";
    }
}
