using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using GuildSaber.UI.GuildSaber.Leaderboard;
using HMUI;
using UnityEngine;
using GuildSaber.API;
using GuildSaber.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using System.Collections;
using GuildSaber.BSPModule;

namespace GuildSaber.UI.Components
{
    public class PointsType : CustomUIComponent
    {
        [UIComponent("PointsDropdown")] DropDownListSetting m_Selector = null;

        TextMeshProUGUI m_PointsText;

        public ApiPlayerData m_Player = default(ApiPlayerData);

        protected override string m_ViewResourceName => "GuildSaber.UI.Components.Views.PointsType.bsml";
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

        private async void OnGuildSelected(int p_SelectedGuild)
        {
            RefreshPoints();
            await Task.Delay(100);
            RefreshSelected();
        }

        [UIValue("DefaultPoints")] private List<object> m_DefaultsPoints = new List<object>() { Plugin.NOT_DEFINED };
        [UIValue("SelectedPoints")] public string m_SelectedPoints = Plugin.NOT_DEFINED;

        [UIAction("OnPointsSelected")]
        private async void OnPointsSelected(string p_Selected)
        {
            m_SelectedPoints = p_Selected;
            Events.m_Instance.SelectPointsTypes(m_SelectedPoints);
            await Task.Delay(100);
            RefreshSelected();
        }

        public void RefreshSelected()
        {
            m_PointsText.enableVertexGradient = true;
            m_PointsText.colorGradient = (GuildSaberModule.m_LeaderboardSelectedGuild.Color.ToUnityColor()).GenerateGradient(0.2f);
            foreach (RankData l_Rank in m_Player.RankData)
                if (l_Rank.PointsName == m_SelectedPoints)
                    m_PointsText.text = $"{l_Rank.Points} {l_Rank.PointsName}";
        }

        public void RefreshPoints()
        {
            if (GuildSaberLeaderboardPanel.Instance.m_PlayerData.Equals(default(PlayerData))) return;
            m_Player = GuildSaberLeaderboardPanel.Instance.m_PlayerData;
            m_DefaultsPoints.Clear();
            foreach (RankData l_Current in m_Player.RankData)
            {
                m_DefaultsPoints.Add($"{l_Current.PointsName}");
            }
            m_Selector.UpdateChoices();
            m_SelectedPoints = (string)m_Selector.values[0];
            m_Selector.Value = m_SelectedPoints;
            m_Selector.ApplyValue();
        }
    }
}
