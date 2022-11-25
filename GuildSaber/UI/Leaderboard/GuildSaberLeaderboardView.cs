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

        [UIComponent("ScoreParamsLayout")] internal readonly VerticalLayoutGroup m_ScoreParamsLayout = null;
        [UIComponent("WorldSelection")] readonly VerticalLayoutGroup m_ScopeSelectionLayout = null;
        [UIComponent("PageUpImage")] public readonly Button m_PageUpImage = null;
        [UIComponent("PageDownImage")] public readonly Button m_PageDownImage = null;

        LeaderboardScoreList m_ScoresList = null;
        public ScopeSelector m_ScopeSelector = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public static GuildSaberLeaderboardView m_Instance = null;

        public static bool Initialized { get; private set; } = false;

        public bool ChangingLeaderboard { get; private set; } = false;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Post Parse
        /// </summary>
        [UIAction("#post-parse")]
        private async void PostParse()
        {
            m_Instance = this;

            //Plugin.Log.Info("Creating GuildSaber leaderboard view");
            m_ScoresList = CustomUIComponent.CreateItem<LeaderboardScoreList>(m_ScoreParamsLayout.transform, true, true);
            m_ScopeSelector = CustomUIComponent.CreateItem<ScopeSelector>(m_ScopeSelectionLayout.transform, true, true);

            await WaitUtils.Wait(() => GuildSaberLeaderboardPanel.PanelInstance != null, 10);

            Initialized = true;
        }

        /// <summary>
        /// When page up button pressed
        /// </summary>
        [UIAction("PageUp")]
        private void PageUp()
        {
            m_ScoresList.PageUp();
        }

        /// <summary>
        /// When page down button pressed
        /// </summary>
        [UIAction("PageDown")]
        private void PageDown()
        {
            m_ScoresList.PageDown();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Set leaderboard mode
        /// </summary>
        /// <param name="p_Mode"></param>
        public void SetLeaderboardViewMode(ELeaderboardViewMode p_Mode)
        {
            try
            {
                m_ScoresList.m_ScoreList.gameObject.SetActive(p_Mode == ELeaderboardViewMode.Scores);
                m_ScopeSelectionLayout.gameObject.SetActive(p_Mode == ELeaderboardViewMode.Scores || (p_Mode == ELeaderboardViewMode.UnPassed && m_ScoresList.m_SelectedScope != ELeaderboardScope.Global));
                m_ScoresList.m_NotRankedText.gameObject.SetActive(p_Mode == ELeaderboardViewMode.UnPassed || p_Mode == ELeaderboardViewMode.NotRanked);
                m_ScoresList.m_ErrorText.gameObject.SetActive(p_Mode == ELeaderboardViewMode.Error);
                m_ScoresList.m_LoadingLayout.gameObject.SetActive(p_Mode == ELeaderboardViewMode.Loading);
                m_PageDownImage.gameObject.SetActive(p_Mode == ELeaderboardViewMode.Scores);
                m_PageUpImage.gameObject.SetActive(p_Mode == ELeaderboardViewMode.Scores);
                switch (p_Mode)
                {
                    case ELeaderboardViewMode.NotRanked:
                        m_ScoresList.m_NotRankedText.SetText("Map not ranked");
                        m_ScoresList.m_NotRankedText.color = Color.red;
                        LeaderboardHeaderManager.ChangeText(Localization.Get("TITLE_HIGHSCORES"));
                        break;
                    case ELeaderboardViewMode.UnPassed:
                        LeaderboardHeaderManager.ChangeText(Localization.Get("TITLE_HIGHSCORES"));
                        m_ScoresList.m_NotRankedText.SetText("Map un passed");
                        m_ScoresList.m_NotRankedText.color = Color.yellow;
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
            catch (Exception l_E)
            {
                GSLogger.Instance.Error(l_E, nameof(GuildSaberLeaderboardView), nameof(SetLeaderboardViewMode));
            }
        }

        public void OnLeaderboardSet(IDifficultyBeatmap difficultyBeatmap)
        {
            if (m_ScoresList == null) return;
            m_ScoresList.OnLeaderboardSet(difficultyBeatmap);
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
