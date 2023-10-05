using BeatSaberPlus.SDK.UI;
using CP_SDK.XUI;
using GuildSaber.API;
using GuildSaber.Logger;
using GuildSaber.UI.Card;
using GuildSaber.UI.CustomLevelSelectionMenu.Components;
using GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard.Components;
using GuildSaber.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard
{
    internal class GuildSaberLeaderboardViewController : ViewController<GuildSaberLeaderboardViewController>
    {
        protected XUIHLayout m_LeaderboardPanelContainer;

        LeaderboardPanel m_LeaderboardPanel;

        XUIVLayout m_ScopeSelectorLayout;
        XUIVLayout m_LeaderboardCellsContainer;

        XUIText m_LoadingText;
        XUIText m_ErrorText;

        LeaderboardScopeSelector m_ScopeSelector;

        protected List<LeaderboardCell> m_Cells = new List<LeaderboardCell>();

        protected IDifficultyBeatmap m_SelectedBeatmap;

        public enum ELeaderboardMode
        {
            Error,
            Normal,
            Loading
        }

        protected override void OnViewCreation()
        {
            Templates.FullRectLayout(
                XUIHLayout.Make(
                    m_LeaderboardPanel = LeaderboardPanel.Make()
                    )
                    .SetWidth(100)
                    .Bind(ref m_LeaderboardPanelContainer),
                XUIHLayout.Make(
                    XUIText.Make("Error loading leaderboard").SetColor(Color.red).Bind(ref m_ErrorText),
                    XUIText.Make("Loading...").Bind(ref m_LoadingText),
                    XUIVLayout.Make(
                        m_ScopeSelector = LeaderboardScopeSelector.Make()
                        ).Bind(ref m_ScopeSelectorLayout),
                    XUIVLayout.Make(
                        
                        )
                        .SetSpacing(-0.5f)
                        .Bind(ref m_LeaderboardCellsContainer)
                    )
                ).BuildUI(transform);

            MapButton.eOnMapSelected += OnMapSelected;

            SetMode(ELeaderboardMode.Normal);
            m_ScopeSelector.SetScope(ELeaderboardScope.Global);
            m_ScopeSelector.eOnScopeChanged += OnScopeChanged;
        }
        
        protected void OnScopeChanged(ELeaderboardScope p_Scope)
        {
            OnMapSelected(m_SelectedBeatmap);
        }

        protected void OnMapSelected(IDifficultyBeatmap p_Beatmap)
        { 
            string l_Hash = GSBeatmapUtils.DifficultyBeatmapToHash(p_Beatmap);
            m_SelectedBeatmap = p_Beatmap;
            SetLeaderboard(CustomLevelSelectionMenuReferences.SelectedGuildId, l_Hash, p_Beatmap, 1);
        }

        public void SetMode(ELeaderboardMode p_Mode)
        {
            m_ErrorText.SetActive(p_Mode == ELeaderboardMode.Error);
            m_ScopeSelectorLayout.SetActive(p_Mode == ELeaderboardMode.Normal);
            m_LeaderboardCellsContainer.SetActive(p_Mode == ELeaderboardMode.Normal);
            m_LoadingText.SetActive(p_Mode == ELeaderboardMode.Loading);
        }

        public async void SetGuild(int p_GuildId)
        {
            GuildData l_GuildData = GuildApi.GetGuildFromId(p_GuildId);
            var l_Banner = await GuildSaberUtils.GetImage(l_GuildData.Banner);
            Texture2D l_Result;
            if (l_Banner.IsError)
            {
                l_Result = CustomLevelSelectionMenuReferences.DefaultLogo;
            } else
            {
                l_Result = l_Banner.Texture;
            }
            m_LeaderboardPanel.SetGuild(l_Result, PlayerCardUI.m_Player.Name);
        }

        public async void SetLeaderboard(int p_GuildID, string p_MapHash, IDifficultyBeatmap p_Beatmap, int p_Page, int p_SearchType = 1)
        {
            ApiPlayerData l_PlayerData = PlayerCardUI.m_Player;
            try
            {
                ApiMapLeaderboardCollectionStruct l_Leaderboard = await GuildApi.GetLeaderboard(p_GuildID, p_MapHash, p_Beatmap, p_Page, (m_ScopeSelector.GetSelectedScope() == ELeaderboardScope.Around) ? (int)GuildSaberModule.GSPlayerId : 0, (m_ScopeSelector.GetSelectedScope() == ELeaderboardScope.Country) ? l_PlayerData.Country : string.Empty, p_Beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName);

                UpdateLeaderboardCells(l_Leaderboard);
            } catch (Exception l_E)
            {
                GSLogger.Instance.Error(l_E, nameof(GuildSaberLeaderboardViewController), nameof(SetLeaderboard));
                SetMode(ELeaderboardMode.Error);
            }
        }

        public void UpdateLeaderboardCells(ApiMapLeaderboardCollectionStruct p_Leaderboard)
        {
            for (int l_i = 0; l_i < p_Leaderboard.Leaderboards.Count();l_i++)
            {
                if (l_i > m_Cells.Count() - 1)
                {
                    LeaderboardCell l_Cell = LeaderboardCell.Make();
                    l_Cell.BuildUI(m_LeaderboardCellsContainer.Element.LElement.transform);
                    l_Cell.SetPointsType(p_Leaderboard.Leaderboards[l_i].PointsData[0]);
                    l_Cell.SetScore(p_Leaderboard.Leaderboards[l_i], (int)p_Leaderboard.CustomData.MaxScore);
                    m_Cells.Add(l_Cell);
                } else
                {
                    m_Cells[l_i].SetScore(p_Leaderboard.Leaderboards[l_i], (int)p_Leaderboard.CustomData.MaxScore);
                }
            }
        }

    }
}
