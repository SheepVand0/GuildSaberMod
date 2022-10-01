using System.Collections.Generic;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage;
using GuildSaber.API;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using HMUI;
using GuildSaber.AssetBundles;
using IPA.Utilities;
using System.Collections;
using GuildSaber.UI.GuildSaber.Leaderboard;
using GuildSaber.Utils;
using ModestTree;

namespace GuildSaber.UI.Components
{
    class LeaderboardScoreCell
    {
        public static Material m_LineMat;

        #region UIComponents
        [UIComponent("Elems")] public HorizontalLayoutGroup m_ElemsLayout = null;
        [UIComponent("Interactable")] public Button m_Interactable = null;
        [UIComponent("Info")] public ModalView m_InfoModal = null;
        //[UIComponent("InfoName")] private TextMeshProUGUI m_InfoName = null;
        //[UIComponent("InfoRank")]
        #endregion

        #region Custom Components
        [UIValue("Rank")] private CustomText m_CRank = null;
        [UIValue("Name")] private CustomText m_CPlayerName = null;
        [UIValue("Points")] private CustomText m_CPoints = null;
        [UIValue("ScoreText")] private CustomText m_CScore = null;
        [UIValue("AccText")] private CustomText m_CAcc = null;
        [UIValue("Modifiers")] private CustomText m_Modifiers = null;
        #endregion

        UnityEngine.Color m_Red = new Utils.Color.Color(255, 0, 55).ToUnityColor();
        private static UnityEngine.Color m_Blue = new(0f, 0.8f, 1f, 0.8f);

        public static float ScaleFactor { get => 1.2f; }
        private float InteractableScaleY { get => (6.5f * ScaleFactor) * 0.9f; }
        private float ScoreFontSize { get => 2.5f * ScaleFactor; }
        private float LeaderWidth { get => 90 * ScaleFactor + (7* 1.4f); }

        float BasePoints { get; set; }
        public string Rank { get; set; }
        public string PlayerName { get; set; }
        public string Points { get; set; }
        public string Score { get; set; }
        public string Acc { get; set; }
        public string Id { get; set; }
        public string Modifiers { get; set; }

        private bool m_HasBeenParsed = false;

        public LeaderboardScoreCell(int p_Rank, string p_Name, float p_Points, string p_PointsName, int p_Score, float p_Acc, string p_Id, string p_Modifiers)
        {
            Init(p_Rank, p_Name, p_Points, p_PointsName, p_Score, p_Acc, p_Id, p_Modifiers);
        }

        public void Init(int p_Rank, string p_Name, float p_Points, string p_PointsName, int p_Score, float p_Acc, string p_Id, string p_Modfiers)
        {
            Rank = $"{p_Rank}";
            PlayerName = p_Name;
            BasePoints = p_Points;
            Points = $"{p_Points.ToString("0.00")} {p_PointsName}";
            Score = p_Score.ToString();
            Acc = $"{p_Acc.ToString("00.00")}%";
            Id = p_Id;
            Modifiers = GuildSaberUtils.GetPlayerNameToFit(p_Modfiers, 4);

            if (m_HasBeenParsed)
                SetTexts();
        }

        public void SetTexts()
        {
            m_CRank.SetText(Rank);
            m_CPlayerName.SetText(GuildSaberUtils.GetPlayerNameToFit(PlayerName, 22));
            m_CPoints.SetText(Points);
            m_CScore.SetText($"{m_CScore.RichText}{Score}");
            m_CAcc.SetText(Acc);
            m_Modifiers.SetText(Modifiers);
        }

        [UIAction("#post-parse")]
        private void _PostParse()
        {
            List<ItemParam> l_CurrentParams = new List<ItemParam>()
                {
                    new ItemParam("FontSize", ScoreFontSize),
                    new ItemParam("AnchorPosX", 2f),
                    new ItemParam("Alignment", TextAlignmentOptions.Left),
                    new ItemParam("LayoutAlignment", TextAnchor.MiddleLeft),
                    new ItemParam("Color", UnityEngine.Color.white),
                    new ItemParam("Italic", true),
                    new ItemParam("RichText", string.Empty)
                };

            float l_OffsetFromRank = 7f;
            float l_NameOffset = -4f;

            m_CRank = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

            l_CurrentParams[1] = new ItemParam("AnchorPosX", (3f + l_OffsetFromRank) + (ScaleFactor) * 0.7f);

            m_CPlayerName = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

            l_CurrentParams[1] = new ItemParam("AnchorPosX", (34f + l_OffsetFromRank + l_NameOffset) + (ScaleFactor * 1.02f));
            l_CurrentParams[4] = new ItemParam("Color", m_Red);

            m_CPoints = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

            l_CurrentParams[1] = new ItemParam("AnchorPosX", (-40f + l_OffsetFromRank + l_NameOffset) + (ScaleFactor * 1.04f));
            l_CurrentParams[2] = new ItemParam("Alignment", TextAlignmentOptions.Right);
            l_CurrentParams[4] = new ItemParam("Color", UnityEngine.Color.white);
            l_CurrentParams[6] = new ItemParam("RichText", "<mspace=0.5em>");

            m_CScore = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

            l_CurrentParams[0] = new ItemParam("FontSize", ScoreFontSize / ScaleFactor);
            l_CurrentParams[1] = new ItemParam("AnchorPosX", (-36f + l_OffsetFromRank + l_NameOffset) + (ScaleFactor * 1.06f));
            l_CurrentParams[4] = new ItemParam("Color", Color.grey);
            l_CurrentParams[5] = new ItemParam("Italic", false);
            l_CurrentParams[6] = new ItemParam("RichText", string.Empty);

            m_Modifiers = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

            l_CurrentParams[0] = new ItemParam("FontSize", ScoreFontSize);
            l_CurrentParams[1] = new ItemParam("AnchorPosX", (-25f + l_OffsetFromRank + l_NameOffset) + (ScaleFactor * 1.06f));
            l_CurrentParams[4] = new ItemParam("Color", m_Red);

            m_CAcc = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

            m_HasBeenParsed = true;

            SetTexts();
        }

        private bool m_IsTheCurrentPlayer = false;

        public void SetInfo(string p_ScoreSaberId)
        {
            ApiPlayerData l_Info = GuildApi.GetPlayerByScoreSaberIdAndGuild(p_ScoreSaberId, GuildSaberLeaderboardPanel.m_Instance.m_SelectedGuild);
        }

        public void Reset()
        {
            m_IsTheCurrentPlayer = false;
            UnityEngine.Color l_White = UnityEngine.Color.white;
            m_CRank.SetColor(l_White);
            m_CPlayerName.SetColor(l_White);
            m_CScore.SetColor(l_White);
            m_ElemsLayout.gameObject.SetActive(false);
        }

        public void SetCurrentColorToCurrentPlayer()
        {
            m_IsTheCurrentPlayer = true;
        }

        public void Destroy()
        {
            GameObject.DestroyImmediate(m_ElemsLayout.gameObject);
        }

        public async void Actualize()
        {
            BeatSaberPlus.SDK.UI.Backgroundable.SetOpacity(m_Interactable.gameObject, 0f);

            if (!m_IsTheCurrentPlayer) return;

            m_CRank.SetColor(m_Blue);
            m_CPlayerName.SetColor(m_Blue);
            m_CScore.SetColor(m_Blue);
        }

        public void SetEmpty()
        {
            Rank = string.Empty;
            PlayerName = string.Empty;
            Points = string.Empty;
            Score = string.Empty;
            Acc = string.Empty;
            Modifiers = string.Empty;

            SetTexts();
        }

        public void Hide()
        {
            m_ElemsLayout.gameObject.SetActive(false);
            m_Interactable.gameObject.SetActive(false);
        }

        public void Show()
        {
            m_ElemsLayout.gameObject.SetActive(true);
            m_Interactable.gameObject.SetActive(true);
            Actualize();
        }
    }

    class LeaderboardScoreList : CustomUIComponent
    {
        protected override string m_ViewResourceName => "GuildSaber.UI.Components.Views.LeaderboardScoreList.bsml";

        #region UIComponent
        [UIComponent("ScoreList")] CustomCellListTableData m_ScoreList = null;
        #endregion

        private float ListCellSize { get => 4 * LeaderboardScoreCell.ScaleFactor; }

        #region UIValues
        [UIValue("ScoreCells")] List<object> m_Scores = new List<object>();
        #endregion
        public void SetScores(MapLeaderboardCustomData p_CustomData, List<MapLeaderboardContent> p_Scores, string p_PointsNames)
        {
            Plugin.Log.Info(p_Scores.Count.ToString());

            bool l_IsCellHasBeenCreated = false;
            foreach (LeaderboardScoreCell l_Current in m_Scores)
            {
                l_Current.Reset();
                l_Current.Hide();
            }

            for (int l_i = 0; l_i < p_Scores.Count; l_i++)
            {
                MapLeaderboardContent l_Score = p_Scores[l_i];
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
                    m_Scores.Add(new LeaderboardScoreCell(l_Score.Rank, l_Score.Name, l_RankData.Points, l_RankData.PointsName, l_Score.BaseScore, (float)l_Score.BaseScore * 100 / p_CustomData.MaxScore, l_Score.ScoreSaberID.ToString(), l_Score.Modifiers));
                    l_IsCellHasBeenCreated = true;
                }
                else
                {
                    LeaderboardScoreCell l_CurrentCell = (LeaderboardScoreCell)m_Scores[l_i];
                    l_CurrentCell.Init(l_Score.Rank, l_Score.Name, l_RankData.Points, l_RankData.PointsName, l_Score.BaseScore, (float)l_Score.BaseScore * 100 / p_CustomData.MaxScore, l_Score.ScoreSaberID.ToString(), l_Score.Modifiers);
                    if (l_Score.ScoreSaberID.ToString() == Plugin.m_PlayerId)
                    {
                        l_CurrentCell.SetCurrentColorToCurrentPlayer();
                    }
                }
            }

            if (l_IsCellHasBeenCreated)
                m_ScoreList.tableView.ReloadData();

            for (int l_i = 0; l_i < Plugin.m_ScoresPerPage - (Plugin.m_ScoresPerPage - p_Scores.Count); l_i++)
            {
                if (l_i >= p_Scores.Count)
                    ((LeaderboardScoreCell)m_Scores[l_i]).Hide();
                else
                {
                    ((LeaderboardScoreCell)m_Scores[l_i]).Actualize();
                    ((LeaderboardScoreCell)m_Scores[l_i]).Show();
                }
            }
        }
    }
}
