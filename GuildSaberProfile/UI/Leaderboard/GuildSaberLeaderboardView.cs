using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberMarkupLanguage.Components;
using GuildSaberProfile.API;
using GuildSaberProfile.Utils;
using GuildSaberProfile.UI.Components;
using UnityEngine.UI;
using UnityEngine;

namespace GuildSaberProfile.UI.GuildSaber.Leaderboard
{
    [HotReload(RelativePathToLayout = @"LeaderboardView.bsml")]
    [ViewDefinition("GuildSaberProfile.UI.GuildSaber.View.LeaderboardView.bsml")]
    public class GuildSaberLeaderboardView : BSMLAutomaticViewController
    {
        [UIComponent("VerticalElems")] VerticalLayoutGroup m_VerticalElems = null;
        [UIComponent("ScoreParamsLayout")] HorizontalLayoutGroup m_ScoreParamsLayout = null;

        private GuildSaberLeaderboardPanel _LeaderboardPanel;

        public string m_CurrentPointsName { get; internal set; }
        public string m_CurrentMapHash { get; internal set; }
        public IDifficultyBeatmap m_CurrentBeatmap { get; internal set; }
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

            Events.m_Instance.e_OnBeatmapSelected += OnBeatmapSelected;
            Events.m_Instance.e_OnPointsTypeChange += OnPointsTypeChange;
            Events.m_Instance.e_OnGuildSelected += OnGuildSelected;
            Events.e_OnLeaderboardShown += OnLeaderboardShow;
        }

        private void OnLeaderboardShow()
        {
            GetLeaderboard(_LeaderboardPanel.m_SelectedGuild);
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

        public void GetLeaderboard(string p_Guild)
        {
            Plugin.Log.Info("Getting Leaderboard");
            string l_Hash = GSBeatmapUtils.DifficultyBeatmapToHash(m_CurrentBeatmap);
            m_Leaderboard = GuildApi.GetLeaderboard(p_Guild, l_Hash, m_CurrentBeatmap,0, null, null, 10);
            if (m_Leaderboard.Leaderboards == null) return;
            Plugin.Log.Info(m_Leaderboard.Leaderboards.Count.ToString() + " Scores found");
            m_ScoresList.SetScores(m_Leaderboard.CustomData, m_Leaderboard.Leaderboards, m_CurrentPointsName);
        }

        #region Events
        private void OnBeatmapSelected(StandardLevelDetailViewController p_LevelDetailViewController, IDifficultyBeatmap p_Map)
        {
            m_CurrentBeatmap = p_Map;

            if (Events.m_IsGuildSaberLeaderboardShown)
                GetLeaderboard(_LeaderboardPanel.m_SelectedGuild);
        }
        #endregion
    }
}
