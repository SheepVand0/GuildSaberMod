using BeatSaberMarkupLanguage.Attributes;
using CSProfile.Configuration;

namespace CSProfile.UI.Settings;

public class SettingTabViewController
{

    [UIValue("ShowCardInMenu")]
    protected bool ShowCardInMenu
    {
        get => PluginConfig.Instance.ShowCardInMenu;
        set
        {
            PluginConfig.Instance.ShowCardInMenu = value;
            Plugin.PlayerCard.UpdateCardVisibility();
        }
    }

    [UIValue("ShowCardInGame")]
    protected bool ShowCardInGame
    {
        get => PluginConfig.Instance.ShowCardIngame;
        set => PluginConfig.Instance.ShowCardIngame = value;
    }

    [UIValue("ShowCardHandle")]
    protected bool ShowCardHandle
    {
        get => PluginConfig.Instance.CardHandleVisible;
        set
        {
            PluginConfig.Instance.CardHandleVisible = value;
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
            Plugin.PlayerCard.CardViewController.UpdateLevelsDetails();
        }
    }

    [UIValue("ShowPlayTime")]
    protected bool ShowPlayTime
    {
        get => PluginConfig.Instance.ShowPlayTime;
        set => PluginConfig.Instance.ShowPlayTime = value;
    }

    [UIAction("RefreshCard")]
    protected void RefreshCard()
    {
        Plugin.DestroyCard();
        Plugin.CreateCard();
    }

    [UIAction("#post-parse")]
    internal void PostParse()
    {
        // Code to run after BSML finishes
    }
}
