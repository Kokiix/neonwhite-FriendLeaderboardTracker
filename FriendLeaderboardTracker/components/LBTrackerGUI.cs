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
        GUI.Window(id: 0, new Rect(980, 150, 800, 800), WindowFunction, "Friend LB Tracker (F)");
    }

    int _topControlHeight = 50;
    int _margin = 50;
    GUIStyle _diffTextStyle;

    static bool displayWinningTimes = true;

    void WindowFunction(int windowID)
    {
        if (_diffTextStyle == null)
        {
            _diffTextStyle = new(GUI.skin.box)
            {
                alignment = TextAnchor.UpperLeft,
                richText = true,
            };
        }

        var newToggleVal = GUI.Toggle(new Rect(
            _margin,
            _topControlHeight,
            250,
            50),
            displayWinningTimes,
            "Display levels where you are winning");
        if (newToggleVal != displayWinningTimes)
        {
            displayWinningTimes = newToggleVal;
            GenerateTimeString();
        }

        GUI.Box(new Rect(
            _margin,
            _margin + _topControlHeight,
            800 - _margin * 2,
            800 - _margin * 2 - _topControlHeight),
        DisplayString,
        _diffTextStyle);

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