using BeatLeader.Models;
using BeatSaberMarkupLanguage.Attributes;
using GuildSaber.API;
using GuildSaber.Logger;
using GuildSaber.Utils;
using HMUI;
using OVR.OpenVR;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;


#nullable enable
namespace GuildSaber.UI.Leaderboard.Components.SubComponents
{
    internal class LeaderboardScoreCell
    {
        public const float ANIMATION_DURATION = 0.3f;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private static readonly Color s_Blue = new Color(0.0f, 0.8f, 1f, 0.8f);

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private CustomText m_CAcc;
        private CustomText m_CModifiers;
        private CustomText m_CPlayerName;
        private CustomText m_CPoints;
        private CustomText m_CRank;
        private CustomText m_CScore;
        [UIComponent("Elems")] public HorizontalLayoutGroup m_ElemsLayout = null;
        [UIComponent("Interactable")] public Button m_Interactable = null;

        ImageView m_ButtonLineImageView;

        float m_OriginalButtonLineYPosition = 0f;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
        public static float ScaleFactor => 1.2f;

        private float InteractableScaleY => (float)(6.5 * ScaleFactor * 0.899999976158142f);

        public static float ScoreFontSize => 2.5f * ScaleFactor;

        private float LeaderWidth => (float)(90.0 * ScaleFactor + 9.80000019073486);

        public bool HasBeenInit { get; private set; } = false;

        private float BasePoints { get; set; }

        public string Rank { get; set; } = string.Empty;

        public string PlayerName { get; set; } = string.Empty;

        public string Points { get; set; } = string.Empty;

        public string Score { get; set; } = string.Empty;

        public string Acc { get; set; } = string.Empty;

        public string Id { get; set; } = string.Empty;

        public string Modifiers { get; set; } = string.Empty;

        public PassState.EState State { get; set; }

        public int IndexInLeaderboard { get; set; }

        public Player BeatLeaderPlayer { get; set; }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private FloatAnimation m_ShowAnimation;

        private static float OriginalLayoutSize = 1f;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        internal void Init(
          ApiMapLeaderboardContentStruct p_Score,
          ApiCustomDataStruct p_LeaderMetadata,
          int p_LeaderboardIndex)
        {
            Rank = $"{p_Score.Rank}";
            PlayerName = p_Score.Name;
            PointsData l_PointsData = new PointsData();
            try
            {
                l_PointsData = p_Score.PointsData.Any<PointsData>() ? p_Score.PointsData.First((p_Index => p_Index.PointsName == GuildSaberLeaderboardPanel.PanelInstance.m_PointsType.m_SelectedPoints)) : new PointsData();
            }
            catch
            {
            }
            BasePoints = l_PointsData.Points;
            State = (PassState.EState)p_Score.State;
            Points = l_PointsData.PointsType != "pass" ? string.Format("{0:0.00} {1}", (object)BasePoints, (object)l_PointsData.PointsName) : string.Empty;
            Score = p_Score.BaseScore.ToString();
            Acc = string.Format("{0:00.00}%", (object)(p_Score.BaseScore * 100U / p_LeaderMetadata.MaxScore));
            Id = p_Score.ID.ToString();
            Modifiers = GuildSaberUtils.GetPlayerNameToFit(p_Score.Modifiers, 4);

            Show();

            IndexInLeaderboard = p_LeaderboardIndex;
            Player player = new Player()
            {
                id = p_Score.BeatLeaderID,
                avatar = p_Score.Avatar,
                rank = (int)p_Score.Rank,
                name = p_Score.Name,
                countryRank = (int)p_Score.Rank
            };
            player.profileSettings = new ProfileSettings()
            {
                hue = 1,
                message = string.Empty,
                saturation = 1f,
                effectName = string.Empty
            };
            try
            {
                if (p_Score.PointsData.Any<PointsData>())
                {
                    foreach (PointsData pointsData2 in p_Score.PointsData.Where<PointsData>((Func<PointsData, bool>)(p_Index => p_Index.PointsName == GuildSaberLeaderboardPanel.PanelInstance.m_PointsType.m_SelectedPoints)))
                        player.pp = pointsData2.Points;
                }
            }
            catch
            {
            }
            BeatLeaderPlayer = player;
        }

        internal void SetupIfNotInitialized()
        {
            if (HasBeenInit == true) return;

            float l_Offset1 = 7f;
            float l_Offset2 = -1f;
            Color l_GuildColor = GuildSaberModule.LeaderboardSelectedGuild.Color.ToUnityColor();

            m_CRank = CustomUIComponent.CreateItem<CustomText>(m_ElemsLayout.transform, true, true, p_PreCallback: (p_Item) =>
            {
                p_Item.FontSize = ScoreFontSize;
                p_Item.AnchorPosX = 2f;
                p_Item.Alignment = (TextAlignmentOptions)513;
                p_Item.LayoutAlignment = (TextAnchor)3;
                p_Item.Color = Color.white;
                p_Item.Italic = true;
                p_Item.RichText = String.Empty;
                p_Item.EnableRichText = true;

            });

            m_CPlayerName = CustomUIComponent.CreateItem<CustomText>(m_ElemsLayout.transform, true, true, p_PreCallback: (p_Item) =>
            {
                p_Item.FontSize = m_CRank.FontSize;
                p_Item.AnchorPosX = (float)(3.0 + l_Offset1 + ScaleFactor * 0.699999988079071f);
                p_Item.Alignment = m_CRank.Alignment;
                p_Item.LayoutAlignment = m_CRank.LayoutAlignment;
                p_Item.Color = Color.white;
                p_Item.Italic = true;
                p_Item.EnableRichText = false;
            });

            m_CPoints = CustomUIComponent.CreateItem<CustomText>(m_ElemsLayout.transform, true, true, p_PreCallback: (p_Item) =>
            {
                p_Item.FontSize = m_CRank.FontSize;
                p_Item.AnchorPosX = (float)(34.0 + l_Offset1 + l_Offset2 + ScaleFactor * 1.01999998092651f);
                p_Item.Alignment = m_CRank.Alignment;
                p_Item.LayoutAlignment = m_CRank.LayoutAlignment;
                p_Item.Color = l_GuildColor;
                p_Item.Italic = true;
                p_Item.EnableRichText = true;
            });

            m_CScore = CustomUIComponent.CreateItem<CustomText>(m_ElemsLayout.transform, true, true, p_PreCallback: (p_Item) =>
            {
                p_Item.FontSize = m_CRank.FontSize;
                p_Item.AnchorPosX = (float)(l_Offset1 - 40.0 + l_Offset2 + (ScaleFactor * 1.03999996185303f));
                p_Item.Alignment = (TextAlignmentOptions)516;
                p_Item.LayoutAlignment = m_CRank.LayoutAlignment;
                p_Item.Color = Color.white;
                p_Item.Italic = false;
                p_Item.RichText = "<mspace=0.5em>";
                p_Item.EnableRichText = true;
            });

            m_CModifiers = CustomUIComponent.CreateItem<CustomText>(m_ElemsLayout.transform, true, true, p_PreCallback: (p_Item) =>
            {
                p_Item.FontSize = m_CRank.FontSize * 0.7f;
                p_Item.AnchorPosX = (float)(l_Offset1 - 32.5 + l_Offset2 + ScaleFactor * 1.05999994277954f);
                p_Item.Alignment = (TextAlignmentOptions)516;
                p_Item.LayoutAlignment = m_CRank.LayoutAlignment;
                p_Item.Color = Color.gray;
                p_Item.Italic = false;
                p_Item.RichText = string.Empty;
                p_Item.EnableRichText = false;
            });

            m_CAcc = CustomUIComponent.CreateItem<CustomText>(m_ElemsLayout.transform, true, true, p_PreCallback: (p_Item) =>
            {
                p_Item.FontSize = ScoreFontSize;
                p_Item.AnchorPosX = (float)(l_Offset1 - 24.0 + l_Offset2 + ScaleFactor * 1.05999994277954f);
                p_Item.Alignment = (TextAlignmentOptions)516;
                p_Item.LayoutAlignment = m_CRank.LayoutAlignment;
                p_Item.Color = l_GuildColor;
                p_Item.Italic = false;
                p_Item.RichText = string.Empty;
                p_Item.EnableRichText = false;
            });

            HasBeenInit = true;
            Events.Instance.e_OnGuildSelected += OnGuildSelected;
        }

        private void ApplyScaleAndPositionOnButtonLine(float p_Ignored = 0)
        {
            return;
            if (m_ButtonLineImageView == null)
            {
                var l_ButtonLineImageView = m_Interactable.GetComponentsInChildren<ImageView>()[1];
                l_ButtonLineImageView.transform.localScale = new UnityEngine.Vector3(1, 0.7f);
                m_ButtonLineImageView = l_ButtonLineImageView;
                m_OriginalButtonLineYPosition = m_ButtonLineImageView.transform.localPosition.y;
            }
            var l_ButttonLineTransform = m_ButtonLineImageView.transform.localPosition;
            m_ButtonLineImageView.transform.localPosition = new UnityEngine.Vector3(l_ButttonLineTransform.x, m_OriginalButtonLineYPosition - 1.5f, l_ButttonLineTransform.z);
        }

        internal void Show()
        {
            //await WaitUtils.Wait(() => LeaderboardScoreList.Instance.Initialized, 10);
            StopAnimation();
            SetupIfNotInitialized();
            SetTexts();
            m_Interactable.gameObject.SetActive(true);

            if (GuildSaberModule.GSPlayerId.HasValue && Id == GuildSaberModule.GSPlayerId.ToString())
                SetCellToCurrentPlayer();
            else
                SetCellToRandomPlayer();

            if (m_ShowAnimation == null)
            {
                m_ShowAnimation = m_Interactable.gameObject.AddComponent<FloatAnimation>();
                m_ShowAnimation.Init(float.Epsilon, 1, ANIMATION_DURATION);
                m_ShowAnimation.OnChange += OnChange;
                m_ShowAnimation.OnFinished += ApplyScaleAndPositionOnButtonLine;
            }

            m_ShowAnimation.Play();

        }

        internal void StopAnimation()
        {
            if (m_ShowAnimation == null) return;

            m_ShowAnimation.Stop();
        }

        private void OnChange(float p_Value)
        {
            var l_Scale = m_ElemsLayout.transform.localScale;
            m_ElemsLayout.transform.localScale = new UnityEngine.Vector3(l_Scale.x, p_Value * OriginalLayoutSize, l_Scale.z);
            var l_IntScale = m_Interactable.transform.localScale;
            m_Interactable.transform.localScale = new UnityEngine.Vector3(l_IntScale.x, p_Value, l_IntScale.z);
        }

        internal void SetTexts()
        {
            m_CRank.SetText(Rank);
            m_CPlayerName.SetText(GuildSaberUtils.GetPlayerNameToFit(PlayerName, 22));
            if (Points == string.Empty)
            {
                if (State != PassState.EState.Allowed && State != PassState.EState.NeedConfirmation)
                {
                    Points = "<color=#" + ColorUtility.ToHtmlStringRGB(Color.red) + ">D";
                    m_CPoints.SetText(Points);
                }
                else if (State == PassState.EState.NeedConfirmation)
                {
                    Points = "<color=#" + ColorUtility.ToHtmlStringRGB(Color.yellow) + ">N";
                    m_CPoints.SetText(Points);
                }
            }
            m_CPoints.SetText(Points);
            m_CScore.SetText(m_CScore.RichText + Score);
            m_CAcc.SetText(Acc);
            m_CModifiers.SetText(Modifiers);
        }

        internal void Reset()
        {
            //await WaitUtils.Wait(() => LeaderboardScoreList.Instance.Initialized, 10);
            //await WaitUtils.Wait(() => HasBeenInit, 10);
            m_Interactable.gameObject.SetActive(false);
            StopAnimation();
            if (!HasBeenInit) return;
            SetEmpty();
            Color l_White = Color.white;
            m_CRank.SetColor(l_White);
            m_CPlayerName.SetColor(l_White);
            m_CScore.SetColor(l_White);
        }

        internal void SetCellToCurrentPlayer()
        {
            m_CRank.SetColor(s_Blue);
            m_CPlayerName.SetColor(s_Blue);
            m_CScore.SetColor(s_Blue);
        }

        internal void SetCellToRandomPlayer()
        {
            if (!HasBeenInit) return;
            m_CRank.SetColor(Color.white);
            m_CPlayerName.SetColor(Color.white);
            m_CScore.SetColor(Color.white);
        }

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

        private void OnGuildSelected(int p_Guild)
        {
            Color l_GuildColor = GuildSaberModule.LeaderboardSelectedGuild.Color.ToUnityColor();
            m_CAcc.SetColor(l_GuildColor);
            m_CPoints.SetColor(l_GuildColor);
        }

        [UIAction("ShowInfo")]
        private void ShowInfo()
        {
            ScoreCellInfoModal infoModal = LeaderboardScoreList.s_InfoModal;
            ApiMapLeaderboardContentStruct leaderboard = LeaderboardScoreList.s_Leaderboard.Leaderboards[IndexInLeaderboard];
            ApiCustomDataStruct customData = LeaderboardScoreList.s_Leaderboard.CustomData;

            List<string> p_BannedModifiers = new()
            {
                "NF", "SS", "NA", "NB", "ZM"
            };

            Player beatLeaderPlayer = BeatLeaderPlayer;
            infoModal.SetModalInfo(leaderboard, customData, p_BannedModifiers, beatLeaderPlayer);
            LeaderboardScoreList.s_InfoModal.Show();
        }
    }
}
