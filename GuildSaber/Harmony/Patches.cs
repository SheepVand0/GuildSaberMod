using System;
using System.Linq;
using System.Threading.Tasks;
using BeatSaberPlus.SDK.Game;
using GuildSaber.BSPModule;
using GuildSaber.Configuration;
using GuildSaber.Installers;
using GuildSaber.Logger;
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

    //[HarmonyPatch(typeof(MenuTransitionsHelper), nameof(MenuTransitionsHelper.RestartGame))]
    class OnReload
    {
        public static void Prefix()
        {
            if (GuildSaberCustomLeaderboard.Instance == null) return;

            GuildSaberCustomLeaderboard.Instance.Dispose();
        }
    }

    //[HarmonyPatch(typeof(GameScenesManager), nameof(GameScenesManager.PushScenes))]
    class OnStart
    {
        public static void Prefix()
        {
            if (GuildSaberCustomLeaderboard.Instance == null) return;

            //GSLogger.Instance.Log("here", IPA.Logging.Logger.LogLevel.NoticeUp);

            // ReSharper disable once SimplifyConditionalTernaryExpression
            if (GSConfig.Instance.LeaderboardEnabled &&
                Resources.FindObjectsOfTypeAll<PracticeViewController>().ElementAt(0) == null
                    ? false
                    : Resources.FindObjectsOfTypeAll<PracticeViewController>().ElementAt(0).gameObject.activeInHierarchy)
                return;

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

    //[HarmonyPatch(typeof(FadeInOutController), nameof(FadeInOutController.FadeOut))]
    class OnReturnToMenu
    {
        public static async void Postfix(float duration)
        {
            await Task.Delay((int)duration*1000);
            await Task.Delay(500);

            // ReSharper disable once SimplifyConditionalTernaryExpression
            if (Resources.FindObjectsOfTypeAll<PracticeViewController>().ElementAt(0) == null ? false : Resources.FindObjectsOfTypeAll<PracticeViewController>().ElementAt(0).gameObject.activeInHierarchy) return;



            //if (Logic.ActiveScene is Logic.SceneType.Menu or Logic.SceneType.None) return;

            bool l_MoveNext = false;

            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            while (!l_MoveNext)
            {
                ResultsViewController l_ResultsViewController = Resources.FindObjectsOfTypeAll<ResultsViewController>().ElementAt(0);
                GameObject l_NavigationButtonsPanel = GameObject.Find("LeaderboardNavigationButtonsPanel");
                GameObject l_MainScreen = GameObject.Find("MainScreen");
                await WaitUtils.Wait(() =>
                {
                    if (Logic.ActiveScene != Logic.SceneType.Menu) return true;
                    if (!l_ResultsViewController)
                        if (l_ResultsViewController.gameObject.activeSelf)
                            return true;
                    if (l_NavigationButtonsPanel == null) return true;
                    l_MoveNext = l_NavigationButtonsPanel.gameObject.activeSelf;
                    return l_MoveNext;
                }, 10);
            }

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
