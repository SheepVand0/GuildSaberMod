using GuildSaber.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

// ReSharper disable once CheckNamespace
namespace GuildSaber.UI.Leaderboard
{
    internal class LeaderboardLevelStatsViewManager
    {
        public static GameObject GameLevelStatsView = null;

        public static bool Initialized { get; private set; } = false;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Setup Manager
        /// </summary>
        public static void Setup()
        {
            Events.e_OnLeaderboardShown += OnShow;
            Events.e_OnLeaderboardHide += OnHide;

            Initialized = true;
        }

        private static void GetReferences()
        {
            GameLevelStatsView = (GuildSaberUtils.FindGm("RightScreen.PlatformLeaderboardViewController.LevelStatsView") ?? null)!;
            if (GameLevelStatsView == null) return;
        }

        private static void OnShow(bool p_FirstActivation)
        {
            if (!GuildSaberCustomLeaderboard.Initialized/* || !GuildSaberCustomLeaderboard.IsShown*/) return;

            Hide();
        }

        private static void OnHide()
        {
            if (!GuildSaberCustomLeaderboard.Initialized) return;

            Show();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Show Game Level Stats View
        /// </summary>
        public static async void Show()
        {
            GetReferences();

            await WaitUtils.Wait(() => GameLevelStatsView != null, 100, p_MaxTryCount: 20);

            if (GameLevelStatsView == null) return;

            foreach (Transform l_Transform in GameLevelStatsView.transform)
                l_Transform.gameObject.SetActive(true);
        }

        /// <summary>
        /// Hide Game Level Stats View
        /// </summary>
        public static async void Hide()
        {
            GetReferences();

            await WaitUtils.Wait(() => GameLevelStatsView != null, 1);

            foreach (Transform l_Transform in GameLevelStatsView.transform)
                l_Transform.gameObject.SetActive(false);
        }
    }
}
