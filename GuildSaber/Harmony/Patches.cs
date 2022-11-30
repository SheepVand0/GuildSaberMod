using System;
using System.Linq;
using System.Threading.Tasks;
using BeatLeader.Models;
using BeatSaberPlus.SDK.Game;
using GuildSaber.BSPModule;
using GuildSaber.Configuration;
using GuildSaber.Installers;
using GuildSaber.Logger;
using GuildSaber.UI.Leaderboard;
using GuildSaber.UI.Leaderboard.Components;
using GuildSaber.Utils;
using HarmonyLib;
using HMUI;
using LeaderboardCore.Models;
using Polyglot;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRUIControls;
using Zenject;

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
    public static class OnShow
    {
        public static void Postfix(CustomLeaderboard __instance)
        {
            if (__instance.GetType() != typeof(GuildSaberCustomLeaderboard)) return;

            GuildSaberCustomLeaderboard.IsShown = true;
            Events.OnLeaderboardShow(GuildSaberCustomLeaderboard.Instance.m_PanelViewController.m_IsFirstActivation);
        }
    }

    [HarmonyPatch(typeof(CustomLeaderboard), "Hide")]
    public static class OnHide
    {
        public static void Prefix(CustomLeaderboard __instance)
        {
            if (__instance.GetType() != typeof(GuildSaberCustomLeaderboard)) return;

            GuildSaberCustomLeaderboard.IsShown = false;
            LeaderboardHeaderManager.ChangeTextForced(Localization.Get("TITLE_HIGHSCORES"), false);
            LeaderboardHeaderManager.ResetColors();
            Events.OnLeaderboardIsHide();
        }
    }

    [HarmonyPatch(typeof(MenuTransitionsHelper), nameof(MenuTransitionsHelper.RestartGame))]
    class OnReload
    {
        public static bool s_RepopPatch;

        public static void Prefix()
        {
            if (GuildSaberCustomLeaderboard.Instance == null || !GSConfig.Instance.LeaderboardEnabled) return;

            s_RepopPatch = true;

            Events.InvokeOnReload();

            GuildSaberCustomLeaderboard.Instance.Dispose();
        }
    }

    [HarmonyPatch(typeof(PauseController), nameof(PauseController.HandlePauseMenuManagerDidPressMenuButton))]
    class OnMapExit
    {
        public static async void Postfix()
        {
            if (!LeaderboardScoreList.s_StartedReplayFromMod) return;

            var l_GameScenesManager = Resources.FindObjectsOfTypeAll<GameScenesManager>().First();
            l_GameScenesManager.PopScenes();


            LeaderboardScoreList.s_StartedReplayFromMod = false;

            await WaitUtils.Wait(() => GameObject.Find("Logo") != null, 10, 1000);

            /*foreach (var l_Current in Environment.GetCommandLineArgs())
            {
                if (l_Current.ToLower().Contains("fpfc"))
                {
                    Resources.FindObjectsOfTypeAll<FirstPersonFlyingController>().First().gameObject.SetActive(true);
                    Resources.FindObjectsOfTypeAll<Camera>().First().gameObject.SetActive(true);
                }

            }*/
        }
    }

    [HarmonyPatch(typeof(SoloFreePlayFlowCoordinator), "SinglePlayerLevelSelectionFlowCoordinatorDidActivate")]
    class AfterReloadPatch
    {
        public static async void Postfix()
        {
            if (!OnReload.s_RepopPatch) return;

            await Task.Delay(1500);

            GuildSaberCustomLeaderboard.Instance.Initialize();
        }
    }

    [HarmonyPatch(typeof(FlowCoordinator), "SetTitle", new Type[]
    {
        typeof(string), typeof(ViewController.AnimationType)
    })]
    class UwUModeSoloCoordinatorPatch
    {
        private static void Prefix(FlowCoordinator __instance, ref string value, ref ViewController.AnimationType animationType)
        {
            if (GSConfig.Instance.UwUMode)
                value = "UwU";
        }
    }
}
