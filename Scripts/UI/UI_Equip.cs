using UnityEngine;

public class UI_Equip : MonoBehaviour
{
    public GameObject boat;// 배 기본 스탯 (속도, 방어도, 인벤토리 중량)
    public GameObject engine;// 이동 관련 스탯 (속도, 연료통 크기)
    public GameObject inventoryBox;// 운송 관련 스탯 (인벤토리 크기, 신선도 유지)

    public Data_Manager.LodStruct fishingLod;// 낚시 관련 스탯
    public GameObject bait;// 미끼 (해당 물고기 )

    public void OpenCanvas()
    {

    }
}
