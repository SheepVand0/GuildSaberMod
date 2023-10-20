using CP_SDK.UI.Components;
using CP_SDK.XUI;
using GuildSaber.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

namespace GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard.Components
{
    internal class LeaderboardPointsSelector : XUIHLayout
    {

        public event Action<PointsData> eOnChange;

        protected LeaderboardPointsSelector(string p_Name, params IXUIElement[] p_Childs) : base(p_Name, p_Childs)
        {
            OnReady(OnCreation);
        }

        public static LeaderboardPointsSelector Make()
        {
            return new LeaderboardPointsSelector("GuildSaberPointsSelector");
        }

        XUIDropdown m_Dropdown;

        protected List<PointsData> m_Points;
        protected string m_SelectedPointsType = GuildApi.PASS_POINTS_TYPE;

        private void OnCreation(CHOrVLayout p_Layout)
        {
            SetWidth(15);
            SetHeight(2);
            XUIDropdown.Make()
                .Bind(ref m_Dropdown)
                .BuildUI(Element.LElement.transform);

            var l_DropdownText = Element.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            GSText.PatchText(l_DropdownText);
            m_Dropdown.OnValueChanged(OnValueSelected);
            var l_Image = m_Dropdown.Element.GetComponentInChildren<Image>();
            l_Image.color = new UnityEngine.Color(0, 0, 0, 0.9f);
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
            m_Dropdown.SetOptions(l_Choices);
            UpdateSelectedText(GetSelectedPoints());
        }

        public void OnValueSelected(int p_Index, string p_Value)
        {
            PointsData l_Points = m_Points[p_Index];
            m_SelectedPointsType = l_Points.PointsType;
            eOnChange?.Invoke(l_Points);
            UpdateSelectedText(l_Points);
        }

        private void UpdateSelectedText(PointsData p_Points)
        {
            var l_Text = Element.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            l_Text.SetText($"{p_Points.Points} {p_Points.PointsName}");
        }

        public string GetSelectedValue() => m_SelectedPointsType;

        public PointsData GetSelectedPoints() => m_Points.Where(x => x.PointsType == m_SelectedPointsType).First();
    }
}
