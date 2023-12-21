using CP_SDK.UI.Components;
using CP_SDK.XUI;
using GuildSaber.API;
using GuildSaber.UI.Defaults;
using GuildSaber.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard.Components
{
    internal class LeaderboardPointsSelector : GSButtonDropdown
    {

        public event Action<PointsData> eOnChange;

        protected LeaderboardPointsSelector() : base(new List<string>())
        {
            OnReady(OnCreation);
        }

        public static LeaderboardPointsSelector Make()
        {
            return new LeaderboardPointsSelector();
        }

        protected List<PointsData> m_Points;
        protected string m_SelectedPointsType = GuildApi.PASS_POINTS_TYPE;

        private void OnCreation(CSecondaryButton p_Layout)
        {
            SetWidth(10);
            SetHeight((LeaderboardHeaderPanel.HEADER_HEIGHT / 2) - 1);

            OnValueChanged(OnValueSelected);
        }

        public void SetPoints(List<PointsData> p_Points)
        {
            m_Points = p_Points;
            UpdateChoices();
        }

        public void SetSelectedPoints(string p_SelectedPoints)
        {
            if (p_SelectedPoints != GuildApi.ACC_POINTS_TYPE && p_SelectedPoints != GuildApi.PASS_POINTS_TYPE)
                return;

            m_SelectedPointsType = p_SelectedPoints;
            UpdateChoices();
        }

        public void UpdateChoices()
        {
            List<string> l_Choices = new List<string>();
            foreach (var l_Item in m_Points)
                l_Choices.Add(l_Item.PointsName);
            SetOptions(l_Choices);
        }

        public void OnValueSelected(string p_Value, int p_Index)
        {
            PointsData l_Points = m_Points[p_Index];
            m_SelectedPointsType = l_Points.PointsType;
            eOnChange?.Invoke(l_Points);
        }

        private void UpdateSelectedText(PointsData p_Points)
        {
            var l_Text = Element.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            l_Text.SetText($"{p_Points.Points} {p_Points.PointsName}");
            l_Text.colorGradient = GuildSaberUtils.GenerateGradient(GuildSaberLeaderboardViewController.Instance.GetGuildColor().ColorWithAlpha(1), 0.2f);
            l_Text.enableVertexGradient = true;
        }

        public string GetSelectedValue() => m_SelectedPointsType;

        public PointsData GetSelectedPoints() => m_Points.Where(x => x.PointsType == m_SelectedPointsType).First();
    }
}
