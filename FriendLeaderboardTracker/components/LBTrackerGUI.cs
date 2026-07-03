using System.Text;
using BepInEx;
using UnityEngine;

class LBTrackerGUI : MonoBehaviour
{
    static string DisplayString;

    void Awake()
    {
        // debug
        if (!Leaderboard.completedFetch && !Leaderboard.fetchInProgress)
            Leaderboard.FetchAllLBs();

        GenerateTimeString();
    }

    void OnGUI()
    {
        GUILayout.Window(id: 0, new Rect(980, 150, 800, 800), WindowFunction, "Friend LB Tracker (F)",
        GUILayout.Width(800),
        GUILayout.Height(800));
    }

    int _topControlHeight = 50;
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
        var refreshButton = GUILayout.Button("Refresh (R)", GUILayout.Width(100));
        GUILayout.Space(50);
        var newToggleVal = GUILayout.Toggle(
            displayWinningTimes,
            "Display levels where you are winning (D)");
        GUILayout.EndHorizontal();

        GUILayout.Space(25);

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
        GUILayout.Box(
            DisplayString,
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
    }

    const string red = "#FF0000";
    const string green = "#00FF00";
    internal static void GenerateTimeString()
    {
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

        DisplayString = output.ToString();
    }
}