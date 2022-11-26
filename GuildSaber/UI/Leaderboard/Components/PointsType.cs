using System.Collections.Generic;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using GuildSaber.API;
using GuildSaber.BSPModule;
using GuildSaber.Utils;
using HMUI;
using TMPro;

namespace GuildSaber.UI.Leaderboard.Components
{
    public class PointsType : CustomUIComponent
    {
        protected override string ViewResourceName => "GuildSaber.UI.Leaderboard.Components.Views.PointsType.bsml";

        [UIComponent("PointsDropdown")] DropDownListSetting m_Selector = null;
        TextMeshProUGUI m_PointsText;

        [UIValue("DefaultPoints")] private List<object> m_DefaultsPoints = new List<object>()
        {
            Plugin.NOT_DEFINED
        };
        [UIValue("SelectedPoints")] public string m_SelectedPoints = Plugin.NOT_DEFINED;

        public ApiPlayerData m_Player = default;

        private static UnityEngine.GameObject s_GameObjectReference;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Leaderboard Post Load
        /// </summary>
        private void OnLeaderboardViewPostLoad()
        {
            Events.Instance.SelectPointsTypes(m_SelectedPoints);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// After View Creation
        /// </summary>
        protected override void AfterViewCreation()
        {
            s_GameObjectReference = gameObject;

            m_PointsText = m_Selector.GetComponentInChildren<TextMeshProUGUI>();
            ImageView l_ImageView = m_Selector.GetComponentInChildren<ImageView>();
            l_ImageView.gameObject.SetActive(false);
            RefreshPoints();
            RefreshSelected();
            Events.Instance.e_OnGuildSelected += OnGuildSelected;
            Events.e_OnLeaderboardPostLoad += OnLeaderboardViewPostLoad;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Refresh selected points color and text
        /// </summary>
        public async void RefreshSelected()
        {
            await WaitUtils.Wait(() => m_Selector != null, 100);
            await WaitUtils.Wait(() => m_PointsText != null, 100);
            await WaitUtils.Wait(() => GuildSaberLeaderboardPanel.PanelInstance != null, 100);

            m_PointsText.enableVertexGradient = true;
            m_PointsText.colorGradient = (GuildSaberModule.LeaderboardSelectedGuild.Color.ToUnityColor()).GenerateGradient(0.2f);
            foreach (RankData l_Rank in m_Player.RankData)
                if (l_Rank.PointsName == m_SelectedPoints)
                    m_PointsText.text = $"{l_Rank.Points} {l_Rank.PointsName}";
        }

        /// <summary>
        /// Refresh Points List
        /// </summary>
        public async void RefreshPoints()
        {
            await WaitUtils.Wait(() => m_Selector != null, 100);
            await WaitUtils.Wait(() => GuildSaberLeaderboardPanel.PanelInstance != null, 100);

            if (!s_GameObjectReference.activeInHierarchy) return;

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData.Equals(default(PlayerData))) return;
            m_Player = GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData;
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

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On Guild selected
        /// </summary>
        /// <param name="p_SelectedGuild"></param>
        private async void OnGuildSelected(int p_SelectedGuild)
        {
            RefreshPoints();
            await Task.Delay(100);
            RefreshSelected();
            await WaitUtils.Wait(() => GuildSaberLeaderboardView.m_Instance.ChangingLeaderboard == false, 10);
            Events.Instance.SelectPointsTypes(m_SelectedPoints);
        }

        /// <summary>
        /// On Dropdown element selected
        /// </summary>
        /// <param name="p_Selected"></param>
        [UIAction("OnPointsSelected")]
        private async void OnPointsSelected(string p_Selected)
        {
            m_SelectedPoints = p_Selected;
            Events.Instance.SelectPointsTypes(m_SelectedPoints);
            await Task.Delay(100);
            RefreshSelected();
        }


        public static int GetPointsIDByName(ApiPlayerData p_Player, GuildData p_Guild, string p_Name)
        {
            for (int l_i = 0; l_i < p_Player.RankData.Count; l_i++)
            {
                if (p_Player.RankData[l_i].PointsName.ToLower() != p_Name.ToLower()) continue;

                return p_Player.RankData[l_i].PointsIndex;
            }

            return p_Player.RankData[0].PointsIndex;
        }
    }
}
