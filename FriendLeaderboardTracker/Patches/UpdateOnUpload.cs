using HarmonyLib;
using Steamworks;
using UnityEngine;

[HarmonyPatch(typeof(LeaderboardIntegrationSteam))]
static class UpdateOnUpload
{
    static string lastLevelUploaded;

    [HarmonyPatch("UploadScore_Internal"), HarmonyPostfix]
    static void OnUploadStart(string boardName)
    {
        lastLevelUploaded = boardName;
    }

    [HarmonyPatch("OnLeaderboardScoreUploaded2"), HarmonyPostfix]
    static void OnUploadFinish(LeaderboardScoreUploaded_t pCallback)
    {

        foreach (var missionLvlPair in Leaderboard.levelTimeDiffs)
        {
            if (missionLvlPair.Value.ContainsKey(Leaderboard.LEVEL_NAMES[lastLevelUploaded]))
            {
                Leaderboard.currMission = missionLvlPair.Key;
                Leaderboard.currLevel = Leaderboard.LEVEL_NAMES[lastLevelUploaded];
            }
        }

        Leaderboard.GetLBData(lastLevelUploaded);
    }
}