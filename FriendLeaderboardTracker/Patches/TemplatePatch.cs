using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(LevelBase), "baseInit")]
static class TemplatePatch
{
    static void Postfix(LevelBase __instance)
    {
        Debug.LogError(__instance.skippable);
        __instance.conductor.barNumber = 50;
        Debug.LogError(__instance.conductor.barNumber);
    }
}