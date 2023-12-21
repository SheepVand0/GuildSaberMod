using CP_SDK.UI.Components;
using CP_SDK.XUI;
using GuildSaber.API;
using GuildSaber.Utils;
using OVR.OpenVR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard.Components
{
    internal class LeaderboardCell : XUISecondaryButton
    {

        public const float ANIMATION_DURATION = 0.1f;

        public const int CELL_HEIGHT = 5;
        public const int CELL_WIDTH = 90;

        public static Color s_Blue = new Color(0.0f, 0.8f, 1f, 0.8f);

        protected GSText m_RankText;
        protected GSText m_ScoreAccText;

        protected FloatAnimation m_CellAnimation;

        protected PointsData m_SelectedPointsType;
        protected Color m_GuildColor = new Color();
        protected ApiMapLeaderboardContentStruct m_Score;

        protected int m_MaxScore;
        protected bool m_Ready = false;

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        protected LeaderboardCell(string p_Name, string p_Label, Action p_OnClick = null) : base(p_Name, p_Label, p_OnClick)
        {
            OnReady(OnCreation);
            OnClick(OnButtonClicked);
        }

        public static LeaderboardCell Make()
        {
            return new LeaderboardCell("GuildSaberLeaderboardCell", string.Empty);
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        private async void OnCreation(CSecondaryButton p_Button)
        {
            float l_FontSize = 3f;

            XUIHLayout.Make(
                XUIHLayout.Make(
                    GSText.Make("1").Bind(ref m_RankText).SetAlign(TMPro.TextAlignmentOptions.CaplineLeft),
                    GSText.Make("").Bind(ref m_ScoreAccText).SetColor(new Color(1, 1, 1, 1f)).SetAlign(TMPro.TextAlignmentOptions.CaplineLeft)
                )
                .ForEachDirect<XUIText>((x) =>
                {
                    x.SetFontSize(l_FontSize);
                    x.OnReady((y) =>
                    {
                        y.SetMargins(-20, 0, -25, 0);
                        y.LElement.minWidth = 20;
                        y.LElement.preferredWidth = 20;
                        y.RTransform.anchorMin = Vector2.zero;
                        y.RTransform.anchorMax = Vector2.one;
                        y.RTransform.sizeDelta = Vector2.zero;
                    });
                })
            )
            .SetWidth(CELL_WIDTH)
            .SetHeight(CELL_HEIGHT)
            .BuildUI(Element.LElement.transform);

            SetWidth(CELL_WIDTH);
            SetHeight(CELL_HEIGHT);
            Element.LElement.minHeight = CELL_HEIGHT;
            Element.LElement.preferredHeight = CELL_HEIGHT;

            //m_CellAnimation = Element.gameObject.AddComponent<FloatAnimation>();
            //m_CellAnimation.Init(0, 1, ANIMATION_DURATION);
            //m_CellAnimation.OnChange += OnAnimation;
            //m_CellAnimation.OnFinished += OnAnimationFinished;

            Texture2D l_Background = await TextureUtils.CreateFlatTexture(CELL_WIDTH * 40, 4 * 40, Color.black.ColorWithAlpha(0.6f));
            Texture2D l_BackgroundWithLine = await TextureUtils.AddLine(l_Background, l_Background.width, 5, 0, 0, Color.white.ColorWithAlpha(1));

            SetBackgroundSprite(Sprite.Create(l_BackgroundWithLine, new Rect(0, 0, l_BackgroundWithLine.width, l_BackgroundWithLine.height), new Vector2()));
            
            m_Ready = true;
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public async void SetScore(ApiMapLeaderboardContentStruct p_Score, int p_MaxScore)
        {
            if (GuildSaberLeaderboardViewController.Instance.IsChanging()) return;

            Element.SetBackgroundColor(new Color(1, 1, 1, 1));
            m_Score = p_Score;
            m_MaxScore = p_MaxScore;

            await WaitUtils.Wait(() => m_Ready == true, 1, p_MaxTryCount: 1000);

            var l_Points = p_Score.PointsData.Where(x => x.PointsType == m_SelectedPointsType.PointsType).First();
            string l_PointsStrPre = string.Empty;
            
            if (l_Points.Points == 0)
                l_PointsStrPre = "0";
            else
                l_PointsStrPre = l_Points.Points.ToString("0.00");

            string l_PointsStr = $"<color=#{UnityEngine.ColorUtility.ToHtmlStringRGB(m_GuildColor)}>{l_PointsStrPre} {l_Points.PointsName.ToUpper()}";

            if (l_Points.PointsType == GuildApi.PASS_POINTS_TYPE)
            {
                l_PointsStr = string.Empty;
            }

            float l_Acc = ((float)p_Score.BaseScore / p_MaxScore) * 100;

            Color l_RankNameColor = Color.white;
            if (p_Score.Name == GuildSaberModule.BasicPlayerData.Name)
                l_RankNameColor = s_Blue;

            string l_PassStateText = string.Empty;
            if (((PassState.EState)p_Score.State).HasFlag(PassState.EState.Allowed) == false)
                l_PassStateText = $"<color=#{ColorUtility.ToHtmlStringRGB(Color.red)}>D";


            m_RankText.SetText(
                $"<color=#{ColorUtility.ToHtmlStringRGBA(l_RankNameColor.ColorWithAlpha(1))}>{p_Score.Rank} <pos=15%><i>{GuildSaberUtils.GetPlayerNameToFit(p_Score.Name, 18)} <pos=60%>{l_PointsStr} <pos=75%>{l_PassStateText}");
            //m_ScoreAccText.SetText($"<align=right><pos=-50%>{p_Score.Modifiers} <pos=-60%>{l_Acc:0.00}% <pos=-85%>{p_Score.ModifiedScore}");
            string l_Modifiers = string.Empty;
            if (p_Score.Modifiers != string.Empty)
            {
                l_Modifiers = "M";
            }
            int l_BaseOffset = 55;
            m_ScoreAccText.SetText($"<pos={l_BaseOffset}%><mspace=0.5em><align=right><color=#FFFFFF>{p_Score.BaseScore}<mspace=0em><align=left> <pos={l_BaseOffset + 25}%><color=#{ColorUtility.ToHtmlStringRGB(m_GuildColor * 1.1f)}>{l_Acc:0.00}% <pos={l_BaseOffset + 40}%><color=#{ColorUtility.ToHtmlStringRGBA(new Color(1, 1, 1, 0.6f))}>{l_Modifiers}");
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public void SetGuildColor(Color p_Color)
        {
            m_GuildColor = p_Color;
        }

        public void SetPoints(PointsData p_PointsType)
        {
            m_SelectedPointsType = p_PointsType;
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public void Hide()
        {
            Element.SetBackgroundColor(new Color(0, 0, 0, 0));
            m_RankText.SetText(string.Empty);
            m_ScoreAccText.SetText(string.Empty);
            //m_CellAnimation.Stop();
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        private void OnButtonClicked()
        {
            GuildSaberLeaderboardViewController.Instance.ShowScoreDetailsModal();
            GuildSaberLeaderboardViewController.Instance.SetScoreDetailsModalData(m_Score, m_MaxScore);
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public void PlayAnimation(int p_IndexInList, ApiMapLeaderboardContentStruct p_Score, int p_MaxScore)
        {
            OnReady(async x =>
            {
                await Task.Delay((int)((ANIMATION_DURATION * 0.3f) * 1000) * p_IndexInList);

                SetScore(p_Score, p_MaxScore);

                FastAnimator.Animate(new List<FastAnimator.FloatAnimKey>() { 
                    new FastAnimator.FloatAnimKey(0, 0),
                    new FastAnimator.FloatAnimKey(1, ANIMATION_DURATION)
                }, OnAnimation, OnAnimationFinished);
            });
        }

        private void OnAnimation(float p_Value)
        {
            Element.gameObject.transform.localScale = new Vector3(1, p_Value, 1);
        }

        private void OnAnimationFinished(/*float p_Val*/)
        {
            Element.gameObject.transform.localScale = Vector3.one;
        }

    }
}
