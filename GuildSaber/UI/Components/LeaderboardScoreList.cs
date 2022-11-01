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
using CP_SDK.Unity;
using GuildSaber.BSPModule;
using OVR.OpenVR;

namespace GuildSaber.UI.Components
{
    internal class LeaderboardScoreCell
    {

        [UIComponent("Elems")] public HorizontalLayoutGroup m_ElemsLayout = null;
        [UIComponent("Interactable")] public Button m_Interactable = null;
        [UIComponent("Info")] public ModalView m_InfoModal = null;
        //[UIComponent("InfoName")] private TextMeshProUGUI m_InfoName = null;
        //[UIComponent("InfoRank")]

        [UIValue("Rank")] private CustomText m_CRank = null;
        [UIValue("Name")] private CustomText m_CPlayerName = null;
        [UIValue("Points")] private CustomText m_CPoints = null;
        [UIValue("ScoreText")] private CustomText m_CScore = null;
        [UIValue("AccText")] private CustomText m_CAcc = null;
        [UIValue("Modifiers")] private CustomText m_CModifiers = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        UnityEngine.Color m_Red = new Utils.Color.Color(255, 0, 55).ToUnityColor32();
        private static UnityEngine.Color m_Blue = new(0f, 0.8f, 1f, 0.8f);

        public static float ScaleFactor { get => 1.2f; }
        private float InteractableScaleY { get => (6.5f * ScaleFactor) * 0.9f; }
        private float ScoreFontSize { get => 2.5f * ScaleFactor; }
        private float LeaderWidth { get => 90 * ScaleFactor + (7 * 1.4f); }

        public bool HasBeenParsed { get; private set; } = false;

        float BasePoints { get; set; } = 0f;
        public string Rank { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public string Points { get; set; } = string.Empty;
        public string Score { get; set; } = string.Empty;
        public string Acc { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Modifiers { get; set; } = string.Empty;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Update cell on leaderboard refresh
        /// </summary>
        /// <param name="p_Rank">Player rank in this map</param>
        /// <param name="p_Name">player name</param>
        /// <param name="p_Points">Points</param>
        /// <param name="p_PointsName">Selected points type name</param>
        /// <param name="p_Score">Score</param>
        /// <param name="p_Acc">Acc</param>
        /// <param name="p_Id">Player GuildSaber Id</param>
        /// <param name="p_Modfiers">Modifiers</param>
        internal void Init(int p_Rank, string p_Name, float p_Points, string p_PointsName, int p_Score, float p_Acc, string p_Id, string p_Modfiers)
        {
            Rank = $"{p_Rank}";
            PlayerName = p_Name;
            BasePoints = p_Points;
            Points = $"{p_Points.ToString("0.00")} {p_PointsName}";
            Score = p_Score.ToString();
            Acc = $"{p_Acc.ToString("00.00")}%";
            Id = p_Id;
            Modifiers = GuildSaberUtils.GetPlayerNameToFit(p_Modfiers, 4);
            Show();
        }

        /// <summary>
        /// PostParse
        /// </summary>
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
                    new ItemParam("RichText", string.Empty),
                    new ItemParam("EnableRichText", true)
                };

            float l_OffsetFromRank = 7f;
            float l_NameOffset = -1f;

            Color l_StartColor = GuildSaberModule.m_LeaderboardSelectedGuild.Color.ToUnityColor();

            m_CRank = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

            l_CurrentParams[1] = new ItemParam("AnchorPosX", (3f + l_OffsetFromRank) + (ScaleFactor) * 0.7f);
            l_CurrentParams[7] = new ItemParam("EnableRichText", false);

            m_CPlayerName = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

            l_CurrentParams[1] = new ItemParam("AnchorPosX", (34f + l_OffsetFromRank + l_NameOffset) + (ScaleFactor * 1.02f));
            l_CurrentParams[4] = new ItemParam("Color", l_StartColor);
            l_CurrentParams[7] = new ItemParam("EnableRichText", true);

            m_CPoints = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

            l_CurrentParams[1] = new ItemParam("AnchorPosX", (-40f + l_OffsetFromRank + l_NameOffset) + (ScaleFactor * 1.04f));
            l_CurrentParams[2] = new ItemParam("Alignment", TextAlignmentOptions.Right);
            l_CurrentParams[4] = new ItemParam("Color", UnityEngine.Color.white);
            l_CurrentParams[6] = new ItemParam("RichText", "<mspace=0.5em>");

            m_CScore = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

            l_CurrentParams[0] = new ItemParam("FontSize", ScoreFontSize / ScaleFactor);
            l_CurrentParams[1] = new ItemParam("AnchorPosX", (-32.5f + l_OffsetFromRank + l_NameOffset) + (ScaleFactor * 1.06f));
            l_CurrentParams[4] = new ItemParam("Color", Color.grey);
            l_CurrentParams[5] = new ItemParam("Italic", false);
            l_CurrentParams[6] = new ItemParam("RichText", string.Empty);

            m_CModifiers = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

            l_CurrentParams[0] = new ItemParam("FontSize", ScoreFontSize);
            l_CurrentParams[1] = new ItemParam("AnchorPosX", (-24f + l_OffsetFromRank + l_NameOffset) + (ScaleFactor * 1.06f));
            l_CurrentParams[4] = new ItemParam("Color", l_StartColor);

            m_CAcc = CustomUIComponent.CreateItemWithParams<CustomText>(m_ElemsLayout.transform, true, true, l_CurrentParams);

            HasBeenParsed = true;

            Events.m_Instance.e_OnGuildSelected += OnGuildSelected;
        }

        /// <summary>
        /// Apply all values on texts
        /// </summary>
        internal void SetTexts()
        {
            m_CRank.SetText(Rank);
            m_CPlayerName.SetText(GuildSaberUtils.GetPlayerNameToFit(PlayerName, 22));
            m_CPoints.SetText(Points);

            m_CScore.SetText($"{m_CScore.RichText}{Score}");
            m_CAcc.SetText(Acc);
            m_CModifiers.SetText(Modifiers);
        }

        /// <summary>
        /// Reset cell colors
        /// </summary>
        internal void Reset()
        {
            SetEmpty();
            UnityEngine.Color l_White = UnityEngine.Color.white;
            m_CRank.SetColor(l_White);
            m_CPlayerName.SetColor(l_White);
            m_CScore.SetColor(l_White);

            m_Interactable.gameObject.SetActive(false);
        }

        /// <summary>
        /// Set current cell to player to highlight it in blue
        /// </summary>
        internal void SetCellToCurrentPlayer()
        {
            m_CRank.SetColor(m_Blue);
            m_CPlayerName.SetColor(m_Blue);
            m_CScore.SetColor(m_Blue);
        }

        /// <summary>
        /// Show it when have a score
        /// </summary>
        internal void Show()
        {
            SetTexts();

            m_Interactable.gameObject.SetActive(true);
        }
        /// <summary>
        /// Set texts to empty
        /// </summary>
        public void SetEmpty()
        {
            Rank = string.Empty;
            PlayerName = string.Empty;
            Points = string.Empty;
            Score = string.Empty;
            Acc = string.Empty;
            Modifiers = string.Empty;

            m_CRank.SetText(string.Empty);
            m_CPlayerName.SetText(string.Empty);
            m_CPoints.SetText(string.Empty);
            m_CScore.SetText(string.Empty);
            m_CAcc.SetText(string.Empty);
            m_CModifiers.SetText(string.Empty);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private void OnGuildSelected(int p_Guild)
        {
            //m_CAcc, m_CPoints
            Color l_NewColor = GuildSaberModule.m_LeaderboardSelectedGuild.Color.ToUnityColor();
            m_CAcc.SetColor(l_NewColor);
            m_CPoints.SetColor(l_NewColor);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        internal void SetInfo(string p_ScoreSaberId)
        {
            ApiPlayerData l_Info = GuildApi.GetPlayerByScoreSaberIdAndGuild(p_ScoreSaberId, GuildSaberLeaderboardPanel.Instance.m_SelectedGuild);
        }

    }

    class LeaderboardScoreList : CustomUIComponent
    {
        protected override string m_ViewResourceName => "GuildSaber.UI.Components.Views.LeaderboardScoreList.bsml";

        [UIComponent("ScoreList")] CustomCellListTableData m_ScoreList = null;

        [UIValue("ScoreCells")] List<object> m_Scores = new List<object>();

        public bool Initialized { get; private set; }

        private float ListCellSize { get => 4 * LeaderboardScoreCell.ScaleFactor; }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Set current scores to list
        /// </summary>
        /// <param name="p_CustomData"></param>
        /// <param name="p_Scores"></param>
        /// <param name="p_PointsNames"></param>
        public async void SetScores(MapLeaderboardCustomData p_CustomData, List<MapLeaderboardContent> p_Scores, string p_PointsNames)
        {
            GuildSaberLeaderboardView.m_Instance.SetLeaderboardViewMode(ELeaderboardViewMode.Scores);

            if (Initialized == false)
            {
                for (int l_i = 0; l_i < GuildSaberModule.SCORES_BY_PAGE; l_i++)
                    m_Scores.Add(new LeaderboardScoreCell());

                m_ScoreList.tableView.ReloadData();

                Initialized = true;
            }

            LeaderboardScoreCell l_Cell = (LeaderboardScoreCell)m_Scores[GuildSaberModule.SCORES_BY_PAGE - 1];

            if (l_Cell.HasBeenParsed == false)
                await WaitUtils.WaitUntil(() => l_Cell.HasBeenParsed, 50, 0);


            foreach (LeaderboardScoreCell l_Current in m_Scores)
            {
                l_Current.Reset();
            }

            await Task.Delay(100);

            for (int l_i = 0; l_i < p_Scores.Count; l_i++)
            {
                MapLeaderboardContent l_Score = p_Scores[l_i];
                RankData l_RankData = new RankData();

                if (string.IsNullOrEmpty(p_PointsNames)) l_RankData = l_Score.RankData[0];
                else
                {
                    foreach (RankData l_Current in l_Score.RankData)
                    {
                        if (l_Current.PointsName == p_PointsNames)
                        {
                            l_RankData = l_Current;
                        }
                    }
                }
                LeaderboardScoreCell l_CurrentCell = (LeaderboardScoreCell)m_Scores[l_i];
                l_CurrentCell.Init(l_Score.Rank, l_Score.Name, l_RankData.Points, l_RankData.PointsName, l_Score.BaseScore, (float)l_Score.BaseScore * 100 / p_CustomData.MaxScore, l_Score.ScoreSaberID.ToString(), l_Score.Modifiers);
                if (l_Score.ID == BSPModule.GuildSaberModule.m_GSPlayerId)
                {
                    l_CurrentCell.SetCellToCurrentPlayer();
                }
            }
        }
    }
}
