using BeatSaberMarkupLanguage.Attributes;
using GuildSaberProfile.Configuration;
using TMPro;
using GuildSaberProfile.UI.Card;

namespace GuildSaberProfile.UI.Settings;

public class SettingTabViewController
{
    #region Components
    //[UIComponent("GuildList")] DropdownWithTableView m_GuildListDropDown = null;
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
            if(Plugin.PlayerCard != null)
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
    public void RefreshCard()
    {
        Plugin.DestroyCard();
        Plugin.CreateCard();
    }

    [UIAction("ResetPosMenu")]
    private void ResetPosMenu()
    {
        PlayerCard_UI.ResetMenuCardPosition();
    }

    [UIAction("ResetPosGame")]
    private void ResetPosGame() {
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
