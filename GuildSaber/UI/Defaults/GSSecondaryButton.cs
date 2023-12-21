using CP_SDK.UI.Components;
using CP_SDK.XUI;
using GuildSaber.Logger;
using GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard.Components;
using OVR.OpenVR;
using PlaylistManager.HarmonyPatches;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GuildSaber.UI.Defaults
{
    internal class GSSecondaryButton : CP_SDK.XUI.XUISecondaryButton
    {
        int m_Width;
        int m_Height;
            
        protected GSSecondaryButton(string p_Label, Action p_OnClick = null) : base("GuildSaberSecondaryButton", p_Label, p_OnClick)
        {
            OnReady(_SetupStyle);
        }

        protected GSSecondaryButton(string p_Name, string p_Label, int p_Width, int p_Height, Action p_OnClick = null) : base(p_Name, p_Label, p_OnClick)
        {
            OnReady(_SetupStyle);
            m_Width = p_Width;
            m_Height = p_Height;
        }

        public static new GSSecondaryButton Make(string p_Label, Action p_OnClick = null)
        {
            return new GSSecondaryButton(p_Label, p_OnClick);
        }

        public static GSSecondaryButton Make(string p_Label, int p_Width, int p_Height, string p_Name = "GuildSaberSecondaryButton", Action p_OnClick = null)
        {
            return new GSSecondaryButton(p_Name, p_Label, p_Width, p_Height, p_OnClick);
        }

        static Texture2D s_WhiteTexture = null;

        public virtual Color GetColor() => Color.black.ColorWithAlpha(0.7f);

        private void _SetupStyle(CSecondaryButton p_Button)
        {
            SetupStyle(p_Button, m_Width, m_Height, GetColor());
        }

        public static async void SetupStyle(CSecondaryButton p_Button, int p_Width, int p_Height, Color p_Color)
        {
            p_Button.SetWidth(p_Width);
            p_Button.SetHeight(p_Height);

            var l_Sprite = await GetBackground(p_Width, p_Height);
            p_Button.SetBackgroundColor(p_Color);
            p_Button.SetBackgroundSprite(l_Sprite);
            GSText.PatchText(p_Button.gameObject.GetComponentInChildren<TextMeshProUGUI>());
        }

        public static async Task<Sprite> GetBackground(int p_Width, int p_Height)
        {
            Texture2D l_Tex = new Texture2D(p_Width * 7, p_Height * 7);

            for (int l_X = 0; l_X < l_Tex.width; l_X++)
            {
                for (int l_Y = 0; l_Y < l_Tex.height; l_Y++)
                {
                    l_Tex.SetPixel(l_X, l_Y, Color.white);
                }
            }
            Texture2D l_NewTex = await Utils.TextureUtils.CreateRoundedTexture(/*await Utils.TextureUtils.Gradient(l_Tex, new Color(1, 1, 1, 0.7f), new Color(1f, 1f, 1f, 1), p_UseAlpha: true)*/l_Tex, 10);
            Sprite l_Sprite = Sprite.Create(l_NewTex, new Rect(0, 0, l_Tex.width, l_Tex.height), new Vector2(0, 0), 1000, 0, SpriteMeshType.FullRect);
            return l_Sprite;
        }
    }
}
