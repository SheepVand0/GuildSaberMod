using CP_SDK.UI.Components;
using CP_SDK.XUI;
using GuildSaber.API;
using GuildSaber.UI.CustomLevelSelectionMenu.FlowCoordinators;
using GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers;
using GuildSaber.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GuildSaber.UI.CustomLevelSelectionMenu.Components
{
    internal class PlaylistButton : XUISecondaryButton
    {

        protected PlaylistButton(string p_Name, Action p_OnClick = null) : base(p_Name, string.Empty, p_OnClick)
        {
            OnReady(OnButtonReady);
            OnClick(OnClicked);
        }

        public static PlaylistButton Make()
        {
            return new PlaylistButton("PlaylistButton");
        }

        protected List<PlaylistModelSong> m_Level = new List<PlaylistModelSong>();

        public PlaylistButton SetLevel(Texture2D p_Cover, List<PlaylistModelSong> p_Hashes)
        {
            m_Level = p_Hashes;
            OnReady(x =>
            {
                Texture2D l_Cover = TextureUtils.CreateRoundedTexture(p_Cover, p_Cover.width * 0.01f);
                Sprite l_Sprite = Sprite.Create(l_Cover, new Rect(0, 0, l_Cover.width, l_Cover.height), new Vector2());
                SetBackgroundSprite(l_Sprite);
                SetWidth(10);
                SetHeight(10);
            });
            return this;
        }

        private void OnButtonReady(CSecondaryButton p_Button)
        {

        }

        private void OnClicked()
        {

        }
    }
}
