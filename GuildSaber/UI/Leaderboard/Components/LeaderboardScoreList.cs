using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using GuildSaber.API;
using GuildSaber.BSPModule;
using GuildSaber.Utils;
using TMPro;
using UnityEngine;
using GuildSaber.Configuration;
using GuildSaber.Logger;
using UnityEngine.UI;
using GuildSaber.UI.Leaderboard.Components.SubComponents;

namespace GuildSaber.UI.Leaderboard.Components
{


    class LeaderboardScoreList : CustomUIComponent
    {
        protected override string ViewResourceName => "GuildSaber.UI.Leaderboard.Components.Views.LeaderboardScoreList.bsml";

        [UIComponent("ScoreList")] internal readonly CustomCellListTableData m_ScoreList = null;
        [UIComponent("NotRankedText")] internal readonly TextMeshProUGUI m_NotRankedText = null;
        [UIComponent("ErrorText")] internal readonly TextMeshProUGUI m_ErrorText = null;
        [UIComponent("Loading")] internal readonly GridLayoutGroup m_LoadingLayout = null;

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        [UIValue("ScoreCells")] List<object> m_ListComponentScores = new List<object>();

        public CustomLevelStatsView? m_CustomLevelStatsView = null;

        public bool Initialized { get; private set; }

        private float ListCellSize
        {
            get => 4 * LeaderboardScoreCell.ScaleFactor;
        }

        public ApiMapLeaderboardCollectionStruct Leaderboard { get; private set; } = default(ApiMapLeaderboardCollectionStruct);
        public int m_Page = 1;
        public string CurrentPointsName { get; internal set; } = string.Empty;
        //public string CurrentMapHash { get; internal set; } = string.Empty;
        public static IDifficultyBeatmap? CurrentBeatMap { get; internal set; } = null;

        internal ELeaderboardScope m_SelectedScope = ELeaderboardScope.Global;
        public bool ChangingLeaderboard { get; private set; } = false;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        protected override async void AfterViewCreation()
        {
            m_CustomLevelStatsView = CustomUIComponent.CreateItem<CustomLevelStatsView>(GuildSaberLeaderboardView.m_Instance.m_ScoreParamsLayout.transform, true, true);

            await WaitUtils.Wait(() => GuildSaberLeaderboardPanel.PanelInstance != null, 10);

            CurrentBeatMap = Resources.FindObjectsOfTypeAll<LevelCollectionNavigationController>()[0].selectedDifficultyBeatmap;
            //await WaitUtils.Wait(() => CurrentBeatMap != null, 100, 100, 10);
            /*if (!gameObject.activeInHierarchy) return;*/
            SetLeaderboard(GuildSaberLeaderboardPanel.PanelInstance.m_SelectedGuild, false);

            Initialized = true;

            Events.e_OnLeaderboardShown += OnLeaderboardShow;
            Events.Instance.e_OnPointsTypeChange += OnPointsTypeChange;
            Events.Instance.e_OnGuildSelected += OnGuildSelected;
            Events.Instance.e_OnScopeSelected += OnScopeSelected;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
        ///Events
        /// <summary>
        /// When the leaderboard is shown
        /// </summary>
        /// <param name="p_FirstActivation"></param>
        private void OnLeaderboardShow(bool p_FirstActivation)
        {
            if (p_FirstActivation || !Initialized) return;

            SetLeaderboard(GuildSaberLeaderboardPanel.PanelInstance.m_SelectedGuild, true);
        }

        /// <summary>
        /// On beat map selected
        /// </summary>
        /// <param name="p_DifficultyBeatmap"></param>
        internal void OnLeaderboardSet(IDifficultyBeatmap p_DifficultyBeatmap)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (p_DifficultyBeatmap == null || !Initialized) return;

            CurrentBeatMap = p_DifficultyBeatmap;

            //await WaitUtils.Wait(() => GuildSaberLeaderboardPanel.PanelInstance != null, 100, 0, 10);

            m_Page = 1;
            if (GuildSaberCustomLeaderboard.IsShown)
                SetLeaderboard(GuildSaberLeaderboardPanel.PanelInstance.m_SelectedGuild, false);
        }

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
            if (!Initialized) return;
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

            if (gameObject.activeInHierarchy)
                SetScores(Leaderboard.CustomData, Leaderboard.Leaderboards, CurrentPointsName);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Set leaderboard from current beat map
        /// </summary>
        /// <param name="p_Guild">Guild Id</param>
        public async void SetLeaderboard(int p_Guild, bool p_NeedUpdate, int p_Page = 0)
        {
            ///Change conditions

            if (ChangingLeaderboard) return;

            ChangingLeaderboard = true;

            await WaitUtils.Wait(() => GuildSaberLeaderboardView.m_Instance.gameObject.activeInHierarchy, 10, p_CodeLine: 471);

            if (GuildSaberModule.IsStateError())
            {
                m_ErrorText.SetTextError(new System.Exception($"Error during getting player data : {GuildSaberModule.ModErrorState}"), GuildSaberUtils.ErrorMode.Message);
                GuildSaberLeaderboardView.m_Instance.SetLeaderboardViewMode(ELeaderboardViewMode.Error);
                // ReSharper disable once Unity.NoNullPropagation
                m_CustomLevelStatsView?.Clear();
                ChangingLeaderboard = false;
                return;
            }

            ///Init change

            GuildSaberLeaderboardView.m_Instance.SetLeaderboardViewMode(ELeaderboardViewMode.Loading);

            if (CurrentBeatMap == null)
            {
                GuildSaberLeaderboardView.m_Instance.SetLeaderboardViewMode(ELeaderboardViewMode.Error);
                ChangingLeaderboard = false;
                return;
            }

            ///Initing current map values

            string l_Hash = GSBeatmapUtils.DifficultyBeatmapToHash(CurrentBeatMap);
            string l_Country =
                (GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData.Country != string.Empty && m_SelectedScope == ELeaderboardScope.Country) ? GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData.Country : string.Empty;

            ///Page managing

            int l_Page = p_Page switch
            {
                -2 => GuildSaberUtils.CalculatePageByRank((int)Leaderboard.PlayerScore?.Rank!),
                <= 0 => m_Page,
                _ => p_Page
            };

            m_Page = l_Page;

            ///Scope managing

            if (m_SelectedScope == ELeaderboardScope.Country)
            {
                l_Country = GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData.Country;
            }

            ///Points managing

            if (CurrentPointsName == string.Empty)
                CurrentPointsName = GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData.RankData[0].PointsName;

            ///Getting leaderboard from API

            Leaderboard = await GuildApi.GetLeaderboard(p_Guild, l_Hash, CurrentBeatMap, l_Page, GuildSaberModule.GSPlayerId ?? 0, p_Country: l_Country, p_Mode: CurrentBeatMap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName, 10, PointsType.GetPointsIDByName(GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData, GuildSaberModule.LeaderboardSelectedGuild, CurrentPointsName));

            ///Set around me button active if player have a score on current map

            GuildSaberLeaderboardView.m_Instance.m_ScopeSelector.m_AroundImage.gameObject.SetActive(Leaderboard.PlayerScore != null);


            ///If map not ranked

            if (Leaderboard.Equals(default(ApiMapLeaderboardCollectionStruct)) || Leaderboard.Leaderboards == null || Leaderboard.Metadata.Equals(null))
            {
                GuildSaberLeaderboardView.m_Instance.SetLeaderboardViewMode(ELeaderboardViewMode.NotRanked);
                SetHeader(true);
                ChangingLeaderboard = false;
                return;
            }
            else if (!Leaderboard.Equals(default(ApiMapLeaderboardCollectionStruct)) && Leaderboard.Leaderboards.Count == 0)
            {
                ///If map is un passed
                GuildSaberLeaderboardView.m_Instance.SetLeaderboardViewMode(ELeaderboardViewMode.UnPassed);
                ChangeHeaderText($"Level {Leaderboard.CustomData.LevelValue} - {Leaderboard.CustomData.CategoryName.VerifiedCategory()}");
                GuildSaberLeaderboardView.m_Instance.m_PageDownImage.interactable = false;
                if (m_Page == 1)
                    GuildSaberLeaderboardView.m_Instance.m_PageUpImage.interactable = false;
                else if (m_Page > 1)
                    GuildSaberLeaderboardView.m_Instance.m_PageUpImage.interactable = true;
                SetHeader(false);
                // ReSharper disable once Unity.NoNullPropagation
                m_CustomLevelStatsView?.Clear();
                ChangingLeaderboard = false;
                return;
            }

            ///Page button managing

            GuildSaberLeaderboardView.m_Instance.m_PageDownImage.interactable = m_Page != Leaderboard.Metadata.MaxPage;
            GuildSaberLeaderboardView.m_Instance.m_PageUpImage.interactable = m_Page > 1;

            ///Set scores on ScoreList

            SetScores(Leaderboard.CustomData, Leaderboard.Leaderboards, CurrentPointsName);

            ///If player have a score initing score modal

            try
            {
                if (GuildSaberLeaderboardView.m_Instance.m_ScopeSelector.m_AroundImage.gameObject.activeInHierarchy)
                {
                    // ReSharper disable once Unity.NoNullPropagation
                    m_CustomLevelStatsView?.SetModalInfo((int)Leaderboard.PlayerScore?.BadCuts!, (int)Leaderboard.PlayerScore?.MissedNotes!,
                        (Leaderboard.PlayerScore?.HasScoreStatistic ?? false) ? (int)Leaderboard.PlayerScore?.ScoreStatistic?.PauseCount! : null,
                        (EHMD)Leaderboard.PlayerScore?.HMD!);

                    m_CustomLevelStatsView.Init((int)Leaderboard.PlayerScore?.Rank!,
                        Leaderboard.PlayerScore?.Name,
                        (PassState.EState)Leaderboard.PlayerScore?.State!);
                }
                else
                {
                    // ReSharper disable once Unity.NoNullPropagation
                    m_CustomLevelStatsView?.Clear();
                }
            }
            catch (Exception l_E)
            {
                GSLogger.Instance.Error(l_E, nameof(LeaderboardScoreList), nameof(SetLeaderboard));
            }

            ///If need update changing header

            if (p_NeedUpdate)
            {
                await WaitUtils.Wait(() =>
                {
                    if (LeaderboardHeaderManager.m_Header != null)
                        if (!LeaderboardHeaderManager.m_Header.activeSelf)
                            return false;
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
        public async void SetHeader(bool p_Unranked)
        {
            await WaitUtils.Wait(() => gameObject.activeInHierarchy, 10, p_CodeLine: 305);

            if (!GuildSaberCustomLeaderboard.IsShown) return;

            ChangeHeaderText(p_Unranked ? Polyglot.Localization.Get("TITLE_HIGHSCORES") : $"Level {Leaderboard.CustomData.LevelValue} - {Leaderboard.CustomData.CategoryName.VerifiedCategory()}");

            await WaitUtils.Wait(() => GuildSaberLeaderboardPanel.PanelInstance.gameObject.activeSelf, 1);

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

        public void PageUp()
        {
            m_Page -= 1;
            SetLeaderboard(GuildSaberLeaderboardPanel.PanelInstance.m_SelectedGuild, false, m_Page);
        }

        public void PageDown()
        {
            m_Page += 1;
            SetLeaderboard(GuildSaberLeaderboardPanel.PanelInstance.m_SelectedGuild, false, m_Page);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Set current scores to list
        /// </summary>
        /// <param name="p_CustomData"></param>
        /// <param name="p_Scores"></param>
        /// <param name="p_PointsNames"></param>
        public async void SetScores(ApiCustomDataStruct p_CustomData, List<ApiMapLeaderboardContentStruct> p_Scores, string p_PointsNames)
        {
            GuildSaberLeaderboardView.m_Instance.SetLeaderboardViewMode(ELeaderboardViewMode.Scores);

            if (m_ListComponentScores.Count != GuildSaberModule.SCORES_BY_PAGE)
            {
                m_ListComponentScores.Clear();
                for (int l_i = 0; l_i < GuildSaberModule.SCORES_BY_PAGE; l_i++)
                {
                    var l_CurrentCell = new LeaderboardScoreCell();
                    m_ListComponentScores.Add(l_CurrentCell);
                }

                m_ScoreList.tableView.ReloadData();
            }

            LeaderboardScoreCell l_Cell = (LeaderboardScoreCell)m_ListComponentScores[GuildSaberModule.SCORES_BY_PAGE - 1];

            if (l_Cell.HasBeenParsed == false)
                await WaitUtils.Wait(() => l_Cell.HasBeenParsed, 50, 0);

            foreach (LeaderboardScoreCell l_Current in m_ListComponentScores)
            {
                l_Current.Reset();
                await WaitUtils.Wait(() => l_Current.m_ElemsLayout.gameObject.activeSelf, 1, p_CodeLine: 374);
            }

            for (int l_i = 0; l_i < p_Scores.Count; l_i++)
            {
                if (l_i > GuildSaberModule.SCORES_BY_PAGE - 1) continue;

                ApiMapLeaderboardContentStruct l_Score = p_Scores[l_i];
                PointsData l_PointData = default(PointsData);

                if (string.IsNullOrEmpty(p_PointsNames)) l_PointData = l_Score.PointsData[0];
                else
                {
                    foreach (PointsData l_Current in l_Score.PointsData)
                    {
                        if (l_Current.PointsName != p_PointsNames) continue;

                        l_PointData = l_Current;
                        break;
                    }
                }

                try
                {
                    LeaderboardScoreCell l_CurrentCell = (LeaderboardScoreCell)m_ListComponentScores[l_i];
                    l_CurrentCell.Init((int)l_Score.Rank, l_Score.Name, l_PointData.Points, l_PointData.PointsName, (int)l_Score.BaseScore, (float)l_Score.BaseScore * 100 / p_CustomData.MaxScore, l_Score.ScoreSaberID.ToString(), l_Score.Modifiers);
                    l_CurrentCell.SetModalInfo((int)l_Score.BadCuts, (int)l_Score.MissedNotes, (l_Score.ScoreStatistic != null) ? (int)l_Score.ScoreStatistic?.PauseCount! : null, (int)l_Score.ModifiedScore, new List<string>
                    {
                        "NF",
                        "SS",
                        "NA",
                        "NO",
                        "NB",
                        "ZM"
                    }, (PassState.EState)l_Score.State, (EHMD)l_Score.HMD, long.Parse(l_Score.UnixTimeSet));
                    if (l_Score.ScoreSaberID == GuildSaberModule.SsPlayerId.ToString())
                    {
                        l_CurrentCell.SetCellToCurrentPlayer();
                    }
                }
                catch (Exception l_E)
                {
                    GSLogger.Instance.Error(l_E, nameof(LeaderboardScoreList), nameof(SetScores));
                }
            }
        }
    }
}
