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
    internal class LeaderboardPanel : XUIVLayout
    {
        protected LeaderboardPanel(string p_Name, params IXUIElement[] p_Childs) : base(p_Name, p_Childs)
        {
            OnReady(OnCreation);
        }

        public static LeaderboardPanel Make()
        {
            return new LeaderboardPanel("GuildSaberLeaderboardPanel");
        }

        XUIText m_NameText;
        LeaderboardPointsSelector m_PointsSelector;
        XUIImage m_LogoImage;

        protected void OnCreation(CVLayout p_Layout)
        {
            SetWidth(100);
            SetHeight(20);
            XUIHLayout.Make(
                XUIHLayout.Make(
                        XUIVLayout.Make(
                            XUIImage.Make(
                            
                            )
                            .SetType(UnityEngine.UI.Image.Type.Simple)
                            .Bind(ref m_LogoImage)
                         ).SetWidth(18)
                          .SetHeight(18),
                        XUIVLayout.Make(
                            XUIText.Make(string.Empty)
                            .Bind(ref m_NameText),
                            m_PointsSelector = LeaderboardPointsSelector.Make()
                        ).OnReady(x => x.CSizeFitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.MinSize)
                        /*.ForEachDirect<XUIText>(x =>
                        {
                            x.OnReady((y) =>
                            {
                                //y.SetMargins(-15, 0, -15, 0);
                                y.LElement.minWidth = 20;
                                y.LElement.preferredWidth = 20;
                                y.RTransform.anchorMin = Vector2.zero;
                                y.RTransform.anchorMax = Vector2.one;
                                y.RTransform.sizeDelta = Vector2.zero;
                            });
                        })*/
                    )
            ).SetWidth(100)
             .SetHeight(20)
            
            .BuildUI(Element.LElement.transform);
        }

        public async void SetGuild(Texture2D p_Banner, Texture2D p_Logo, int p_GuildId, string p_PlayerName)
        {
            Texture2D l_Texture = p_Banner;//TextureUtils.MakeCorrespondHeight(p_Texture, new Rect(0, 0, p_Texture.width, p_Texture.height * 0.5f));
            

            TextureUtils.FixedHeight l_NewHeigth = TextureUtils.GetHeight(100, 20, l_Texture.width, l_Texture.height);
            l_Texture = await TextureUtils.AddOffset(await TextureUtils.CreateRoundedTexture(l_Texture, l_Texture.width * 0.01f), l_NewHeigth.TextureOffset);
            SetBackground(true);
            SetBackgroundSprite(Sprite.Create(l_Texture, new Rect(0, 0, l_Texture.width, l_Texture.height), new Vector2()));
            SetBackgroundColor(new Color(1, 1, 1, 0.7f));

            m_LogoImage.SetSprite(Sprite.Create(await TextureUtils.CreateRoundedTexture(p_Logo, p_Logo.width * 0.01f), new Rect(0, 0, p_Logo.width, p_Logo.height), new Vector2()));

            m_NameText.SetText(p_PlayerName);

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
