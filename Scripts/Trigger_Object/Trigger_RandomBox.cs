using UnityEngine;
using static Data_Manager;

public class Trigger_RandomBox : Trigger_Setting
{
    public AreaType areaType;
    public Sprite iconImage;
    string[] itemRewards;//  보상 아이템 ID들
    public Data_ItemList dataItemList;
    public int randomItemCount = 3;// 랜덤으로 줄 아이템 개수

    public void SetAreaType(AreaType _areaType)
    {
        areaType = _areaType;
        deleTriggerAction = SetTrigger;
        GetIconSprite = iconImage;// 트리거 아이콘 설정
        SetItem();
    }

    void SetItem()
    {
        // 보상 아이템 세팅
        randomItemCount = Random.Range(0, 3);
        itemRewards = dataItemList.GetRandomItems(randomItemCount);
    }

    void SetTrigger()
    {
        StatsManager.current.CatchBox();// 박스 수집 체크
        Game_Manager.current.GetInventory.SetResult(itemRewards);// 결과 아이템 세팅
        Game_Manager.current.GetMainUI.OpenCanvas(false);// 메인 유아이 닫기
        Game_Manager.current.GetMainUI.dele_CloseButton = CloseButton;// 인벤토리의 닫기 버튼 세팅
        gameObject.SetActive(false);// 트리거 오브젝트 비활성화
    }

    void CloseButton()
    {
        Game_Manager.current.GetInventory.CloseResult();
        Game_Manager.current.GetMainUI.OpenCanvas(true);// 메인 유아이 닫기
    }
}
