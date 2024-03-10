using CP_SDK.UI;
using CP_SDK.XUI;
using GuildSaber.Configuration;
using GuildSaber.UI.Card;
using UnityEngine;

namespace GuildSaber.UI.BeatSaberPlusSettings;

internal class Settings : ViewController<Settings>
{
    private XUIToggle m_BoolCardEnabled = null;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    protected override void OnViewCreation()
    {
        Templates.FullRectLayoutMainView(
            Templates.TitleBar("Guild Saber"),

            XUIText.Make("Enable Card:")
                .SetColor(Color.yellow)
                .SetAlign(TMPro.TextAlignmentOptions.Center),
            XUIToggle.Make()
                .SetValue(GSConfig.Instance.CardEnabled)
                .Bind(ref m_BoolCardEnabled),

            XUIVSpacer.Make(35f)
        ).BuildUI(transform);

        m_BoolCardEnabled.OnValueChanged(OnBoolCardChanged);
    }

    private void OnBoolCardChanged(bool p_Value)
    {
        GSConfig.Instance.CardEnabled = m_BoolCardEnabled.Element.GetValue();
        if (PlayerCardUI.m_Instance != null) PlayerCardUI.SetCardActive(GSConfig.Instance.CardEnabled);
        GSConfig.Instance.Save();
    }
}
