using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fishing_Complate : MonoBehaviour
{
    public GameObject fishInfomation;
    public Button closeButton;

    void Start()
    {
        fishInfomation.gameObject.SetActive(false);
        closeButton.onClick.AddListener(CloseButton);
    }

    void CloseButton()
    {
        fishInfomation.gameObject.SetActive(false);

        Game_Manager.current.inventory.OpenCanvas(true);
        Data_Manager.ItemStruct fishStruct = Game_Manager.current.inventory.GetTempItem();
        Game_Manager.current.inventory.AddItem(fishStruct);
    }

    public void SetFish(Trigger_Setting _fish)
    {
        StartCoroutine(SetDisplaying());
    }

    IEnumerator SetDisplaying()
    {
        fishInfomation.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f); // 연출 시간???

        closeButton.gameObject.SetActive(true);
    }
}
