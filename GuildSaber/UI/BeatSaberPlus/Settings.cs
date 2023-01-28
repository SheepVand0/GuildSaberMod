using BeatSaberPlus.SDK.UI;
using CP_SDK.XUI;
using GuildSaber.Configuration;
using GuildSaber.UI.Card;
using GuildSaber.UI.Leaderboard;
using UnityEngine;

namespace GuildSaber.UI.BeatSaberPlusSettings;

internal class Settings : EmptyViewController<Settings>
{
    private XUIToggle m_BoolCardEnabled = null;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    private XUIToggle m_BoolLeaderboardEnabled = null;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    protected override void OnViewCreation()
    {
        Templates.FullRectLayoutMainView(
            Templates.TitleBar("Guild Saber"),

            new XUIText("Enable Card:")
                .SetColor(Color.yellow)
                .SetAlign(TMPro.TextAlignmentOptions.Center),
            new XUIToggle()
                .SetValue(GSConfig.Instance.CardEnabled)
                .Bind(ref m_BoolCardEnabled),

            new XUIText("Enable Leaderboard:")
                .SetColor(Color.yellow)
                .SetAlign(TMPro.TextAlignmentOptions.Center),
            new XUIToggle()
                .SetValue(GSConfig.Instance.LeaderboardEnabled)
                .Bind(ref m_BoolLeaderboardEnabled),

            new XUIVSpacer(35f)
        ).BuildUI(transform);

        m_BoolCardEnabled.OnValueChanged(OnBoolCardChanged);
        m_BoolLeaderboardEnabled.OnValueChanged(OnBoolLeaderboardChanged);
    }

    private void OnBoolCardChanged(bool p_Value)
    {
        GSConfig.Instance.CardEnabled = m_BoolCardEnabled.Element.GetValue();
        if (PlayerCardUI.m_Instance != null && Events.m_EventsEnabled) PlayerCardUI.SetCardActive(GSConfig.Instance.CardEnabled);
        GSConfig.Instance.Save();
    }

    private void OnBoolLeaderboardChanged(bool p_Value)
    {
        GSConfig.Instance.LeaderboardEnabled = m_BoolLeaderboardEnabled.Element.GetValue();
        if (GSConfig.Instance.LeaderboardEnabled)
            GuildSaberCustomLeaderboard.Instance.Initialize();
        else
            GuildSaberCustomLeaderboard.Instance.Dispose();
        GSConfig.Instance.Save();
    }
}
