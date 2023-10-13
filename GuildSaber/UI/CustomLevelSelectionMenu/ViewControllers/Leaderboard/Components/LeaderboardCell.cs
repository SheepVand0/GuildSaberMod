using CP_SDK.UI.Components;
using CP_SDK.XUI;
using GuildSaber.API;
using GuildSaber.Utils;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard.Components
{
    internal class LeaderboardCell : XUISecondaryButton
    {

        public const int CELL_HEIGHT = 2;

        XUIText m_RankText;
        XUIText m_NameText;
        XUIText m_PointsText;
        XUIText m_ScoreAccText;
        XUIText m_AccText;

        protected LeaderboardCell(string p_Name, string p_Label, Action p_OnClick = null) : base(p_Name, p_Label, p_OnClick)
        {
            OnReady(OnCreation);
            OnClick(OnButtonClicked);
        }

        public static LeaderboardCell Make()
        {
            return new LeaderboardCell("GuildSaberLeaderboardCell", string.Empty);
        }

        PointsData m_SelectedPointsType;

        Color m_GuildColor = new Color();

        ApiMapLeaderboardContentStruct m_Score;
        int m_MaxScore;

        bool m_Ready = false;

        private async void OnCreation(CSecondaryButton p_Button)
        {
            float l_FontSize = 2;

            XUIHLayout.Make(
                XUIHLayout.Make(
                    XUIText.Make("1").Bind(ref m_RankText).SetAlign(TMPro.TextAlignmentOptions.CaplineLeft),
                    //XUIText.Make("Sheep").Bind(ref m_NameText).SetAlign(TMPro.TextAlignmentOptions.Left),
                    //XUIText.Make("0.00").Bind(ref m_PointsText).SetAlign(TMPro.TextAlignmentOptions.Left),
                    XUIText.Make("").Bind(ref m_ScoreAccText).SetColor(new Color(1, 1, 1, 0.7f)).SetAlign(TMPro.TextAlignmentOptions.Right)
                //XUIText.Make(".00%").Bind(ref m_AccText).SetAlign(TMPro.TextAlignmentOptions.Right)
                )
                .ForEachDirect<XUIText>((x) =>
                {
                    x.SetFontSize(l_FontSize);
                    x.OnReady((y) =>
                    {
                        y.SetMargins(-15, 0, -15, 0);
                        y.LElement.minWidth = 20;
                        y.LElement.preferredWidth = 20;
                        y.RTransform.anchorMin = Vector2.zero;
                        y.RTransform.anchorMax = Vector2.one;
                        y.RTransform.sizeDelta = Vector2.zero;
                    });
                })
            )
            .SetWidth(80)
            .BuildUI(Element.LElement.transform);

            SetWidth(80);
            SetHeight(CELL_HEIGHT);


            p_Button.SetBackgroundColor(new Color(1, 1, 1, 0.7f));

            Texture2D l_Background = await TextureUtils.CreateFlatTexture(80 * 15, 2 * 15, Color.black);
            Texture2D l_BackgroundWithLine = await TextureUtils.AddLine(l_Background, l_Background.width, 1, 0, 0, Color.white.ColorWithAlpha(1));
            //l_BackgroundWithLine.mipMapBias = 1;

            //File.WriteAllBytes("eee.png", l_BackgroundWithLine.EncodeToPNG());

            SetBackgroundSprite(Sprite.Create(l_BackgroundWithLine, new Rect(0, 0, l_BackgroundWithLine.width, l_BackgroundWithLine.height), new Vector2()));
            
            m_Ready = true;
        }

        public void SetGuildColor(Color p_Color)
        {
            m_GuildColor = p_Color;
        }

        public void SetPoints(PointsData p_PointsType)
        {
            m_SelectedPointsType = p_PointsType;
        }

        public async void SetScore(ApiMapLeaderboardContentStruct p_Score, int p_MaxScore)
        {
            Element.SetBackgroundColor(new Color(1, 1, 1, 1));
            m_Score = p_Score;
            m_MaxScore = p_MaxScore;

            await WaitUtils.Wait(() => m_Ready == true, 1, p_MaxTryCount: 1000);

            var l_Points = p_Score.PointsData.Where(x => x.PointsType == m_SelectedPointsType.PointsType).First();

            string l_PointsStr = $"<color=#{UnityEngine.ColorUtility.ToHtmlStringRGB(m_GuildColor)}>{l_Points.Points:0.00} {l_Points.PointsName.ToUpper()}";

            if (l_Points.PointsType == GuildApi.PASS_POINTS_TYPE)
            {
                l_PointsStr = string.Empty;
            }

            float l_Acc = ((float)p_Score.ModifiedScore / p_MaxScore) * 100;

            Color l_RankNameColor = Color.white;
            if (p_Score.Name == GuildSaberModule.BasicPlayerData.Name)
                l_RankNameColor = Color.blue;

            
            m_RankText.SetText($"<color=#{ColorUtility.ToHtmlStringRGB(l_RankNameColor)}>{p_Score.Rank} <pos=15%>{GuildSaberUtils.GetPlayerNameToFit(p_Score.Name, 18)} <pos=60%>{l_PointsStr}");
            m_ScoreAccText.SetText($"{p_Score.ModifiedScore} <pos=20%>{l_Acc:0.00}%");

        }

        public void Hide()
        {
            Element.SetBackgroundColor(new Color(0, 0, 0, 0));
            m_RankText.SetText(string.Empty);
            m_ScoreAccText.SetText(string.Empty);
        }

        private void OnButtonClicked()
        {
            GuildSaberLeaderboardViewController.Instance.ShowScoreDetailsModal();
            GuildSaberLeaderboardViewController.Instance.SetScoreDetailsModalData(m_Score, m_MaxScore);
        }
    }
}
