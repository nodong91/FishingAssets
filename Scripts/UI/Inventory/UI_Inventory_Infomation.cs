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
        m_itemName.text = _itemStruct.name;
        string explanation = "";
        explanation += "Explanation : " + _itemStruct.explanation + "\n";
        explanation += "Icon : " + _itemStruct.icon.name + "\n";
        explanation += "Size : " + _itemStruct.shape.Length + "\n";
        explanation += "Weight : " + _itemStruct.weight + "\n";
        explanation += "Price : " + _itemStruct.price;
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
