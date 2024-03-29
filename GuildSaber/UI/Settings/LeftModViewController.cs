﻿using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberPlus.SDK.UI;
using GuildSaber.BSPModule;
using GuildSaber.Configuration;
using GuildSaber.UI.Card;
using GuildSaber.Utils;
using TMPro;
using UnityEngine;

namespace GuildSaber.UI.GuildSaber;

public class LeftModViewController : ViewController<LeftModViewController>
{

    protected override string GetViewContentDescription()
    {
        return Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "GuildSaber.UI.GuildSaber.View.LeftModViewController.bsml");
    }

    [UIComponent("ErrorText")] private readonly TextMeshProUGUI m_ErrorText = null;
    [UIComponent("ErrorText2")] private readonly TextMeshProUGUI m_ErrorText2 = null;
    [UIObject("BG")] GameObject m_BG = null;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    [UIValue("ShowSettingsModal")]
    public bool ShowSettingsModal
    {
        get => GSConfig.Instance.ShowSettingsModal;
        set => GSConfig.Instance.ShowSettingsModal = value;
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    protected override void OnViewCreation()
    {
        Backgroundable.SetOpacity(m_BG, 0.5f);
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public void ShowError(bool p_Visible)
    {
        m_ErrorText.gameObject.SetActive(p_Visible);
        m_ErrorText2.gameObject.SetActive(p_Visible);
        m_ErrorText.text = "Error during getting data from " + GuildSaberModule.CardSelectedGuild.Name;
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    [UIValue("ShowCardInMenu")]
    protected bool ShowCardInMenu
    {
        get => GSConfig.Instance.ShowCardInMenu;
        set
        {
            GSConfig.Instance.ShowCardInMenu = value;
            GSConfig.Instance.Save();
            if (PlayerCardUI.m_Instance != null)
                PlayerCardUI.m_Instance.UpdateCardVisibility();
        }
    }

    [UIValue("ShowCardInGame")]
    protected bool ShowCardInGame
    {
        get => GSConfig.Instance.ShowCardInGame;
        set { GSConfig.Instance.ShowCardInGame = value; GSConfig.Instance.Save(); }
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    [UIAction("RefreshCard")]
    private async void RefreshCard()
    {
        if (!PlayerCardUI.GetIsCardActive()) return;

        if (PlayerCardUI.m_Instance == null)
            await PlayerCardUI.CreateCard();
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
}
