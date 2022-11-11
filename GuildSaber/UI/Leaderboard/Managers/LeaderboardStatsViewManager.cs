using UnityEngine;

namespace GuildSaber.UI.Leaderboard
{
    internal class LeaderboardLevelStatsViewManager
    {
        public static GameObject GameLevelStatsView = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Setup Manager
        /// </summary>
        public static void Setup()
        {
            GameLevelStatsView = Utils.GuildSaberUtils.FindGm("RightScreen.PlatformLeaderboardViewController.LevelStatsView");
            Events.e_OnLeaderboardShown += (p_FirstActivation) =>
            {
                if (!GuildSaberCustomLeaderboard.Initialized) return;

                Show();
            };
            Events.e_OnLeaderboardHide += () =>
            {
                if (!GuildSaberCustomLeaderboard.Initialized) return;

                Hide();
            };
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Show Game Level Stats View
        /// </summary>
        public static void Show()
        {
            foreach (Transform l_Transform in GameLevelStatsView.transform)
                l_Transform.gameObject.SetActive(false);
        }

        /// <summary>
        /// Hide Game Level Stats View
        /// </summary>
        public static void Hide()
        {
            foreach (Transform l_Transform in GameLevelStatsView.transform)
                l_Transform.gameObject.SetActive(true);
        }
    }
}
