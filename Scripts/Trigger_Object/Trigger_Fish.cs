using UnityEngine;
using static Data_Manager;

public class Trigger_Fish : MonoBehaviour
{
    public Trigger_Setting triggerSetting;
    public Sprite catchIcon;

    public void SetAreaType(AreaType _areaType)
    {
        triggerSetting.deleTriggerAction = FishingStart;// 낚시 게임 스타트
        triggerSetting.GetIconSprite = catchIcon;// 트리거 아이콘 설정
    }

    void FishingStart()
    {
        if (Game_Manager.current.CheckLicense() == false)// 낚시 면허가 없으면 사용 불가
            return;

        Game_Manager.current.GetFishing.SetFishingTrigger();
        triggerSetting.gameObject.SetActive(false);// 트리거 오브젝트 비활성화
    }
}
