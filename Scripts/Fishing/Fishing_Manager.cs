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
    public Data_Manager.FishStruct.FishType catchFishType;
    public Fishing_Hit fishingHit;
    public Fishing_Main fishingMain;
    public Fishing_Sub_Strength fishingSubStrength;

    void Start()
    {
        SetMouse();
        StateMachine(FishingState.Hit);
    }

    void SetMouse()
    {
        Singleton_Controller.INSTANCE.key_MouseLeft += InputMouseLeft;
        Singleton_Controller.INSTANCE.key_MouseRight += InputMouseRight;
    }

    void EndGame()
    {
        if (state < FishingState.Complate)
        {
            StateMachine(state + 1);
        }
        else
        {
            StateMachine(FishingState.Ready);
        }
    }

    void StateMachine(FishingState _state)
    {
        state = _state;
        switch (state)
        {
            case FishingState.Ready:

                break;

            case FishingState.Hit:
                fishingHit.deleEndGame = EndGame;
                fishingHit.StartGame();
                break;

            case FishingState.Main:
                fishingMain.deleEndGame = EndGame;
                fishingMain.StartGame();
                break;

            case FishingState.Sub:
                StateSub();
                break;

            case FishingState.Complate:

                break;
        }
    }

    void StateSub()
    {
        switch (catchFishType)
        {
            case Data_Manager.FishStruct.FishType.Strength:
                fishingSubStrength.deleEndGame = EndGame;
                fishingSubStrength.StartGame();
                break;
            case Data_Manager.FishStruct.FishType.Agility:
                break;
            case Data_Manager.FishStruct.FishType.Health:
                break;
        }
    }

    //==================================================================================================================================
    // 왼쪽 액션
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

                break;
        }
    }
    void StateSubAction(bool _left)
    {
        switch (catchFishType)
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
                break;
            case Data_Manager.FishStruct.FishType.Health:
                break;
        }
    }

    //==================================================================================================================================
    // 오른쪽 액션
    //==================================================================================================================================
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
}
