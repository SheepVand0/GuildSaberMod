using System;
using System.Linq;
using GuildSaber.Configuration;
using GuildSaber.Utils;
using HMUI;
using JetBrains.Annotations;
using LeaderboardCore.Interfaces;
using LeaderboardCore.Managers;
using LeaderboardCore.Models;
using UnityEngine;
using Zenject;

namespace GuildSaber.UI.Leaderboard
{
    [UsedImplicitly]
    class GuildSaberCustomLeaderboard : CustomLeaderboard, IInitializable, IDisposable, INotifyLeaderboardLoad
    {
        public static GuildSaberCustomLeaderboard? Instance = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public readonly CustomLeaderboardManager? m_CustomLeaderboardManager = null;
        public readonly GuildSaberLeaderboardPanel? m_PanelViewController = null;
        public readonly GuildSaberLeaderboardView? m_LeaderboardViewController = null;

        protected override ViewController panelViewController => m_PanelViewController;
        protected override ViewController leaderboardViewController => m_LeaderboardViewController;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public static bool IsShown = false;
        public static bool Initialized { get; internal set; } = false;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Zenject lol
        /// </summary>
        /// <param name="p_CustomLeaderboardManager"></param>
        /// <param name="p_PanelViewController"></param>
        /// <param name="p_LeaderboardViewController"></param>
        public GuildSaberCustomLeaderboard(CustomLeaderboardManager p_CustomLeaderboardManager,
                                           GuildSaberLeaderboardPanel p_PanelViewController,
                                           GuildSaberLeaderboardView p_LeaderboardViewController)
        {
            m_CustomLeaderboardManager = p_CustomLeaderboardManager;
            m_PanelViewController = p_PanelViewController;
            m_LeaderboardViewController = p_LeaderboardViewController;

            Instance = this;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On Initialize
        /// </summary>
        public void Initialize()
        {
            if (GSConfig.Instance.LeaderboardEnabled)
               m_CustomLeaderboardManager.Register(this);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            m_CustomLeaderboardManager.Unregister(this);
        }

        /// <summary>
        /// On Load
        /// </summary>
        /// <param name="p_Loaded"></param>
        public async void OnLeaderboardLoaded(bool p_Loaded)
        {
            if (!p_Loaded)  return;

            if (Initialized) return;

            Initialized = true;
            LeaderboardLevelStatsViewManager.Setup();

            Events.Instance.EventOnLeaderboardPostLoad();

            await WaitUtils.Wait(() => IsShown, 100, 0, 5);

            if (m_LeaderboardViewController.gameObject.activeInHierarchy)
                LeaderboardLevelStatsViewManager.Hide();
        }

    }
}
