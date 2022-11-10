using HMUI;
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
using GuildSaber.UI.Components;
using GuildSaber.Configuration;
using GuildSaber.BSPModule;

namespace GuildSaber.UI.GuildSaber.Leaderboard
{
    [UsedImplicitly]
    class GuildSaberCustomLeaderboard : CustomLeaderboard, IInitializable, IDisposable
    {
        public readonly CustomLeaderboardManager _customLeaderboardManager = null;
        public readonly GuildSaberLeaderboardPanel _panelViewController = null;
        public readonly GuildSaberLeaderboardView _leaderboardViewController = null;

        protected override ViewController panelViewController => _panelViewController;
        protected override ViewController leaderboardViewController => _leaderboardViewController;

        public static bool IsShown = false;

        public static bool Initialized { get; private set; } = false;

        public static GuildSaberCustomLeaderboard Instance = null;


        public GuildSaberCustomLeaderboard(CustomLeaderboardManager customLeaderboardManager,
                                           GuildSaberLeaderboardPanel panelViewController,
                                           GuildSaberLeaderboardView leaderboardViewController)
        {
            _customLeaderboardManager = customLeaderboardManager;
            _panelViewController = panelViewController;
            _leaderboardViewController = leaderboardViewController;

            Instance = this;

            Events.e_OnLeaderboardPostLoad += OnLeaderboardPostLoad;
        }

        private void OnLeaderboardPostLoad()
        {
            Initialized = true;
            LeaderboardLevelStatsViewManager.Setup();
        }

        public void Initialize()
        {
            if (GSConfig.Instance.LeaderboardEnabled)
               _customLeaderboardManager.Register(this);
        }

        public void Dispose()
        {
            _customLeaderboardManager.Unregister(this);
        }
    }

    public class LeaderboardHeaderManager
    {
        public static LeaderboardHeaderManager m_Instance = null;

        public static ImageView _HeaderImageView = null;

        static GameObject m_Header = null;

        public static Color m_Color0 = new Color(1, 0.5f, 0, 3);
        public static readonly Color m_Color1 = new Color(0.2f, 0.1f, 1, 0.75f);

        static UpdateView m_UpdatesModal = null;

        public static async void GetPanel()
        {
            if (m_Header != null && _HeaderImageView != null)
            {
                if (m_UpdatesModal == null)
                {
                    m_UpdatesModal = CustomUIComponent.CreateItem<UpdateView>(m_Header.transform, true, true);
                    m_UpdatesModal.CheckUpdates();
                }
                return;
            }

            await WaitUtils.WaitUntil(() => (m_Header = GuildSaberUtils.FindGm("RightScreen.PlatformLeaderboardViewController.HeaderPanel")) != null, 10);

            _HeaderImageView = m_Header.GetComponentInChildren<ImageView>();
        }
        public void ChangeColors()
        {
            if (!GuildSaberCustomLeaderboard.Initialized) return;
            GetPanel();
            SetColors(m_Color0, m_Color1);
        }

        public async static void SetColors(Color p_Color0, Color p_Color1)
        {
            if (!GuildSaberCustomLeaderboard.Initialized) return;

            await WaitUtils.WaitUntil(() => m_Header != null, 100);

            _HeaderImageView.color0 = p_Color0;
            _HeaderImageView.color1 = p_Color1;
        }

        public static void ResetColors()
        {
            if (!GuildSaberCustomLeaderboard.Initialized || _HeaderImageView == null || GuildSaberModule.ModState == GuildSaberModule.EModState.APIError) return;

            _HeaderImageView.color0 = Color.gray;
            _HeaderImageView.color1 = Color.clear;
        }

        public static async void ChangeText(string p_Text)
        {
            if (!GuildSaberCustomLeaderboard.Initialized) return;
            GetPanel();

            await WaitUtils.WaitUntil(() => m_Header != null, 100);

            m_Header.GetComponentInChildren<TextMeshProUGUI>().text = p_Text;
        }

        public static void ChangeTextForced(string p_Text, bool p_ChangeReallyForce)
        {
            if (!p_ChangeReallyForce)
                if (!GuildSaberCustomLeaderboard.Initialized) return;
            if (GuildSaberModule.ModState == GuildSaberModule.EModState.APIError)
                return;
            GetPanel();
            m_Header.GetComponentInChildren<TextMeshProUGUI>().text = p_Text;
        }

        public static void ChangeTextForced(string p_Text)
        {
            ChangeTextForced(p_Text, false);
        }
    }

    internal class LeaderboardLevelStatsViewManager
    {
        public static GameObject GameLevelStatsView = null;

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

        public static void Show()
        {
            foreach (Transform l_Transform in GameLevelStatsView.transform)
                l_Transform.gameObject.SetActive(false);
        }

        public static void Hide()
        {
            foreach (Transform l_Transform in GameLevelStatsView.transform)
                l_Transform.gameObject.SetActive(true);
        }
    }
}
