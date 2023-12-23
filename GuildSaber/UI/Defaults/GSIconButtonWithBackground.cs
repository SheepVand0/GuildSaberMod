using CP_SDK.UI.Components;
using CP_SDK.XUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GuildSaber.UI.Defaults
{
    internal class GSIconButtonWithBackground : XUIVLayout
    {
        protected GSIconButtonWithBackground(string p_Name, params IXUIElement[] p_Childs) : base(p_Name, p_Childs)
        {
            OnReady(OnButtonCreation);
        }

        public static GSIconButtonWithBackground Make()
        {
            return new GSIconButtonWithBackground("GuildSaberIconButtonWithBackgroundButton");
        }

        //////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////

        private void OnButtonCreation(CHOrVLayout p_Layout)
        {
            XUIIconButton.Make().Bind(ref Button).BuildUI(p_Layout.transform);
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

        public GSIconButtonWithBackground SetInteractable(bool p_Value)
        { 
            OnReady(x =>
            {
                Button.SetInteractable(p_Value);
            });
            return this;
        }

        public GSIconButtonWithBackground OnClick(Action p_Value, bool p_Add = true)
        {
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

        public GSIconButtonWithBackground SetBackground(Sprite p_Sprite)
        {
            SetBackground(true);
            SetBackgroundSprite(p_Sprite);
            return this;
        }

        public GSIconButtonWithBackground SetBackground(Texture2D p_Texture)
        {
            SetBackground(Sprite.Create(p_Texture, new Rect(0, 0, p_Texture.width, p_Texture.height), new Vector2()));
            return this;
        }

        public new GSIconButtonWithBackground SetWidth(float p_Width)
        {
            OnReady(x =>
            {
                base.SetWidth(p_Width);

            });
            return this;
        }

        public new GSIconButtonWithBackground SetHeight(float p_Height)
        {
            OnReady(x =>
            {
                base.SetHeight(p_Height);
                base.SetMinHeight(p_Height);
                GetIconButton().SetHeight(p_Height);
                GetIconButton().SetWidth(p_Height);
            });
            return this;
        }
    }
}
