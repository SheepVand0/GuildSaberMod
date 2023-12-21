using BeatSaberMarkupLanguage;
using CP_SDK.UI.Components;
using CP_SDK.XUI;
using GuildSaber.API;
using GuildSaber.UI.CustomLevelSelectionMenu.FlowCoordinators;
using GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers;
using GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard.Components;
using GuildSaber.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
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

        int m_GuildId;

        public void SetCategoryData(ApiCategory p_Category)
        {
            m_CurrentCategory = p_Category;

            if (m_CurrentCategory.Name == null || m_CurrentCategory.Name == string.Empty)
            {
                SetText("Default");
            } else
            {
                SetText(Utils.GuildSaberUtils.GetPlayerNameToFit(m_CurrentCategory.Name, 8));
            }
        }

        public static CategoryButton Make(ApiCategory p_Category)
        {
            var l_But = new CategoryButton("CategoryButton");
            l_But.SetCategoryData(p_Category);
            return l_But;
        }

        public void SetGuildId(int p_GuildId)
        {
            m_GuildId = p_GuildId;
        }

        private async void OnButtonReady(CSecondaryButton p_Button)
        {
            SetWidth(20);
            SetHeight(20);

            if (BorderTexture == null)
            {
                var l_BorderTexture = AssemblyUtils.LoadTextureFromAssembly("GuildSaber.Resources.BorderSquare.png");
                BorderTexture = await TextureUtils.CreateRoundedTexture(l_BorderTexture, 20);
            }

            p_Button.SetBackgroundSprite(Sprite.Create(BorderTexture, new Rect(0, 0, BorderTexture.width, BorderTexture.height), new Vector2()));
            p_Button.SetBackgroundColor(new Color(1, 1, 1, 1));
            GSText.PatchText(p_Button.gameObject.GetComponentInChildren<TextMeshProUGUI>());
        }

        private void ShowCategoryLevels()
        {
            if (LevelsFlowCoordinator.Instance == null)
                LevelsFlowCoordinator.Instance = BeatSaberUI.CreateFlowCoordinator<LevelsFlowCoordinator>();

            CustomLevelSelectionMenuReferences.SelectedCategory = m_CurrentCategory;
            LevelSelectionViewController.Instance.SetLevels(m_GuildId, m_CurrentCategory);
            CategorySelectionFlowCoordinator.Instance.Dismiss();
        }
    }
}
