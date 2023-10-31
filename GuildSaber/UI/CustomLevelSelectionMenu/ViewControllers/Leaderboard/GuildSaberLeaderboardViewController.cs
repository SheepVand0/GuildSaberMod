using BeatSaberPlus.SDK.UI;
using CP_SDK.Unity;
using CP_SDK.XUI;
using GuildSaber.API;
using GuildSaber.Logger;
using GuildSaber.UI.CustomLevelSelectionMenu.Components;
using GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard.Components;
using GuildSaber.UI.Defaults;
using GuildSaber.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard
{
    internal class GuildSaberLeaderboardViewController : ViewController<GuildSaberLeaderboardViewController>
    {
        protected XUIHLayout m_LeaderboardPanelContainer;

        LeaderboardPanel m_LeaderboardPanel;

        XUIVLayout m_MainLayout;
        XUIVLayout m_ScopeSelectorLayout;
        XUIVLayout m_LeaderboardCellsContainer;
        XUIHLayout m_ShowLeaderboardButtonContainer;
        XUIVLayout m_LoadingTextLayout;

        LeaderboardHeaderPanel m_HeaderPanel;

        GSLoadingIndicator m_LoadingText;
        XUIText m_ErrorText;
        XUIText m_NoScoreText;

        GSSecondaryButton m_ShowLeaderboardButton;

        LeaderboardScopeSelector m_ScopeSelector;

        protected List<LeaderboardCell> m_Cells = new List<LeaderboardCell>();

        public static LeaderboardScoreDetailsModal m_ScoreDetailsModal;

        protected IDifficultyBeatmap m_SelectedBeatmap;

        Color m_GuildColor = new Color();

        public static ApiPlayerData GuildPlayerData = default;

        protected string m_SelectedPointsType = string.Empty;
        protected ApiMapLeaderboardCollectionStruct m_Leaderboard;

        protected bool m_IsChanging;

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public enum ELeaderboardMode
        {
            Error,
            Normal,
            Loading,
            NoScore
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        protected override void OnViewCreation()
        {
            //Templates.FullRectLayout(
            XUIVLayout.Make(
                XUIHLayout.Make(
                    m_LeaderboardPanel = LeaderboardPanel.Make()
                )
                    .SetMinWidth(100)
                    .SetMinHeight(10)
                    .Bind(ref m_LeaderboardPanelContainer),
                XUIVSpacer.Make(3),
                m_HeaderPanel = (LeaderboardHeaderPanel)LeaderboardHeaderPanel.Make()
                    .SetMinHeight(LeaderboardHeaderPanel.HEADER_HEIGHT),
                XUIHLayout.Make(
                    XUIText.Make("Error loading leaderboard").SetColor(Color.red).Bind(ref m_ErrorText),
                    XUIText.Make("No score set on this leaderboard").SetColor(Color.yellow).Bind(ref m_NoScoreText),
                    (m_LoadingText = GSLoadingIndicator.Make())
                    .SetWidth(7)
                    .SetHeight(7),
                    //XUIText.Make("Loading...").Bind(ref m_LoadingText),
                    XUIVLayout.Make(
                        m_ScopeSelector = LeaderboardScopeSelector.Make()
                        ).Bind(ref m_ScopeSelectorLayout),
                    XUIVLayout.Make(
                        ).OnReady(x =>
                        {
                            for (int l_i = 0; l_i < 10; l_i++)
                            {
                                LeaderboardCell l_Cell = LeaderboardCell.Make();

                                l_Cell.BuildUI(m_LeaderboardCellsContainer.Element.LElement.transform);
                                m_Cells.Add(l_Cell);
                            }
                        })
                        .Bind(ref m_LeaderboardCellsContainer)
                        .SetSpacing(0)
                        .SetWidth(80)
                        .SetMinHeight(10 * LeaderboardCell.CELL_HEIGHT)
                    ),
               XUIVSpacer.Make(4)
                )
                .SetSpacing(0)
                .Bind(ref m_MainLayout)
                .BuildUI(transform);

            MapButton.eOnMapSelected += EventOnMapSelected;
            m_ScopeSelector.eOnPageChanged += OnPageChanged;

            SetMode(ELeaderboardMode.Normal);
            m_ScopeSelector.SetScope(ELeaderboardScope.Global);
            m_ScopeSelector.eOnScopeChanged += OnScopeChanged;
            m_LeaderboardPanel.GetPointsSelector().eOnChange += OnPointsChanged;

            Hide();
        }

        protected override void OnViewDeactivation()
        {
            HideScoreSaber();
            foreach (var l_Item in m_Cells)
            {
                l_Item.Element.gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public Color GetGuildColor()
        => m_GuildColor;

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        protected void OnScopeChanged(ELeaderboardScope p_Scope)
        {
            OnMapSelectedImplementation(m_SelectedBeatmap, true, m_ScopeSelector.GetMaxPage());
        }

        protected void EventOnMapSelected(IDifficultyBeatmap p_Beatmap)
        {
            Show();
            OnMapSelectedImplementation(p_Beatmap, true, 2);
        }

        protected void OnMapSelectedImplementation(IDifficultyBeatmap p_Beatmap, bool p_ResetPage = false, int p_MaxPage = 0)
        {
            if (p_ResetPage)
            {
                m_ScopeSelector.SetPage(1, p_MaxPage);
            }

            string l_Hash = GSBeatmapUtils.DifficultyBeatmapToHash(p_Beatmap);
            m_SelectedBeatmap = p_Beatmap;
            SetLeaderboard(CustomLevelSelectionMenuReferences.SelectedGuildId, l_Hash, p_Beatmap, m_ScopeSelector.GetPage());
        }

        protected void OnPageChanged()
        {
            OnMapSelectedImplementation(m_SelectedBeatmap);
        }

        protected void OnPointsChanged(PointsData p_Points)
        {
            m_SelectedPointsType = p_Points.PointsType;
            UpdateLeaderboardCells(m_Leaderboard);
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public void Show()
        {
            m_MainLayout.SetActive(true);
        }

        public void Hide()
        {
            m_MainLayout.SetActive(false);
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public void SetMode(ELeaderboardMode p_Mode)
        {
            m_ErrorText.SetActive(p_Mode == ELeaderboardMode.Error);
            m_ScopeSelectorLayout.SetActive(p_Mode == ELeaderboardMode.Normal);
            m_LeaderboardCellsContainer.SetActive(p_Mode == ELeaderboardMode.Normal || p_Mode == ELeaderboardMode.Loading);
            m_LoadingText.SetActive(p_Mode == ELeaderboardMode.Loading);
            m_NoScoreText.SetActive(p_Mode == ELeaderboardMode.NoScore);
            if (p_Mode == ELeaderboardMode.Loading)
            {
                HideLeaderboardCells();
            }

            m_ScopeSelector.SetActive(p_Mode == ELeaderboardMode.Normal || (p_Mode == ELeaderboardMode.NoScore && m_ScopeSelector.GetSelectedScope().HasFlag(ELeaderboardScope.Global) == false));
        }

        public async void SetGuild(int p_GuildId)
        {
            Hide();

            GuildData l_GuildData = GuildApi.GetGuildFromId(p_GuildId);
            var l_Banner = await GuildSaberUtils.GetImage(l_GuildData.Logo);
            Texture2D l_Result;
            if (l_Banner.IsError)
            {
                l_Result = CustomLevelSelectionMenuReferences.DefaultLogo;
            }
            else
            {
                l_Result = l_Banner.Texture;
            }

            var l_Logo = await GuildSaberUtils.GetImage(/*l_GuildData.Logo*/GuildSaberModule.BasicPlayerData.Avatar);
            Texture2D l_LogoResult;
            if (l_Logo.IsError)
            {
                l_LogoResult = CustomLevelSelectionMenuReferences.DefaultLogo;
            } else
            {
                l_LogoResult = l_Logo.Texture;
            }

            ApiPlayerData l_PlayerDataForGuild = await GuildApi.GetPlayerData(false, p_GuildId);
            if (l_PlayerDataForGuild.Equals(default)) return;

            GuildPlayerData = l_PlayerDataForGuild;

            m_LeaderboardPanel.SetGuild(l_Result, l_LogoResult, p_GuildId, GuildSaberModule.BasicPlayerData.Name) ;
            m_GuildColor = l_GuildData.Color.ToUnityColor();
        }

        public async void SetLeaderboard(int p_GuildID, string p_MapHash, IDifficultyBeatmap p_Beatmap, int p_Page, int p_SearchType = 1, bool p_RetryOnCatch = true)
        {
            ApiPlayerData l_PlayerData = GuildSaberModule.BasicPlayerData;
            try
            {
                m_IsChanging = true;

                SetMode(ELeaderboardMode.Loading);
                ApiMapLeaderboardCollectionStruct l_Leaderboard = await GuildApi.GetLeaderboard(p_GuildID, p_MapHash, p_Beatmap, p_Page, (m_ScopeSelector.GetSelectedScope() == ELeaderboardScope.Around) ? (int)GuildSaberModule.GSPlayerId : 0, (m_ScopeSelector.GetSelectedScope() == ELeaderboardScope.Country) ? l_PlayerData.Country : string.Empty, p_Beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName);
                m_Leaderboard = l_Leaderboard;
                if (l_Leaderboard.Leaderboards.Count == 0)
                {
                    SetMode(ELeaderboardMode.NoScore);
                    return;
                }
                SetMode(ELeaderboardMode.Normal);
                m_ScopeSelector.SetPage(m_ScopeSelector.GetPage(), (int)l_Leaderboard.Metadata.MaxPage);

                m_IsChanging = false;

                UpdateLeaderboardCells(l_Leaderboard);

                if (!m_IsLeaderboardOnScoreSaber) return;

                var l_LeaderboardViewController = Resources.FindObjectsOfTypeAll<PlatformLeaderboardViewController>().First();
                l_LeaderboardViewController.SetData(m_SelectedBeatmap);
            }
            catch (Exception l_E)
            {
                GSLogger.Instance.Error(l_E, nameof(GuildSaberLeaderboardViewController), nameof(SetLeaderboard));
                SetMode(ELeaderboardMode.Error);
                if (p_RetryOnCatch == true)
                {
                    m_LeaderboardPanel.GetPointsSelector().SetSelectedPoints(GuildApi.PASS_POINTS_TYPE);
                    SetLeaderboard(p_GuildID, p_MapHash, p_Beatmap, p_Page, p_SearchType, false);
                }
            }
        }

        public void SetScoreDetailsModalData(ApiMapLeaderboardContentStruct p_Score, int p_MaxScore)
        {
            var l_Modal = GetScoreDetailsModal();
            l_Modal.SetScore(p_Score, p_MaxScore);
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public bool IsChanging()
        => m_IsChanging;

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public void HideLeaderboardCells()
        {
            foreach (var l_Item in m_Cells)
            {
                l_Item.Hide();
            }
        }

        public void UpdateLeaderboardCells(ApiMapLeaderboardCollectionStruct p_Leaderboard)
        {
            HideLeaderboardCells();

            for (int l_i = 0; l_i < p_Leaderboard.Leaderboards.Count(); l_i++)
            {
                if (l_i == 0)
                {
                    m_HeaderPanel.SetPoints(
                        p_Leaderboard.Leaderboards[l_i].PointsData.Where((x) => x.PointsType == GuildApi.PASS_POINTS_TYPE).First());
                }

                m_Cells[l_i].SetGuildColor(m_GuildColor);
                var l_PointsType = p_Leaderboard.Leaderboards[l_i].PointsData.Where(x => x.PointsType == m_LeaderboardPanel.GetPointsSelector().GetSelectedValue()).First();
                m_Cells[l_i].SetPoints(l_PointsType);
                m_Cells[l_i].PlayAnimation(l_i, p_Leaderboard.Leaderboards[l_i], (int)p_Leaderboard.CustomData.MaxScore);
            }

            m_HeaderPanel.SetLevel(p_Leaderboard.CustomData.LevelValue, CustomLevelSelectionMenuReferences.SelectedCategory.Name);
            m_HeaderPanel.SetColor(m_GuildColor);
            
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public LeaderboardScoreDetailsModal GetScoreDetailsModal()
        {
            if (m_ScoreDetailsModal == null)
                m_ScoreDetailsModal = CreateModal<LeaderboardScoreDetailsModal>();
            return m_ScoreDetailsModal;
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public void ShowScoreDetailsModal()
        {
            ShowModal(GetScoreDetailsModal());
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        bool m_IsLeaderboardOnScoreSaber;

        public void OnScoreSaberButton()
        {
            if (m_IsLeaderboardOnScoreSaber)
            {
                HideScoreSaber();
            } else
            {
                ShowScoreSaber();
            }
        }

        public void ShowScoreSaber()
        {
            if (m_IsLeaderboardOnScoreSaber) return;

            var l_ScoreSaberButton = LevelSelectionViewController.Instance.GetMapDetails().GetShowScoreSaberButton();
            l_ScoreSaberButton.SetText("Hide ScoreSaber");
            l_ScoreSaberButton.SetInteractable(false);
            var l_LeaderboardViewController = Resources.FindObjectsOfTypeAll<PlatformLeaderboardViewController>().First();
            l_LeaderboardViewController.SetData(m_SelectedBeatmap);
            //l_LeaderboardViewController.Refresh(true, true);
            MTCoroutineStarter.Start(PresentViewControllerCoroutine(l_LeaderboardViewController, () => l_ScoreSaberButton.SetInteractable(true), AnimationDirection.Vertical, false));
            m_IsLeaderboardOnScoreSaber = true;
        }
        
        public void HideScoreSaber()
        {
            if (!m_IsLeaderboardOnScoreSaber) return;
            var l_ScoreSaberButton = LevelSelectionViewController.Instance.GetMapDetails().GetShowScoreSaberButton();
            l_ScoreSaberButton.SetText("Show ScoreSaber");
            l_ScoreSaberButton.SetInteractable(false);

            var l_LeaderboardViewController = Resources.FindObjectsOfTypeAll<PlatformLeaderboardViewController>().First();
            MTCoroutineStarter.Start(l_LeaderboardViewController.DismissViewControllerCoroutine(() => l_ScoreSaberButton.SetInteractable(true), AnimationDirection.Vertical, false)); ;
            m_IsLeaderboardOnScoreSaber = false;
        }

    }
}
