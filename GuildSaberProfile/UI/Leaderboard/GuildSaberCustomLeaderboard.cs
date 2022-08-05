using HMUI;
using LeaderboardCore.Models;
using LeaderboardCore.Managers;
using Zenject;
using JetBrains.Annotations;
using System;
using UnityEngine;
using TMPro;

namespace GuildSaberProfile.UI.GuildSaber.Leaderboard
{
    [UsedImplicitly]
    class GuildSaberCustomLeaderboard : CustomLeaderboard, IInitializable, IDisposable
    {
        public readonly CustomLeaderboardManager _customLeaderboardManager;
        public readonly GuildSaberLeaderboardPanel _panelViewController;
        public readonly GuildSaberLeaderboardView _leaderboardViewController;

        public LeaderboardHeaderManager m_HeaderManager;
        protected override ViewController panelViewController => _panelViewController;
        protected override ViewController leaderboardViewController => _leaderboardViewController;

        public GuildSaberCustomLeaderboard(CustomLeaderboardManager customLeaderboardManager,
                                           GuildSaberLeaderboardPanel panelViewController,
                                           GuildSaberLeaderboardView leaderboardViewController)
        {
            _customLeaderboardManager = customLeaderboardManager;
            _panelViewController = panelViewController;
            _leaderboardViewController = leaderboardViewController;

            Events.e_OnLeaderboardShown += OnShow;
            Events.e_OnLeaderboardHide += OnHide;
        }

        public void Initialize()
        {
            _customLeaderboardManager.Register(this);
        }

        public void Dispose()
        {
            _customLeaderboardManager.Unregister(this);
        }

        public void OnShow()
        {
            if (m_HeaderManager == null)
                m_HeaderManager = new GameObject("Leaderboardheader").AddComponent<LeaderboardHeaderManager>();

            m_HeaderManager.ChangeColors();
        }

        public void OnHide()
        {
            m_HeaderManager.StopChangingColors();
        }

        public void RefreshLeaderboard(string p_Hash)
        {

        }
    }

    public class LeaderboardHeaderManager : MonoBehaviour
    {
        public ImageView _HeaderImageView;

        public bool m_Idle = false;

        public static Color m_Color0 { get => new Color(1, 0.5f, 0, 3); set { } }
        public static readonly Color m_Color1 = new Color(0.2f, 0.1f, 1, 0.75f);

        public void ChangeColors()
        {
            try
            {
                Plugin.Log.Info("Changing GuildSaber Leaderboard Colors");
                var l_ScreenTransform = GameObject.Find("RightScreen").transform;
                var l_Header = l_ScreenTransform.FindChildRecursive("HeaderPanel").gameObject;

                _HeaderImageView = l_Header.GetComponentInChildren<ImageView>();
                l_Header.GetComponentInChildren<TextMeshProUGUI>().text = "UwU";

                m_Idle = false;
            } catch(Exception p_E)
            {
                Plugin.Log.Error($"An Error occurred when changing leaderboard colors -> {p_E.Message}");
            }
        }

        public void StopChangingColors()
        {
            m_Idle = true;
        }

        public void Update()
        {
            if (m_Idle == true) return;

            if (_HeaderImageView == null) return;

            _HeaderImageView.color0 = m_Color0;
            _HeaderImageView.color1 = m_Color1;

            if (_HeaderImageView.color1 == m_Color0 && _HeaderImageView.color1 == m_Color1)
                m_Idle = true;
        }
    }
}
