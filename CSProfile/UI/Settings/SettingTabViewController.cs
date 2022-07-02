using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using CSProfile.Configuration;
using System.Threading.Tasks;

namespace CSProfile.UI.Settings
{
    public class SettingTabViewController
    {

        [UIValue("showCard")]
        protected bool showCard
        {
            get => PluginConfig.Instance.ShowCard;
            set { PluginConfig.Instance.ShowCard = value; Plugin.PlayerCard.UpdateCardVisibility(); }
        }

        [UIValue("showCardHandle")]
        protected bool showCardHandle
        {
            get => PluginConfig.Instance.CardHandleVisible;
            set { PluginConfig.Instance.CardHandleVisible = value; Plugin.PlayerCard.UpdateCardHandleVisibility(); }
        }

        [UIValue("detailLevels")]
        protected bool ShowDetailedLevels
        {
            get => PluginConfig.Instance.ShowDetaislLevels;
            set { PluginConfig.Instance.ShowDetaislLevels = value; Plugin.PlayerCard._cardViewController.UpdateLevelsDetails(); }
        }

        [UIValue("showPlayTime")]
        protected bool showPlayTime
        {
            get => PluginConfig.Instance.ShowPlayTime;
            set => PluginConfig.Instance.ShowPlayTime = value;
        }

        [UIAction("refreshCard")]
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
}
