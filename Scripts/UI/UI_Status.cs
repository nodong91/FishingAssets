using UnityEngine;
using static Data_Manager;

public class UI_Status : MonoBehaviour
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;

    [Header("[ 스테이터스 ]")]
    public SetStatus totalStatus => Game_Manager.current.currentStatus;
   
    private void Start()
    {
        OpenCanvas(false);
    }

    public void OpenCanvas(bool _open)
    {
        if (_open)
            SetStatusText();
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
    }


    [Header(" [ Catch ]")]
    public TMPro.TMP_Text catchRadius;// 물고기를 잡는 범위
    public TMPro.TMP_Text catchSpeed;// 낚시대가 물고기를 향해 이동하는 속도
    public TMPro.TMP_Text catchPower;// 낚시대의 힘
    public TMPro.TMP_Text catchMaxHealth;// 낚시대의 최대 체력
    public TMPro.TMP_Text catchAttakSpeed;// 물고기를 공격하는 빈도

    [Header(" [ Ship ]")]
    public TMPro.TMP_Text shipSpeed;// 배의 이동 속도
    public TMPro.TMP_Text maxWeight;// 인벤토리 중량
    public TMPro.TMP_Text maxEnergy;// 연료통 크기
    public TMPro.TMP_Text efficient;// 에너지 효율
    public TMPro.TMP_Text maxBoxSize;// 인벤토리 크기
    public TMPro.TMP_Text shipHealth;// 배 체력
    public TMPro.TMP_Text freshness;// 신선도 유지 - 꼭 필요한가??????  

    [Header(" [ Fish ]")]
    public TMPro.TMP_Text LuckFish;// 희귀 물고기 확률
    public TMPro.TMP_Text FishAmount;// 낚시 횟수 증가
    public TMPro.TMP_Text FishPrice;// 판매 물고기 가격 증가
    void SetStatusText()
    {
        catchRadius.text = totalStatus.catchRadius.ToString();
        catchSpeed.text = totalStatus.catchSpeed.ToString();
        catchPower.text = totalStatus.catchPower.ToString();
        catchMaxHealth.text = totalStatus.catchMaxHealth.ToString();
        catchAttakSpeed.text = totalStatus.catchAttakSpeed.ToString();

        shipSpeed.text = totalStatus.shipSpeed.ToString();
        maxWeight.text = totalStatus.maxWeight.ToString();
        maxEnergy.text = totalStatus.maxEnergy.ToString();
        efficient.text = totalStatus.efficient.ToString();
        maxBoxSize.text = totalStatus.maxBoxSize.ToString();
        shipHealth.text = totalStatus.shipHealth.ToString();
        freshness.text = totalStatus.freshness.ToString();

        LuckFish.text = totalStatus.luckFish.ToString();
        FishAmount.text = totalStatus.fishAmount.ToString();
        FishPrice.text = totalStatus.fishPrice.ToString();
    }

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    OpenCanvas(false);
    //}
}
