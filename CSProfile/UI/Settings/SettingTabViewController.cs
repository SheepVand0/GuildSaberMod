using BeatSaberMarkupLanguage.Attributes;
using CSProfile.Configuration;
using CSProfile.Time;

namespace CSProfile.UI.Settings
{
    public class SettingTabViewController
    {

        [UIValue("ShowCard")]
        protected bool showCard
        {
            get => PluginConfig.Instance.ShowCard;
            set { PluginConfig.Instance.ShowCard = value; Plugin.PlayerCard.UpdateCardVisibility(); }
        }

        [UIValue("ShowCardHandle")]
        protected bool showCardHandle
        {
            get => PluginConfig.Instance.CardHandleVisible;
            set { PluginConfig.Instance.CardHandleVisible = value; Plugin.PlayerCard.UpdateCardHandleVisibility(); }
        }

        [UIValue("DetailLevels")]
        protected bool ShowDetailedLevels
        {
            get => PluginConfig.Instance.ShowDetailsLevels;
            set { PluginConfig.Instance.ShowDetailsLevels = value; Plugin.PlayerCard._CardViewController.UpdateLevelsDetails(); }
        }

        [UIValue("ShowPlayTime")]
        protected bool showPlayTime
        {
            get => PluginConfig.Instance.ShowPlayTime;
            set => PluginConfig.Instance.ShowPlayTime = value;
        }

        [UIAction("RefreshCard")]
        protected void RefreshCard()
        {
            OptimizedDateTime l_CurrentTime = Plugin.PlayerCard._CardViewController.GetComponent<TimeManager>().m_Time;
            Plugin.DestroyCard();
            Plugin.CreateCard();
            Plugin.PlayerCard._CardViewController.GetComponent<TimeManager>().m_Time = l_CurrentTime;
        }

        [UIAction("#post-parse")]
        internal void PostParse()
        {
            // Code to run after BSML finishes
        }
    }
}
