using BepInEx;
using FriendLeaderboardTracker;
using HarmonyLib;
using Steamworks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
class FriendLeaderboardTrackerPlugin : BaseUnityPlugin
{
    internal static FriendLeaderboardTrackerPlugin Instance;
    readonly Harmony _harmony = new(MyPluginInfo.PLUGIN_GUID);

    void Awake()
    {
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;
        Instance = this;

        _harmony.PatchAll();
        SceneManager.sceneLoaded += Leaderboard.FetchAllLBs;
    }

    void OnDestroy()
    {
        _harmony.UnpatchSelf();
        SceneManager.sceneLoaded -= Leaderboard.FetchAllLBs;
        if (gameObject.TryGetComponent(out LBTrackerGUI gui))
            Destroy(gui);
    }

    LBTrackerGUI gui;

    void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame && SceneManager.GetActiveScene().name == "Heaven_Environment")
        {
            if (gui)
            {
                Destroy(gui);
                gui = null;
            }
            else
                gui = gameObject.AddComponent<LBTrackerGUI>();
        }

        if (Keyboard.current.rKey.wasPressedThisFrame && gui && !Leaderboard.fetchInProgress)
        {
            Leaderboard.FetchAllLBs();
        }

        if (Keyboard.current.dKey.wasPressedThisFrame && gui)
        {
            LBTrackerGUI.displayWinningTimes = !LBTrackerGUI.displayWinningTimes;
            LBTrackerGUI.GenerateTimeString();
        }
    }
}