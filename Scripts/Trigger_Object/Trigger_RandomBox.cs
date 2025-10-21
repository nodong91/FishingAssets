using UnityEngine;
using static Data_Manager;

public class Trigger_RandomBox : Trigger_Setting
{
    public AreaType areaType;
    public Sprite iconImage;

    public void SetAreaType(AreaType _areaType)
    {
        areaType = _areaType;
        deleTriggerAction = SetTrigger;
        GetIconSprite = iconImage;// 트리거 아이콘 설정
    }

    void SetTrigger()
    {

        gameObject.SetActive(false);// 트리거 오브젝트 비활성화
    }
}
