using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

static class Leaderboard
{
    internal static bool completedFetch = false;
    internal static bool fetchInProgress = false;
    internal static void FetchAllLBs(Scene s, LoadSceneMode mode)
    {
        if (s.name == "Heaven_Environment")
        {
            if (!completedFetch && !fetchInProgress)
                FetchAllLBs();
        }
        else if (FriendLeaderboardTrackerPlugin.Instance.TryGetComponent(out LBTrackerGUI gui))
            GameObject.Destroy(gui);
    }
    internal static void FetchAllLBs()
    {
        FriendLeaderboardTrackerPlugin.Instance.StartCoroutine(FetchAllRoutine());
    }

    internal static Dictionary<string, Dictionary<string, string>> levelTimeDiffs = [];

    static string _currMission = "";
    static string _currLevel = "";

    // I couldn't find a way to access these names from the source code TwT
    static readonly Dictionary<string, string> MISSION_NAMES = new()
    {
        {"Interface/MISSION_01_TITLE", "Rebirth"},
        {"Interface/MISSION_02_TITLE", "Killer Inside"},
        {"Interface/MISSION_03_TITLE", "Only Shallow"},
        {"Interface/MISSION_04_TITLE", "The Old City"},
        {"Interface/MISSION_05_TITLE", "The Burn that Cures"},
        {"Interface/MISSION_06_TITLE", "Covenant"},
        {"Interface/MISSION_07_TITLE", "Reckoning"},
        {"Interface/MISSION_08_TITLE", "Benediction"},
        {"Interface/MISSION_09_TITLE", "Aprocrypha"},
    };

    static IEnumerator FetchAllRoutine()
    {
        completedFetch = false;
        fetchInProgress = true;

        var data = Singleton<Game>.Instance.GetGameData();
        foreach (var campaign in data.campaigns)
        {
            foreach (var mission in campaign.missionData)
            {
                if (MISSION_NAMES.ContainsKey(mission.missionDisplayName))
                    _currMission = MISSION_NAMES[mission.missionDisplayName];
                else
                    _currMission = mission.missionDisplayName;
                levelTimeDiffs[_currMission] = [];

                foreach (var level in mission.levels)
                {
                    _currLevel = level.levelDisplayName;
                    GetLBData(level.levelID);
                    yield return new WaitWhile(() => FetchInProgress);
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

        completedFetch = true;
        fetchInProgress = false;
        LBTrackerGUI.GenerateTimeString();
    }

    static CallResult<LeaderboardFindResult_t> _lbFindResult = CallResult<LeaderboardFindResult_t>.Create(OnLBFound);
    static CallResult<LeaderboardScoresDownloaded_t> _lbDownloadResult = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLBDownloaded);
    static bool FetchInProgress = true;

    static void GetLBData(string levelID)
    {
        FetchInProgress = true;
        _lbFindResult.Set(SteamUserStats.FindLeaderboard(levelID));
    }

    static void OnLBFound(LeaderboardFindResult_t pCallback, bool bIOFailure)
    {
        _lbDownloadResult.Set(SteamUserStats.DownloadLeaderboardEntries(
            pCallback.m_hSteamLeaderboard,
            ELeaderboardDataRequest.k_ELeaderboardDataRequestFriends,
            nRangeStart: 1,
            nRangeEnd: 10000));
    }

    static void OnLBDownloaded(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)
    {
        if (pCallback.m_cEntryCount == 0)
        {
            levelTimeDiffs[_currMission][_currLevel] = "Not Played";
            FetchInProgress = false;
            return;
        }

        var details = new int[1];
        string diff = "Not Played";

        SteamUserStats.GetDownloadedLeaderboardEntry(
            pCallback.m_hSteamLeaderboardEntries,
            0,
            out var firstPlace,
            details,
            cDetailsMax: 1);

        if (firstPlace.m_steamIDUser == SteamUser.GetSteamID())
        {
            if (pCallback.m_cEntryCount >= 2)
            {
                SteamUserStats.GetDownloadedLeaderboardEntry(
                    pCallback.m_hSteamLeaderboardEntries,
                    1,
                    out var secondPlace,
                    details,
                    cDetailsMax: 1);
                var timeDiff = secondPlace.m_nScore - firstPlace.m_nScore;
                diff = "-" + timeDiff.ToString().PadLeft(5, '0').Insert(2, ".");
            }
            else
            {
                diff = "You are the only player";
            }
        }
        else
        {
            for (int i = 1; i < pCallback.m_cEntryCount; i++)
            {
                SteamUserStats.GetDownloadedLeaderboardEntry(
                    pCallback.m_hSteamLeaderboardEntries,
                    i,
                    out var entry,
                    details,
                    cDetailsMax: 1);
                if (entry.m_steamIDUser == SteamUser.GetSteamID())
                {
                    var timeDiff = entry.m_nScore - firstPlace.m_nScore;
                    diff = "+" + timeDiff.ToString().PadLeft(5, '0').Insert(2, ".");
                    break;
                }
            }
        }

        levelTimeDiffs[_currMission][_currLevel] = diff;
        FetchInProgress = false;
        LBTrackerGUI.GenerateTimeString();
    }
}