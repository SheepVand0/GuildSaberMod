using HarmonyLib;
using GuildSaber.Installers;
using GuildSaber.UI.GuildSaber.Leaderboard;
using GuildSaber.Utils;
using GuildSaber.Configuration;
using System;
using Zenject;
using LeaderboardCore.Models;
using UnityEngine;
using TMPro;
using HMUI;
using Polyglot;

namespace GuildSaber.Harmony
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
                Events.OnLeaderboardShow(l_Leaderboard._panelViewController.m_IsFirtActivation);
                if (l_Leaderboard._panelViewController.m_IsFirtActivation)
                    l_Leaderboard._panelViewController.Reload(ReloadMode.FromCurrent, true, true);
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
                Events.OnLeaderboardIsHide();
                LeaderboardHeaderManager.m_Instance.ChangeText(Localization.Get("TITLE_HIGHSCORES"));
            }
        }
    }

    [HarmonyPatch(typeof(FlowCoordinator), "SetTitle", new Type[] { typeof(string), typeof(ViewController.AnimationType)})]
    class UwUModeSoloCoordinatorPatch
    {
        private static void Prefix(FlowCoordinator __instance, ref string value, ref ViewController.AnimationType animationType)
        {
            if (GSConfig.Instance.UwUMode)
                value = "UwU";
        }
    }
}
