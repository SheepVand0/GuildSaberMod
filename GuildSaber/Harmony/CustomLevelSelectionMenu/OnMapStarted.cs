using GuildSaber.Logger;
using GuildSaber.UI.CustomLevelSelectionMenu.FlowCoordinators;
using GuildSaber.UI.CustomLevelSelectionMenu.Practice;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using static BeatmapObjectSpawnMovementData;
using PViewController = GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.PracticeViewController;
using IPA.Utilities;
using GuildSaber.UI.CustomLevelSelectionMenu;

namespace GuildSaber.Harmony.CustomLevelSelectionMenu
{
    [HarmonyPatch(typeof(BeatmapObjectSpawnMovementData), nameof(BeatmapObjectSpawnMovementData.Init))]
    internal class OnMapStarted
    {
        public static void Prefix(BeatmapObjectSpawnMovementData __instance, ref int noteLinesCount, ref float startNoteJumpMovementSpeed, ref float startBpm, ref NoteJumpValueType noteJumpValueType, ref float noteJumpValue, ref IJumpOffsetYProvider jumpOffsetYProvider, ref Vector3 rightVec, ref Vector3 forwardVec)
        {
            if (!CustomLevelSelectionMenuReferences.IsInPractice) return;

            startNoteJumpMovementSpeed = PViewController.Instance.GetCustomNJS();
        }

        /*public static void Postfix(BeatmapObjectSpawnMovementData __instance)
        {
            GSLogger.Instance.Log("Current NJS : ", IPA.Logging.Logger.LogLevel.NoticeUp);
            GSLogger.Instance.Log(__instance.GetField<float, BeatmapObjectSpawnMovementData>("_noteJumpMovementSpeed"), IPA.Logging.Logger.LogLevel.NoticeUp);
        }*/

    }
}
