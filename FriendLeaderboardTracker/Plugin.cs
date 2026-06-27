using BepInEx;
using FriendLeaderboardTracker;
using HarmonyLib;
using UnityEngine;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
class FriendLeaderboardTrackerPlugin : BaseUnityPlugin
{
    readonly Harmony _harmony = new(MyPluginInfo.PLUGIN_GUID);

    void Awake()
    {
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;

        _harmony.PatchAll();
    }

    void OnDestroy()
    {
        _harmony.UnpatchSelf();
    }

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.T))
    //     {
    //     }
    // }
}