using UnityEngine;
using static Data_Manager;

public class Singleton_Continue : MonoSingleton<Singleton_Continue>
{
    public Data_Continue continueData;
    const string saveData = "SaveContinue";

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

            health = Game_Manager.current.GetPlayer.GetHealth,
            energy = Game_Manager.current.GetPlayer.GetEnergy,
            money = Game_Manager.current.GetMainUI.TryMoney,
            destroySlot = Game_Manager.current.GetInventory.TryDestroySlot,
        };
        SaveContinue();
    }

    public void GetContinue()
    {
        continueData = LoadContinue();
        //if (continueData == null)
        //    return;

        Game_Manager.current.GetTimeUI.SetStart(continueData);// 시간
        Game_Manager.current.GetMainUI.SetMoney(continueData.money);// 돈
        Game_Manager.current.GetInventory.TryDestroySlot = continueData.destroySlot;// 부서진 슬롯
    }

    void SaveContinue()
    {
        Static_JsonManager.SaveCountinueData(saveData, continueData);
    }

    public Data_Continue LoadContinue()
    {
        if (Static_JsonManager.TryLoadCountinueData(saveData, out Data_Continue _data))
        {
            return _data;
        }
        else
        {
            Data_Status_Default defaultStatusData = Game_Manager.current.defaultStatusData;
            Vector3 defaultPosition = new Vector3(0.5f, 0.2f, 10.7f);
            Quaternion defaultRotate = Quaternion.Euler(-0.15f, 103f, 0.02f);
            Data_Continue data = new Data_Continue
            {
                playerPosition = defaultPosition,// 초기 위치
                playerRotation = defaultRotate,
                playerScale = Vector3.one,

                timeSpeed = 15f,
                minute = 30f,
                hour = 7,
                day = 0,
                weatherType = UI_Time.WeatherType.Sun,

                health = defaultStatusData.defaultStatus.shipHealth,
                energy = defaultStatusData.defaultStatus.maxEnergy,
                money = 0f,
            };
            return data;
        }
    }
}
