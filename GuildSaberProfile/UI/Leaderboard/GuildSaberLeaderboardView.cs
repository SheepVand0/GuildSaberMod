using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberMarkupLanguage.Components;
using GuildSaberProfile.API;
using GuildSaberProfile.Utils;
using GuildSaberProfile.UI.Components;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

/*
*
*
*Fix leaderboard Panel not working correctly, still stay in loading mode,
*add a not ranked map text
*fix leaderboard cell distance
*
*/
namespace GuildSaberProfile.UI.GuildSaber.Leaderboard
{
    [HotReload(RelativePathToLayout = @"LeaderboardView.bsml")]
    [ViewDefinition("GuildSaberProfile.UI.GuildSaber.View.LeaderboardView.bsml")]
    public class GuildSaberLeaderboardView : BSMLAutomaticViewController
    {
        [UIComponent("VerticalElems")] VerticalLayoutGroup m_VerticalElems = null;
        [UIComponent("ScoreParamsLayout")] HorizontalLayoutGroup m_ScoreParamsLayout = null;
        [UIComponent("NotRankedText")] TextMeshProUGUI m_NotRankedText = null;
        [UIComponent("ErrorText")] TextMeshProUGUI m_ErrorText = null;

        private GuildSaberLeaderboardPanel _LeaderboardPanel;
        //private LevelSelectionNavigationController

        public string m_CurrentPointsName { get; internal set; }
        public string m_CurrentMapHash { get; internal set; }
        public static IDifficultyBeatmap m_CurrentBeatmap { get; internal set; }
        public ApiMapLeaderboardCollectionStruct m_Leaderboard { get; private set; }

        LeaderboardScoreList m_ScoresList = null;

        [UIAction("#post-parse")]
        private void PostParse()
        {
            Plugin.Log.Info("Creating GuildSaber leaderboard view");

            _LeaderboardPanel = Resources.FindObjectsOfTypeAll<GuildSaberLeaderboardPanel>()[0];
            m_ScoresList = CustomUIComponent.CreateItem<LeaderboardScoreList>(m_ScoreParamsLayout.transform, true, true);

            BindEvents();

            Events.m_Instance.EventOnPostLoadLeaderboard();
        }

        private void BindEvents()
        {
            if (Events.m_Instance == null) { Plugin.Log.Error("Events manager is null"); return; }

            Events.e_OnLeaderboardShown += OnLeaderboardShow;
            Events.e_OnBeatmapSelected += OnBeatmapSelected;
            Events.e_OnBeatmapSelected_NoReturned += _OnBeatmapSelected;
            Events.m_Instance.e_OnPointsTypeChange += OnPointsTypeChange;
            Events.m_Instance.e_OnGuildSelected += OnGuildSelected;
        }

        private void _OnBeatmapSelected()
        {

        }

        public void GetLeaderboard(string p_Guild)
        {
            Plugin.Log.Info("Getting Leaderboard");
            if (m_CurrentBeatmap == null) { SetLeaderboardViewMode(LeaderboardViewMode.Error); return; }
            string l_Hash = GSBeatmapUtils.DifficultyBeatmapToHash(m_CurrentBeatmap);
            m_Leaderboard = GuildApi.GetLeaderboard(p_Guild, l_Hash, m_CurrentBeatmap,0, null, null, 10);
            if (m_Leaderboard.Leaderboards == null) { SetLeaderboardViewMode(LeaderboardViewMode.NotRanked); return; }
            Plugin.Log.Info(m_Leaderboard.Leaderboards.Count.ToString() + " Scores found");
            m_ScoresList.SetScores(m_Leaderboard.CustomData, m_Leaderboard.Leaderboards, m_CurrentPointsName);
            SetLeaderboardViewMode(LeaderboardViewMode.Scores);
        }

        #region Events
        private void OnBeatmapSelected(StandardLevelDetailViewController p_LevelDetailViewController, IDifficultyBeatmap p_Map)
        {
            m_CurrentBeatmap = p_Map;

            if (Events.m_IsGuildSaberLeaderboardShown)
                GetLeaderboard(_LeaderboardPanel.m_SelectedGuild);
        }

        private void OnLeaderboardShow(bool p_FirstActivation)
        {
            if (p_FirstActivation)
                m_CurrentBeatmap = Resources.FindObjectsOfTypeAll<LevelCollectionNavigationController>()[0].selectedDifficultyBeatmap;

            if (m_CurrentBeatmap == null) { SetLeaderboardViewMode(LeaderboardViewMode.Error); return; }

            try
            {
                GetLeaderboard(_LeaderboardPanel.m_SelectedGuild);
            }
            catch (Exception p_E)
            {
                SetLeaderboardViewMode(LeaderboardViewMode.Error);
                m_ErrorText.SetTextError(p_E, GuildSaberUtils.ErrorMode.Message);
            }
        }

        private void OnGuildSelected(string p_Guild)
        {
            GetLeaderboard(p_Guild);
        }

        private void OnPointsTypeChange(string p_PointsName)
        {
            m_CurrentPointsName = p_PointsName;
            m_ScoresList.SetScores(m_Leaderboard.CustomData, m_Leaderboard.Leaderboards, m_CurrentPointsName);
        }
        #endregion

        #region Other
        public void SetLeaderboardViewMode(LeaderboardViewMode p_Mode)
        {
            switch (p_Mode)
            {
                case LeaderboardViewMode.Scores:
                    m_ScoreParamsLayout.gameObject.SetActive(true);
                    m_NotRankedText.gameObject.SetActive(false);
                    m_ErrorText.gameObject.SetActive(false);
                    break;
                case LeaderboardViewMode.NotRanked:
                    m_ScoreParamsLayout.gameObject.SetActive(false);
                    m_NotRankedText.gameObject.SetActive(true);
                    m_ErrorText.gameObject.SetActive(false);
                    break;
                case LeaderboardViewMode.Error:
                    m_ScoreParamsLayout.gameObject.SetActive(false);
                    m_NotRankedText.gameObject.SetActive(false);
                    m_ErrorText.gameObject.SetActive(true);
                    break;
                default: return;
            }
        }
        #endregion
    }

    public enum LeaderboardViewMode
    {
        Scores, NotRanked, Error
    }
}
