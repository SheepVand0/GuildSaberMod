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
            get => PluginConfig.Instance.m_showCard;
            set { PluginConfig.Instance.m_showCard = value; Plugin.m_playerCard.UpdateCardVisibility(); }
        }

        [UIValue("showCardHandle")]
        protected bool showCardHandle
        {
            get => PluginConfig.Instance.m_cardHandleVisible;
            set { PluginConfig.Instance.m_cardHandleVisible = value; Plugin.m_playerCard.UpdateCardHandleVisibility(); }
        }

        [UIValue("detailLevels")]
        protected bool ShowDetailedLevels
        {
            get => PluginConfig.Instance.m_showDetaislLevels;
            set { PluginConfig.Instance.m_showDetaislLevels = value; Plugin.m_playerCard._cardViewController.UpdateLevelsDetails(); }
        }

        [UIValue("showPlayTime")]
        protected bool showPlayTime
        {
            get => PluginConfig.Instance.m_showPlayTime;
            set => PluginConfig.Instance.m_showPlayTime = value;
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
