using CP_SDK.UI.Components;
using CP_SDK.XUI;
using GuildSaber.API;
using GuildSaber.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard.Components
{
    internal class LeaderboardHeaderPanel : XUIHLayout
    {

        public const int HEADER_WIDTH = 100;
        public const int HEADER_HEIGHT = 10;

        private Color m_GuildColor = Color.white;

        protected XUIText m_Text;
        protected PointsData m_PointsData;
        protected float m_Level;
        protected string m_LevelName;

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
            SetWidth(HEADER_WIDTH);
            SetHeight(HEADER_HEIGHT);
            XUIText.Make(string.Empty)
                .Bind(ref m_Text)
                .BuildUI(Element.LElement.transform);
        }

        public async void UpdateBackground()
        {
            Texture2D l_Background = await TextureUtils.Gradient(await TextureUtils.CreateFlatTexture(HEADER_WIDTH * 10, HEADER_HEIGHT * 10, Color.white), Color.black.ColorWithAlpha(0), m_GuildColor.ColorWithAlpha(1) * 2, p_UseAlpha: true);
            Texture2D l_RoundedBackground = await TextureUtils.CreateRoundedTexture(l_Background, 10);
            SetBackground(true);
            SetBackgroundSprite(Sprite.Create(l_RoundedBackground, new Rect(0, 0, l_RoundedBackground.width, l_RoundedBackground.height), new Vector2()));
            SetBackgroundColor(new Color(1, 1, 1, 1));
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
            string l_Text = $"Level {m_Level:0,0} - {m_LevelName} - {m_PointsData.Points:0,00} {m_PointsData.PointsName}";
            m_Text.SetText(l_Text.ToUpper());
        }
    }
}
