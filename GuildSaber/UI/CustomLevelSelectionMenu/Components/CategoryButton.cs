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

namespace GuildSaber.UI.CustomLevelSelectionMenu.Components
{
    internal class CategoryButton : XUISecondaryButton
    {
        protected CategoryButton(string p_Name, Action p_OnClick = null) : base(p_Name, string.Empty, p_OnClick)
        {
            OnClick(ShowCategoryLevels);
            OnReady(OnButtonReady);
        }

        static Texture2D BorderTexture;

        ApiCategory m_CurrentCategory = default;

        public void SetCategoryData(ApiCategory p_Category)
        {
            m_CurrentCategory = p_Category;

            SetText(Utils.GuildSaberUtils.GetPlayerNameToFit(m_CurrentCategory.Name, 8));
        }

        public static CategoryButton Make(ApiCategory p_Category)
        {
            var l_But = new CategoryButton("CategoryButton");
            l_But.SetCategoryData(p_Category);
            return l_But;
        }

        private void OnButtonReady(CSecondaryButton p_Button)
        {
            SetWidth(20);
            SetHeight(20);

            if (BorderTexture == null)
            {
                BorderTexture = AssemblyUtils.LoadTextureFromAssembly("GuildSaber.Resources.BorderSquare.png");
                BorderTexture = TextureUtils.CreateRoundedTexture(BorderTexture, 20, p_PushPixels: true);
            }

            p_Button.SetBackgroundSprite(Sprite.Create(BorderTexture, new Rect(0, 0, BorderTexture.width, BorderTexture.height), new Vector2()));
            p_Button.SetBackgroundColor(new Color(1, 1, 1, 1));
        }

        private void ShowCategoryLevels()
        {

        }
    }
}
