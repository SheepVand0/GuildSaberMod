using BeatSaberMarkupLanguage.Attributes;
using CSProfile.Configuration;

namespace CSProfile.UI.Settings;

public class SettingTabViewController
{

    [UIValue("ShowCard")]
    protected bool ShowCard
    {
        get => PluginConfig.Instance.ShowCard;
        set
        {
            PluginConfig.Instance.ShowCard = value;
            Plugin.PlayerCard.UpdateCardVisibility();
        }
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
