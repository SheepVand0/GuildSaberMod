using HarmonyLib;
using GuildSaberProfile.Installers;
using GuildSaberProfile.UI.GuildSaber.Leaderboard;
using GuildSaberProfile.Utils;
using System;
using Zenject;
using LeaderboardCore.Models;

namespace GuildSaberProfile.Harmony
{
    [HarmonyPatch(typeof(MainSettingsMenuViewControllersInstaller), nameof(MainSettingsMenuViewControllersInstaller.InstallBindings))]
    public static class OnMenuInstallerPatch
    {
        private static void Postfix(MainSettingsMenuViewControllersInstaller __instance)
        {
            try
            {
                //On Menu Settings Install installing Leaderboard Bindings
                DiContainer l_Container = __instance.GetContainer();
                MenuInstaller.Install(l_Container);
            } catch (Exception p_E)
            {
                Plugin.Log.Error("Error during binding Leaderboard (GuildSaberLeaderboard)");
                Plugin.Log.Error($"Here the stacktrace : {p_E}");
            }

        }
    }

    [HarmonyPatch(typeof(CustomLeaderboard),"Show")]
    public static class OnLeaderboardShow
    {
        private static void Postfix(CustomLeaderboard __instance)
        {
            //Checking if showed Leaderboard is the GuildSaber one
            if (__instance.GetType() == typeof(GuildSaberCustomLeaderboard))
            {
                GuildSaberCustomLeaderboard l_Leaderboard = (GuildSaberCustomLeaderboard)__instance;
                l_Leaderboard.OnShow();
                if (l_Leaderboard._panelViewController.m_IsFirtActivation)
                    l_Leaderboard._panelViewController.Reload(ReloadMode.FromApi, true, true);
            }
        }
    }

    [HarmonyPatch(typeof(CustomLeaderboard), "Hide")]
    public static class OnLeaderboardHide
    {
        private static void Postfix(CustomLeaderboard __instance)
        {
            //Checking if showed Leaderboard is the GuildSaber one
            if (__instance.GetType() == typeof(GuildSaberCustomLeaderboard))
            {
                GuildSaberCustomLeaderboard l_Leaderboard = (GuildSaberCustomLeaderboard)__instance;
                l_Leaderboard.OnHide();
            }
        }
    }
}
