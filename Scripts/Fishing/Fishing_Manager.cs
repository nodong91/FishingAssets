using System.Collections;
using UnityEngine;

public class Fishing_Manager : MonoBehaviour
{
    public enum FishingState
    {
        Ready,
        Hit,
        Main,
        Sub,
        Complate
    }
    public FishingState state;
    Fishing_Setting fishSetting;
    public Data_Manager.FishStruct.FishType fishType;

    public Fishing_Hit fishingHit;
    public Fishing_Main fishingMain;
    public Fishing_Sub_Strength fishingSubStrength;
    public Fishing_Sub_Agility fishingSubAgility;
    public Fishing_Sub_Health fishingSubHealth;
    public Fishing_Complate fishingComplate;

    // 순서
    // 히트 - 파이트 - 릴링 - 파이트 - 릴링 - 물고기 체력방전 시 캐치, 줄이 못버티면 놓침
    // 파이트 - 성공 (물고기 체력), 실패 (줄 타격)
    // 릴링 - 지속적으로 물고기 체력 타격
    // 릴링 시 물고기가 공격할 때(색이 변하던가 해서 알려줘야) 영역안에 들어가 있으면 줄 타격 (너무 영역이 크면 빠져나가기 힘들게)

    void Start()
    {
        fishingHit.SetStart();
        fishingMain.SetStart();
        fishingSubStrength.SetStart();
        fishingSubAgility.SetStart();
        fishingSubHealth.SetStart();
    }

    public void StartGame(Fishing_Setting _fishSetting)
    {
        fishSetting = _fishSetting;
        if (fishSetting != null)
        {
            fishType = fishSetting.fishType;
            SetMouse();
            StateMachine(FishingState.Hit);
        }
    }

    void EndGame(FishingState _fishState)
    {
        StateMachine(_fishState);
    }

    void SetMouse()
    {
        Singleton_Controller.INSTANCE.key_MouseLeft += InputMouseLeft;
        Singleton_Controller.INSTANCE.key_MouseRight += InputMouseRight;
    }

    void RemoveMouse()
    {
        Singleton_Controller.INSTANCE.key_MouseLeft -= InputMouseLeft;
        Singleton_Controller.INSTANCE.key_MouseRight -= InputMouseRight;
    }

    void StateMachine(FishingState _state)
    {
        state = _state;
        switch (state)
        {
            case FishingState.Ready:
                StateReady();
                break;

            case FishingState.Hit:
                fishingHit.deleEndGame = EndGame;
                fishingHit.StartGame();
                break;

            case FishingState.Main:
                fishingMain.deleEndGame = EndGame;
                fishingMain.StartGame(fishSetting);
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
        fishSetting = null;
        Game_Manager.current.player.SetControll();
        RemoveMouse();
    }

    void StateSub()
    {
        switch (fishSetting.fishType)
        {
            case Data_Manager.FishStruct.FishType.Strength:
                fishingSubStrength.deleEndGame = EndGame;
                fishingSubStrength.StartGame();
                break;

            case Data_Manager.FishStruct.FishType.Agility:
                fishingSubAgility.deleEndGame = EndGame;
                fishingSubAgility.StartGame();
                break;

            case Data_Manager.FishStruct.FishType.Health:
                fishingSubHealth.deleEndGame = EndGame;
                fishingSubHealth.StartGame();
                break;
        }
    }

    void StateComplate()
    {
        fishingComplate.SetFish(fishSetting);// 물고기 스탯 출력
        StateMachine(FishingState.Ready);
    }

    //==================================================================================================================================
    // 액션
    //==================================================================================================================================
    void InputMouseLeft(bool _input)
    {
        if (_input)
        {
            StateAction(true);
        }
        else
        {

        }
    }

    void InputMouseRight(bool _input)
    {
        if (_input)
        {
            StateAction(false);
        }
        else
        {

        }
    }

    void StateAction(bool _left)
    {
        switch (state)
        {
            case FishingState.Ready:

                break;

            case FishingState.Hit:
                fishingHit.Action();
                break;

            case FishingState.Main:
                fishingMain.Action();
                break;

            case FishingState.Sub:
                StateSubAction(_left);
                break;

            case FishingState.Complate:
                StateComplateAction();
                break;
        }
    }

    void StateSubAction(bool _left)
    {
        switch (fishSetting.fishType)
        {
            case Data_Manager.FishStruct.FishType.Strength:
                if (_left == true)
                {
                    fishingSubStrength.Action_Left();
                }
                else
                {
                    fishingSubStrength.Action_Right();
                }
                break;

            case Data_Manager.FishStruct.FishType.Agility:
                if (_left == true)
                {
                    fishingSubAgility.Action_Left();
                }
                else
                {
                    fishingSubAgility.Action_Right();
                }
                break;

            case Data_Manager.FishStruct.FishType.Health:
                if (_left == true)
                {
                    fishingSubHealth.Action_Left();
                }
                else
                {
                    fishingSubHealth.Action_Right();
                }
                break;
        }
    }

    void StateComplateAction()
    {
        //StateMachine(FishingState.Ready);
    }
}
