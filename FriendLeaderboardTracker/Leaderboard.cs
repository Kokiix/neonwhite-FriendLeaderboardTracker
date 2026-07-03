using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

static class Leaderboard
{
    internal static int playedMaps = 0;
    internal static int totalWins = 0;

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
        totalWins = 0;
        FriendLeaderboardTrackerPlugin.Instance.StartCoroutine(FetchAllRoutine());
    }

    internal static Dictionary<string, Dictionary<string, string>> levelTimeDiffs = [];

    internal static string currMission = "";
    internal static string currLevel = "";

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
        {"Interface/MISSION_10_TITLE", "The Third Temple"},
        {"Interface/MISSION_11_TITLE", "Thousand Pound Butterfly"},
        {"Interface/MISSION_12_TITLE", "Hand of God"},
    };

    internal static readonly Dictionary<string, string> LEVEL_NAMES = new()
    {
    { "TUT_MOVEMENT", "Movement" },
    { "TUT_SHOOTINGRANGE", "Pummel" },
    { "SLUGGER", "Gunner" },
    { "TUT_FROG", "Cascade" },
    { "TUT_JUMP", "Elevate" },
    { "GRID_TUT_BALLOON", "Bounce" },
    { "TUT_BOMB2", "Purify" },
    { "TUT_BOMBJUMP", "Climb" },
    { "TUT_FASTTRACK", "Fasttrack" },
    { "GRID_PORT", "Glass Port" },
    { "GRID_PAGODA", "Take Flight" },
    { "TUT_RIFLE", "Godspeed" },
    { "TUT_RIFLEJOCK", "Dasher" },
    { "TUT_DASHENEMY", "Thrasher" },
    { "GRID_JUMPDASH", "Outstretched" },
    { "GRID_SMACKDOWN", "Smackdown" },
    { "GRID_MEATY_BALLOONS", "Catwalk" },
    { "GRID_FAST_BALLOON", "Fastlane" },
    { "GRID_DRAGON2", "Distinguish" },
    { "GRID_DASHDANCE", "Dancer" },
    { "TUT_GUARDIAN", "Guardian" },
    { "TUT_UZI", "Stomp" },
    { "TUT_JUMPER", "Jumper" },
    { "TUT_BOMB", "Dash Tower" },
    { "GRID_DESCEND", "Descent" },
    { "GRID_STAMPEROUT", "Driller" },
    { "GRID_CRUISE", "Canals" },
    { "GRID_SPRINT", "Sprint" },
    { "GRID_MOUNTAIN", "Mountain" },
    { "GRID_SUPERKINETIC", "Superkinetic" },
    { "GRID_ARRIVAL", "Arrival" },
    { "FLOATING", "Forgotten City" },
    { "GRID_BOSS_YELLOW", "The Clocktower" },
    { "GRID_HOPHOP", "Fireball" },
    { "GRID_RINGER_TUTORIAL", "Ringer" },
    { "GRID_RINGER_EXPLORATION", "Cleaner" },
    { "GRID_HOPSCOTCH", "Warehouse" },
    { "GRID_BOOM", "Boom" },
    { "GRID_SNAKE_IN_MY_BOOT", "Streets" },
    { "GRID_FLOCK", "Steps" },
    { "GRID_BOMBS_AHOY", "Demolition" },
    { "GRID_ARCS", "Arcs" },
    { "GRID_APARTMENT", "Apartment" },
    { "TUT_TRIPWIRE", "Hanging Gardens" },
    { "GRID_TANGLED", "Tangled" },
    { "GRID_HUNT", "Waterworks" },
    { "GRID_CANNONS", "Killswitch" },
    { "GRID_FALLING", "Falling" },
    { "TUT_SHOCKER2", "Shocker" },
    { "TUT_SHOCKER", "Bouquet" },
    { "GRID_PREPARE", "Prepare" },
    { "GRID_TRIPMAZE", "Triptrack" },
    { "GRID_RACE", "Race" },
    { "TUT_FORCEFIELD2", "Bubble" },
    { "GRID_SHIELD", "Shield" },
    { "SA L VAGE2", "Overlook" },
    { "GRID_VERTICAL", "Pop" },
    { "GRID_MINEFIELD", "Minefield" },
    { "TUT_MIMIC", "Mimic" },
    { "GRID_MIMICPOP", "Trigger" },
    { "GRID_SWARM", "Greenhouse" },
    { "GRID_SWITCH", "Sweep" },
    { "GRID_TRAPS2", "Fuse" },
    { "TUT_ROCKETJUMP", "Heavens Edge" },
    { "TUT_ZIPLINE", "Zipline" },
    { "GRID_CLIMBANG", "Swing" },
    { "GRID_ROCKETUZI", "Chute" },
    { "GRID_CRASHLAND", "Crash" },
    { "GRID_ESCALATE", "Ascent" },
    { "GRID_SPIDERCLAUS", "Straightaway" },
    { "GRID_FIRECRACKER_2", "Firecracker" },
    { "GRID_SPIDERMAN", "Streak" },
    { "GRID_DESTRUCTION", "Mirror" },
    { "GRID_HEAT", "Escalation" },
    { "GRID_BOLT", "Bolt" },
    { "GRID_PON", "Godstreak" },
    { "GRID_CHARGE", "Plunge" },
    { "GRID_MIMICFINALE", "Mayhem" },
    { "GRID_BARRAGE", "Barrage" },
    { "GRID_1GUN", "Estate" },
    { "GRID_HECK", "Trapwire" },
    { "GRID_ANTFARM", "Ricochet" },
    { "GRID_FORTRESS", "Fortress" },
    { "GRID_GODTEMPLE_ENTRY", "Holy Ground" },
    { "GRID_BOSS_GODSDEATHTEMPLE", "The Third Temple" },
    { "GRID_EXTERMINATOR", "Spree" },
    { "GRID_FEVER", "Breakthrough" },
    { "GRID_SKIPSLIDE", "Glide" },
    { "GRID_CLOSER", "Closer" },
    { "GRID_HIKE", "Hike" },
    { "GRID_SKIP", "Switch" },
    { "GRID_CEILING", "Access" },
    { "GRID_BOOP", "Congregation" },
    { "GRID_TRIPRAP", "Sequence" },
    { "GRID_ZIPRAP", "Marathon" },
    { "TUT_ORIGIN", "Sacrifice" },
    { "GRID_BOSS_RAPTURE", "Absolution" },
    { "SIDEQUEST_OBSTACLE_PISTOL", "Elevate Traversal I" },
    { "SIDEQUEST_OBSTACLE_PISTOL_SHOOT", "Elevate Traversal II" },
    { "SIDEQUEST_OBSTACLE_MACHINEGUN", "Purify Traversal" },
    { "SIDEQUEST_OBSTACLE_RIFLE_2", "Godspeed Traversal" },
    { "SIDEQUEST_OBSTACLE_UZI2", "Stomp Traversal" },
    { "SIDEQUEST_OBSTACLE_SHOTGUN", "Fireball Traversal" },
    { "SIDEQUEST_OBSTACLE_ROCKETLAUNCHER", "Dominion Traversal" },
    { "SIDEQUEST_RAPTURE_QUEST", "Book of Life Traversal" },
    { "SIDEQUEST_DODGER", "Doghouse" },
    { "GRID_GLASSPATH", "Choker" },
    { "GRID_GLASSPATH2", "Chain" },
    { "GRID_HELLVATOR", "Hellevator" },
    { "GRID_GLASSPATH3", "Razor" },
    { "SIDEQUEST_ALL_SEEING_EYE", "All Seeing Eye" },
    { "SIDEQUEST_RESIDENTSAWB", "Resident Saw I" },
    { "SIDEQUEST_RESIDENTSAW", "Resident Saw II" },
    { "SIDEQUEST_SUNSET_FLIP_POWERBOMB", "Sunset Flip Powerbomb" },
    { "GRID_BALLOONLAIR", "Balloon Mountain" },
    { "SIDEQUEST_BARREL_CLIMB", "Climbing Gym" },
    { "SIDEQUEST_FISHERMAN_SUPLEX", "Fisherman Suplex" },
    { "SIDEQUEST_STF", "STF" },
    { "SIDEQUEST_ARENASIXNINE", "Arena" },
    { "SIDEQUEST_ATTITUDE_ADJUSTMENT", "Attitude Adjustment" },
    { "SIDEQUEST_ROCKETGODZ", "Rocket" }
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
                    currMission = MISSION_NAMES[mission.missionDisplayName];
                else
                {
                    currMission = mission.missionDisplayName;
                    if (currMission == "Green's Maps") break;
                }
                levelTimeDiffs[currMission] = [];

                foreach (var level in mission.levels)
                {
                    if (LEVEL_NAMES.ContainsKey(level.levelID))
                    {
                        currLevel = LEVEL_NAMES[level.levelID];
                        GetLBData(level.levelID);
                        yield return new WaitWhile(() => FetchInProgress);
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
        }
        // debug
        yield return null;

        completedFetch = true;
        fetchInProgress = false;
        LBTrackerGUI.GenerateTimeString();
    }

    static CallResult<LeaderboardFindResult_t> _lbFindResult = CallResult<LeaderboardFindResult_t>.Create(OnLBFound);
    static CallResult<LeaderboardScoresDownloaded_t> _lbDownloadResult = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLBDownloaded);
    static bool FetchInProgress = true;

    internal static void GetLBData(string levelID)
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
            levelTimeDiffs[currMission][currLevel] = "Not Played";
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
            playedMaps++;
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
                totalWins++;
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
                    playedMaps++;
                    var timeDiff = entry.m_nScore - firstPlace.m_nScore;
                    diff = "+" + timeDiff.ToString().PadLeft(5, '0').Insert(2, ".");
                    break;
                }
            }
        }

        levelTimeDiffs[currMission][currLevel] = diff;
        FetchInProgress = false;
        LBTrackerGUI.GenerateTimeString();
    }
}