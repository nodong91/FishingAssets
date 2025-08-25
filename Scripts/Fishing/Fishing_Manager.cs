using System.Collections;
using UnityEngine;
using static Data_Manager;
using static Data_Quest;

public class Fishing_Manager : FishingTest
{
    [Header(" [ Manager ]")]
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;

    private FishStruct fishStruct;
    private FishStruct.RandomSize randomSize; 
    
    FishStruct[] fishStructs;
    int fishingAmount; // 낚시 횟수

    public GameObject fishInfomation;
    public Custom_Button closeButton;
    public Custom_Button startButton, outButton; // 결과 버튼 (필요시 사용)

    //==================================================================================================================================

    public void SetStart()
    {
        OffCamera();
        deleEndFishing = FishingComplate;
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, false));

        SetComplate();
    }

    public void SetFishingStart(FishStruct[] _fishStructs, int _fishingAmount)
    {
        // 낚시 시작
        fishStructs = _fishStructs;
        fishingAmount = _fishingAmount;
        FishingStart();
    }

    void FishingStart()
    {
        fishingAmount--;
        if (fishingAmount > 0)
        {
            startButton.gameObject.SetActive(true); // 버튼 활성화
        }
        else
        {
            startButton.gameObject.SetActive(false); // 버튼 비활성화
            Debug.LogWarning("낚시 횟수가 0 이하입니다.");
        }

        FishStruct fish = Singleton_Data.INSTANCE.Dict_Fish["Fs_1001"];
        FishingStart(fish);
        SetStart(fishStruct);// 낚시 시작
    }

    public void FishingStart(FishStruct _fishStruct)
    {
        fishStruct = _fishStruct;// 물고기 정보
        randomSize = fishStruct.GetRandom();
        SetStart(_fishStruct);// 낚시 시작

        Option_Manager.current.SetThemeMusic("Battle");
        Game_Manager.current.GetMainUI.OpenCanvas(false);
        Game_Manager.current.OutOfControll(true);

        Game_Manager.current.GetInventory.CloseResult();
    }

    void FishingComplate(bool _comp)
    {
        if (_comp == true)
        {
            Debug.LogError("낚시 성공");
            SetFish();// 물고기 스탯 출력
        }
        else
        {
            Debug.LogError("낚시 실패");
            StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, true));
        }
        Option_Manager.current.SetThemeMusic(null);// 테마 음악 초기화
    }

    void EndFishing()
    {
        Game_Manager.current.GetInventory.CloseResult();
        Game_Manager.current.GetMainUI.OpenCanvas(true);// 메인 UI 다시 열기
        Game_Manager.current.OutOfControll(false);// 게임 컨트롤 가능

        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, false));
        OffCamera();// 카메라 꺼짐
        fishStruct = default;
    }

    //==================================================================================================================================
    // 낚시 
    //==================================================================================================================================

    void SetComplate()
    {
        fishInfomation.gameObject.SetActive(false);
        closeButton.SetButton(CloseButton);
        startButton.SetButton(FishingStart);
        outButton.SetButton(OutButton);
    }

    public void SetFish()// 낚시 성공 후 물고기 정보 설정
    {
        StartCoroutine(SetDisplaying());
    }

    void CloseButton()
    {
        fishInfomation.gameObject.SetActive(false);

        ItemStruct fishItem = fishStruct.itemStruct;
        float size = randomSize.size;

        ResultStruct fishResult = new ResultStruct
        {
            inventorySize = new Vector2Int(7, 7), // 인벤토리 크기
            money = 0, // 돈
            itemID = new string[1] { fishItem.id }, // 아이템 ID
        };

        Game_Manager.current.GetInventory.SetResult(fishResult);// 퀘스트 완료 후 결과 아이템 설정
        Game_Manager.current.GetInventory.OpenResult();
        Game_Manager.current.GetFishGuide.AddFishClass(fishItem.id, size);// 생선 가이드에 추가
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, true));
    }

    void OutButton()
    {
        EndFishing(); // 낚시 완료 후 델리게이트 호출
    }

    IEnumerator SetDisplaying()
    {
        fishInfomation.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f); // 연출 시간???

        closeButton.gameObject.SetActive(true);
    }
}
