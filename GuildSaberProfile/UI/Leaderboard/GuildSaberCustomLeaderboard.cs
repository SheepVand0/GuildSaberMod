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

        private void OnShow(bool p_FirstActivation)
        {
            if (m_HeaderManager == null)
                m_HeaderManager = new GameObject("Leaderboardheader").AddComponent<LeaderboardHeaderManager>();

            m_HeaderManager.ChangeColors();
        }

        private void OnHide()
        {
            m_HeaderManager.StopChangingColors();
        }
    }

    public class LeaderboardHeaderManager : MonoBehaviour
    {
        public ImageView _HeaderImageView;

        GameObject m_Header;

        public bool m_Idle = false;

        public static Color m_Color0 = new Color(1, 0.5f, 0, 3);
        public static readonly Color m_Color1 = new Color(0.2f, 0.1f, 1, 0.75f);
        public void ChangeColors()
        {
            try
            {
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

            if (_HeaderImageView == null)
            {
                var l_ScreenTransform = GameObject.Find("RightScreen").transform;
                m_Header = l_ScreenTransform.FindChildRecursive("HeaderPanel").gameObject;

                _HeaderImageView = m_Header.GetComponentInChildren<ImageView>();
            }

            _HeaderImageView.color0 = m_Color0;
            _HeaderImageView.color1 = m_Color1;

            if (_HeaderImageView.color1 == m_Color0 && _HeaderImageView.color1 == m_Color1)
                m_Idle = true;
        }

        public void ChangeText(string p_Text)
        {
            if (m_Header == null) return;

            m_Header.GetComponentInChildren<TextMeshProUGUI>().text = p_Text;
        }
    }
}
