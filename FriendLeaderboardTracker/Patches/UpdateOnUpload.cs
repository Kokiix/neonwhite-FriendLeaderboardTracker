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
        foreach (var missionLvlPair in FetchLeaderboards.levelTimeDiffs)
        {
            if (missionLvlPair.Value.ContainsKey(FetchLeaderboards.LEVEL_NAMES[lastLevelUploaded]))
            {
                FetchLeaderboards.currMission = missionLvlPair.Key;
                FetchLeaderboards.currLevel = FetchLeaderboards.LEVEL_NAMES[lastLevelUploaded];

                var prevTime = FetchLeaderboards.levelTimeDiffs[FetchLeaderboards.currMission][FetchLeaderboards.currLevel];
                if (prevTime != "Not Played")
                {
                    FetchLeaderboards.playedMaps--;
                    if (prevTime.StartsWith("-"))
                    {
                        FetchLeaderboards.totalWins--;
                    }
                }

                FetchLeaderboards.FetchSingleLB(lastLevelUploaded);
                return;
            }
        }

    }
}