// Decompiled with JetBrains decompiler
// Type: GuildSaber.UI.Leaderboard.Components.PointsType
// Assembly: GuildSaber, Version=0.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 2E8F8B5B-092B-47A3-9F65-4C90A48B7328
// Assembly location: C:\Users\user\Desktop\GuildSaber.dll

using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using GuildSaber.API;
using GuildSaber.Logger;
using GuildSaber.Utils;
using HMUI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


#nullable enable
namespace GuildSaber.UI.Leaderboard.Components
{
    public class PointsType : CustomUIComponent
    {
        protected override string ViewResourceName => "GuildSaber.UI.Leaderboard.Components.Views.PointsType.bsml";

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private static GameObject s_GameObjectReference;

        [UIValue("SelectedPoints")] public string m_SelectedPoints = "Undefined";

        [UIComponent("PointsDropdown")] private readonly DropDownListSetting m_SelectorBase = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private static DropDownListSetting m_Selector;

        [UIValue("DefaultPoints")] private List<object> m_DefaultsPoints = new List<object>() { "Undefined" };

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public ApiPlayerData m_Player = new ApiPlayerData();
        private TextMeshProUGUI m_PointsText;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public void Awake() => s_GameObjectReference = gameObject;

        private void OnLeaderboardViewPostLoad() => Events.Instance.SelectPointsTypes(m_SelectedPoints);

        protected override void AfterViewCreation()
        {
            PointsType.m_Selector = m_SelectorBase;
            m_PointsText = m_Selector.GetComponentInChildren<TextMeshProUGUI>();
            m_Selector.GetComponentInChildren<ImageView>().gameObject.SetActive(false);
            Events.Instance.e_OnGuildSelected += OnGuildSelected;
            Events.e_OnLeaderboardPostLoad += OnLeaderboardViewPostLoad;
        }

        public async void RefreshSelected()
        {
            await WaitUtils.Wait(() => m_Selector != null, 1);
            await WaitUtils.Wait(() => m_PointsText != null, 1);
            await WaitUtils.Wait(() => s_GameObjectReference.activeSelf, 1);
            try
            {
                m_PointsText.enableVertexGradient = true;
                m_PointsText.colorGradient = GuildSaberModule.LeaderboardSelectedGuild.Color.ToUnityColor().GenerateGradient(0.2f);
                Task task4 = await WaitUtils.Wait(() => m_Player.RankData != null, 10);
                foreach (RankData rankData in m_Player.RankData)
                {
                    RankData l_Rank = rankData;
                    if (l_Rank.PointsName == m_SelectedPoints)
                        m_PointsText.text = $"{l_Rank.Points} {l_Rank.PointsName}";
                    l_Rank = new RankData();
                }
            }
            catch (Exception ex)
            {
                GSLogger.Instance.Error(ex, nameof(PointsType), nameof(RefreshSelected));
            }
        }

        public async void RefreshPoints()
        {
            await WaitUtils.Wait(() => s_GameObjectReference.activeInHierarchy, 10, 0, 10);
            await WaitUtils.Wait(() => m_Selector != null, 1);
            if (GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData.Equals(null))
                return;

            m_Player = GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData;
            m_Selector.values.Clear();
            foreach (RankData rankData in m_Player.RankData)
            {
                RankData l_Current = rankData;
                m_Selector.values.Add((l_Current.PointsName ?? string.Empty));
                l_Current = new RankData();
            }
            m_Selector.UpdateChoices();
            m_SelectedPoints = (string)m_Selector.values[0];
            m_Selector.Value = m_SelectedPoints;
            m_Selector.ApplyValue();
            Events.Instance.SelectPointsTypes(m_SelectedPoints);
            RefreshSelected();
        }

        private async void OnGuildSelected(int p_SelectedGuild)
        {
            await WaitUtils.Wait(() => s_GameObjectReference.activeInHierarchy, 1, p_CodeLine: 117);
            RefreshPoints();
            //await WaitUtils.Wait(() => !GuildSaberLeaderboardView.m_Instance.ChangingLeaderboard, 10);
            Events.Instance.SelectPointsTypes(m_SelectedPoints);
        }

        [UIAction("OnPointsSelected")]
        private async void OnPointsSelected(string p_Selected)
        {
            m_SelectedPoints = p_Selected;
            Events.Instance.SelectPointsTypes(m_SelectedPoints);
            await Task.Delay(50);
            RefreshSelected();
        }

        public static int GetPointsIDByName(ApiPlayerData p_Player, GuildData p_Guild, string p_Name)
        {
            for (int index = 0; index < p_Player.RankData.Count; ++index)
            {
                if (!(p_Player.RankData[index].PointsName.ToLower() != p_Name.ToLower()))
                    return p_Player.RankData[index].PointsIndex;
            }
            return p_Player.RankData[0].PointsIndex;
        }
    }
}
