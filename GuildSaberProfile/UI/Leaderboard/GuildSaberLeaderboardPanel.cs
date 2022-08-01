﻿using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Components.Settings;
using TMPro;
using UnityEngine.UI;
using HMUI;
using IPA.Utilities;
using UnityEngine;
using System.Collections.Generic;
using GuildSaberProfile.Configuration;
using GuildSaberProfile.UI.GuildSaber.Components;
using Newtonsoft.Json;

namespace GuildSaberProfile.UI.GuildSaber.Leaderboard
{
    [HotReload(RelativePathToLayout = @"LeaderboardPanel.bsml")]
    [ViewDefinition("GuildSaberProfile.UI.GuildSaber.View.LeaderboardPanel.bsml")]
    public class GuildSaberLeaderboardPanel : BSMLAutomaticViewController
    {
        #region Components
        [UIComponent("PlayerErrorTxt")] private readonly TextMeshProUGUI m_ErrorText = null;
        [UIComponent("Elems")] private readonly HorizontalLayoutGroup m_ElemsLayout = null;
        [UIComponent("LoadingGrid")] private readonly GridLayoutGroup m_LoadingLayout = null;
        [UIComponent("PlayerName")] private readonly TextMeshProUGUI m_PlayerName = null;
        [UIComponent("GSImage")] private readonly Image m_GSImage = null;
        [UIComponent("BgHorizontal")] private readonly VerticalLayoutGroup m_BackgroundLayout = null;
        [UIComponent("GuildSelector")] private readonly DropDownListSetting m_GuildSelector = null;
        [UIComponent("AvatarImageGrid")] private readonly GridLayoutGroup m_AvatarPlayerGrid = null;
        private PlayerAvatar m_PlayerAvatar = null;
        #endregion

        [UIValue("LeaderGuilds")] private List<object> m_AvailablesGuilds = new() { Plugin.NOT_DEFINED };

        protected PlayerGuildsInfo m_PlayerGuildsInfo = new PlayerGuildsInfo();

        public bool m_IsFirtActivation = true;
        [UIValue("LeaderboardGuild")] public string m_SelectedGuild
        { get => PluginConfig.Instance.SelectedGuild; set { } }

        #region Actions
        [UIAction("OnGuildSelected")]
        private void OnGuildSelected(string p_Selected)
        {
            m_SelectedGuild = p_Selected;
            Reload(ReloadMode.FromApi, true, false);
        }
        #endregion

        #region Functions
        public void Reload(ReloadMode p_ReloadMode, bool p_SetLoadingModeBeforeGettingData, bool p_ReloadStyle)
        {
            if (p_SetLoadingModeBeforeGettingData)
                SetLeaderboardPanelViewMode(LeaderboardPanelViewMode.Loading);

            //-----------------------------------------Panel Style-----------------------------------------
            switch (p_ReloadMode) {
                case ReloadMode.FromCurrent:
                    m_PlayerGuildsInfo = Plugin.GetPlayerInfoFromCurrent();
                    break;
                case ReloadMode.FromApi:
                    m_PlayerGuildsInfo = Plugin.GetPlayerInfoFromAPI();
                    break;
                default: return;
            }

            if (string.IsNullOrEmpty(m_PlayerGuildsInfo.m_ReturnPlayer.Name))
            {
                SetLeaderboardPanelViewMode(LeaderboardPanelViewMode.Error);
                if (p_ReloadMode == ReloadMode.FromCurrent)
                    Reload(ReloadMode.FromApi, true, true);
                else
                    return;
            }

            m_PlayerName.text = m_PlayerGuildsInfo.m_ReturnPlayer.Name;
            SetLeaderboardGuildsChoices(m_PlayerGuildsInfo.m_AvailableGuilds);
            if (m_PlayerAvatar == null)
                m_PlayerAvatar = CustomUIComponent.CreateItem<PlayerAvatar>(m_AvatarPlayerGrid.transform, true, true);

            if (p_ReloadStyle)
            {
                ImageView l_BackgroundView = m_BackgroundLayout.GetComponent<ImageView>();
                l_BackgroundView.SetField("_skew", 0.0f);
                //-----------------------------------------Croping Icon to fit to panel-----------------------------------------
                Texture2D l_IconTexture = Utilities.FindTextureInAssembly("GuildSaberProfile.Resources.BSCCIconBlue.png");
                Color[] l_Texture = l_IconTexture.GetPixels(0, (int)(l_IconTexture.height / 2.25f), l_IconTexture.width, (int)(l_IconTexture.height / 4.5f));
                Texture2D l_ResultTexture = new(l_IconTexture.width, (int)(l_IconTexture.height / 4.5f - (l_IconTexture.width / l_IconTexture.height) - 1));
                l_ResultTexture.SetPixels(l_Texture);
                l_ResultTexture.Apply();
                l_BackgroundView.overrideSprite = Sprite.Create(l_ResultTexture, new Rect(0, 0, l_ResultTexture.width, l_ResultTexture.height), new Vector2(0, 0));
                //----------------------------------------------------------------------------------
                l_BackgroundView.color = new(0.7f, 0.7f, 0.7f, 0.9f);
                l_BackgroundView.color0 = new(0.7f, 0.7f, 0.7f, 0.9f);
                l_BackgroundView.color1 = new(0.7f, 0.7f, 0.7f, 0.4f);
                l_BackgroundView.SetField("_gradientDirection", ImageView.GradientDirection.Horizontal);
                l_BackgroundView.SetField("_flipGradientColors", false);
            }

            SetLeaderboardPanelViewMode(LeaderboardPanelViewMode.Normal);
        }

        public void SetLeaderboardPanelViewMode(LeaderboardPanelViewMode p_ViewMode)
        {
            switch (p_ViewMode)
            {
                case LeaderboardPanelViewMode.Normal:
                    m_ElemsLayout.gameObject.SetActive(true);
                    m_ErrorText.gameObject.SetActive(false);
                    m_LoadingLayout.gameObject.SetActive(false);
                    break;
                case LeaderboardPanelViewMode.Loading:
                    m_ElemsLayout.gameObject.SetActive(false);
                    m_ErrorText.gameObject.SetActive(false);
                    m_LoadingLayout.gameObject.SetActive(true);
                    break;
                case LeaderboardPanelViewMode.Error:
                    m_ElemsLayout.gameObject.SetActive(false);
                    m_ErrorText.gameObject.SetActive(true);
                    m_LoadingLayout.gameObject.SetActive(false);
                    break;
                default: return;
            }
        }

        public void SetLeaderboardGuildsChoices(List<string> l_Guilds)
        {
            m_AvailablesGuilds.Clear();
            foreach (string l_Current in l_Guilds)
            {
                m_AvailablesGuilds.Add(l_Current);
            }
            m_GuildSelector.values = m_AvailablesGuilds;
            m_GuildSelector.UpdateChoices();
        }
        #endregion
    }

    public enum LeaderboardPanelViewMode
    {
        Normal,
        Loading,
        Error
    }

    public enum ReloadMode
    {
        FromCurrent,
        FromApi
    }
}
