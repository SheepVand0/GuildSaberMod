using HMUI;
using LeaderboardCore.Models;
using LeaderboardCore.Managers;
using Zenject;
using JetBrains.Annotations;
using System;
using UnityEngine;
using TMPro;
using GuildSaber.Utils;
using GuildSaber.UI.Components;
using GuildSaber.Configuration;
using GuildSaber.BSPModule;
using LeaderboardCore.Interfaces;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GuildSaber.UI.Leaderboard
{
    [UsedImplicitly]
    class GuildSaberCustomLeaderboard : CustomLeaderboard, IInitializable, IDisposable, INotifyLeaderboardLoad
    {
        public static GuildSaberCustomLeaderboard Instance = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public readonly CustomLeaderboardManager _customLeaderboardManager = null;
        public readonly GuildSaberLeaderboardPanel _panelViewController = null;
        public readonly GuildSaberLeaderboardView _leaderboardViewController = null;

        protected override ViewController panelViewController => _panelViewController;
        protected override ViewController leaderboardViewController => _leaderboardViewController;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public static bool IsShown = false;
        public static bool Initialized { get; private set; } = false;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Zenject lol
        /// </summary>
        /// <param name="customLeaderboardManager"></param>
        /// <param name="panelViewController"></param>
        /// <param name="leaderboardViewController"></param>
        public GuildSaberCustomLeaderboard(CustomLeaderboardManager customLeaderboardManager,
                                           GuildSaberLeaderboardPanel panelViewController,
                                           GuildSaberLeaderboardView leaderboardViewController)
        {
            _customLeaderboardManager = customLeaderboardManager;
            _panelViewController = panelViewController;
            _leaderboardViewController = leaderboardViewController;

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
               _customLeaderboardManager.Register(this);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _customLeaderboardManager.Unregister(this);
        }

        /// <summary>
        /// On Load
        /// </summary>
        /// <param name="loaded"></param>
        public void OnLeaderboardLoaded(bool loaded)
        {
            if (loaded)
            {
                if (Initialized) return;

                Events.m_Instance.EventOnLeaderboardPostLoad();

                Initialized = true;
                LeaderboardLevelStatsViewManager.Setup();
            }
        }

    }
}
