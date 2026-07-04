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
    internal static FriendLeaderboardTrackerPlugin instance;
    readonly Harmony _harmony = new(MyPluginInfo.PLUGIN_GUID);

    void Awake()
    {
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;
        instance = this;

        _harmony.PatchAll();
        SceneManager.sceneLoaded += FetchLeaderboards.OnSceneLoad;
    }

    void OnDestroy()
    {
        _harmony.UnpatchSelf();
        SceneManager.sceneLoaded -= FetchLeaderboards.OnSceneLoad;
        if (gameObject.TryGetComponent(out LBTrackerGUI gui))
            Destroy(gui);
    }

    LBTrackerGUI _gui;

    void Update()
    {
        if (Keyboard.current.gKey.wasPressedThisFrame && SceneManager.GetActiveScene().name == "Heaven_Environment")
        {
            if (_gui)
            {
                Destroy(_gui);
                _gui = null;
            }
            else
                _gui = gameObject.AddComponent<LBTrackerGUI>();
        }

        if (Keyboard.current.rKey.wasPressedThisFrame && _gui && !FetchLeaderboards.fetchInProgress)
        {
            FetchLeaderboards.FetchAllLBs();
        }

        if (Keyboard.current.dKey.wasPressedThisFrame && _gui)
        {
            LBTrackerGUI.displayWinningTimes = !LBTrackerGUI.displayWinningTimes;
            LBTrackerGUI.UpdateGUIText();
        }
    }
}