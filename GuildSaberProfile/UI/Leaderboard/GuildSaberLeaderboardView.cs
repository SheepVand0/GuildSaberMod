using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberMarkupLanguage.Components;
using GuildSaber.API;
using GuildSaber.Utils;
using GuildSaber.UI.Components;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;
using System.Threading.Tasks;
using LeaderboardCore.Interfaces;
using System.Collections.Generic;
using System.Linq;
using GuildSaber.Configuration;
using Polyglot;
using System.Collections;

/*
*
*
*Fix leaderboard Panel not working correctly, still stay in loading mode,
*add a not ranked map text
*fix leaderboard cell distance
*
*/
namespace GuildSaber.UI.GuildSaber.Leaderboard
{
    [HotReload(RelativePathToLayout = @"LeaderboardView.bsml")]
    [ViewDefinition("GuildSaber.UI.GuildSaber.View.LeaderboardView.bsml")]
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

        public static GuildSaberLeaderboardView m_Instance;

        private ELeaderboardScope m_SelectedScope = ELeaderboardScope.Global;

        public string m_CurrentPointsName { get; internal set; }
        public string m_CurrentMapHash { get; internal set; }
        public static IDifficultyBeatmap m_CurrentBeatmap { get; internal set; }
        public ApiMapLeaderboardCollection m_Leaderboard { get; private set; }
        public int Page { get => 1; internal set { } }
        #region Setup
        [UIAction("#post-parse")]
        private void PostParse()
        {
            //Plugin.Log.Info("Creating GuildSaber leaderboard view");
            m_ScoresList = CustomUIComponent.CreateItem<LeaderboardScoreList>(m_ScoreParamsLayout.transform, true, true);
            m_ScopeSelector = CustomUIComponent.CreateItem<ScopeSelector>(m_ScopeSelectionLayout.transform, true, true);

            BindEvents();

            m_Instance = this;

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
        public void GetLeaderboard(int p_Guild)
        {
            StartCoroutine(_GetLeaderboard(p_Guild));
        }
        public IEnumerator _GetLeaderboard(int p_Guild)
        {
            //Plugin.Log.Info(m_CurrentBeatmap.GetEnvironmentInfo().environmentType.typeNameLocalizationKey);
            SetLeaderboardViewMode(ELeaderboardViewMode.Loading);
            if (m_CurrentBeatmap == null) { SetLeaderboardViewMode(ELeaderboardViewMode.Error); yield break; }
            string l_Hash = GSBeatmapUtils.DifficultyBeatmapToHash(m_CurrentBeatmap);
            long l_Id = 0;
            string l_Country =
                (GuildSaberLeaderboardPanel.m_Instance.m_PlayerData.Country != string.Empty && m_SelectedScope == ELeaderboardScope.Country) ?
                GuildSaberLeaderboardPanel.m_Instance.m_PlayerData.Country : string.Empty;
            int l_Page = Page;
            if (m_SelectedScope == ELeaderboardScope.Around) { l_Id = BSPModule.GuildSaber.m_SSPlayerId; }
            if (m_SelectedScope == ELeaderboardScope.Country) { l_Country = GuildSaberLeaderboardPanel.m_Instance.m_PlayerData.Country; }
            m_Leaderboard = GuildApi.GetLeaderboard(p_Guild, l_Hash, m_CurrentBeatmap, l_Page, l_Id, p_Country: l_Country, 10);
            if (m_Leaderboard.Equals(default(ApiMapLeaderboardCollection))) { SetLeaderboardViewMode(ELeaderboardViewMode.NotRanked); yield break; }
            else if (!m_Leaderboard.Equals(default(ApiMapLeaderboardCollection)) && m_Leaderboard.Leaderboards.Count == 0)
            {
                SetLeaderboardViewMode(ELeaderboardViewMode.Unpassed);
                ChangeHeaderText($"Level {m_Leaderboard.CustomData.LevelValue} - {m_Leaderboard.CustomData.CategoryName.VerifiedCategory()}");
                yield break;
            }

            ChangeHeaderText($"Level {m_Leaderboard.CustomData.LevelValue} - {m_Leaderboard.CustomData.CategoryName.VerifiedCategory()}");
            /*m_PageUpImage.enabled = true;
            m_PageDownImage.enabled = false;

            if (Page == m_Leaderboard.Metadata.MaxPage)
                m_PageDownImage.enabled = false;
            if (Page == 1)
                m_PageUpImage.enabled = true;*/

            m_ScoresList.SetScores(m_Leaderboard.CustomData, m_Leaderboard.Leaderboards, m_CurrentPointsName);
            SetLeaderboardViewMode(ELeaderboardViewMode.Scores);
            yield return null;
        }

        public void ChangeHeaderText(string p_Text)
        {
            if (GSConfig.Instance.UwUMode)
                p_Text += " `(*>﹏<*)′";

            LeaderboardHeaderManager.m_Instance.ChangeText(p_Text);
        }
        #endregion

        #region Events
        private void OnScopeSelected(ELeaderboardScope p_Scope)
        {
            m_SelectedScope = p_Scope;
            GetLeaderboard(GuildSaberLeaderboardPanel.m_Instance.m_SelectedGuild);
        }
        public void OnLeaderboardSet(IDifficultyBeatmap difficultyBeatmap)
        {
            m_CurrentBeatmap = difficultyBeatmap;

            if (Events.m_IsGuildSaberLeaderboardShown)
                GetLeaderboard(GuildSaberLeaderboardPanel.m_Instance.m_SelectedGuild);
        }

        private void OnLeaderboardShow(bool p_FirstActivation)
        {
            if (p_FirstActivation)
                m_CurrentBeatmap = Resources.FindObjectsOfTypeAll<LevelCollectionNavigationController>()[0].selectedDifficultyBeatmap;

            if (m_CurrentBeatmap == null) { SetLeaderboardViewMode(ELeaderboardViewMode.Error); return; }
        }

        public void OnScoreUploaded()
        {

        }
        private void OnGuildSelected(int p_Guild) { GetLeaderboard(p_Guild); }

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
            GetLeaderboard(GuildSaberLeaderboardPanel.m_Instance.m_SelectedGuild);
        }

        [UIAction("PageDown")]
        private void PageDown()
        {
            Page += 1;
            GetLeaderboard(GuildSaberLeaderboardPanel.m_Instance.m_SelectedGuild);
        }
        #endregion

        #region Other
        public void SetLeaderboardViewMode(ELeaderboardViewMode p_Mode)
        {
            m_ScoreParamsLayout.gameObject.SetActive(p_Mode == ELeaderboardViewMode.Scores);
            m_ScopeSelectionLayout.gameObject.SetActive(p_Mode == ELeaderboardViewMode.Scores || p_Mode == ELeaderboardViewMode.Unpassed);
            m_NotRankedText.gameObject.SetActive(p_Mode == ELeaderboardViewMode.Unpassed || p_Mode == ELeaderboardViewMode.NotRanked);
            m_ErrorText.gameObject.SetActive(p_Mode == ELeaderboardViewMode.Error);
            m_LoadingLayout.gameObject.SetActive(p_Mode == ELeaderboardViewMode.Loading);
            switch (p_Mode)
            {
                case ELeaderboardViewMode.NotRanked:
                    m_NotRankedText.SetText("Map not ranked");
                    m_NotRankedText.color = Color.red;
                    LeaderboardHeaderManager.m_Instance.ChangeText(Localization.Get("TITLE_HIGHSCORES"));
                    break;
                case ELeaderboardViewMode.Unpassed:
                    LeaderboardHeaderManager.m_Instance.ChangeText(Localization.Get("TITLE_HIGHSCORES"));
                    m_NotRankedText.SetText("Map unpassed");
                    m_NotRankedText.color = Color.yellow;
                    break;
            }
        }
        #endregion
    }

    public enum ELeaderboardViewMode
    {
        Scores, NotRanked, Unpassed, Loading, Error
    }
}
