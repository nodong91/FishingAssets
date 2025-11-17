using UnityEngine;
using Steamworks;

public class StatsManager : MonoBehaviour
{
    public int catchCount = 0;
    public static StatsManager current;

    private void Awake()
    {
        current = this;
    }

    public void CatchFish()
    {
        SteamUserStats.GetStat("Stats_Catch_Fish", out catchCount);// 스탯 가져오기
        catchCount++;
        SteamUserStats.SetStat("Stats_Catch_Fish", catchCount);// 스탯 저장하기
        SteamUserStats.StoreStats();

        if (SteamManager.Initialized)
        {
            if (catchCount >= 1)
            {
                SteamUserStats.GetAchievement("First_Kill", out bool _achieved);
                Debug.LogWarning($"(카운트 : {catchCount} First_Kill 완료 : {_achieved})");
                if (_achieved == false)// 완료 되지 않은 경우 완료
                {
                    SteamUserStats.SetAchievement("First_Kill");// 완료
                    SteamUserStats.StoreStats();
                }
            }

            if (catchCount >= 10)
            {
                SteamUserStats.GetAchievement("Beginner_Angler", out bool _achieved);
                if (_achieved == false)// 완료 되지 않은 경우 완료
                {
                    SteamUserStats.SetAchievement("Beginner_Angler");// 완료
                    SteamUserStats.StoreStats();
                }
            }
        }
    }
}
