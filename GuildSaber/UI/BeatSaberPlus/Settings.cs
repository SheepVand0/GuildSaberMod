using BeatSaberMarkupLanguage;
using BeatSaberPlus.SDK.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using BeatSaberMarkupLanguage.Attributes;
using ToggleSetting = BeatSaberMarkupLanguage.Components.Settings.ToggleSetting;
using GuildSaber.Configuration;
using BeatSaberMarkupLanguage.Parser;
using GuildSaber.UI.Card;
using BeatmapEditor3D;
using GuildSaber.UI.Leaderboard;

namespace GuildSaber.UI.BeatSaberPlusSettings
{
    internal class Settings : ViewController<Settings>
    {
        protected override string GetViewContentDescription()
        {
            return Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "GuildSaber.UI.BeatSaberPlus.View.Settings.bsml");

        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        [UIComponent("BoolLeaderboardEnabled")] ToggleSetting m_BoolLeaderboardEnabled = null;
        [UIComponent("BoolCardEnabled")] ToggleSetting m_BoolCardEnabled = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        protected override void OnViewCreation()
        {
            BSMLAction l_ToggleCardEvent = new BSMLAction(this, this.GetType().GetMethod(nameof(OnBoolCardChanged), BindingFlags.NonPublic | BindingFlags.Instance));
            BSMLAction l_ToggleLeaderboardEvent = new BSMLAction(this, this.GetType().GetMethod(nameof(OnBoolLeaderboardChanged), BindingFlags.NonPublic | BindingFlags.Instance));

            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_BoolCardEnabled, l_ToggleCardEvent, GSConfig.Instance.CardEnabled, true);
            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_BoolLeaderboardEnabled, l_ToggleLeaderboardEvent, GSConfig.Instance.LeaderboardEnabled, true);
        }

        private void OnBoolCardChanged(object p_Value)
        {
            GSConfig.Instance.CardEnabled = m_BoolCardEnabled.Value;
            if (PlayerCardUI.m_Instance != null && Events.m_EventsEnabled == true)
                PlayerCardUI.SetCardActive(GSConfig.Instance.CardEnabled);
            GSConfig.Instance.Save();
        }

        private void OnBoolLeaderboardChanged(object p_Value)
        {
            GSConfig.Instance.LeaderboardEnabled = m_BoolLeaderboardEnabled.Value;
            if (GSConfig.Instance.LeaderboardEnabled)
            {
                GuildSaberCustomLeaderboard.Instance.Initialize();
            } else
            {
                GuildSaberCustomLeaderboard.Instance.Dispose();
            }
            GSConfig.Instance.Save();
        }
    }
}
