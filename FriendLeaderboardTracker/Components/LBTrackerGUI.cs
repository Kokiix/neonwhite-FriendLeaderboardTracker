using System.Text;
using BepInEx;
using UnityEngine;

class LBTrackerGUI : MonoBehaviour
{

    internal static bool displayWinningTimes = true;

    void Awake()
    {
        // debug
        if (!FetchLeaderboards.completedFetch && !FetchLeaderboards.fetchInProgress)
            FetchLeaderboards.FetchAllLBs();

        UpdateGUIText();
    }

    Rect _windowPos = new Rect(980, 150, 800, 800);

    void OnGUI()
    {
        _windowPos = GUILayout.Window(id: 0, _windowPos, WindowFunction, "Friend LB Tracker (G)",
        GUILayout.Width(800),
        GUILayout.Height(800));
    }


    static string _timeDiffsString;
    static string _totalWinsString;
    GUIStyle _diffTextStyle;
    Vector2 _scrollPosition;

    int _margin = 50;

    void WindowFunction(int windowID)
    {
        if (_diffTextStyle == null)
        {
            _diffTextStyle = new(GUI.skin.box)
            {
                alignment = TextAnchor.UpperLeft,
                richText = true,
                wordWrap = true,
            };
        }

        // Layout
        GUILayout.BeginHorizontal();
        GUILayout.Space(_margin);
        GUILayout.BeginVertical();
        GUILayout.Space(25);

        GUILayout.BeginHorizontal();
        GUILayout.Box(_totalWinsString);
        GUILayout.Space(85);
        var refreshButton = GUILayout.Button("Refresh (R)", GUILayout.Width(100));
        GUILayout.Space(25);
        var newToggleVal = GUILayout.Toggle(
            displayWinningTimes,
            "Display levels where you are winning (D)");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(25);

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
        GUILayout.Box(
            _timeDiffsString,
            _diffTextStyle);
        GUILayout.EndScrollView();

        GUILayout.Space(_margin);
        GUILayout.EndVertical();
        GUILayout.Space(_margin);
        GUILayout.EndHorizontal();

        // Side effects
        if (refreshButton && !FetchLeaderboards.fetchInProgress)
        {
            FetchLeaderboards.FetchAllLBs();
        }

        if (newToggleVal != displayWinningTimes)
        {
            displayWinningTimes = newToggleVal;
            UpdateGUIText();
        }

        GUI.DragWindow();
    }

    const string _red = "#FF0000";
    const string _green = "#00FF00";
    internal static void UpdateGUIText()
    {
        _totalWinsString = $"You are leading on: {FetchLeaderboards.totalWins} / {FetchLeaderboards.playedMaps} played maps";

        StringBuilder output = new();

        foreach (var missionPair in FetchLeaderboards.levelTimeDiffs)
        {
            output.AppendLine($"<size=20><b>{missionPair.Key}</b></size>");
            foreach (var levelPair in missionPair.Value)
            {
                if (levelPair.Value.StartsWith("-") && !displayWinningTimes) continue;

                var coloredTime = levelPair.Value.StartsWith("-") ?
                $"<color={_green}>{levelPair.Value}</color>"
                : $"<color={_red}>{levelPair.Value}</color>";
                output.AppendLine($"<size=15>{levelPair.Key}</size>: {coloredTime}");
            }
        }

        if (!FetchLeaderboards.completedFetch)
        {
            output.AppendLine();
            output.AppendLine("<i>Times are still being fetched!</i>");
        }

        _timeDiffsString = output.ToString();
    }
}