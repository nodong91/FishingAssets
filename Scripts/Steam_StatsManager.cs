using Steamworks;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Steam_StatsManager : MonoBehaviour
{
    public static Steam_StatsManager current;

    private void Awake()
    {
        current = this;
    }

    public void ResetStats()
    {
        if (SteamManager.Initialized)
        {
            SteamUserStats.ResetAllStats(true);// 모든 업적 제거
        }
    }

    public void CatchFish()
    {
        int addCount = AddCount("Stats_Catch_Fish", 1);
        Debug.LogWarning($"(물고기 카운트 : {addCount})");

        if (SteamManager.Initialized)
        {
            if (addCount >= 1)
            {
                string achievement = "First_Kill";
                ComplateAchievement(achievement);
            }

            if (addCount >= 10)
            {
                string achievement = "Beginner_Angler";
                ComplateAchievement(achievement);
            }

            if (addCount >= 50)
            {
                string achievement = "Pro_Angler";
                ComplateAchievement(achievement);
            }

            if (addCount >= 100)
            {
                string achievement = "Great_Angler";
                ComplateAchievement(achievement);
            }

            if (addCount >= 1000)
            {
                string achievement = "Legendary_Angler";
                ComplateAchievement(achievement);
            }
        }
    }

    public void CatchLegendary()
    {
        int addCount = AddCount("Stats_Legendary", 1);
        string achievement = "First_Legendary";
        ComplateAchievement(achievement);

        if (addCount >= 6)// 레전더리 등급 개수
        {
            achievement = "Legendary_Collecter";
            ComplateAchievement(achievement);
        }
    }

    public void First_Rare()
    {
        string achievement = "First_Rare";
        ComplateAchievement(achievement);
    }

    public void First_Epic()
    {
        string achievement = "First_Epic";
        ComplateAchievement(achievement);
    }

    public void NightFishing()
    {
        //int addCount = AddCount("Night_Fishing", 1);
        //Debug.LogWarning($"(밤 카운트 : {addCount})");
        //if (addCount >= 1)
        //{
        string achievement = "Night_Fishing";
        ComplateAchievement(achievement);
        //}
    }

    public void GameOver()
    {
        int addCount = AddCount("Stats_GameOver", 1);
        Debug.LogWarning($"(게임오버 카운트 : {addCount})");
        if (SteamManager.Initialized)
        {
            if (addCount >= 1)
            {
                string achievement = "First_Bankruptcy";
                ComplateAchievement(achievement);
            }
        }
    }

    public void CatchBox()
    {
        int addCount = AddCount("Stats_Catch_Box", 1);
        Debug.LogWarning($"(박스 카운트 : {addCount})");
        if (SteamManager.Initialized)
        {
            if (addCount >= 1)
            {
                string achievement = "Lucky_Box";
                ComplateAchievement(achievement);
            }
        }
    }

    public void CountLottery(int _price)
    {
        int addCount = AddCount("Stats_Lottery", 1);
        int priceCount = AddCount("Stats_Lottery_Price", _price);
        Debug.LogWarning($"(복권 당첨 카운트 : {addCount})");
        if (SteamManager.Initialized)
        {
            if (addCount >= 1)
            {
                string achievement = "First_Win";
                ComplateAchievement(achievement);
            }

            if (addCount >= 10)
            {
                string achievement = "Winning_Machine";
                ComplateAchievement(achievement);
            }

            if (addCount >= 100)
            {
                string achievement = "Lottery_Addict";
                ComplateAchievement(achievement);
            }

            if (priceCount >= 10000)
            {
                string achievement = "Winnings_Killer";
                ComplateAchievement(achievement);
            }

            if (priceCount >= 1000000)
            {
                string achievement = "Legendary_Winner";
                ComplateAchievement(achievement);
            }
        }
    }

    public void StatsMoney(int _value)
    {
        int setCount = SetCount("Stats_Money", _value);
        Debug.LogWarning($"(머니 카운트 : {setCount})");
        if (SteamManager.Initialized)
        {
            if (setCount >= 10000000)// 천만원
            {
                string achievement = "The_Rich";
                ComplateAchievement(achievement);
            }
        }
    }

    //==========================================================================================================================
    // 카운트
    //==========================================================================================================================

    int AddCount(string _cord, int _value)
    {
        SteamUserStats.GetStat(_cord, out int setCount);// 스탯 가져오기
        setCount += _value;
        SteamUserStats.SetStat(_cord, setCount);// 스탯 저장하기
        SteamUserStats.StoreStats();
        return setCount;
    }

    int SetCount(string _cord, int _value)
    {
        SteamUserStats.GetStat(_cord, out int setCount);// 스탯 가져오기
        setCount = _value;
        SteamUserStats.SetStat(_cord, setCount);// 스탯 저장하기
        SteamUserStats.StoreStats();
        return setCount;
    }

    //==========================================================================================================================
    // 완료
    //==========================================================================================================================

    void ComplateAchievement(string _achievement)
    {
        SteamUserStats.GetAchievement(_achievement, out bool _achieved);
        Debug.LogWarning($"({_achievement} 완료 : {_achieved})");
        if (_achieved == false)// 완료 되지 않은 경우 완료
        {
            SteamUserStats.SetAchievement(_achievement);// 완료
            SteamUserStats.StoreStats();
        }
    }
}
