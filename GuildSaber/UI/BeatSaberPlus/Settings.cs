using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberPlus.SDK.UI;
using GuildSaber.Configuration;
using GuildSaber.UI.Card;
using GuildSaber.UI.Leaderboard;
using ToggleSetting = BeatSaberMarkupLanguage.Components.Settings.ToggleSetting;

/// It's needed to make it work
// ReSharper disable once CheckNamespace
namespace GuildSaber.UI.BeatSaberPlusSettings
{
    internal class Settings : ViewController<Settings>
    {
        [UIComponent("BoolCardEnabled")] private readonly ToggleSetting m_BoolCardEnabled = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        [UIComponent("BoolLeaderboardEnabled")]
        private readonly ToggleSetting m_BoolLeaderboardEnabled = null;
        protected override string GetViewContentDescription()
        {
            return Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "GuildSaber.UI.BeatSaberPlus.View.Settings.bsml");

        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        protected override void OnViewCreation()
        {
            var l_ToggleCardEvent = new BSMLAction(this, GetType().GetMethod(nameof(OnBoolCardChanged), BindingFlags.NonPublic | BindingFlags.Instance));
            var l_ToggleLeaderboardEvent = new BSMLAction(this, GetType().GetMethod(nameof(OnBoolLeaderboardChanged), BindingFlags.NonPublic | BindingFlags.Instance));

            global::BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_BoolCardEnabled, l_ToggleCardEvent, GSConfig.Instance.CardEnabled, true);
            global::BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_BoolLeaderboardEnabled, l_ToggleLeaderboardEvent, GSConfig.Instance.LeaderboardEnabled, true);
        }

        private void OnBoolCardChanged(object p_Value)
        {
            GSConfig.Instance.CardEnabled = m_BoolCardEnabled.Value;
            if (PlayerCardUI.m_Instance != null && Events.m_EventsEnabled)
            {
                PlayerCardUI.SetCardActive(GSConfig.Instance.CardEnabled);
            }
            GSConfig.Instance.Save();
        }

        private void OnBoolLeaderboardChanged(object p_Value)
        {
            GSConfig.Instance.LeaderboardEnabled = m_BoolLeaderboardEnabled.Value;
            if (GSConfig.Instance.LeaderboardEnabled)
            {
                GuildSaberCustomLeaderboard.Instance.Initialize();
            }
            else
            {
                GuildSaberCustomLeaderboard.Instance.Dispose();
            }
            GSConfig.Instance.Save();
        }
    }
}
