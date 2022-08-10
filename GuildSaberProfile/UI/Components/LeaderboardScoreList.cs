using System.Collections.Generic;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using GuildSaberProfile.API;
using TMPro;
using GuildSaberProfile.Utils.Color;
using UnityEngine.UI;
using UnityEngine;

namespace GuildSaberProfile.UI.Components
{
    class LeaderboardScoreCell
    {
        #region UIComponents
        [UIComponent("RankAndNameH")] private HorizontalLayoutGroup m_RankAndNameLayout = null;
        [UIComponent("ScoreH")] private HorizontalLayoutGroup m_ScoreLayout = null;
        [UIComponent("Points")] private TextMeshProUGUI m_CPoints = null;
        #endregion

        #region Custom Components
        [UIValue("Rank")] private CustomText m_CRank = null;
        [UIValue("ScoreText")] private CustomText m_CScore = null;
        [UIValue("Name")] private CustomText m_CPlayerName = null;
        [UIValue("AccText")] private CustomText m_CAcc = null;
        #endregion

        public int ScoreFontSize { get => 3; set { } }
        public string Rank { get; set; }
        public string PlayerName { get; set; }
        public string Points { get; set; }
        public string Score { get; set; }
        public string Acc { get; set; }

        public LeaderboardScoreCell(int p_Rank, string p_Name, float p_Points, string p_PointsName, int p_Score, float p_Acc)
        {
            Rank = $"{p_Rank}";
            PlayerName = p_Name;
            Points = $"{p_Points.ToString("0.00")} {p_PointsName}";
            Score = p_Score.ToString();
            Acc = $"{p_Acc.ToString("00.00")}%";
        }

        [UIAction("#post-parse")]
        private void PostParse()
        {
            UnityEngine.Color l_Red = new Utils.Color.Color(255, 0, 55).ToUnityColor();
            m_CPoints.color = l_Red;

            List<ItemParam> l_CurrentParams = new List<ItemParam>()
            {
                new ItemParam("FontSize", ScoreFontSize),
                new ItemParam("AnchorPosX", -14f),
                new ItemParam("Text", Rank.ToString()),
                new ItemParam("Alignment", TextAlignmentOptions.Left),
                new ItemParam("LayoutAlignment", TextAnchor.MiddleLeft)
            };

            m_CRank = CustomUIComponent.CreateItemWithParams<CustomText>(m_RankAndNameLayout.transform, true, true, l_CurrentParams);

            l_CurrentParams[1] = new ItemParam("AnchorPosX", -10f);
            l_CurrentParams[2] = new ItemParam("Text", PlayerName);

            m_CPlayerName = CustomUIComponent.CreateItemWithParams<CustomText>(m_RankAndNameLayout.transform, true, true, l_CurrentParams);

            l_CurrentParams[1] = new ItemParam("AnchorPosX", -4f);
            l_CurrentParams[2] = new ItemParam("Text", Score.ToString());
            l_CurrentParams[3] = new ItemParam("Alignment", TextAlignmentOptions.Right);

            m_CScore = CustomUIComponent.CreateItemWithParams<CustomText>(m_ScoreLayout.transform, true, true, l_CurrentParams);

            l_CurrentParams[1] = new ItemParam("AnchorPosX", 1f);
            l_CurrentParams[2] = new ItemParam("Text", Acc.ToString());

            m_CAcc = CustomUIComponent.CreateItemWithParams<CustomText>(m_ScoreLayout.transform, true, true, l_CurrentParams);
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
