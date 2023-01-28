using GuildSaber.Configuration;
using GuildSaber.Installers;
using GuildSaber.UI.Leaderboard;
using GuildSaber.UI.Leaderboard.Managers;
using GuildSaber.Utils;
using HarmonyLib;
using HMUI;
using LeaderboardCore.Models;
using Polyglot;
using Zenject;

namespace GuildSaber.Harmony;

[HarmonyPatch(typeof(PlatformLeaderboardViewController), nameof(PlatformLeaderboardViewController.SetData))]
public static class OnMapSelected
{
    private static void Postfix(IDifficultyBeatmap difficultyBeatmap)
    {
        if (GuildSaberLeaderboardView.m_Instance != null)
            GuildSaberLeaderboardView.m_Instance.OnLeaderboardSet(difficultyBeatmap);
    }

}

[HarmonyPatch(typeof(MainSettingsMenuViewControllersInstaller), nameof(MainSettingsMenuViewControllersInstaller.InstallBindings))]
public static class MenuInstallerPatch
{
    private static void Postfix(MainSettingsMenuViewControllersInstaller __instance)
    {
        //On Menu Settings Install installing Leaderboard Bindings
        DiContainer l_Container = __instance.GetContainer();
        MenuInstaller.Install(l_Container);
    }
}

[HarmonyPatch(typeof(CustomLeaderboard), "Show")]
public static class OnShow
{
    public static void Postfix(CustomLeaderboard __instance)
    {
        if (__instance.GetType() != typeof(GuildSaberCustomLeaderboard)) return;

        GuildSaberCustomLeaderboard.IsShown = true;
        Events.OnLeaderboardShow(GuildSaberCustomLeaderboard.Instance.m_PanelViewController.m_IsFirstActivation);
        LeaderboardHeaderManager.ShowCustom();
    }
}

[HarmonyPatch(typeof(CustomLeaderboard), "Hide")]
public static class OnHide
{
    public static void Prefix(CustomLeaderboard __instance)
    {
        if (__instance.GetType() != typeof(GuildSaberCustomLeaderboard)) return;

        GuildSaberCustomLeaderboard.IsShown = false;
        //LeaderboardHeaderManager.ChangeTextForced(Localization.Get("TITLE_HIGHSCORES"));
        //LeaderboardHeaderManager.ResetColors();
        Events.OnLeaderboardIsHide();
        LeaderboardHeaderManager.HideCustom();
    }
}

[HarmonyPatch(typeof(MenuTransitionsHelper), nameof(MenuTransitionsHelper.RestartGame))]
internal class OnReload
{
    public static void Prefix()
    {
        if (GuildSaberCustomLeaderboard.Instance == null || !GSConfig.Instance.LeaderboardEnabled) return;

        Events.InvokeOnReload();

        GuildSaberCustomLeaderboard.Instance.Dispose();
    }
}

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