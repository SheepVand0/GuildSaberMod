using System.Collections.Generic;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage;
using GuildSaberProfile.API;
using TMPro;
using GuildSaberProfile.Utils.Color;
using UnityEngine.UI;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;

namespace GuildSaberProfile.UI.Components
{
    class LeaderboardScoreCell
    {
        #region UIComponents
        [UIComponent("Elems")] public HorizontalLayoutGroup m_ElemsLayout = null;
        #endregion

        #region Custom Components
        [UIValue("Rank")] private CustomText m_CRank = null;
        [UIValue("Name")] private CustomText m_CPlayerName = null;
        [UIValue("Points")] private CustomText m_CPoints = null;
        [UIValue("ScoreText")] private CustomText m_CScore = null;
        [UIValue("AccText")] private CustomText m_CAcc = null;
        #endregion

        UnityEngine.Color m_Red = new Utils.Color.Color(255, 0, 55).ToUnityColor();
        private static UnityEngine.Color m_Blue = new(0f, 0.7f, 1f, 0.8f);

        public float ScoreFontSize { get => 2.5f; set { } }
        public string Rank { get; set; }
        public string PlayerName { get; set; }
        public string Points { get; set; }
        public string Score { get; set; }
        public string Acc { get; set; }

        private bool m_HasBeenParsed = false;

        public LeaderboardScoreCell(int p_Rank, string p_Name, float p_Points, string p_PointsName, int p_Score, float p_Acc)
        {
            Init(p_Rank, p_Name, p_Points, p_PointsName, p_Score, p_Acc);
        }

        public void Init(int p_Rank, string p_Name, float p_Points, string p_PointsName, int p_Score, float p_Acc)
        {
            Rank = $"{p_Rank}";
            PlayerName = p_Name;
            Points = $"{p_Points.ToString("0.00")} {p_PointsName}";
            Score = p_Score.ToString();
            Acc = $"{p_Acc.ToString("00.00")}%";

            if (m_HasBeenParsed)
                SetTexts();
        }

        public void SetTexts()
        {
            m_CRank.SetText(Rank);
            m_CPlayerName.SetText(PlayerName);
            m_CPoints.SetText(Points);
            m_CScore.SetText($"{m_CScore.RichText}{Score}");
            m_CAcc.SetText(Acc);
        }

        [UIAction("#post-parse")]
        private void PostParse()
        {
            List<ItemParam> l_CurrentParams = new List<ItemParam>()
            {
                new ItemParam("FontSize", ScoreFontSize),
                new ItemParam("AnchorPosX", 0f),
                new ItemParam("Alignment", TextAlignmentOptions.Left),
                new ItemParam("LayoutAlignment", TextAnchor.MiddleLeft),
                new ItemParam("Color", UnityEngine.Color.white),
                new ItemParam("Italic", true),
                new ItemParam("RichText", string.Empty)
            };

            float l_OffsetFromRank = 5f;

            m_CRank = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

            l_CurrentParams[1] = new ItemParam("AnchorPosX", 3f+l_OffsetFromRank);

            m_CPlayerName = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

            l_CurrentParams[1] = new ItemParam("AnchorPosX", 31f+l_OffsetFromRank);
            l_CurrentParams[4] = new ItemParam("Color", m_Red);

            m_CPoints = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

            l_CurrentParams[1] = new ItemParam("AnchorPosX", -44f+l_OffsetFromRank);
            l_CurrentParams[2] = new ItemParam("Alignment", TextAlignmentOptions.Right);
            l_CurrentParams[4] = new ItemParam("Color", UnityEngine.Color.white);
            l_CurrentParams[6] = new ItemParam("RichText", "<mspace=0.5em>");

            m_CScore = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

            l_CurrentParams[1] = new ItemParam("AnchorPosX", -34f+l_OffsetFromRank);
            l_CurrentParams[4] = new ItemParam("Color", m_Red);
            l_CurrentParams[5] = new ItemParam("Italic", false);
            l_CurrentParams[6] = new ItemParam("RichText", string.Empty);

            m_CAcc = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

            m_HasBeenParsed = true;

            SetTexts();
        }

        public void Reset()
        {
            UnityEngine.Color l_White = UnityEngine.Color.white;
            m_CRank.Color = l_White;
            m_CPlayerName.Color = l_White;
            m_CScore.Color = l_White;
        }

        public void SetCurrentColorToCurrentPlayer()
        {
            m_CRank.Color = m_Blue;
            m_CPlayerName.Color = m_Blue;
            m_CScore.Color = m_Blue;
        }

        public void Destroy()
        {
            GameObject.DestroyImmediate(m_ElemsLayout);
        }

        public void SetEmpty()
        {
            Rank = string.Empty;
            PlayerName = string.Empty;
            Points = string.Empty;
            Score = string.Empty;
            Acc = string.Empty;

            SetTexts();
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
        public void SetScores(ApiCustomDataStruct p_CustomData, List<ApiMapLeaderboardContentStruct> p_Scores, string p_PointsNames)
        {
            foreach (LeaderboardScoreCell l_Current in m_Scores) {
                l_Current.Reset();
            }

            for (int l_i = 0; l_i < p_Scores.Count; l_i++)
            {
                ApiMapLeaderboardContentStruct l_Score = p_Scores[l_i];
                RankData l_RankData = new RankData();

                if (string.IsNullOrEmpty(p_PointsNames)) l_RankData = l_Score.RankData[0];
                else
                    foreach (RankData l_Current in l_Score.RankData)
                    {
                        if (l_Current.PointsName == p_PointsNames)
                        {
                            l_RankData = l_Current;
                        }
                    }
                if (m_Scores.ElementAtOrDefault(l_i) == null)
                {
                    m_Scores.Add(new LeaderboardScoreCell(l_Score.Rank, l_Score.Name, l_RankData.Points, l_RankData.PointsName, l_Score.BaseScore, (float)l_Score.BaseScore * 100 / p_CustomData.MaxScore));
                }
                else
                {
                    LeaderboardScoreCell l_CurrentCell = (LeaderboardScoreCell)m_Scores[l_i];
                    l_CurrentCell.Init(l_Score.Rank, l_Score.Name, l_RankData.Points, l_RankData.PointsName, l_Score.BaseScore, (float)l_Score.BaseScore * 100 / p_CustomData.MaxScore);
                    if (l_Score.ScoreSaberID.ToString() == Plugin.m_PlayerId)
                    {
                        Plugin.Log.Info("Setting");
                        //l_CurrentCell.SetCurrentColorToCurrentPlayer();
                    }
                }
            }

            m_ScoreList.tableView.ReloadData();

            for (int l_i = 0; l_i < Plugin.m_ScoresPerPage; l_i++)
            {
                if (l_i >= p_Scores.Count)
                    ((LeaderboardScoreCell)m_Scores[l_i]).SetEmpty();
            }
        }
    }
}
