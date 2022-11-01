﻿using HMUI;
using LeaderboardCore.Models;
using LeaderboardCore.Managers;
using Zenject;
using JetBrains.Annotations;
using System;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System.Collections;
using CP_SDK.Unity;
using GuildSaber.Utils;
using System.Collections.Generic;

namespace GuildSaber.UI.GuildSaber.Leaderboard
{
    [UsedImplicitly]
    class GuildSaberCustomLeaderboard : CustomLeaderboard, IInitializable, IDisposable
    {
        public readonly CustomLeaderboardManager _customLeaderboardManager;
        public readonly GuildSaberLeaderboardPanel _panelViewController;
        public readonly GuildSaberLeaderboardView _leaderboardViewController;

        protected override ViewController panelViewController => _panelViewController;
        protected override ViewController leaderboardViewController => _leaderboardViewController;

        public static bool IsShow = false;

        public GuildSaberCustomLeaderboard(CustomLeaderboardManager customLeaderboardManager,
                                           GuildSaberLeaderboardPanel panelViewController,
                                           GuildSaberLeaderboardView leaderboardViewController)
        {
            _customLeaderboardManager = customLeaderboardManager;
            _panelViewController = panelViewController;
            _leaderboardViewController = leaderboardViewController;

            Events.e_OnLeaderboardHide += OnHide;
            Events.e_OnLeaderboardShown += OnShow;
        }

        private void OnShow(bool p_FirstActivation)
        {
            IsShow = true;
        }

        public void Initialize()
        {
            _customLeaderboardManager.Register(this);
        }

        public void Dispose()
        {
            _customLeaderboardManager.Unregister(this);
        }

        private void OnHide()
        {
            IsShow = false;
            LeaderboardHeaderManager.SetColors(Color.gray, Color.clear);
        }
    }

    public class LeaderboardHeaderManager
    {
        public static LeaderboardHeaderManager m_Instance;

        public static ImageView _HeaderImageView;

        static GameObject m_Header;

        public static Color m_Color0 = new Color(1, 0.5f, 0, 3);
        public static readonly Color m_Color1 = new Color(0.2f, 0.1f, 1, 0.75f);

        public static async void GetPanel()
        {
            if (m_Header != null && _HeaderImageView != null) return;

            await WaitUtils.WaitUntil(() => (m_Header = GuildSaberUtils.FindGm("RightScreen.PlatformLeaderboardViewController.HeaderPanel")) != null, 10);

            _HeaderImageView = m_Header.GetComponentInChildren<ImageView>();
        }
        public void ChangeColors()
        {
            GetPanel();
            SetColors(m_Color0, m_Color1);
        }

        public async static void SetColors(Color p_Color0, Color p_Color1)
        {
            await WaitUtils.WaitUntil(() => m_Header != null, 100);

            _HeaderImageView.color0 = p_Color0;
            _HeaderImageView.color1 = p_Color1;
        }

        public static async void ChangeText(string p_Text)
        {
            GetPanel();

            await WaitUtils.WaitUntil(() => m_Header != null, 100);

            m_Header.GetComponentInChildren<TextMeshProUGUI>().text = p_Text;
        }

        public static void ChangeTextForced(string p_Text)
        {
            GetPanel();
            m_Header.GetComponentInChildren<TextMeshProUGUI>().text = p_Text;
        }
    }
}
