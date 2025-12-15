using UnityEngine;
using static Data_Manager;

public class Trigger_Fish : MonoBehaviour
{
    public Trigger_Setting triggerSetting;
    public Sprite catchIcon, dontCatchIcon;

    public void SetAreaType(AreaType _areaType)
    {
        triggerSetting.deleTriggerAction = FishingStart;// 낚시 게임 스타트

        //if (Game_Manager.current.CheckLicense() == false)// 낚시 면허가 없으면 사용 불가
        //    triggerSetting.GetIconSprite = catchIcon;// 트리거 아이콘 설정
        //else
        //    triggerSetting.GetIconSprite = dontCatchIcon;
        triggerSetting.deleTriggerEnter = CheckLicense;
    }
    bool onFishing = false;
    void CheckLicense()
    {
        onFishing = Game_Manager.current.CheckLicense();
        if (onFishing == false)// 낚시 면허가 없으면 사용 불가
        {
            triggerSetting.GetIconSprite = dontCatchIcon;
        }
        else
        {
            triggerSetting.GetIconSprite = catchIcon;// 트리거 아이콘 설정
        }
    }

    void FishingStart()
    {
        if (onFishing == false)
            return;
        Game_Manager.current.StartFishing();
        triggerSetting.gameObject.SetActive(false);// 트리거 오브젝트 비활성화
    }
}
