using HarmonyLib;
using GuildSaber.Installers;
using GuildSaber.UI.Leaderboard;
using GuildSaber.Utils;
using GuildSaber.Configuration;
using System;
using Zenject;
using LeaderboardCore.Models;
using UnityEngine;
using TMPro;
using HMUI;
using Polyglot;
using CP_SDK.Unity;
using GuildSaber.Logger;

namespace GuildSaber.Harmony
{
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
    public static class OnLeaderboardShow
    {
        private static void Postfix(CustomLeaderboard __instance)
        {
            //Checking if showed Leaderboard is the GuildSaber one
            if (__instance.GetType() == typeof(GuildSaberCustomLeaderboard))
            {
                GuildSaberCustomLeaderboard l_Leaderboard = (GuildSaberCustomLeaderboard)__instance;
                GuildSaberCustomLeaderboard.IsShown = true;
                Events.OnLeaderboardShow(l_Leaderboard._panelViewController.m_IsFirtActivation);
            }
        }
    }

    [HarmonyPatch(typeof(CustomLeaderboard), "Hide")]
    public static class OnLeaderboardHide
    {
        private static void Prefix(CustomLeaderboard __instance)
        {
            //Checking if showed Leaderboard is the GuildSaber one
            if (__instance.GetType() == typeof(GuildSaberCustomLeaderboard))
            {
                GuildSaberCustomLeaderboard.IsShown = false;
                LeaderboardHeaderManager.ChangeTextForced(Localization.Get("TITLE_HIGHSCORES"), false);
                LeaderboardHeaderManager.ResetColors();
                Events.OnLeaderboardIsHide();
            }
        }
    }

    [HarmonyPatch(typeof(FlowCoordinator), "SetTitle", new Type[] { typeof(string), typeof(ViewController.AnimationType) })]
    class UwUModeSoloCoordinatorPatch
    {
        private static void Prefix(FlowCoordinator __instance, ref string value, ref ViewController.AnimationType animationType)
        {
            if (GSConfig.Instance.UwUMode)
                value = "UwU";
        }
    }
}
