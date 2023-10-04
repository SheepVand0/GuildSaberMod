using CP_SDK.UI.Components;
using CP_SDK.XUI;
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

        protected void OnCreation(CVLayout p_Layout)
        {
            SetWidth(100);
            SetHeight(20);
            XUIHLayout.Make(
                XUIVLayout.Make(
                        XUIText.Make(string.Empty)
                        .Bind(ref m_NameText)
                    )
                .SetWidth(100)
                .SetHeight(20)
            )
            
            .BuildUI(Element.LElement.transform);
        }

        public void SetGuild(Texture2D p_Texture, string p_PlayerName)
        {
            Texture2D l_Texture = p_Texture;//TextureUtils.MakeCorrespondHeight(p_Texture, new Rect(0, 0, p_Texture.width, p_Texture.height * 0.5f));
            

            TextureUtils.FixedHeigth l_NewHeigth = TextureUtils.GetHeigth((int)(l_Texture.width * (float)(20 / 100)), l_Texture.width, l_Texture.height);
            l_Texture = TextureUtils.AddOffset(TextureUtils.CreateRoundedTexture(l_Texture, l_Texture.width * 0.01f), l_NewHeigth.RoundedTextureOffset);
            SetBackground(true);
            SetBackgroundSprite(Sprite.Create(l_Texture, new Rect(0, 0, l_Texture.width, l_Texture.height), new Vector2()));
            SetBackgroundColor(new Color(1, 1, 1, 0.7f));

            m_NameText.SetText(p_PlayerName);
        }
    }
}
