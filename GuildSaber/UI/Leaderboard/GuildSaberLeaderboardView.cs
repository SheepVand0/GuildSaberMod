﻿using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using GuildSaber.API;
using GuildSaber.Utils;
using GuildSaber.UI.Components;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using GuildSaber.Configuration;
using Polyglot;
using GuildSaber.BSPModule;
using Button = UnityEngine.UI.Button;
using GuildSaber.Logger;
using LeaderboardCore.Interfaces;

namespace GuildSaber.UI.Leaderboard
{
    [HotReload(RelativePathToLayout = @"LeaderboardView.bsml")]
    [ViewDefinition("GuildSaber.UI.GuildSaber.View.LeaderboardView.bsml")]
    internal class GuildSaberLeaderboardView : BSMLAutomaticViewController, INotifyLeaderboardSet
    {

        [UIComponent("ScoreParamsLayout")] VerticalLayoutGroup m_ScoreParamsLayout = null;
        [UIComponent("WorldSelection")] VerticalLayoutGroup m_ScopeSelectionLayout = null;
        [UIComponent("PageUpImage")] Button m_PageUpImage = null;
        [UIComponent("PageDownImage")] Button m_PageDownImage = null;

        [UIComponent("NotRankedText")] TextMeshProUGUI m_NotRankedText = null;
        [UIComponent("ErrorText")] TextMeshProUGUI m_ErrorText = null;
        [UIComponent("Loading")] GridLayoutGroup m_LoadingLayout = null;

        LeaderboardScoreList m_ScoresList = null;
        ScopeSelector m_ScopeSelector = null;
        public CustomLevelStatsView CustomLevelStatsView = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public static GuildSaberLeaderboardView m_Instance = null;

        private ELeaderboardScope m_SelectedScope = ELeaderboardScope.Global;

        public string m_CurrentPointsName { get; internal set; } = string.Empty;
        public string m_CurrentMapHash { get; internal set; } = string.Empty;
        public static IDifficultyBeatmap m_CurrentBeatmap { get; internal set; } = null;
        public ApiMapLeaderboardCollectionStruct m_Leaderboard { get; private set; } = default(ApiMapLeaderboardCollectionStruct);
        public int Page = 1;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Post Parse
        /// </summary>
        [UIAction("#post-parse")]
        private void PostParse()
        {
            //Plugin.Log.Info("Creating GuildSaber leaderboard view");
            m_ScoresList = CustomUIComponent.CreateItem<LeaderboardScoreList>(m_ScoreParamsLayout.transform, true, true);
            CustomLevelStatsView = CustomUIComponent.CreateItem<CustomLevelStatsView>(m_ScoreParamsLayout.transform, true, true);
            m_ScopeSelector = CustomUIComponent.CreateItem<ScopeSelector>(m_ScopeSelectionLayout.transform, true, true);

            BindEvents();

            m_Instance = this;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Bind all leaderboard events
        /// </summary>
        private void BindEvents()
        {
            if (Events.m_Instance == null)
            {
                GSLogger.Instance.Error(new System.Exception("Events manager is null"), nameof(GuildSaberLeaderboardView), nameof(BindEvents));
                return;
            }

            Events.e_OnLeaderboardShown += OnLeaderboardShow;
            Events.m_Instance.e_OnPointsTypeChange += OnPointsTypeChange;
            Events.m_Instance.e_OnGuildSelected += OnGuildSelected;
            Events.m_Instance.e_OnScopeSelected += OnScopeSelected;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// When the leaderboard is shown
        /// </summary>
        /// <param name="p_FirstActivation"></param>
        private void OnLeaderboardShow(bool p_FirstActivation)
        {
            if (p_FirstActivation)
                m_CurrentBeatmap = Resources.FindObjectsOfTypeAll<LevelCollectionNavigationController>()[0].selectedDifficultyBeatmap;
            else
                SetLeaderboard(GuildSaberLeaderboardPanel.PanelInstance.m_SelectedGuild, true);

            if (m_CurrentBeatmap == null) { SetLeaderboardViewMode(ELeaderboardViewMode.Error); return; }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On beatmap selected
        /// </summary>
        /// <param name="difficultyBeatmap"></param>
        public void OnLeaderboardSet(IDifficultyBeatmap difficultyBeatmap)
        {
            GuildSaberLeaderboardView.m_CurrentBeatmap = difficultyBeatmap;

            Page = 1;
            if (GuildSaberCustomLeaderboard.IsShown)
                SetLeaderboard(GuildSaberLeaderboardPanel.PanelInstance.m_SelectedGuild, false);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Set leaderboard from current beatmap
        /// </summary>
        /// <param name="p_Guild">Guild Id</param>
        public async void SetLeaderboard(int p_Guild, bool p_WithDelay, int p_Page = 0)
        {
            if (!GuildSaberCustomLeaderboard.Initialized) return;

            if (GuildSaberModule.IsStateError())
            {
                m_ErrorText.SetTextError(new System.Exception($"Error during getting player data : {GuildSaberModule.ModErrorState}"), GuildSaberUtils.ErrorMode.Message);
                SetLeaderboardViewMode(ELeaderboardViewMode.Error);
                CustomLevelStatsView.Clear();
                return;
            }

            await WaitUtils.WaitUntil(() => gameObject.activeInHierarchy, 10);

            SetLeaderboardViewMode(ELeaderboardViewMode.Loading);

            if (m_CurrentBeatmap == null) { SetLeaderboardViewMode(ELeaderboardViewMode.Error); return; }

            string l_Hash = GSBeatmapUtils.DifficultyBeatmapToHash(m_CurrentBeatmap);
            string l_Country =
                (GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData.Country != string.Empty && m_SelectedScope == ELeaderboardScope.Country) ?
                GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData.Country : string.Empty;
            int l_Page = 0;

            if (p_Page == -2)
                l_Page = CalculatePageByRank((int)m_Leaderboard.PlayerScore?.Rank);
            else if (p_Page <= 0)
                l_Page = Page;
            else
                l_Page = p_Page;
            if (l_Page != Page)
                Page = l_Page;

            if (m_SelectedScope == ELeaderboardScope.Country) { l_Country = GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData.Country; }

            GSLogger.Instance.Log(m_CurrentBeatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName, IPA.Logging.Logger.LogLevel.InfoUp);
            if (m_CurrentPointsName == string.Empty)
                m_CurrentPointsName = GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData.RankData[0].PointsName;

            m_Leaderboard = await GuildApi.GetLeaderboard(p_Guild, l_Hash, m_CurrentBeatmap, l_Page, GuildSaberModule.m_GSPlayerId ?? 0, p_Country: l_Country, p_Mode: m_CurrentBeatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName, 10, PointsType.GetPointsIDByName(GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData, GuildSaberModule.m_LeaderboardSelectedGuild, m_CurrentPointsName));

            m_ScopeSelector.m_AroundImage.gameObject.SetActive(m_Leaderboard.PlayerScore != null);

            if (m_Leaderboard.Equals(default(ApiMapLeaderboardCollectionStruct)) || m_Leaderboard.Leaderboards == null || m_Leaderboard.Metadata.Equals(null)) { SetLeaderboardViewMode(ELeaderboardViewMode.NotRanked); SetHeader(true); return; }
            else if (!m_Leaderboard.Equals(default(ApiMapLeaderboardCollectionStruct)) && m_Leaderboard.Leaderboards.Count == 0)
            {
                SetLeaderboardViewMode(ELeaderboardViewMode.Unpassed);
                ChangeHeaderText($"Level {m_Leaderboard.CustomData.LevelValue} - {m_Leaderboard.CustomData.CategoryName.VerifiedCategory()}");
                m_PageDownImage.interactable = false;
                if (Page == 1)
                    m_PageUpImage.interactable = false;
                else if (Page > 1)
                    m_PageUpImage.interactable = true;
                SetHeader(false);
                CustomLevelStatsView.Clear();
                return;
            }

            m_PageDownImage.interactable = Page != m_Leaderboard.Metadata.MaxPage;
            m_PageUpImage.interactable = Page > 1;

            m_ScoresList.SetScores(m_Leaderboard.CustomData, m_Leaderboard.Leaderboards, m_CurrentPointsName);

            if (m_ScopeSelector.m_AroundImage.gameObject.activeInHierarchy)
            {
                CustomLevelStatsView.SetModalInfo((int)m_Leaderboard.PlayerScore?.BadCuts, (int)m_Leaderboard.PlayerScore?.MissedNotes, (m_Leaderboard.PlayerScore?.HasScoreStatistic ?? false) ? (int)m_Leaderboard.PlayerScore?.ScoreStatistic?.PauseCount : null, (EHMD)m_Leaderboard.PlayerScore?.HMD);
                CustomLevelStatsView.Init((int)m_Leaderboard.PlayerScore?.Rank,
                    m_Leaderboard.PlayerScore?.Name,
                    (PassState.EState)m_Leaderboard.PlayerScore?.State);
            }
            else
            {
                CustomLevelStatsView.Clear();
            }

            if (p_WithDelay)
                await Task.Delay(700);

            SetHeader(false);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Change header to Level of the current map and the category
        /// </summary>
        public void SetHeader(bool p_Unranked)
        {
            if (!GuildSaberCustomLeaderboard.IsShown) return;

            if (p_Unranked)
                ChangeHeaderText(Localization.Get("TITLE_HIGHSCORES"));
            else
                ChangeHeaderText($"Level {m_Leaderboard.CustomData.LevelValue} - {m_Leaderboard.CustomData.CategoryName.VerifiedCategory()}");

            GuildSaberLeaderboardPanel.PanelInstance.SetColors();
        }

        /// <summary>
        /// Change leaderboard Header
        /// </summary>
        /// <param name="p_Text"></param>
        public void ChangeHeaderText(string p_Text)
        {
            if (GSConfig.Instance.UwUMode)
                p_Text += " `(*>﹏<*)′";

            LeaderboardHeaderManager.ChangeText(p_Text);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Event on scope selected
        /// </summary>
        /// <param name="p_Scope"></param>
        private void OnScopeSelected(ELeaderboardScope p_Scope)
        {
            m_SelectedScope = p_Scope;

            int l_Page = (p_Scope == ELeaderboardScope.Around) ? -2 : 1;

            SetLeaderboard(GuildSaberLeaderboardPanel.PanelInstance.m_SelectedGuild, false, l_Page);
        }

        /// <summary>
        /// On guild is selected
        /// </summary>
        /// <param name="p_Guild"></param>
        private async void OnGuildSelected(int p_Guild)
        {
            await WaitUtils.WaitUntil(() => m_CurrentBeatmap != null, 10);
            SetLeaderboard(p_Guild, false);
        }

        /// <summary>
        /// On other points type selected
        /// </summary>
        /// <param name="p_PointsName"></param>
        private void OnPointsTypeChange(string p_PointsName)
        {
            m_CurrentPointsName = p_PointsName;
            if (m_ScoresList.gameObject.activeInHierarchy)
                m_ScoresList.SetScores(m_Leaderboard.CustomData, m_Leaderboard.Leaderboards, m_CurrentPointsName);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// When page up button pressed
        /// </summary>
        [UIAction("PageUp")]
        private void PageUp()
        {
            Page -= 1;
            SetLeaderboard(GuildSaberLeaderboardPanel.PanelInstance.m_SelectedGuild, false, Page);
        }

        /// <summary>
        /// When page down button pressed
        /// </summary>
        [UIAction("PageDown")]
        private void PageDown()
        {
            Page += 1;
            SetLeaderboard(GuildSaberLeaderboardPanel.PanelInstance.m_SelectedGuild, false, Page);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Set leaderboard mode
        /// </summary>
        /// <param name="p_Mode"></param>
        public void SetLeaderboardViewMode(ELeaderboardViewMode p_Mode)
        {
            m_ScoreParamsLayout.gameObject.SetActive(p_Mode == ELeaderboardViewMode.Scores);
            m_ScopeSelectionLayout.gameObject.SetActive(p_Mode == ELeaderboardViewMode.Scores || (p_Mode == ELeaderboardViewMode.Unpassed && m_SelectedScope != ELeaderboardScope.Global));
            m_NotRankedText.gameObject.SetActive(p_Mode == ELeaderboardViewMode.Unpassed || p_Mode == ELeaderboardViewMode.NotRanked);
            m_ErrorText.gameObject.SetActive(p_Mode == ELeaderboardViewMode.Error);
            m_LoadingLayout.gameObject.SetActive(p_Mode == ELeaderboardViewMode.Loading);
            m_PageDownImage.gameObject.SetActive(p_Mode == ELeaderboardViewMode.Scores);
            m_PageUpImage.gameObject.SetActive(p_Mode == ELeaderboardViewMode.Scores);
            switch (p_Mode)
            {
                case ELeaderboardViewMode.NotRanked:
                    m_NotRankedText.SetText("Map not ranked");
                    m_NotRankedText.color = Color.red;
                    LeaderboardHeaderManager.ChangeText(Localization.Get("TITLE_HIGHSCORES"));
                    break;
                case ELeaderboardViewMode.Unpassed:
                    LeaderboardHeaderManager.ChangeText(Localization.Get("TITLE_HIGHSCORES"));
                    m_NotRankedText.SetText("Map unpassed");
                    m_NotRankedText.color = Color.yellow;
                    break;
            }
        }

        /// <summary>
        /// Get a page from a rank
        /// </summary>
        /// <param name="p_Rank"></param>
        /// <returns></returns>
        private static int CalculatePageByRank(int p_Rank)
        {
            if (p_Rank % GuildSaberModule.SCORES_BY_PAGE != 0)
                return (p_Rank / GuildSaberModule.SCORES_BY_PAGE) + 1;

            return (p_Rank / GuildSaberModule.SCORES_BY_PAGE);
        }
    }

    public enum ELeaderboardViewMode
    {
        Scores, NotRanked, Unpassed, Loading, Error
    }
}
