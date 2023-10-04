using CP_SDK.UI.Components;
using CP_SDK.XUI;
using GuildSaber.API;
using GuildSaber.UI.Leaderboard.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard.Components
{
    internal class LeaderboardCell : XUISecondaryButton
    {
        
        XUIText m_RankText;
        XUIText m_NameText;
        XUIText m_PointsText;
        XUIText m_ScoreText;
        XUIText m_AccText;

        protected LeaderboardCell(string p_Name, string p_Label, Action p_OnClick = null) : base(p_Name, p_Label, p_OnClick)
        {
            OnReady(OnCreation);
        }

        public static LeaderboardCell Make()
        {
            return new LeaderboardCell("GuildSaberLeaderboardCell", string.Empty);
        }

        PointsData m_SelectedPointsType;

        private void OnCreation(CSecondaryButton p_Button)
        {
            float l_FontSize = 2;

            XUIHLayout.Make(
                XUIHLayout.Make(
                    XUIText.Make("1 - ")
                        .SetAlign(TMPro.TextAlignmentOptions.Left)
                        .SetFontSize(l_FontSize)
                        .Bind(ref m_RankText),
                    XUIText.Make("Sheep").Bind(ref m_NameText)
                        .SetFontSize(l_FontSize)
                        .SetAlign(TMPro.TextAlignmentOptions.Left),
                    XUIText.Make("0.00").Bind(ref m_PointsText)
                        .SetFontSize(l_FontSize)
                        .SetAlign(TMPro.TextAlignmentOptions.Left)
                ).SetWidth(40),
                XUIHLayout.Make(
                    XUIText.Make("69000000").Bind(ref m_ScoreText).SetColor(new UnityEngine.Color(1, 1, 1, 0.7f))
                        .SetFontSize(l_FontSize)
                        .SetAlign(TMPro.TextAlignmentOptions.Right),
                    XUIText.Make("69.00%").Bind(ref m_AccText)
                        .SetFontSize(l_FontSize)
                        .SetAlign(TMPro.TextAlignmentOptions.Right)
                )
            )
            .BuildUI(Element.LElement.transform);
            SetWidth(80);
            SetHeight(4);
            Element.SetBackgroundColor(new Color(0, 0, 0, 0.7f));
        }

        public void SetGuildColor(Color p_Color)
        {
            m_PointsText.SetColor(p_Color);
        }

        public void SetPointsType(PointsData p_PointsType)
        {
            m_SelectedPointsType = p_PointsType;
        }

        public void SetScore(ApiMapLeaderboardContentStruct p_Score, int p_MaxScore)
        {
            m_RankText.SetText($"{p_Score.Rank} - ");
            m_NameText.SetText($"{p_Score.Name}");
            var l_Points = p_Score.PointsData.Where(x => x.PointsType == m_SelectedPointsType.PointsType).First();
            m_PointsText.SetText($"{l_Points.Points:0,00} {l_Points.PointsName.ToUpper()}");
            m_ScoreText.SetText($"{p_Score.ModifiedScore}");
            float l_Acc = (p_Score.ModifiedScore / p_MaxScore) * 100;
            m_AccText.SetText($"{l_Acc:0,00}%");
        }

    }
}
