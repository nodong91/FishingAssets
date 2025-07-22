using System.Collections;
using UnityEngine;

public class Fishing_Manager : MonoBehaviour
{
    public Fishing_Camera fishingCamera;
    public enum FishingState
    {
        Ready,
        Hit,
        Main,
        Sub,
        Complate
    }
    public FishingState state;
    public Data_Manager.FishStruct fishStruct;
    public Data_Manager.FishStruct.RandomSize randomSize;

    public Fishing_Hit fishingHit;
    public Fishing_Main fishingMain;
    public Fishing_Sub_Strength fishingSubStrength;
    public Fishing_Sub_Agility fishingSubAgility;
    public Fishing_Sub_Health fishingSubHealth;
    public Fishing_Complate fishingComplate;

    public delegate void DeleInputMouse(bool _input);
    public DeleInputMouse inputMouseLeft;
    public DeleInputMouse inputMouseRight;
    // 순서
    // 히트 - 파이트 - 릴링 - 파이트 - 릴링 - 물고기 체력방전 시 캐치, 줄이 못버티면 놓침
    // 파이트 - 성공 (물고기 체력), 실패 (줄 타격)
    // 릴링 - 지속적으로 물고기 체력 타격
    // 릴링 시 물고기가 공격할 때(색이 변하던가 해서 알려줘야) 영역안에 들어가 있으면 줄 타격 (너무 영역이 크면 빠져나가기 힘들게)

    void Start()
    {
        fishingCamera.OffCamera();

        fishingHit.SetStart();
        fishingMain.SetStart();
        fishingSubStrength.SetStart();
        fishingSubAgility.SetStart();
        fishingSubHealth.SetStart();
    }

    public void StartGame(Data_Manager.FishStruct _fishStruct)
    {
        Game_Manager.current.OutOfControll(true);

        Transform player = Game_Manager.current.player.transform;
        fishingCamera.transform.position = player.position;
        fishingCamera.transform.rotation = player.rotation;

        fishingCamera.SetCamera();

        fishStruct = _fishStruct;
        randomSize = fishStruct.GetRandom();

        StateMachine(FishingState.Hit);
    }

    void EndGame(FishingState _fishState)
    {
        StateMachine(_fishState);
        Debug.LogWarning(_fishState);
    }

    private void Update()
    {
        if (state != FishingState.Ready)
        {
            if (Input.GetMouseButtonDown(0))
            {
                InputMouseLeft(true);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                InputMouseLeft(false);
            }
            if (Input.GetMouseButtonDown(1))
            {
                InputMouseRight(true);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                InputMouseRight(false);
            }
        }
    }

    //==================================================================================================================================
    // 액션
    //==================================================================================================================================
    void InputMouseLeft(bool _input)
    {
        inputMouseLeft?.Invoke(_input);
    }

    void InputMouseRight(bool _input)
    {
        inputMouseRight?.Invoke(_input);
    }

    void StateMachine(FishingState _state)
    {
        state = _state;
        switch (state)
        {
            case FishingState.Ready:
                inputMouseLeft = null;
                inputMouseRight = null;
                StateReady();
                break;

            case FishingState.Hit:
                inputMouseLeft = fishingHit.InputMouseLeft;
                inputMouseRight = fishingHit.InputMouseRight;

                fishingHit.deleEndGame = EndGame;
                fishingHit.StartGame();
                break;

            case FishingState.Main:
                inputMouseLeft = fishingMain.InputMouseLeft;
                inputMouseRight = fishingMain.InputMouseRight;

                fishingMain.deleEndGame = EndGame;
                fishingMain.StartGame(fishStruct);
                break;

            case FishingState.Sub:
                StateSub();
                break;

            case FishingState.Complate:
                StateComplate();
                break;
        }
    }
    void StateReady()
    {
        Game_Manager.current.OutOfControll(false);
        fishingCamera.OffCamera();
        fishStruct = default;
    }

    void StateSub()
    {
        switch (fishStruct.fishType)
        {
            case Data_Manager.FishStruct.FishType.Strength:
                inputMouseLeft = fishingSubStrength.InputMouseLeft;
                inputMouseRight = fishingSubStrength.InputMouseRight;

                fishingSubStrength.deleEndGame = EndGame;
                fishingSubStrength.StartGame();
                break;

            case Data_Manager.FishStruct.FishType.Agility:
                inputMouseLeft = fishingSubAgility.InputMouseLeft;
                inputMouseRight = fishingSubAgility.InputMouseRight;

                fishingSubAgility.deleEndGame = EndGame;
                fishingSubAgility.StartGame();
                break;

            case Data_Manager.FishStruct.FishType.Health:
                inputMouseLeft = fishingSubHealth.InputMouseLeft;
                inputMouseRight = fishingSubHealth.InputMouseRight;

                fishingSubHealth.deleEndGame = EndGame;
                fishingSubHealth.StartGame();
                break;
        }
    }

    void StateComplate()
    {
        fishingComplate.SetFish(fishStruct, randomSize);// 물고기 스탯 출력
        StateMachine(FishingState.Ready);
    }
}
