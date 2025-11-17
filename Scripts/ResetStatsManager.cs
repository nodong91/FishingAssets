using UnityEngine;

public class ResetStatsManager : MonoBehaviour
{
    private bool ResetStatsOnGameStart = false;
    private bool AlsoResetAchievements = false;

    void Start()
    {
        if (SteamManager.Initialized)
        {
            if (ResetStatsOnGameStart == true)
            {
                Steamworks.SteamUserStats.ResetAllStats(AlsoResetAchievements);// 모든 업적 제거
            }
        }
    }
}
