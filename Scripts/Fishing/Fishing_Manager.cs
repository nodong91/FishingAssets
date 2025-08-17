using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;
using static Data_Quest;
//using static Data_Manager;

public class Fishing_Manager : MonoBehaviour
{
    public FishingTest fishingTest;
    //public Fishing_Camera fishingCamera;
    public enum FishingState
    {
        Ready,
        Hit,
        Main,
        Sub,
        Complate
    }
    public FishingState state;
    public FishStruct fishStruct;
    public FishStruct.RandomSize randomSize;

    public delegate void DeleInputMouse(bool _input);
    public DeleInputMouse inputMouseLeft;
    public DeleInputMouse inputMouseRight;
    // 순서
    // 히트 - 파이트 - 릴링 - 파이트 - 릴링 - 물고기 체력방전 시 캐치, 줄이 못버티면 놓침
    // 파이트 - 성공 (물고기 체력), 실패 (줄 타격)
    // 릴링 - 지속적으로 물고기 체력 타격
    // 릴링 시 물고기가 공격할 때(색이 변하던가 해서 알려줘야) 영역안에 들어가 있으면 줄 타격 (너무 영역이 크면 빠져나가기 힘들게)

    public void SetStart()
    {
        fishingTest.OffCamera();
        fishingTest.deleEndFishing = FishingComplate;

        SetComplate();
    }

    public void FishingStart(FishStruct _fishStruct)
    {
        Game_Manager.current.GetMainUI.OpenCanvas(false);

        Option_Manager.current.SetThemeMusic("Battle");
        Game_Manager.current.OutOfControll(true);

        Transform player = Game_Manager.current.player.transform;
        fishingTest.transform.position = player.position;
        fishingTest.transform.rotation = player.rotation;

        fishingTest.SetCamera();

        fishStruct = _fishStruct;// 잡힌 물고기
        randomSize = fishStruct.GetRandom();
    }

    void FishingComplate(bool _comp)
    {
        if (_comp == true)
        {
            Debug.LogError("낚시 성공");
            SetFish(fishStruct, randomSize);// 물고기 스탯 출력
        }
        else
        {
            Debug.LogError("낚시 실패");
            EndFishing();
        }
        Option_Manager.current.SetThemeMusic(null);// 테마 음악 초기화
    }

    void EndFishing()
    {
        Game_Manager.current.GetInventory.CloseResult();
        Game_Manager.current.GetMainUI.OpenCanvas(true);// 메인 UI 다시 열기

        inputMouseLeft = null;
        inputMouseRight = null;

        Game_Manager.current.OutOfControll(false);// 게임 컨트롤 가능
        fishingTest.OffCamera();// 카메라 꺼짐
        fishStruct = default;
    }

    //==================================================================================================================================
    // 낚시 
    //==================================================================================================================================

    public GameObject fishInfomation;
    public Button closeButton;
    public Button resultButton; // 결과 버튼 (필요시 사용)

    void SetComplate()
    {
        fishInfomation.gameObject.SetActive(false);
        closeButton.onClick.AddListener(CloseButton);
        resultButton.onClick.AddListener(ResultButton);
    }

    public void SetFish(FishStruct _fishStruct, FishStruct.RandomSize _randomSize)
    {
        fishStruct = _fishStruct;
        randomSize = _randomSize;
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
    }
    //public Action deleEndFishing;
    void ResultButton()
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
