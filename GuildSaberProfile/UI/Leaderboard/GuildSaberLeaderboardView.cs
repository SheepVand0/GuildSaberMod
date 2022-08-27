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
using System.Threading.Tasks;
using LeaderboardCore.Interfaces;
using System.Collections.Generic;
using System.Linq;
using GuildSaberProfile.Configuration;
using Polyglot;

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
    public class GuildSaberLeaderboardView : BSMLAutomaticViewController, INotifyLeaderboardSet, INotifyScoreUpload
    {
        #region
        #region Layouts
        [UIComponent("VerticalElems")] VerticalLayoutGroup m_VerticalElems = null;
        [UIComponent("ScoreParamsLayout")] VerticalLayoutGroup m_ScoreParamsLayout = null;
        [UIComponent("WorldSelection")] VerticalLayoutGroup m_ScopeSelectionLayout = null;
        [UIComponent("ElemsHorizontal")] HorizontalLayoutGroup m_HorizontalElems = null;
        [UIComponent("PageUpImage")] ClickableImage m_PageUpImage = null;
        [UIComponent("PageDownImage")] ClickableImage m_PageDownImage = null;
        #endregion
        [UIComponent("NotRankedText")] TextMeshProUGUI m_NotRankedText = null;
        [UIComponent("ErrorText")] TextMeshProUGUI m_ErrorText = null;
        [UIComponent("Loading")] GridLayoutGroup m_LoadingLayout = null;
        LeaderboardScoreList m_ScoresList = null;
        ScopeSelector m_ScopeSelector = null;
        #endregion

        public static GuildSaberLeaderboardPanel _LeaderboardPanel;
        private ELeaderboardScope m_SelectedScope = ELeaderboardScope.Global;

        public string m_CurrentPointsName { get; internal set; }
        public string m_CurrentMapHash { get; internal set; }
        public static IDifficultyBeatmap m_CurrentBeatmap { get; internal set; }
        public ApiMapLeaderboardCollectionStruct m_Leaderboard { get; private set; }

        public int Page { get => 1; internal set { } }

        #region Setup
        [UIAction("#post-parse")]
        private void PostParse()
        {
            Plugin.Log.Info("Creating GuildSaber leaderboard view");

            _LeaderboardPanel = Resources.FindObjectsOfTypeAll<GuildSaberLeaderboardPanel>()[0];
            m_ScoresList = CustomUIComponent.CreateItem<LeaderboardScoreList>(m_ScoreParamsLayout.transform, true, true);
            m_ScopeSelector = CustomUIComponent.CreateItem<ScopeSelector>(m_ScopeSelectionLayout.transform, true, true);

            BindEvents();

            Events.m_Instance.EventOnPostLoadLeaderboard();
        }

        private void BindEvents()
        {
            if (Events.m_Instance == null) { Plugin.Log.Error("Events manager is null"); return; }

            Events.e_OnLeaderboardShown += OnLeaderboardShow;
            Events.m_Instance.e_OnPointsTypeChange += OnPointsTypeChange;
            Events.m_Instance.e_OnGuildSelected += OnGuildSelected;
            Events.m_Instance.e_OnScopeSelected += OnScopeSelected;
        }
        #endregion

        #region Leaderboard
        public async void GetLeaderboard(string p_Guild)
        {
            try
            {
                SetLeaderboardViewMode(ELeaderboardViewMode.Loading);
                if (m_CurrentBeatmap == null) { SetLeaderboardViewMode(ELeaderboardViewMode.Error); return; }
                string l_Hash = GSBeatmapUtils.DifficultyBeatmapToHash(m_CurrentBeatmap);
                string l_Id = "null";
                string l_Country = "null";
                string l_Page = Page.ToString();
                if (m_SelectedScope == ELeaderboardScope.Around) { l_Id = Plugin.m_PlayerId; }
                if (m_SelectedScope == ELeaderboardScope.Country) { l_Country = _LeaderboardPanel.m_PlayerGuildsInfo.m_ReturnPlayer.Country; }
                await Task.Run(delegate
                {
                    m_Leaderboard = GuildApi.GetLeaderboard(p_Guild, l_Hash, m_CurrentBeatmap, l_Page, l_Id, p_Country: l_Country, "10");
                });

                if (m_Leaderboard.Leaderboards is null) { SetLeaderboardViewMode(ELeaderboardViewMode.NotRanked); return; }
                else if (!m_Leaderboard.Leaderboards.Any())
                {
                    SetLeaderboardViewMode(ELeaderboardViewMode.Unpassed);
                    ChangeHeaderText($"Level {m_Leaderboard.CustomData.Level} - {m_Leaderboard.CustomData.Category.VerifiedCategory()}");
                    return;
                }

                ChangeHeaderText($"Level {m_Leaderboard.CustomData.Level} - {m_Leaderboard.CustomData.Category.VerifiedCategory()}");

                /*m_PageUpImage.enabled = true;
                m_PageDownImage.enabled = false;

                if (Page == m_Leaderboard.Metadata.MaxPage)
                    m_PageDownImage.enabled = false;
                if (Page == 1)
                    m_PageUpImage.enabled = true;*/

                m_ScoresList.SetScores(m_Leaderboard.CustomData, m_Leaderboard.Leaderboards, m_CurrentPointsName);
                SetLeaderboardViewMode(ELeaderboardViewMode.Scores);
            }
            catch (Exception l_Ex)
            {
                m_ErrorText.SetTextError(l_Ex, GuildSaberUtils.ErrorMode.Message);
                SetLeaderboardViewMode(ELeaderboardViewMode.Error);
                Plugin.Log.Error(l_Ex.StackTrace);
            }
        }

        public async void ChangeHeaderText(string p_Text)
        {
            if (_LeaderboardPanel.m_HeaderManager == null) _LeaderboardPanel.m_HeaderManager = Resources.FindObjectsOfTypeAll<LeaderboardHeaderManager>()[0];
            if (PluginConfig.Instance.UwUMode)
                p_Text += " `(*>﹏<*)′";

            _LeaderboardPanel.m_HeaderManager.ChangeText(p_Text);
        }
        #endregion

        #region Events
        private void OnScopeSelected(ELeaderboardScope p_Scope)
        {
            m_SelectedScope = p_Scope;
            GetLeaderboard(_LeaderboardPanel.m_SelectedGuild);
        }
        public void OnLeaderboardSet(IDifficultyBeatmap difficultyBeatmap)
        {
            m_CurrentBeatmap = difficultyBeatmap;
            if (Events.m_IsGuildSaberLeaderboardShown)
                GetLeaderboard(_LeaderboardPanel.m_SelectedGuild);
        }

        private void OnLeaderboardShow(bool p_FirstActivation)
        {
            if (p_FirstActivation)
                m_CurrentBeatmap = Resources.FindObjectsOfTypeAll<LevelCollectionNavigationController>()[0].selectedDifficultyBeatmap;

            if (m_CurrentBeatmap == null) { SetLeaderboardViewMode(ELeaderboardViewMode.Error); return; }

            GetLeaderboard(_LeaderboardPanel.m_SelectedGuild);
        }

        public void OnScoreUploaded()
        {

        }
        private void OnGuildSelected(string p_Guild) { GetLeaderboard(p_Guild); }

        private void OnPointsTypeChange(string p_PointsName)
        {
            m_CurrentPointsName = p_PointsName;
            if (m_ScoresList.gameObject.activeInHierarchy)
                m_ScoresList.SetScores(m_Leaderboard.CustomData, m_Leaderboard.Leaderboards, m_CurrentPointsName);
        }

        [UIAction("PageUp")]
        private void PageUp()
        {
            Page -= 1;
            GetLeaderboard(_LeaderboardPanel.m_SelectedGuild);
        }

        [UIAction("PageDown")]
        private void PageDown()
        {
            Page += 1;
            GetLeaderboard(_LeaderboardPanel.m_SelectedGuild);
        }
        #endregion

        #region Other
        public async void SetLeaderboardViewMode(ELeaderboardViewMode p_Mode)
        {
            switch (p_Mode)
            {
                case ELeaderboardViewMode.Scores:
                    m_ScoreParamsLayout.gameObject.SetActive(true);
                    m_ScopeSelectionLayout.gameObject.SetActive(true);
                    m_NotRankedText.gameObject.SetActive(false);
                    m_ErrorText.gameObject.SetActive(false);
                    m_LoadingLayout.gameObject.SetActive(false);
                    break;
                case ELeaderboardViewMode.NotRanked:
                    m_ScoreParamsLayout.gameObject.SetActive(false);
                    m_ScopeSelectionLayout.gameObject.SetActive(false);
                    m_NotRankedText.gameObject.SetActive(true);
                    m_ErrorText.gameObject.SetActive(false);
                    m_LoadingLayout.gameObject.SetActive(false);
                    m_NotRankedText.SetText("Map not ranked");
                    m_NotRankedText.color = Color.red;
                    _LeaderboardPanel.m_HeaderManager.ChangeText(Localization.Get("TITLE_HIGHSCORES"));
                    break;
                case ELeaderboardViewMode.Unpassed:
                    m_ScoreParamsLayout.gameObject.SetActive(false);
                    m_ScopeSelectionLayout.gameObject.SetActive(true);
                    m_NotRankedText.gameObject.SetActive(true);
                    m_ErrorText.gameObject.SetActive(false);
                    m_LoadingLayout.gameObject.SetActive(false);
                    m_NotRankedText.SetText("Map unpassed");
                    m_NotRankedText.color = Color.yellow;
                    break;
                case ELeaderboardViewMode.Loading:
                    m_ScoreParamsLayout.gameObject.SetActive(false);
                    m_ScopeSelectionLayout.gameObject.SetActive(false);
                    m_NotRankedText.gameObject.SetActive(false);
                    m_ErrorText.gameObject.SetActive(false);
                    m_LoadingLayout.gameObject.SetActive(true);
                    break;
                case ELeaderboardViewMode.Error:
                    m_ScoreParamsLayout.gameObject.SetActive(false);
                    m_ScopeSelectionLayout.gameObject.SetActive(false);
                    m_NotRankedText.gameObject.SetActive(false);
                    m_ErrorText.gameObject.SetActive(true);
                    m_LoadingLayout.gameObject.SetActive(false);
                    break;
                default: return;
            }
        }
        #endregion
    }

    public enum ELeaderboardViewMode
    {
        Scores, NotRanked, Unpassed, Loading, Error
    }
}
