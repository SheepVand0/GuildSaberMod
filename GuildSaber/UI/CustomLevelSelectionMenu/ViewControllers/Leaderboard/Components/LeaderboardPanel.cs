﻿using CP_SDK.UI.Components;
using CP_SDK.XUI;
using GuildSaber.API;
using GuildSaber.Logger;
using GuildSaber.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard.Components
{
    internal class LeaderboardPanel : XUIHLayout
    {
        protected LeaderboardPanel(string p_Name, params IXUIElement[] p_Childs) : base(p_Name, p_Childs)
        {
            OnReady(OnCreation);
        }

        public static LeaderboardPanel Make()
        {
            return new LeaderboardPanel("GuildSaberLeaderboardPanel");
        }

        GSText m_NameText;
        LeaderboardPointsSelector m_PointsSelector;
        XUIImage m_LogoImage;

        protected void OnCreation(CHLayout p_Layout)
        {
            XUIHLayout.Make(
                XUIVLayout.Make(
                    XUIImage.Make()
                        .SetType(UnityEngine.UI.Image.Type.Simple)
                        .Bind(ref m_LogoImage)
                    ).OnReady(x =>
                    {
                        x.LElement.minWidth = 15;
                        x.LElement.minHeight = 15;
                    }),


                XUIHLayout.Make(
                    GSText.Make(string.Empty)
                        .Bind(ref m_NameText)
                        .SetFontSize(5)
                        .SetAlign(TMPro.TextAlignmentOptions.Bottom)
                        .OnReady(x =>
                        {
                            x.LElement.minWidth = 40;
                            x.LElement.minHeight = 8;
                            //x.LElement.enabled = false;
                            x.RTransform.anchorMin = Vector3.zero;
                            x.RTransform.anchorMax = Vector2.one;
                            x.RTransform.sizeDelta = Vector2.zero;
                            x.SetMargins(-17, 0, 0, 1);
                        }),
                        m_PointsSelector = LeaderboardPointsSelector.Make()
                )
                .SetSpacing(3f)
                //.SetBackground(true)
                .OnReady(x =>
                {
                    x.LElement.minHeight = 5;
                    x.LElement.minWidth = 70;
                    x.RTransform.anchorMin = Vector2.zero;
                    x.RTransform.anchorMax = Vector2.one;
                    x.RTransform.sizeDelta = Vector2.zero;
                })

            )
            .SetMinHeight(20)
            .SetMinWidth(100)
            .SetHeight(20)
            .SetWidth(100)
            .SetSpacing(1)
            .BuildUI(Element.LElement.transform);

            //Element.LElement.minWidth = 100;
            //Element.LElement.minHeight = 10;
        }

        public async void SetGuild(Texture2D p_Banner, Texture2D p_Logo, int p_GuildId, string p_PlayerName)
        {
            Texture2D l_Texture = p_Banner;
            
            TextureUtils.FixedHeight l_NewHeigth = TextureUtils.GetHeight(100, 20, l_Texture.width, l_Texture.height);
            l_Texture = await TextureUtils.AddOffset(l_Texture, l_NewHeigth.TextureOffset);
            Texture2D l_RoundedTexture = await TextureUtils.CreateRoundedTexture(l_Texture, l_Texture.width * 0.01f);
            SetBackground(true);
            SetBackgroundSprite(Sprite.Create(l_RoundedTexture, new Rect(0, 0, l_Texture.width, l_Texture.height), new Vector2()));
            SetBackgroundColor(new Color(1, 1, 1, 0.5f));

            m_LogoImage.SetSprite(Sprite.Create(await TextureUtils.CreateRoundedTexture(p_Logo, p_Logo.width / 2), new Rect(0, 0, p_Logo.width, p_Logo.height), new Vector2()));

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
            m_PointsSelector.SetPoints(l_Points);
            m_PointsSelector.SetSelectedPoints(GuildApi.PASS_POINTS_TYPE);
        }

        public LeaderboardPointsSelector GetPointsSelector() => m_PointsSelector;
    }
}
