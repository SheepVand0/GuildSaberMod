using BeatSaberMarkupLanguage.Attributes;
using GuildSaberProfile.Configuration;
using System.Collections.Generic;
using HMUI;
using TMPro;

namespace GuildSaberProfile.UI.Settings;

public class SettingTabViewController
{
    #region Components
    //[UIComponent("GuildList")] DropdownWithTableView m_GuildListDropDown = null;
    [UIComponent("ErrorText")] private readonly TextMeshProUGUI m_ErrorText = null;
    [UIComponent("ErrorText2")] private readonly TextMeshProUGUI m_ErrorText2 = null;
    #endregion

    #region UIValues
    [UIValue("AvailableGuilds")]
    private List<object> m_AvailableGuilds = new List<object>() { "CS", "BSCC" };

    [UIValue("SelectedGuild")]
    protected string SelectedGuild {
        get => PluginConfig.Instance.SelectedGuild;
        set => PluginConfig.Instance.SelectedGuild = value;
    }

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

    [UIValue("ShowCardHandle")]
    protected bool ShowCardHandle
    {
        get => PluginConfig.Instance.CardHandleVisible;
        set
        {
            PluginConfig.Instance.CardHandleVisible = value;
            if (Plugin.PlayerCard != null)
                Plugin.PlayerCard.UpdateCardHandleVisibility();
        }
    }

    [UIValue("DetailLevels")]
    protected bool ShowDetailedLevels
    {
        get => PluginConfig.Instance.ShowDetailsLevels;
        set
        {
            PluginConfig.Instance.ShowDetailsLevels = value;
            if (Plugin.PlayerCard != null)
                Plugin.PlayerCard.CardViewController.UpdateLevelsDetails();
        }
    }

    [UIValue("ShowPlayTime")]
    protected bool ShowPlayTime
    {
        get => PluginConfig.Instance.ShowPlayTime;
        set => PluginConfig.Instance.ShowPlayTime = value;
    }
    #endregion

    #region UIActions
    [UIAction("RefreshCard")]
    protected void RefreshCard()
    {
        Plugin.DestroyCard();
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
        Plugin.PlayerCard.ResetMenuCardPosition();
    }

    [UIAction("ResetPosGame")]
    private void ResetPosInGame()
    {
        Plugin.PlayerCard.ResetInGameCardPosition();
    }
#endregion

    public void ShowError(bool p_Visible)
    {
        m_ErrorText.gameObject.SetActive(p_Visible);
        m_ErrorText2.gameObject.SetActive(p_Visible);
        m_ErrorText.text = "Error during getting data from " + PluginConfig.Instance.SelectedGuild;
    }
}
