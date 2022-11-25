using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.ViewControllers;
using GuildSaber.API;
using GuildSaber.BSPModule;
using GuildSaber.Configuration;
using GuildSaber.Logger;
using GuildSaber.UI.Leaderboard.Components;
using GuildSaber.Utils;
using HMUI;
using IPA.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GuildSaber.UI.Leaderboard
{
    [HotReload(RelativePathToLayout = @"LeaderboardPanel.bsml")]
    [ViewDefinition("GuildSaber.UI.GuildSaber.View.LeaderboardPanel.bsml")]
    internal class GuildSaberLeaderboardPanel : BSMLAutomaticViewController
    {
        public static GuildSaberLeaderboardPanel PanelInstance;

        [UIComponent("PlayerErrorTxt")] private readonly TextMeshProUGUI m_ErrorText = null;
        [UIComponent("Elems")] private readonly HorizontalLayoutGroup m_ElemsLayout = null;
        [UIComponent("LoadingGrid")] private readonly GridLayoutGroup m_LoadingLayout = null;
        [UIComponent("PlayerName")] private readonly TextMeshProUGUI m_PlayerName = null;
        [UIComponent("GSImage")] private readonly ImageView m_GSImage = null;
        [UIComponent("BgHorizontal")] private readonly VerticalLayoutGroup m_BackgroundLayout = null;
        [UIComponent("GuildSelector")] private readonly DropDownListSetting m_GuildSelector = null;
        [UIComponent("NameLayout")] private readonly HorizontalLayoutGroup m_NameLayout = null;

        private PlayerAvatar m_PlayerAvatar = null;
        public PointsType m_PointsType = null;

        [UIValue("LeaderGuilds")] private readonly List<object> m_AvailableGuilds = new()
        {
            GSConfig.Instance.SelectedGuild
        };
        [UIValue("LeaderboardGuild")] private string m_DropdownSelectedGuild = GuildSaberUtils.GetGuildFromId(GSConfig.Instance.SelectedGuild).Name;
        public int m_SelectedGuild = GSConfig.Instance.SelectedGuild;

        public ApiPlayerData m_PlayerData = default(ApiPlayerData);
        public bool m_IsFirstActivation = true;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Post parse
        /// </summary>
        [UIAction("#post-parse")] private void PostParse()
        {
            PanelInstance = this;
            Reload(ReloadMode.FromCurrent, true, true);
            LeaderboardHeaderManager.CreateUpdateView();

            SceneManager.sceneLoaded += (p_Scene, p_SceneMode) =>
            {
                if (p_Scene.name.ToLower().Contains("menu"))
                    PanelInstance = this;
            };
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            GSLogger.Instance.Log("Trying to deactivate panel", IPA.Logging.Logger.LogLevel.InfoUp);
        }
        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Set header colors from current guild
        /// </summary>
        internal async void SetColors()
        {
            if (GuildSaberModule.ModState == GuildSaberModule.EModState.APIError)
            {
                LeaderboardHeaderManager.SetColors(Color.red, Color.clear);
                return;
            }

            await WaitUtils.Wait(() => m_SelectedGuild == GuildSaberModule.LeaderboardSelectedGuild.ID, 100);

            VertexGradient l_Gradient = GuildSaberModule.LeaderboardSelectedGuild.Color.ToUnityColor().GenerateGradient(0.25f, 1f);
            VertexGradient l_Gradient1 = GuildSaberModule.LeaderboardSelectedGuild.Color.ToUnityColor().GenerateGradient(0.2f, 1f);
            LeaderboardHeaderManager.SetColors(l_Gradient.topLeft, l_Gradient.bottomLeft);
            m_GSImage.gradient = true;
            m_GSImage.color0 = l_Gradient1.topLeft;
            m_GSImage.color1 = l_Gradient1.bottomLeft;
        }

        /// <summary>
        /// On Guild selected
        /// </summary>
        /// <param name="p_Selected"></param>
        [UIAction("OnGuildSelected")]
        private void OnGuildSelected(string p_Selected)
        {
            GuildData l_Guild = GuildApi.GetGuildFromName(p_Selected);
            m_SelectedGuild = l_Guild.ID;
            GuildSaberModule.LeaderboardSelectedGuild = l_Guild;
            Reload(ReloadMode.FromApi, true, true);
        }

        /// <summary>
        /// Reload panel
        /// </summary>
        /// <param name="p_ReloadMode">Get the player info from Card or Api ?</param>
        /// <param name="p_SetLoadingModeBeforeGettingData">Set leaderboard to loading mode before getting</param>
        /// <param name="p_ReloadStyle">Reload shaders</param>
        public async void Reload(ReloadMode p_ReloadMode, bool p_SetLoadingModeBeforeGettingData, bool p_ReloadStyle)
        {
            GSLogger.Instance.Log("Reloading", IPA.Logging.Logger.LogLevel.InfoUp);

            //await WaitUtils.Wait(() => gameObject.activeInHierarchy, 10, p_CodeLine: 105);

            PanelInstance = this;

            if (GuildSaberModule.IsStateError())
            {
                SetLeaderboardPanelViewMode(LeaderboardPanelViewMode.Error);
                return;
            }

            if (p_SetLoadingModeBeforeGettingData)
                SetLeaderboardPanelViewMode(LeaderboardPanelViewMode.Loading);

            if (m_IsFirstActivation)
                m_SelectedGuild = GSConfig.Instance.SelectedGuild;
            await Task.Run(delegate { GuildSaberModule.LeaderboardSelectedGuild = GuildSaberUtils.GetGuildFromId(m_SelectedGuild); });
            ///-----------------------------------------Panel Style-----------------------------------------
            switch (p_ReloadMode)
            {
                case ReloadMode.FromCurrent:
                    if (m_SelectedGuild == GSConfig.Instance.SelectedGuild) m_PlayerData = GuildApi.GetPlayerDataFromCurrent();
                    else
                    {
                        Reload(ReloadMode.FromApi, true, true);
                        return;
                    }
                    break;
                case ReloadMode.FromApi:
                    m_PlayerData = await GuildApi.GetPlayerInfoFromAPI(false, m_SelectedGuild, true);
                    break;
            }

            if (m_PlayerAvatar != null) m_PlayerAvatar.UpdateShader(m_PlayerData.Color.ToUnityColor32());

            if (m_PlayerData.Equals(default(ApiPlayerData)))
            {
                SetLeaderboardPanelViewMode(LeaderboardPanelViewMode.Error);
                if (p_ReloadMode == ReloadMode.FromCurrent) Reload(ReloadMode.FromApi, true, true);
                else return;
            }

            m_PlayerName.text = GuildSaberUtils.GetPlayerNameToFit(m_PlayerData.Name, 12);
            SetLeaderboardGuildsChoices(GuildSaberModule.AvailableGuilds);

            if (m_IsFirstActivation)
            {
                m_PointsType = CustomUIComponent.CreateItem<PointsType>(m_NameLayout.transform, true, true);
                m_PlayerAvatar = CustomUIComponent.CreateItem<PlayerAvatar>(m_ElemsLayout.transform, true, true);
                m_IsFirstActivation = false;
            }

            if (p_ReloadStyle)
            {
                ImageView l_BackgroundView = m_BackgroundLayout.GetComponent<ImageView>();
                l_BackgroundView.SetField("_skew", 0.0f);

                ///-----------------------------------------Croping Icon to fit to panel-----------------------------------------

                Texture2D l_IconTexture = null;
                try
                {
                    l_IconTexture = await GuildSaberUtils.GetImage(GuildSaberModule.LeaderboardSelectedGuild.Logo);
                }
                catch (Exception l_E)
                {
                    GSLogger.Instance.Error(l_E, nameof(GuildSaberLeaderboardPanel), nameof(Reload));
                    l_IconTexture = Utilities.FindTextureInAssembly("GuildSaber.Resources.GuildSaberLogoOrange.png");
                }

                await WaitUtils.Wait(() => l_IconTexture != null, 10);

                Color[] l_Texture = l_IconTexture.GetPixels(0, (int)(l_IconTexture.height / 2.25f), l_IconTexture.width, (int)(l_IconTexture.height / 4.5f));

                Texture2D l_ResultTexture = new(l_IconTexture.width, (int)(l_IconTexture.height / 4.5f - (l_IconTexture.width / l_IconTexture.height) - 1));
                l_ResultTexture.SetPixels(l_Texture);
                l_ResultTexture.Apply();

                if (l_BackgroundView != null)
                {
                    l_BackgroundView.overrideSprite = Sprite.Create(l_ResultTexture, new Rect(0, 0, l_ResultTexture.width, l_ResultTexture.height), new Vector2(0, 0));

                    ///----------------------------------------------------------------------------------
                    l_BackgroundView.color = new(0.7f, 0.7f, 0.7f, 0.9f);
                    l_BackgroundView.color0 = new(0.7f, 0.7f, 0.7f, 0.9f);
                    l_BackgroundView.color1 = new(0.7f, 0.7f, 0.7f, 0.1f);
                    l_BackgroundView.SetField("_gradientDirection", ImageView.GradientDirection.Horizontal);
                    l_BackgroundView.SetField("_flipGradientColors", false);
                }
            }

            Events.Instance.SelectGuild(m_SelectedGuild);

            SetLeaderboardPanelViewMode(LeaderboardPanelViewMode.Normal);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Set panel mode
        /// </summary>
        /// <param name="p_ViewMode">Mode</param>
        public void SetLeaderboardPanelViewMode(LeaderboardPanelViewMode p_ViewMode)
        {
            if (m_ElemsLayout == null || m_ErrorText == null || m_LoadingLayout == null) return;
            m_ElemsLayout.gameObject.SetActive(p_ViewMode == LeaderboardPanelViewMode.Normal);
            m_ErrorText.gameObject.SetActive(p_ViewMode == LeaderboardPanelViewMode.Error);
            m_LoadingLayout.gameObject.SetActive(p_ViewMode == LeaderboardPanelViewMode.Loading);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Set dropdown guilds
        /// </summary>
        /// <param name="p_Guilds">GuildData list</param>
        private void SetLeaderboardGuildsChoices(List<GuildData> p_Guilds)
        {
            m_AvailableGuilds.Clear();
            for (int l_i = 0; l_i < p_Guilds.Count; l_i++)
            {
                GuildData l_Current = p_Guilds[l_i];
                m_AvailableGuilds.Add(l_Current.Name);
                if (l_Current.ID == m_SelectedGuild)
                {
                    m_GuildSelector.Value = l_Current.Name;
                    m_GuildSelector.ApplyValue();
                }
            }
            m_GuildSelector.values = m_AvailableGuilds;
            m_GuildSelector.UpdateChoices();
        }
    }

    /// <summary>
    /// Panel View Mode
    /// </summary>
    public enum LeaderboardPanelViewMode
    {
        Normal,
        Loading,
        Error
    }

    /// <summary>
    /// Panel reload mode
    /// </summary>
    public enum ReloadMode
    {
        FromCurrent,
        FromApi
    }
}
