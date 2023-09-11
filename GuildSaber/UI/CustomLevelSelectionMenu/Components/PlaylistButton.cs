using CP_SDK.UI.Components;
using CP_SDK.XUI;
using GuildSaber.API;
using GuildSaber.UI.CustomLevelSelectionMenu.FlowCoordinators;
using GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers;
using GuildSaber.Utils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SColor = SixLabors.ImageSharp.Color;

namespace GuildSaber.UI.CustomLevelSelectionMenu.Components
{
    internal class PlaylistButton : XUISecondaryButton
    {

        protected PlaylistButton(string p_Name, Action p_OnClick = null) : base(p_Name, string.Empty, p_OnClick)
        {
            OnClick(OnClicked);
        }

        public static PlaylistButton Make()
        {
            return new PlaylistButton("PlaylistButton");
        }

        protected float m_LevelNumber;
        protected List<PlaylistModelSong> m_Level = new List<PlaylistModelSong>();

        public PlaylistButton SetLevel(string p_Base64, List<PlaylistModelSong> p_Hashes, float p_LevelNumber)
        {
            m_Level = p_Hashes;
            m_LevelNumber = p_LevelNumber;
            OnReady(async x =>
            {
                byte[] l_ImageBytes = new byte[p_Base64.Length];
                l_ImageBytes = Convert.FromBase64String(p_Base64);

                Image<Rgba32> l_Image = null;
                Texture2D l_LevelCover = null;

                try
                {
                    await Task.Run(() =>
                    {
                        l_Image = Image.Load<Rgba32>(l_ImageBytes);
                    });

                    l_LevelCover = new Texture2D(l_Image.Width, l_Image.Height);

                    await Task.Run(() =>
                    {
                        for (int l_X = 0; l_X < l_Image.Width; l_X++)
                        {
                            for (int l_Y = 0; l_Y < l_Image.Height; l_Y++)
                            {
                                int l_FixedX = l_Image.Width - 1 - (l_X);
                                int l_FixedY = l_Image.Height - 1 - (l_Y);
                                SColor l_Color = l_Image[l_X, l_FixedY];
                                Rgba32 l_Pixel = l_Color.ToPixel<Rgba32>();
                                l_LevelCover.SetPixel(l_X, l_Y, new UnityEngine.Color((float)l_Pixel.R / 255, (float)l_Pixel.G / 255, (float)l_Pixel.B / 255, (float)l_Pixel.A / 255));
                            }
                        }
                    });
                }
                catch (Exception l_E)
                {
                    l_LevelCover = AssemblyUtils.LoadTextureFromAssembly("GuildSaber.Resources.GsWhiteLogo.png");
                }

                Texture2D l_Cover = TextureUtils.CreateRoundedTexture(l_LevelCover, l_LevelCover.width * 0.01f);
                Sprite l_Sprite = Sprite.Create(l_Cover, new Rect(0, 0, l_Cover.width, l_Cover.height), new Vector2());
                SetBackgroundSprite(l_Sprite);
                SetWidth(10);
                SetHeight(10);
            });
            return this;
        }

        private void OnClicked()
        {
            LevelSelectionViewController.Instance.SetSelectedPlaylist(m_LevelNumber);
        }
    }
}
