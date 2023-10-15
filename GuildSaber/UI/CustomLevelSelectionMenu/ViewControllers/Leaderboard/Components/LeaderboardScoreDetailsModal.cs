using BeatmapEditor3D;
using CP_SDK.UI;
using CP_SDK.UI.Components;
using CP_SDK.XUI;
using GuildSaber.API;
using GuildSaber.UI.Defaults;
using GuildSaber.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TMPro;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;

namespace GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard.Components
{
    internal class LeaderboardScoreDetailsModal : IModal
    {
        bool IsCreated = false;

        XUIText m_NameText;
        XUIText m_ScoreText;
        XUIText m_AccuracyText;
        XUIText m_ModifiersText;
        XUIText m_FullComboText;
        XUIText m_MissText;
        XUIText m_BadCutsText;
        XUIText m_PassStateText;
        XUIText m_PauseCountText;
        XUIText m_HMDText;
        XUIText m_TimeText;

        public override void OnClose()
        {
            ModalUtils.OnModalHide(this);
        }

        public override void OnShow()
        {
            if (IsCreated == false)

            XUIVLayout.Make(
                XUIText.Make("Name").Bind(ref m_NameText),
                XUIText.Make("Score").Bind(ref m_ScoreText),
                XUIText.Make("Accuracy").Bind(ref m_AccuracyText),
                XUIText.Make("").Bind(ref m_ModifiersText),
                XUIHLayout.Make(
                    XUIText.Make($"<color=#{ColorUtility.ToHtmlStringRGB(Color.green)}>Full Combo").Bind(ref m_FullComboText),
                    XUIText.Make("Miss").Bind(ref m_MissText),
                    XUIText.Make("BadCuts").Bind(ref m_BadCutsText)
                ),
                XUIText.Make("PauseCount").Bind(ref m_PauseCountText),
                XUIText.Make("PassState").Bind(ref m_PassStateText),
                XUIText.Make("HMD").Bind(ref m_HMDText),
                XUIText.Make("TimeText").Bind(ref m_TimeText),
                GSSecondaryButton.Make("Close", 15, 7, p_OnClick: () =>
                {
                    this.OnClose();
                })
            )
            .ForEachDirect<XUIText>(x =>
            {
                x.OnReady(y =>
                {
                    //y.SetMargins(-5, 0, -5, 0);
                    y.LElement.minWidth = 50;
                    y.LElement.preferredWidth = 50;
                    y.RTransform.anchorMin = Vector2.zero;
                    y.RTransform.anchorMax = Vector2.one;
                    y.RTransform.sizeDelta = Vector2.zero;
                });
            })
            /*.SetBackground(true)
            .SetBackgroundColor(Color.black.ColorWithAlpha(0.7f))*/
            .SetWidth(70)
            .BuildUI(RTransform);

            IsCreated = true;

            RTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 70);
            RTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 85);

            GetComponentInChildren<Image>().color = Color.black.ColorWithAlpha(0.95f);

            ModalUtils.OnModalShow(this);
        }

        protected void EnableRichText(XUIText p_Text)
        {
            var l_Text = p_Text.Element.GetComponentInChildren<TextMeshProUGUI>();
            l_Text.richText = true;
        }

        public static string GetFormattedText(string p_Object, string p_Value, Color p_ColorValue)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGBA(Color.white.ColorWithAlpha(0.7f))}>{p_Object}: <color=#{ColorUtility.ToHtmlStringRGB(p_ColorValue)}>{p_Value}";
        }

        public void SetScore(ApiMapLeaderboardContentStruct p_Score, int p_MaxScore)
        {
            m_NameText.SetText(p_Score.Name);
            m_ScoreText.SetText(GetFormattedText("Score", p_Score.ModifiedScore.ToString(), Color.white));
            m_AccuracyText.SetText(GetFormattedText("Acc", (((float)p_Score.BaseScore / p_MaxScore)*100).ToString("0.00") + "%", Color.white));
            m_ModifiersText.SetActive(p_Score.Modifiers != string.Empty);
            m_ModifiersText.SetText(GetFormattedText("Modifiers", p_Score.Modifiers, Color.white));

            if (p_Score.FullCombo)
            {
                m_FullComboText.SetActive(true);
                m_MissText.SetActive(false);
                m_BadCutsText.SetActive(false);
            } else
            {
                m_FullComboText.SetActive(false);
                m_MissText.SetActive(true);
                m_MissText.SetText(GetFormattedText("Misses", p_Score.BadCuts.ToString(), Color.red));
                m_BadCutsText.SetActive(true);
                m_BadCutsText.SetText(GetFormattedText("Bad cuts", p_Score.BadCuts.ToString(), Color.red));
            }

            if (p_Score.ScoreStatistic != null)
            {
                Color l_ResultColor = Color.red;
                if (p_Score.ScoreStatistic.Value.PauseCount == 0)
                {
                    l_ResultColor = Color.green;
                }

                m_PauseCountText.SetText(GetFormattedText("Pause count", p_Score.ScoreStatistic.Value.PauseCount.ToString(), l_ResultColor));
            }
            else
            {
                m_PauseCountText.SetText(GetFormattedText("Pause count", "?", Color.yellow));
            }

            var l_PassState = (PassState.EState)p_Score.State;
            m_PassStateText.SetText(GetFormattedText("Pass state", l_PassState.ToString(), PassState.GetColorFromPassState(l_PassState)));
            m_HMDText.SetText(GetFormattedText("Set on", ((EHMD)p_Score.HMD).ToString(), Color.white));
            m_TimeText.SetText(Formatters.TimeFormatFromUnix(int.Parse(p_Score.UnixTimeSet)));
        }

    }
}
