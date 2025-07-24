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
        Game_Manager.current.inventory.OpenInventory(true);

        ItemStruct fishItem = fishStruct.itemStruct;
        float size = randomSize.size;

        Game_Manager.current.inventory.AddItem(fishItem);// 인벤토리에 생선 추가
        Game_Manager.current.instFishGuide.AddFishClass(fishItem.id, size);
    }
    FishStruct fishStruct;
    FishStruct.RandomSize randomSize;
    public void SetFish(FishStruct _fishStruct, FishStruct.RandomSize _randomSize)
    {
        fishStruct = _fishStruct;
        randomSize = _randomSize;
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
