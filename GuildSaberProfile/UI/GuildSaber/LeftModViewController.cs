using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberMarkupLanguage.Attributes;
using GuildSaberProfile.Configuration;
using GuildSaberProfile.UI.Card;
using TMPro;
using System.Threading.Tasks;

namespace GuildSaberProfile.UI.GuildSaber
{
    [HotReload(RelativePathToLayout = @"ModViewController.bsml")]
    [ViewDefinition("GuildSaberProfile.UI.GuildSaber.View.LeftModViewController.bsml")]
    class LeftModViewController : BSMLAutomaticViewController
    {

        [UIValue("ShowSettingsModal")]
        public bool ShowSettingsModal
        {
            get => PluginConfig.Instance.ShowSettingsModal;
            set => PluginConfig.Instance.ShowSettingsModal = value;
        }

        #region Components
        [UIComponent("ErrorText")] private readonly TextMeshProUGUI m_ErrorText = null;
        [UIComponent("ErrorText2")] private readonly TextMeshProUGUI m_ErrorText2 = null;
        #endregion

        #region UIValues
        [UIValue("ShowCardInMenu")]
        protected bool ShowCardInMenu
        {
            get => PluginConfig.Instance.ShowCardInMenu;
            set
            {
                PluginConfig.Instance.ShowCardInMenu = value;
                if (Plugin.PlayerCard != null)
                    Plugin.PlayerCard.UpdateCardVisibility();
            }
        }

        [UIValue("ShowCardInGame")]
        protected bool ShowCardInGame
        {
            get => PluginConfig.Instance.ShowCardInGame;
            set => PluginConfig.Instance.ShowCardInGame = value;
        }
        #endregion

        #region UIActions
        [UIAction("RefreshCard")]
        public async Task RefreshCard()
        {
            await Plugin.DestroyCard();
            Plugin.CreateCard();
        }

        [UIAction("ResetPosMenu")]
        private void ResetPosMenu()
        {
            PlayerCard_UI.ResetMenuCardPosition();
        }

        [UIAction("ResetPosGame")]
        private void ResetPosGame()
        {
            PlayerCard_UI.ResetInGameCardPosition();
        }
        #endregion

        public void ShowError(bool p_Visible)
        {
            m_ErrorText.gameObject.SetActive(p_Visible);
            m_ErrorText2.gameObject.SetActive(p_Visible);
            m_ErrorText.text = "Error during getting data from " + PluginConfig.Instance.SelectedGuild;
        }
    }
}
