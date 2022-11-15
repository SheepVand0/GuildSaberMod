using System;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using GuildSaber.API;
using GuildSaber.BSPModule;
using GuildSaber.Configuration;
using GuildSaber.Logger;
using GuildSaber.UI.Leaderboard.Components;
using GuildSaber.Utils;
using LeaderboardCore.Interfaces;
using Polyglot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GuildSaber.UI.Leaderboard
{
    [HotReload(RelativePathToLayout = @"LeaderboardView.bsml")]
    [ViewDefinition("GuildSaber.UI.GuildSaber.View.LeaderboardView.bsml")]
    internal class GuildSaberLeaderboardView : BSMLAutomaticViewController, INotifyLeaderboardSet
    {

        [UIComponent("ScoreParamsLayout")] readonly VerticalLayoutGroup m_ScoreParamsLayout = null;
        [UIComponent("WorldSelection")] readonly VerticalLayoutGroup m_ScopeSelectionLayout = null;
        [UIComponent("PageUpImage")] readonly Button m_PageUpImage = null;
        [UIComponent("PageDownImage")] readonly Button m_PageDownImage = null;

        [UIComponent("NotRankedText")] readonly TextMeshProUGUI m_NotRankedText = null;
        [UIComponent("ErrorText")] readonly TextMeshProUGUI m_ErrorText = null;
        [UIComponent("Loading")] readonly GridLayoutGroup m_LoadingLayout = null;

        LeaderboardScoreList m_ScoresList = null;
        ScopeSelector m_ScopeSelector = null;
        public CustomLevelStatsView m_CustomLevelStatsView = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public static GuildSaberLeaderboardView m_Instance = null;

        private ELeaderboardScope m_SelectedScope = ELeaderboardScope.Global;

        public string CurrentPointsName { get; internal set; } = string.Empty;
        public string CurrentMapHash { get; internal set; } = string.Empty;
        public static IDifficultyBeatmap? CurrentBeatMap { get; internal set; } = null;
        public ApiMapLeaderboardCollectionStruct Leaderboard { get; private set; } = default(ApiMapLeaderboardCollectionStruct);
        public int m_Page = 1;

        public static bool Initialized { get; private set; } = false;

        public bool ChangingLeaderboard { get; private set; } = false;

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
            m_CustomLevelStatsView = CustomUIComponent.CreateItem<CustomLevelStatsView>(m_ScoreParamsLayout.transform, true, true);
            m_ScopeSelector = CustomUIComponent.CreateItem<ScopeSelector>(m_ScopeSelectionLayout.transform, true, true);

            BindEvents();

            m_Instance = this;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Bind all leaderboard events
        /// </summary>
        private async void BindEvents()
        {
            if (Events.Instance == null)
            {
                GSLogger.Instance.Error(new Exception("Events manager is null"), nameof(GuildSaberLeaderboardView), nameof(BindEvents));
                return;
            }

            await WaitUtils.Wait(() => GuildSaberLeaderboardPanel.PanelInstance != null, 10);

            Events.e_OnLeaderboardShown += OnLeaderboardShow;
            Events.Instance.e_OnPointsTypeChange += OnPointsTypeChange;
            Events.Instance.e_OnGuildSelected += OnGuildSelected;
            Events.Instance.e_OnScopeSelected += OnScopeSelected;
            Events.e_OnLeaderboardPostLoad += async () =>
            {
                await WaitUtils.Wait(() => CurrentBeatMap != null, 100, 100, 10);
                if (!gameObject.activeInHierarchy) return;
                SetLeaderboard(GuildSaberLeaderboardPanel.PanelInstance.m_SelectedGuild, false);
            };

            Initialized = true;
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
                CurrentBeatMap = Resources.FindObjectsOfTypeAll<LevelCollectionNavigationController>()[0].selectedDifficultyBeatmap;
            else
                SetLeaderboard(GuildSaberLeaderboardPanel.PanelInstance.m_SelectedGuild, true);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On beat map selected
        /// </summary>
        /// <param name="difficultyBeatmap"></param>
        public async void OnLeaderboardSet(IDifficultyBeatmap difficultyBeatmap)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (difficultyBeatmap == null) return;

            CurrentBeatMap = difficultyBeatmap;

            await WaitUtils.Wait(() => GuildSaberLeaderboardPanel.PanelInstance != null, 100, 0, 10);

            m_Page = 1;
            if (GuildSaberCustomLeaderboard.IsShown)
                SetLeaderboard(GuildSaberLeaderboardPanel.PanelInstance.m_SelectedGuild, false);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Set leaderboard from current beat map
        /// </summary>
        /// <param name="p_Guild">Guild Id</param>
        public async void SetLeaderboard(int p_Guild, bool p_NeedUpdate, int p_Page = 0)
        {
            if (!GuildSaberCustomLeaderboard.Initialized) return;

            ChangingLeaderboard = true;

            if (m_ScoresList == null)
                await WaitUtils.Wait(() => m_ScoresList != null, 10);

            if (GuildSaberModule.IsStateError())
            {
                m_ErrorText.SetTextError(new Exception($"Error during getting player data : {GuildSaberModule.ModErrorState}"), GuildSaberUtils.ErrorMode.Message);
                SetLeaderboardViewMode(ELeaderboardViewMode.Error);
                m_CustomLevelStatsView.Clear();
                return;
            }

            await WaitUtils.Wait(() => gameObject.activeInHierarchy, 10);

            SetLeaderboardViewMode(ELeaderboardViewMode.Loading);

            if (CurrentBeatMap == null)
            {
                SetLeaderboardViewMode(ELeaderboardViewMode.Error);
                ChangingLeaderboard = false;
                return;
            }

            string l_Hash = GSBeatmapUtils.DifficultyBeatmapToHash(CurrentBeatMap);
            string l_Country =
                (GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData.Country != string.Empty && m_SelectedScope == ELeaderboardScope.Country) ? GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData.Country : string.Empty;

            int l_Page = p_Page switch
            {
                -2 => CalculatePageByRank((int)Leaderboard.PlayerScore?.Rank!),
                <= 0 => m_Page,
                _ => p_Page
            };

            m_Page = l_Page;

            if (m_SelectedScope == ELeaderboardScope.Country)
            {
                l_Country = GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData.Country;
            }

            if (CurrentPointsName == string.Empty)
                CurrentPointsName = GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData.RankData[0].PointsName;

            Leaderboard = await GuildApi.GetLeaderboard(p_Guild, l_Hash, CurrentBeatMap, l_Page, GuildSaberModule.GSPlayerId ?? 0, p_Country: l_Country, p_Mode: CurrentBeatMap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName, 10, PointsType.GetPointsIDByName(GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData, GuildSaberModule.LeaderboardSelectedGuild, CurrentPointsName));

            m_ScopeSelector.m_AroundImage.gameObject.SetActive(Leaderboard.PlayerScore != null);

            if (Leaderboard.Equals(default(ApiMapLeaderboardCollectionStruct)) || Leaderboard.Leaderboards == null || Leaderboard.Metadata.Equals(null))
            {
                SetLeaderboardViewMode(ELeaderboardViewMode.NotRanked);
                SetHeader(true);
                ChangingLeaderboard = false;
                return;
            }
            else if (!Leaderboard.Equals(default(ApiMapLeaderboardCollectionStruct)) && Leaderboard.Leaderboards.Count == 0)
            {
                SetLeaderboardViewMode(ELeaderboardViewMode.UnPassed);
                ChangeHeaderText($"Level {Leaderboard.CustomData.LevelValue} - {Leaderboard.CustomData.CategoryName.VerifiedCategory()}");
                m_PageDownImage.interactable = false;
                if (m_Page == 1)
                    m_PageUpImage.interactable = false;
                else if (m_Page > 1)
                    m_PageUpImage.interactable = true;
                SetHeader(false);
                m_CustomLevelStatsView.Clear();
                ChangingLeaderboard = false;
                return;
            }

            m_PageDownImage.interactable = m_Page != Leaderboard.Metadata.MaxPage;
            m_PageUpImage.interactable = m_Page > 1;

            m_ScoresList.SetScores(Leaderboard.CustomData, Leaderboard.Leaderboards, CurrentPointsName);

            if (m_ScopeSelector.m_AroundImage.gameObject.activeInHierarchy)
            {
                m_CustomLevelStatsView.SetModalInfo((int)Leaderboard.PlayerScore?.BadCuts!, (int)Leaderboard.PlayerScore?.MissedNotes!,
                    (Leaderboard.PlayerScore?.HasScoreStatistic ?? false) ? (int)Leaderboard.PlayerScore?.ScoreStatistic?.PauseCount! : null,
                    (EHMD)Leaderboard.PlayerScore?.HMD!);

                m_CustomLevelStatsView.Init((int)Leaderboard.PlayerScore?.Rank!,
                    Leaderboard.PlayerScore?.Name,
                    (PassState.EState)Leaderboard.PlayerScore?.State!);
            }
            else
            {
                m_CustomLevelStatsView.Clear();
            }

            if (p_NeedUpdate)
            {
                await WaitUtils.Wait(() =>
                {
                    SetHeader(false);
                    return false;
                }, 1, 0, 900);
            }
            else
            {
                SetHeader(false);
            }

            ChangingLeaderboard = false;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Change header to Level of the current map and the category
        /// </summary>
        public void SetHeader(bool p_Unranked)
        {
            if (!GuildSaberCustomLeaderboard.IsShown) return;

            ChangeHeaderText(p_Unranked ? Localization.Get("TITLE_HIGHSCORES") : $"Level {Leaderboard.CustomData.LevelValue} - {Leaderboard.CustomData.CategoryName.VerifiedCategory()}");

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

            if (Leaderboard.Leaderboards == null) return;
            else if (Leaderboard.Leaderboards.Count == 0) return;

            SetLeaderboard(GuildSaberLeaderboardPanel.PanelInstance.m_SelectedGuild, false, l_Page);
        }

        /// <summary>
        /// On guild is selected
        /// </summary>
        /// <param name="p_Guild"></param>
        private async void OnGuildSelected(int p_Guild)
        {
            await WaitUtils.Wait(() => CurrentBeatMap != null, 10);
            SetLeaderboard(p_Guild, false);
        }

        /// <summary>
        /// On other points type selected
        /// </summary>
        /// <param name="p_PointsName"></param>
        private void OnPointsTypeChange(string p_PointsName)
        {
            CurrentPointsName = p_PointsName;

            if (Leaderboard.Leaderboards == null) return;
            else if (Leaderboard.Leaderboards.Count == 0) return;

            if (m_ScoresList.gameObject.activeInHierarchy)
                m_ScoresList.SetScores(Leaderboard.CustomData, Leaderboard.Leaderboards, CurrentPointsName);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// When page up button pressed
        /// </summary>
        [UIAction("PageUp")]
        private void PageUp()
        {
            m_Page -= 1;
            SetLeaderboard(GuildSaberLeaderboardPanel.PanelInstance.m_SelectedGuild, false, m_Page);
        }

        /// <summary>
        /// When page down button pressed
        /// </summary>
        [UIAction("PageDown")]
        private void PageDown()
        {
            m_Page += 1;
            SetLeaderboard(GuildSaberLeaderboardPanel.PanelInstance.m_SelectedGuild, false, m_Page);
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
            m_ScopeSelectionLayout.gameObject.SetActive(p_Mode == ELeaderboardViewMode.Scores || (p_Mode == ELeaderboardViewMode.UnPassed && m_SelectedScope != ELeaderboardScope.Global));
            m_NotRankedText.gameObject.SetActive(p_Mode == ELeaderboardViewMode.UnPassed || p_Mode == ELeaderboardViewMode.NotRanked);
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
                case ELeaderboardViewMode.UnPassed:
                    LeaderboardHeaderManager.ChangeText(Localization.Get("TITLE_HIGHSCORES"));
                    m_NotRankedText.SetText("Map un passed");
                    m_NotRankedText.color = Color.yellow;
                    break;
                case ELeaderboardViewMode.Scores:
                    break;
                case ELeaderboardViewMode.Loading:
                    break;
                case ELeaderboardViewMode.Error:
                    break;
                default:
                    GSLogger.Instance.Error(new Exception("Not Valid"), nameof(GuildSaberLeaderboardView), nameof(SetLeaderboardViewMode));
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
        Scores,
        NotRanked,
        UnPassed,
        Loading,
        Error
    }
}
