using System.Collections.Generic;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using GuildSaberProfile.Configuration;
using UnityEngine;

namespace GuildSaberProfile.UI.Card.Settings;

public class SettingsModal
{

    [UIComponent("ToggleShowHandle")] private readonly ToggleSetting m_ToggleShowHandle = null;

    public GameObject m_Parent;
    public BSMLParserParams m_ParserParams;

    public SettingsModal(GameObject p_Parent)
    {
        m_Parent = p_Parent;
        m_ParserParams = BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "GuildSaberProfile.UI.Card.Settings.SettingsModal.bsml"), m_Parent, this);
    }

    public void ShowModal()
    {
        m_ParserParams.EmitEvent("ShowSettings");
    }

    #region UIValues

    [UIValue("AvailableGuilds")]
    public List<object> m_AvailableGuilds = new List<object>
        { "CS", "BSCC" };

    [UIValue("SelectedGuild")]
    protected string SelectedGuild
    {
        get => PluginConfig.Instance.SelectedGuild;
        set { }
    }

    [UIValue("ShowCardHandle")]
    protected bool ShowCardHandle
    {
        get => PluginConfig.Instance.CardHandleVisible;
        set
        {
            PluginConfig.Instance.CardHandleVisible = value;
            Plugin.PlayerCard.UpdateCardHandleVisibility();
            UpdateToggleCardHandleVisibility();
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
    protected void RefreshCard()
    {
        Plugin.m_Refresher.RefreshCard();
    }

    [UIAction("UpdateCard")]
    private void UpdateCard(string p_Selected)
    {
        PluginConfig.Instance.SelectedGuild = p_Selected;
        m_ParserParams.EmitEvent("HideSettings");
        Plugin.m_Refresher.RefreshCard();
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