using System.Collections.Generic;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using GuildSaberProfile.API;
using TMPro;
using GuildSaberProfile.Utils.Color;

namespace GuildSaberProfile.UI.Components
{
    class LeaderboardScoreCell
    {
        #region UIComponents
        [UIComponent("Rank")] private TextMeshProUGUI m_CRank = null;
        [UIComponent("Name")] private TextMeshProUGUI m_CPlayerName = null;
        [UIComponent("Points")] private TextMeshProUGUI m_CPoints = null;
        [UIComponent("Score")] private TextMeshProUGUI m_CScore = null;
        [UIComponent("Acc")] private TextMeshProUGUI m_CAcc = null;
        #endregion

        public int ScoreFontSize { get => 4; set { } }

        public string Rank { get; set; }
        public string PlayerName { get; set; }
        public string Points { get; set; }
        public string Score { get; set; }
        public string Acc { get; set; }

        public LeaderboardScoreCell(int p_Rank, string p_Name, float p_Points, string p_PointsName, int p_Score, float p_Acc)
        {
            Rank = p_Rank.ToString();
            PlayerName = p_Name;
            Points = $"{p_Points.ToString("0.00")} {p_PointsName}";
            Score = p_Score.ToString();
            Acc = $"{p_Acc.ToString("00.00")}%";
        }

        [UIAction("#post-parse")]
        private void PostParse()
        {
            UnityEngine.Color l_Red = new Color(255, 0, 55).ToUnityColor();
            m_CPoints.color = l_Red;
            m_CAcc.color = l_Red;
        }
    }

    class LeaderboardScoreList : CustomUIComponent
    {
        protected override string m_ViewResourceName => "GuildSaberProfile.UI.Components.Views.LeaderboardScoreList.bsml";

        #region UIComponent
        [UIComponent("ScoreList")] CustomCellListTableData m_ScoreList = null;
        #endregion

        #region UIValues
        [UIValue("ScoreCells")] List<object> m_Scores = new List<object>();
        #endregion

        public override void OnCreate(){}

        public void SetScores(ApiCustomDataStruct p_CustomData, List<ApiMapLeaderboardContentStruct> p_Scores, string p_PointsNames)
        {
            m_Scores.Clear();
            for (int l_i = 0; l_i < p_Scores.Count; l_i++)
            {
                ApiMapLeaderboardContentStruct l_Score = p_Scores[l_i];
                RankData l_RankData = new RankData();
                if (string.IsNullOrEmpty(p_PointsNames)) l_RankData = l_Score.RankData[0];
                else
                    foreach (RankData l_Current in l_Score.RankData)
                    {
                        Plugin.Log.Info($"{l_Current.PointsName} && {p_PointsNames}");
                        if (l_Current.PointsName == p_PointsNames)
                        {
                            l_RankData = l_Current;
                            Plugin.Log.Info($"2 -> {l_Current.Points}");
                        }
                    }
                m_Scores.Add(new LeaderboardScoreCell(l_Score.Rank,l_Score.Name,l_RankData.Points, l_RankData.PointsName, l_Score.BaseScore,(float)l_Score.BaseScore * 100 / p_CustomData.MaxScore));
            }
            m_ScoreList.tableView.ReloadData();
        }
    }
}
