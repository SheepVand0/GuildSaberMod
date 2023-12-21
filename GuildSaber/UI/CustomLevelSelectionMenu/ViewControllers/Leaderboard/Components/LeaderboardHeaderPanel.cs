using CP_SDK.UI.Components;
using CP_SDK.XUI;
using GuildSaber.API;
using GuildSaber.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard.Components
{
    internal class LeaderboardHeaderPanel : XUIHLayout
    {

        public const int HEADER_WIDTH = 90;
        public const int HEADER_HEIGHT = 7;

        private Color m_GuildColor = Color.white;

        protected GSText m_Text;
        protected PointsData m_PointsData;
        protected float m_Level;
        protected string m_LevelName;

        LeaderboardPointsSelector m_PointsSelector;

        public LeaderboardPointsSelector GetPointsSelector() => m_PointsSelector;

        protected LeaderboardHeaderPanel(string p_Name, params IXUIElement[] p_Childs) : base(p_Name, p_Childs)
        {
            OnReady(OnElementCreation);
        }

        public static LeaderboardHeaderPanel Make()
        {
            return new LeaderboardHeaderPanel("GuildSaberLeaderboardHeaderPanel");
        }

        protected void OnElementCreation(CHOrVLayout p_layout)
        {
            XUIHSpacer.Make(40).BuildUI(Element.LElement.transform);

            GSText.Make(string.Empty)
                .SetMargins(-5, 0, 0, 0)
                .Bind(ref m_Text)
                .OnReady(x =>
                {
                    x.LElement.minHeight = HEADER_HEIGHT;
                    x.LElement.minWidth = 12;
                })
                .BuildUI(Element.LElement.transform);

            (m_PointsSelector = LeaderboardPointsSelector.Make())
                .BuildUI(Element.LElement.transform);

            SetWidth(HEADER_WIDTH);
            SetMinWidth(HEADER_WIDTH);
            SetHeight(HEADER_HEIGHT);
            SetMinHeight(HEADER_HEIGHT);
            SetSpacing(0);

            Element.LElement.minHeight = HEADER_HEIGHT;
        }

        public async void UpdateBackground()
        {
            VertexGradient l_HeaderGradient = m_GuildColor.GenerateGradient(0.1f);
            Texture2D l_Background = await TextureUtils.Gradient(await TextureUtils.CreateFlatTexture(HEADER_WIDTH * 10, HEADER_HEIGHT * 10, Color.white), l_HeaderGradient.bottomLeft, l_HeaderGradient.bottomRight, p_UseAlpha: false);
            Texture2D l_RoundedBackground = await TextureUtils.CreateRoundedTexture(l_Background, 10);
            SetBackground(true);
            SetBackgroundSprite(Sprite.Create(l_RoundedBackground, new Rect(0, 0, l_RoundedBackground.width, l_RoundedBackground.height), new Vector2()));
            SetBackgroundColor(new Color(1, 1, 1, 0.3f));
        }

        public void SetColor(Color p_Color)
        {
            m_GuildColor = p_Color;
            UpdateBackground();
        }

        public void SetLevel(float p_Level, string p_Name)
        {
            m_Level = p_Level;
            m_LevelName = p_Name;
            UpdateText();
        }

        public void SetPoints(PointsData p_Points)
        {
            m_PointsData = p_Points;
            UpdateText();
        }

        public void UpdateText()
        {
            m_Text.SetFontSize(7);
            string l_Text = $"{m_PointsData.Points:0,00} {m_PointsData.PointsName}";
            m_Text.SetText("<voffset=1em>" + l_Text.ToUpper() + "</voffset>");
        }
    }
}
