using System.Text;
using BepInEx;
using UnityEngine;

class LBTrackerGUI : MonoBehaviour
{
    static string _timeDiffsString;
    static string _totalWinsString;

    void Awake()
    {
        // debug
        if (!Leaderboard.completedFetch && !Leaderboard.fetchInProgress)
            Leaderboard.FetchAllLBs();

        GenerateTimeString();
    }

    Rect _windowPos = new Rect(980, 150, 800, 800);

    void OnGUI()
    {
        _windowPos = GUILayout.Window(id: 0, _windowPos, WindowFunction, "Friend LB Tracker (F)",
        GUILayout.Width(800),
        GUILayout.Height(800));
    }

    int _margin = 50;
    GUIStyle _diffTextStyle;
    Vector2 _scrollPosition;

    internal static bool displayWinningTimes = true;

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
        if (refreshButton && !Leaderboard.fetchInProgress)
        {
            Leaderboard.FetchAllLBs();
        }

        if (newToggleVal != displayWinningTimes)
        {
            displayWinningTimes = newToggleVal;
            GenerateTimeString();
        }

        GUI.DragWindow();
    }

    const string red = "#FF0000";
    const string green = "#00FF00";
    internal static void GenerateTimeString()
    {
        _totalWinsString = $"You are leading on: {Leaderboard.totalWins} / {Leaderboard.playedMaps} played maps";

        StringBuilder output = new();

        foreach (var missionPair in Leaderboard.levelTimeDiffs)
        {
            output.AppendLine($"<size=20><b>{missionPair.Key}</b></size>");
            foreach (var levelPair in missionPair.Value)
            {
                if (levelPair.Value.StartsWith("-") && !displayWinningTimes) continue;

                var coloredTime = levelPair.Value.StartsWith("-") ?
                $"<color={green}>{levelPair.Value}</color>"
                : $"<color={red}>{levelPair.Value}</color>";
                output.AppendLine($"<size=15>{levelPair.Key}</size>: {coloredTime}");
            }
        }

        if (!Leaderboard.completedFetch)
        {
            output.AppendLine();
            output.AppendLine("<i>Times are still being fetched!</i>");
        }

        _timeDiffsString = output.ToString();
    }
}