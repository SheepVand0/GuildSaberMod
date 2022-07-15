using System.Reflection;
using System.Collections.Generic;
using GuildSaberProfile.UI.Card;
using GuildSaberProfile.Configuration;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using UnityEngine;
using System.Threading.Tasks;

namespace GuildSaberProfile.UI.Card.Settings
{
    public class SettingsModal
    {
        public BSMLParserParams m_ParserParams = null;

        public GameObject m_Parent = null;

        public SettingsModal(GameObject p_Parent)
        {
            m_Parent = p_Parent;
            m_ParserParams = BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), $"GuildSaberProfile.UI.Card.Settings.SettingsModal.bsml"), m_Parent, this);
        }

        public void ShowModal()
        {
            m_ParserParams.EmitEvent("ShowSettings");
        }

        [UIComponent("ToggleShowHandle")] private readonly ToggleSetting m_ToggleShowHandle = null;

        #region UIValues
        [UIValue("AvailableGuilds")]
        public List<object> m_AvailableGuilds = new List<object>() { "CS", "BSCC" };

        [UIValue("SelectedGuild")]
        protected string SelectedGuild
        {
            get => PluginConfig.Instance.SelectedGuild;
            set => PluginConfig.Instance.SelectedGuild = value;
        }

        [UIValue("ShowCardHandle")]
        protected bool ShowCardHandle
        {
            get => PluginConfig.Instance.CardHandleVisible;
            set
            {
                PluginConfig.Instance.CardHandleVisible = value;
                Plugin.PlayerCard.UpdateCardHandleVisibility();
                this.UpdateToggleCardHandleVisibility();
            }
        }

        [UIValue("DetailLevels")]
        protected bool ShowDetailedLevels
        {
            get => PluginConfig.Instance.ShowDetailsLevels;
            set
            {
                PluginConfig.Instance.ShowDetailsLevels = value;
                Plugin.PlayerCard.CardViewController.UpdateLevelsDetails();
            }
        }

        [UIValue("ShowPlayTime")]
        protected bool ShowPlayTime
        {
            get => PluginConfig.Instance.ShowPlayTime;
            set => PluginConfig.Instance.ShowPlayTime = value;
        }

        public void UpdateToggleCardHandleVisibility()
        {
            if (Plugin.CurrentSceneName == "GameCore")
                m_ToggleShowHandle.gameObject.SetActive(PluginConfig.Instance.CardHandleVisible);
            else
                m_ToggleShowHandle.gameObject.SetActive(true);
        }
        #endregion

        #region UIActions
        [UIAction("RefreshCard")]
        protected async Task RefreshCard()
        {
            await Plugin.DestroyCard();
            Plugin.CreateCard();
        }

        [UIAction("UpdateCard")]
        private void UpdateCard(string p_Selected)
        {
            PluginConfig.Instance.SelectedGuild = p_Selected;
            RefreshCard();
        }

        [UIAction("ResetPosMenu")]
        private void ResetPosMenu()
        {
            PlayerCard_UI.ResetMenuCardPosition();
        }

        [UIAction("ResetPosGame")]
        private void ResetPosInGame()
        {
            PlayerCard_UI.ResetInGameCardPosition();
        }
        #endregion
    }
}
