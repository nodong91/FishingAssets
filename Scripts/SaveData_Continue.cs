using UnityEngine;
using static Data_Manager;

public class SaveData_Continue : MonoBehaviour
{
    public Data_Continue continueData;
    public string saveData = "SaveContinue";

    public static SaveData_Continue current;

    private void Awake()
    {
        current = this;
    }

    //===========================================================================================================================
    // 저장 및 불러오기
    //===========================================================================================================================
    public void SetContinue()
    {
        continueData = new Data_Continue
        {
            playerPosition = Game_Manager.current.GetPlayer.transform.position,
            playerRotation = Game_Manager.current.GetPlayer.transform.rotation,
            playerScale = Game_Manager.current.GetPlayer.transform.localScale,

            timeSpeed = Game_Manager.current.GetTimeUI.timeSpeed,
            minute = Game_Manager.current.GetTimeUI.minute,
            hour = Game_Manager.current.GetTimeUI.hour,
            day = Game_Manager.current.GetTimeUI.day,
            weatherType = Game_Manager.current.GetTimeUI.weatherType,

            energy = Game_Manager.current.GetInventory.TryEnergy,
            money = Game_Manager.current.GetInventory.TryMoney,

            destroySlot = Game_Manager.current.GetInventory.TryDestroySlot,
        };
        SaveContinue();
    }

    public void GetContinue()
    {
        LoadContinue();
        if (continueData == null)
            return;
        // 위치
        Game_Manager.current.GetPlayer.transform.position = continueData.playerPosition;
        Game_Manager.current.GetPlayer.transform.rotation = continueData.playerRotation;
        Game_Manager.current.GetPlayer.transform.localScale = continueData.playerScale;

        float timeSpeed = continueData.timeSpeed;
        float minute = continueData.minute;
        int hour = continueData.hour;
        int day = continueData.day;
        Game_Manager.current.GetTimeUI.SetStart(timeSpeed, minute, hour, day);// 시간

        Game_Manager.current.GetInventory.TryMoney = continueData.money;// 돈
        Game_Manager.current.GetInventory.TryDestroySlot = continueData.destroySlot;// 부서진 슬롯
    }

    void SaveContinue()
    {
        Static_JsonManager.SaveCountinueData(saveData, continueData);
    }

    void LoadContinue()
    {
        if (Static_JsonManager.TryLoadCountinueData(saveData, out Data_Continue _data))
        {
            continueData = _data;
        }
        else
        {
            continueData = new Data_Continue
            {
                playerPosition = new Vector3(0f, 0f, 15f),// 초기 위치
                playerRotation = Quaternion.identity,
                playerScale = Vector3.one,

                timeSpeed = 10f,
                minute = 30f,
                hour = 7,
                day = 0,
                weatherType = UI_Time.WeatherType.Sun,

                energy = 0f,
                money = 0f,
            };
        }
    }

}
