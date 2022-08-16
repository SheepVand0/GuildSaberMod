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

namespace GuildSaberProfile.UI.Components
{
    public class PointsType : CustomUIComponent
    {
        [UIComponent("PointsText")] ClickableText m_PointsText = null;
        [UIComponent("PointsSelectorModal")] ModalView m_SelectorModal = null;
        [UIComponent("PointsDropdown")] DropDownListSetting m_Selector = null;

        public PlayerApiReworkOutput m_Player = new PlayerApiReworkOutput();

        protected override string m_ViewResourceName => "GuildSaberProfile.UI.Components.Views.PointsType.bsml";

        private GuildSaberLeaderboardPanel _LeaderboardPanel = null;

        public override void PostCreate()
        {
            _LeaderboardPanel = Resources.FindObjectsOfTypeAll<GuildSaberLeaderboardPanel>()[0];
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

        [UIAction("OnTextClick")]
        public void OpenModal()
        {
            m_SelectorModal.Show(true, true, null);
        }

        [UIAction("OnPointsSelected")]
        private void OnPointsSelected(string p_Selected)
        {
            m_SelectedPoints = p_Selected;
            RefreshSelected();
            Events.m_Instance.SelectPointsTypes(m_SelectedPoints);
        }

        public void RefreshSelected()
        {
            if (string.IsNullOrEmpty(m_Player.Name)) return;
            m_PointsText.enableVertexGradient = true;
            m_PointsText.colorGradient = ((Color)m_Player.ProfileColor.ToUnityColor()).GenerateGradient(0.2f);
            foreach (RankData l_Current in m_Player.RankData)
                if (l_Current.PointsName == m_SelectedPoints) m_PointsText.text = $"{l_Current.Points} {l_Current.PointsName}";
            foreach (RankData l in m_Player.RankData)
                Plugin.Log.Info(l.Points.ToString());
        }

        public void RefreshPoints()
        {
            m_Player = _LeaderboardPanel.m_PlayerGuildsInfo.m_ReturnPlayer;
            if (m_Player.Name == string.Empty) return;
            List<RankData> l_RankData = m_Player.RankData;
            m_Selector.values.Clear();
            foreach (RankData l_Current in l_RankData)
                m_Selector.values.Add(l_Current.PointsName);
            m_Selector.UpdateChoices();
            m_Selector.Value = m_Selector.values[0];
            m_Selector.ApplyValue();
        }
    }
}
