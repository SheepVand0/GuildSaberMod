using System;
using System.Linq;
using BeatSaberPlus.SDK.Game;
using GuildSaber.Configuration;
using GuildSaber.Installers;
using GuildSaber.UI.Leaderboard;
using GuildSaber.Utils;
using HarmonyLib;
using HMUI;
using LeaderboardCore.Models;
using Polyglot;
using UnityEngine;
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
            Events.OnLeaderboardShow(GuildSaberCustomLeaderboard.Instance.m_PanelViewController.m_IsFirtActivation);
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
        public static void Prefix()
        {
            if (GuildSaberCustomLeaderboard.Instance == null) return;

            if (LeaderboardHeaderManager.m_HeaderImageView.gameObject.activeInHierarchy)
            {
                LeaderboardHeaderManager.ChangeTextForced(Localization.Get("TITLE_HIGHSCORES"), false);
                LeaderboardHeaderManager.ResetColors();
            }

            GuildSaberCustomLeaderboard.Initialized = false;
            GuildSaberLeaderboardPanel.PanelInstance.m_IsFirtActivation = true;
            GuildSaberCustomLeaderboard.Instance.Dispose();
        }
    }

    [HarmonyPatch(typeof(GameScenesManager), nameof(GameScenesManager.PushScenes))]
    class OnStart
    {
        public static void Prefix()
        {
            if (GuildSaberCustomLeaderboard.Instance == null) return;

            //GSLogger.Instance.Log("here", IPA.Logging.Logger.LogLevel.NoticeUp);

            // ReSharper disable once SimplifyConditionalTernaryExpression
            if (GSConfig.Instance.LeaderboardEnabled && Resources.FindObjectsOfTypeAll<PracticeViewController>().ElementAt(0) == null ? true : !Resources.FindObjectsOfTypeAll<PracticeViewController>().ElementAt(0).isActivated)
            {
                if (LeaderboardHeaderManager.m_HeaderImageView != null)
                {

                    if (LeaderboardHeaderManager.m_HeaderImageView.gameObject.activeInHierarchy)
                    {
                        LeaderboardHeaderManager.ChangeTextForced(Localization.Get("TITLE_HIGHSCORES"), false);
                        LeaderboardHeaderManager.ResetColors();
                    }
                }
                GuildSaberCustomLeaderboard.Instance.Dispose();
            }
        }
    }

    [HarmonyPatch(typeof(GameScenesManager), nameof(GameScenesManager.PopScenes))]
    class OnReturnToMenu
    {
        public static async void Postfix(float minDuration)
        {
            // ReSharper disable once SimplifyConditionalTernaryExpression
            if (Resources.FindObjectsOfTypeAll<PracticeViewController>().ElementAt(0) == null ? true : !Resources.FindObjectsOfTypeAll<PracticeViewController>().ElementAt(0).gameObject.activeInHierarchy) return;

            if (Logic.ActiveScene is Logic.SceneType.Menu or Logic.SceneType.None) return;

            await WaitUtils.Wait(() => Logic.ActiveScene == Logic.SceneType.Menu, 100, 500);

            await WaitUtils.Wait(() => GameObject.Find("LeaderboardNavigationButtonsPanel") != null, 100);
            await WaitUtils.Wait(() => GameObject.Find("LeaderboardNavigationButtonsPanel").gameObject.activeInHierarchy, 100, 50);

            // ReSharper disable once SimplifyConditionalTernaryExpression
            await WaitUtils.Wait(() => Resources.FindObjectsOfTypeAll<ResultsViewController>().ElementAt(0) == null ? true : !Resources.FindObjectsOfTypeAll<ResultsViewController>().ElementAt(0).gameObject.activeInHierarchy,10);

            GuildSaberCustomLeaderboard.Instance.Initialize();
        }
    }

    [HarmonyPatch(typeof(FlowCoordinator), "SetTitle", new Type[] {  typeof(string), typeof(ViewController.AnimationType) })]
    class UwUModeSoloCoordinatorPatch
    {
        private static void Prefix(FlowCoordinator __instance, ref string value, ref ViewController.AnimationType animationType)
        {
            if (GSConfig.Instance.UwUMode)
                value = "UwU";
        }
    }
}
