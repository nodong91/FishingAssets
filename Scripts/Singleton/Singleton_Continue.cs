using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Data_Manager;

public class Singleton_Continue : MonoSingleton<Singleton_Continue>
{
    //===========================================================================================================================
    // 저장 및 불러오기
    //===========================================================================================================================
    public void SaveContinue()
    {
        if (Game_Manager.current.shipData == null)
            return;

        StartCoroutine(SetContinue());
    }

    IEnumerator SetContinue()
    {
        Debug.LogError("저장 : 컨티뉴");
        yield return null;

        Data_Continue continueData = new Data_Continue
        {
            shipData = Game_Manager.current.shipData.id,
            playerPosition = Game_Manager.current.GetPlayer.transform.position,
            playerRotation = Game_Manager.current.GetPlayer.transform.rotation,
            playerScale = Game_Manager.current.GetPlayer.transform.localScale,

            timeSpeed = Game_Manager.current.GetTimeUI.timeSpeed,
            minute = Game_Manager.current.GetTimeUI.minute,
            hour = Game_Manager.current.GetTimeUI.hour,
            day = Game_Manager.current.GetTimeUI.day,
            weatherType = Game_Manager.current.GetTimeUI.weatherType,

            energy = Game_Manager.current.GetPlayer.GetEnergy,// 에너지 어떻게 해야하나~
            money = Game_Manager.current.GetMainUI.TryMoney,
            destroySlot = Game_Manager.current.GetInventory.TryDestroySlot,

            loanActive = Game_Manager.current.GetMainUI.timeUI.loanActive,
            loanTime = Game_Manager.current.GetMainUI.timeUI.loanTime,
        };
        Static_JsonManager.SaveCountinueData(Const_Save._continue, continueData);
    }

    public Data_Continue LoadContinue()
    {
        if (Static_JsonManager.TryLoadCountinueData(Const_Save._continue, out Data_Continue _data))
        {
            return _data;
        }

        Data_Continue continueData = new Data_Continue()
        {
            shipData = "",

            playerPosition = new Vector3(0.5f, 0.2f, 10.7f),// 초기 위치
            playerRotation = Quaternion.Euler(-0.15f, 103f, 0.02f),
            playerScale = Vector3.one,

            timeSpeed = 10f,
            minute = 3f,
            hour = 7,
            day = 0,
            weatherType = UI_Time.WeatherType.Sun,

            energy = 0,
            money = 1000f,
            destroySlot = new List<Vector2Int>(),

            loanActive = false,
            loanTime = 0,
        };
        // 기본 세팅
        return continueData;
    }
}
