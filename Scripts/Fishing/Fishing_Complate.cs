using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;

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

        Trigger_Fish fish = catchFish.GetTriggerFish;
        ItemStruct fishItem = fish.fishStruct.itemStruct;
        float size = fish.randomSize.size;

        Game_Manager.current.inventory.AddItem(fishItem);// 인벤토리에 생선 추가
        Game_Manager.current.fishGuide.AddFishClass(fishItem.id, size);
    }
    Trigger_Setting catchFish;

    public void SetFish(Trigger_Setting _fish)
    {
        catchFish = _fish;
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
