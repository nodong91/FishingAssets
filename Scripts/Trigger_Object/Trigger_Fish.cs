using UnityEngine;
using static Data_Manager;

public class Trigger_Fish : MonoBehaviour
{
    public Trigger_Setting triggerSetting;
    public AreaType areaType;
    public Sprite catchIcon;

    void Start()
    {
        triggerSetting.deleTriggerAction = FishingStart;// 낚시 게임 스타트
        triggerSetting.GetIconSprite = catchIcon;// 트리거 아이콘 설정
    }

    public void SetAreaType(AreaType _areaType)
    {
        areaType = _areaType;
    }

    void FishingStart()
    {
        Debug.LogWarning("낚시 시작");
        Game_Manager.current.StartFishing(areaType);
        triggerSetting.gameObject.SetActive(false);// 트리거 오브젝트 비활성화
    }
}
