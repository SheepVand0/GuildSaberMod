﻿using BeatLeader.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using CP_SDK.Config;
using GuildSaber.API;
using GuildSaber.Configuration;
using GuildSaber.Logger;
using GuildSaber.UI.Leaderboard.Components.SubComponents;
using GuildSaber.UI.Leaderboard.Managers;
using GuildSaber.Utils;
using Polyglot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


#nullable enable
namespace GuildSaber.UI.Leaderboard.Components
{
    internal class LeaderboardScoreList : CustomUIComponent
    {
        protected override string ViewResourceName => "GuildSaber.UI.Leaderboard.Components.Views.LeaderboardScoreList.bsml";

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public static LeaderboardScoreList Instance;
        internal static bool StartedReplayFromMod = false;
        internal static ReplayLaunchData ReplayLaunchData = null;
        private static List<object> s_ListComponentScores = new List<object>();

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private static CustomLevelStatsView s_CustomLevelStatsView;
        private static GameObject s_GameObjectReference;
        internal static ApiMapLeaderboardCollectionStruct s_Leaderboard = new ApiMapLeaderboardCollectionStruct();
        private static string s_CurrentPointsName = string.Empty;
        public int m_Page = 1;

        [UIComponent("ErrorText")] internal readonly TextMeshProUGUI m_ErrorText = null;
        [UIComponent("Loading")] internal readonly GridLayoutGroup m_LoadingLayout = null;
        [UIComponent("NotRankedText")] internal readonly TextMeshProUGUI m_NotRankedText = null;

        [UIComponent("ScoreList")] internal readonly CustomCellListTableData m_ScoreList = null;

        internal static ScoreCellInfoModal s_InfoModal;

        [UIValue("ScoreCells")] internal List<object> m_ListComponentScores = new List<object>();

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        internal ELeaderboardScope m_SelectedScope = ELeaderboardScope.Global;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public bool Initialized { get; private set; }

        private float ListCellSize => 4f * LeaderboardScoreCell.ScaleFactor;

        public static IDifficultyBeatmap? CurrentBeatMap { get; internal set; }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        protected override async void AfterViewCreation()
        {

            s_CustomLevelStatsView = CreateItem<CustomLevelStatsView>((GuildSaberLeaderboardView.m_Instance.m_ScoreParamsLayout).transform, true, true);
            s_InfoModal = CreateItem<ScoreCellInfoModal>((GuildSaberLeaderboardView.m_Instance.m_ScoreParamsLayout).transform, true, true);

            await WaitUtils.Wait(() => GuildSaberLeaderboardPanel.PanelInstance != null, 10);
            CurrentBeatMap = Resources.FindObjectsOfTypeAll<LevelCollectionNavigationController>()[0].selectedDifficultyBeatmap;

            //SetLeaderboard(GuildSaberLeaderboardPanel.PanelInstance.m_SelectedGuild, false);

            Initialized = true;

            Events.e_OnLeaderboardHide += OnLeaderboardHide;
            Events.Instance.e_OnPointsTypeChange += OnPointsTypeChange;
            Events.Instance.e_OnGuildSelected += OnGuildSelected;
            Events.Instance.e_OnScopeSelected += OnScopeSelected;
            Events.e_OnReload += () => m_ListComponentScores.Clear();

            Instance = this;
            s_GameObjectReference = gameObject;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On Leaderboard Hide
        /// </summary>
        private void OnLeaderboardHide()
        {
            foreach (var l_Cell in s_ListComponentScores)
            {
                ((LeaderboardScoreCell)l_Cell).StopAnimation();
            }
        }

        private void OnLeaderboardShow()
        {
            if (!gameObject.activeInHierarchy) return;

            UpdateLeaderboard(GuildSaberLeaderboardPanel.PanelInstance.m_SelectedGuild);
        }

        /// <summary>
        /// On Leaderboard Set
        /// </summary>
        /// <param name="p_DifficultyBeatmap"></param>
        internal void OnLeaderboardSet(IDifficultyBeatmap p_DifficultyBeatmap)
        {
            if (p_DifficultyBeatmap == null || !Initialized)
                return;
            LeaderboardScoreList.CurrentBeatMap = p_DifficultyBeatmap;
            m_Page = 1;
            if (!gameObject.activeInHierarchy) return;

            UpdateLeaderboard(GuildSaberLeaderboardPanel.PanelInstance.m_SelectedGuild);
        }

        /// <summary>
        /// On scope selected
        /// </summary>
        /// <param name="p_Scope"></param>
        private void OnScopeSelected(ELeaderboardScope p_Scope)
        {
            m_SelectedScope = p_Scope;
            int p_Page = p_Scope == ELeaderboardScope.Around ? -2 : 1;
            UpdateLeaderboard(GuildSaberLeaderboardPanel.PanelInstance.m_SelectedGuild, p_Page);
        }

        /// <summary>
        /// On GuildSelected
        /// </summary>
        /// <param name="p_Guild"></param>
        private void OnGuildSelected(int p_Guild)
        {
            if (!Initialized || !s_GameObjectReference.activeInHierarchy)
                return;
            UpdateLeaderboard(p_Guild, p_SetScores: false);
        }

        /// <summary>
        /// On Points type change
        /// </summary>
        /// <param name="p_PointsName"></param>
        private void OnPointsTypeChange(string p_PointsName)
        {
            s_CurrentPointsName = p_PointsName;
            if (s_Leaderboard.Equals(null) || s_Leaderboard.Leaderboards == null)
            {
                UpdateLeaderboard(GuildSaberLeaderboardPanel.PanelInstance.m_SelectedGuild);
                return;
            }
            if (s_Leaderboard.Leaderboards.Count == 0)
            {
                UpdateLeaderboard(GuildSaberLeaderboardPanel.PanelInstance.m_SelectedGuild);
                return;
            }
            try
            {
                if (!s_GameObjectReference.activeInHierarchy)
                    return;
                SetScores(s_Leaderboard.CustomData, s_Leaderboard.Leaderboards, s_CurrentPointsName);
            }
            catch (Exception ex)
            {
                GSLogger.Instance.Error(ex, nameof(LeaderboardScoreList), nameof(OnPointsTypeChange));
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Set leaderboard from stored difficulty beatmap, guild, and selected page
        /// </summary>
        /// <param name="p_Guild"></param>
        /// <param name="p_NeedUpdate"></param>
        /// <param name="p_Page"></param>
        public async void UpdateLeaderboard(int p_Guild, int p_Page = 0, bool p_SetScores = true)
        {
            await WaitUtils.Wait(() => GuildSaberLeaderboardView.s_GameObjectReference.activeInHierarchy && Initialized, 100, p_MaxTryCount: 15, p_CodeLine: 176);
            if (!s_GameObjectReference.activeInHierarchy)
                return;

            if (GuildSaberModule.IsStateError())
            {
                m_ErrorText.SetTextError(GuildSaberModule.ModErrorState == GuildSaberModule.EModErrorState.BadRequest_400 ? new Exception("Error during getting player data \n: Bad Request, maybe you mod is outdated or the api is down") : new Exception("Error during getting player data : \n Go scan !"), GuildSaberUtils.ErrorMode.Message);

                GuildSaberLeaderboardView.m_Instance.SetLeaderboardViewMode(ELeaderboardViewMode.Error);
                s_CustomLevelStatsView.Clear();
                return;
            }

            ///Init change
            GuildSaberLeaderboardView.m_Instance.SetLeaderboardViewMode(ELeaderboardViewMode.Loading);

            if (CurrentBeatMap == null)
            {
                GuildSaberLeaderboardView.m_Instance.SetLeaderboardViewMode(ELeaderboardViewMode.Error);
                return;
            }

            ///Initing current map values

            string l_Hash = GSBeatmapUtils.DifficultyBeatmapToHash(CurrentBeatMap);
            string l_Country =
                GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData.Country != string.Empty && m_SelectedScope == ELeaderboardScope.Country ? GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData.Country : string.Empty;

            ///Page managing

            int l_Page = p_Page switch
            {
                -2 => GuildSaberUtils.CalculatePageByRank((int)s_Leaderboard.PlayerScore?.Rank!),
                <= 0 => m_Page,
                _ => p_Page
            };

            m_Page = l_Page;

            ///Scope managing

            if (m_SelectedScope == ELeaderboardScope.Country) l_Country = GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData.Country;

            ///Points managing

            if (s_CurrentPointsName == string.Empty) s_CurrentPointsName = GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData.RankData[0].PointsName;

            ///Getting leaderboard from API

            s_Leaderboard = await GuildApi.GetLeaderboard(p_Guild, l_Hash, CurrentBeatMap, l_Page, GuildSaberModule.GSPlayerId ?? 0, l_Country, CurrentBeatMap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName, 10, PointsType.GetPointsIDByName(GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData, GuildSaberModule.LeaderboardSelectedGuild, s_CurrentPointsName));

            ///Set around me button active if player have a score on current map

            GuildSaberLeaderboardView.m_Instance.m_ScopeSelector.m_AroundImage.gameObject.SetActive(s_Leaderboard.PlayerScore != null);


            ///If map not ranked

            if (s_Leaderboard.Equals(default(ApiMapLeaderboardCollectionStruct)) || s_Leaderboard.Leaderboards == null || s_Leaderboard.Metadata.Equals(null))
            {
                GuildSaberLeaderboardView.m_Instance.SetLeaderboardViewMode(ELeaderboardViewMode.NotRanked);
                s_CustomLevelStatsView.Clear();
                SetHeader(true);
                return;
            }
            if (!s_Leaderboard.Equals(default(ApiMapLeaderboardCollectionStruct)) && s_Leaderboard.Leaderboards.Count == 0)
            {
                ///If map is un passed
                GuildSaberLeaderboardView.m_Instance.SetLeaderboardViewMode(ELeaderboardViewMode.UnPassed);
                ChangeHeaderText($"Level {s_Leaderboard.CustomData.LevelValue} - {s_Leaderboard.CustomData.CategoryName.VerifiedCategory()}");
                GuildSaberLeaderboardView.m_Instance.m_PageDownImage.interactable = false;
                if (m_Page == 1)
                    GuildSaberLeaderboardView.m_Instance.m_PageUpImage.interactable = false;
                else if (m_Page > 1) GuildSaberLeaderboardView.m_Instance.m_PageUpImage.interactable = true;
                SetHeader(false);
                s_CustomLevelStatsView.Clear();
                return;
            }

            ///Page button managing

            GuildSaberLeaderboardView.m_Instance.m_PageDownImage.interactable = m_Page != s_Leaderboard.Metadata.MaxPage;
            GuildSaberLeaderboardView.m_Instance.m_PageUpImage.interactable = m_Page > 1;

            ///Set scores on ScoreList
            if (p_SetScores == true)
            {
                SetScores(s_Leaderboard.CustomData, s_Leaderboard.Leaderboards, s_CurrentPointsName);
            }

            ///If player have a score initing score modal

            try
            {
                if (GuildSaberLeaderboardView.m_Instance.m_ScopeSelector.m_AroundImage.gameObject.activeInHierarchy)
                {
                    s_CustomLevelStatsView.SetModalInfo((int)s_Leaderboard.PlayerScore?.BadCuts!, (int)s_Leaderboard.PlayerScore?.MissedNotes!,
                        s_Leaderboard.PlayerScore?.HasScoreStatistic ?? false ? (int)s_Leaderboard.PlayerScore?.ScoreStatistic?.PauseCount! : null,
                        (EHMD)s_Leaderboard.PlayerScore?.HMD!);

                    s_CustomLevelStatsView.Init((int)s_Leaderboard.PlayerScore?.Rank!,
                        s_Leaderboard.PlayerScore?.Name,
                        (PassState.EState)s_Leaderboard.PlayerScore?.State!);
                }
                else { s_CustomLevelStatsView.Clear(); }
            }
            catch (Exception l_E) { GSLogger.Instance.Error(l_E, nameof(LeaderboardScoreList), nameof(UpdateLeaderboard)); }

            ///If need update changing header

            SetHeader(false);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Set Header text
        /// </summary>
        /// <param name="p_Unranked"></param>
        public async void SetHeader(bool p_Unranked)
        {
            if (LeaderboardHeaderManager.m_Header == null || !LeaderboardHeaderManager.m_Header.activeSelf || !GuildSaberCustomLeaderboard.IsShown)
                return;

            string l_Text;
            if (!p_Unranked)
            {
                ApiCustomDataStruct l_CustomData = s_Leaderboard.CustomData;

                float l_LevelValue = l_CustomData.LevelValue;
                l_CustomData = s_Leaderboard.CustomData;

                string l_VerifiedCategory = l_CustomData.CategoryName.VerifiedCategory();
                l_CustomData = s_Leaderboard.CustomData;

                float l_PassPoints = l_CustomData.PassPoints;

                l_Text = $"Level {l_LevelValue} - {l_VerifiedCategory} - {l_PassPoints} PassPoints";
            }
            else
                l_Text = Localization.Get("TITLE_HIGHSCORES");

            ChangeHeaderText(l_Text);
            await WaitUtils.Wait(() => GuildSaberLeaderboardPanel.PanelInstance.gameObject.activeSelf, 1);
            GuildSaberLeaderboardPanel.PanelInstance.SetColors();
            LeaderboardHeaderManager.ShowCustom();
        }

        public void ChangeHeaderText(string p_Text)
        {
            if (JsonConfig<GSConfig>.Instance.UwUMode)
                p_Text += " `(*>﹏<*)′";
            LeaderboardHeaderManager.ChangeText(p_Text);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// One page up on leaderboard
        /// </summary>
        public void PageUp()
        {
            --m_Page;
            UpdateLeaderboard(GuildSaberLeaderboardPanel.PanelInstance.m_SelectedGuild, m_Page);
        }

        /// <summary>
        /// One page down on leaderboard
        /// </summary>
        public void PageDown()
        {
            ++m_Page;
            UpdateLeaderboard(GuildSaberLeaderboardPanel.PanelInstance.m_SelectedGuild, m_Page);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Set scores from a score list
        /// </summary>
        /// <param name="p_CustomData"></param>
        /// <param name="p_Scores"></param>
        /// <param name="p_PointsNames"></param>
        public async void SetScores(
          ApiCustomDataStruct p_CustomData,
          List<ApiMapLeaderboardContentStruct> p_Scores,
          string p_PointsNames)
        {
            if (s_Leaderboard.Leaderboards == null)
                return;

            await WaitUtils.Wait(() => s_GameObjectReference.activeSelf, 100, p_MaxTryCount: 10);

            if (!s_GameObjectReference.activeSelf)
                return;

            GuildSaberLeaderboardView.m_Instance.SetLeaderboardViewMode(ELeaderboardViewMode.Scores);

            if (m_ListComponentScores.Count != GuildSaberModule.SCORES_BY_PAGE)
            {
                m_ListComponentScores.Clear();

                for (int l_i = 0; l_i < 10; ++l_i)
                {
                    LeaderboardScoreCell l_CurrentCell = new LeaderboardScoreCell();
                    m_ListComponentScores.Add(l_CurrentCell);
                }

                s_ListComponentScores = m_ListComponentScores;

                await WaitUtils.Wait(() => m_ScoreList != null, 1);
                m_ScoreList.tableView.ReloadData();
            }
            LeaderboardScoreCell l_Cell = (LeaderboardScoreCell)s_ListComponentScores[9];

            for (int l_i = 0; l_i < GuildSaberModule.SCORES_BY_PAGE - p_Scores.Count; l_i++)
            {
                ((LeaderboardScoreCell)s_ListComponentScores[l_i + p_Scores.Count]).Reset();
            }

            for (int l_i = 0; l_i < p_Scores.Count; ++l_i)
            {
                if (l_i <= 9)
                {
                    ApiMapLeaderboardContentStruct l_Score = p_Scores[l_i];
                    PointsData l_PointData = new PointsData();
                    if (string.IsNullOrEmpty(p_PointsNames))
                    {
                        l_PointData = l_Score.PointsData[0];
                    }
                    else
                    {
                        foreach (PointsData l_Current in l_Score.PointsData)
                        {
                            if (!(l_Current.PointsName != p_PointsNames))
                            {
                                if (l_Current.PointsIndex == 1)
                                {
                                    l_PointData.Points = float.NaN;
                                    l_PointData.PointsName = string.Empty;
                                    break;
                                }
                                l_PointData = l_Current;
                                break;
                            }
                        }
                    }

                    try
                    {
                        LeaderboardScoreCell l_CurrentCell = (LeaderboardScoreCell)s_ListComponentScores[l_i];
                        l_CurrentCell.Init(l_Score, p_CustomData, l_i);
                        await Task.Delay((int)(LeaderboardScoreCell.ANIMATION_DURATION * 100));
                    }
                    catch (Exception ex)
                    {
                        GSLogger.Instance.Error(ex, nameof(LeaderboardScoreList), nameof(SetScores));
                    }
                }
            }
        }
    }
}
