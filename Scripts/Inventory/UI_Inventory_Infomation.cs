using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory_Infomation : MonoBehaviour
{
    public TMPro.TMP_Text m_itemName, m_Explanation;
    public Button buyButton, sellButton;

    public delegate void Dele_Button();
    public Dele_Button deleBuyButton, deleSellButton;

    public void SetInfomation()
    {
        buyButton.onClick.AddListener(BuyButton);
        sellButton.onClick.AddListener(SellButton);
    }

    public void SetDisplay(Data_Manager.ItemStruct _itemStruct)
    {
        m_itemName.text = _itemStruct.Name;
        string explanation = "";
        explanation += "Explanation : " + _itemStruct.Explanation + "\n";
        explanation += "Icon : " + _itemStruct.Icon.name + "\n";
        explanation += "Size : " + _itemStruct.Size.Length + "\n";
        explanation += "Weight : " + _itemStruct.Weight + "\n";
        explanation += "Price : " + _itemStruct.Price;
        m_Explanation.text = explanation;
    }

    void BuyButton()
    {
        deleBuyButton?.Invoke();
    }

    void SellButton()
    {
        deleSellButton?.Invoke();
    }
}
