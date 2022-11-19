using GuildSaber.Utils;
using UnityEngine;

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
            GameLevelStatsView = GuildSaberUtils.FindGm("RightScreen.PlatformLeaderboardViewController.LevelStatsView");
            Events.e_OnLeaderboardShown += (p_FirstActivation) =>
            {
                if (!GuildSaberCustomLeaderboard.Initialized || !GuildSaberCustomLeaderboard.IsShown) return;

                Hide();
            };
            Events.e_OnLeaderboardHide += () =>
            {
                if (!GuildSaberCustomLeaderboard.Initialized) return;

                Show();
            };

            Initialized = true;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Show Game Level Stats View
        /// </summary>
        public static async void Show()
        {
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
            await WaitUtils.Wait(() => GameLevelStatsView != null, 1);

            foreach (Transform l_Transform in GameLevelStatsView.transform)
                l_Transform.gameObject.SetActive(false);
        }
    }
}
