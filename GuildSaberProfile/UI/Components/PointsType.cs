using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using GuildSaberProfile.UI.GuildSaber.Leaderboard;
using HMUI;
using UnityEngine;
using GuildSaberProfile.API;
using GuildSaberProfile.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;

namespace GuildSaberProfile.UI.Components
{
    public class PointsType : CustomUIComponent
    {
        [UIComponent("PointsDropdown")] DropDownListSetting m_Selector = null;

        TextMeshProUGUI m_PointsText;

        public PlayerApiReworkOutput m_Player = new PlayerApiReworkOutput();

        protected override string m_ViewResourceName => "GuildSaberProfile.UI.Components.Views.PointsType.bsml";
        protected override void PostCreate()
        {
            m_PointsText = m_Selector.GetComponentInChildren<TextMeshProUGUI>();
            ImageView l_ImageView = m_Selector.GetComponentInChildren<ImageView>();
            /*l_ImageView.color = Color.white.ColorWithAlpha(0);
            l_ImageView.color0 = Color.white.ColorWithAlpha(0);
            l_ImageView.color1 = Color.white.ColorWithAlpha(0);*/
            l_ImageView.gameObject.SetActive(false);
            RefreshPoints();
            RefreshSelected();
            Events.m_Instance.e_OnGuildSelected += OnGuildSelected;
            Events.m_Instance.e_OnLeaderboardPostLoad += OnLeaderboardViewPostLoad;
        }

        private void OnLeaderboardViewPostLoad()
        {
            Events.m_Instance.SelectPointsTypes(m_SelectedPoints);
        }

        private void OnGuildSelected(string p_SelectedGuild)
        {
            RefreshPoints();
            RefreshSelected();
        }

        [UIValue("DefaultPoints")] private readonly List<object> m_DefaultsPoints = new List<object>() { Plugin.NOT_DEFINED };
        [UIValue("SelectedPoints")] public string m_SelectedPoints = Plugin.NOT_DEFINED;

        [UIAction("OnPointsSelected")]
        private async void OnPointsSelected(string p_Selected)
        {
            await Task.Run(delegate
            {
                m_SelectedPoints = p_Selected;
                Events.m_Instance.SelectPointsTypes(m_SelectedPoints);
            });
            RefreshSelected();
        }

        public void RefreshSelected()
        {
            if (string.IsNullOrEmpty(GuildSaberLeaderboardView._LeaderboardPanel.m_PlayerGuildsInfo.m_ReturnPlayer.Name)) return;
            m_Player = GuildSaberLeaderboardView._LeaderboardPanel.m_PlayerGuildsInfo.m_ReturnPlayer;
            m_PointsText.enableVertexGradient = true;
            m_PointsText.colorGradient = ((Color)m_Player.ProfileColor.ToUnityColor()).GenerateGradient(0.2f);
            foreach (RankData l_Rank in m_Player.RankData)
                if (l_Rank.PointsName == m_SelectedPoints)
                    m_PointsText.text = $"{l_Rank.Points} {l_Rank.PointsName}";
        }

        public void RefreshPoints()
        {
            m_Player = GuildSaberLeaderboardView._LeaderboardPanel.m_PlayerGuildsInfo.m_ReturnPlayer;
            if (m_Player.Name == string.Empty) return;
            m_Selector.values.Clear();
            foreach (RankData l_Current in m_Player.RankData)
                m_Selector.values.Add($"{l_Current.PointsName}");
            m_Selector.UpdateChoices();
            m_SelectedPoints = (string)m_Selector.values[0];
            m_Selector.Value = m_SelectedPoints;
            m_Selector.ApplyValue();
        }
    }
}
