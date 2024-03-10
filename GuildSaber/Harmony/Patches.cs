using GuildSaber.Configuration;
using GuildSaber.Utils;
using HarmonyLib;
using HMUI;
using LeaderboardCore.Models;
using Polyglot;
using Zenject;

namespace GuildSaber.Harmony;


/*[HarmonyPatch(typeof(SoloFreePlayFlowCoordinator), "SinglePlayerLevelSelectionFlowCoordinatorDidActivate")]
class AfterReloadPatch
{
    public static async void Postfix()
    {
        if (!OnReload.s_RepopPatch) return;

        await Task.Delay(1500);

        GuildSaberCustomLeaderboard.Instance.Initialize();
    }
}*/

[HarmonyPatch(typeof(FlowCoordinator), "SetTitle", typeof(string), typeof(ViewController.AnimationType))]
internal class UwUModeSoloCoordinatorPatch
{
    private static void Prefix(FlowCoordinator __instance, ref string value, ref ViewController.AnimationType animationType)
    {
        if (GSConfig.Instance.UwUMode) value = "UwU";
    }
}

/*[HarmonyPatch(typeof(GameNoteController), nameof(GameNoteController.HandleBigWasCutBySaber))]
class CheesePatch
{
    private static List<(NoteCutDirection, UnityEngine.Vector3)> s_MinVelocityForCutDirection =
    new List<(NoteCutDirection, Vector3)>();

    public static void Postfix(GameNoteController __instance, Saber saber, UnityEngine.Vector3 cutPoint,  UnityEngine.Quaternion orientation, UnityEngine.Vector3 cutDirVec)
    {
        GSLogger.Instance.Log("Cuting", IPA.Logging.Logger.LogLevel.DebugUp);
        GSLogger.Instance.Log(cutDirVec.ToString(), IPA.Logging.Logger.LogLevel.DebugUp);
    }
}*/