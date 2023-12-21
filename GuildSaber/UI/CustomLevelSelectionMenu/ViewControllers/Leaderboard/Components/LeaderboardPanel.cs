using CP_SDK.UI.Components;
using CP_SDK.XUI;
using GuildSaber.API;
using GuildSaber.Logger;
using GuildSaber.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard.Components
{
    internal class LeaderboardPanel : XUIHLayout
    {
        public const int IMAGES_SIZE = 20;

        protected LeaderboardPanel(string p_Name, params IXUIElement[] p_Childs) : base(p_Name, p_Childs)
        {
            OnReady(OnCreation);
        }

        public static LeaderboardPanel Make()
        {
            return new LeaderboardPanel("GuildSaberLeaderboardPanel");
        }

        protected GSText m_NameText;
        internal LeaderboardPointsSelector PointsSelector;
        protected XUIImage m_LogoImage;
        protected XUIImage m_PlayerImage;
        protected XUIText m_PointsText;

        protected void OnCreation(CHLayout p_Layout)
        {

            // GuildSaber logo
            XUIVLayout.Make(
                XUIImage.Make()
                    .SetType(UnityEngine.UI.Image.Type.Simple)
                    .Bind(ref m_LogoImage)
            ).OnReady(x =>
            {
                //x.CSizeFitter.horizontalFit = x.CSizeFitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.MinSize;
                x.LElement.preferredHeight = IMAGES_SIZE;
                x.LElement.preferredWidth = IMAGES_SIZE;
                x.LElement.minWidth = IMAGES_SIZE;
                x.LElement.minHeight = IMAGES_SIZE;
                //x.RTransform.anchorMin = new Vector2(-5, 0);
                //x.RTransform.anchorMax = new Vector2(-5, 0);
            }).BuildUI(Element.LElement.transform);

            // Name, points
            XUIHLayout.Make(
                XUIHLayout.Make(
                GSText.Make(string.Empty)
                    .Bind(ref m_NameText)
                    .SetFontSize(6)
                    .SetMargins(-15, 0, 0, 0)
                    .OnReady(x => x.LElement.preferredWidth = x.LElement.minWidth = 17)
                    .OnReady(x => x.RTransform.anchorMin = x.RTransform.sizeDelta = Vector2.zero)
                    .OnReady(x => x.RTransform.anchorMax = Vector2.one),
                GSText.Make(string.Empty)
                    .Bind(ref m_PointsText)
                    .SetFontSize(5)
                    .SetStyle(FontStyles.Italic)
                    .OnReady(x => x.LElement.preferredWidth = x.LElement.minWidth = 30)
                ).SetWidth(80)
                .SetMinWidth(80)
             )
            .SetBackground(false)
            .SetMinHeight(10)
            .SetHeight(10)
            .SetWidth(60)
            .SetMinWidth(60)
            .BuildUI(Element.LElement.transform);

            //Player Image
            XUIVLayout.Make(
                XUIImage.Make()
                    .SetType(UnityEngine.UI.Image.Type.Simple)
                    .Bind(ref m_PlayerImage)
            )
            .OnReady(x =>
            {
                //x.CSizeFitter.horizontalFit = x.CSizeFitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.MinSize;
                x.LElement.minWidth = IMAGES_SIZE;
                x.LElement.minHeight = IMAGES_SIZE;
                x.LElement.preferredHeight = IMAGES_SIZE;
                x.LElement.preferredWidth = IMAGES_SIZE;
            })
            .BuildUI(Element.LElement.transform);


            //.SetMinHeight(15)
            //.SetMinWidth(90)
            //.SetHeight(15)
            //.SetWidth(90)
            SetSpacing(-1f);

            //Element.LElement.minWidth = 100;
            //Element.LElement.minHeight = 10;
        }

        public async void SetGuild(Texture2D p_PlayerImage, Texture2D p_Logo, int p_GuildId, string p_PlayerName)
        {
            Texture2D l_Texture = p_Logo;

            TextureUtils.FixedHeight l_BackgroundImageNewHeigth = TextureUtils.GetHeight(90, 20, l_Texture.width, l_Texture.height);
            l_Texture = await TextureUtils.AddOffset(l_Texture, l_BackgroundImageNewHeigth.TextureOffset);
            Texture2D l_RoundedTexture = await TextureUtils.CreateRoundedTexture(l_Texture, l_Texture.width * 0.01f);
            SetBackground(true);
            SetBackgroundSprite(Sprite.Create(l_RoundedTexture, new Rect(0, 0, l_Texture.width, l_Texture.height), new Vector2()));
            SetBackgroundColor(new Color(1, 1, 1, 0.2f));

            VertexGradient l_Gradient = GuildSaberLeaderboardViewController.Instance.GetGuildColor().GenerateGradient(0.1f, 1.4f);
            m_LogoImage.SetSprite(
                Sprite.Create(
                    await TextureUtils.CreateRoundedTexture(
                        await TextureUtils.Gradient(
                            CustomLevelSelectionMenuReferences.DefaultWhiteLogo, l_Gradient.bottomLeft, l_Gradient.bottomRight, p_UseAlpha: false),
                        CustomLevelSelectionMenuReferences.DefaultWhiteLogo.width * 0.05f),
                    new Rect(0, 0, CustomLevelSelectionMenuReferences.DefaultWhiteLogo.width,
                    CustomLevelSelectionMenuReferences.DefaultWhiteLogo.height),
                    new Vector2()));

            m_PlayerImage.SetSprite(
                Sprite.Create(
                    await TextureUtils.CreateRoundedTexture(p_PlayerImage, p_PlayerImage.width * 0.5f), new Rect(0, 0, p_PlayerImage.width, p_PlayerImage.height), Vector2.zero));

            m_NameText.SetText(GuildSaberUtils.GetPlayerNameToFit(p_PlayerName, 16));

            List<RankData> l_RankData = GuildSaberLeaderboardViewController.GuildPlayerData.RankData;
            List<PointsData> l_Points = new List<PointsData>();
            foreach (var l_Rank in l_RankData)
            {
                PointsData l_IndexPoints = new PointsData();
                l_IndexPoints.Points = l_Rank.Points;
                l_IndexPoints.PointsIndex = l_Rank.PointsIndex;
                l_IndexPoints.PointsName = l_Rank.PointsName;
                l_IndexPoints.PointsType = l_Rank.PointsType;
                l_Points.Add(l_IndexPoints);
            }
            PointsSelector.SetPoints(l_Points);
            PointsSelector.SetSelectedPoints(GuildApi.PASS_POINTS_TYPE);
            UpdatePointsText(PointsSelector.GetSelectedPoints());
        }

        public void UpdatePointsText(PointsData p_Points)
        {
            m_PointsText.SetText($"{p_Points.Points} {p_Points.PointsName}");
            m_PointsText.Element.TMProUGUI.enableVertexGradient = true;
            m_PointsText.Element.TMProUGUI.colorGradient = Utils.GuildSaberUtils.GenerateGradient(GuildSaberLeaderboardViewController.Instance.GetGuildColor(), 0.2f, 1.1f);
        }

        public LeaderboardPointsSelector GetPointsSelector() => PointsSelector;

        public LeaderboardPointsSelector SetPointsSelector(LeaderboardPointsSelector p_Selector)
        {
            if (PointsSelector != null) return PointsSelector;
            PointsSelector = p_Selector;
            PointsSelector.eOnChange += UpdatePointsText;

            return PointsSelector;
        }
    }
}
