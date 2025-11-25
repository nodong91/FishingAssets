using UnityEngine;
using Steamworks;

public class StatsManager : MonoBehaviour
{
    public static StatsManager current;

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
        int catchCount = IntCount("Stats_Catch_Fish");
        Debug.LogWarning($"(물고기 카운트 : {catchCount})");

        if (SteamManager.Initialized)
        {
            if (catchCount >= 1)
            {
                SteamUserStats.GetAchievement("First_Kill", out bool _achieved);
                Debug.LogWarning($"(First_Kill 완료 : {_achieved})");
                if (_achieved == false)// 완료 되지 않은 경우 완료
                {
                    SteamUserStats.SetAchievement("First_Kill");// 완료
                    SteamUserStats.StoreStats();
                }
            }

            if (catchCount >= 10)
            {
                SteamUserStats.GetAchievement("Beginner_Angler", out bool _achieved);
                Debug.LogWarning($"(Beginner_Angler 완료 : {_achieved})");
                if (_achieved == false)// 완료 되지 않은 경우 완료
                {
                    SteamUserStats.SetAchievement("Beginner_Angler");// 완료
                    SteamUserStats.StoreStats();
                }
            }

            if (catchCount >= 50)
            {
                SteamUserStats.GetAchievement("Pro_Angler", out bool _achieved);
                Debug.LogWarning($"(Pro_Angler 완료 : {_achieved})");
                if (_achieved == false)// 완료 되지 않은 경우 완료
                {
                    SteamUserStats.SetAchievement("Pro_Angler");// 완료
                    SteamUserStats.StoreStats();
                }
            }

            if (catchCount >= 100)
            {
                SteamUserStats.GetAchievement("Great_Angler", out bool _achieved);
                Debug.LogWarning($"(Great_Angler 완료 : {_achieved})");
                if (_achieved == false)// 완료 되지 않은 경우 완료
                {
                    SteamUserStats.SetAchievement("Great_Angler");// 완료
                    SteamUserStats.StoreStats();
                }
            }

            if (catchCount >= 100)
            {
                SteamUserStats.GetAchievement("Legendary_Angler", out bool _achieved);
                Debug.LogWarning($"(Legendary_Angler 완료 : {_achieved})");
                if (_achieved == false)// 완료 되지 않은 경우 완료
                {
                    SteamUserStats.SetAchievement("Legendary_Angler");// 완료
                    SteamUserStats.StoreStats();
                }
            }
        }
    }

    public void NightFishing()
    {
        int catchNight = IntCount("Night_Fishing");
        Debug.LogWarning($"(밤 카운트 : {catchNight})");
        if (catchNight >= 1)
        {
            SteamUserStats.GetAchievement("Night_Fishing", out bool _achieved);
            Debug.LogWarning($"(Night_Fishing 완료 : {_achieved})");
            if (_achieved == false)// 완료 되지 않은 경우 완료
            {
                SteamUserStats.SetAchievement("Night_Fishing");// 완료
                SteamUserStats.StoreStats();
            }
        }
    }

    public void GameOver()
    {
        int gameOver = IntCount("Stats_GameOver");
        Debug.LogWarning($"(게임오버 카운트 : {gameOver})");
        if (SteamManager.Initialized)
        {
            if (gameOver >= 1)
            {
                SteamUserStats.GetAchievement("First_Bankruptcy", out bool _achieved);
                Debug.LogWarning($"(First_Bankruptcy 완료 : {_achieved})");
                if (_achieved == false)// 완료 되지 않은 경우 완료
                {
                    SteamUserStats.SetAchievement("First_Bankruptcy");// 완료
                    SteamUserStats.StoreStats();
                }
            }
        }
    }

    public void CatchBox()
    {
        int catchBox = IntCount("Stats_Catch_Box");
        Debug.LogWarning($"(박스 카운트 : {catchBox})");
        if (SteamManager.Initialized)
        {
            if (catchBox >= 1)
            {
                SteamUserStats.GetAchievement("Lucky_Box", out bool _achieved);
                Debug.LogWarning($"(Lucky_Box 완료 : {_achieved})");
                if (_achieved == false)// 완료 되지 않은 경우 완료
                {
                    SteamUserStats.SetAchievement("Lucky_Box");// 완료
                    SteamUserStats.StoreStats();
                }
            }
        }
    }

    int IntCount(string _cord)
    {
        SteamUserStats.GetStat(_cord, out int setCount);// 스탯 가져오기
        setCount++;
        SteamUserStats.SetStat(_cord, setCount);// 스탯 저장하기
        SteamUserStats.StoreStats();
        return setCount;
    }

    public void StatsMoney(int _value)
    {
        SteamUserStats.GetStat("Stats_Money", out int setCount);// 스탯 가져오기
        setCount = _value;
        SteamUserStats.SetStat("Stats_Money", setCount);// 스탯 저장하기
        SteamUserStats.StoreStats();

        Debug.LogWarning($"(머니 카운트 : {setCount})");
        if (SteamManager.Initialized)
        {
            if (setCount >= 10000000)// 천만원
            {
                SteamUserStats.GetAchievement("The_Rich", out bool _achieved);
                Debug.LogWarning($"(The_Rich 완료 : {_achieved})");
                if (_achieved == false)// 완료 되지 않은 경우 완료
                {
                    SteamUserStats.SetAchievement("The_Rich");// 완료
                    SteamUserStats.StoreStats();
                }
            }
        }
    }
}
