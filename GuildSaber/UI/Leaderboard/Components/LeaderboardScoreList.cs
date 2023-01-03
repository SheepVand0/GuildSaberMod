using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeatLeader.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using GuildSaber.API;
using GuildSaber.BSPModule;
using GuildSaber.Utils;
<<<<<<< Updated upstream
=======
using HMUI;
using Polyglot;
>>>>>>> Stashed changes
using TMPro;
using UnityEngine;
using GuildSaber.Configuration;
using GuildSaber.Logger;
using UnityEngine.UI;
using GuildSaber.UI.Leaderboard.Components.SubComponents;

namespace GuildSaber.UI.Leaderboard.Components
{

<<<<<<< Updated upstream
=======
    public static LeaderboardScoreList Instance;

    internal static bool StartedReplayFromMod = false;
    internal static ReplayLaunchData ReplayLaunchData = null;
    private static List<object> s_ListComponentScores = new List<object>();
>>>>>>> Stashed changes

    class LeaderboardScoreList : CustomUIComponent
    {
        protected override string ViewResourceName => "GuildSaber.UI.Leaderboard.Components.Views.LeaderboardScoreList.bsml";

        [UIComponent("ScoreList")] internal readonly CustomCellListTableData m_ScoreList = null;
        [UIComponent("NotRankedText")] internal readonly TextMeshProUGUI m_NotRankedText = null;
        [UIComponent("ErrorText")] internal readonly TextMeshProUGUI m_ErrorText = null;
        [UIComponent("Loading")] internal readonly GridLayoutGroup m_LoadingLayout = null;

<<<<<<< Updated upstream
        public static LeaderboardScoreList Instance;
=======
    internal static ApiMapLeaderboardCollectionStruct s_Leaderboard = default(ApiMapLeaderboardCollectionStruct);
    private static string s_CurrentPointsName = string.Empty;
    public int m_Page = 1;
    [UIComponent("ErrorText")] internal readonly TextMeshProUGUI m_ErrorText = null;
    [UIComponent("Loading")] internal readonly GridLayoutGroup m_LoadingLayout = null;
    [UIComponent("NotRankedText")] internal readonly TextMeshProUGUI m_NotRankedText = null;
>>>>>>> Stashed changes

        internal static bool s_StartedReplayFromMod = false;
        internal static ReplayLaunchData s_ReplayLaunchData = null;

<<<<<<< Updated upstream
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        [UIValue("ScoreCells")] internal List<object> m_ListComponentScores = new List<object>();
        private static List<object> s_ListComponentScores = new List<object>();
=======
    internal static ScoreCellInfoModal s_InfoModal;

    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    [UIValue("ScoreCells")] internal List<object> m_ListComponentScores = new List<object>();
>>>>>>> Stashed changes

        static CustomLevelStatsView s_CustomLevelStatsView = null;

        private static GameObject s_GameObjectReference = null;

        private bool m_FromReload = false;

        public bool Initialized { get; private set; }

<<<<<<< Updated upstream
        private float ListCellSize
        {
            get => 4 * LeaderboardScoreCell.ScaleFactor;
        }
=======
    protected override async void AfterViewCreation()
    {
        s_CustomLevelStatsView = CreateItem<CustomLevelStatsView>(GuildSaberLeaderboardView.m_Instance.m_ScoreParamsLayout.transform, true, true);
        s_InfoModal = CreateItem<ScoreCellInfoModal>(GuildSaberLeaderboardView.m_Instance.m_ScoreParamsLayout.transform, true, true);
>>>>>>> Stashed changes

        static ApiMapLeaderboardCollectionStruct s_Leaderboard = default(ApiMapLeaderboardCollectionStruct);
        public int m_Page = 1;
        private static string s_CurrentPointsName = string.Empty;
        //public string CurrentMapHash { get; internal set; } = string.Empty;
        public static IDifficultyBeatmap? CurrentBeatMap { get; internal set; } = null;

        internal ELeaderboardScope m_SelectedScope = ELeaderboardScope.Global;
        public bool ChangingLeaderboard { get; private set; } = false;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        protected override async void AfterViewCreation()
        {
            s_CustomLevelStatsView = CustomUIComponent.CreateItem<CustomLevelStatsView>(GuildSaberLeaderboardView.m_Instance.m_ScoreParamsLayout.transform, true, true);

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
            Events.e_OnReload += async () =>
            {
                m_FromReload = true;
                m_ListComponentScores.Clear();
            };

            Instance = this;
            s_GameObjectReference = gameObject;
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

            SetLeaderboard(GuildSaberLeaderboardPanel.PanelInstance.m_SelectedGuild, false, l_Page);
        }

        /// <summary>
        /// On guild is selected
        /// </summary>
        /// <param name="p_Guild"></param>
        private void OnGuildSelected(int p_Guild)
        {
            if (!Initialized || !s_GameObjectReference.activeInHierarchy) return;

            SetLeaderboard(p_Guild, false);
        }

        /// <summary>
        /// On other points type selected
        /// </summary>
        /// <param name="p_PointsName"></param>
        /// <param name="p_PointsName"></param>
        private void OnPointsTypeChange(string p_PointsName)
        {
            s_CurrentPointsName = p_PointsName;

            if (s_Leaderboard.Equals(null)) return;
            else if (s_Leaderboard.Leaderboards == null) return;
            else if (s_Leaderboard.Leaderboards.Count == 0) return;

            try
            {
                if (s_GameObjectReference.activeInHierarchy)
                {
                    SetScores(s_Leaderboard.CustomData, s_Leaderboard.Leaderboards, s_CurrentPointsName);
                }
            }
            catch (Exception l_E)
            {
                GSLogger.Instance.Error(l_E, nameof(LeaderboardScoreList), nameof(OnPointsTypeChange));
            }
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

            if (m_FromReload)
            {
                m_FromReload = false;
                return;
            }

            ChangingLeaderboard = true;

            await WaitUtils.Wait(() => GuildSaberLeaderboardView.s_GameObjectReference.activeInHierarchy && Initialized, 100, p_MaxTryCount: 15, p_CodeLine: 176);
            if (!gameObject.activeInHierarchy) return;



            if (GuildSaberModule.IsStateError())
            {
                m_ErrorText.SetTextError(GuildSaberModule.ModErrorState == GuildSaberModule.EModErrorState.BadRequest_400 ? new System.Exception($"Error during getting player data \n: Bad Request, maybe you mod is outdated or the api is down") : new Exception("Error during getting player data : \n Go scan !"), GuildSaberUtils.ErrorMode.Message);

                GuildSaberLeaderboardView.m_Instance.SetLeaderboardViewMode(ELeaderboardViewMode.Error);
                s_CustomLevelStatsView.Clear();
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
                -2 => GuildSaberUtils.CalculatePageByRank((int)s_Leaderboard.PlayerScore?.Rank!),
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

            if (s_CurrentPointsName == string.Empty)
                s_CurrentPointsName = GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData.RankData[0].PointsName;

            ///Getting leaderboard from API

            s_Leaderboard = await GuildApi.GetLeaderboard(p_Guild, l_Hash, CurrentBeatMap, l_Page, GuildSaberModule.GSPlayerId ?? 0, p_Country: l_Country, p_Mode: CurrentBeatMap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName, 10, PointsType.GetPointsIDByName(GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData, GuildSaberModule.LeaderboardSelectedGuild, s_CurrentPointsName));

            ///Set around me button active if player have a score on current map

            GuildSaberLeaderboardView.m_Instance.m_ScopeSelector.m_AroundImage.gameObject.SetActive(s_Leaderboard.PlayerScore != null);


            ///If map not ranked

            if (s_Leaderboard.Equals(default(ApiMapLeaderboardCollectionStruct)) || s_Leaderboard.Leaderboards == null || s_Leaderboard.Metadata.Equals(null))
            {
                GuildSaberLeaderboardView.m_Instance.SetLeaderboardViewMode(ELeaderboardViewMode.NotRanked);
                s_CustomLevelStatsView.Clear();
                SetHeader(true);
                ChangingLeaderboard = false;
                return;
            }
            else if (!s_Leaderboard.Equals(default(ApiMapLeaderboardCollectionStruct)) && s_Leaderboard.Leaderboards.Count == 0)
            {
                ///If map is un passed
                GuildSaberLeaderboardView.m_Instance.SetLeaderboardViewMode(ELeaderboardViewMode.UnPassed);
                ChangeHeaderText($"Level {s_Leaderboard.CustomData.LevelValue} - {s_Leaderboard.CustomData.CategoryName.VerifiedCategory()}");
                GuildSaberLeaderboardView.m_Instance.m_PageDownImage.interactable = false;
                if (m_Page == 1)
                    GuildSaberLeaderboardView.m_Instance.m_PageUpImage.interactable = false;
                else if (m_Page > 1)
                    GuildSaberLeaderboardView.m_Instance.m_PageUpImage.interactable = true;
                SetHeader(false);
                s_CustomLevelStatsView.Clear();
                ChangingLeaderboard = false;
                return;
            }

            ///Page button managing

            GuildSaberLeaderboardView.m_Instance.m_PageDownImage.interactable = m_Page != s_Leaderboard.Metadata.MaxPage;
            GuildSaberLeaderboardView.m_Instance.m_PageUpImage.interactable = m_Page > 1;

            ///Set scores on ScoreList

            SetScores(s_Leaderboard.CustomData, s_Leaderboard.Leaderboards, s_CurrentPointsName);

            ///If player have a score initing score modal

            try
            {
                if (GuildSaberLeaderboardView.m_Instance.m_ScopeSelector.m_AroundImage.gameObject.activeInHierarchy)
                {
                    s_CustomLevelStatsView.SetModalInfo((int)s_Leaderboard.PlayerScore?.BadCuts!, (int)s_Leaderboard.PlayerScore?.MissedNotes!,
                        (s_Leaderboard.PlayerScore?.HasScoreStatistic ?? false) ? (int)s_Leaderboard.PlayerScore?.ScoreStatistic?.PauseCount! : null,
                        (EHMD)s_Leaderboard.PlayerScore?.HMD!);

                    s_CustomLevelStatsView.Init((int)s_Leaderboard.PlayerScore?.Rank!,
                        s_Leaderboard.PlayerScore?.Name,
                        (PassState.EState)s_Leaderboard.PlayerScore?.State!);
                }
                else
                {
                    s_CustomLevelStatsView.Clear();
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
                    SetHeader(false);

                    return false;
                }, 1, 0, 1100);
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
            //await WaitUtils.Wait(() => gameObject.activeSelf, 10, p_CodeLine: 305);

            if (LeaderboardHeaderManager.m_Header == null || !LeaderboardHeaderManager.m_Header.activeSelf) return;

            if (!GuildSaberCustomLeaderboard.IsShown) return;

            ChangeHeaderText(p_Unranked ? Polyglot.Localization.Get("TITLE_HIGHSCORES") : $"Level {s_Leaderboard.CustomData.LevelValue} - {s_Leaderboard.CustomData.CategoryName.VerifiedCategory()} - {s_Leaderboard.CustomData.PassPoints} PassPoints");

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
            //await WaitUtils.Wait(() => gameObject.activeSelf, 1, p_CodeLine: 362);

            if (s_Leaderboard.Leaderboards == null) return;

            await WaitUtils.Wait(() => s_GameObjectReference.activeSelf, 100, p_MaxTryCount: 10);
            if (!s_GameObjectReference.activeSelf) return;

            GuildSaberLeaderboardView.m_Instance.SetLeaderboardViewMode(ELeaderboardViewMode.Scores);

            if (m_ListComponentScores.Count != GuildSaberModule.SCORES_BY_PAGE)
            {
                m_ListComponentScores.Clear();
                for (int l_i = 0; l_i < GuildSaberModule.SCORES_BY_PAGE; l_i++)
                {
                    var l_CurrentCell = new LeaderboardScoreCell();
                    m_ListComponentScores.Add(l_CurrentCell);
                }

<<<<<<< Updated upstream
                s_ListComponentScores = m_ListComponentScores;

                await WaitUtils.Wait(() => m_ScoreList != null, 1);

                m_ScoreList.tableView.ReloadData();
            }

            LeaderboardScoreCell l_Cell = (LeaderboardScoreCell)s_ListComponentScores[GuildSaberModule.SCORES_BY_PAGE - 1];

            if (l_Cell.HasBeenParsed == false)
                await WaitUtils.Wait(() => l_Cell.HasBeenParsed, 50, 0);

            foreach (LeaderboardScoreCell l_Current in s_ListComponentScores)
            {
                l_Current.Reset();
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

                try
                {
                    ///Player settings for BeatLeader Replay

                    Player l_Player = new Player
                    {
                        id = l_Score.BeatLeaderID,
                        avatar = l_Score.Avatar,
                        rank = (int)l_Score.Rank,
                        name = l_Score.Name,
                        countryRank = (int)l_Score.Rank
                    };

                    ProfileSettings l_Settings = new()
                    {
                        hue = 1,
                        message = string.Empty,
                        saturation = 1,
                        effectName = string.Empty
                    };

                    l_Player.profileSettings = l_Settings;

                    foreach (PointsData l_Index in l_Score.PointsData.Where(p_Index => p_Index.PointsName == GuildSaberLeaderboardPanel.PanelInstance.m_PointsType.m_SelectedPoints))
                    {
                        l_Player.pp = l_Index.Points;
                    }

                    LeaderboardScoreCell l_CurrentCell = (LeaderboardScoreCell)s_ListComponentScores[l_i];
                    l_CurrentCell.Init((int)l_Score.Rank, l_Score.Name, l_PointData.Points, l_PointData.PointsName, (int)l_Score.BaseScore, (float)l_Score.BaseScore * 100 / p_CustomData.MaxScore, l_Score.ScoreSaberID.ToString(), l_Score.Modifiers);
                    l_CurrentCell.SetModalInfo((int)l_Score.BadCuts, (int)l_Score.MissedNotes, (l_Score.ScoreStatistic != null) ? (int)l_Score.ScoreStatistic?.PauseCount! : null, (int)l_Score.ModifiedScore, new List<string>
                    {
                        "NF",
                        "SS",
                        "NA",
                        "NO",
                        "NB",
                        "ZM"
                    }, (PassState.EState)l_Score.State, (EHMD)l_Score.HMD, long.Parse(l_Score.UnixTimeSet), l_Player, l_Score.ReplayURL);
                    if (l_Score.ID == GuildSaberModule.GSPlayerId)
                    {
                        l_CurrentCell.SetCellToCurrentPlayer();
                    }
                }
                catch (Exception l_E)
                {
                    GSLogger.Instance.Error(l_E, nameof(LeaderboardScoreList), nameof(SetScores));
                }
=======
            try {
                ///Player settings for BeatLeader Replay
                var l_CurrentCell = (LeaderboardScoreCell)s_ListComponentScores[l_i];
                l_CurrentCell.Init(l_Score, p_CustomData, l_i);
>>>>>>> Stashed changes
            }

            SetHeader(false);
        }
    }
}
