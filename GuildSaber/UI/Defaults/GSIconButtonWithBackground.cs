using CP_SDK.UI.Components;
using CP_SDK.XUI;
using GuildSaber.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GuildSaber.UI.Defaults
{
    internal class GSIconButtonWithBackground : GSSecondaryButton
    {
        protected GSIconButtonWithBackground() : base(string.Empty)
        {
            OnReady(OnButtonCreation);
        }

        public static GSIconButtonWithBackground Make()
        {
            return new GSIconButtonWithBackground();
        }

        //////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////

        private void OnButtonCreation(CSecondaryButton p_Button)
        {
            XUIIconButton.Make().Bind(ref Button).BuildUI(p_Button.transform);
        }

        //////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////

        protected XUIIconButton Button;

        public XUIIconButton GetIconButton() => Button;

        //////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////
        
        public GSIconButtonWithBackground Bind(ref GSIconButtonWithBackground p_Val)
        {
            p_Val = this;
            return this;
        }

        public new GSIconButtonWithBackground SetInteractable(bool p_Value)
        {
            base.SetInteractable(p_Value);
            OnReady(x =>
            {
                Button.SetInteractable(p_Value);
            });
            return this;
        }

        public new GSIconButtonWithBackground OnClick(Action p_Value, bool p_Add = true)
        {
            base.OnClick(p_Value, p_Add);
            OnReady(x =>
            {
                Button.OnClick(p_Value, p_Add);
            }); 
            return this;
        }

        public GSIconButtonWithBackground SetIcon(Sprite p_Sprite)
        {
            Button.SetSprite(p_Sprite);
            return this;
        }

        public GSIconButtonWithBackground SetIcon(Texture2D p_Texture)
        {
            Button.SetSprite(Sprite.Create(p_Texture, new Rect(0, 0, p_Texture.width, p_Texture.height), new Vector2()));
            return this;
        }

        public GSIconButtonWithBackground SetWidth(float p_Width, bool p_UpdateIconButton = false)
        {
            base.SetWidth((int)p_Width);
            if (!p_UpdateIconButton) return this;
            OnReady(x =>
            {
                GetIconButton().SetWidth(p_Width * 0.5f);
                GetIconButton().SetHeight(p_Width * 0.5f);
            });
            return this;
        }

        public GSIconButtonWithBackground SetHeight(float p_Height, bool p_UpdateIconButton = false)
        {
            base.SetHeight((int)p_Height);
            if (!p_UpdateIconButton) return this;
            OnReady(x =>
            {
                GetIconButton().SetHeight(p_Height * 0.5f);
                GetIconButton().SetWidth(p_Height * 0.5f);
            });
            return this;
        }
    }
}
