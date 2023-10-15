using CP_SDK.UI.Components;
using CP_SDK.XUI;
using GuildSaber.Logger;
using OVR.OpenVR;
using PlaylistManager.HarmonyPatches;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GuildSaber.UI.Defaults
{
    internal class GSSecondaryButton : CP_SDK.XUI.XUISecondaryButton
    {
        int m_Width;
        int m_Height;

        protected GSSecondaryButton(string p_Name, string p_Label, int p_Width, int p_Height, Action p_OnClick = null) : base(p_Name, p_Label, p_OnClick)
        {
            OnReady(SetupStyle);
            m_Width = p_Width;
            m_Height = p_Height;
        }

        public static GSSecondaryButton Make(string p_Label, int p_Width, int p_Height, string p_Name = "GuildSaberSecondaryButton", Action p_OnClick = null)
        {
            return new GSSecondaryButton(p_Name, p_Label, p_Width, p_Height, p_OnClick);
        }

        static Texture2D s_WhiteTexture = null;

        public virtual Color GetColor() => Color.black.ColorWithAlpha(0.7f);

        private async void SetupStyle(CSecondaryButton p_Button)
        {
            SetWidth(m_Width);
            SetHeight(m_Height);
            //Texture2D l_Tex = new Texture2D(m_Width, m_Height, (UnityEngine.Experimental.Rendering.GraphicsFormat)87, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
            Texture2D l_Tex = new Texture2D(m_Width * 7, m_Height * 7);
            //Texture2D l_Tex = await Utils.GuildSaberUtils.GetImage("https://cdn.discordapp.com/attachments/872926978825650239/1091025688238235738/test.png");

            for (int l_X = 0; l_X < l_Tex.width; l_X++)
            {
                for (int l_Y = 0; l_Y < l_Tex.height; l_Y++)
                {
                    l_Tex.SetPixel(l_X, l_Y, Color.white);
                }
            }
            Texture2D l_NewTex = await Utils.TextureUtils.CreateRoundedTexture(await Utils.TextureUtils.Gradient(l_Tex, new Color(1, 1, 1, 0.7f), new Color(1f, 1f, 1f, 1), p_UseAlpha: true), 10);
            Sprite l_Sprite = Sprite.Create(l_NewTex, new Rect(0, 0, l_Tex.width, l_Tex.height), new Vector2(0, 0), 1000, 0, SpriteMeshType.FullRect);
            p_Button.SetBackgroundColor(GetColor());
            p_Button.SetBackgroundSprite(l_Sprite);
            //XUIImage.Make(l_Sprite).BuildUI(p_Button.transform);
            //p_Button.SetBackgroundSprite(l_Sprite);
            //Element.GetComponentInChildren<>
        }
    }
}
