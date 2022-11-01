using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using GuildSaber.API;
using GuildSaber.Configuration;
using GuildSaber.UI.Card;
using TMPro;

namespace GuildSaber.UI.GuildSaber;

[HotReload(RelativePathToLayout = @"ModViewController.bsml")]
[ViewDefinition("GuildSaber.UI.GuildSaber.View.LeftModViewController.bsml")]
public class LeftModViewController : BSMLAutomaticViewController
{

    [UIValue("ShowSettingsModal")]
    public bool ShowSettingsModal
    {
        get => GSConfig.Instance.ShowSettingsModal;
        set => GSConfig.Instance.ShowSettingsModal = value;
    }

    public void ShowError(bool p_Visible)
    {
        m_ErrorText.gameObject.SetActive(p_Visible);
        m_ErrorText2.gameObject.SetActive(p_Visible);
        m_ErrorText.text = "Error during getting data from " + BSPModule.GuildSaberModule.m_CardSelectedGuild.Name;
    }

    #region Components

    [UIComponent("ErrorText")] private readonly TextMeshProUGUI m_ErrorText = null;
    [UIComponent("ErrorText2")] private readonly TextMeshProUGUI m_ErrorText2 = null;

    #endregion

    #region UIValues

    [UIValue("ShowCardInMenu")]
    protected bool ShowCardInMenu
    {
        get => GSConfig.Instance.ShowCardInMenu;
        set
        {
            GSConfig.Instance.ShowCardInMenu = value;
            if (PlayerCardUI.m_Instance != null)
                PlayerCardUI.m_Instance.UpdateCardVisibility();
        }
    }

    [UIValue("ShowCardInGame")]
    protected bool ShowCardInGame
    {
        get => GSConfig.Instance.ShowCardInGame;
        set => GSConfig.Instance.ShowCardInGame = value;
    }

    #endregion

    #region UIActions

    [UIAction("RefreshCard")]
    private void RefreshCard()
    {
        if (!PlayerCardUI.GetIsCardActive()) return;

        if (PlayerCardUI.m_Instance == null)
            PlayerCardUI.CreateCard();
        else
            PlayerCardUI.RefreshCard(true);
    }

    [UIAction("ResetPosMenu")]
    private void ResetPosMenu()
    {
        PlayerCardUI.ResetMenuCardPosition();
    }

    [UIAction("ResetPosGame")]
    private void ResetPosGame()
    {
        PlayerCardUI.ResetInGameCardPosition();
    }

    #endregion

}
